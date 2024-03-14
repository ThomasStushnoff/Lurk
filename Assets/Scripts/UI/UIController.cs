using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Image staminaMeter;
        [SerializeField] private Image sanityMeter;

        public void UpdateStamina(float value) => staminaMeter.fillAmount = value;
        
        public void UpdateSanity(float value) => sanityMeter.fillAmount = value;
    }
}