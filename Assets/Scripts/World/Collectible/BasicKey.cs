using Interfaces;
using Managers;
using Objects;
using UnityEngine;

namespace World.Collectible
{
    public class BasicKey : MonoBehaviour, ICollectible
    {
        // Unique identifier for the collectible.
        public const string Uid = "BasicKey001";
        
        public void Collect()
        {
            var newItem = new ItemStack
            {
                data = new Item
                {
                    identifier = Uid,
                    itemName = "Basic Key",
                    type = ItemType.NonConsumable,
                }
            };
            
            var added = InventoryManager.Instance.inventory.AddItem(newItem);
            if (added)
            {
                Debug.Log("Collected Basic Key.");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory is full.");
            }
        }
    }
}