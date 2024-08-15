using QuickRanking.Core;
using QuickRanking.UI;
using UnityEngine;

namespace QuickRanking {
    public static class Facade {
        public static void CallRanking(double score, int rankingIndex = 0, string rankingPanelId = null) {
            RankingSetting rankingSetting = QuickRankingSetting.Instance.RankingSettings[rankingIndex];
            RankingScore scoreObj = new RankingScore(rankingSetting, score);

            // ハイスコアの更新
            QuickRankingControlCenter.Instance.SetHighScoreIfBetter(scoreObj);

            // ランキングパネルの表示
            RankingPanel rankingPanel;
            if(rankingPanelId == null) {
                rankingPanel = RankingPanel.Panels[0];
            }
            else {
                rankingPanel = RankingPanel.FindById(rankingPanelId);
            }
            rankingPanel.Show(scoreObj);
        }

        public static void CallRankingWithoutScore(int rankingIndex = 0, string rankingPanelId = null) {
            RankingSetting rankingSetting = QuickRankingSetting.Instance.RankingSettings[rankingIndex];

            // ランキングパネルの表示
            RankingPanel rankingPanel;
            if(rankingPanelId == null) {
                rankingPanel = RankingPanel.Panels[0];
            }
            else {
                rankingPanel = RankingPanel.FindById(rankingPanelId);
            }
            rankingPanel.Show(rankingSetting);
        }
    }
}