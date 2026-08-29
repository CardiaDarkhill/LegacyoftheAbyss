#nullable disable
using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using LegacyoftheAbyss.Shade;

public partial class LegacyHelper
{
    private static bool s_loggedCameraBoundsFailure;

    /// <summary>
    /// The world-space rectangle the player can currently see, measured at the depth of
    /// <paramref name="atWorldPoint"/>.
    /// <para>
    /// Derived by projecting the viewport corners rather than from <c>orthographicSize</c>, because
    /// Silksong's camera is <b>perspective</b> - tk2d drives it through
    /// <c>CalculateScaleForPerspectiveCamera</c>, the darkness pass copies its <c>fieldOfView</c>,
    /// and <see cref="CameraController"/> converts through viewport depth for the same reason.
    /// Assuming orthographic here silently disabled both the camera leash and the co-op lean.
    /// </para>
    /// </summary>
    internal static bool TryGetCameraViewBounds(Vector3 atWorldPoint, out Rect view)
    {
        view = default;

        var cameras = GameCameras.instance;
        var camera = cameras != null ? cameras.mainCamera : Camera.main;
        if (camera == null)
        {
            // No camera at all is ordinary outside a gameplay scene, so this one stays quiet.
            return false;
        }

        // The viewport corners have to be taken at the plane the characters occupy; under a
        // perspective camera the visible width depends on that depth.
        float depth = camera.WorldToViewportPoint(atWorldPoint).z;
        Vector3 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
        Vector3 topRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, depth));

        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;
        if (depth <= 0f || width <= 0f || height <= 0f)
        {
            // Everything downstream of this quietly does nothing, which is indistinguishable from
            // the feature being switched off - so say it once rather than leave it to be guessed.
            if (!s_loggedCameraBoundsFailure)
            {
                s_loggedCameraBoundsFailure = true;
                LogWarning(
                    "Camera view bounds could not be measured, so the co-op camera and the Knight's "
                    + $"camera leash are inactive. depth={depth}, width={width}, height={height}.");
            }

            return false;
        }

        view = new Rect(bottomLeft.x, bottomLeft.y, width, height);
        return true;
    }

    /// <summary>
    /// Keeps Hornet and a second player in one shot: leans the camera toward the midpoint between
    /// them, and widens the view a little once they no longer both fit.
    /// <para>
    /// Not a split screen: see the Roadmap for why Silksong's camera cannot be split. This is the
    /// part of that goal reachable without touching how anything renders.
    /// </para>
    /// </summary>
    internal static class CompanionCameraBias
    {
        /// <summary>What the camera should do this frame.</summary>
        internal struct Framing
        {
            /// <summary>Added to the camera's follow delta, in world units.</summary>
            public Vector2 Offset;

            /// <summary>How much wider the view should be. 1 is the game's own framing.</summary>
            public float ZoomScale;

            public bool IsNeutral => Offset == Vector2.zero && Mathf.Approximately(ZoomScale, 1f);

            public static Framing Neutral => new Framing { Offset = Vector2.zero, ZoomScale = 1f };
        }

        private static Vector2 s_offset;
        private static Vector2 s_offsetVelocity;
        private static float s_zoom = 1f;
        private static float s_zoomVelocity;

        private const float SmoothTime = 0.35f;

        /// <summary>
        /// How much of the way to the midpoint the camera goes. 1 frames both equally; less keeps
        /// the shot weighted toward Hornet, who is the one whose room this is.
        /// </summary>
        private const float MidpointShare = 0.5f;

        /// <summary>Kept clear at the frame edge so neither character rides the very edge.</summary>
        private const float EdgeMargin = 3.5f;

        // The unzoomed field of view, taken the first time the camera is widened so it can be put
        // back exactly. Captured lazily because the game sets it per scene.
        private static float s_baseFieldOfView;
        private static bool s_haveBaseFieldOfView;
        private static float s_lastWrittenFieldOfView;

        // Why the lean is or is not doing anything, for the bug reporter. Every stage below can
        // decline for a legitimate reason, and from outside they all look identical to the feature
        // being broken - which is exactly how this shipped inert twice.
        private static bool s_patchApplied;
        private static bool s_patchUnavailable;
        private static string s_lastDecision = "not evaluated yet";
        private static string s_zoomNote = string.Empty;

        internal static void MarkPatchApplied() => s_patchApplied = true;

        internal static void MarkPatchUnavailable() => s_patchUnavailable = true;

        /// <summary>One line naming the current state, for <c>BugReportState</c>.</summary>
        internal static string DescribeState()
        {
            if (s_patchUnavailable)
            {
                return "patch could not be applied";
            }

            if (!s_patchApplied)
            {
                return "patch never processed";
            }

            return FormattableString.Invariant(
                $"enabled={ModConfig.Instance.companionCameraBiasEnabled}, offset={s_offset}, zoom={s_zoom:0.###}, {s_lastDecision}{s_zoomNote}");
        }

        internal static void Reset()
        {
            s_offset = Vector2.zero;
            s_offsetVelocity = Vector2.zero;
            s_zoom = 1f;
            s_zoomVelocity = 0f;
            RestoreFieldOfView();
        }

        /// <summary>
        /// Where the shot should sit and how wide it should be, or neutral when the bias is off or
        /// there is no second player out.
        /// </summary>
        private static Framing ResolveFraming()
        {
            if (!ModConfig.Instance.companionCameraBiasEnabled)
            {
                s_lastDecision = "off in config";
                return Framing.Neutral;
            }

            var companion = FindSecondPlayer();
            if (companion == null)
            {
                s_lastDecision = "no companion spawned";
                return Framing.Neutral;
            }

            var hero = HeroController.UnsafeInstance;
            if (hero == null)
            {
                s_lastDecision = "no hero";
                return Framing.Neutral;
            }

            Vector3 heroPosition = hero.transform.position;
            if (!TryGetCameraViewBounds(heroPosition, out var view))
            {
                s_lastDecision = "camera bounds unmeasurable";
                return Framing.Neutral;
            }

            Vector2 separation = (Vector2)companion.transform.position - (Vector2)heroPosition;

            // Lean halfway toward the companion, capped so neither character leaves the shot.
            float maxX = Mathf.Max(0f, view.width * 0.5f - EdgeMargin);
            float maxY = Mathf.Max(0f, view.height * 0.5f - EdgeMargin);
            var offset = new Vector2(
                Mathf.Clamp(separation.x * MidpointShare, -maxX, maxX),
                Mathf.Clamp(separation.y * MidpointShare, -maxY, maxY));

            // With the shot centred between them each sits half the separation from the middle, so
            // widen only once that no longer fits - and never by more than the configured share.
            float neededHalfWidth = Mathf.Abs(separation.x) * 0.5f + EdgeMargin;
            float neededHalfHeight = Mathf.Abs(separation.y) * 0.5f + EdgeMargin;
            float zoom = Mathf.Max(
                neededHalfWidth / Mathf.Max(0.001f, view.width * 0.5f),
                neededHalfHeight / Mathf.Max(0.001f, view.height * 0.5f));

            float maxZoom = 1f + Mathf.Max(0f, ModConfig.Instance.companionCameraMaxZoom);
            zoom = Mathf.Clamp(zoom, 1f, maxZoom);

            s_lastDecision = FormattableString.Invariant($"leaning by {offset}, separation {separation}");
            return new Framing { Offset = offset, ZoomScale = zoom };
        }

        /// <summary>
        /// The spawned companion the camera should account for, or null. Any character counts - a
        /// Shade is leashed closer than a Knight, so it simply asks for a smaller lean rather than
        /// none at all.
        /// </summary>
        private static ShadeController FindSecondPlayer()
        {
            foreach (var companion in ShadeCompanionRegistry.All)
            {
                if (companion.Controller != null)
                {
                    return companion.Controller;
                }
            }

            return null;
        }

        /// <summary>Eases toward the wanted framing, applies the zoom, and hands back the lean.</summary>
        internal static Framing Step(float deltaTime)
        {
            Framing desired = ResolveFraming();

            s_offset = Vector2.SmoothDamp(s_offset, desired.Offset, ref s_offsetVelocity, SmoothTime, Mathf.Infinity, deltaTime);
            s_zoom = Mathf.SmoothDamp(s_zoom, desired.ZoomScale, ref s_zoomVelocity, SmoothTime, Mathf.Infinity, deltaTime);

            if (desired.IsNeutral && s_offset.sqrMagnitude < 0.0001f && Mathf.Abs(s_zoom - 1f) < 0.001f)
            {
                s_offset = Vector2.zero;
                s_offsetVelocity = Vector2.zero;
                s_zoom = 1f;
                s_zoomVelocity = 0f;
            }

            ApplyZoom(s_zoom);
            return new Framing { Offset = s_offset, ZoomScale = s_zoom };
        }

        /// <summary>
        /// Widens the view by raising the camera's field of view.
        /// <para>
        /// Field of view rather than pulling the camera back, because the darkness pass copies
        /// <c>mainCamera.fieldOfView</c> onto its own camera every frame - so the cutout that lets
        /// the characters be seen in a dark room widens with the shot for free. Moving the camera
        /// instead would leave the two disagreeing.
        /// </para>
        /// </summary>
        private static void ApplyZoom(float scale)
        {
            var cameras = GameCameras.instance;
            var camera = cameras != null ? cameras.mainCamera : null;
            if (camera == null)
            {
                return;
            }

            if (!s_haveBaseFieldOfView)
            {
                s_baseFieldOfView = camera.fieldOfView;
                s_lastWrittenFieldOfView = s_baseFieldOfView;
                s_haveBaseFieldOfView = true;
            }
            else if (!Mathf.Approximately(camera.fieldOfView, s_lastWrittenFieldOfView))
            {
                // Something else owns the field of view - tk2d rewrites it in UpdateCameraMatrix,
                // and the game changes it per scene. Re-baseline off whatever it set rather than
                // stacking our zoom on top of a value we did not write, and say so in the report.
                s_baseFieldOfView = camera.fieldOfView;
                s_zoomNote = ", fov re-baselined (something else is writing it)";
            }

            // Visible height at a given depth goes with tan(fov/2), so scaling the view means
            // scaling the tangent rather than the angle.
            float halfBase = s_baseFieldOfView * 0.5f * Mathf.Deg2Rad;
            float target = 2f * Mathf.Atan(Mathf.Tan(halfBase) * scale) * Mathf.Rad2Deg;

            camera.fieldOfView = target;
            s_lastWrittenFieldOfView = target;
        }

        private static void RestoreFieldOfView()
        {
            if (!s_haveBaseFieldOfView)
            {
                return;
            }

            var cameras = GameCameras.instance;
            var camera = cameras != null ? cameras.mainCamera : null;
            if (camera != null)
            {
                camera.fieldOfView = s_baseFieldOfView;
                s_lastWrittenFieldOfView = s_baseFieldOfView;
            }
        }
    }

    /// <summary>
    /// Adds the lean to the delta the camera's destination is built from.
    /// <para>
    /// This patches <c>CameraController.UpdateTargetDestinationDelta</c> rather than the camera
    /// target's own <c>Update</c>. Both feed the same number, but the target re-derives its
    /// position from the hero every frame and steps toward it, so an offset written there is
    /// something the target then walks back off. Writing the delta puts the lean in immediately
    /// before <c>LateUpdate</c> consumes it, where nothing else can undo it.
    /// </para>
    /// </summary>
    [HarmonyPatch]
    private class CameraController_TargetDelta_CompanionBias
    {
        /// <summary>
        /// Resolved by shape rather than named through the attribute: an overload would make
        /// <c>AccessTools</c> throw, and an unrecognised assembly should cost this one feature
        /// rather than every patch in the mod.
        /// </summary>
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var candidates = new List<MethodBase>();
            foreach (var method in typeof(CameraController).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                if (method.Name == "UpdateTargetDestinationDelta"
                    && method.GetParameters().Length == 0
                    && method.ReturnType == typeof(void))
                {
                    candidates.Add(method);
                }
            }

            if (candidates.Count != 1)
            {
                LogWarning($"Co-op camera disabled: CameraController.UpdateTargetDestinationDelta resolved to {candidates.Count} methods.");
                CompanionCameraBias.MarkPatchUnavailable();
                yield break;
            }

            CompanionCameraBias.MarkPatchApplied();
            yield return candidates[0];
        }

        private static void Postfix(
            CameraController __instance,
            ref float ___targetDeltaX,
            ref float ___targetDeltaY,
            float ___xLockMin,
            float ___xLockMax,
            float ___yLockMin,
            float ___yLockMax)
        {
            var framing = CompanionCameraBias.Step(Time.deltaTime);
            if (framing.Offset == Vector2.zero)
            {
                return;
            }

            float x = ___targetDeltaX + framing.Offset.x;
            float y = ___targetDeltaY + framing.Offset.y;

            // Inside a lock area the camera target clamps its own destination to the locked region,
            // and writing the delta here goes around that. Re-applying the same bounds keeps the
            // lean legal instead of standing aside entirely - lock areas cover ordinary rooms, not
            // just set pieces, so skipping them switched this off almost everywhere.
            if (__instance != null && __instance.CurrentLockArea != null)
            {
                x = Mathf.Clamp(x, ___xLockMin, ___xLockMax);
                y = Mathf.Clamp(y, ___yLockMin, ___yLockMax);
            }

            ___targetDeltaX = x;
            ___targetDeltaY = y;
        }
    }
}
