using System;
using Project.Scripts.Essentials;
using Project.Scripts.UI;
using UnityEngine;

namespace Project.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        public enum eScreenType
        {
            Loading,
            Lobby,
            Arena,
        }

        public static event Action<string> OnLobbySelected;

        [SerializeField] private ScreenDictionary m_ScreenDictionary;

        private void OnEnable()
        {
            InputManager.OnInputSelect             += (s, p) => ((Screen_Arena) m_ScreenDictionary[eScreenType.Arena]).DrawSelectionRect(s, p);
            InputManager.OnInputSelectRelease      += (b) => ((Screen_Arena) m_ScreenDictionary[eScreenType.Arena]).ClearSelectionRect();
            Screen_Lobby.OnLobbySelectionCompleted += OnLobbySelectionCompleted;

            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            InputManager.OnInputSelect             -= (s, p) => ((Screen_Arena) m_ScreenDictionary[eScreenType.Arena]).DrawSelectionRect(s, p);
            InputManager.OnInputSelectRelease      -= (b) => ((Screen_Arena) m_ScreenDictionary[eScreenType.Arena]).ClearSelectionRect();
            Screen_Lobby.OnLobbySelectionCompleted -= OnLobbySelectionCompleted;
        }

        public void SetLobbyScreen()
        {
            m_ScreenDictionary[eScreenType.Loading].Close();
            m_ScreenDictionary[eScreenType.Lobby].Open();
        }

        public void SetConnectText()
        {
            ((Screen_Loading) m_ScreenDictionary[eScreenType.Loading]).SetTextOnConnect();
        }

        public void SetPreLoadArenaScreen()
        {
            ((Screen_Loading) m_ScreenDictionary[eScreenType.Loading]).SetPreLoadText();
        }

        public void SetArenaScreen()
        {
            m_ScreenDictionary[eScreenType.Loading].Close();
            m_ScreenDictionary[eScreenType.Arena].Open();
        }

        private void OnLobbySelectionCompleted(string lobbyName)
        {
            m_ScreenDictionary[eScreenType.Lobby].Close();
            m_ScreenDictionary[eScreenType.Loading].Open();
            ((Screen_Loading) m_ScreenDictionary[eScreenType.Loading]).SetTextBeforeConnect();
            OnLobbySelected?.Invoke(lobbyName);
        }
    }

    [Serializable] public class ScreenDictionary : UnitySerializedDictionary<UIManager.eScreenType, ScreenBase> { }
}