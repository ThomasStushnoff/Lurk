using Entities;
using Entities.Player;
using Interfaces;
using Managers;
using UnityEngine;

namespace World.Interactables
{
    public class Page : MonoBehaviour, IInteractable
    {
        [SerializeField] private PageType type = PageType.Page1;
        
        public void BeginInteract(BaseEntity entity)
        {
            PlayerController.EnableCursor();
            PrefabManager.Create(GetPrefabType());
            InputManager.DisableMovementInput();
        }
        
        public void EndInteract()
        {
            throw new System.NotImplementedException();
        }

        private PrefabType GetPrefabType()
        {
            return type switch
            {
                PageType.Page1 => PrefabType.Page1,
                PageType.Page2 => PrefabType.Page2,
                _ => PrefabType.Page1
            };
        }
    }

    public enum PageType
    {
        Page1,
        Page2,
    }
}