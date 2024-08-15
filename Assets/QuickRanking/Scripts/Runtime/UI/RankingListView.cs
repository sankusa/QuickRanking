using System.Collections.Generic;
using QuickRanking.Core;
using UnityEngine;

namespace QuickRanking.UI {
    public class RankingListView : MonoBehaviour {
        [SerializeField] private RankingRowView _rankingRowViewPrefab;
        [SerializeField] private Transform _rankingRowViewParent;

        private List<RankingRowView> _rowViewList =null;

        void Start() {
            SetUpIfYet();
        }

        private void SetUpIfYet() {
            if(_rowViewList != null) return;

            _rowViewList = new List<RankingRowView>();
            foreach(Transform t in _rankingRowViewParent.transform) {
                RankingRowView rowView = t.GetComponent<RankingRowView>();
                if(rowView != null && rowView.isActiveAndEnabled) _rowViewList.Add(rowView);
            }
        }

        public void Set(RankingSetting rankingSetting, List<RankingRow> rankingData) {
            SetUpIfYet();

            int targetRowViewIndex = 0;

            foreach(RankingRow row in rankingData) {
                if(targetRowViewIndex >= _rowViewList.Count) {
                    RankingRowView rowView = Instantiate(_rankingRowViewPrefab, _rankingRowViewParent);
                    _rowViewList.Add(rowView);
                }

                RankingRowView targetRowView = _rowViewList[targetRowViewIndex];
                if(targetRowView.gameObject.activeSelf == false) targetRowView.gameObject.SetActive(true);
                targetRowView.Set(rankingSetting, row);

                targetRowViewIndex++;
            }

            for(; targetRowViewIndex < _rowViewList.Count; targetRowViewIndex++) {
                _rowViewList[targetRowViewIndex].gameObject.SetActive(false);
            }
        }
    }
}