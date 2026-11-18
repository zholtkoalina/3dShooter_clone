using AltTester.AltTesterSDK.Driver;
using AltTesterTests.Shared;

namespace AltTesterTests
{
    [TestFixture]
    public class MovementInputTests : BaseUITest
    {
        [Test]
        public void MovementInput_WalkForwardChecks()
        {
            // Objects
            const string PlayerObjectName = "Player0";
            const string SkinnedMeshRendererName = "SkinnedMeshRenderer";

            var hasSkinnedMeshRenderer = _altDriver.FindObjects(By.PATH, $"//{PlayerObjectName}//*[@component={SkinnedMeshRendererName}]").Any();

            Assert.That(hasSkinnedMeshRenderer, Is.True,
                $"The spawned {PlayerObjectName} should have a {SkinnedMeshRendererName} component.");
            
            _altDriver.KeyDown(AltKeyCode.W);
            _altDriver.SetDelayAfterCommand(1);
            _altDriver.KeyUp(AltKeyCode.W);
        }
        
        [Test]
        public void MovementInput_JumpChecks()
        {
            // Objects
            const string PlayerObjectName = "Player0";
            const string SkinnedMeshRendererName = "SkinnedMeshRenderer";

            var hasSkinnedMeshRenderer = _altDriver.FindObjects(By.PATH, $"//{PlayerObjectName}//*[@component={SkinnedMeshRendererName}]").Any();

            Assert.That(hasSkinnedMeshRenderer, Is.True,
                $"The spawned {PlayerObjectName} should have a {SkinnedMeshRendererName} component.");
            
            _altDriver.KeyDown(AltKeyCode.Space);
            _altDriver.KeyUp(AltKeyCode.Space);
        }
    }
}