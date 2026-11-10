# Workspace access rule

Do not access, read, write, or execute anything outside `C:\work\3dShooter` unless you first ask the user and receive permission. This includes browsing the web, network access, and using files or tools outside the project folder.

The user has granted standing permission to read the Unity test result file at `C:\Users\Alyna\AppData\LocalLow\DefaultCompany\3dShooter\TestResults.xml`. This permission applies only to reading that file.

If completing a task requires access outside the project folder, ask the user for permission and identify the specific path and action needed. Do not treat this rule as a reason to stop without asking when outside access is necessary.

You are allowed and encouraged to ask the user clarifying questions to make tasks more complete, or whenever you are unsure how to proceed or are stuck.

## Fixing failed Unity tests

- Read the full failure message and stack trace first. Use the first project-owned stack frame to locate the failing test or helper, then inspect the relevant test, setup, and referenced scene or prefab before changing code.
- Do not rewrite or change the test scenario while fixing a failure. Preserve what the test is intended to exercise and its expected behavior; fix the underlying cause. Ask the user before changing the scenario if that becomes necessary.
- For GameObject lookup timeouts, verify the actual runtime hierarchy and exact object names. Remove unintended whitespace from path segments. If a stable hierarchy path is not necessary, locate the expected component under the known player root with `GetComponentInChildren` and assert the component or asset property directly.
- Check callbacks and captured variables carefully; make sure a lookup stores the result in the intended variable before asserting against it.
- Separate test-runner failures from test failures. Messages such as “running tests during Play mode is not possible” mean the test did not run. A missing test DLL may follow a compile or assembly-definition error; inspect Unity Console/compiler errors before changing test assertions.
- Run tests in the already-open Unity Editor whenever the task asks for a test run or when a Unity test is the right way to verify a fix. Use the Editor's Test Runner (or its `TestRunnerApi` from the Editor) and select the exact test by its discovered full name and correct mode (`PlayMode` or `EditMode`). Do not start another Unity process or use batch mode for verification.
- Before starting a test, check that the Editor is not compiling or updating. If compilation, domain reload, test discovery, scene loading, or asset initialization is in progress, wait for it to finish; do not treat the start command's success as the test result. Unity test startup can take a while. Keep checking the open Editor's status and Console/Test Runner until the run completes.
- Confirm that the requested test was discovered and actually executed. After fixing a failure, rerun the same test through the open Editor. Wait for the run to finish and inspect its Test Runner result or result output. A successful launch, saved-results notice, or unrelated Console messages are not proof of a pass; report a test as passing only after observing its actual successful result.
- If the result is written to a path outside `C:\work\3dShooter`, follow the workspace access rule: ask the user for permission to read that specific result file, then inspect it if permission is granted. If the open Editor cannot be controlled from this session, explain that limitation and ask the user how to proceed instead of claiming the test passed.

## Writing Unity tests

- Base new tests on the closest existing test or the specific test the user references. Follow its fixture, setup/teardown, naming, and helper conventions unless the requested behavior requires a change.
- Exercise the real input or interaction path with the project's test helpers and the actual configured control. Assert the resulting gameplay behavior, not only that the input was sent.
- Establish preconditions before acting. If an action has a cooldown, grounded requirement, stamina requirement, animation state, or other gate, wait for the configured runtime value/state rather than guessing with a fixed delay. Preserve the intended test scenario while fixing setup or timing issues.
- Capture relevant state before the action and assert the expected change afterward. Prefer observable component properties and world state. A state probe such as `IsGrounded` may need a corresponding position/transform assertion when exact placement matters.
- Verify event channels directly when the test requires them. For parameterless `GameEvent` assets, snapshot `RaiseCount` before the action and assert it increases afterward. For payload events such as `OnMoveInput`, assert the payload through `LastValue`. Avoid test-only booleans, counters, or listener bookkeeping when the event already exposes a direct way to verify a raise.
- Keep event assets and GUID references stable while writing tests. Do not rename or move production assets just to make a test lookup convenient. If the user explicitly requires different asset names, preserve the `.meta` GUID and keep the asset filename and `m_Name` aligned.
- Use bounded waits such as `TestExtensions.WaitUntil` for asynchronous Unity state changes. Prefer conditions over arbitrary sleeps; include enough timeout for normal initialization and cooldowns. Its timeout message includes the calling test method, file, and line to identify the wait that failed.
- Distinguish input state from resulting gameplay state. For example, a zero `OnMoveInput.LastValue` means there is no movement input, but speed may still be decelerating; wait for `CoreMovement.CurrentSpeed` to reach zero before asserting the player stopped.
