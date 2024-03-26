using Audio;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
namespace World.Environmental
{
    public class Door : BaseObject
    {
        [TitleHeader("Door Settings")]
        [SerializeField] private Vector3 openRotation = new Vector3(0, 90, 0);
        [SerializeField] private Vector3 closedRotation = new Vector3(0, 0, 0);
        [SerializeField] private float speed = 2.0f;
        [TitleHeader("Audio Settings")]
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx openSoundFx;
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx closeSoundFx;
        [SerializeField, CanBeNull] private AudioDataEnumSoundFx slamSoundFx;

        public UnityEvent onOpen;
        public UnityEvent onClose;
        public UnityEvent onSlam;
        
        private bool _isOpening;
        
        private void Start()
        {
            onOpen.AddListener(Open);
            onClose.AddListener(Close);
            onSlam.AddListener(Slam);
        }
        
        private void OnDestroy()
        {
            onOpen.RemoveAllListeners();
            onClose.RemoveAllListeners();
            onSlam.RemoveAllListeners();
        }
        
        private void Open()
        {
            var target = Quaternion.Euler(openRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
            if (openSoundFx is not AudioDataEnumSoundFx.None) audioSource.PlaySoundFx(openSoundFx);
        }

        private void Close()
        {
            var target = Quaternion.Euler(closedRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * Time.deltaTime);
            if (closeSoundFx is not AudioDataEnumSoundFx.None) audioSource.PlaySoundFx(closeSoundFx);
        }
        
        private void Slam()
        {
            var target = Quaternion.Euler(closedRotation);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, speed * 2.0f * Time.deltaTime);
            if (slamSoundFx is not AudioDataEnumSoundFx.None) audioSource.PlaySoundFx(slamSoundFx);
        }
    }
}