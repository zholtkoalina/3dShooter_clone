using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace AutomationTests.TestFramework.Shared
{
    public static class KeyboardExtensions
    {
        private static Keyboard m_TestKeyboard;
        private static readonly List<Key> m_PressedKeys = new List<Key>();

        /// <summary>Presses a key on a virtual keyboard for PlayMode tests.</summary>
        public static IEnumerator PressKey(Key key)
        {
            if (m_TestKeyboard == null)
                m_TestKeyboard = InputSystem.AddDevice<Keyboard>();

            if (!m_PressedKeys.Contains(key))
                m_PressedKeys.Add(key);
            
            InputSystem.QueueStateEvent(m_TestKeyboard, new KeyboardState(m_PressedKeys.ToArray()));
            yield return null;
        }

        /// <summary>Releases a key previously pressed with <see cref="PressKey"/>.</summary>
        public static IEnumerator ReleaseKey(Key key)
        {
            if (m_TestKeyboard == null)
                yield break;

            m_PressedKeys.Remove(key);
            InputSystem.QueueStateEvent(m_TestKeyboard, new KeyboardState(m_PressedKeys.ToArray()));
            yield return null;

            if (m_PressedKeys.Count == 0)
                ClearKeyboard();
        }

        /// <summary>Removes the virtual keyboard and clears its state after a test</summary>
        public static void ClearKeyboard()
        {
            m_PressedKeys.Clear();
            if (m_TestKeyboard == null)
                return;

            InputSystem.RemoveDevice(m_TestKeyboard);
            m_TestKeyboard = null;
        }
    }
}
