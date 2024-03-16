using Interfaces;
using Managers;
using UnityEngine;

namespace World.Collectible
{
    public class BasicConsumable : MonoBehaviour, ICollectible
    {
        [SerializeField] private float sanityBoost = 5.0f;
        
        public void Collect()
        {
            var player = GameManager.Instance.localPlayer;
            player.UpdateSanity(sanityBoost);
        }
    }
}