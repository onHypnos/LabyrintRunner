using System;
using System.Collections.Generic;
using Characters.Enemy;
using Characters.Player;
using UnityEngine;

namespace Services
{
    [CreateAssetMenu(menuName = "Create AssetProvider", fileName = "AssetProvider", order = 0)]
    public class AssetProvider : ScriptableObject
    {
        [SerializeField] private List<PlayerCharacter> _playerCharactersExamples = new();
        [SerializeField] private List<EnemyCharacter> _enemyExamples = new();
        
        public EnemyCharacter GetEnemyExample(int id)
        {
            if (id < _enemyExamples.Count)
            {
                return _enemyExamples[id];
            }
            else
            {
                throw new Exception($"No enemy prefab example for id {id}");
            }
        }

        public PlayerCharacter GetPlayerCharacterExample(int id)
        {
            if (id < _playerCharactersExamples.Count)
            {
                return _playerCharactersExamples[id];
            }
            else
            {
                throw new Exception($"No hero prefab example for id {id}");
            }
        }
    }
}