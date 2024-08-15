using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickRanking.Common {
    public class AbstractInputField : MonoBehaviour {
        [SerializeField] private InputField _legacyInputField;
        [SerializeField] private TMP_InputField _tmpInputField;

        void Reset() {
            _legacyInputField = GetComponent<InputField>();
            _tmpInputField = GetComponent<TMP_InputField>();
        }

        public void SetText(string text) {
            if(_legacyInputField != null) _legacyInputField.text = text;
            if(_tmpInputField != null) _tmpInputField.text = text;
        }

        public string GetText() {
            string text = null;
            if(_legacyInputField != null) text = _legacyInputField.text;
            if(_tmpInputField != null) text = _tmpInputField.text;
            return text;
        }
    }
}