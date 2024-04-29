using System;
using System.Collections;
using System.Collections.Generic;
using Extensions.Reactive;
using GameStates;
using Services;
using Services.SaveLoad;
using UI;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("SO")]
    [SerializeField] private ConfigProvider _configProvider;
    [SerializeField] private AssetProvider _assetProvider;
    
    [Header("ChildComponents")]
    [SerializeField] private UpdateRunner _updateRunner;
    [SerializeField] private LoadingCurtain _loadingCurtain;
    
    public void Start()
    {
        Application.targetFrameRate = _configProvider.Settings.ApplicationTargetFramerate;
        DontDestroyOnLoad(gameObject);

        ReactiveEvent<AsyncOperation, ReactiveEvent> onShowCurtainCall =
            new ReactiveEvent<AsyncOperation, ReactiveEvent>();
        
        _loadingCurtain.SetCtx(new LoadingCurtain.Ctx
        {
            ConfigProvider = _configProvider,
            OnShowCurtainCall = onShowCurtainCall,
        });
        
        var gameStates = new GameStatesMachine(new GameStatesMachine.Ctx
        {
            UpdateRunner = _updateRunner,
            ConfigProvider = _configProvider,
            AssetProvider = _assetProvider,
            SceneLoader = new SceneLoader(new SceneLoader.Ctx
            {
                ShowCurtainCall = onShowCurtainCall
            }),
            SaveLoadService = new SaveLoadService()
        });
    }
}