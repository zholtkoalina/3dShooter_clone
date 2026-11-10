using System.Collections;
using System.Linq;
using AutomationTests.TestFramework.Shared;
using AutomationTests.TestFramework.Support;
using Blocks.Gameplay.Core;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace AutomationTests.TestFramework
{
    [TestFixture]
    public class MovementInputTests : BaseUITests
    {
        const float deltaDistance = 0.5f;
        
        [UnityTest]
        public IEnumerator MovementInput_WalkForwardChecks()
        {
            const float walkDurationSeconds = 1f;
            const float deltaTime = 0.3f;
            
            var basicUI = new BasicUI();
            yield return basicUI.WaitPlayerSpawned();

            // Parameters
            var playerObject = GameObject.Find(BasicUI.PlayerObjectName);
            var movement = playerObject.GetComponent<CoreMovement>();
            var onMoveInputEvent = Resources.FindObjectsOfTypeAll<Vector2Event>().FirstOrDefault(gameEvent => gameEvent.name == "OnMoveInput");
            var startPosition = playerObject.transform.position;
            
            // Press walk button
            yield return KeyboardExtensions.PressKey(Key.W);
            Assert.That(onMoveInputEvent.LastValue, Is.Not.EqualTo(Vector2.zero), "Event should be active while W is pressed");
            
            yield return new WaitForSecondsRealtime(walkDurationSeconds);

            // Release walk button
            yield return KeyboardExtensions.ReleaseKey(Key.W);
            yield return TestExtensions.WaitUntil(() => movement.CurrentSpeed.Equals(0), deltaTime);
            Assert.That(movement.CurrentSpeed, Is.EqualTo(0));
            Assert.That(onMoveInputEvent.LastValue, Is.EqualTo(Vector2.zero), "Event should not be active in idle");

            // Measure distance
            var stopPosition = playerObject.transform.position;
            var distance = Vector3.ProjectOnPlane(startPosition - stopPosition, Vector3.up).magnitude;
            Assert.That(distance, Is.EqualTo(walkDurationSeconds * movement.moveSpeed).Within(deltaDistance));
        }

        [UnityTest]
        public IEnumerator MovementInput_JumpChecks()
        {
            var basicUI = new BasicUI();
            yield return basicUI.WaitPlayerSpawned();

            // Parameters
            var playerObject = GameObject.Find(BasicUI.PlayerObjectName);
            var movement = playerObject.GetComponent<CoreMovement>();
            var jumpAbility = playerObject.GetComponent<JumpAbility>();
            var staminaBar = basicUI.Hud.StaminaBar;
            
            // Events
            var onJumpPressedEvent = Resources.FindObjectsOfTypeAll<GameEvent>().FirstOrDefault(gameEvent => gameEvent.name == "OnJumpPressed");
            var onJumpReleasedEvent = Resources.FindObjectsOfTypeAll<GameEvent>().FirstOrDefault(gameEvent => gameEvent.name == "OnJumpReleased");
            var jumpPressedCount = onJumpPressedEvent.RaiseCount;
            var jumpReleasedCount = onJumpReleasedEvent.RaiseCount;
            
            // Cooldown before first Jump can be made
            yield return TestExtensions.WaitUntil(() => movement.TimeSinceLanded >= jumpAbility.LandingCooldown);
            
            Assert.That(movement.IsGrounded, Is.True, "Player should be grounded before jumping");
            Assert.That(staminaBar.value, Is.EqualTo(staminaBar.highValue));
            var groundY = playerObject.transform.position.y;
            
            // Press jump button
            yield return KeyboardExtensions.PressKey(Key.Space);
            yield return TestExtensions.WaitUntil(() => !movement.IsGrounded);
            Assert.That(onJumpPressedEvent.RaiseCount, Is.GreaterThan(jumpPressedCount), "Event should be raised when Space is pressed");
            Assert.That(playerObject.transform.position.y, Is.GreaterThan(groundY), "Player should be above the ground during Jump");
            Assert.That(staminaBar.value, Is.LessThan(staminaBar.highValue));

            // Release jump button
            yield return KeyboardExtensions.ReleaseKey(Key.Space);
            yield return TestExtensions.WaitUntil(() => movement.IsGrounded);
            Assert.That(onJumpReleasedEvent.RaiseCount, Is.GreaterThan(jumpReleasedCount), "Event should be raised when Space is released");
            Assert.That(playerObject.transform.position.y, Is.EqualTo(groundY).Within(deltaDistance), "Player should be grounded after Jump");
            Assert.That(staminaBar.value, Is.LessThan(staminaBar.highValue), "Stamina still isn't recovered right after Jump");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            KeyboardExtensions.ClearKeyboard();
            yield return null;
        }
    }
}
