using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace UI
{
    public struct Dialogue
    {
        public string Speaker;
        public string Content;
    }
    
    public class DialogueController : MonoBehaviour
    {
        [TitleHeader("Text References")]
        public TMP_Text speaker;
        public TMP_Text content;
        
        [SerializeField] private bool isReading;
        
        private readonly Queue<Dialogue> _dialogue = new();
        
        public void Awake()
        {
            // Clear text boxes.
            speaker.text = "";
            content.text = "";
        }

        private void Update()
        {
            if (isReading) return;
            isReading = true;
            
            if (_dialogue.Count == 0) return;
            var next = _dialogue.Dequeue();
            speaker.text = next.Speaker;
            content.text = next.Content;
            // TODO: Display the text over time.
        }
        
        /// <summary>
        /// Checks if there is more dialogue to be read.
        /// </summary>
        /// <returns>True if there is more dialogue to be read.</returns>
        public bool HasDialogue() => _dialogue.Count > 0;

        /// <summary>
        /// Starts the dialogue reader.
        /// </summary>
        public void Display()
        {
            // TODO: Employ a better system.
            if (_dialogue.Count == 0) return;
            isReading = false;
        }
        
        /// <summary>
        /// Displays the text content on screen.
        /// </summary>
        /// <param name="speaking">The speaker speaking.</param>
        /// <param name="text">The words the speaker is speaking.</param>
        public void Display(string speaking, string text)
        {
            speaker.text = speaking;
            content.text = text;
        }
        
        /// <summary>
        /// Pushes the dialogue to the queue.
        /// </summary>
        /// <param name="dialogue">The dialogue to display.</param>
        public void Display(Dialogue dialogue)
        {
            _dialogue.Enqueue(dialogue);
        }
        
        /// <summary>
        /// Pushes the dialogue to the queue.
        /// </summary>
        /// <param name="dialogue">The dialogue to display.</param>
        public void Display(IEnumerable<Dialogue> dialogue)
        {
            foreach (var item in dialogue)
                _dialogue.Enqueue(item);
        }
    }
}