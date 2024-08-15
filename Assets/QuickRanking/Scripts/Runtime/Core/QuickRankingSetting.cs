using System;
using System.Collections.Generic;
using QuickRanking.Common;
using UnityEngine;

namespace QuickRanking.Core {
    [CreateAssetMenu(
        fileName = nameof(QuickRankingSetting),
        menuName = nameof(QuickRanking) + "/" + nameof(QuickRankingSetting)
    )]
    public class QuickRankingSetting : SingletonScriptableObject<QuickRankingSetting> {
        [SerializeField] private string _playerIdSaveKey = Guid.NewGuid().ToString("N");
        public string PlayerIdSaveKey => _playerIdSaveKey;

        [SerializeField, Min(1)] private int _minPlayerNameLength = 1;
        public int MinPlayerNameLength => _minPlayerNameLength;

        [SerializeField, Min(1)] private int _maxPlayerNameLength = 10;
        public int MaxPlayerNameLength => _maxPlayerNameLength;

        [SerializeField] private int _rankingFetchCount = 50;
        public int RankingFetchCount => _rankingFetchCount;

        [SerializeField, Min(0)] private float _dataReflectionWaitTimeLimit = 5;
        public float DataReflectionWaitTimeLimit => _dataReflectionWaitTimeLimit;

        [SerializeField] private List<RankingSetting> _rankingSettings;
        public IReadOnlyList<RankingSetting> RankingSettings => _rankingSettings;

        public bool IsCorrectPlayerNameLengh(int length) {
            return _minPlayerNameLength <= length && length <= _maxPlayerNameLength;
        }
    }
}
