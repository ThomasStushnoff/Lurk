using System;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Enumerator for all item types.
    /// </summary>
    public enum ItemType
    {
        Consumable,
        NonConsumable,
    }
    
    /// <summary>
    /// Represents an item's data.
    /// </summary>
    [CreateAssetMenu(fileName = "Item Data", menuName = "Presets/Item")]
    public class Item : ScriptableObject
    {
        [TitleHeader("Attributes")]
        public string identifier;
        public string itemName;
        public ItemType type;
        public Sprite sprite;
        public GameObject prefab;
    
        /// <summary>
        /// Returns the item's sprite.
        /// </summary>
        /// <returns>The sprite object.</returns>
        public Sprite GetSprite() => sprite;
    
        /// <summary>
        /// Creates a new instance of this item.
        /// </summary>
        /// <returns>A new item stack instance.</returns>
        public ItemStack New()
        {
            return new ItemStack
            {
                // Item Data
                data = this,
                quantity = 1
            };
        }
    }
    
    /// <summary>
    /// Represents an item in an inventory.
    /// </summary>
    [Serializable]
    public class ItemStack
    {
        public Item data;
        public int quantity;
    }
}