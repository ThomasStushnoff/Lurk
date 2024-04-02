using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [ExecuteInEditMode]
    public class Tooltip : MonoBehaviour
    {
        public TMP_Text headerText;
        public TMP_Text contentText;
        public int characterWrapLimit;
        
        private LayoutElement _layoutElement;
        
        private void Awake() => _layoutElement = GetComponent<LayoutElement>();
        
        private void Update()
        {
            if (string.IsNullOrEmpty(contentText.text)) return;
            
            var headerLength = headerText.text.Length;
            var contentLength = contentText.text.Length;
            _layoutElement.enabled = contentLength > characterWrapLimit || headerLength > characterWrapLimit;
        }
    }
}