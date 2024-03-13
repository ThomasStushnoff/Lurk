using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Objects
{
    [CreateAssetMenu(fileName = "Prefabs", menuName = "Objects/Prefabs")]
    public class Prefabs : ScriptableObject
    {
        public List<Prefab> prefabs;
    }
}
