using System.Collections;
using Blocks.Gameplay.Shooter;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace AutomationTests.TestFramework
{
    public class BaseUITests
    {
        [UnitySetUp]
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
            yield return TestExtensions.WaitForGameObject(netManagerPanelName, result => networkManagerPanel = result);

            // Host button
            var uiComponent = networkManagerPanel.GetComponent<UIDocument>();
            var hostButton = uiComponent.rootVisualElement.Q<Button>(hostButtonName);
            yield return hostButton.ClickButton();
        }
    }
}
