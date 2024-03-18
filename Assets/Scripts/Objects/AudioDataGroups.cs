using System;
using System.Collections.Generic;
using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Represents audio data groups to be used as editor presets.
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Data Group", menuName = "Data/Audio Data Group")]
    public class AudioDataGroups : ScriptableObject
    {
        public List<AudioGroup> groups = new List<AudioGroup>();
    }

    [Serializable]
    public class AudioGroup
    {
        public string groupName;
        public List<AudioData> audioData = new List<AudioData>();
    }
}