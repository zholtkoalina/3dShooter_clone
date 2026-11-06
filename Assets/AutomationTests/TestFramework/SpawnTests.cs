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
    public class SpawnTests : BaseUITests
    {
        [UnityTest]
        public IEnumerator SpawnPlayer_DefaultCoreHUD()
        {
            const string playerElement = "Player0";
            const string playerMeshName = "Armature_Mesh";
            const string healthBarElement = "player-health-bar";
            const string staminaBarElement = "player-stamina-bar";
            
            // Player
            GameObject playerObject = null;
            yield return TestExtensions.WaitForGameObject(playerElement, result => playerObject = result);
            Assert.That(playerObject.GetComponent<CharacterController>().enabled, Is.True);

            SkinnedMeshRenderer playerMeshRenderer = null;
            yield return new WaitUntil(() =>
                playerMeshRenderer = playerObject.GetComponentInChildren<SkinnedMeshRenderer>(true));
            
            Assert.That(playerMeshRenderer.sharedMesh.name, Is.EqualTo(playerMeshName));
            
            // Check bars are full
            var uiComponent = playerObject.GetComponent<UIDocument>().rootVisualElement;

            ProgressBar healthBar = null;
            ProgressBar staminaBar = null;
            yield return new WaitUntil(() =>
            {
                healthBar = uiComponent.Q<ProgressBar>(healthBarElement);
                staminaBar = uiComponent.Q<ProgressBar>(staminaBarElement);
                return healthBar != null && staminaBar != null;
            });
            
            Assert.That(healthBar.value, Is.EqualTo(healthBar.highValue));
            Assert.That(staminaBar.value, Is.EqualTo(staminaBar.highValue));
        }
        
        [UnityTest]
        public IEnumerator SpawnPlayer_DefaultWeaponHUD()
        {
            const string playerElement = "Player0";
            const string weaponNameElement = "weapon-type-label";
            const string weaponIconElement = "weapon-icon";
            const string ammoBarElement = "player-ammo-bar";
            
            // Player
            GameObject playerObject = null;
            yield return TestExtensions.WaitForGameObject(playerElement, result => playerObject = result);
            Assert.That(playerObject.GetComponent<CharacterController>().enabled, Is.True);

            // Weapon
            var weaponController = playerObject.GetComponent<WeaponController>();
            yield return new WaitUntil(() => weaponController.CurrentWeapon != null);
            weaponController.CurrentWeapon.GetAmmoInfo(out _, out var maxBullets);

            // Check 
            var uiComponent = playerObject.GetComponent<UIDocument>().rootVisualElement;

            Label weaponName = null;
            Image weaponIcon = null;
            ProgressBar ammoBar = null;
            yield return new WaitUntil(() =>
            {
                weaponName = uiComponent.Q<Label>(weaponNameElement);
                weaponIcon = uiComponent.Q<Image>(weaponIconElement);
                ammoBar = uiComponent.Q<ProgressBar>(ammoBarElement);
                return weaponName != null && weaponIcon != null && ammoBar != null;
            });
            
            Assert.That(weaponName.text, Is.EqualTo(weaponController.CurrentWeapon.GetWeaponData().weaponName));
            Assert.That(weaponIcon.sprite, Is.EqualTo(weaponController.CurrentWeapon.GetWeaponData().weaponIcon));
            Assert.That(ammoBar.value, Is.EqualTo(maxBullets));
            Assert.That(ammoBar.value, Is.EqualTo(ammoBar.highValue));
        }
    }
}
