#pragma warning disable CS8618 // [XPath] members are assigned by GameObject.Create

namespace AltTesterTests.Shared
{
    
    [XPath("//Player0")]
    public class PlayerObject : GameObject
    {
        [XPath("//*[@component=SkinnedMeshRenderer]")] public GameObject SkinnedMeshRenderer;
    }
    
    [XPath("//MasterEnvironment_FPS")]
    public class MainScene : GameObject
    {
        [XPath("//*[contains(@name,Pfb_SpawnPad)]")] public IReadOnlyList<GameObject> SpawnPadList;
    }
}
