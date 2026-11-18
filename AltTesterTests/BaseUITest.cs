using AltTester.AltTesterSDK.Driver;

namespace AltTesterTests
{
    public class BaseUITest
    {
        protected AltDriver _altDriver;

        [OneTimeSetUp]
        public void SetUp()
        {
            _altDriver = new AltDriver();
            _altDriver.SetTimeScale(1f);
            
            // Objects
            const string TestSceneName = "[BB] Shooter";
            const string NetworkManagerName = "NetworkManager";

            // Methods
            const string NetworkUIComponentName = "Blocks.Gameplay.Core.GameNetworkUI";
            const string SubmitHostButtonMethodName = "SubmitHostButton";
            const string NetworkManagerAssemblyName = "Blocks.Gameplay.Core";

            // Scene
            _altDriver.LoadScene(TestSceneName);
            
            // Network panel
            var networkManager = _altDriver.WaitForObject(By.NAME, NetworkManagerName);
            
            // Host button
            networkManager.CallComponentMethod<object>(NetworkUIComponentName, SubmitHostButtonMethodName,
                NetworkManagerAssemblyName, System.Array.Empty<object>());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // Make screenshot of the last scene to help with debugging
            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "AltTester.png");
            _altDriver.GetPNGScreenshot(screenshotPath);

            // Stop the host at the end of the run so the next run can start a new one
            var existing = _altDriver.FindObject(By.NAME, "NetworkManager");
            existing.CallComponentMethod<object>("Unity.Netcode.NetworkManager", "Shutdown", "Unity.Netcode.Runtime",
                new object[] { false }, new[] { "System.Boolean" });

            _altDriver.Stop();
        }
    }
}