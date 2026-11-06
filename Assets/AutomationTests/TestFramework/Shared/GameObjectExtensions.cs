using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutomationTests.TestFramework.Shared
{
    public static class GameObjectExtensions
    {
        /// <summary>Finds an active GameObject by hierarchy path, waiting up to the timeout.</summary>
        public static IEnumerator WaitForGameObject(string path, System.Action<GameObject> onFound,
            float timeoutSeconds = 5f, GameObject current = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new System.ArgumentException("Hierarchy path cannot be null or empty.", nameof(path));
            if (onFound == null)
                throw new System.ArgumentNullException(nameof(onFound));

            var segments = path.Split('/');
            var firstSegment = 0;
            while (firstSegment < segments.Length && string.IsNullOrEmpty(segments[firstSegment]))
                firstSegment++;
            if (firstSegment == segments.Length)
                throw new System.ArgumentException("Hierarchy path must contain a GameObject name.", nameof(path));

            var elapsed = 0f;
            while (true)
            {
                var match = current != null ? current : GameObject.Find(segments[firstSegment]);
                if (match != null && match.name == segments[firstSegment])
                {
                    for (var i = firstSegment + 1; i < segments.Length; i++)
                    {
                        if (string.IsNullOrEmpty(segments[i]))
                            throw new System.ArgumentException(
                                $"Hierarchy path '{path}' contains an empty segment.", nameof(path));

                        var child = match.transform.Find(segments[i]);
                        if (child == null)
                        {
                            match = null;
                            break;
                        }
                        match = child.gameObject;
                    }

                    if (match != null && match.activeInHierarchy)
                    {
                        onFound(match);
                        yield break;
                    }
                }

                if (current != null)
                    yield break;

                if (elapsed >= timeoutSeconds)
                    throw new System.TimeoutException(
                        $"Active GameObject at '{path}' was not found within {timeoutSeconds} seconds.");

                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
        }

        /// <summary>Finds all active GameObjects at a hierarchy path, waiting up to the timeout.</summary>
        public static IEnumerator WaitForGameObjects(string path, System.Action<List<GameObject>> onFound,
            float timeoutSeconds = 5f)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new System.ArgumentException("Hierarchy path cannot be null or empty.", nameof(path));
            if (onFound == null)
                throw new System.ArgumentNullException(nameof(onFound));

            var elapsed = 0f;
            while (true)
            {
                var matches = new List<GameObject>();
                foreach (var candidate in Resources.FindObjectsOfTypeAll<Transform>())
                {
                    if (!candidate.gameObject.scene.IsValid() || !candidate.gameObject.activeInHierarchy)
                        continue;

                    GameObject found = null;
                    yield return WaitForGameObject(path, result => found = result, timeoutSeconds,
                        candidate.gameObject);

                    if (found != null && !matches.Contains(found))
                        matches.Add(found);
                }

                if (matches.Count > 0)
                {
                    onFound(matches);
                    yield break;
                }

                if (elapsed >= timeoutSeconds)
                    throw new System.TimeoutException(
                        $"Active GameObjects at '{path}' were not found within {timeoutSeconds} seconds.");

                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
        }
    }
}
