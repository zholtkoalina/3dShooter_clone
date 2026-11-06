using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace AutomationTests.TestFramework.Shared
{
    public class BaseUITests
    {
        [UnityOneTimeSetUp]
        public IEnumerator SetUp()
        {
            const string testSceneName = "[BB] Shooter";
            const string hostButtonName = "host-button";
            const string netManagerPanelName = "NetworkManager";
        
            // Scene
            SceneManager.LoadScene(testSceneName);
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name ==  testSceneName);
        
            // Network panel
            GameObject networkManagerPanel = null;
            yield return GameObjectExtensions.WaitForGameObject(netManagerPanelName, result => networkManagerPanel = result);

            // Host button
            var uiComponent = networkManagerPanel.GetComponent<UIDocument>();
            var hostButton = uiComponent.rootVisualElement.Q<Button>(hostButtonName);
            yield return hostButton.ClickButton();
        }
    }
}
