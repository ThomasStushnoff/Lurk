using Managers;
using UnityEngine;

namespace World
{
    public class BasicRoom : MonoBehaviour
    {
        [TitleHeader("Room Settings")]
        [SerializeField] private RoomLight roomLight;
        [SerializeField] private float roomTime = 100f;
        [SerializeField] private float roomFlickerTime = 80f;
        
        [TitleHeader("Sanity Settings")]
        [SerializeField] private float sanityDrainMultiplier = 10f;
        
        private float _currentTime;
        private bool _isPlayerInRoom;
    
        private void Start() => _currentTime = roomTime;

        private void Update()
        {
            if (!_isPlayerInRoom) return;
        
            _currentTime -= Time.deltaTime;
        
            if (_currentTime <= roomFlickerTime)
            {
                roomLight.StartFlickering();
                if (!roomLight.isFlickering) roomLight.StartFlickering();
                GameManager.Instance.localPlayer.UpdateSanity(Time.deltaTime * sanityDrainMultiplier * -1);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.IsPlayer()) return;
            _isPlayerInRoom = true;
            _currentTime = roomTime;
            roomLight.StopFlickering();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.IsPlayer()) return;
            _isPlayerInRoom = false;
        }
    }
}
