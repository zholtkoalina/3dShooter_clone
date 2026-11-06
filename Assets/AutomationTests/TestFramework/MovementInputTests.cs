using System.Collections;
using System.Linq;
using AutomationTests.TestFramework.Shared;
using Blocks.Gameplay.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using TestData = AutomationTests.TestFramework.Support.TestData;

namespace AutomationTests.TestFramework
{
    [TestFixture]
    public class MovementInputTests : BaseUITests
    {
        [UnityTest]
        public IEnumerator MovementInput_MoveForwardChecks()
        {
            const float movementDurationSeconds = 1f;
            const float deltaTime = 0.3f;
            const float deltaDistance = 0.5f;
            
            GameObject playerObject = null;
            yield return GameObjectExtensions.WaitForGameObject(TestData.PlayerObjectName, result => playerObject = result);

            var movement = playerObject.GetComponent<CoreMovement>();
            var onMoveInputEvent = Resources.FindObjectsOfTypeAll<Vector2Event>().FirstOrDefault(gameEvent => gameEvent.name == "OnMoveInput");
            var startPosition = playerObject.transform.position;
            
            // Start moving
            yield return KeyboardExtensions.PressKey(Key.W);
            Assert.That(onMoveInputEvent.LastValue, Is.Not.EqualTo(Vector2.zero), "OnMoveInput should be active while W is pressed");
            
            yield return new WaitForSecondsRealtime(movementDurationSeconds);

            // Stop moving
            yield return KeyboardExtensions.ReleaseKey(Key.W);
            yield return new WaitForSecondsRealtime(deltaTime); // Wait for deceleration to finish
            Assert.That(onMoveInputEvent.LastValue, Is.EqualTo(Vector2.zero), "OnMoveInput should not be active when player is idle");
            Assert.That(movement.CurrentSpeed, Is.EqualTo(0));

            // Measure distance
            var stopPosition = playerObject.transform.position;
            var distance = Vector3.ProjectOnPlane(startPosition - stopPosition, Vector3.up).magnitude;
            Assert.That(distance, Is.EqualTo(movementDurationSeconds * movement.moveSpeed).Within(deltaDistance));
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            KeyboardExtensions.ClearKeyboard();
            yield return null;
        }
    }
}
