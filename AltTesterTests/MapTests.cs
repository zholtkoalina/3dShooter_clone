using AltTester.AltTesterSDK.Driver;
using AltTesterTests.Shared;
using AltTesterTests.UI;
using AwesomeAssertions;

namespace AltTesterTests
{
    [TestFixture]
    public class MapTests : BaseUITest
    {
        [Test]
        public void Map_BasicObjectsChecks()
        {
            const int SpawnPadCount = 4;
            const int TargetCount = 6;
            const int JumpPadCount = 3;
            
            // Player
            var skinnedMeshRenderer = Unity.WaitFor<PlayerObject>().SkinnedMeshRenderer;
            skinnedMeshRenderer.Exists().Should().BeTrue();
            
            // Spawn pads
            var mainScene = Unity.WaitFor<MainScene>();
            mainScene.SpawnPadList.Count.Should().Be(SpawnPadCount);
            mainScene.TargetList.Count.Should().Be(TargetCount);
            mainScene.JumpPadList.Count.Should().Be(JumpPadCount);
        }
    }
}
