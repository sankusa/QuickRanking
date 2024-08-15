using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QuickRanking.Common;
using QuickRanking.Core;
using QuickRanking.Playfab;

namespace QuickRanking {
    public class QuickRankingControlCenter : Singleton<QuickRankingControlCenter> {
        private readonly IDataSaver _dataSaver;

        private readonly QuickRankingSetting _quickRankingSetting;
        private readonly PlayerIdManager _playerIdManager;
        private readonly HighScoreManager _highScoreManager;

        private readonly IBackendController _backendController;

        public QuickRankingControlCenter() {
            _dataSaver = new PlayerPrefsDataSaver();

            _quickRankingSetting = QuickRankingSetting.Instance;
            _playerIdManager = new PlayerIdManager(_dataSaver, _quickRankingSetting);
            _highScoreManager = new HighScoreManager(_dataSaver);

            _backendController = new PlayfabController(_playerIdManager);
        }

        public bool SetHighScoreIfBetter(RankingScore score) {
            return _highScoreManager.SetHighScoreIfBetter(score);
        }

        public RankingScore GetHighScore(RankingSetting rankingSetting) {
            return _highScoreManager.GetHighScore(rankingSetting);
        }

        public async Task<string> SendDisplayNameAsync(string displayName, CancellationToken cancellationToken = default) {
            // Debug.Log(nameof(SendDisplayNameAsync) + " : Start");
            string errorMessage = await _backendController.SendDisplayNameAsync(displayName, cancellationToken);
            // Debug.Log(nameof(SendDisplayNameAsync) + " : " + (errorMessage == null ? "End" : $"Error : {errorMessage}"));
            return errorMessage;
        }

        public async Task<string> SendScoreAsync(RankingScore score, CancellationToken cancellationToken = default) {
            // Debug.Log(nameof(SendScoreAsync) + " : Start");
            string errorMessage = await _backendController.SendScoreAsync(score, cancellationToken);
            // Debug.Log(nameof(SendScoreAsync) + " : " + (errorMessage == null ? "End" : $"Error : {errorMessage}"));
            return errorMessage;
        }

        public async Task<(string errorMessage, List<RankingRow> ranking)> GetRankingAsync(RankingSetting rankingSetting, CancellationToken cancellationToken = default) {
            // Debug.Log(nameof(GetRankingAsync) + " : Start");
            (string, List<RankingRow>) result = await _backendController.GetRankingAsync(rankingSetting, _quickRankingSetting.RankingFetchCount, cancellationToken);
            // Debug.Log(nameof(GetRankingAsync) + " : " + (result.Item1 == null ? "End" : $"Error : {result.Item1}"));
            return result;
        }

        public async Task<(string errorMessage, RankingRow playerRow)> GetPlayerRankingRowAsync(RankingSetting rankingSetting, CancellationToken cancellationToken = default) {
            // Debug.Log(nameof(GetPlayerRankingRowAsync) + " : Start");
            (string, RankingRow) result = await _backendController.GetPlayerRankingRowAsync(rankingSetting, cancellationToken);
            // Debug.Log(nameof(GetPlayerRankingRowAsync) + " : " + (result.Item1 == null ? "End" : $"Error : {result.Item1}"));
            return result;
        }

        public async Task<(string errorMessage, bool timeout)> WaitForPlayerRowReflectionAsync(string displayName, RankingScore highScore, CancellationToken cancellationToken = default) {
            return await _backendController.WaitForPlayerRowReflectionAsync(displayName, highScore, _quickRankingSetting.DataReflectionWaitTimeLimit, cancellationToken);
        }
    }
}