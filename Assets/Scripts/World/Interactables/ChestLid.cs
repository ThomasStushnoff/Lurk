using Entities;
using Interfaces;
using UnityEngine;

namespace World.Interactables
{
    public class ChestLid : BaseObject, IInteractable
    {
        [SerializeField] private Chest parent;

        private void Start() => tooltipData = parent.tooltipData;

        public void BeginInteract(BaseEntity entity) => parent.BeginInteract(entity);
        
        public void EndInteract() => parent.EndInteract();
    }
}