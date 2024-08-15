using UnityEngine;

namespace QuickRanking.Common {
    /// <summary>
    /// PlayerPrefsによる実装
    /// </summary>
    public class PlayerPrefsDataSaver : IDataSaver {
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public string GetString(string key) => PlayerPrefs.GetString(key);
        public void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public int GetInt(string key) => PlayerPrefs.GetInt(key);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public float GetFloat(string key) => PlayerPrefs.GetFloat(key);
        public void SetDouble(string key, double value) => PlayerPrefs.SetString(key, value.ToString("R"));
        public double GetDouble(string key) => double.Parse(PlayerPrefs.GetString(key));
    }
}