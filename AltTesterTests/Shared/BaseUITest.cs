using AltTester.AltTesterSDK.Driver;
using Blocks.Gameplay.Core;
using Unity.Netcode;

namespace AltTesterTests.Shared
{
    public class BaseUITest : TestExtensions
    {
        protected AltDriver Unity;
        
        private const string TestSceneName = "[BB] Shooter";
        private const string NetworkManagerName = "NetworkManager";

        [OneTimeSetUp]
        public void SetUp()
        {
            Unity = new AltDriver();
            Unity.SetTimeScale(1f);
            
            // Scene
            Unity.LoadScene(TestSceneName);
            
            // Start battle as host
            var networkManager = Unity.WaitForObject(By.NAME, NetworkManagerName);
            var gameNetworkUI = typeof(GameNetworkUI);
            networkManager.CallComponentMethod<object>(gameNetworkUI.FullName, nameof(GameNetworkUI.SubmitHostButton),
                gameNetworkUI.Assembly.GetName().Name, System.Array.Empty<object>());
        }

        [TearDown]
        public void TakeScreenshot()
        {
            // Make a screenshot after each test to help with debugging
            var testName = TestContext.CurrentContext.Test.Name;
            var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{testName}.png");
            Unity.GetPNGScreenshot(screenshotPath);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            // Stop the host at the end of the run so the next run can start a new one
            var existing = Unity.FindObject(By.NAME, NetworkManagerName);
            var networkManager = typeof(NetworkManager);
            existing.CallComponentMethod<object>(networkManager.FullName, nameof(NetworkManager.Shutdown),
                networkManager.Assembly.GetName().Name, [false], [typeof(bool).FullName]);

            Unity.Stop();
        }
    }
}