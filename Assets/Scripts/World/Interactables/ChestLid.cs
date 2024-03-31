using Entities;
using Interfaces;
using UnityEngine;

namespace World.Interactables
{
    public class ChestLid : MonoBehaviour, IInteractable
    {
        [SerializeField] private Chest parent;
        
        public void BeginInteract(BaseEntity entity) => parent.BeginInteract(entity);
        
        public void EndInteract() => parent.EndInteract();
    }
}