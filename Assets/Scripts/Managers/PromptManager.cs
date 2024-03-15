using UI;
using UnityEngine;

namespace Managers
{
    public class PromptManager : Singleton<PromptManager>
    {
        [TitleHeader("References")]
        public HUDController hudController;
        public DialogueController dialogueController;
        
        /// <summary>
        /// Shows the dialogue prompt.
        /// </summary>
        private void ShowDialoguePrompt()
        {
            // Hide the UI from the HUD controller.
            hudController.HideUI();
            // Show the dialogue box.
            dialogueController.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Adds a dialogue prompt to the queue.
        /// </summary>
        /// <param name="dialogue">The next dialogue to add.</param>
        public void AppendDialogue(Dialogue dialogue) => dialogueController.Display(dialogue);
        
        /// <summary>
        /// Displays the first dialogue in the queue on screen.
        /// </summary>
        public void ShowDialogue()
        {
            // Check if there is dialogue to show.
            if (!dialogueController.HasDialogue())
                Debug.LogError("No dialogue to show.");
            
            // Show the dialogue.
            ShowDialoguePrompt();
            dialogueController.Display();
        }
        
        /// <summary>
        /// Displays the following dialogue on screen.
        /// </summary>
        /// <param name="speaker">The person speaking.</param>
        /// <param name="content">What the person is saying.</param>
        public void ShowDialogue(string speaker, string content)
        {
            // Show the dialogue.
            ShowDialoguePrompt();
            // Queue the new dialogue.
            dialogueController.Display(speaker, content);
            // Start the dialogue.
            dialogueController.Display();
        }
        
        /// <summary>
        /// Advances the dialogue to the next line.
        /// Places the player back in-game if there isn't another line.
        /// </summary>
        public void AdvanceDialogue()
        {
            // Check if there is another line.
            if (!dialogueController.HasDialogue())
            {
                // Show the UI from the HUD controller.
                hudController.ShowUI();
                // Hide the dialogue box.
                dialogueController.gameObject.SetActive(false);
            }
            else
            {
                // Advance the dialogue.
                dialogueController.Display();
            }
        }
    }
}