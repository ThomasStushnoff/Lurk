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
        [SerializeField] private DialogueController controller;
        
        private AudioSource _controllerAudioSource;
        
        private void Start()
        {
            // _controller = FindObjectOfType<DialogueController>();
            _controllerAudioSource = controller.GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.IsPlayer() && !data && !controller) return;
            Debug.Log("Entered narration trigger.");
            // Queue the dialogue if it exists.
            data.dialogueContents?.Select(content => new Dialogue(narratorName, content))
                .ToList().ForEach(dialogue => PromptManager.Instance.AppendDialogue(dialogue, true));

            // Play the narration if it exists.
            if (!data.audioData) return;
            if (!_controllerAudioSource.isPlaying)
                _controllerAudioSource.ConfigureAndPlay(data.audioData);
        }
    }
}