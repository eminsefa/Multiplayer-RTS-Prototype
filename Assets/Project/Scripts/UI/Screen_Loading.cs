using UnityEngine;

namespace Project.Scripts.UI
{
    public class Screen_Loading : ScreenBase
    {
        [SerializeField] private GameObject m_LoadingText;
        [SerializeField] private GameObject m_ConnectingText;
        [SerializeField] private GameObject m_WaitingText;

        public void SetTextBeforeConnect()
        {
            m_LoadingText.SetActive(false);
            m_ConnectingText.SetActive(true);
        }
        
        public void SetTextOnConnect()
        {
            m_LoadingText.SetActive(false);
            m_ConnectingText.SetActive(false);
            m_WaitingText.SetActive(true);
        }

        public void SetPreLoadText()
        {
            m_LoadingText.SetActive(true);
            m_ConnectingText.SetActive(false);
            m_WaitingText.SetActive(false);
        }
    }
}