using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

namespace AutomationTests.TestFramework
{
    public static class TestExtensions
    {
        /// <summary>Finds an active GameObject by hierarchy path, waiting up to the timeout.</summary>
        public static IEnumerator WaitForGameObject(string path, System.Action<GameObject> onFound, float timeoutSeconds = 5f)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new System.ArgumentException("Hierarchy path cannot be null or empty.", nameof(path));
            if (onFound == null)
                throw new System.ArgumentNullException(nameof(onFound));
            if (timeoutSeconds < 0f)
                throw new System.ArgumentOutOfRangeException(nameof(timeoutSeconds), "Timeout cannot be negative.");

            var segments = path.Split('/');
            var firstSegment = 0;
        
            // Remove empty segments
            while (firstSegment < segments.Length && string.IsNullOrEmpty(segments[firstSegment]))
                firstSegment++;
            if (firstSegment == segments.Length)
                throw new System.ArgumentException("Hierarchy path must contain a GameObject name.", nameof(path));

            var elapsed = 0f;
            while (true)
            {
                var current = GameObject.Find(segments[firstSegment]);
                if (current != null)
                {
                    for (var i = firstSegment + 1; i < segments.Length; i++)
                    {
                        if (string.IsNullOrEmpty(segments[i]))
                            throw new System.ArgumentException($"Hierarchy path '{path}' contains an empty segment.", nameof(path));

                        var child = current.transform.Find(segments[i]);
                        if (child == null)
                        {
                            current = null;
                            break;
                        }
                        current = child.gameObject;
                    }

                    if (current != null && current.activeInHierarchy)
                    {
                        onFound(current);
                        yield break;
                    }
                }

                if (elapsed >= timeoutSeconds)
                    throw new System.TimeoutException($"Active GameObject at '{path}' was not found within {timeoutSeconds} seconds.");

                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
        }

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
