using System;
using System.Collections.Generic;
using Fusion;
using Project.Scripts.Managers;
using Sirenix.OdinInspector;

namespace Project.Scripts.Player
{
    public class RoomPlayer : NetworkBehaviour
    {
        public static event Action<RoomPlayer> OnPlayerLoadedLevel;

        public static List<RoomPlayer> Players = new List<RoomPlayer>();
        public static RoomPlayer       Local;

        public ControllerBase Controller { get; private set; }

        public bool IsReady { get; private set; }

        public override void Spawned()
        {
            base.Spawned();

            Players.Add(this);

            if (Object.HasInputAuthority)
            {
                Local = this;

                LevelManager.OnLevelPreLoaded += RPC_OnPlayerLoadedArena;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void OnDisable()
        {
            Players.Remove(this);
            LevelManager.OnLevelPreLoaded -= RPC_OnPlayerLoadedArena;
        }

        [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.All)]
        public void RPC_OnPlayerLoadedArena()
        {
            IsReady = true;
            OnPlayerLoadedLevel?.Invoke(this);
        }

        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_SetController(ControllerBase controller)
        {
            Controller = controller;
            if (!HasInputAuthority) return;
            
            Object.RequestStateAuthority();
        }

        [Button]
        private void test()
        {
            Controller.Object.RequestStateAuthority();
        }
    }
}