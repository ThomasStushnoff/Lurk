using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class PromptManager : Singleton<PromptManager>
    {
        private HUDController _hudController;
        private DialogueController _dialogueController;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (!TryGetControllers())
                Debug.LogWarning("Failed to get controllers.");
            
            _dialogueController.OnDialoguesFinished += AdvanceDialogue;
        }
        
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!TryGetControllers())
                Debug.LogWarning("Failed to get controllers.");
            
            _dialogueController.OnDialoguesFinished += AdvanceDialogue;
        }

        protected override void OnSceneUnloaded(Scene scene)
        {
            // Free pointers.
            _hudController = null;
            _dialogueController = null;
            // Unsubscribe from events.
            if (_dialogueController != null) _dialogueController.OnDialoguesFinished -= AdvanceDialogue;
        }

        private bool TryGetControllers()
        {
            // Find the HUD controller in the scene.
            _hudController = FindObjectOfType<HUDController>(true);
            // Find the dialogue controller in the scene.
            _dialogueController = FindObjectOfType<DialogueController>(true);
            
            return _hudController != null && _dialogueController != null;
        }
        
        /// <summary>
        /// Shows the dialogue prompt.
        /// </summary>
        private void ShowDialoguePrompt()
        {
            // Hide the UI from the HUD controller.
            _hudController.HideUI();
            // Show the dialogue box.
            _dialogueController.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Hides the dialogue prompt.
        /// </summary>
        private void HideDialoguePrompt()
        {
            // Show the UI from the HUD controller.
            _hudController.ShowUI();
            // Hide the dialogue box.
            _dialogueController.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Adds a dialogue prompt to the queue.
        /// </summary>
        /// <param name="dialogue">The next dialogue to add.</param>
        public void AppendDialogue(Dialogue dialogue) => _dialogueController.AddDialogue(dialogue);

        /// <summary>
        /// Displays the first dialogue in the queue on screen.
        /// </summary>
        /// <param name="dialogue">The dialogue to display.</param>
        /// <param name="show">Whether to show the dialogue prompt.</param>
        public void AppendDialogue(Dialogue dialogue, bool show)
        {
            _dialogueController.AddDialogue(dialogue);
            if (show) ShowDialoguePrompt();
            _dialogueController.DisplayNextDialogue();
        }
        
        /// <summary>
        /// Advances the dialogue to the next line.
        /// Places the player back in-game if there isn't another line.
        /// </summary>
        public void AdvanceDialogue()
        {
            // Check if there is another line.
            if (!_dialogueController.HasDialogue())
            {
                // Hide the dialogue prompt.
                HideDialoguePrompt();
            }
            else
            {
                // Advance the dialogue.
                _dialogueController.DisplayNextDialogue();
            }
        }
    }
}