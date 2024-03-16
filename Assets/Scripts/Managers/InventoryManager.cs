using System;
using System.Collections.Generic;
using Objects;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Basic inventory holder.
    /// </summary>
    [Serializable]
    public class Inventory
    {
        public List<ItemStack> consumables = new List<ItemStack>();
        public List<ItemStack> nonConsumables = new List<ItemStack>();
        public int maxCapacity = 20;
        
        public bool AddItem(ItemStack item)
        {
            if (GetTotalItemCount() >= maxCapacity)
            {
                Debug.Log("Inventory is full.");
                return false;
            }
            
            switch (item.data.type)
            {
                case ItemType.Consumable:
                    consumables.Add(item);
                    break;
                case ItemType.NonConsumable:
                    nonConsumables.Add(item);
                    break;
            }
            
            return true;
        }
        
        public int GetTotalItemCount() => consumables.Count + nonConsumables.Count;
    }
    
    public class InventoryManager : Singleton<InventoryManager>
    {
        public Inventory inventory = new Inventory();
        
        public void AddItem(Item item)
        {
            var itemStack = item.New();
            inventory.AddItem(itemStack);
            // Update Inventory UI.
        }
        
        public void UseItem(ItemStack item)
        {
            if (item.data.type == ItemType.Consumable)
            {
                // TODO: Add more functionality here.
                // Update sanity, stamina, etc.
                inventory.consumables.Remove(item);
                // Update Inventory UI.
            }
            
        }
    }
}