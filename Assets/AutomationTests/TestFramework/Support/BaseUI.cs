using System.Collections;
using System.Linq;
using AutomationTests.TestFramework.Shared;
using UnityEngine;
using UnityEngine.UIElements;

namespace AutomationTests.TestFramework.Support
{
    internal class BasicUI
    {
        public const string PlayerObjectName = "Player0";
        public const string PlayerMeshName = "Armature_Mesh";
        public const string SpawnPadName = "Pfb_SpawnPad";
        

        private GameObject gameObject;
        public UIElements Hud => new(gameObject.GetComponent<UIDocument>().rootVisualElement);
        public IEnumerator WaitPlayerSpawned()
        {
            return GameObjectExtensions.WaitForGameObject(PlayerObjectName, result => gameObject = result);
        }

        public IEnumerator WaitPlayerMesh()
        {
            return new WaitUntil(() => gameObject.GetComponentInChildren<SkinnedMeshRenderer>(true));
        }
        
        public Transform[] GetSpawnPoints() => GameObject.FindObjectsByType<Transform>()
            .Where(transform => transform.name == SpawnPadName).ToArray();
    }

}
