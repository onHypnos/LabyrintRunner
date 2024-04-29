using Extensions.Reactive;
using TMPro;
using Tools.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StartGameUI : BaseMonoBehaviour , IUIPanel
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _startGameButton;
        public struct Ctx
        {
            public CompositeDisposable Disposable;
            public ReactiveEvent OnGameStart;
        }

        private Ctx _ctx;
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _startGameButton.onClick.AddListener(_ctx.OnGameStart.Notify);
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