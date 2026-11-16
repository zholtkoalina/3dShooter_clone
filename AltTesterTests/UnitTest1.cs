using AltTester.AltTesterSDK.Driver;
using NUnit.Framework;

namespace AutomationTests.AltTesterFramework.Editor
{
    public class NewAltTest
    {
        private AltDriver altDriver;

        [OneTimeSetUp]
        public void SetUp()
        {
            altDriver = new AltDriver();
            altDriver.SetTimeScale(1f);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            altDriver.GetPNGScreenshot( @"C:\work\3dShooter\Temp\Bin\Debug");
            altDriver.Stop();
        }

        [Test]
        public void Test()
        {
            const string TestSceneName = "[BB] Shooter";
            const string NetworkManagerName = "NetworkManager";
            const string NetworkManagerComponentName = "GameNetworkManager";
            const string NetworkManagerAssemblyName = "Blocks.Gameplay.Core";
    
            altDriver.LoadScene(TestSceneName);
            var networkManager = altDriver.WaitForObject(By.NAME, NetworkManagerName);

            // The host button is a UI Toolkit VisualElement, not a GameObject component.
            // Trigger the same manager method used by that button's callback.
            networkManager.CallComponentMethod<object>(NetworkManagerComponentName, "StartHostConnection",
                NetworkManagerAssemblyName, System.Array.Empty<object>());

            Assert.That(
                networkManager.WaitForComponentProperty(NetworkManagerComponentName, "IsHost", true,
                    NetworkManagerAssemblyName, timeout: 10), Is.True,
                "Starting a host should make the NetworkManager report IsHost.");
        }
    }
}