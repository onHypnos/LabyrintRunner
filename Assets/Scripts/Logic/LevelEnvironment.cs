using System.Collections.Generic;
using System.Linq;
using Characters.Enemy;
using Characters.Player;
using Extensions.Reactive;
using Fabrics;
using Services.SaveLoad;
using Tools.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic
{
    public class LevelEnvironment : BaseMonoBehaviour
    {
        [SerializeField] private List<EnemySpawner> _enemySpawners;
        [SerializeField] private HeroSpawner _heroSpawn;
        [SerializeField] private WinEncounter _encounter;
        
        public struct Ctx
        {
            public EnemyFabric EnemyFabric;
            public HeroFabric HeroFabric;
            
            public ReactiveEvent<ReactiveEvent<SaveData.PlayerData>> OnSavePlayerHandler;
            public ReactiveEvent<ReactiveEvent<SaveData.LevelData>> OnSaveLevelHandler;
            
            public ReactiveProperty<float> TimeRemaining;
            public ReactiveProperty<int> TriesAmount;
            public ReactiveEvent PlayerReachWinEncounter;
            public ReactiveProperty<PlayerCharacter> CurrentPlayer;
        }

        private Ctx _ctx;
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.OnSavePlayerHandler.SubscribeWithSkip(SavePlayerData);
            _ctx.OnSaveLevelHandler.SubscribeWithSkip(SaveLevelData);
            _encounter.SetCtx(new WinEncounter.Ctx
            {
                PlayerReachWinEncounter = _ctx.PlayerReachWinEncounter
            });
        }

        public void LoadLevel(SaveData data = null)
        {
            var character = _ctx.HeroFabric.SpawnHeroCharacter();
            _ctx.CurrentPlayer.SetValueAndForceNotify(character);
            _heroSpawn.SetNewCharacter(character);
            if (data != null)
            {
                character.transform.position = data.PlayerDataInstance.Position;
                character.transform.rotation = data.PlayerDataInstance.Rotation;
            }

            foreach (var spawner in _enemySpawners)
            {
                var enemy = _ctx.EnemyFabric.SpawnEnemy(0,spawner.ID, spawner.GetPosition);
                spawner.SetNewEnemy(enemy);

                if (data!= null)
                {
                    var save = data.LevelDataInstance.Enemies.Find(item => item.ID == spawner.ID);
                    if (save != null)
                    {
                        enemy.LoadData(save.Position,save.Rotation);
                    }
                    else
                    {
                        enemy.LoadData(spawner.GetPosition, Quaternion.identity);
                    }
                }
                else
                {
                    enemy.LoadData(spawner.GetPosition, Quaternion.identity);
                }
            }
        }

        private void SavePlayerData(ReactiveEvent<SaveData.PlayerData> data)
        {
            var playerData = _ctx.CurrentPlayer.Value.SavedData();
            playerData.RemainingTime = _ctx.TimeRemaining.Value;
            playerData.Tries = _ctx.TriesAmount.Value;
            data.Notify(playerData);
        }

        private void SaveLevelData(ReactiveEvent<SaveData.LevelData> data)
        {
            var levelData = new SaveData.LevelData();
            foreach (var spawner in _enemySpawners)
            {
                levelData.Enemies.Add(spawner.GetSavedData());
            }
            data.Notify(levelData);
        }
    }
}
