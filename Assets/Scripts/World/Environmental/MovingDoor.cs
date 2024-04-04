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
        [SerializeField] private bool testOpenDoor;

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

        private void Update()
        {
            if (testOpenDoor && !_isOpen)
            {
                StartCoroutine(OpenDoor());
            }
            else if (!testOpenDoor && _isOpen)
            {
                StartCoroutine(CloseDoor());
            }
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
                var openPos = transform.localPosition.OverrideNonZero(openLocalPosition);
                transform.localPosition = Vector3.Lerp(_closedPosition, openPos, t / openTime);
                yield return null;
            }
        }

        private IEnumerator CloseDoor()
        {
            _audioSource.PlayOneShot(openSoundFx, true);
            var t = 0.0f;
            var startingPos = transform.localPosition;
            while (t < openTime)
            {
                t += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(startingPos, _closedPosition, t / openTime);
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 direction = openLocalPosition - _closedPosition;
            Vector3 start = transform.position;
            Vector3 end = start + direction;

            Gizmos.DrawLine(start, end);

            float arrowHeadLength = 0.25f;
            float arrowHeadAngle = 20.0f;

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(end, right * arrowHeadLength);
            Gizmos.DrawRay(end, left * arrowHeadLength);
        }
    }
}