# Workspace access rule

Do not access, read, write, or execute anything outside `C:\work\3dShooter` unless you first ask the user and receive permission. This includes browsing the web, network access, and using files or tools outside the project folder.

You are allowed and encouraged to ask the user clarifying questions to make tasks more complete, or whenever you are unsure how to proceed or are stuck.

## Fixing failed Unity tests

- Read the full failure message and stack trace first. Use the first project-owned stack frame to locate the failing test or helper, then inspect the relevant test, setup, and referenced scene or prefab before changing code.
- For GameObject lookup timeouts, verify the actual runtime hierarchy and exact object names. Remove unintended whitespace from path segments. If a stable hierarchy path is not necessary, locate the expected component under the known player root with `GetComponentInChildren` and assert the component or asset property directly.
- Check callbacks and captured variables carefully; make sure a lookup stores the result in the intended variable before asserting against it.
- Separate test-runner failures from test failures. Messages such as “running tests during Play mode is not possible” mean the test did not run. A missing test DLL may follow a compile or assembly-definition error; inspect Unity Console/compiler errors before changing test assertions.
- Run and verify the requested test through the already-open Unity Editor’s Test Runner. Do not start another Unity process or use batch mode for test verification. If the open Editor cannot be controlled from this session, explain that limitation and ask the user how to proceed instead of claiming the test passed.
- Report a test as passing only after observing its actual successful result in Unity.
- Do not rewrite or change the test scenario while fixing a failure. Preserve what the test is intended to exercise and its expected behavior; fix the underlying cause. Ask the user before changing the scenario if that becomes necessary.
