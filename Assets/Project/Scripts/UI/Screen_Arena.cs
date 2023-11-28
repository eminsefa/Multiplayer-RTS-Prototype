using UnityEngine;

namespace Project.Scripts.UI
{
    public class Screen_Arena : ScreenBase
    {
        [SerializeField] private RectTransform m_SelectionBox;

        public void DrawSelectionRect(Vector2 startPos, Vector2 endPos)
        {
            m_SelectionBox.gameObject.SetActive(true);
            
            var width  = endPos.x - startPos.x;
            var height = endPos.y - startPos.y;

            m_SelectionBox.sizeDelta        = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            m_SelectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
        }
        
        public void ClearSelectionRect()
        {
            m_SelectionBox.sizeDelta = Vector2.zero;
            m_SelectionBox.gameObject.SetActive(false);
        }
    }
}