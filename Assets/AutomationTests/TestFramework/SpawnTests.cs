using System.Collections;
using System.Linq;
using AutomationTests.TestFramework.Shared;
using AutomationTests.TestFramework.Support;
using Blocks.Gameplay.Shooter;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AutomationTests.TestFramework
{
    [TestFixture]
    public class SpawnTests : BaseUITests
    {
        [UnityTest]
        public IEnumerator PlayerSpawn_DefaultCoreHUDChecks()
        {
            // Player
            var basicUI = new BasicUI();
            yield return basicUI.WaitPlayerSpawned();
            var playerObject = GameObject.Find(BasicUI.PlayerObjectName);
            Assert.That(playerObject.GetComponent<CharacterController>().enabled, Is.True);

            yield return basicUI.WaitPlayerMesh();
            var playerMeshRenderer = playerObject.GetComponentInChildren<SkinnedMeshRenderer>(true);
            Assert.That(playerMeshRenderer.sharedMesh.name, Is.EqualTo(BasicUI.PlayerMeshName));
            
            // Check bars are full
            var healthBar = basicUI.Hud.HealthBar;
            var staminaBar = basicUI.Hud.StaminaBar;
            
            Assert.That(healthBar.value, Is.EqualTo(healthBar.highValue));
            Assert.That(staminaBar.value, Is.EqualTo(staminaBar.highValue));
        }
        
        [UnityTest]
        public IEnumerator PlayerSpawn_DefaultWeaponHUDChecks()
        {
            // Player
            var basicUI = new BasicUI();
            yield return basicUI.WaitPlayerSpawned();
            var playerObject = GameObject.Find(BasicUI.PlayerObjectName);

            // Weapon
            var weaponController = playerObject.GetComponent<WeaponController>();
            yield return TestExtensions.WaitUntil(() => weaponController.CurrentWeapon != null);
            weaponController.CurrentWeapon.GetAmmoInfo(out _, out var maxBullets);

            // Check 
            var weaponName = basicUI.Hud.WeaponName;
            var weaponIcon = basicUI.Hud.WeaponIcon;
            var ammoBar = basicUI.Hud.AmmoBar;
            
            Assert.That(weaponName.text, Is.EqualTo(weaponController.CurrentWeapon.GetWeaponData().weaponName));
            Assert.That(weaponIcon.sprite, Is.EqualTo(weaponController.CurrentWeapon.GetWeaponData().weaponIcon));
            Assert.That(ammoBar.value, Is.EqualTo(maxBullets));
            Assert.That(ammoBar.value, Is.EqualTo(ammoBar.highValue));
        }

        [UnityTest]
        public IEnumerator PlayerSpawn_SpawnPointChecks()
        {
            // Player
            var basicUI = new BasicUI();
            yield return basicUI.WaitPlayerSpawned();
            var playerObject = GameObject.Find(BasicUI.PlayerObjectName);
            
            var userPosition = playerObject.transform.position;
            var spawnObjectArray = basicUI.GetSpawnPoints();

            Assert.That(spawnObjectArray, Is.Not.Null, "No spawn pad transforms were found");
            Assert.That(spawnObjectArray.Any(x => Vector3.Distance(x.position, userPosition) < 1f), Is.True);
        }
    }
}
