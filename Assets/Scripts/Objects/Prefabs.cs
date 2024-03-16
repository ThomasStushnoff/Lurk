using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "Prefabs", menuName = "Presets/Prefabs")]
    public class Prefabs : ScriptableObject
    {
        public List<Prefab> prefabs;
    }
}
