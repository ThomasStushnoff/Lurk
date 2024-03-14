using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class LightManager : Singleton<LightManager>
    {
        private List<Light> _lights = new List<Light>();

        private void Start() => FindObjectsOfType<Light>().ToList().ForEach(it => _lights.Add(it));
        
        // public void RegisterLight(Light light)
        // {
        //     if (!_lights.Contains(light))
        //     {
        //         _lights.Add(light);
        //     }
        // }
        //
        // public void UnregisterLight(Light light)
        // {
        //     if (_lights.Contains(light))
        //     {
        //         _lights.Remove(light);
        //     }
        // }
    }
}