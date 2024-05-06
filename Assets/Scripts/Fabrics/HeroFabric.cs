using Characters.Player;
using Services;
using Services.SaveLoad;
using Tools.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabrics
{
    public class HeroFabric
    {
        public struct Ctx
        {
            public AssetProvider AssetProvider;
            public UpdateRunner UpdateRunner;
            public ConfigProvider ConfigProvider;
            public IReadOnlyReactiveProperty<Vector3> UiTouchInput;
        }

        private Ctx _ctx;
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
        }

        public PlayerCharacter SpawnHeroCharacter()
        {
            PlayerCharacter hero = Object.Instantiate(_ctx.AssetProvider.GetPlayerCharacterExample(id: 0));
            hero.SetCtx(new PlayerCharacter.Ctx
            {
                UiTouchInput = _ctx.UiTouchInput,
                UpdateRunner = _ctx.UpdateRunner,
                ConfigProvider = _ctx.ConfigProvider
            });
            return hero;
        }
    }
}