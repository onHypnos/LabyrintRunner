using Extensions.Reactive;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    public class SceneLoader
    {
        public struct Ctx
        {
            public ReactiveEvent<AsyncOperation, ReactiveEvent> ShowCurtainCall;

        }

        private Ctx _ctx;

        public SceneLoader(Ctx ctx)
        {
            _ctx = ctx;
        }

        public void LoadScene(string sceneName, ReactiveEvent onSceneLoadedCallback)
        {
            var loading = SceneManager.LoadSceneAsync(sceneName);
            _ctx.ShowCurtainCall.Notify(loading, onSceneLoadedCallback);
        }
    }
}