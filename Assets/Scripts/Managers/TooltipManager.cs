using System;
using Controllers;
using Objects;
using UnityEngine;

namespace Managers
{
    public class TooltipManager : Singleton<TooltipManager>
    {
        public TooltipController controller;
        
        /// <summary>
        /// Special singleton initializer method.
        /// </summary>
        public new static void Initialize()
        {
            var prefab = Resources.Load<GameObject>("Prefabs/Managers/TooltipManager");
            if (prefab == null) throw new Exception("Missing TooltipManager prefab!");
            
            var instance = Instantiate(prefab);
            if (instance == null) throw new Exception("Failed to instantiate TooltipManager prefab!");
            
            instance.name = "Managers.TooltipManager (Singleton)";
        }
        
        public void SetTooltipData(TooltipData data) => controller.SetTooltipData(data);
        
        public void ShowTooltip() => controller.ShowTooltip();

        public void HideTooltip()
        {
            if (!IsTooltipActive()) return;
            controller.HideTooltip();
            controller.ClearTooltipData();
        }
        
        private bool IsTooltipActive() => controller.IsTooltipActive();
    }
}