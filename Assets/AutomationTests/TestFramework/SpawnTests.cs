using System.Collections;
using Blocks.Gameplay.Shooter;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace AutomationTests.TestFramework
{
    [TestFixture]
    public class SpawnTests
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
            var uiDocumentComponent = networkManagerPanel.GetComponent<UIDocument>();
            var hostButton = uiDocumentComponent.rootVisualElement.Q<Button>(hostButtonName);
            yield return hostButton.ClickButton();
        }

        [UnityTest]
        public IEnumerator SpawnPlayer_DefaultState()
        {
            const string playerName = "Player0";
            const string playerHealthBarName = "player-health-bar";
            const string playerAmmoBarName = "player-ammo-bar";
            
            GameObject playerObject = null;
            yield return TestExtensions.WaitForGameObject(playerName, result => playerObject = result);
            Assert.That(playerObject.GetComponent<CharacterController>().enabled, Is.True);

            var weaponController = playerObject.GetComponent<WeaponController>();
            yield return new WaitUntil(() => weaponController.CurrentWeapon != null);
            weaponController.CurrentWeapon.GetAmmoInfo(out _, out var maxBullets);

            var uiDocumentRoot = playerObject.GetComponent<UIDocument>().rootVisualElement;

            ProgressBar healthBar = null;
            ProgressBar ammoBar = null;
            yield return new WaitUntil(() =>
            {
                healthBar = uiDocumentRoot.Q<ProgressBar>(playerHealthBarName);
                ammoBar = uiDocumentRoot.Q<ProgressBar>(playerAmmoBarName);
                return healthBar != null && ammoBar != null;
            });

            // Check health is full.
            Assert.That(healthBar.value, Is.EqualTo(healthBar.highValue));

            // Check the ammo HUD matches the weapon's current clip and capacity.
            Assert.That(ammoBar.value, Is.EqualTo(maxBullets));
            Assert.That(ammoBar.value, Is.EqualTo(ammoBar.highValue));
        }
    }
}
