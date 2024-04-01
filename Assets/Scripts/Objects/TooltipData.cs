using UnityEngine;

namespace Objects
{
    /// <summary>
    /// Represents tooltip data for use in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "Tooltip Data", menuName = "Data/Tooltip Data")]
    public class TooltipData : ScriptableObject
    {
        [Tooltip("The header text to display in the tooltip.")]
        public string header;
        
        [TextArea(10, 10), Tooltip("The content text to display in the tooltip.")]
        public string content;
    }
}