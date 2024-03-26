using UnityEngine;
using UnityEngine.Events;

namespace World.Environmental
{
    public class Deposit : MonoBehaviour
    {
        [ReadOnly] public bool hasDeposited;
        
        public UnityEvent onDepositMade;
        
        public void DepositItem()
        {
            if (hasDeposited) return;
            
            hasDeposited = true;
            onDepositMade?.Invoke();
        }
    }
}