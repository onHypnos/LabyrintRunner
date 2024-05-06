using System;
using Characters.Player;
using Extensions.Reactive;
using Fabrics;
using Logic;
using Services;
using Services.SaveLoad;
using Tools.Extensions;
using UI;
using UniRx;
using UnityEngine;

namespace GameStates
{
    public class GameSceneMediator : BaseMonoBehaviour, IExecutablePerFrame
    {
        [SerializeField] private GameSceneUIMediator _gameSceneUIMediator;
        [SerializeField] private LevelEnvironment _levelEnvironment;
        
        private HeroFabric _heroFabric = new();
        private EnemyFabric _enemyFabric = new();

        private ReactiveEvent _onGamePause = new();
        private ReactiveEvent _onGameResumed = new();
        private ReactiveEvent _onGameStart = new();
        private ReactiveEvent _onGameLose = new();
        private ReactiveEvent _onGameWin = new();
        
        private ReactiveProperty<float> _timeRemaining = new();
        private ReactiveProperty<int> _triesAmount = new();
        private ReactiveProperty<Vector3> _uiTouchInput = new();
        private ReactiveProperty<PlayerCharacter> _currentPlayer = new();
        private bool _countTimer = false; //todo refactor говнокод
        
        public struct Ctx
        {
            public CompositeDisposable Disposables;
            public AssetProvider AssetProvider;
            public ReactiveEvent EndScene;  
            public SaveLoadService SaveLoadService;
            public UpdateRunner UpdateRunner;
            public ConfigProvider ConfigProvider;
        }

        private Ctx _ctx;
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.UpdateRunner.Subscribe(this);
            
            _heroFabric.SetCtx(new HeroFabric.Ctx
            {
                AssetProvider = _ctx.AssetProvider,
                UpdateRunner = _ctx.UpdateRunner,
                ConfigProvider = _ctx.ConfigProvider,
                UiTouchInput = _uiTouchInput,
            });
            
            _enemyFabric.SetCtx(new EnemyFabric.Ctx
            {
                AssetProvider = _ctx.AssetProvider,
                UpdateRunner = _ctx.UpdateRunner,
                ConfigProvider = _ctx.ConfigProvider,
                OnLoseGame = _onGameLose,
                CurrentPlayerCharacter = _currentPlayer
            });
            
            _gameSceneUIMediator.SetCtx(new GameSceneUIMediator.Ctx
            {
                Disposable = _ctx.Disposables,
                UpdateRunner = _ctx.UpdateRunner,
                TimeRemaining = _timeRemaining,
                TriesAmount = _triesAmount,
                CallSaving = _ctx.SaveLoadService.CallSaving,
                CallLoading = _ctx.SaveLoadService.CallLoading,
                GameStart = _onGameStart,
                MenuButtonClicked = _onGamePause,
                ResumeButtonClicked = _onGameResumed,
                OnLoad = _ctx.SaveLoadService.OnLoad,
                OnLoseGame = _onGameLose,
                OnWinGame = _onGameWin,
                UiTouchInput = _uiTouchInput,
            });

            _levelEnvironment.SetCtx(new LevelEnvironment.Ctx
            {
                EnemyFabric = _enemyFabric,
                HeroFabric = _heroFabric,
                OnSavePlayerHandler = _ctx.SaveLoadService.OnSavePlayerHandler,
                OnSaveLevelHandler = _ctx.SaveLoadService.OnSaveLevelHandler,
                TimeRemaining = _timeRemaining,
                TriesAmount = _triesAmount,
                PlayerReachWinEncounter = _onGameWin,
                CurrentPlayer = _currentPlayer,
            });

            _triesAmount.SetValueAndForceNotify(0);
            _ctx.SaveLoadService.OnLoad.SubscribeWithSkip(LoadLevel);
            _onGameLose.SubscribeWithSkip(OnGameLose);
            _onGameWin.SubscribeWithSkip(OnGameWin);
            _onGameStart.SubscribeWithSkip(OnGameStart);
            
            LoadLevel();
        }

        private void OnGameStart()
        {
            _countTimer = true;
        }

        private void OnGameWin()
        {
            _triesAmount.SetValueAndForceNotify(0);
            OnGameEnd();
        }

        private void OnGameLose()
        {
            _triesAmount.SetValueAndForceNotify(_triesAmount.Value + 1);
            OnGameEnd();
        }

        private void OnGameEnd()
        {
            _countTimer = false;
            LoadLevel();
        }
        
        private void LoadLevel(SaveData data = null)
        {
            if (data != null)
            {
                _timeRemaining.SetValueAndForceNotify(data.PlayerDataInstance.RemainingTime);
                _triesAmount.SetValueAndForceNotify(data.PlayerDataInstance.Tries);
                _levelEnvironment.LoadLevel(data);
            }
            else
            {
                _timeRemaining.SetValueAndForceNotify(_ctx.ConfigProvider.Settings.DefaultLevelTime);
                _levelEnvironment.LoadLevel();
            }
            _onGameStart.Notify();
        }

        public void Execute()
        {
            if (_countTimer)
            {
                _timeRemaining.SetValueAndForceNotify(_timeRemaining.Value - Time.deltaTime * _ctx.UpdateRunner.GetGroupTimeScale);
            }
        }
    }
}