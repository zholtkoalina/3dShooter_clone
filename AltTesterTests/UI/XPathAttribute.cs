namespace AltTesterTests.Shared
{
    /// <summary>
    /// AltTester <c>By.PATH</c> (XPath-like) query for a game object. On a member, the path is relative to the
    /// declaring class: the two are concatenated, e.g. "//Player0" + "//*[@component=X]"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class XPathAttribute : Attribute
    {
        public XPathAttribute(string path) => Path = path;

        public string Path { get; }
    }
}
