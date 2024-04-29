using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services.SaveLoad
{
    [Serializable]
    public class SaveData
    {
        public PlayerData PlayerDataInstance;
        public LevelData LevelDataInstance;

        public SaveData(PlayerData playerData, LevelData levelData)
        {
            PlayerDataInstance = playerData;
            LevelDataInstance = levelData;
        }
        
        
        [Serializable]
        public class PlayerData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public float RemainingTime;
            public int Tries;
        }

        [Serializable]
        public class LevelData
        {
            public List<EnemyData> Enemies = new();
        }
        
        [Serializable]
        public class EnemyData
        {
            public int ID;
            public Vector3 Position;
            public Quaternion Rotation;
        }
    }
}