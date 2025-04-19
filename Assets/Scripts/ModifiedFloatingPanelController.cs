using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Assertions;

namespace UnityEngine.XR.ARFoundation
{
    public class ModifiedFloatingPanelController : MonoBehaviour
    {
        [Header("Camera & Player Space Settings")]
        public Transform m_Camera;
        public Transform m_PlayerSpacePanel;
        public bool m_KeepPanelInFront = true;

        [Tooltip("Distance in meters the panel should stay in front of the camera.")]
        public float m_DistanceFromCamera = 2.0f;

        [Tooltip("Smoothing time for position interpolation.")]
        public float m_PositionSmoothTime = 0.2f;

        [Tooltip("Rotation speed in degrees per second.")]
        public float m_RotationSpeed = 180f;

        private Vector3 m_PanelVelocity = Vector3.zero;

        protected void Awake()
        {
            if (m_Camera == null)
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                    m_Camera = mainCamera.transform;
                else
                    Debug.LogWarning("No Main Camera found for XR Origin.", this);
            }
        }

        protected void Start()
        {
            // Your existing XR initialization can go here if needed
        }

        protected void LateUpdate()
        {
            if (m_KeepPanelInFront && m_Camera != null && m_PlayerSpacePanel != null)
            {
                UpdatePanelPositionAndRotation();
            }
        }

        void UpdatePanelPositionAndRotation()
        {
            // Target position directly in front of the camera
            Vector3 targetPosition = m_Camera.position + m_Camera.forward * m_DistanceFromCamera;

            // Smoothly move the panel to target position
            m_PlayerSpacePanel.position = Vector3.SmoothDamp(
                m_PlayerSpacePanel.position,
                targetPosition,
                ref m_PanelVelocity,
                m_PositionSmoothTime
            );

            // Smoothly rotate the panel to face the camera
            Quaternion targetRotation = Quaternion.LookRotation(m_PlayerSpacePanel.position - m_Camera.position);
            float maxAngleStep = m_RotationSpeed * Time.deltaTime;
            m_PlayerSpacePanel.rotation = Quaternion.RotateTowards(m_PlayerSpacePanel.rotation, targetRotation, maxAngleStep);
        }
    }
}
