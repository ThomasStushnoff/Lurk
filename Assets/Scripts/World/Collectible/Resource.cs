using Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace World.Collectible
{
    public class Resource : MonoBehaviour, ICollectible
    {
        public UnityEvent onCollect;
        
        public void Collect()
        {
            onCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}