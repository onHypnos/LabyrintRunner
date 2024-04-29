using Services.SaveLoad;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Enemy
{
    public class EnemySpawner : BaseMonoBehaviour
    {
        [SerializeField] private int _id;
        private EnemyCharacter _character;
        
        
        public Vector3 GetPosition => transform.position;
        public int ID => _id;

        public void SetNewEnemy(EnemyCharacter enemy)
        {
            if(_character)
                Destroy(_character.gameObject);
            _character = enemy;
        }

        public SaveData.EnemyData GetSavedData()
        {
            var enemyData = _character.SaveData();
            
            return enemyData;
        }
    }
}