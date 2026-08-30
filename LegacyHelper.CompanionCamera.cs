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

        /// <summary>Kept clear at the frame edge so neither character rides the very edge.</summary>
        private const float EdgeMargin = 3.5f;

        /// <summary>
        /// The furthest the shot may drift off Hornet, as a share of the frame. A fixed margin let
        /// her be pushed most of the way to the edge once the companion was far enough out.
        /// </summary>
        private const float MaxLeanFraction = 0.45f;

        // The unzoomed field of view, taken the first time the camera is widened so it can be put
        // back exactly. Captured lazily because the game sets it per scene.
        private static float s_baseFieldOfView;
        private static bool s_haveBaseFieldOfView;
        private static float s_lastWrittenFieldOfView;

        // The projection is what actually widens the shot; see ApplyZoom.
        private static Matrix4x4 s_baseProjection;
        private static Matrix4x4 s_lastWrittenProjection;
        private static bool s_haveBaseProjection;
        private static bool s_projectionScaled;
        private static bool s_cullHooked;
        private static float s_wantedZoom = 1f;
        private static float s_appliedZoom = 1f;

        // Why the lean is or is not doing anything, for the bug reporter. Every stage below can
        // decline for a legitimate reason, and from outside they all look identical to the feature
        // being broken - which is exactly how this shipped inert twice.
        private static bool s_patchApplied;
        private static bool s_patchUnavailable;
        private static string s_lastDecision = "not evaluated yet";

        /// <summary>
        /// How much of the requested lean a camera lock area refused, per axis, last frame.
        /// <para>
        /// Lock areas cover ordinary rooms, and inside one the camera cannot leave the locked
        /// region however far the companion goes. Neither the zoom nor the leash knew that: the
        /// zoom widened only by what the <i>requested</i> lean failed to cover, and the leash
        /// measured reach from a lean the camera was never going to be allowed. Between them the
        /// Knight was let all the way off screen with the numbers reading healthy.
        /// </para>
        /// </summary>
        private static Vector2 s_leanRefused;

        /// <summary>Told by the patch how much lean actually survived the lock area's clamp.</summary>
        internal static void ReportAppliedLean(Vector2 wanted, Vector2 applied)
        {
            s_leanRefused = new Vector2(
                Mathf.Max(0f, Mathf.Abs(wanted.x) - Mathf.Abs(applied.x)),
                Mathf.Max(0f, Mathf.Abs(wanted.y) - Mathf.Abs(applied.y)));
        }

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
                $"enabled={ModConfig.Instance.companionCameraBiasEnabled}, offset={s_offset}, zoom wanted={s_zoom:0.###} applied={(s_projectionScaled ? s_appliedZoom : 1f):0.###}, lock refused {s_leanRefused}, {s_lastDecision}");
        }

        internal static void Reset()
        {
            s_offset = Vector2.zero;
            s_offsetVelocity = Vector2.zero;
            s_zoom = 1f;
            s_zoomVelocity = 0f;
            s_leanRefused = Vector2.zero;
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

            float halfWidth = view.width * 0.5f;
            float halfHeight = view.height * 0.5f;

            // How far from centre a character can sit and still read as comfortably on screen.
            float comfortX = Mathf.Max(0.001f, halfWidth - EdgeMargin);
            float comfortY = Mathf.Max(0.001f, halfHeight - EdgeMargin);

            // Lean only by what is needed to bring the companion inside that, rather than by half
            // the separation regardless. Leaning while both already fit is what pulled the shot off
            // a level boundary and wasted the room above Hornet.
            float wantedX = Mathf.Max(0f, Mathf.Abs(separation.x) - comfortX) * Mathf.Sign(separation.x);
            float wantedY = Mathf.Max(0f, Mathf.Abs(separation.y) - comfortY) * Mathf.Sign(separation.y);

            // Capped as a share of the frame rather than by a fixed margin, so Hornet stays well
            // inside the shot instead of being pushed most of the way to the edge.
            float maxLeanX = halfWidth * MaxLeanFraction;
            float maxLeanY = halfHeight * MaxLeanFraction;
            var offset = new Vector2(
                Mathf.Clamp(wantedX, -maxLeanX, maxLeanX),
                Mathf.Clamp(wantedY, -maxLeanY, maxLeanY));

            // Whatever the capped lean could not cover, widen the view to cover instead - measured
            // against the lean that survives the lock area rather than the one requested, or a
            // clamped lean silently costs the shot the width that was meant to make up for it.
            float effectiveX = Mathf.Sign(offset.x) * Mathf.Max(0f, Mathf.Abs(offset.x) - s_leanRefused.x);
            float effectiveY = Mathf.Sign(offset.y) * Mathf.Max(0f, Mathf.Abs(offset.y) - s_leanRefused.y);
            float companionFromCentreX = Mathf.Abs(separation.x - effectiveX);
            float companionFromCentreY = Mathf.Abs(separation.y - effectiveY);
            float zoom = Mathf.Max(companionFromCentreX / comfortX, companionFromCentreY / comfortY);

            float maxZoom = 1f + Mathf.Max(0f, ModConfig.Instance.companionCameraMaxZoom);
            zoom = Mathf.Clamp(zoom, 1f, maxZoom);

            s_lastDecision = FormattableString.Invariant($"leaning by {offset}, separation {separation}");
            return new Framing { Offset = offset, ZoomScale = zoom };
        }

        /// <summary>
        /// How far from Hornet a companion may roam before it genuinely cannot be shown any more.
        /// <para>
        /// This is the box the camera can <em>eventually</em> cover, not the one it is covering
        /// right now: the furthest it will lean, plus the widest it will zoom. The leash has to use
        /// this rather than the live frame, or it stops the companion at the current edge - which
        /// is the very thing that would have made the camera lean and zoom further. That deadlock
        /// is what "fighting the leash" was.
        /// </para>
        /// <para>
        /// So the companion briefly outruns the visible frame while the camera catches up over the
        /// smoothing time, which is the intended trade: the alternative is it never gets the room
        /// at all.
        /// </para>
        /// </summary>
        internal static bool TryGetCompanionRoam(Vector3 heroPosition, Vector2 extents, float margin, out Rect roam)
        {
            roam = default;
            if (!TryGetCameraViewBounds(heroPosition, out var view))
            {
                return false;
            }

            // The live frame already carries whatever zoom is applied; take it back out so the
            // headroom is measured once rather than compounding with itself.
            float applied = s_projectionScaled ? Mathf.Max(0.001f, s_appliedZoom) : 1f;
            float baseHalfWidth = view.width * 0.5f / applied;
            float baseHalfHeight = view.height * 0.5f / applied;

            float maxZoom = ModConfig.Instance.companionCameraBiasEnabled
                ? 1f + Mathf.Max(0f, ModConfig.Instance.companionCameraMaxZoom)
                : 1f;
            float lean = ModConfig.Instance.companionCameraBiasEnabled ? MaxLeanFraction : 0f;

            // Measured against the frame as it will be once widened, not as it is now: the lean is
            // capped as a share of the live frame, so a lean cap taken from the unzoomed one gave
            // away a slice of reach that the zoom was about to provide.
            float zoomedHalfWidth = baseHalfWidth * maxZoom;
            float zoomedHalfHeight = baseHalfHeight * maxZoom;

            // Minus what the lock area is currently refusing: reach the camera will not be allowed
            // to take is not reach, and promising it is what let the companion walk off screen.
            float reachX = zoomedHalfWidth * lean - s_leanRefused.x + (zoomedHalfWidth - margin - extents.x);
            float reachY = zoomedHalfHeight * lean - s_leanRefused.y + (zoomedHalfHeight - margin - extents.y);

            reachX = Mathf.Max(0f, reachX);
            reachY = Mathf.Max(0f, reachY);

            roam = new Rect(
                heroPosition.x - reachX,
                heroPosition.y - reachY,
                reachX * 2f,
                reachY * 2f);
            return true;
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
        /// Widens the view.
        /// <para>
        /// Field of view alone does not do it: tk2d assigns <c>projectionMatrix</c> outright in
        /// <c>UpdateCameraMatrix</c>, and an explicitly set projection makes <c>fieldOfView</c>
        /// inert. The bug reporter caught exactly that - "fov re-baselined" every frame while the
        /// requested zoom sat at its cap doing nothing. So the projection itself is scaled, at
        /// <c>onPreCull</c>, the last point before the camera draws and after everything that
        /// writes it has run.
        /// </para>
        /// <para>
        /// The field of view is still written alongside, because the darkness pass copies
        /// <c>mainCamera.fieldOfView</c> onto its own camera; that keeps the darkness cutout the
        /// size of the widened shot rather than lighting the wrong area.
        /// </para>
        /// </summary>
        private static void ApplyZoom(float scale)
        {
            var camera = ResolveMainCamera();
            if (camera == null)
            {
                return;
            }

            EnsureCullHook();
            s_wantedZoom = scale;

            if (!s_haveBaseFieldOfView)
            {
                s_baseFieldOfView = camera.fieldOfView;
                s_lastWrittenFieldOfView = s_baseFieldOfView;
                s_haveBaseFieldOfView = true;
            }
            else if (!Mathf.Approximately(camera.fieldOfView, s_lastWrittenFieldOfView))
            {
                // Whoever owns the field of view has written it again; re-baseline off their value
                // rather than stacking on one we did not write.
                s_baseFieldOfView = camera.fieldOfView;
            }

            // Visible height at a given depth goes with tan(fov/2), so scaling the view means
            // scaling the tangent rather than the angle.
            float halfBase = s_baseFieldOfView * 0.5f * Mathf.Deg2Rad;
            float target = 2f * Mathf.Atan(Mathf.Tan(halfBase) * scale) * Mathf.Rad2Deg;

            camera.fieldOfView = target;
            s_lastWrittenFieldOfView = target;
        }

        private static Camera ResolveMainCamera()
        {
            var cameras = GameCameras.instance;
            return cameras != null ? cameras.mainCamera : null;
        }

        private static void EnsureCullHook()
        {
            if (s_cullHooked)
            {
                return;
            }

            Camera.onPreCull += ApplyProjectionZoom;
            s_cullHooked = true;
        }

        /// <summary>
        /// Scales the projection the frame is actually drawn with. Fires for every camera, so it
        /// checks it has the gameplay one; re-bases whenever the matrix is not the one it last
        /// wrote, so a recomputed projection replaces the zoom rather than compounding with it.
        /// </summary>
        private static void ApplyProjectionZoom(Camera camera)
        {
            if (camera == null || camera != ResolveMainCamera())
            {
                return;
            }

            Matrix4x4 current = camera.projectionMatrix;
            if (!s_haveBaseProjection || current != s_lastWrittenProjection)
            {
                s_baseProjection = current;
                s_haveBaseProjection = true;
            }

            if (Mathf.Approximately(s_wantedZoom, 1f))
            {
                if (s_projectionScaled)
                {
                    camera.projectionMatrix = s_baseProjection;
                    s_lastWrittenProjection = s_baseProjection;
                    s_projectionScaled = false;
                    s_appliedZoom = 1f;
                }

                return;
            }

            // Widening the frustum is dividing the two scale terms: more world at the same depth.
            // Both axes together, so the aspect is untouched.
            Matrix4x4 scaled = s_baseProjection;
            scaled.m00 = s_baseProjection.m00 / s_wantedZoom;
            scaled.m11 = s_baseProjection.m11 / s_wantedZoom;

            camera.projectionMatrix = scaled;
            s_lastWrittenProjection = scaled;
            s_projectionScaled = true;
            s_appliedZoom = s_wantedZoom;
        }

        private static void RestoreFieldOfView()
        {
            s_wantedZoom = 1f;

            var camera = ResolveMainCamera();
            if (camera == null)
            {
                return;
            }

            if (s_haveBaseFieldOfView)
            {
                camera.fieldOfView = s_baseFieldOfView;
                s_lastWrittenFieldOfView = s_baseFieldOfView;
            }

            if (s_projectionScaled && s_haveBaseProjection)
            {
                camera.projectionMatrix = s_baseProjection;
                s_lastWrittenProjection = s_baseProjection;
                s_projectionScaled = false;
                s_appliedZoom = 1f;
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
                // Nothing was asked for, so nothing was refused. Leaving the previous frame's
                // shortfall standing would keep shrinking the leash after the camera was free again.
                CompanionCameraBias.ReportAppliedLean(Vector2.zero, Vector2.zero);
                return;
            }

            float baseX = ___targetDeltaX;
            float baseY = ___targetDeltaY;
            float x = baseX + framing.Offset.x;
            float y = baseY + framing.Offset.y;

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

            CompanionCameraBias.ReportAppliedLean(framing.Offset, new Vector2(x - baseX, y - baseY));
        }
    }
}
