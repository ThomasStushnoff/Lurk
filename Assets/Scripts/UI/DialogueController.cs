using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public struct Dialogue
    {
        public string Speaker;
        public string Content;
        
        public Dialogue(string speaker, string content)
        {
            Speaker = speaker;
            Content = content;
        }
    }
    
    public class DialogueController : MonoBehaviour
    {
        [TitleHeader("Text References")]
        public TMP_Text speakerText;
        public TMP_Text contentText;
        
        [TitleHeader("Text Settings")]
        [Tooltip("The speed at which the text is typed out. The lower the value, the faster the text is typed.")]
        [SerializeField] private float typingSpeed = 0.05f;
        [SerializeField] private bool isAutoPlay;
        [SerializeField] private bool isReading;
        public event Action OnDialoguesFinished;
        
        private Queue<Dialogue> _dialogues = new Queue<Dialogue>();
        
        public void Awake()
        {
            // Clear text boxes.
            speakerText.text = "";
            contentText.text = "";
        }
        
        /// <summary>
        /// Checks if there is more dialogue to be read.
        /// </summary>
        /// <returns>True if there is more dialogue to be read.</returns>
        public bool HasDialogue() => _dialogues.Count > 0;
        
        /// <summary>
        /// Types out the dialogue over time.
        /// </summary>
        /// <param name="content">The text to display.</param>
        private IEnumerator TypeDialogue(string content)
        {
            isReading = true;
            // Clear the text box.
            contentText.text = "";
            
            foreach (var letter in content.ToCharArray())
            {
                contentText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            isReading = false;
            
            if (_dialogues.Count == 0)
                OnDialoguesFinished?.Invoke();
            
            if (isAutoPlay)
                DisplayNextDialogue();
        }
        
        /// <summary>
        /// Displays the next dialogue in the queue.
        /// </summary>
        public void DisplayNextDialogue()
        {
            if (isReading || _dialogues.Count == 0) return;
            
            var next = _dialogues.Dequeue();
            speakerText.text = next.Speaker;
            StartCoroutine(TypeDialogue(next.Content));
        }
        
        /// <summary>
        /// Pushes the dialogue to the queue.
        /// </summary>
        /// <param name="dialogue">The dialogue to display.</param>
        public void AddDialogue(Dialogue dialogue) => _dialogues.Enqueue(dialogue);
        
        /// <summary>
        /// Pushes the dialogue to the queue.
        /// </summary>
        /// <param name="dialogues">The dialogue to display.</param>
        public void AddDialogues(IEnumerable<Dialogue> dialogues)
        {
            foreach (var item in dialogues)
                _dialogues.Enqueue(item);
        }
    }
}