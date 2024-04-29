using Tools.Extensions;
using UnityEngine;

namespace UI
{
    public class EndGame : BaseMonoBehaviour, IUIPanel
    {
        [SerializeField] private GameObject _panel;
        
        public void Show()
        {
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }
    }
}