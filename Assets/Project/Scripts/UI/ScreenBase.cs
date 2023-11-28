using UnityEngine;

public class ScreenBase : MonoBehaviour
{
    [SerializeField] protected GameObject m_Panel;
    
    public void Open()
    {
        m_Panel.SetActive(true);
    }

    public void Close()
    {
        m_Panel.SetActive(false);
    }
}
