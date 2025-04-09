/*
 * Copyright (c) 2023-2024 Qualcomm Technologies, Inc. and/or its subsidiaries.
 * All rights reserved.
 */

using UnityEngine;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class GyroToRotation : MonoBehaviour
    {
        public Camera xrCamera;
        public GameObject controllerRepresentation;

        [Tooltip("Rotation rate reported by the gyro (debugging).")]
        public Vector3 rotationRate = new Vector3(0, 0, 0);

        [Tooltip("Multiplier for adjusting how sensitive the rotation is to device movement.")]
        [Range(0.1f, 10.0f)]
        public float RotationSensitivity = 1.0f; // 1 = default, >1 = more sensitive

        public Vector3 RotationRate
        {
            get => rotationRate;
            set => rotationRate = value;
        }

        private void Awake()
        {
            if (xrCamera == null)
            {
                xrCamera = OriginLocationUtility.GetOriginCamera(true);
            }

            EnableGyro(true);
        }

        private void Start()
        {
            ResetGyro();
        }

        protected void Update()
        {
            if (Input.gyro.enabled)
            {
                rotationRate = Input.gyro.rotationRate;

                // Re-map and invert axes if needed
                float rx = -rotationRate.x;
                float ry = -rotationRate.z;
                float rz = -rotationRate.y;

                // Apply sensitivity multiplier
                rotationRate.x = rx * RotationSensitivity;
                rotationRate.y = ry * RotationSensitivity;
                rotationRate.z = rz * RotationSensitivity;

                controllerRepresentation.transform.Rotate(RotationRate, Space.Self);
            }
        }

        public void EnableGyro(bool isOn)
        {
            Input.gyro.enabled = isOn;
        }

        public void ResetGyro()
        {
            Vector3 forward = xrCamera.transform.forward;
            forward.y = 0;
            controllerRepresentation.transform.forward = forward;
        }
    }
}
