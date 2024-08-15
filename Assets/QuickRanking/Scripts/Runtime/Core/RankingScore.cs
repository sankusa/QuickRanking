using System;

namespace QuickRanking.Core {
    public class RankingScore {
        private readonly RankingSetting _rankingSetting;
        public RankingSetting RankingSetting => _rankingSetting;

        private readonly double _value;
        public double Value => _value;

        public RankingScore(RankingSetting rankingSetting, double value) {
            _rankingSetting = rankingSetting;
            _value = rankingSetting.TruncateDecimalPart(value);
        }

        public bool IsBetterScoreThan(RankingScore score) {
            if(_rankingSetting != score._rankingSetting) {
                throw new ArgumentException();
            }
            
            if(_rankingSetting.OrderBy == RankingSetting.Order.Descending) {
                return _value > score._value;
            }
            else {
                return _value < score._value;
            }
        }

        public string GenerateScoreText() {
            return _rankingSetting.GenerateScoreText(_value);
        }
    }
}