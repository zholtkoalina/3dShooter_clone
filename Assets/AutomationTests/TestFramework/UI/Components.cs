using System.Linq;
using UnityEngine;

namespace AutomationTests.TestFramework.UI
{
    internal static class Components
    {
        public const string OnMoveEventName = "OnMoveInput";
        public const string OnJumpPressEventName = "OnJumpPressed";
        public const string OnJumpReleasEventName = "OnJumpReleased";


        public static TEvent GetEvent<TEvent>(string name) where TEvent : ScriptableObject
        {
            return Resources.FindObjectsOfTypeAll<TEvent>().FirstOrDefault(gameEvent => gameEvent.name == name);
        }
    }

}
