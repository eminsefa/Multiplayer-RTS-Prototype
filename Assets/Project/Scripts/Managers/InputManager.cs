using System;
using Project.Scripts.Essentials;
using UnityEngine;

namespace Project.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static event Action<Vector3>          OnInputClick;
        public static event Action<Bounds>           OnInputSelectRelease;
        public static event Action<Vector2, Vector2> OnInputSelect;

        public static event Action<Vector2> OnDragCameraMove;

        private InputVariables m_InputVars;

        private Vector3 m_StartMousePos;
        private Rect    m_SelectionRect;
        private bool    m_IsSelecting = false;

        private void OnEnable()
        {
            m_InputVars = GameConfig.Instance.Input;
            
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if(GameLauncher.GameState!=GameLauncher.eGameState.Arena) return;
            HandleInputDrag();
        }

        private void HandleInputDrag()
        {
            Vector3 currentMousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0)) m_StartMousePos = currentMousePos;

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(1)) m_StartMousePos = currentMousePos;
            if (Input.GetMouseButton(0)) HandleDrawSelectionRect(m_StartMousePos, currentMousePos);
            if (Input.GetMouseButton(1)) HandleDragCameraMovement(m_StartMousePos, currentMousePos);
            if (Input.GetMouseButtonUp(0))
            {
                if (m_IsSelecting) ReleaseSelection(m_StartMousePos, currentMousePos);
                else OnInputClick?.Invoke(currentMousePos);
            }
#else
            if (Input.GetMouseButton(0))
            {
                if (Input.touchCount == 1) HandleDrawSelectionRect(m_StartMousePos, currentMousePos);
                else HandleDragCameraMovement(m_StartMousePos, currentMousePos);
            }
             if (Input.GetMouseButtonUp(0))
            {
                if (m_IsSelecting) ReleaseSelection(m_StartMousePos, currentMousePos);
                else if (Input.touchCount == 1)OnInputClick?.Invoke(currentMousePos);
            }
#endif
        }

        private void HandleDrawSelectionRect(Vector2 startPos, Vector2 endPos)
        {
            if ((endPos - startPos).sqrMagnitude < m_InputVars.DrawSelectionBoxThresholdSq) return;

            m_IsSelecting = true;
            OnInputSelect?.Invoke(startPos, endPos);
        }

        private void ReleaseSelection(Vector2 startPos, Vector2 endPos)
        {
            m_IsSelecting = false;

            var v1 = CameraManager.MainCam.ScreenToViewportPoint(startPos);
            var v2 = CameraManager.MainCam.ScreenToViewportPoint(endPos);

            var min = Vector3.Min(v1, v2);
            var max = Vector3.Max(v1, v2);

            min.z = CameraManager.MainCam.nearClipPlane;
            max.z = CameraManager.MainCam.farClipPlane;

            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            OnInputSelectRelease?.Invoke(bounds);
        }

        private void HandleDragCameraMovement(Vector2 startPos, Vector2 endPos)
        {
            if (m_IsSelecting) ReleaseSelection(startPos, endPos);

            OnDragCameraMove?.Invoke(endPos - startPos);
            m_StartMousePos = endPos;
        }
    }
}