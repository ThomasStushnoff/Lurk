using Interfaces;
using UnityEngine;

namespace World
{
    public class BaseObject : MonoBehaviour, IBaseObject
    {
        public uint runtimeID;
        
        protected virtual void Awake()
        {
            runtimeID = (uint)GetInstanceID();
        }
        
        public uint RuntimeID => runtimeID;
    }
}