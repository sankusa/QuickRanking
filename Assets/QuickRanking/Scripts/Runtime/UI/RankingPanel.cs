using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QuickRanking.Common;
using QuickRanking.Core;
using UnityEngine;
using UnityEngine.UI;

namespace QuickRanking.UI {
    public class RankingPanel : MonoBehaviour {
        private static List<RankingPanel> _panels = new List<RankingPanel>();
        public static IReadOnlyList<RankingPanel> Panels => _panels;
        public static RankingPanel FindById(string id) => _panels.Find(x => x._id == id);

        [SerializeField] private string _id;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        [SerializeField] private RankingListView _rankingListView;
        [SerializeField] private RankingRowView _playerRankingRowView;
        [SerializeField] private AbstractText _currentScoreText;
        [SerializeField] private AbstractText _highScoreText;

        [SerializeField] private GameObject _sendArea;
        [SerializeField] private AbstractInputField _nameInputField;
        [SerializeField] private AbstractText _playerNameLengthInfoText;
        [SerializeField] private Button _sendButton;

        [SerializeField] private AbstractText _messageText;

        [SerializeField] private GameObject _busyPanel;

        [SerializeField] private Button _updateButton;
        [SerializeField] private Button _hideButton;

        private RankingSetting _rankingSetting;
        private RankingScore _currentScore;
        private RankingScore _highScore;
        private List<RankingRow> _ranking = new List<RankingRow>();
        private RankingRow _playerRankingRow;

        private int _isRequestingCounter;

        private CancellationTokenSource _cts = new CancellationTokenSource();
        private CancellationToken _cancellationToken;
        
        void Awake() {
            _cancellationToken = _cts.Token;

            _panels.Add(this);
        }

        void Start() {
            _sendButton.onClick.AddListener(OnSendButtonClick);
            _updateButton.onClick.AddListener(OnUpdateButtonClick);
            _hideButton.onClick.AddListener(OnHideButtonClick);
        }

        void Update() {
            _busyPanel.SetActive(_isRequestingCounter > 0);

            _sendArea.SetActive(_highScore != null);
            _sendButton.interactable = _highScore != null
                && (
                    _playerRankingRow == null
                    || _highScore.IsBetterScoreThan(_playerRankingRow.Score)
                    || _nameInputField.GetText() != _playerRankingRow.DisplayName
                )
                && QuickRankingSetting.Instance.IsCorrectPlayerNameLengh(_nameInputField.GetText().Length);
        }

        void OnDestroy() {
            _panels.Remove(this);

            _cts.Cancel();
        }

        public void Show(RankingScore score) {
            _currentScore = score;
            ShowInternal(_currentScore.RankingSetting);
        }

        public void Show(RankingSetting rankingSetting) {
            _currentScore = null;
            ShowInternal(rankingSetting);
        }

        private void ShowInternal(RankingSetting rankingSetting) {
            _rankingSetting = rankingSetting;
            _highScore = QuickRankingControlCenter.Instance.GetHighScore(_rankingSetting);

            _canvasGroup.alpha = 1;
            _graphicRaycaster.enabled = true;

            RefleshAsync().Forget();
        }

        public void Hide() {
            _canvasGroup.alpha = 0;
            _graphicRaycaster.enabled = false;
        }

        private void OnSendButtonClick() {
            OnSendButtonClickAsync().Forget();
        }

        private void OnUpdateButtonClick() {
            RefleshAsync().Forget();
        }

        private void OnHideButtonClick() {
            Hide();
        }

        public async Task OnSendButtonClickAsync() {
            _cancellationToken.ThrowIfCancellationRequested();

            _isRequestingCounter++;

            // 送信
            if(_playerRankingRow == null || _nameInputField.GetText() != _playerRankingRow.DisplayName) {
                bool uploadDisplayNameSuccess = await UploadDisplayNameAsync();
                if(uploadDisplayNameSuccess == false) goto End;
            }
            if(_playerRankingRow == null || _highScore.IsBetterScoreThan(_playerRankingRow.Score)) {
                bool uploadHighScoreAsyncSuccess = await UploadHighScoreAsync();
                if(uploadHighScoreAsyncSuccess == false) goto End;
            }

            // データ反映待機
            var result = await QuickRankingControlCenter.Instance.WaitForPlayerRowReflectionAsync(_nameInputField.GetText(), _highScore, _cancellationToken);
            bool waitForPlayerRowReflectionSuccess = result.errorMessage == null;
            if(waitForPlayerRowReflectionSuccess == false) {
                _messageText.SetText("通信エラー");
                Debug.LogWarning(result.errorMessage);
                goto End;
            }
            bool waitReflectionTimeout = result.timeout;

            // リフレッシュ
            bool refleshSuccess = await RefleshAsync();
            if(refleshSuccess == false) goto End;

            // メッセージ設定
            if(waitReflectionTimeout) {
                _messageText.SetText("送信成功 ・・・ 反映に時間がかかっています");
            }
            else {
                _messageText.SetText("送信成功");
            }

        End:
            _isRequestingCounter--;
        }

        public async Task<bool> RefleshAsync() {
            _cancellationToken.ThrowIfCancellationRequested();

            _isRequestingCounter++;

            bool success = true;
            success &= await DownloadRankingAsync();
            success &= await DownloadPlayerRankingRowAsync();

            _isRequestingCounter--;

            if(success) {
                _messageText.SetText("取得成功");
            }

            Repaint();

            return success;
        }

        private async Task<bool> UploadDisplayNameAsync() {
            _cancellationToken.ThrowIfCancellationRequested();

            _isRequestingCounter++;

            string errorMessage = await QuickRankingControlCenter.Instance.SendDisplayNameAsync(_nameInputField.GetText(), _cancellationToken);
            bool success = errorMessage == null;
            if(success == false) {
                _messageText.SetText("通信エラー");
                Debug.LogWarning(errorMessage);
            }

            _isRequestingCounter--;

            return success;
        }

        private async Task<bool> UploadHighScoreAsync() {
            _cancellationToken.ThrowIfCancellationRequested();

            _isRequestingCounter++;

            string errorMessage = await QuickRankingControlCenter.Instance.SendScoreAsync(_currentScore, _cancellationToken);
            bool success = errorMessage == null;
            if(success == false) {
                _messageText.SetText("通信エラー");
                Debug.LogWarning(errorMessage);
            }

            _isRequestingCounter--;

            return success;
        }

        private async Task<bool> DownloadRankingAsync() {
            _cancellationToken.ThrowIfCancellationRequested();

            _isRequestingCounter++;

            var rankingResult = await QuickRankingControlCenter.Instance.GetRankingAsync(_rankingSetting, _cancellationToken);
            bool success = rankingResult.errorMessage == null;
            if(success) {
                _ranking = rankingResult.ranking;
            }
            else {
                _messageText.SetText("通信エラー");
                Debug.LogWarning(rankingResult.errorMessage);
            }

            _isRequestingCounter--;

            return success;
        }

        private async Task<bool> DownloadPlayerRankingRowAsync() {
            _cancellationToken.ThrowIfCancellationRequested();

            _isRequestingCounter++;

            var playerRecordResult = await QuickRankingControlCenter.Instance.GetPlayerRankingRowAsync(_rankingSetting, _cancellationToken);
            bool success = playerRecordResult.errorMessage == null;
            if(success) {
                _playerRankingRow = playerRecordResult.playerRow;
            }
            else {
                _messageText.SetText("通信エラー");
                Debug.LogWarning(playerRecordResult.errorMessage);
            }

            _isRequestingCounter--;

            return success;
        }

        private void Repaint() {
            _currentScoreText.SetText(_currentScore == null ? "----" : _currentScore.GenerateScoreText());
            _highScoreText.SetText(_highScore == null ? "----" : _highScore.GenerateScoreText());

            _rankingListView.Set(_rankingSetting, _ranking);

            if(_playerRankingRow == null) {
                _playerRankingRowView.Set(0, "----", "----");
                _nameInputField.SetText(string.Empty);
            }
            else {
                _playerRankingRowView.Set(_rankingSetting, _playerRankingRow);
                _nameInputField.SetText(_playerRankingRow.DisplayName);
            }

            _playerNameLengthInfoText.SetText($"{QuickRankingSetting.Instance.MinPlayerNameLength}～{QuickRankingSetting.Instance.MaxPlayerNameLength}文字");
        }
    }
}