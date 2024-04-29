using Cinemachine;
using Services;
using Services.SaveLoad;
using Tools.Extensions;
using UniRx;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerCharacter : BaseMonoBehaviour
    {
        [SerializeField] private CharacterMovement _characterMovement;
        [SerializeField] private CinemachineVirtualCamera _camera;
        
        public struct Ctx
        {
            public IReadOnlyReactiveProperty<Vector3> UiTouchInput;
            public UpdateRunner UpdateRunner;
            public ConfigProvider ConfigProvider;
        }

        private Ctx _ctx;
        private CompositeDisposable _compositeDisposable = new();
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            
            _camera.transform.parent = null;//todo refactor говнокод
            
            _characterMovement.SetCtx(new CharacterMovement.Ctx
            {
                Disposable = _compositeDisposable,
                UiTouchInput = _ctx.UiTouchInput,
                UpdateRunner = _ctx.UpdateRunner,
                CharacterSpeed = _ctx.ConfigProvider.Settings.DefaultCharacterSpeed,

            });
        }

        public SaveData.PlayerData SavedData()
        {
            return new SaveData.PlayerData
            {
                Position = transform.position,
                Rotation = transform.rotation,
                RemainingTime = 0,
                Tries = 0
            };
        }
        
        protected override void OnDestroy()
        {
            _compositeDisposable.Dispose();
            Destroy(_camera); //todo refactor говнокод
            base.OnDestroy();
        }
    }
}
