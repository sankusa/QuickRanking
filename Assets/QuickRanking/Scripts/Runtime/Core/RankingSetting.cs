using System;
using UnityEngine;

namespace QuickRanking.Core {
    [CreateAssetMenu(
        fileName = nameof(RankingSetting),
        menuName = nameof(QuickRanking) + "/" + nameof(RankingSetting)
    )]
    public class RankingSetting : ScriptableObject {
        public enum Order {
            Descending = 0,
            Ascending = 1,
        }

        [SerializeField] private string _rankingId;
        public string RankingId => _rankingId;

        [SerializeField] private string _rankingName;
        public string RankingName => _rankingName;

        [SerializeField] private string _scoreSaveKey = Guid.NewGuid().ToString("N");
        public string ScoreSaveKey => _scoreSaveKey;

        [SerializeField] private Order _orderBy;
        public Order OrderBy => _orderBy;

        [SerializeField] private string _scorePrefix;

        [SerializeField] private string _scoreSuffix;

        [SerializeField, Min(0)] private int _decimalDigits;
        public int DecimalDigits => _decimalDigits;

        public string GenerateScoreText(double score) {
            return $"{_scorePrefix}{score}{_scoreSuffix}";
        }

        public double TruncateDecimalPart(double value) {
            double x = Math.Pow(10, _decimalDigits);
            return Math.Truncate(value * x) / x;
        }
    }
}
