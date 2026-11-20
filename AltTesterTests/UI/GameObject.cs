using System.Collections;
using System.Reflection;
using AltTester.AltTesterSDK.Driver;

namespace AltTesterTests.Shared
{
    /// <summary>
    /// Base class for game object descriptions. Create it with <see cref="Create{T}"/>; every field or property of
    /// type <see cref="GameObject"/> (or derived), or <see cref="IReadOnlyList{GameObject}"/>, marked with
    /// <see cref="XPathAttribute"/> is filled in automatically. Lookups happen when you use the object, not on creation.
    /// </summary>
    public class GameObject
    {
        private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private AltDriver _driver = null!;
        private AltObject? _resolved;

        public string Path { get; private set; } = "";

        public static T Create<T>(AltDriver driver, string parentPath = "") where T : GameObject, new()
        {
            var type = typeof(T);
            return (T)Create(type, driver, parentPath + (type.GetCustomAttribute<XPathAttribute>()?.Path ?? ""));
        }

        /// <summary>Current world position. Queried from the game on every read, so it is never stale.</summary>
        public AltVector3 Position => this.Find().GetWorldPosition();

        /// <summary>Horizontal (XZ-plane) distance between two world positions; height is ignored.</summary>
        public static double Distance(AltVector3 from, AltVector3 to) =>
            Math.Sqrt(Math.Pow(to.x - from.x, 2) + Math.Pow(to.z - from.z, 2));

        /// <summary>Reads a property or field of a component on this object. Read from the game on every call.</summary>
        public T GetComponentProperty<TComponent, T>(string propertyName) =>
            this.Find().GetComponentProperty<T>(typeof(TComponent).FullName, propertyName,
                typeof(TComponent).Assembly.GetName().Name);

        internal AltDriver Driver => _driver;

        internal AltObject? Resolved => _resolved;

        private static GameObject Create(Type type, AltDriver driver, string path)
        {
            var gameObject = (GameObject)Activator.CreateInstance(type)!;
            gameObject._driver = driver;
            gameObject.Path = path;
            gameObject.InitializeMembers();
            return gameObject;
        }

        internal static GameObject FromResolved(AltDriver driver, AltObject resolved, string path)
        {
            var gameObject = new GameObject { _driver = driver, _resolved = resolved, Path = path };
            return gameObject;
        }

        private void InitializeMembers()
        {
            foreach (var member in GetType().GetMembers(Members))
            {
                var xPath = member.GetCustomAttribute<XPathAttribute>();
                if (xPath == null)
                    continue;

                var memberPath = Path + xPath.Path;
                var memberType = member switch
                {
                    FieldInfo field => field.FieldType,
                    PropertyInfo property => property.PropertyType,
                    _ => null
                };

                object value;
                if (memberType != null && typeof(GameObject).IsAssignableFrom(memberType))
                    value = Create(memberType, _driver, memberPath);
                else if (memberType != null && memberType.IsAssignableFrom(typeof(GameObjectList)))
                    value = new GameObjectList(_driver, memberPath);
                else
                    throw new InvalidOperationException(
                        $"{GetType().Name}.{member.Name}: [XPath] members must be GameObject or IReadOnlyList<GameObject>.");

                if (member is FieldInfo f) f.SetValue(this, value);
                else ((PropertyInfo)member).SetValue(this, value);
            }
        }
    }

    public static class AltDriverGameObjectExtensions
    {
        /// <summary>Finds the object now; throws <see cref="NotFoundException"/> if it is absent.</summary>
        public static AltObject Find(this GameObject gameObject) =>
            gameObject.Resolved ?? gameObject.Driver.FindObject(By.PATH, gameObject.Path);

        public static bool Exists(this GameObject gameObject) =>
            gameObject.Resolved != null || gameObject.Driver.FindObjects(By.PATH, gameObject.Path).Any();

        /// <summary>Taps (clicks) the object, waiting for it to appear first.</summary>
        public static T Tap<T>(this T gameObject, int count = 1, float interval = 0.1f, bool wait = true)
            where T : GameObject
        {
            var target = gameObject.Resolved ?? gameObject.Driver.WaitForObject(By.PATH, gameObject.Path);
            target.Tap(count, interval, wait);
            return gameObject;
        }

        /// <summary>Presses a key (down, hold for <paramref name="duration"/>, up).</summary>
        public static T PressKey<T>(this T gameObject, AltKeyCode keyCode, float power = 1, float duration = 0.1f,
            bool wait = true) where T : GameObject
        {
            gameObject.Driver.PressKey(keyCode, power, duration, wait);
            return gameObject;
        }

        /// <summary>Creates the described object and waits until it appears: <c>Unity.WaitFor&lt;Player&gt;().SkinnedMeshRenderer</c>.</summary>
        public static T WaitFor<T>(this AltDriver driver, double timeout = 5) where T : GameObject, new()
        {
            var gameObject = GameObject.Create<T>(driver);
            driver.WaitForObject(By.PATH, gameObject.Path, timeout: timeout);
            return gameObject;
        }
    }

    /// <summary>All objects matching an XPath query. The query runs each time the list is read.</summary>
    public sealed class GameObjectList : IReadOnlyList<GameObject>
    {
        private readonly AltDriver _driver;
        private readonly string _path;

        public GameObjectList(AltDriver driver, string path)
        {
            _driver = driver;
            _path = path;
        }

        public int Count => Items().Count;

        public GameObject this[int index] => Items()[index];

        public IEnumerator<GameObject> GetEnumerator() => Items().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private List<GameObject> Items() => _driver.FindObjects(By.PATH, _path)
            .Select(x => GameObject.FromResolved(_driver, x, _path)).ToList();
    }
}
