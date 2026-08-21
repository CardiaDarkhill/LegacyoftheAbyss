#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LegacyoftheAbyss.Shade;
using UnityEngine;
using GlobalEnums;

public partial class LegacyHelper
{
    public partial class ShadeController : MonoBehaviour
    {
        private GameObject aggroProxy;
        private Collider2D aggroProxyCollider;

        private void Start()
        {
            SetupPhysics();
            if (hornetTransform == null)
            {
                var hornet = GameObject.FindWithTag("Player");
                if (hornet != null)
                {
                    hornetTransform = hornet.transform;
                }
            }

            sr = GetComponent<SpriteRenderer>();
            transform.localScale = Vector3.one * SpriteScale;
            LoadShadeSprites();
            if (sr != null)
            {
                if (idleAnimFrames != null && idleAnimFrames.Length > 0)
                    sr.sprite = idleAnimFrames[0];
                else if (floatAnimFrames != null && floatAnimFrames.Length > 0)
                    sr.sprite = floatAnimFrames[0];
                else if (inactiveSprite != null)
                    sr.sprite = inactiveSprite;
                var c = sr.color; c.a = 0.9f; sr.color = c;
            }

            baseMaxDistance = maxDistance;
            baseSoftLeashRadius = softLeashRadius;
            baseHardLeashRadius = hardLeashRadius;
            baseSnapLeashRadius = snapLeashRadius;
            baseSprintMultiplier = sprintMultiplier;
            baseFireCooldown = fireCooldown;
            baseNailCooldown = nailCooldown;
            baseFocusSoulCost = focusSoulCost;
            baseProjectileSoulCost = projectileSoulCost;
            baseShriekSoulCost = shriekSoulCost;
            baseQuakeSoulCost = quakeSoulCost;
            baseSoulGainPerHit = soulGainPerHit;
            baseFocusChannelTime = focusChannelTime;
            baseFocusHealRange = focusHealRange;
            baseTeleportChannelTime = teleportChannelTime;
            baseHitKnockbackForce = hitKnockbackForce;
            if (baseShadeMaxHP <= 0)
            {
                baseShadeMaxHP = shadeMaxHP;
            }
            ResetCharmDerivedStats();
            wasInactive = (!isDying && GetTotalCurrentHealth() <= 0);

            // Ensure the shade can act as a pogo surface for Hornet
            try { gameObject.tag = "Recoiler"; } catch { }

            SetupShadeLight();
            cachedHud = UnityEngine.Object.FindFirstObjectByType<SimpleHUD>();
            PushSoulToHud();
            CheckHazardOverlap();
            // Ensure pogo target is present for Hornet downslash bounces
            // Add a dedicated pogo target with HitResponse so hero slashes can register even when OnlyDamageEnemies is true
            EnsurePogoTarget();
            EnsureAggroProxyCollider();
            bool hasSavedState = LegacyHelper.HasSavedShadeState;
            int computedMax = -1;
            try
            {
                var pd = GameManager.instance != null ? GameManager.instance.playerData : null;
                if (pd != null)
                {
                    int playerDerivedMax = Mathf.Max(1, (pd.maxHealth + 1) / 2);
                    computedMax = playerDerivedMax;
                    if (!hasSavedState || playerDerivedMax > shadeMaxHP)
                    {
                        shadeMaxHP = playerDerivedMax;
                    }
                    if (!hasSavedState && shadeHP <= 0)
                    {
                        shadeHP = Mathf.Clamp((pd.health + 1) / 2, 0, shadeMaxHP);
                    }
                    shadeHP = Mathf.Clamp(shadeHP, 0, shadeMaxHP);
                    shadeLifeblood = Mathf.Clamp(shadeLifeblood, 0, shadeLifebloodMax);
                    PushShadeStatsToHud(suppressDamageAudio: true);
                }
            }
            catch { }

            if (computedMax > baseShadeMaxHP)
            {
                baseShadeMaxHP = computedMax;
            }
            else if (baseShadeMaxHP <= 0)
            {
                baseShadeMaxHP = shadeMaxHP;
            }

            baselineStatsInitialized = true;

            lastSavedHP = lastSavedMax = lastSavedLifeblood = lastSavedLifebloodMax = lastSavedSoul = -999;
            PersistIfChanged();
            lastSoulForReady = shadeSoul;
            TryPlaySpawnAnimation();
            QueueCharmLoadoutRecompute();
            PersistIfChanged();
        }

        private void OnDestroy()
        {
            try
            {
                try { aggroProxyTracker?.ForceExitTrackedRemaskers(); } catch { }
                if (sceneProtectionSuppressingPersistence)
                {
                    ExitPersistenceSuppression();
                    sceneProtectionSuppressingPersistence = false;
                }

                bool desiredDamageState = sceneProtectionActive ? sceneProtectionDesiredDamageState : canTakeDamage;
                LegacyHelper.SaveShadeState(
                    shadeHP,
                    shadeMaxHP,
                    shadeLifeblood,
                    shadeLifebloodMax,
                    shadeSoul,
                    desiredDamageState,
                    baseShadeMaxHP);
            }
            catch
            {
            }

            if (aggroProxyCollider)
            {
                aggroProxyCollider.enabled = false;
            }
        }

        private void EnsureAggroProxyCollider()
        {
            try
            {
                int heroLayer = gameObject.layer;
                string heroTag = null;
                try
                {
                    var hc = HeroController.instance;
                    if (hc && hc.gameObject)
                    {
                        heroLayer = hc.gameObject.layer;
                        heroTag = hc.gameObject.tag;
                    }
                }
                catch
                {
                }

                if (!aggroProxy)
                {
                    aggroProxy = new GameObject("ShadeAggroProxy");
                    aggroProxy.transform.SetParent(transform, false);
                }

                aggroProxy.transform.localPosition = Vector3.zero;
                aggroProxy.transform.localRotation = Quaternion.identity;
                aggroProxy.transform.localScale = Vector3.one;
                aggroProxy.layer = heroLayer;
                if (string.IsNullOrEmpty(heroTag))
                {
                    heroTag = "Player";
                }
                aggroProxy.tag = heroTag;

                var proxyCollider = aggroProxy.GetComponent<CapsuleCollider2D>();
                if (!proxyCollider)
                {
                    proxyCollider = aggroProxy.AddComponent<CapsuleCollider2D>();
                }

                proxyCollider.isTrigger = true;
                proxyCollider.direction = CapsuleDirection2D.Vertical;

                Vector2 size = new Vector2(0.9f, 1.4f);
                Vector2 offset = Vector2.zero;
                if (bodyCol is CapsuleCollider2D capsule)
                {
                    size = capsule.size;
                    offset = capsule.offset;
                }
                else if (bodyCol is BoxCollider2D box)
                {
                    size = box.size;
                    offset = box.offset;
                }
                else if (bodyCol)
                {
                    var bounds = bodyCol.bounds;
                    size = bounds.size;
                    offset = bounds.center - transform.position;
                }

                proxyCollider.size = size;
                proxyCollider.offset = offset;

                var tracker = aggroProxy.GetComponent<AggroProxyTracker>();
                if (!tracker)
                {
                    tracker = aggroProxy.AddComponent<AggroProxyTracker>();
                }
                tracker.Attach(this, proxyCollider);

                aggroProxyTracker = tracker;
                aggroProxyCollider = proxyCollider;

                bool desiredActive = !isInactive && isActiveAndEnabled && !assistModeEnabled;
                if (aggroProxyCollider.enabled != desiredActive)
                {
                    if (!desiredActive)
                    {
                        try { aggroProxyTracker?.ForceExitTrackedRemaskers(); } catch { }
                    }
                    aggroProxyCollider.enabled = desiredActive;
                }
            }
            catch
            {
            }
        }

        internal void ApplyCharmLoadout(IEnumerable<ShadeCharmDefinition> loadout)
        {
            var previousSnapshot = charmSnapshot;
            var previousEquipped = equippedCharms.ToArray();

            var sanitized = new List<ShadeCharmDefinition>();
            if (loadout != null)
            {
                foreach (var charm in loadout)
                {
                    if (charm == null)
                    {
                        continue;
                    }

                    sanitized.Add(charm);
                }
            }

            charmSnapshot = ShadeCharmCalculator.BuildSnapshot(s_defaultCharmStats, sanitized);
            abilityOverrides = charmSnapshot.AbilityOverrides;

            equippedCharms.Clear();
            equippedCharms.AddRange(charmSnapshot.Definitions);

            var currentDefinitions = charmSnapshot.Definitions;
            ShadeCharmDefinition[] removedCharms = Array.Empty<ShadeCharmDefinition>();
            if (previousEquipped.Length > 0)
            {
                var currentSet = new HashSet<ShadeCharmDefinition>(currentDefinitions);
                removedCharms = previousEquipped
                    .Where(charm => charm != null && !currentSet.Contains(charm))
                    .ToArray();
            }

            charmUpdateCallbacks.Clear();
            charmDamageCallbacks.Clear();
            foreach (var equipped in equippedCharms)
            {
                if (equipped.Hooks.OnUpdate != null)
                {
                    charmUpdateCallbacks.Add(equipped.Hooks.OnUpdate);
                }
                if (equipped.Hooks.OnShadeDamaged != null)
                {
                    charmDamageCallbacks.Add(equipped.Hooks.OnShadeDamaged);
                }
            }

            moveSpeed = charmSnapshot.MoveSpeed;
            sprintMultiplier = charmSnapshot.SprintMultiplier;
            sprintDashMultiplier = charmSnapshot.SprintDashMultiplier;
            sprintDashDuration = charmSnapshot.SprintDashDuration;
            sprintDashCooldown = charmSnapshot.SprintDashCooldown;
            fireCooldown = charmSnapshot.FireCooldown;
            nailCooldown = charmSnapshot.NailCooldown;
            shriekCooldown = charmSnapshot.ShriekCooldown;
            quakeCooldown = charmSnapshot.QuakeCooldown;
            teleportCooldown = charmSnapshot.TeleportCooldown;
            projectileSoulCost = charmSnapshot.ProjectileSoulCost;
            shriekSoulCost = charmSnapshot.ShriekSoulCost;
            quakeSoulCost = charmSnapshot.QuakeSoulCost;
            focusSoulCost = charmSnapshot.FocusSoulCost;

            int previousSoulMax = shadeSoulMax;
            shadeSoulMax = charmSnapshot.ShadeSoulCapacity;
            int clampedSoul = Mathf.Clamp(shadeSoul, 0, shadeSoulMax);
            bool soulAdjusted = clampedSoul != shadeSoul;
            shadeSoul = clampedSoul;
            lastSoulForReady = Mathf.Min(lastSoulForReady, shadeSoul);

            if (soulAdjusted || shadeSoulMax != previousSoulMax)
            {
                PushSoulToHud();
            }

            bool previousApplyingLoadout = applyingCharmLoadout;
            applyingCharmLoadout = true;
            try
            {
                if (removedCharms.Length > 0)
                {
                    var removedContext = new ShadeCharmContext(this, previousSnapshot);
                    foreach (var removed in removedCharms)
                    {
                        try { removed.Hooks.OnRemoved?.Invoke(removedContext); }
                        catch { }
                    }
                }

                if (equippedCharms.Count > 0)
                {
                    var appliedContext = new ShadeCharmContext(this, charmSnapshot);
                    foreach (var applied in equippedCharms)
                    {
                        try { applied.Hooks.OnApplied?.Invoke(appliedContext); }
                        catch { }
                    }
                }
            }
            finally
            {
                applyingCharmLoadout = previousApplyingLoadout;
            }

            if (pendingRestoredLifebloodMax >= 0)
            {
                int clamped = Mathf.Clamp(pendingRestoredLifeblood, 0, Mathf.Max(0, shadeLifebloodMax));
                if (shadeLifeblood != clamped)
                {
                    shadeLifeblood = clamped;
                    PushShadeStatsToHud(suppressDamageAudio: true);
                    PersistIfChanged();
                }
                pendingRestoredLifeblood = -1;
                pendingRestoredLifebloodMax = -1;
            }

            if (soulAdjusted || shadeSoulMax != previousSoulMax)
            {
                PersistIfChanged();
            }
        }

        internal void QueueCharmLoadoutRecompute()
        {
            if (baselineStatsInitialized)
            {
                RecomputeCharmLoadout();
            }
            else
            {
                pendingCharmLoadoutRecompute = true;
            }
        }

        private void EnterPersistenceSuppression()
        {
            persistenceSuppressionDepth++;
        }

        private void ExitPersistenceSuppression()
        {
            if (persistenceSuppressionDepth > 0)
            {
                persistenceSuppressionDepth--;
                if (persistenceSuppressionDepth == 0)
                {
                    FlushDeferredHealthSync();
                }
            }
        }

        private void FlushDeferredHealthSync()
        {
            if (!pendingDeferredHealthSync)
            {
                return;
            }

            bool suppressDamage = pendingDeferredHealthSuppressDamage;
            pendingDeferredHealthSync = false;
            pendingDeferredHealthSuppressDamage = false;
            PushShadeStatsToHud(suppressDamageAudio: suppressDamage);
            PersistIfChanged();
        }

        private void LoadShadeSprites()
        {
            loadedSpriteTextures.Clear();
            loadedSkinId = ShadeSkinManager.SelectedSkinId;
            try
            {
                string SpritePath(string fileName) => ShadeSkinManager.ResolveSpritePath(fileName);
                idleAnimFrames = LoadSpriteStrip(SpritePath("Shade_Idle_Sheet.png"), 9);
                floatAnimFrames = LoadSpriteStrip(SpritePath("Shade_Float_Sheet.png"), 6);
                vengefulAnimFrames = LoadSpriteStrip(SpritePath("Vengeful_Spirit_Sheet.png"), 2);
                shadeSoulAnimFrames = LoadSpriteStrip(SpritePath("Shade_Soul_Sheet.png"), 4);
                fireballCastAnimFrames = LoadSpriteStrip(SpritePath("Shade_Fireball_Cast_Sheet.png"), 4);
                quakeCastAnimFrames = LoadSpriteStrip(SpritePath("Shade_Quake_Cast_Sheet.png"), 2);
                shriekCastAnimFrames = LoadSpriteStrip(SpritePath("Shade_Shriek_Cast_Sheet.png"), 2);
                abyssShriekAnimFrames = LoadSpriteStrip(SpritePath("Abyss_Shriek_sheet.png"), 8);
                howlingWraithsAnimFrames = LoadSpriteStrip(SpritePath("Howling_Wraiths_Sheet.png"), 7);
                deathAnimFrames = LoadSpriteStrip(SpritePath("Shade_Death_Sheet.png"), 6);
                descendAnimFrames = LoadSpriteStrip(SpritePath("Shade_Descend_Sheet.png"), 3);
                descendAuraAnimFrames = LoadSpriteStrip(SpritePath("Quake_Descend_Aura_Sheet.png"), 3);
                dDiveSlamAnimFrames = LoadSpriteStrip(SpritePath("DDive_Slam_Sheet.png"), 2);
                dDarkSlamAnimFrames = LoadSpriteStrip(SpritePath("DDark_Slam_Sheet.png"), 6);
                dDarkBurstAnimFrames = LoadSpriteStrip(SpritePath("DDark_Burst_sheet.png"), 7);
                baldurShellFocusAnimFrames = LoadSpriteStrip(SpritePath("Shade_Baldur_Shell_Sheet.png"), 7);
                if (baldurShellFocusAnimFrames == null || baldurShellFocusAnimFrames.Length == 0)
                {
                    baldurShellFocusAnimFrames = LoadSpriteStrip(ModPaths.GetAssetPath("Baldur_sheet.png"), 7);
                }
                var inactive = LoadSpriteStrip(SpritePath("ShadeInactive.png"));
                inactiveSprite = inactive.Length > 0 ? inactive[0] : null;
            }
            catch
            {
                idleAnimFrames = System.Array.Empty<Sprite>();
                floatAnimFrames = System.Array.Empty<Sprite>();
                vengefulAnimFrames = System.Array.Empty<Sprite>();
                shadeSoulAnimFrames = System.Array.Empty<Sprite>();
                fireballCastAnimFrames = System.Array.Empty<Sprite>();
                quakeCastAnimFrames = System.Array.Empty<Sprite>();
                shriekCastAnimFrames = System.Array.Empty<Sprite>();
                abyssShriekAnimFrames = System.Array.Empty<Sprite>();
                howlingWraithsAnimFrames = System.Array.Empty<Sprite>();
                deathAnimFrames = System.Array.Empty<Sprite>();
                descendAnimFrames = System.Array.Empty<Sprite>();
                descendAuraAnimFrames = System.Array.Empty<Sprite>();
                dDiveSlamAnimFrames = System.Array.Empty<Sprite>();
                dDarkSlamAnimFrames = System.Array.Empty<Sprite>();
                dDarkBurstAnimFrames = System.Array.Empty<Sprite>();
                baldurShellFocusAnimFrames = System.Array.Empty<Sprite>();
                inactiveSprite = null;
            }
        }

        /// <summary>
        /// Reloads every sheet through the currently selected skin. Safe to call while the
        /// Shade is alive; the previous skin's textures are released once any in-flight VFX
        /// still holding them have had a chance to finish.
        /// </summary>
        internal void ReloadSkinSprites()
        {
            if (string.Equals(loadedSkinId, ShadeSkinManager.SelectedSkinId, StringComparison.OrdinalIgnoreCase))
                return;

            var previousTextures = new List<Texture2D>(loadedSpriteTextures);
            LoadShadeSprites();

            // Drop the cached array reference so HandleAnimation re-seeds from the new sheets.
            currentAnimFrames = null;
            animFrameIndex = 0;
            animTimer = 0f;
            if (sr != null)
            {
                if (isInactive && inactiveSprite != null)
                    sr.sprite = inactiveSprite;
                else if (idleAnimFrames != null && idleAnimFrames.Length > 0)
                    sr.sprite = idleAnimFrames[0];
                else if (inactiveSprite != null)
                    sr.sprite = inactiveSprite;
            }
            if (inactivePulseSr != null)
                inactivePulseSr.sprite = inactiveSprite;

            if (previousTextures.Count > 0)
            {
                if (isActiveAndEnabled)
                    StartCoroutine(ReleaseTexturesAfterDelay(previousTextures, RetiredSkinTextureLifetime));
                else
                    ReleaseTextures(previousTextures);
            }
        }

        private IEnumerator ReleaseTexturesAfterDelay(List<Texture2D> textures, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            ReleaseTextures(textures);
        }

        private static void ReleaseTextures(List<Texture2D> textures)
        {
            if (textures == null) return;
            foreach (var tex in textures)
            {
                if (tex != null)
                    UnityEngine.Object.Destroy(tex);
            }
            textures.Clear();
        }

        private Sprite[] LoadSpriteStrip(string path, int frames = 0)
        {
            if (!File.Exists(path)) return System.Array.Empty<Sprite>();
            var bytes = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            TryLoadImage(tex, bytes);
            tex.filterMode = FilterMode.Point;
            loadedSpriteTextures.Add(tex);
            int cols = frames > 0 ? frames : Mathf.Max(1, tex.width / tex.height);
            int frameWidth = tex.width / cols;
            int frameHeight = tex.height;
            var sprites = new Sprite[cols];
            for (int i = 0; i < cols; i++)
                sprites[i] = Sprite.Create(tex, new Rect(i * frameWidth, 0, frameWidth, frameHeight), new Vector2(0.5f, 0.5f));
            return sprites;
        }

        public void TriggerSpawnEntrance()
        {
            pendingSpawnAnimation = true;
            TryPlaySpawnAnimation();
        }

        private void TryPlaySpawnAnimation()
        {
            if (!pendingSpawnAnimation)
                return;
            if (!isActiveAndEnabled)
                return;
            if (!sr)
                sr = GetComponent<SpriteRenderer>();
            if (sr == null)
                return;
            if (deathAnimFrames == null || deathAnimFrames.Length == 0)
                return;

            StopSpawnAnimation();
            spawnRoutine = StartCoroutine(SpawnAppearanceRoutine());
            pendingSpawnAnimation = false;
        }

        private void StopSpawnAnimation()
        {
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }
            isSpawning = false;
            pendingSpawnAnimation = false;
        }

        private IEnumerator SpawnAppearanceRoutine()
        {
            isSpawning = true;
            var frames = deathAnimFrames;
            if (frames != null && frames.Length > 0)
            {
                float perFrame = 0.5f / frames.Length;
                for (int i = frames.Length - 1; i >= 0; i--)
                {
                    if (sr != null)
                        sr.sprite = frames[i];
                    yield return new WaitForSeconds(perFrame);
                }
            }
            else
            {
                yield return null;
            }
            spawnRoutine = null;
            isSpawning = false;
            currentAnimFrames = null;
            if (sr != null)
            {
                var c = sr.color;
                c.a = 0.9f;
                sr.color = c;
                if (idleAnimFrames != null && idleAnimFrames.Length > 0)
                {
                    sr.sprite = idleAnimFrames[0];
                    animFrameIndex = 0;
                    animTimer = 0f;
                }
            }
        }

        private static bool TryLoadImage(Texture2D tex, byte[] bytes)
        {
            try
            {
                var t = System.Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
                if (t != null)
                {
                    var m = t.GetMethod("LoadImage", BindingFlags.Public | BindingFlags.Static, null, new System.Type[] { typeof(Texture2D), typeof(byte[]), typeof(bool) }, null);
                    if (m != null) { m.Invoke(null, new object[] { tex, bytes, false }); return true; }
                }
            }
            catch { }
            return false;
        }

        private void EnsureInactivePulse()
        {
            if (inactivePulseSr != null) return;
            var pulseGO = new GameObject("InactivePulse");
            pulseGO.transform.SetParent(transform, false);
            pulseGO.transform.localPosition = Vector3.zero;
            inactivePulseSr = pulseGO.AddComponent<SpriteRenderer>();
            if (sr != null)
            {
                inactivePulseSr.sortingLayerID = sr.sortingLayerID;
                inactivePulseSr.sortingOrder = sr.sortingOrder - 1;
            }
            inactivePulseSr.color = new Color(1f, 1f, 1f, 0f);
        }

        private void HandleAnimation()
        {
            if (sr == null) return;
            sr.flipX = (facing == 1);

            if (isSpawning)
                return;

            if (isCastingSpell && currentAnimFrames != null)
                return;

            if (isInactive)
            {
                if (inactiveSprite != null) sr.sprite = inactiveSprite;
                var cInact = sr.color;
                cInact.a = 0.8f + 0.2f * Mathf.Sin(Time.time * 3f);
                sr.color = cInact;
                EnsureInactivePulse();
                if (inactivePulseSr != null)
                {
                    inactivePulseSr.gameObject.SetActive(true);
                    inactivePulseSr.sprite = inactiveSprite;
                    float alpha = 0.35f + 0.25f * Mathf.Sin(Time.time * 3f);
                    var pc = inactivePulseSr.color;
                    pc.a = alpha;
                    inactivePulseSr.color = pc;
                    float scale = 1.05f + 0.03f * Mathf.Sin(Time.time * 2f);
                    inactivePulseSr.transform.localScale = new Vector3(scale, scale, 1f);
                }
                return;
            }
            else if (inactivePulseSr != null)
            {
                inactivePulseSr.gameObject.SetActive(false);
            }

            var c = sr.color; c.a = 0.9f; sr.color = c;
            Sprite[] frames = (Mathf.Abs(lastMoveDelta.x) > 0.01f) ? floatAnimFrames : idleAnimFrames;
            if (frames == null || frames.Length == 0) return;

            if (currentAnimFrames != frames)
            {
                currentAnimFrames = frames;
                animFrameIndex = 0;
                animTimer = 0f;
                sr.sprite = frames[0];
            }

            animTimer += Time.deltaTime;
            if (animTimer >= AnimFrameTime)
            {
                animTimer -= AnimFrameTime;
                animFrameIndex = (animFrameIndex + 1) % frames.Length;
                sr.sprite = frames[animFrameIndex];
            }
        }

        private void EnsurePogoTarget()
        {
            try
            {
                var pogo = transform.Find("PogoTarget")?.gameObject;
                if (pogo == null)
                {
                    pogo = new GameObject("PogoTarget");
                    pogo.transform.SetParent(transform, false);
                    pogo.transform.localPosition = Vector3.zero;
                }

                // Ensure collider present (trigger) so slash damager trigger can overlap
                var col = pogo.GetComponent<Collider2D>();
                if (!col)
                {
                    var cap = pogo.AddComponent<CapsuleCollider2D>();
                    cap.direction = CapsuleDirection2D.Vertical;
                    cap.size = new Vector2(0.95f, 1.45f);
                    cap.isTrigger = true;
                    col = cap;
                }
                else col.isTrigger = true;

                // Put on INTERACTIVE_OBJECT (or Default) so DownAttack's HitResponded path will process and allow bounce
                int interactiveLayer = LayerMask.NameToLayer("Interactive Object");
                if (interactiveLayer < 0) interactiveLayer = LayerMask.NameToLayer("Default");
                if (interactiveLayer < 0) interactiveLayer = 0;
                pogo.layer = interactiveLayer;

                // Attach HitResponse and configure to accept hero nail hits downward only
                // Optional puff-on-collision helper
                if (!pogo.GetComponent<ShadePogoPuff>()) pogo.AddComponent<ShadePogoPuff>();

                // Make sure shade's own rigidbody exists for trigger events
                if (rb) { /* already present */ }
            }
            catch { }
        }

    }
}
#nullable restore
