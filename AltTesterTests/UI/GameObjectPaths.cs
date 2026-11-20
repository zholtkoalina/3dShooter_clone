using AltTesterTests.Shared;

#pragma warning disable CS8618 // [XPath] members are assigned by GameObject.Create

namespace AltTesterTests.UI
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
        [XPath("//*[contains(@name,Pfb_Target)]")] public IReadOnlyList<GameObject> TargetList;
        [XPath("//*[contains(@name,Pfb_jumpPad)]")] public IReadOnlyList<GameObject> JumpPadList;
    }
}
