using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.XR.Management;
using System.Collections;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class CanvasControllerCompanion : MonoBehaviour
    {
        public UnityEvent OnTouchpadEnd;
        public Button detectButton;
        public Button addButton;
        public Button deleteButton;

        private CanvasControllerCompanionInputDeviceState deviceState;
        private CanvasControllerCompanionInputDevice inputDevice;

        private Button[] buttons;
        private int currentButtonIndex = 0;
        private Coroutine holdCoroutine;
        private float pressStartTime;
        private bool buttonClicked;

        private void Awake()
        {
            deviceState = new CanvasControllerCompanionInputDeviceState();
            deviceState.trackingState = 1;

            buttons = new Button[] { detectButton, addButton, deleteButton };
            SetButtonColors();
        }

        public void SendTouchpadEvent(int phase, Vector2 position)
        {
            var bit = 1 << 1;
            if (phase != 0)
            {
                deviceState.buttons |= (ushort)bit;

                if (phase == 1)
                {
                    pressStartTime = Time.time;
                    buttonClicked = false;

                    // Start hold detection
                    if (holdCoroutine != null)
                        StopCoroutine(holdCoroutine);
                    holdCoroutine = StartCoroutine(HoldAndClick());
                }
            }
            else
            {
                deviceState.buttons &= (ushort)~bit;
                OnTouchpadEnd?.Invoke();

                // Stop hold detection
                if (holdCoroutine != null)
                {
                    StopCoroutine(holdCoroutine);
                    holdCoroutine = null;
                }

                float holdDuration = Time.time - pressStartTime;

                if (holdDuration < 0.5f && !buttonClicked)
                {
                    CycleButtonSelection();
                }
            }

            deviceState.touchpadPosition.x = position.x;
            deviceState.touchpadPosition.y = position.y;

            InputSystem.QueueStateEvent(InputSystem.GetDevice<CanvasControllerCompanionInputDevice>(), deviceState);
        }

        private void CycleButtonSelection()
        {
            currentButtonIndex = (currentButtonIndex + 1) % buttons.Length;
            SetButtonColors();
        }

        private void SetButtonColors()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                var image = buttons[i].targetGraphic as Image;
                if (image != null)
                {
                    image.color = (i == currentButtonIndex) ? Color.green : Color.white;
                }
            }
        }


        private IEnumerator HoldAndClick()
        {
            yield return new WaitForSeconds(0.5f);
            buttons[currentButtonIndex].onClick.Invoke();
            buttonClicked = true;
        }

        public void SendMenuButtonEvent(int phase)
        {
            var bit = 1 << 0;
            if (phase != 0)
            {
                deviceState.buttons |= (ushort)bit;
            }
            else
            {
                deviceState.buttons &= (ushort)~bit;
            }

            InputSystem.QueueStateEvent(InputSystem.GetDevice<CanvasControllerCompanionInputDevice>(), deviceState);
        }

        public void Quit()
        {
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
