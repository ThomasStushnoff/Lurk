using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using UnityEngine;

namespace World.Environmental
{
    [RequireComponent(typeof(AudioSource))]
    public class MovingDoor : MonoBehaviour
    {
        [SerializeField] private Vector3 openLocalPosition;
        [SerializeField] private float openTime = 2.0f;
        [SerializeField] private List<Deposit> deposits;
        [SerializeField] private AudioDataEnumSoundFx openSoundFx;
        
        private AudioSource _audioSource;
        private Vector3 _closedPosition;
        private bool _isOpen;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _closedPosition = transform.localPosition;
            
            foreach (var deposit in deposits)
                deposit.onDepositMade.AddListener(CheckDeposits);
        }

        private void OnDestroy()
        {
            foreach (var deposit in deposits)
                deposit.onDepositMade.RemoveListener(CheckDeposits);
        }

        private void CheckDeposits()
        {
            if (deposits.Any(deposit => !deposit.hasDeposited)) return;

            if (!_isOpen) StartCoroutine(OpenDoor());
        }
        
        private IEnumerator OpenDoor()
        {
            _audioSource.PlayOneShot(openSoundFx, true);
            _isOpen = true;
            var t = 0.0f;
            while (t < openTime)
            {
                t += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(_closedPosition, openLocalPosition, t / openTime);
                yield return null;
            }
            
            transform.localPosition = openLocalPosition;
        }
    }
}