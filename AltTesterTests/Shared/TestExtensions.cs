using AltTester.AltTesterSDK.Driver;
using Blocks.Gameplay.Core;
using Unity.Netcode;

namespace AltTesterTests.Shared
{
    public class TestExtensions
    {
        /// <summary>Polls <paramref name="condition"/> until it is true; fails with the calling test line on timeout.</summary>
        protected static void WaitUntil(Func<bool> condition, double timeoutSeconds = 5, int pollIntervalMs = 50,
            [System.Runtime.CompilerServices.CallerMemberName] string caller = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int line = 0)
        {
            var deadline = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            while (!condition())
            {
                if (DateTime.UtcNow > deadline)
                    throw new TimeoutException($"Condition not met within {timeoutSeconds}s in {caller}, line {line}");
                Thread.Sleep(pollIntervalMs);
            }
        }
    }
}