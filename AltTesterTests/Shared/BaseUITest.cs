using AltTester.AltTesterSDK.Driver;
using System.Linq;

namespace AltTesterTests.Shared
{
    public class BaseUITest
    {
        protected AltDriver _altDriver;
        
        private const string TestSceneName = "[BB] Shooter";
        private const string NetworkManagerName = "NetworkManager";

        [OneTimeSetUp]
        public void SetUp()
        {
            _altDriver = new AltDriver();
            _altDriver.SetTimeScale(1f);
            
            // Scene
            _altDriver.LoadScene(TestSceneName);
            
            // Start battle as host
            var networkManager = _altDriver.WaitForObject(By.NAME, NetworkManagerName);
            networkManager.CallComponentMethod<object>("Blocks.Gameplay.Core.GameNetworkUI", "SubmitHostButton",
                "Blocks.Gameplay.Core", System.Array.Empty<object>());
        }

        [TearDown]
        public void TakeScreenshot()
        {
            // Make a screenshot after each test to help with debugging
            var testName = TestContext.CurrentContext.Test.Name;
            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{testName}.png");
            _altDriver.GetPNGScreenshot(screenshotPath);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // Stop the host at the end of the run so the next run can start a new one
            var existing = _altDriver.FindObject(By.NAME, NetworkManagerName);
            existing.CallComponentMethod<object>("Unity.Netcode.NetworkManager", "Shutdown", "Unity.Netcode.Runtime",
                new object[] { false }, new[] { "System.Boolean" });

            _altDriver.Stop();
        }
    }
}