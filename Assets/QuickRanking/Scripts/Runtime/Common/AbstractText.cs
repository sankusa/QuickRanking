using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickRanking.Common {
    public class AbstractText : MonoBehaviour {
        [SerializeField] private Text _legacyText;
        [SerializeField] private TMP_Text _tmpText;

        void Reset() {
            _legacyText = GetComponent<Text>();
            _tmpText = GetComponent<TMP_Text>();
        }

        public void SetText(string text) {
            if(_legacyText != null) _legacyText.text = text;
            if(_tmpText != null) _tmpText.text = text;
        }
    }
}