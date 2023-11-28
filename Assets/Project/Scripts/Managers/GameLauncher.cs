using Cysharp.Threading.Tasks;
using Fusion;
using Project.Scripts.Essentials;
using Project.Scripts.Player;
using Project.Scripts.ProjectLevel;
using UnityEngine;
using Zenject;

namespace Project.Scripts.Managers
{
    public class GameLauncher : MonoBehaviour
    {
        public enum eGameState
        {
            Launch,
            Lobby,
            Arena
        }

        public static bool       IsDecider;
        public static eGameState GameState = eGameState.Launch;

        [Inject] private UIManager     m_UIManager;
        [Inject] private CameraManager m_CameraManager;
        [Inject] private LevelManager  m_LevelManager;
        [Inject] private NetworkRunner m_NetworkRunner;

        [SerializeField] private RoomPlayer m_RoomPlayer;

        private void OnEnable()
        {
            UIManager.OnLobbySelected      += OnLobbySelected;
            Level.OnLevelSet               += OnLevelSet;
            RoomPlayer.OnPlayerLoadedLevel += OnPlayerLoadedLevel;

            Application.targetFrameRate = GameConfig.Instance.TargetFrameRate;

            LoadLobby();

            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(m_NetworkRunner.gameObject);
        }

        private void OnDisable()
        {
            UIManager.OnLobbySelected      -= OnLobbySelected;
            RoomPlayer.OnPlayerLoadedLevel -= OnPlayerLoadedLevel;
            Level.OnLevelSet               -= OnLevelSet;
        }

        private async void LoadLobby()
        {
            await UniTask.Delay(500);
            m_LevelManager.LoadLobby(OnLobbyLoaded);
        }

        private void OnLobbyLoaded()
        {
            GameState = eGameState.Lobby;
            m_UIManager.SetLobbyScreen();
        }

        private void OnLobbySelected(string lobbyName)
        {
            ConnectLobby(lobbyName);
        }

        private void OnPlayerLoadedLevel(RoomPlayer roomPlayer)
        {
            if (!IsDecider) return;
            for (var i = 0; i < RoomPlayer.Players.Count; i++)
            {
                if (!RoomPlayer.Players[i].IsReady) return;
            }

            StartGame();
        }

        private void OnLevelSet()
        {
            m_CameraManager.InitGameCam();
            m_UIManager.SetArenaScreen();
        }

        private async void ConnectLobby(string lobbyName)
        {
            m_NetworkRunner.ProvideInput = true;
            await m_NetworkRunner.StartGame(new StartGameArgs
                                            {
                                                SceneManager = m_LevelManager,
                                                Scene        = 0,
                                                SessionName  = lobbyName,
                                                GameMode     = GameMode.Shared
                                            });
            m_UIManager.SetConnectText();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            var roomPlayerCount       = RoomPlayer.Players.Count;
            if (!IsDecider) IsDecider = roomPlayerCount == 0;

            if (roomPlayerCount == 1) m_UIManager.SetPreLoadArenaScreen();

            if (IsDecider) Spawn(playerRef);
        }

        private async void Spawn(PlayerRef playerRef)
        {
            RoomPlayer networkPlayerObject = m_NetworkRunner.Spawn(m_RoomPlayer, Vector3.zero, Quaternion.identity, playerRef);

            if (playerRef != m_NetworkRunner.LocalPlayer)
            {
                while (!networkPlayerObject.Object.IsValid)
                {
                    await UniTask.Yield();
                }

                await UniTask.Delay(500);
                m_LevelManager.LoadArena();
            }
        }

        private async void StartGame()
        {
            await LevelManager.CurrentLevel.SetControllers();

            LevelManager.CurrentLevel.RPC_SetLevel();

            await UniTask.Delay(500);

            var roomPlayers = RoomPlayer.Players;
            foreach (var roomPlayer in roomPlayers)
            {
                while (roomPlayer.Controller == null || !roomPlayer.Controller.IsInit)
                {
                    await UniTask.Yield();
                }
            }

            LevelManager.CurrentLevel.RPC_StartGame();
        }
    }
}