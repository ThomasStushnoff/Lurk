using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private Image staminaMeter;
        [SerializeField] private Image sanityMeter;

        public void UpdateStamina(float value) => staminaMeter.fillAmount = value;
        
        public void UpdateSanity(float value) => sanityMeter.fillAmount = value;
        
        public void HideUI() => gameObject.SetActive(false);
        
        public void ShowUI() => gameObject.SetActive(true);
    }
}