using System;
using System.Collections.Generic;
using Extensions.Reactive;
using Services;
using Services.SaveLoad;

namespace GameStates
{
    public class GameStatesMachine
    {
        public struct Ctx
        {
            public UpdateRunner UpdateRunner;
            public ConfigProvider ConfigProvider;
            public AssetProvider AssetProvider;
            public SceneLoader SceneLoader;
            public SaveLoadService SaveLoadService;
        }
    
        private Ctx _ctx;
        private Dictionary<Type, IGameState> _gameStates = new();
        private IGameState _currentState;
        
        public GameStatesMachine(Ctx ctx)
        {
            _ctx = ctx;

            ReactiveEvent MoveToNextScene = new ReactiveEvent();
            MoveToNextScene.SubscribeWithSkip(ChangeState<GameSceneState>);//while only one gamescene
            
            _gameStates.Add(typeof(GameSceneState), new GameSceneState(new GameSceneState.Ctx
            {
                SceneLoader = _ctx.SceneLoader,
                AssetProvider = _ctx.AssetProvider,
                SaveLoadService = _ctx.SaveLoadService,
                EnterNextScene = MoveToNextScene,
                UpdateRunner = _ctx.UpdateRunner,
                ConfigProvider = _ctx.ConfigProvider
            }));
            
            ChangeState<GameSceneState>();
        }

        public void ChangeState<T>() where T:IGameState
        {
            _currentState?.ExitState();
            _currentState = _gameStates[typeof(T)];
            _currentState.EnterState();
        }

        
    }
}