using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Project.Scripts.ProjectLevel;
using UnityEngine.SceneManagement;

namespace Project.Scripts.Managers
{
    public class LevelManager : NetworkSceneManagerBase
    {
        public static event Action OnLevelPreLoaded;

        public static Level CurrentLevel;

        private const int ARENA_SCENE = 2;
        private const int LOBBY_SCENE = 1;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async void LoadLobby(Action onLobbyLoaded)
        {
            await LoadScene(LOBBY_SCENE);
            onLobbyLoaded?.Invoke();
        }

        public void LoadArena()
        {
            Runner.SetActiveScene(ARENA_SCENE);
        }

        private async UniTask LoadScene(int index)
        {
            await SceneManager.LoadSceneAsync(index);
        }

        protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
        {
            List<NetworkObject> sceneObjects = new List<NetworkObject>();

            if (newScene >= LOBBY_SCENE)
            {
                yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
                Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
                sceneObjects = FindNetworkObjects(loadedScene, disable: false);
            }

            finished(sceneObjects);
            yield return null;
            if (newScene <= LOBBY_SCENE) yield break;

            OnLevelPreLoaded?.Invoke();
        }

        public static void SetLevel(Level level)
        {
            CurrentLevel = level;
        }
    }
}