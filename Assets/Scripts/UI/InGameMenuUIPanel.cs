using Extensions.Reactive;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameMenuUIPanel : BaseMonoBehaviour , IUIPanel
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _closeMenu;
        public struct Ctx
        {
            public ReactiveEvent CallSaving;
            public ReactiveEvent CallLoading;
            public ReactiveEvent CloseMenu;
        }

        private Ctx _ctx;
        
        
        public void Show()
        {
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _loadButton.onClick.AddListener(_ctx.CallLoading.Notify);
            _saveButton.onClick.AddListener(_ctx.CallSaving.Notify);
            _closeMenu.onClick.AddListener(_ctx.CloseMenu.Notify);
        }
    }
}