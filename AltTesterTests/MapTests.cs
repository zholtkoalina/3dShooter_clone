using AltTester.AltTesterSDK.Driver;
using AltTesterTests.Shared;
using AwesomeAssertions;
using Blocks.Gameplay.Core;
using UnityEngine;
using GameObject = AltTesterTests.Shared.GameObject;

namespace AltTesterTests
{
    [TestFixture]
    public class MapTests : BaseUITest
    {
        [Test]
        public void Map_BasicObjectsChecks()
        {
            // Player
            var skinnedMeshRenderer = Unity.WaitFor<PlayerObject>().SkinnedMeshRenderer;
            skinnedMeshRenderer.Exists().Should().BeTrue();
            
            // Spawn pads
            var spawnPadList = Unity.WaitFor<MainScene>().SpawnPadList;
            spawnPadList.Count.Should().BeGreaterThan(0);
            
        }
    }
}
