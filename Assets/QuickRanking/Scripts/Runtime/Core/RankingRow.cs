namespace QuickRanking.Core {
    public class RankingRow {
        int _rank;
        public int Rank => _rank;

        string _playerId;
        public string PlayerId => _playerId;

        string _displayName;
        public string DisplayName => _displayName;

        private RankingScore _score;
        public RankingScore Score => _score;

        public RankingRow(int rank, string playerId, string displayName, RankingScore score) {
            _rank = rank;
            _playerId = playerId;
            _displayName = displayName;
            _score = score;
        }
    }
}