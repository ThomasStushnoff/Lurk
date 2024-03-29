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
        public List<AudioEnumMapping> enumMappings = new List<AudioEnumMapping>();
    }

    [Serializable]
    public class AudioGroup
    {
        public string groupName;
        public List<AudioData> audioData = new List<AudioData>();
    }
    
    [Serializable]
    public class AudioEnumMapping
    {
        public string enumName;
        public int enumValue;
    }
}