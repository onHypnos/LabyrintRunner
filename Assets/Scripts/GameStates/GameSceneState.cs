using Data;
using Extensions.Reactive;
using Services;
using Services.SaveLoad;
using Tools.Extensions;
using UniRx;
using UnityEngine;

namespace GameStates
{
    public interface IGameState
    {
        void EnterState();
        void ExitState();
    }

    //todo refactor to IDisposable
    public class GameSceneState : IGameState
    {
        public struct Ctx
        {
            public UpdateRunner UpdateRunner;
            public ConfigProvider ConfigProvider;
            public SceneLoader SceneLoader;
            public AssetProvider AssetProvider;
            public SaveLoadService SaveLoadService;
            public ReactiveEvent EnterNextScene;
        }

        private Ctx _ctx;
        private CompositeDisposable _compositeDisposable = new();
        public GameSceneState(Ctx ctx)
        {
            _ctx = ctx;
        }

        public void EnterState()
        {
            var onSceneLoaded = new ReactiveEvent();
            onSceneLoaded.SubscribeOnceWithSkip(InitGameScene);
            _ctx.SceneLoader.LoadScene(Consts.GameSceneName, onSceneLoaded);
        }

        private void InitGameScene()
        {
            GameSceneMediator mediator = GameObject.FindObjectOfType<GameSceneMediator>();
            mediator.SetCtx(new GameSceneMediator.Ctx
            {
                Disposables = _compositeDisposable,
                AssetProvider = _ctx.AssetProvider,
                EndScene = _ctx.EnterNextScene,
                SaveLoadService = _ctx.SaveLoadService,
                UpdateRunner = _ctx.UpdateRunner,
                ConfigProvider = _ctx.ConfigProvider
            });
        }
        
        public void ExitState()
        {
            _compositeDisposable.Dispose();
        }
    }
}