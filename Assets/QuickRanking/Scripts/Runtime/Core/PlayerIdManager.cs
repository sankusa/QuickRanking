using System;
using QuickRanking.Common;

namespace QuickRanking.Core {
    public class PlayerIdManager {
        private readonly IDataSaver _dataSaver;
        private readonly QuickRankingSetting _quickRankingSetting;

        public PlayerIdManager(IDataSaver dataSaver, QuickRankingSetting quickRankingSetting) {
            _dataSaver = dataSaver;
            _quickRankingSetting = quickRankingSetting;
        }

        public string GetPlayerId() {
            // ユーザIDが保存されていなければIDを生成して保存
            if(_dataSaver.HasKey(_quickRankingSetting.PlayerIdSaveKey) == false) {
                _dataSaver.SetString(_quickRankingSetting.PlayerIdSaveKey, Guid.NewGuid().ToString("N"));
            }
            // ユーザIDを取得
            return _dataSaver.GetString(_quickRankingSetting.PlayerIdSaveKey);
        }
    }
}