namespace QuickRanking.Common {
    /// <summary>
    /// key/value形式のデータ永続化機能インターフェース
    /// </summary>
    public interface IDataSaver {
        bool HasKey(string key);
        void SetString(string key, string value);
        string GetString(string key);
        void SetInt(string key, int value);
        int GetInt(string key);
        void SetFloat(string key, float value);
        float GetFloat(string key);
        void SetDouble(string key, double value);
        double GetDouble(string key);
    }
}