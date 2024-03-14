using UnityEngine;

namespace World
{
    public class RoomLight : MonoBehaviour
    {
        [SerializeField] private float minFlickerSpeed = 0.05f;
        [SerializeField] private float maxFlickerSpeed = 0.3f;
        [HideInInspector] public bool isFlickering;
        
        private Light _light;
        private float _timer;

        private void Start() => _light = GetComponent<Light>();

        private void Update()
        {
            if (!isFlickering) return;
            
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                _light.enabled = !_light.enabled;
                _timer = Random.Range(minFlickerSpeed, maxFlickerSpeed);
            }
            else
            {
                _light.enabled = true;
            }
        }
        
        public void StartFlickering() => isFlickering = true;

        public void StopFlickering()
        {
            isFlickering = false;
            _light.enabled = true;
        }
    }
}