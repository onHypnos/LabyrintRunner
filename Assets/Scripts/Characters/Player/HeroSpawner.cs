using Tools.Extensions;
using UnityEngine;

namespace Characters.Player
{
    public class HeroSpawner : BaseMonoBehaviour
    {
        [SerializeField] private PlayerCharacter _playerCharacter;

        public void SetNewCharacter(PlayerCharacter playerCharacter)
        {
            if (_playerCharacter)
            {
                Destroy(_playerCharacter.gameObject);
            }

            _playerCharacter = playerCharacter;
            _playerCharacter.transform.position = transform.position;
        }
    }
}