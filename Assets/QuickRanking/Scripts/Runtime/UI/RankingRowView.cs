using QuickRanking.Common;
using QuickRanking.Core;
using UnityEngine;

namespace QuickRanking.UI {
    public class RankingRowView : MonoBehaviour {
        [SerializeField] private AbstractText _rankText;
        [SerializeField] private AbstractText _displayNameText;
        [SerializeField] private AbstractText _scoreText;

        public void Set(int rank, string displayName, string scoreText) {
            _rankText.SetText(rank.ToString());
            _displayNameText.SetText(displayName);
            _scoreText.SetText(scoreText);
        }

        public void Set(RankingSetting rankingSetting , int rank, string displayName, double score) {
            Set(rank, displayName, rankingSetting.GenerateScoreText(score));
        }

        public void Set(RankingSetting rankingSetting , RankingRow rankingRow) {
            Set(rankingSetting, rankingRow.Rank, rankingRow.DisplayName, rankingRow.Score.Value);
        }
    }
}