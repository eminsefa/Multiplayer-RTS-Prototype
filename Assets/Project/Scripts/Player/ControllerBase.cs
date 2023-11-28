using System.Collections.Generic;
using Fusion;
using Project.Scripts.Interfaces;
using Project.Scripts.Managers;
using UnityEngine;

namespace Project.Scripts.Player
{
    public class ControllerBase : NetworkBehaviour
    {
        public static ControllerBase       LocalController;

        public bool IsInit { get; private set; }

        private RaycastHit m_ClickRayCastHit;

        [SerializeField] private List<CharacterBase> m_Characters;

        [Rpc(sources:RpcSources.StateAuthority, targets:RpcTargets.All)]
        public void RPC_Init(PlayerRef playerRef)
        {
            Object.AssignInputAuthority(playerRef);
            IsInit = true;
            
            if(playerRef!=Runner.LocalPlayer) return;
            
            InputManager.OnInputClick         += OnInputClick;
            InputManager.OnInputSelectRelease += OnInputSelect;

            for (var i = 0; i < m_Characters.Count; i++)
            {
                m_Characters[i].Object.RequestStateAuthority();
                m_Characters[i].Object.AssignInputAuthority(playerRef);
            }

            LocalController = this;
        }

        private void OnDisable()
        {
            InputManager.OnInputClick         -= OnInputClick;
            InputManager.OnInputSelectRelease -= OnInputSelect;
        }

        private void OnInputClick(Vector3 currentMousePos)
        {
            var ray = CameraManager.MainCam.ScreenPointToRay(currentMousePos);
            if (!Physics.Raycast(ray, out m_ClickRayCastHit, 100)) return;

            var hitTr = m_ClickRayCastHit.transform;
            if (hitTr.TryGetComponent(out IClickable clickable))
            {
                clickable.Clicked();
                if (hitTr.parent == transform)
                {
                    for (var i = 0; i < m_Characters.Count; i++)
                    {
                        var c = m_Characters[i];
                        c.SetSelected(c.transform == m_ClickRayCastHit.transform);
                    }

                    return;
                }
            }

            var worldPoint = AstarPath.active.GetNearest(m_ClickRayCastHit.point).position;
            worldPoint.y = 0;

            for (var i = 0; i < m_Characters.Count; i++)
            {
                var c = m_Characters[i];
                if (c.IsSelected) c.Move(worldPoint);
            }
        }

        private void OnInputSelect(Bounds bounds)
        {
            for (var i = 0; i < m_Characters.Count; i++)
            {
                var c = m_Characters[i];
                var v = CameraManager.MainCam.WorldToViewportPoint(c.CenterPos);
                c.SetSelected(bounds.Contains(v));
            }
        }
    }
}