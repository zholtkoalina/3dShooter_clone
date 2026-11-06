using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

namespace AutomationTests.TestFramework.Shared
{
    public static class TestExtensions
    {
        /// <summary>Clicks the UI Toolkit button</summary>
        public static IEnumerator ClickButton(this Button button)
        {
            if (button == null)
                throw new System.ArgumentNullException(nameof(button), "UI Toolkit button is null.");

            // Let UI Toolkit finish layout before reading the button bounds.
            yield return null;

            var root = button.panel.visualTree;
            var rootWidth = root.resolvedStyle.width;
            var rootHeight = root.resolvedStyle.height;
            if (rootWidth <= 0f)
                throw new System.InvalidOperationException("UI panel has no width yet.");
            if (rootHeight <= 0f)
                throw new System.InvalidOperationException("UI panel has no height yet.");

            var buttonCenter = button.worldBound.center;
            var screenPosition = new Vector2(buttonCenter.x * Screen.width / rootWidth,
                Screen.height - buttonCenter.y * Screen.height / rootHeight);

            var wasClicked = false;
            System.Action clickProbe = () => wasClicked = true;
            button.clicked += clickProbe;
            try
            {
                yield return ClickAt(screenPosition);
                if (!wasClicked)
                    throw new System.InvalidOperationException(
                        $"Pointer did not click UI Toolkit button at {screenPosition}.");
            }
            finally
            {
                button.clicked -= clickProbe;
            }
        }

        /// <summary>Clicks a standard Unity UI Button on the target GameObject or its children.</summary>
        public static IEnumerator ClickButton(this GameObject target)
        {
            if (target == null)
                throw new System.ArgumentNullException(nameof(target), "Button target GameObject is null.");

            var unityButton = target.GetComponent<UnityEngine.UI.Button>() ??
                              target.GetComponentInChildren<UnityEngine.UI.Button>();
            if (unityButton == null)
                throw new System.InvalidOperationException($"'{target.name}' has no Unity UI Button component.");

            var canvas = unityButton.GetComponentInParent<Canvas>();
            var eventCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
            var unityButtonPosition = RectTransformUtility.WorldToScreenPoint(
                eventCamera, unityButton.transform.position);

            var wasUnityButtonClicked = false;
            UnityAction clickListener = () => wasUnityButtonClicked = true;
            unityButton.onClick.AddListener(clickListener);
            try
            {
                yield return ClickAt(unityButtonPosition);
                if (!wasUnityButtonClicked)
                    throw new System.InvalidOperationException(
                        $"Pointer did not click Unity UI Button '{unityButton.name}' at {unityButtonPosition}.");
            }
            finally
            {
                unityButton.onClick.RemoveListener(clickListener);
            }
        }

        private static IEnumerator ClickAt(Vector2 screenPosition)
        {
            // A fresh virtual mouse per click keeps repeated helper calls independent.
            var mouse = InputSystem.AddDevice<Mouse>();
            try
            {
                // Send move, press, and release on separate frames for the UI input module.
                InputSystem.QueueStateEvent(mouse, new MouseState { position = screenPosition });
                yield return null;

                // Left button down
                InputSystem.QueueStateEvent(mouse, new MouseState
                {
                    position = screenPosition,
                    buttons = 1
                });
                yield return null;

                // Left button in default position (up)
                InputSystem.QueueStateEvent(mouse, new MouseState { position = screenPosition });
                yield return null;
            }
            finally
            {
                InputSystem.RemoveDevice(mouse);
            }
        }
    }
}
