using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using QuickRanking.Common;
using QuickRanking.Core;
using UnityEngine;

namespace QuickRanking.Playfab {
    /// <summary>
    /// Playfabを利用する場合の実装
    /// </summary>
    public class PlayfabController : IBackendController {
        private PlayerIdManager _playerIdManager;

        public PlayfabController(PlayerIdManager playerIdManager) {
            _playerIdManager = playerIdManager;
        }

        // Playfab側ではint型でしか値を保持できないので、保存したい小数桁数分だけ10倍してから送信する
        private int EncodeScore(RankingScore score) {
            return (score.RankingSetting.OrderBy == RankingSetting.Order.Descending ? 1 : -1)
                * (int)(score.Value * Math.Pow(10, score.RankingSetting.DecimalDigits));
        }
        private RankingScore DecodeScore(RankingSetting rankingSetting, int score) {
            return new RankingScore(
                rankingSetting,
                (rankingSetting.OrderBy == RankingSetting.Order.Descending ? 1 : -1)
                    * (double)score / Math.Pow(10, rankingSetting.DecimalDigits)
            );
        }

        /// <returns>エラーメッセージ</returns>
        private async Task<string> AuthenticateAsync(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            // ユーザIDを取得
            string customId = _playerIdManager.GetPlayerId();

            // 認証
            LoginResult result = null;
            PlayFabError error = null;
            PlayFabClientAPI.LoginWithCustomID(
                new LoginWithCustomIDRequest() {
                    TitleId = PlayFabSettings.TitleId,
                    CustomId = customId,
                    CreateAccount = true,
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                },
                x => result = x,
                x => error = x
            );
            await TaskUtil.WaitUntil(() => result != null || error != null, cancellationToken);

            if(error != null) {
                return error.ErrorMessage;
            }

            return null;
        }

        private async Task<string> AuthenticateIfYetAsync(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if(PlayFabSettings.staticPlayer.IsClientLoggedIn() == false) {
                string errorMessage = await AuthenticateAsync(cancellationToken);
                return errorMessage;
            }

            return null;
        }

        public async Task<string> SendDisplayNameAsync(string displayName, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            // 認証
            await AuthenticateIfYetAsync(cancellationToken);

            UpdateUserTitleDisplayNameResult result = null;
            PlayFabError error = null;
            PlayFabClientAPI.UpdateUserTitleDisplayName(
                new UpdateUserTitleDisplayNameRequest {
                    DisplayName = displayName
                },
                x => result = x,
                x => error = x
            );
            await TaskUtil.WaitUntil(() => result != null || error != null, cancellationToken);

            if(error != null) {
                return error.ErrorMessage;
            }

            return null;
        }

        public async Task<string> SendScoreAsync(RankingScore score, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            RankingSetting rankingSetting = score.RankingSetting;

            // 認証
            await AuthenticateIfYetAsync(cancellationToken);

            // スコアの更新
            UpdatePlayerStatisticsResult result = null;
            PlayFabError error = null;
            PlayFabClientAPI.UpdatePlayerStatistics(
                new UpdatePlayerStatisticsRequest {
                    Statistics = new List<StatisticUpdate> {
                        new StatisticUpdate {
                            StatisticName = rankingSetting.RankingId,
                            Value = EncodeScore(score),
                        }
                    }
                },
                x => result = x,
                x => error = x
            );
            await TaskUtil.WaitUntil(() => result != null || error != null, cancellationToken);

            if(error != null) {
                return error.ErrorMessage;
            }

            return null;
        }

        public async Task<(string errorMessage, List<RankingRow> ranking)> GetRankingAsync(RankingSetting rankingSetting, int maxResultCount, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            // 認証
            await AuthenticateIfYetAsync(cancellationToken);

            // ランキング取得
            List<RankingRow> result = null;
            PlayFabError error = null;
            PlayFabClientAPI.GetLeaderboard(
                new GetLeaderboardRequest {
                    StatisticName = rankingSetting.RankingId,
                    MaxResultsCount = maxResultCount,
                },
                x => {
                    result = new List<RankingRow>();
                    for(int i = 0; i < x.Leaderboard.Count; i++) {
                        PlayerLeaderboardEntry entry = x.Leaderboard[i];
                        int rank = entry.Position + 1;
                        // 同スコアの場合は同ランクに修正
                        if(i > 0 && entry.StatValue == x.Leaderboard[i - 1].StatValue) {
                            rank = result[i-1].Rank;
                        }

                        result.Add(new RankingRow(rank, entry.PlayFabId, entry.DisplayName, DecodeScore(rankingSetting, entry.StatValue)));
                    }
                },
                x => error = x
            );
            await TaskUtil.WaitUntil(() => result != null || error != null, cancellationToken);

            if(error != null) {
                return (error.ErrorMessage, null);
            }

            return (null, result);
        }

        public async Task<(string errorMessage, RankingRow playerRankingRow)> GetPlayerRankingRowAsync(RankingSetting rankingSetting, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            // 認証
            await AuthenticateIfYetAsync(cancellationToken);

            // ランキング取得
            GetLeaderboardAroundPlayerResult result = null;
            PlayFabError error = null;
            PlayFabClientAPI.GetLeaderboardAroundPlayer(
                new GetLeaderboardAroundPlayerRequest {
                    StatisticName = rankingSetting.RankingId,
                    MaxResultsCount = 1,
                },
                x => result = x,
                x => error = x
            );
            await TaskUtil.WaitUntil(() => result != null || error != null, cancellationToken);

            if(error != null) {
                return (error.ErrorMessage, null);
            }

            // レコードが登録されていなくても生成されて帰ってくる(サーバ側には登録されない)ので0件になることはない
            PlayerLeaderboardEntry entry = result.Leaderboard[0];
            RankingRow playerRankingRow = null;
            // 登録済かどうかをDisplayNameで判断
            if(string.IsNullOrEmpty(entry.DisplayName) == false) {
                playerRankingRow = new RankingRow(entry.Position + 1, entry.PlayFabId, entry.DisplayName, DecodeScore(rankingSetting, entry.StatValue));
            }

            return (null, playerRankingRow);
        }

        public async Task<(string errorMessage, bool timeout)> WaitForPlayerRowReflection(string displayName, RankingScore highScore, float timeLimit, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            float startTime = Time.time;

            while(true) {
                var result = await GetPlayerRankingRowAsync(highScore.RankingSetting, cancellationToken);
                if(result.errorMessage != null) {
                    return (result.errorMessage, false);
                }

                if(displayName == result.playerRankingRow.DisplayName && highScore.Value == result.playerRankingRow.Score.Value) {
                    return (null, false);
                }

                float time = Time.time  - startTime;
                if(time > timeLimit) {
                    return (null, true);
                }
            }
        }
    }
}