using Managers;
using Objects;
using UI;
using UnityEngine;

namespace Controllers
{
    public class TooltipController : MonoBehaviour
    {
        public Tooltip tooltip;

        private void Awake()
        {
            if (TooltipManager.Instance.controller == null || TooltipManager.Instance.controller != this)
                TooltipManager.Instance.controller = this;
        }
        
        private void Start() => HideTooltip();
        
        public void SetTooltipData(TooltipData data)
        {
            tooltip.headerText.text = data.header;
            tooltip.contentText.text = data.content;
        }
        
        public void ClearTooltipData()
        {
            tooltip.headerText.text = string.Empty;
            tooltip.contentText.text = string.Empty;
        }
        
        public void ShowTooltip() => tooltip.gameObject.SetActive(true);
        
        public void HideTooltip() => tooltip.gameObject.SetActive(false);
        
        public bool IsTooltipActive() => tooltip.headerText.gameObject.activeSelf && tooltip.contentText.gameObject.activeSelf;
    }
}