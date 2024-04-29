using Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace Services
{
    [CreateAssetMenu(menuName = "Create ConfigProvider", fileName = "ConfigProvider", order = 0)]
    public class ConfigProvider : ScriptableObject
    {
        [SerializeField] private Settings _settings;

        public Settings Settings => _settings;
    }
}