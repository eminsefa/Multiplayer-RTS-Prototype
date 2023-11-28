using System;
using Cysharp.Threading.Tasks;
using Fusion;
using Project.Scripts.Managers;
using Project.Scripts.Player;
using UnityEngine;

namespace Project.Scripts.ProjectLevel
{
    public class Level : NetworkBehaviour
    {
        public static event Action OnLevelSet;
        
        public Vector2             Borders => m_Borders;
        
        [SerializeField] private Transform        m_Light;
        [SerializeField] private ControllerBase[] m_Controllers;
        [SerializeField] private Vector2          m_Borders;

        private void OnEnable()
        {
            LevelManager.SetLevel(this);
        }

        private void OnDisable()
        {
            LevelManager.SetLevel(null);
        }

        public async UniTask SetControllers()
        {
            for (var i = 0; i < m_Controllers.Length; i++)
            {
                while (null==m_Controllers[i].Object)
                {
                    await UniTask.Yield();
                }
                m_Controllers[i].RPC_Init(RoomPlayer.Players[i].Object.InputAuthority);
            }

            for (var i = 0; i < RoomPlayer.Players.Count; i++)
            {
                RoomPlayer.Players[i].RPC_SetController(m_Controllers[i]);
            }
        }
        
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_SetLevel()
        {
            var localControllerTr = ControllerBase.LocalController.transform;
            
            var lightRot          = m_Light.rotation;
            lightRot         = Quaternion.Euler(lightRot.eulerAngles.x, lightRot.eulerAngles.y + localControllerTr.eulerAngles.y, 0f);
            m_Light.rotation = lightRot;
            
            OnLevelSet?.Invoke();
        }
        
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
        public void RPC_StartGame()
        {
            GameLauncher.GameState = GameLauncher.eGameState.Arena;
            RoomPlayer.Local.Controller.Object.RequestStateAuthority();
        }
    }
}