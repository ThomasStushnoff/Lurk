using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Image staminaMeter;
        [SerializeField] private Image sanityMeter;

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
                staminaMeter.fillAmount = 0;
        }

        public void UpdateStamina(float value) => staminaMeter.fillAmount = value;
        
        public void UpdateSanity(float value) => sanityMeter.fillAmount = value;
    }
}