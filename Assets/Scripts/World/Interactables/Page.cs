using Audio;
using Controllers;
using Entities;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using UnityEngine;
using World.Environmental;

namespace World.Interactables
{
    [RequireComponent(typeof(AudioSource), typeof(BoxCollider))]
    public class Page : MonoBehaviour, IInteractable
    {
        [SerializeField] private PageType type = PageType.Page1;
        [SerializeField] private AudioDataEnumSoundFx interactSound;
        [SerializeField] private AudioDataEnumSoundFx endInteractSound;
        [Tooltip("Optional. If you want to trigger an event after you read and put the page down.")]
        [SerializeField, CanBeNull] private LightBulb lightBulb;
        
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        public void BeginInteract(BaseEntity entity)
        {
            GameStateManager.Instance.IsPuzzleActive = true;
            var pageUI = PrefabManager.Create<PageUIController>(GetPrefabType());
            if (lightBulb != null) pageUI.lightBulb = lightBulb;
            InputManager.DisableMovementInput();
            InputManager.DisableInteractInput();
            _audioSource.PlayOneShot(interactSound);
        }
        
        public void EndInteract()
        {
            _audioSource.PlayOneShot(endInteractSound);
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