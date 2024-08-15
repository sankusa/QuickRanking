using UnityEngine;

namespace QuickRanking.Common {
    public class Spinner : MonoBehaviour {
        [SerializeField] private int _phase;
        [SerializeField] private float _span;

        private float _timer;

        Transform t;

        void Start() {
            t = transform;
        }

        void Update() {
            _timer = Mathf.Max(_timer - Time.deltaTime, 0);
            if(_timer == 0) {
                t.Rotate(0, 0, 360f / _phase);
                _timer = _span;
            }
        }
    }
}