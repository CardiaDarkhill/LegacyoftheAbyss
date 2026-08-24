using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using BepInEx.Logging;
using InControl;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// Hotkey-driven bug capture: freeze, screenshot, snapshot, type a note, write it all to disk.
    /// <para>
    /// The whole point of this is the gap between noticing a bug and being able to describe it. Left to
    /// a plain console log, a Shade bug is reported hours later as "it sometimes falls through the
    /// floor near the bell", by which time the scene name, the charm loadout, the log lines and the
    /// exact position are all gone. Pressing one key while it is still on screen keeps all of them.
    /// </para>
    /// <para>
    /// Lives on its own <c>DontDestroyOnLoad</c> object rather than on the plugin behaviour so it keeps
    /// sampling and stays reachable during scene loads - which is when a good share of the bugs worth
    /// reporting actually happen.
    /// </para>
    /// </summary>
    internal sealed class BugReportSystem : MonoBehaviour
    {
        private const string HostObjectName = "LegacyBugReporter";
        private const int MessageCharacterLimit = 4000;
        private const float ToastSeconds = 6f;
        private const int SceneHistoryLength = 8;

        /// <summary>
        /// Units per second above which Hornet is taken to have been moved rather than to have moved.
        /// Set well clear of anything she can do under her own power - a dash tops out around 20 and
        /// a long fall around 30 - so only a genuine reposition trips it.
        /// </summary>
        private const float HeroTeleportSpeedThreshold = 45f;

        /// <summary>
        /// Substrings that mark a stack trace as this mod's problem. Silksong and other plugins throw
        /// plenty of exceptions that are none of our business, and auto-filing a report for each would
        /// bury the ones that matter.
        /// </summary>
        private static readonly string[] OwnCodeMarkers =
        {
            "LegacyHelper",
            "LegacyoftheAbyss",
            "ShadeController",
            "ShadeRuntime",
            "ShadeSettingsMenu",
            "ShadeInventoryPane",
            "SimpleHUD"
        };

        private enum OverlayState
        {
            Idle,

            /// <summary>Hotkey pressed; waiting for end of frame so the screenshot excludes the overlay.</summary>
            Capturing,

            /// <summary>Overlay open, game frozen, waiting for the message.</summary>
            Composing
        }

        private static BugReportSystem? s_instance;

        private BugReportLogRing? _logRing;
        private BugReportLogCollector? _logCollector;
        private BugReportFlightRecorder? _flightRecorder;
        private BugReportEventRing? _eventRing;

        /// <summary>Previous flight sample, for the position-discontinuity check.</summary>
        private BugReportFlightSample? _lastHeroSample;

        /// <summary>Previous flight sample, for the health-change check.</summary>
        private BugReportFlightSample? _lastHealthSample;

        private readonly List<string> _sceneHistory = new List<string>(SceneHistoryLength);
        private readonly HashSet<string> _seenExceptions = new HashSet<string>(StringComparer.Ordinal);
        private readonly Queue<PendingException> _pendingExceptions = new Queue<PendingException>();
        private readonly object _pendingSync = new object();
        private int _autoCaptureCount;

        private OverlayState _state = OverlayState.Idle;
        private string _message = string.Empty;
        private byte[]? _pendingScreenshot;
        private BugReportState? _pendingState;
        private float _captureRealtime;

        /// <summary>
        /// When the hotkey was pressed, not when the message was submitted. The report id is built from
        /// this so the folder name agrees with the "Filed" line inside it - otherwise every id carried
        /// however long the typing took, and a report of something seen at 18:24 filed itself as 18:25.
        /// </summary>
        private DateTime _captureLocalTime;
        private string? _pendingFlightCsv;
        private string? _pendingEventCsv;
        private string? _pendingEventTail;
        private bool _focusRequested;

        private float _nextSampleTime;
        private KeyCode _hotkey = KeyCode.F8;
        private string? _hotkeySource;

        private float _restoreTimeScale = 1f;
        private bool _freezeActive;
        private bool _restoreAcceptingInput = true;
        private bool _restorePauseAllowed = true;
        // Both only ever hold something while the overlay is composing, and both are restored by
        // RestoreGameInput - which OnDestroy also calls, so a plugin reload mid-report cannot leave
        // the game with no input.
        private bool _inputManagerSuspended;
        private EventSystem? _suspendedEventSystem;

        private string? _toastText;
        private float _toastUntil;
        private bool _toastIsError;

        private GUIStyle? _panelStyle;
        private GUIStyle? _titleStyle;
        private GUIStyle? _hintStyle;
        private GUIStyle? _textStyle;
        private GUIStyle? _toastStyle;

        private readonly struct PendingException
        {
            internal PendingException(string message, string stackTrace)
            {
                Message = message;
                StackTrace = stackTrace;
            }

            internal string Message { get; }

            internal string StackTrace { get; }
        }

        /// <summary>
        /// True while the report overlay owns the keyboard. Read by the mod's own key handlers so that
        /// typing a report does not simultaneously toggle damage logging or fire the Shade's nail.
        /// </summary>
        internal static bool IsCapturingText => s_instance != null && s_instance._state == OverlayState.Composing;

        internal static void Install(ManualLogSource? logger)
        {
            if (s_instance != null)
            {
                return;
            }

            try
            {
                if (!ModConfig.Instance.bugReportsEnabled)
                {
                    return;
                }

                var host = new GameObject(HostObjectName);
                UnityEngine.Object.DontDestroyOnLoad(host);
                host.hideFlags = HideFlags.HideAndDontSave;
                host.AddComponent<BugReportSystem>();
            }
            catch (Exception ex)
            {
                logger?.LogWarning("Bug reporter failed to install: " + ex);
            }
        }

        internal static void Shutdown()
        {
            var instance = s_instance;
            if (instance == null)
            {
                return;
            }

            try
            {
                UnityEngine.Object.Destroy(instance.gameObject);
            }
            catch
            {
            }
        }

        private void Awake()
        {
            s_instance = this;
            var config = ModConfig.Instance;

            _logRing = new BugReportLogRing(config.bugReportLogLines);
            _logCollector = new BugReportLogCollector(_logRing);
            try
            {
                BepInEx.Logging.Logger.Listeners.Add(_logCollector);
            }
            catch
            {
                _logCollector = null;
            }

            if (config.bugReportFlightRecorderEnabled)
            {
                _flightRecorder = new BugReportFlightRecorder(
                    config.bugReportFlightRecorderSeconds,
                    config.bugReportFlightRecorderIntervalSeconds);
            }

            if (config.bugReportEventRecorderEnabled)
            {
                _eventRing = new BugReportEventRing(config.bugReportEventRecorderCapacity);
            }

            Application.logMessageReceived += HandleUnityLog;
            SceneManager.sceneLoaded += HandleSceneLoaded;

            try
            {
                RecordScene(SceneManager.GetActiveScene().name);
            }
            catch
            {
            }

            LegacyHelper.LogInfo(
                "Bug reporter ready. Press " + ResolveHotkey() + " to file a report; they land in " + BugReportStore.Root);
        }

        private void OnDestroy()
        {
            RestoreGameInput();

            Application.logMessageReceived -= HandleUnityLog;
            SceneManager.sceneLoaded -= HandleSceneLoaded;

            if (_logCollector != null)
            {
                try
                {
                    BepInEx.Logging.Logger.Listeners.Remove(_logCollector);
                }
                catch
                {
                }

                _logCollector = null;
            }

            if (s_instance == this)
            {
                s_instance = null;
            }
        }

        private void Update()
        {
            float realtime = Time.realtimeSinceStartup;
            BugReportClock.Tick(realtime);

            DrainPendingExceptions();

            if (_state == OverlayState.Idle)
            {
                SampleFlightRecorder(realtime);

                if (Input.GetKeyDown(ResolveHotkey()))
                {
                    BeginCapture();
                }
            }
            else if (_state == OverlayState.Composing)
            {
                // The game is frozen and its input is stopped, so nothing else is going to keep these
                // in the state we set them. Reassert every frame rather than trusting a one-shot: a
                // scene load or a menu transition landing mid-compose would otherwise hand control back
                // to a player who is currently typing.
                EnforceGameInputStopped();
            }
        }

        private void SampleFlightRecorder(float realtime)
        {
            var recorder = _flightRecorder;
            if (recorder == null || realtime < _nextSampleTime)
            {
                return;
            }

            _nextSampleTime = realtime + recorder.IntervalSeconds;

            var sample = new BugReportFlightSample
            {
                Realtime = realtime,
                TimeScale = Time.timeScale,
                FrameMs = Time.unscaledDeltaTime * 1000f
            };

            try
            {
                sample.Scene = SceneManager.GetActiveScene().name;
            }
            catch
            {
            }

            try
            {
                var gameManager = MenuStateUtility.TryGetGameManager();
                var hero = gameManager != null ? gameManager.hero_ctrl : null;
                if (hero != null)
                {
                    Vector3 position = hero.transform.position;
                    sample.HeroX = position.x;
                    sample.HeroY = position.y;
                    sample.HeroState = hero.hero_state.ToString();
                    sample.HeroFlags = BugReportStateCollector.DescribeTrueBoolFields(hero.cState);

                    var playerData = hero.playerData;
                    if (playerData != null)
                    {
                        sample.HeroHp = playerData.health;
                        sample.HeroMaxHp = playerData.maxHealth;
                        sample.Silk = playerData.silk;
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (LegacyHelper.TryGetShadeController(out var shade) && shade != null)
                {
                    shade.CaptureFlightSample(ref sample);
                }
            }
            catch
            {
            }

            DetectHeroTeleport(in sample);
            DetectHealthChanges(in sample);
            recorder.Add(sample);
        }

        /// <summary>
        /// Flags a position jump Hornet cannot have walked, run or dashed. She is repositioned by
        /// boss grabs, cutscenes and hazard respawns, and when that happens for a reason the player
        /// did not expect it is the single most useful line in a report - the flight rows alone show
        /// her somewhere new without ever saying she was moved, because the move and its cause fall
        /// inside one sampling interval.
        /// <para>
        /// Scene has to match: every transition is a legitimate jump of arbitrary size.
        /// </para>
        /// </summary>
        private void DetectHeroTeleport(in BugReportFlightSample sample)
        {
            var previous = _lastHeroSample;
            _lastHeroSample = sample;

            if (!previous.HasValue || _eventRing == null)
            {
                return;
            }

            var last = previous.Value;
            if (!string.Equals(last.Scene, sample.Scene, StringComparison.Ordinal))
            {
                return;
            }

            float elapsed = sample.Realtime - last.Realtime;
            if (elapsed <= 0f)
            {
                return;
            }

            float dx = sample.HeroX - last.HeroX;
            float dy = sample.HeroY - last.HeroY;
            float distance = Mathf.Sqrt((dx * dx) + (dy * dy));

            // Generous enough that a dash, a hard fall or a recoil at a low frame rate stays quiet.
            float allowed = HeroTeleportSpeedThreshold * elapsed;
            if (distance <= allowed)
            {
                return;
            }

            RecordEvent(
                "hero-moved",
                FormattableString.Invariant($"Hornet moved {distance:0.##} units in {elapsed:0.###}s - too far to have run it"),
                FormattableString.Invariant(
                    $"from ({last.HeroX:0.##}, {last.HeroY:0.##}) hp {last.HeroHp} to ({sample.HeroX:0.##}, {sample.HeroY:0.##}) hp {sample.HeroHp}; shade at ({sample.ShadeX:0.##}, {sample.ShadeY:0.##}) hp {sample.ShadeHp} [{sample.ShadeFlags}]"));
        }

        /// <summary>
        /// Records health changes on both sides outright.
        /// <para>
        /// "Did Hornet actually take damage?" was not answerable from a report without cross-reading
        /// the flight rows by eye, even though every other part of the moment was recorded - the
        /// hero-damage rows say what <i>asked</i> to damage her, and most of them are zero-damage
        /// probes. A mask leaving the bar is the thing a reader wants, so it gets its own line.
        /// </para>
        /// </summary>
        private void DetectHealthChanges(in BugReportFlightSample sample)
        {
            var previous = _lastHealthSample;
            _lastHealthSample = sample;

            if (!previous.HasValue || _eventRing == null)
            {
                return;
            }

            var last = previous.Value;
            if (!string.Equals(last.Scene, sample.Scene, StringComparison.Ordinal))
            {
                return;
            }

            if (sample.HeroHp != last.HeroHp)
            {
                RecordEvent(
                    "hero-health",
                    sample.HeroHp < last.HeroHp ? "Hornet took damage" : "Hornet healed",
                    FormattableString.Invariant($"{last.HeroHp} -> {sample.HeroHp} of {sample.HeroMaxHp}"));
            }

            if (sample.ShadePresent && last.ShadePresent && sample.ShadeHp != last.ShadeHp)
            {
                RecordEvent(
                    "shade-health",
                    sample.ShadeHp < last.ShadeHp ? "Shade took damage" : "Shade healed",
                    FormattableString.Invariant($"{last.ShadeHp} -> {sample.ShadeHp} of {sample.ShadeMaxHp}"));
            }
        }

        /// <summary>
        /// Records one discrete event into the rolling window shipped with the next report. Safe to
        /// call from anywhere in the mod and at any time - it is a no-op when the system is not
        /// running or event recording is switched off, and it never throws into its caller.
        /// <para>
        /// Deliberately not gated on the <c>log*</c> config flags. Those govern console noise during
        /// normal play; a report needs this history whether or not the player had logging on, and in
        /// practice they never do.
        /// </para>
        /// </summary>
        internal static void RecordEvent(string category, string summary, string? detail = null)
        {
            try
            {
                var ring = s_instance?._eventRing;
                if (ring == null)
                {
                    return;
                }

                ring.Add(category, summary, detail, Time.realtimeSinceStartup, Time.frameCount);
            }
            catch
            {
            }
        }

        private KeyCode ResolveHotkey()
        {
            string configured = ModConfig.Instance.bugReportHotkey;
            if (string.Equals(configured, _hotkeySource, StringComparison.Ordinal))
            {
                return _hotkey;
            }

            _hotkeySource = configured;
            _hotkey = Enum.TryParse(configured, ignoreCase: true, out KeyCode parsed) && parsed != KeyCode.None
                ? parsed
                : KeyCode.F8;
            return _hotkey;
        }

        // --- capture -----------------------------------------------------------------------------

        private void BeginCapture()
        {
            _state = OverlayState.Capturing;
            _captureRealtime = Time.realtimeSinceStartup;
            _captureLocalTime = DateTime.Now;
            _message = string.Empty;
            _pendingScreenshot = null;

            // The snapshot and the flight recorder have to be read now, on the frame the key was
            // pressed, not when the message is submitted - by then the player has spent thirty seconds
            // typing and the "current" state is thirty seconds of frozen aftermath.
            _pendingState = BugReportStateCollector.Capture("hotkey", null, null);
            _pendingState.SceneHistory = _sceneHistory.ToArray();
            _pendingFlightCsv = _flightRecorder?.ToCsv(_captureRealtime);
            _pendingEventCsv = _eventRing?.ToCsv(_captureRealtime);
            _pendingEventTail = _eventRing?.RenderTail(BugReportEventRing.InlineTailEntries, _captureRealtime);

            StartCoroutine(FinishCaptureRoutine());
        }

        private IEnumerator FinishCaptureRoutine()
        {
            if (ModConfig.Instance.bugReportScreenshot)
            {
                // End of frame is the only point the backbuffer holds a complete frame, and taking it
                // here - one frame before the overlay first draws - is what keeps the report showing
                // the bug rather than a picture of the report form.
                yield return new WaitForEndOfFrame();
                _pendingScreenshot = CaptureScreenshot();
            }

            StopGameInput();
            _focusRequested = true;
            _state = OverlayState.Composing;
        }

        private static byte[]? CaptureScreenshot()
        {
            Texture2D? texture = null;
            Texture2D? scaled = null;
            try
            {
                texture = ScreenCapture.CaptureScreenshotAsTexture();

                // A 4K frame encodes to roughly 8 MB of PNG. That is per report, and reports are meant
                // to be filed freely and mailed around, so the default trades resolution nobody reads
                // at for a file size that does not make a folder of twenty reports a 160 MB problem.
                int maxWidth = ModConfig.Instance.bugReportScreenshotMaxWidth;
                if (maxWidth > 0 && texture.width > maxWidth)
                {
                    scaled = Downscale(texture, maxWidth);
                }

                // PNG encoding is a visible hitch, which is fine: the game is about to be frozen anyway
                // and a screenshot is worth far more than a smooth frame here.
                return (scaled ?? texture).EncodeToPNG();
            }
            catch (Exception ex)
            {
                LegacyHelper.LogWarning("Bug report screenshot failed: " + ex.Message);
                return null;
            }
            finally
            {
                if (texture != null)
                {
                    UnityEngine.Object.Destroy(texture);
                }

                if (scaled != null)
                {
                    UnityEngine.Object.Destroy(scaled);
                }
            }
        }

        /// <summary>
        /// Downscales on the CPU rather than by blitting through a render texture.
        /// <para>
        /// The GPU path is faster, but <c>CaptureScreenshotAsTexture</c> hands back already
        /// gamma-encoded backbuffer pixels, and pushing those through a blit in a linear-colour-space
        /// project re-applies the conversion - every screenshot would come out visibly washed out or
        /// darkened. A screenshot whose colours are wrong is worse than a large one, since half of what
        /// gets reported here is a rendering problem. Averaging bytes cannot change the colour space.
        /// </para>
        /// <para>
        /// It costs a few hundred milliseconds at 4K, which is affordable exactly here: the game is
        /// already frozen behind the overlay and nothing is waiting on this frame.
        /// </para>
        /// </summary>
        private static Texture2D Downscale(Texture2D source, int targetWidth)
        {
            int targetHeight = BugReportImage.ScaledHeight(source.width, source.height, targetWidth);
            var scaledPixels = BugReportImage.BoxDownscale(
                source.GetPixels32(),
                source.width,
                source.height,
                targetWidth,
                targetHeight);

            var result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, mipChain: false);
            result.SetPixels32(scaledPixels);
            result.Apply(updateMipmaps: false);
            return result;
        }

        private void Submit()
        {
            var state = _pendingState ?? BugReportStateCollector.Capture("hotkey", null, null);
            BugReportStore.SplitMessage(_message, out string title, out _);
            state.Title = title;
            state.Message = _message?.Trim();
            state.ReportId = BugReportStore.BuildReportId(_captureLocalTime, title);

            var payload = new BugReportPayload
            {
                State = state,
                LogText = _logRing?.Render(),
                LogTail = _logRing?.RenderTail(BugReportStore.InlineLogTailEntries, BugReportStore.InlineLogTailCharacters),
                FlightCsv = _pendingFlightCsv,
                EventCsv = _pendingEventCsv,
                EventTail = _pendingEventTail,
                ScreenshotPng = _pendingScreenshot
            };

            var result = BugReportStore.Write(payload);
            if (result.Success)
            {
                LegacyHelper.LogInfo("Bug report filed: " + result.Folder);
                ShowToast("Bug report saved: " + result.ReportId, false);
            }
            else
            {
                LegacyHelper.LogWarning("Bug report could not be written: " + result.Error);
                ShowToast("Bug report FAILED: " + result.Error, true);
            }

            CloseOverlay();
        }

        private void Cancel()
        {
            ShowToast("Bug report discarded.", false);
            CloseOverlay();
        }

        private void CloseOverlay()
        {
            _state = OverlayState.Idle;
            _message = string.Empty;
            _pendingScreenshot = null;
            _pendingState = null;
            _pendingFlightCsv = null;
            _pendingEventCsv = null;
            _pendingEventTail = null;
            RestoreGameInput();
        }

        // --- freezing ----------------------------------------------------------------------------

        private void StopGameInput()
        {
            if (!_freezeActive)
            {
                // Snapshot what the game had set before we touched anything. Forcing these back to
                // "running, accepting input, pause allowed" on close would be wrong in exactly the
                // cases worth reporting: open the overlay during a cutscene or an already-paused menu
                // and a blind restore hands control back to a player the game had deliberately taken
                // it from.
                _restoreTimeScale = Time.timeScale;
                _restoreAcceptingInput = true;
                _restorePauseAllowed = true;

                try
                {
                    var handler = HornetInput.FindHandler();
                    if (handler != null)
                    {
                        _restoreAcceptingInput = handler.acceptingInput;
                        _restorePauseAllowed = handler.PauseAllowed;
                    }
                }
                catch
                {
                }

                _freezeActive = true;
            }

            EnforceGameInputStopped();
        }

        private void EnforceGameInputStopped()
        {
            // Freezing serves two purposes: the bug stays on screen while you describe it, and Hornet
            // does not walk into a hazard during the thirty seconds you are not driving her.
            try
            {
                Time.timeScale = 0f;
            }
            catch
            {
            }

            try
            {
                var handler = HornetInput.FindHandler();
                if (handler != null)
                {
                    handler.StopAcceptingInput();
                    handler.PreventPause();
                }
            }
            catch
            {
            }

            // InputHandler.acceptingInput above only gates its *gameplay* branch. It was never
            // enough on its own: InControl keeps polling the keyboard either way, so every letter
            // typed into a report was also read as a game binding. Reported as Hornet acting out
            // the message the moment it was submitted, and as a paused report changing settings
            // under the cursor with each keystroke.
            //
            // Two more switches close that. InControl's own enable flag stops the PlayerActions
            // updating at all, which is what the hero's queued-input state and the pause menu's
            // navigation both read; disabling the EventSystem stops anything driven by Unity's
            // input modules, which is the other half of a menu responding while the overlay owns
            // the keyboard. Both are pure on/off switches, so restoring them cannot lose state.
            try
            {
                if (!_inputManagerSuspended && InputManager.Enabled)
                {
                    InputManager.Enabled = false;
                    _inputManagerSuspended = true;
                }
            }
            catch
            {
            }

            try
            {
                var eventSystem = EventSystem.current;
                if (eventSystem != null && eventSystem.enabled)
                {
                    _suspendedEventSystem = eventSystem;
                    eventSystem.enabled = false;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Puts back exactly what <see cref="StopGameInput"/> found. Also called from OnDestroy, because
        /// a plugin reload while the overlay is open would otherwise leave the game frozen with its
        /// input off and nothing left running that knows how to undo that.
        /// </summary>
        private void RestoreGameInput()
        {
            if (!_freezeActive)
            {
                return;
            }

            _freezeActive = false;

            try
            {
                Time.timeScale = _restoreTimeScale;
            }
            catch
            {
            }

            try
            {
                if (_inputManagerSuspended)
                {
                    InputManager.Enabled = true;
                }
            }
            catch
            {
            }
            finally
            {
                _inputManagerSuspended = false;
            }

            try
            {
                if (_suspendedEventSystem != null)
                {
                    _suspendedEventSystem.enabled = true;
                }
            }
            catch
            {
            }
            finally
            {
                _suspendedEventSystem = null;
            }

            try
            {
                var handler = HornetInput.FindHandler();
                if (handler != null)
                {
                    if (_restoreAcceptingInput)
                    {
                        handler.StartAcceptingInput();
                    }

                    if (_restorePauseAllowed)
                    {
                        handler.AllowPause();
                    }
                }
            }
            catch
            {
            }
        }

        // --- exceptions --------------------------------------------------------------------------

        private void HandleUnityLog(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception)
            {
                return;
            }

            try
            {
                if (!ModConfig.Instance.bugReportAutoCaptureExceptions)
                {
                    return;
                }

                if (!IsOurCode(condition) && !IsOurCode(stackTrace))
                {
                    return;
                }

                lock (_pendingSync)
                {
                    _pendingExceptions.Enqueue(new PendingException(condition ?? string.Empty, stackTrace ?? string.Empty));
                }
            }
            catch
            {
            }
        }

        private static bool IsOurCode(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            foreach (string marker in OwnCodeMarkers)
            {
                if (text!.Contains(marker, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Auto reports are written from Update, never from the log callback itself. That callback fires
        /// mid-throw, quite possibly from inside a Harmony patch on a half-finished game method; doing
        /// file I/O there turns one exception into two.
        /// </summary>
        private void DrainPendingExceptions()
        {
            while (true)
            {
                PendingException pending;
                lock (_pendingSync)
                {
                    if (_pendingExceptions.Count == 0)
                    {
                        return;
                    }

                    pending = _pendingExceptions.Dequeue();
                }

                try
                {
                    WriteAutoReport(pending);
                }
                catch
                {
                }
            }
        }

        private void WriteAutoReport(PendingException pending)
        {
            var config = ModConfig.Instance;
            if (_autoCaptureCount >= config.bugReportAutoCaptureLimit)
            {
                return;
            }

            // A throw inside Update repeats every frame. Deduping on the message plus the first stack
            // frame keeps a single broken code path to a single report instead of one per frame until
            // the session limit is hit.
            string key = pending.Message + "|" + FirstStackFrame(pending.StackTrace);
            if (!_seenExceptions.Add(key))
            {
                return;
            }

            _autoCaptureCount++;

            string title = "Exception: " + Truncate(pending.Message.Replace("\n", " ").Replace("\r", string.Empty), 120);
            var state = BugReportStateCollector.Capture("auto-exception", title, null);
            state.SceneHistory = _sceneHistory.ToArray();
            state.ExceptionMessage = pending.Message;
            state.ExceptionStackTrace = pending.StackTrace;
            state.ReportId = BugReportStore.BuildReportId(DateTime.Now, title);

            var payload = new BugReportPayload
            {
                State = state,
                LogText = _logRing?.Render(),
                LogTail = _logRing?.RenderTail(BugReportStore.InlineLogTailEntries, BugReportStore.InlineLogTailCharacters),
                FlightCsv = _flightRecorder?.ToCsv(state.Realtime),
                EventCsv = _eventRing?.ToCsv(state.Realtime),
                EventTail = _eventRing?.RenderTail(BugReportEventRing.InlineTailEntries, state.Realtime)
            };

            var result = BugReportStore.Write(payload);
            if (result.Success)
            {
                LegacyHelper.LogWarning("Auto-filed a bug report for an unhandled exception: " + result.Folder);
                ShowToast("Auto-filed exception report: " + result.ReportId, true);
            }
        }

        private static string FirstStackFrame(string? stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
            {
                return string.Empty;
            }

            int breakIndex = stackTrace!.IndexOf('\n');
            return breakIndex < 0 ? stackTrace : stackTrace.Substring(0, breakIndex);
        }

        private static string Truncate(string value, int length)
        {
            return value.Length <= length ? value : value.Substring(0, length);
        }

        // --- scenes ------------------------------------------------------------------------------

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            RecordScene(scene.name);
        }

        private void RecordScene(string? sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            if (_sceneHistory.Count > 0 && string.Equals(_sceneHistory[_sceneHistory.Count - 1], sceneName, StringComparison.Ordinal))
            {
                return;
            }

            _sceneHistory.Add(sceneName!);
            if (_sceneHistory.Count > SceneHistoryLength)
            {
                _sceneHistory.RemoveAt(0);
            }
        }

        // --- overlay -----------------------------------------------------------------------------

        private void ShowToast(string text, bool isError)
        {
            _toastText = text;
            _toastIsError = isError;
            _toastUntil = Time.realtimeSinceStartup + ToastSeconds;
        }

        private void OnGUI()
        {
            // Well in front of anything else drawing IMGUI, including other plugins' debug overlays.
            GUI.depth = -1000;
            EnsureStyles();

            if (_state == OverlayState.Composing)
            {
                HandleOverlayKeys();
                DrawOverlay();
            }

            DrawToast();
        }

        /// <summary>
        /// Runs before the text area is drawn so the submit and cancel chords are consumed here rather
        /// than typed into the message.
        /// </summary>
        private void HandleOverlayKeys()
        {
            var current = Event.current;
            if (current == null || current.type != EventType.KeyDown)
            {
                return;
            }

            if (current.keyCode == KeyCode.Escape)
            {
                current.Use();
                Cancel();
                return;
            }

            bool enter = current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter;
            if (enter && (current.control || current.command))
            {
                current.Use();
                Submit();
            }
        }

        private void DrawOverlay()
        {
            float width = Mathf.Min(760f, Screen.width - 80f);
            float height = Mathf.Min(360f, Screen.height - 120f);
            var panel = new Rect((Screen.width - width) / 2f, (Screen.height - height) / 2f, width, height);

            // Dim the whole screen so the panel is readable over bright scenes, but keep it translucent
            // so whatever you are reporting stays visible behind it.
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Box(panel, GUIContent.none, _panelStyle);

            var inner = new Rect(panel.x + 18f, panel.y + 14f, panel.width - 36f, panel.height - 28f);
            GUI.Label(new Rect(inner.x, inner.y, inner.width, 26f), "Report a bug", _titleStyle);

            GUI.Label(
                new Rect(inner.x, inner.y + 28f, inner.width, 22f),
                "First line becomes the title. Game is frozen; state, logs and a screenshot are already captured.",
                _hintStyle);

            var textRect = new Rect(inner.x, inner.y + 54f, inner.width, inner.height - 96f);
            GUI.SetNextControlName("legacy_bug_message");
            string edited = GUI.TextArea(textRect, _message, MessageCharacterLimit, _textStyle);
            if (!string.Equals(edited, _message, StringComparison.Ordinal))
            {
                _message = edited;
            }

            if (_focusRequested)
            {
                GUI.FocusControl("legacy_bug_message");
                _focusRequested = false;
            }

            string footer = "Ctrl+Enter save   -   Esc discard";
            if (_pendingScreenshot != null)
            {
                footer += "   -   screenshot " + (_pendingScreenshot.Length / 1024).ToString(CultureInfo.InvariantCulture) + " KB";
            }

            GUI.Label(new Rect(inner.x, inner.yMax - 36f, inner.width, 22f), footer, _hintStyle);
        }

        private void DrawToast()
        {
            if (string.IsNullOrEmpty(_toastText) || Time.realtimeSinceStartup > _toastUntil)
            {
                return;
            }

            var size = _toastStyle!.CalcSize(new GUIContent(_toastText));
            float width = Mathf.Min(size.x + 28f, Screen.width - 40f);
            var rect = new Rect(20f, Screen.height - 60f, width, 32f);

            GUI.color = _toastIsError ? new Color(0.35f, 0.05f, 0.05f, 0.9f) : new Color(0.05f, 0.05f, 0.08f, 0.85f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 14f, rect.y + 6f, rect.width - 20f, rect.height), _toastText, _toastStyle);
        }

        private void EnsureStyles()
        {
            if (_panelStyle != null)
            {
                return;
            }

            // Built here rather than in Awake: GUI.skin is only valid inside OnGUI, and reading it from
            // Awake yields the null skin and a set of styles that silently render as nothing.
            _panelStyle = new GUIStyle(GUI.skin.box);

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true
            };
            _hintStyle.normal.textColor = new Color(0.78f, 0.78f, 0.82f);

            _textStyle = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = 14,
                wordWrap = true,
                padding = new RectOffset(8, 8, 8, 8)
            };

            _toastStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13
            };
            _toastStyle.normal.textColor = Color.white;
        }
    }
}
