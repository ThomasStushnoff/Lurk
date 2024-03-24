using System;
using Interfaces;
using UnityEngine;

namespace World.Collectible
{
    public class Resource : MonoBehaviour, ICollectible
    {
        public Action OnCollect;
        
        public void Collect()
        {
            OnCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}