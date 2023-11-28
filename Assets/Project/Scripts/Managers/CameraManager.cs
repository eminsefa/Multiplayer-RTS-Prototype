using Cinemachine;
using Project.Scripts.Essentials;
using Project.Scripts.Player;
using UnityEngine;

namespace Project.Scripts.Managers
{
    public class CameraManager : MonoBehaviour
    {
        public static Camera MainCam;

        private Vector3 m_Forward;
        private Vector3 m_ClampedPos;
        private Vector3 m_LerpedPos;

        private Vector3         m_CameraTargetPosition;
        private CameraVariables m_CameraVars;

        [SerializeField] private Camera                   m_MainCam;
        [SerializeField] private CinemachineVirtualCamera m_GameCam;

        private void Awake()
        {
            MainCam = m_MainCam;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            m_CameraVars = GameConfig.Instance.Camera;

            InputManager.OnDragCameraMove += OnDragToMoveCamera;
        }

        private void OnDisable()
        {
            InputManager.OnDragCameraMove -= OnDragToMoveCamera;
        }

        public void InitGameCam()
        {
            var localControllerTr = ControllerBase.LocalController.transform;

            var gameCamTr = m_GameCam.transform;
            gameCamTr.position = localControllerTr.position + Vector3.up * 20 - localControllerTr.forward * 10;
            gameCamTr.rotation = Quaternion.Euler(gameCamTr.rotation.eulerAngles.x, localControllerTr.rotation.eulerAngles.y, 0f);

            m_CameraTargetPosition = gameCamTr.position;
        }

        private void OnDragToMoveCamera(Vector2 dragDelta)
        {
            var camTr = m_GameCam.transform;

            var forward = camTr.forward;
            forward.y = 0;

            var right = camTr.right;
            right.y = 0;

            m_CameraTargetPosition -= dragDelta.y * m_CameraVars.MoveSensitivity * forward
                                    + dragDelta.x * m_CameraVars.MoveSensitivity * right;
        }

        private void Update()
        {
            if (GameLauncher.GameState != GameLauncher.eGameState.Arena) return;
            HandleCamMovement();
        }

        private void HandleCamMovement()
        {
            var levelBorders = LevelManager.CurrentLevel.Borders;

            m_LerpedPos = Vector3.Lerp(m_GameCam.transform.position, m_CameraTargetPosition, m_CameraVars.DragSpeed * Time.deltaTime);

            //Prevents stuck situations
            if (Mathf.Abs(m_LerpedPos.x / (levelBorders.x * 1.25f)) > 0.999f || Mathf.Abs(m_LerpedPos.z / (levelBorders.y * 1.25f)) > 0.999f)
            {
                m_LerpedPos = m_GameCam.transform.position;
            }
            else
            {
                m_GameCam.transform.position = m_LerpedPos;
            }
        }
    }
}