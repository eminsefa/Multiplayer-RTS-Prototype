using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.UI
{
    public class Screen_Lobby : ScreenBase
    {
        public static event Action<string> OnLobbySelectionCompleted;

        [SerializeField] private TMP_InputField m_LobbyName;
        [SerializeField] private Button     m_StartButton;

        private void OnEnable()
        {
            m_StartButton.onClick.AddListener(StartButtonTapped);
        }

        private void StartButtonTapped()
        {
            OnLobbySelectionCompleted?.Invoke(m_LobbyName.text);
        }
    }
}