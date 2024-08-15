using System.Collections.Generic;
using QuickRanking.Core;

namespace QuickRanking.UI {
    public interface IRankingListView {
        void Set(List<RankingRow> rankingData);
    }
}