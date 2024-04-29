using System.Collections;
using Extensions.Reactive;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class LoadingCurtain : MonoBehaviour
    {
        public struct Ctx
        {
            public ConfigProvider ConfigProvider;
            public IReadOnlyReactiveEvent<AsyncOperation, ReactiveEvent> OnShowCurtainCall;
        }

        private Ctx _ctx;
        
        [SerializeField] private Image _progressBar;
        [SerializeField] private Image _curtain;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private GameObject _panel;
        
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.OnShowCurtainCall.SubscribeWithSkip(ShowCurtain);
        }
        
        public void ShowCurtain(AsyncOperation loadingScene, ReactiveEvent OnCurtainEndCallback)
        {
            //todo refactor coroutine call from coroutine runner
            StartCoroutine(LoadingProgress(loadingScene, OnCurtainEndCallback));
        }

        private void Enable()
        {
            _panel.SetActive(true);
        }

        private void Disable()
        {
            _panel.SetActive(false);
        }

        private IEnumerator LoadingProgress(AsyncOperation loadingScene, ReactiveEvent onCurtainEndCallback)
        {
            _progressText.text = "0%";
            StartCoroutine(ShowCurtain());
            while (!loadingScene.isDone)
            {
                _progressBar.fillAmount = loadingScene.progress*100;
                _progressText.text = $"{loadingScene.progress*100:0.0}%";
                yield return null;
            }

            yield return HideCurtain();
            onCurtainEndCallback.Notify();
        }

        private IEnumerator ShowCurtain()
        {
            Enable();
            float curtainShowtime = _ctx.ConfigProvider.Settings.CurtainShowingTime;
            Color tempColor = _curtain.color;
             
            for (float i = 0; i < curtainShowtime; i++)
            {
                tempColor.a = Mathf.Lerp(_curtain.color.a, 255, Mathf.Clamp01(i/curtainShowtime));
                _curtain.color = tempColor;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator HideCurtain()
        {
            float curtainHideTime = _ctx.ConfigProvider.Settings.CurtainHideTime;
            Color tempColor = _curtain.color;
            
            for (float i = curtainHideTime; i > 0; i--)
            {
                tempColor.a = Mathf.Lerp(_curtain.color.a, 255, Mathf.Clamp01(i/curtainHideTime));
                _curtain.color = tempColor;
                yield return new WaitForFixedUpdate();
            }

            tempColor.a = 0;
            _curtain.color = tempColor;
            Disable();
        }
        
    }
}