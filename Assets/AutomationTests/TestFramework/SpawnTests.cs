using System.Collections;
using System.Linq;
using AutomationTests.TestFramework.Shared;
using Blocks.Gameplay.Shooter;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using TestData = AutomationTests.TestFramework.Support.TestData;

namespace AutomationTests.TestFramework
{
    [TestFixture]
    public class SpawnTests : BaseUITests
    {
        [UnityTest]
        public IEnumerator PlayerSpawn_DefaultCoreHUDChecks()
        {
            // Player
            GameObject playerObject = null;
            yield return GameObjectExtensions.WaitForGameObject(TestData.PlayerObjectName, result => playerObject = result);
            Assert.That(playerObject.GetComponent<CharacterController>().enabled, Is.True);

            SkinnedMeshRenderer playerMeshRenderer = null;
            yield return new WaitUntil(() =>
                playerMeshRenderer = playerObject.GetComponentInChildren<SkinnedMeshRenderer>(true));
            
            Assert.That(playerMeshRenderer.sharedMesh.name, Is.EqualTo(TestData.PlayerMeshName));
            
            // Check bars are full
            var uiComponent = playerObject.GetComponent<UIDocument>().rootVisualElement;

            ProgressBar healthBar = null;
            ProgressBar staminaBar = null;
            yield return new WaitUntil(() =>
            {
                healthBar = uiComponent.Q<ProgressBar>(TestData.UIElements.HealthBar);
                staminaBar = uiComponent.Q<ProgressBar>(TestData.UIElements.StaminaBar);
                return healthBar != null && staminaBar != null;
            });
            
            Assert.That(healthBar.value, Is.EqualTo(healthBar.highValue));
            Assert.That(staminaBar.value, Is.EqualTo(staminaBar.highValue));
        }
        
        [UnityTest]
        public IEnumerator PlayerSpawn_DefaultWeaponHUDChecks()
        {
            // Player
            GameObject playerObject = null;
            yield return GameObjectExtensions.WaitForGameObject(TestData.PlayerObjectName, result => playerObject = result);
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
                weaponName = uiComponent.Q<Label>(TestData.UIElements.Name);
                weaponIcon = uiComponent.Q<Image>(TestData.UIElements.Icon);
                ammoBar = uiComponent.Q<ProgressBar>(TestData.UIElements.AmmoBar);
                return weaponName != null && weaponIcon != null && ammoBar != null;
            });
            
            Assert.That(weaponName.text, Is.EqualTo(weaponController.CurrentWeapon.GetWeaponData().weaponName));
            Assert.That(weaponIcon.sprite, Is.EqualTo(weaponController.CurrentWeapon.GetWeaponData().weaponIcon));
            Assert.That(ammoBar.value, Is.EqualTo(maxBullets));
            Assert.That(ammoBar.value, Is.EqualTo(ammoBar.highValue));
        }

        [UnityTest]
        public IEnumerator PlayerSpawn_SpawnPointChecks()
        {
            // Player
            GameObject playerObject = null;
            yield return GameObjectExtensions.WaitForGameObject(TestData.PlayerObjectName, result => playerObject = result);
            Assert.That(playerObject.GetComponent<CharacterController>().enabled, Is.True);
            
            var userPosition = playerObject.transform.position;
            var spawnObjectArray = GameObject.FindObjectsByType<Transform>().Where(t => t.name == "Pfb_SpawnPad")
                .ToArray();

            Assert.That(spawnObjectArray, Is.Not.Null, "No spawn pad transforms were found");
            Assert.That(spawnObjectArray.Any(x => Vector3.Distance(x.position, userPosition) < 1f), Is.True);
        }
    }
}
