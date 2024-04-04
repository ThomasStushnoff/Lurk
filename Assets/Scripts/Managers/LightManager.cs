using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using World;

namespace Managers
{
    public class LightManager : Singleton<LightManager>
    {
        // private List<Light> _lights = new List<Light>();
        private HashSet<SanityLight> _sanityLights = new HashSet<SanityLight>();
        
        // private void Start() => FindObjectsOfType<Light>(true).ToList().ForEach(it => _lights.Add(it));
        //
        // public void RegisterLight(Light l)
        // {
        //     if (!_lights.Contains(l))
        //         _lights.Add(l);
        // }
        //
        // public void UnregisterLight(Light l)
        // {
        //     if (_lights.Contains(l))
        //         _lights.Remove(l);
        // }

        public void RegisterSanityLight(SanityLight l)
        {
            if (!_sanityLights.Contains(l))
                _sanityLights.Add(l);
        }
        
        public void UnregisterSanityLight(SanityLight l)
        {
            if (_sanityLights.Contains(l))
                _sanityLights.Remove(l);
        }
        
        public bool HasSanityLights() => _sanityLights.Count > 0;
    }
}