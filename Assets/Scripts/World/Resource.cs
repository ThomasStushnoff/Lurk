using System;
using Interfaces;
using UnityEngine;

namespace World
{
    public class Resource : MonoBehaviour, ICollectible
    {
        public static Action OnCollect;
        
        public void Collect()
        {
            OnCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}