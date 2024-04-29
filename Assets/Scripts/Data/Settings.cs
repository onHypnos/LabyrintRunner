using UniRx;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Create Settings", fileName = "Settings", order = 0)]
    public class Settings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("Время появления шторки в кадрах")] private float _curtainShowingTime = 1;
        [SerializeField]
        [Tooltip("Время скрытия шторки в кадрах")] private float _curtainHideTime = 1;
        [SerializeField]
        [Tooltip("Фреймрейт")] private int _applicationTargetFramerate = 60;
        [SerializeField]
        [Tooltip("Время на уровень")] private int _defaultLevelTime = 150;
        [SerializeField]
        [Tooltip("Радиус погони врагов")] private float _enemiesBaseChaseDistance;

        [SerializeField] private float _characterSpeed = 3f;
        [SerializeField] private float _enemyBaseSpeed = 3f;
        public float CurtainShowingTime => _curtainShowingTime;
        public float CurtainHideTime => _curtainHideTime;
        public int ApplicationTargetFramerate => _applicationTargetFramerate;
        
        public float EnemiesBaseChaseDistance => _enemiesBaseChaseDistance;

        public int DefaultLevelTime => _defaultLevelTime;
        public float DefaultCharacterSpeed => _characterSpeed;

        public float DefaultEnemySpeed => _enemyBaseSpeed;
    }
}