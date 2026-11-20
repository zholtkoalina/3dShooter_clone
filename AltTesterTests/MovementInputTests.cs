using AltTester.AltTesterSDK.Driver;
using AltTesterTests.Shared;
using AwesomeAssertions;
using Blocks.Gameplay.Core;
using UnityEngine;
using GameObject = AltTesterTests.Shared.GameObject;

namespace AltTesterTests
{
    [TestFixture]
    public class MovementInputTests : BaseUITest
    {
        const float deltaDistance = 0.5f;
        
        [Test]
        public void MovementInput_WalkForwardChecks()
        {
            const float walkDurationSeconds = 1f;
            
            // Parameters
            var player = Unity.WaitFor<PlayerObject>();
            var coreMovement = typeof(CoreMovement);
            var moveSpeed = player.Find().GetComponentProperty<float>(coreMovement.FullName,
                nameof(CoreMovement.moveSpeed), coreMovement.Assembly.GetName().Name);

            // Walk forward
            var startPosition = player.Position;
            Unity.PressKey(AltKeyCode.W, duration: walkDurationSeconds);
            var stopPosition = player.Position;

            // Measure distance
            var distance = GameObject.Distance(startPosition, stopPosition);
            distance.Should().BeApproximately(walkDurationSeconds * moveSpeed, deltaDistance);
        }

        [Test]
        public void MovementInput_JumpChecks()
        {
            var player = Unity.WaitFor<PlayerObject>();

            // Cooldown before first Jump can be made
            WaitUntil(() =>
                player.GetComponentProperty<CoreMovement, float>(nameof(CoreMovement.TimeSinceLanded)) >=
                player.GetComponentProperty<JumpAbility, float>(nameof(JumpAbility.LandingCooldown)));
            
            player.GetComponentProperty<CoreMovement, bool>(nameof(CoreMovement.IsGrounded)).Should().BeTrue("Player should be grounded before jumping");
            var groundY = player.Position.y;

            // Press jump button
            Unity.PressKey(AltKeyCode.Space);
            WaitUntil(() => !player.GetComponentProperty<CoreMovement, bool>(nameof(CoreMovement.IsGrounded)));
            player.Position.y.Should().BeGreaterThan(groundY, "Player should be above the ground during Jump");

            // Wait for landing
            WaitUntil(() => player.GetComponentProperty<CoreMovement, bool>(nameof(CoreMovement.IsGrounded)));
            player.Position.y.Should().BeApproximately(groundY, deltaDistance, "Player should be grounded after Jump");
        }
    }
}
