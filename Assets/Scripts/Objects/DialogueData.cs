using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects
{
    /// <summary>
    /// Represents dialogue data for use in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "Dialogue Data", menuName = "Data/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [FormerlySerializedAs("data")]
        public AudioData audioData;

        public List<string> dialogueContents;
    }
}