using Entities;
using Entities.Player;
using Interfaces;
using Managers;
using Objects;
using UnityEngine;

namespace World.Interactables
{
    [RequireComponent(typeof(AudioSource))]
    public class Page : MonoBehaviour, IInteractable
    {
        [SerializeField] private PageType type = PageType.Page1;
        [SerializeField] private AudioData _interactSound;
        [SerializeField] private AudioData _endInteractSound;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void BeginInteract(BaseEntity entity)
        {
            PlayerController.EnableCursor();
            PrefabManager.Create(GetPrefabType());
            InputManager.DisableMovementInput();
            _audioSource.PlayOneShot(_interactSound);
        }
        
        public void EndInteract()
        {
            _audioSource.PlayOneShot(_endInteractSound);
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