using System;
using System.Collections.Generic;
using Extensions.Reactive;
using Services;
using Services.SaveLoad;
using Tools.Extensions;
using UniRx;
using UnityEngine;

namespace UI
{
    public class GameSceneUIMediator : BaseMonoBehaviour
    {
        [SerializeField] private InGameMenuUIPanel _menu;
        [SerializeField] private InGameUI _inGame;
        [SerializeField] private StartGameUI _startGame;


        public struct Ctx
        {
            public CompositeDisposable Disposable;
            public UpdateRunner UpdateRunner;
            public IReadOnlyReactiveProperty<float> TimeRemaining;
            public IReadOnlyReactiveProperty<int> TriesAmount;
            public ReactiveEvent CallSaving;
            public ReactiveEvent CallLoading;
            public ReactiveEvent GameStart;
            public ReactiveEvent MenuButtonClicked;
            public ReactiveEvent ResumeButtonClicked;
            public ReactiveEvent<SaveData> OnLoad;
            public ReactiveEvent OnLoseGame;
            public ReactiveEvent OnWinGame;
            public ReactiveProperty<Vector3> UiTouchInput;
        }

        private Ctx _ctx;

        private Dictionary<Type, IUIPanel> _panels =new();
        private IUIPanel _activePanel;
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;

            _menu.SetCtx(new InGameMenuUIPanel.Ctx
            {
                CallSaving = _ctx.CallSaving,
                CallLoading = _ctx.CallLoading,
                CloseMenu = _ctx.ResumeButtonClicked,
            });
            _panels.Add(typeof(InGameMenuUIPanel), _menu);
            
            _inGame.SetCtx(new InGameUI.Ctx
            {
                Disposable = _ctx.Disposable,
                TimeRemaining = _ctx.TimeRemaining,
                TriesAmount = _ctx.TriesAmount,
                MenuButtonClicked = _ctx.MenuButtonClicked,
                UiTouchInput = _ctx.UiTouchInput,
                OnLoseGame = _ctx.OnLoseGame,
                OnWinGame = _ctx.OnWinGame
            });
            _panels.Add(typeof(InGameUI), _inGame);
            
            _startGame.SetCtx(new StartGameUI.Ctx
            {
                Disposable = _ctx.Disposable,
                OnGameStart = _ctx.GameStart,
            });
            _panels.Add(typeof(StartGameUI), _startGame);
            
            _ctx.MenuButtonClicked.SubscribeWithSkip(() =>
            {
                _ctx.UpdateRunner.Pause();
                ShowPanel<InGameMenuUIPanel>();
            }).AddTo(_ctx.Disposable);
            _ctx.ResumeButtonClicked.SubscribeWithSkip(() =>
            {
                _ctx.UpdateRunner.Unpause();
                ShowPanel<InGameUI>();
            }).AddTo(_ctx.Disposable);
            _ctx.CallLoading.SubscribeWithSkip(() =>
            {
                _ctx.UpdateRunner.Unpause();
            }).AddTo(_ctx.Disposable);
            
            HideAllPanels();
            ShowPanel<StartGameUI>();
            _ctx.GameStart.SubscribeWithSkip(ShowPanel<InGameUI>);
        }


        private void ShowPanel<T>() where T:IUIPanel
        {
            _activePanel?.Hide();
            _activePanel = _panels[typeof(T)];
            _activePanel.Show();
        }

        private void HideAllPanels()
        {
            _menu.Hide();
            _inGame.Hide();
            _startGame.Hide();
        }
    }
}