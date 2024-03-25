using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace World.Environmental
{
    public class MovingDoor : MonoBehaviour
    {
        [SerializeField] private Vector3 openLocalPosition;
        [SerializeField] private float openSpeed = 2.0f;
        [SerializeField] private List<Deposit> deposits;
        
        private Vector3 _closedPosition;
        private bool _isOpen;

        private void Start()
        {
            _closedPosition = transform.localPosition;
            
            foreach (var deposit in deposits)
                deposit.OnDepositMade += CheckDeposits;
        }

        private void OnDestroy()
        {
            foreach (var deposit in deposits)
                deposit.OnDepositMade -= CheckDeposits;
        }

        private void CheckDeposits()
        {
            if (deposits.Any(deposit => !deposit.hasDeposited)) return;

            if (!_isOpen) StartCoroutine(OpenDoor());
        }
        
        private IEnumerator OpenDoor()
        {
            _isOpen = true;
            var t = 0.0f;
            var openTime = 2.0f;
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