using System.Collections;
using System.Collections.Generic;
using Extensions.Reactive;
using TMPro;
using Tools.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : BaseMonoBehaviour , IUIPanel
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _timeRemainingText;
        [SerializeField] private TMP_Text _triesAmount;
        [SerializeField] private Button _menuButton;
        [SerializeField] private SimpleInputJoystickWrapper _joystickWrapper;
        [SerializeField] private TMP_Text _encounterMsg;
        
        public struct Ctx
        {
            public CompositeDisposable Disposable;
            public IReadOnlyReactiveProperty<float> TimeRemaining;
            public IReadOnlyReactiveProperty<int> TriesAmount;
            public ReactiveEvent MenuButtonClicked;
            public ReactiveProperty<Vector3> UiTouchInput;
            public ReactiveEvent OnLoseGame;
            public ReactiveEvent OnWinGame;
        }

        private Ctx _ctx;
        private Coroutine _showMessageCoroutine;
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.TimeRemaining.Subscribe((value) =>
            {
                _timeRemainingText.text = $"{(int)value / 60}:{(value % 60):00}";
            }).AddTo(_ctx.Disposable);
            
            _ctx.TriesAmount.Subscribe((value) =>
            {
                _triesAmount.text = $"{value}";
            }).AddTo(_ctx.Disposable);
            
            _menuButton.onClick.AddListener(_ctx.MenuButtonClicked.Notify);
            _joystickWrapper.SetCtx(new SimpleInputJoystickWrapper.Ctx
            {
                UiTouchInput = _ctx.UiTouchInput
            });

            _ctx.OnWinGame.SubscribeWithSkip(() => ShowMessage("Nice One", 3f, Color.green))
                .AddTo(_ctx.Disposable);
            _ctx.OnLoseGame.SubscribeWithSkip(() => ShowMessage("Try Again", 3f, Color.red))
                .AddTo(_ctx.Disposable);
        }

        private void ShowMessage(string msg, float time, Color color)
        {
            if (_showMessageCoroutine != null)
            {
                StopCoroutine(_showMessageCoroutine);
            }
            _showMessageCoroutine = StartCoroutine(ShowMsgCoroutine(msg, time, color));
        }

        private IEnumerator ShowMsgCoroutine(string msg, float time, Color color)
        {
            _encounterMsg.text = msg;
            _encounterMsg.color = color;
            _encounterMsg.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(time);
            _encounterMsg.gameObject.SetActive(false);
        }

        public void Show()
        {
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }
    }
}