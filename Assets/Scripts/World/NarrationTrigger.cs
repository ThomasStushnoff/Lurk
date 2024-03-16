using System.Linq;
using Managers;
using Objects;
using UI;
using UnityEngine;

namespace World
{
    public class NarrationTrigger : MonoBehaviour
    {
        [TitleHeader("Narration Settings")]
        [SerializeField] private string narratorName;
        [SerializeField] private DialogueData data;
        public DialogueController _controller;
        
        private AudioSource _controllerAudioSource;
        
        private void Start()
        {
            // _controller = FindObjectOfType<DialogueController>();
            _controllerAudioSource = _controller.GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.IsPlayer()) return;
            
            // Play the narration if it exists.
            if (data != null && data.audioData != null)
            {
                _controllerAudioSource.Configure(data.audioData);
                if (!_controllerAudioSource.isPlaying)
                    _controllerAudioSource.Play();
            }
            
            // Queue the dialogue if it exists.
            if (data != null && data.dialogueContents != null && _controller != null)
                data.dialogueContents.Select(content => new Dialogue(narratorName, content))
                    .ToList().ForEach(dialogue => PromptManager.Instance.AppendDialogue(dialogue, true));
        }
    }
}