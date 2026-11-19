using AltTester.AltTesterSDK.Driver;
using AltTesterTests.Shared;
using AwesomeAssertions;

namespace AltTesterTests
{
    [TestFixture]
    public class MovementInputTests : BaseUITest
    {
        [Test]
        public void MovementInput_WalkForwardChecks()
        {
            var skinnedMeshRenderer = Unity.WaitFor<PlayerObject>().SkinnedMeshRenderer;
            skinnedMeshRenderer.Exists().Should().BeTrue();
            
            var spawnPadList = Unity.WaitFor<MainScene>().SpawnPadList;
            spawnPadList.Count.Should().BeGreaterThan(0);
            
            Unity.PressKey(AltKeyCode.W);
            Unity.SetDelayAfterCommand(1);
           
        }

        [Test]
        public void MovementInput_JumpChecks()
        {
            var player = Unity.WaitFor<PlayerObject>();

            Unity.PressKey(AltKeyCode.Space);
            Unity.SetDelayAfterCommand(1);
        }
    }
}
