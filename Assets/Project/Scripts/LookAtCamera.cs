using UnityEngine;

namespace Project.Scripts
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera m_MainCamera;

        private void Awake()
        {
            m_MainCamera = Camera.main;
        }

        private void Update()
        {
            if (m_MainCamera) SetRot();
        }

        private void SetRot()
        {
            var camRot = m_MainCamera.transform.rotation;
            transform.LookAt(transform.position + camRot * Vector3.forward,
                             camRot * Vector3.up);
        }
    }
}