using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QuickRanking.Core {
    /// <summary>
    /// ランキングDBとのデータ送受信を行う機能のインターフェース
    /// </summary>
    public interface IBackendController {
        /// <summary>プレイヤー名送信</summary>
        /// <returns>エラーメッセージ</returns>
        /// <returns></returns>
        Task<string> SendDisplayNameAsync(string displayName, CancellationToken cancellationToken);

        /// <summary>スコア送信</summary>
        /// <returns>エラーメッセージ</returns>
        Task<string> SendScoreAsync(RankingScore score, CancellationToken cancellationToken);

        /// <summary>ランキング取得</summary>
        /// <returns>エラーメッセージ,ランキング</returns>
        Task<(string errorMessage, List<RankingRow> ranking)> GetRankingAsync(RankingSetting rankingSetting, int maxResultCount, CancellationToken cancellationToken);

        /// <summary>プレイヤーのランキングレコードを取得</summary>
        /// <returns>エラーメッセージ,ランキング</returns>
        Task<(string errorMessage, RankingRow playerRankingRow)> GetPlayerRankingRowAsync(RankingSetting rankingSetting, CancellationToken cancellationToken);

        /// <summary>送信したデータの反映を待機</summary>
        /// <returns>エラーメッセージ,タイムアウト</returns>
        Task<(string errorMessage, bool timeout)> WaitForPlayerRowReflectionAsync(string displayName, RankingScore highScore, float timeLimit, CancellationToken cancellationToken);
    }
}