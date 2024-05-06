using Characters.Enemy;
using Characters.Player;
using Extensions.Reactive;
using Services;
using UniRx;
using UnityEngine;

namespace Fabrics
{
    public class EnemyFabric
    {
        public struct Ctx
        {
            public AssetProvider AssetProvider;
            public UpdateRunner UpdateRunner;
            public ConfigProvider ConfigProvider;
            public ReactiveEvent OnLoseGame;
            public ReactiveProperty<PlayerCharacter> CurrentPlayerCharacter;
        }

        private Ctx _ctx;
        
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
        }

        public EnemyCharacter SpawnEnemy(int typeId, int spawnerID, Vector3 spawnPosition)
        {
            var enemy = Object.Instantiate(_ctx.AssetProvider.GetEnemyExample(typeId), spawnPosition, Quaternion.identity);
            enemy.SetCtx(new EnemyCharacter.Ctx
            {
                OnEnemyReachTarget = _ctx.OnLoseGame,
                ChaseRange = _ctx.ConfigProvider.Settings.EnemiesBaseChaseDistance,
                ID = spawnerID,
                CurrentPlayerCharacter = _ctx.CurrentPlayerCharacter,
                UpdateRunner = _ctx.UpdateRunner,
                BaseEnemySpeed = _ctx.ConfigProvider.Settings.DefaultEnemySpeed,
            });
            return enemy;
        }

        
    }
}