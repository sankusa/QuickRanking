using System;
using QuickRanking.Common;

namespace QuickRanking.Core {
    public class HighScoreManager {
        private readonly IDataSaver _dataSaver;

        public HighScoreManager(IDataSaver dataSaver) {
            _dataSaver = dataSaver;
        }

        private void SetHighScore(RankingSetting rankingSetting, RankingScore score) {
            if(rankingSetting != score.RankingSetting) {
                throw new ArgumentException();
            }
            _dataSaver.SetDouble(rankingSetting.ScoreSaveKey, score.Value);
        }

        public bool SetHighScoreIfBetter(RankingScore score) {
            RankingSetting rankingSetting = score.RankingSetting;

            if(_dataSaver.HasKey(rankingSetting.ScoreSaveKey) == false) {
                SetHighScore(rankingSetting, score);
                return true;
            }
            RankingScore highScore = GetHighScore(rankingSetting);
            if(score.IsBetterScoreThan(highScore)) {
                SetHighScore(rankingSetting, score);
                return true;
            }
            return false;
        }

        public RankingScore GetHighScore(RankingSetting rankingSetting) {
            if(_dataSaver.HasKey(rankingSetting.ScoreSaveKey) == false) {
                return null;
            }
            return new RankingScore(rankingSetting, _dataSaver.GetDouble(rankingSetting.ScoreSaveKey));
        }
    }
}