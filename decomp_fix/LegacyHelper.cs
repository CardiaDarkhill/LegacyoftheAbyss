using System;
using System.Collections;
using System.Reflection;
using BepInEx;
using GlobalEnums;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[BepInPlugin("com.legacyoftheabyss.helper", "Legacy of the Abyss - Helper", "0.1.0")]
public class LegacyHelper : BaseUnityPlugin
{
	[HarmonyPatch(typeof(GameManager), "BeginScene")]
	private class GameManager_BeginScene_Patch
	{
		private static void Postfix(GameManager __instance)
		{
			DisableStartup(__instance);
			bool flag = __instance.IsGameplayScene();
			if (hud != null)
			{
				try
				{
					hud.SetVisible(flag);
				}
				catch
				{
				}
			}
			if (!flag)
			{
				return;
			}
			if (!registeredEnterSceneHandler)
			{
				try
				{
					__instance.OnFinishedEnteringScene += HandleFinishedEnteringScene;
					registeredEnterSceneHandler = true;
				}
				catch
				{
				}
			}
			if (hud == null)
			{
				GameObject gameObject = new GameObject("SimpleHUD");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				hud = gameObject.AddComponent<SimpleHUD>();
				hud.Init(__instance.playerData);
				return;
			}
			try
			{
				hud.SetPlayerData(__instance.playerData);
			}
			catch
			{
			}
		}
	}

	[HarmonyPatch(typeof(GameManager), "Awake")]
	private class GameManager_Awake_Patch
	{
		private static void Postfix(GameManager __instance)
		{
			DisableStartup(__instance);
		}
	}

	[HarmonyPatch(typeof(GameManager), "Start")]
	private class GameManager_Start_Patch
	{
		private static void Postfix(GameManager __instance)
		{
			DisableStartup(__instance);
		}
	}

	[HarmonyPatch(typeof(StartManager), "Start")]
	private class StartManager_Start_Enumerator_Patch
	{
		private static void Prefix(StartManager __instance)
		{
			if (__instance.startManagerAnimator != null)
			{
				__instance.startManagerAnimator.SetBool("WillShowQuote", value: false);
			}
		}

		private static void Postfix(StartManager __instance, ref IEnumerator __result)
		{
			if (__result == null)
			{
				return;
			}
			FieldInfo[] fields = __result.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType == typeof(bool) && fieldInfo.Name.Contains("showIntroSequence"))
				{
					fieldInfo.SetValue(__result, false);
					if (__instance.startManagerAnimator != null)
					{
						__instance.startManagerAnimator.Play("LoadingIcon", 0, 1f);
					}
					break;
				}
			}
		}
	}

	[HarmonyPatch(typeof(RestBenchHelper), "SetOnBench")]
	private class RestBenchHelper_SetOnBench_Patch
	{
		private static void Postfix(bool onBench)
		{
			if (!onBench)
			{
				return;
			}
			try
			{
				if (helper != null)
				{
					ShadeController component = helper.GetComponent<ShadeController>();
					if (component != null)
					{
						component.FullHealFromBench();
						SaveShadeState(component.GetCurrentHP(), component.GetMaxHP(), component.GetShadeSoul());
					}
				}
			}
			catch
			{
			}
		}
	}

	[HarmonyPatch(typeof(HeroController), "BindCompleted")]
	private class HeroController_BindCompleted_Patch
	{
		private static void Postfix(HeroController __instance)
		{
			try
			{
				if (helper != null)
				{
					ShadeController component = helper.GetComponent<ShadeController>();
					if (component != null)
					{
						component.ApplyBindHealFromHornet((__instance != null) ? __instance.transform : null);
					}
				}
			}
			catch
			{
			}
		}
	}

	[HarmonyPatch(typeof(InputHandler), "MapKeyboardLayoutFromGameSettings")]
	private class BlockKeyboardRebinding
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[HarmonyPatch(typeof(InputHandler), "MapDefaultKeyboardLayout")]
	private class BlockDefaultKeyboardMap
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	public class ShadeProjectile : MonoBehaviour
	{
		public int damage = 20;

		public Transform hornetRoot;

		public float lifeSeconds = 1.5f;

		private bool hasHit;

		private void Start()
		{
			UnityEngine.Object.Destroy(base.gameObject, lifeSeconds);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!hasHit && !(other == null) && (!(hornetRoot != null) || !other.transform.IsChildOf(hornetRoot)) && !(other.transform == base.transform) && !other.transform.IsChildOf(base.transform))
			{
				hasHit = true;
				Rigidbody2D component = GetComponent<Rigidbody2D>();
				Vector2 vector = (component ? component.linearVelocity : ((Vector2)(other.transform.position - base.transform.position)));
				float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				if (num < 0f)
				{
					num += 360f;
				}
				HitInstance hitInstance = default(HitInstance);
				hitInstance.Source = base.gameObject;
				hitInstance.AttackType = AttackTypes.Spell;
				hitInstance.DamageDealt = damage;
				hitInstance.Direction = num;
				hitInstance.MagnitudeMultiplier = 1f;
				hitInstance.Multiplier = 1f;
				hitInstance.IsHeroDamage = true;
				hitInstance.IsFirstHit = true;
				HitInstance hitInstance2 = hitInstance;
				HitTaker.Hit(other.gameObject, hitInstance2);
				if (HitTaker.TryGetHealthManager(other.gameObject, out var healthManager))
				{
					healthManager.Hit(hitInstance2);
				}
				StartCoroutine(DestroyNextFrame());
			}
		}

		private IEnumerator DestroyNextFrame()
		{
			yield return null;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public class ShadeController : MonoBehaviour
	{
		public float moveSpeed = 8f;

		public float maxDistance = 14f;

		public float softLeashRadius = 10f;

		public float hardLeashRadius = 22f;

		public float snapLeashRadius = 38f;

		public float softPullSpeed = 6f;

		public float hardPullSpeed = 30f;

		public float hardLeashTimeout = 2.5f;

		private bool inHardLeash;

		private float hardLeashTimer;

		private Rigidbody2D rb;

		private Collider2D bodyCol;

		private int shadeMaxHP;

		private int shadeHP;

		private float hazardCooldown;

		public float projectileSpeed = 22f;

		public float fireCooldown = 0.25f;

		public float nailCooldown = 0.3f;

		public Vector2 muzzleOffset = new Vector2(0.9f, 0f);

		private Transform hornetTransform;

		private float fireTimer;

		private SpriteRenderer sr;

		private Renderer[] shadeLightRenderers;

		public float simpleLightSize = 14f;

		private static Texture2D s_simpleLightTex;

		private static Material s_simpleAdditiveMat;

		private static Mesh s_simpleQuadMesh;

		private int facing = 1;

		private float nailTimer;

		private const KeyCode FireKey = KeyCode.Space;

		private const KeyCode NailKey = KeyCode.J;

		public int shadeSoulMax = 99;

		public int shadeSoul;

		public int soulGainPerHit = 11;

		public int projectileSoulCost = 33;

		private SimpleHUD cachedHud;

		private float hurtCooldown;

		private const float HurtIFrameSeconds = 1.35f;

		private int lastSavedHP;

		private int lastSavedMax;

		private int lastSavedSoul;

		public void RestorePersistentState(int hp, int max, int soul)
		{
			shadeMaxHP = Mathf.Max(1, max);
			shadeHP = Mathf.Clamp(hp, 0, shadeMaxHP);
			shadeSoul = Mathf.Clamp(soul, 0, shadeSoulMax);
		}

		public void FullHealFromBench()
		{
			shadeHP = Mathf.Max(shadeHP, shadeMaxHP);
			PushShadeStatsToHud();
		}

		public int GetCurrentHP()
		{
			return shadeHP;
		}

		public int GetMaxHP()
		{
			return shadeMaxHP;
		}

		public int GetShadeSoul()
		{
			return shadeSoul;
		}

		public void Init(Transform hornet)
		{
			hornetTransform = hornet;
		}

		private void Start()
		{
			SetupPhysics();
			if (hornetTransform == null)
			{
				GameObject gameObject = GameObject.FindWithTag("Player");
				if (gameObject != null)
				{
					hornetTransform = gameObject.transform;
				}
			}
			sr = GetComponent<SpriteRenderer>();
			if (sr != null)
			{
				Color color = sr.color;
				color.a = 0.9f;
				sr.color = color;
			}
			SetupShadeLight();
			cachedHud = UnityEngine.Object.FindFirstObjectByType<SimpleHUD>();
			PushSoulToHud();
			CheckHazardOverlap();
			try
			{
				PlayerData playerData = ((GameManager.instance != null) ? GameManager.instance.playerData : null);
				if (playerData != null)
				{
					int num = Mathf.Max(1, (playerData.maxHealth + 1) / 2);
					shadeMaxHP = num;
					if (!HasSavedShadeState && shadeHP <= 0)
					{
						shadeHP = Mathf.Clamp((playerData.health + 1) / 2, 0, shadeMaxHP);
					}
					shadeHP = Mathf.Clamp(shadeHP, 0, shadeMaxHP);
					PushShadeStatsToHud();
				}
			}
			catch
			{
			}
			lastSavedHP = (lastSavedMax = (lastSavedSoul = -999));
			PersistIfChanged();
		}

		private void Update()
		{
			if (!(hornetTransform == null))
			{
				if (hazardCooldown > 0f)
				{
					hazardCooldown = Mathf.Max(0f, hazardCooldown - Time.deltaTime);
				}
				if (hurtCooldown > 0f)
				{
					hurtCooldown = Mathf.Max(0f, hurtCooldown - Time.deltaTime);
				}
				HandleMovementAndFacing();
				if (!inHardLeash)
				{
					HandleFire();
					HandleNailAttack();
				}
				if (!cachedHud)
				{
					cachedHud = UnityEngine.Object.FindFirstObjectByType<SimpleHUD>();
				}
				PushSoulToHud();
				CheckHazardOverlap();
				SyncShadeLight();
				PersistIfChanged();
			}
		}

		public void ApplyBindHealFromHornet(Transform hornet)
		{
			try
			{
				Transform transform = ((hornet != null) ? hornet : hornetTransform);
				if (!(transform == null) && Vector2.Distance(transform.position, base.transform.position) <= 3.5f)
				{
					int num = shadeHP;
					shadeHP = Mathf.Min(shadeHP + 2, shadeMaxHP);
					if (shadeHP != num)
					{
						PushShadeStatsToHud();
						PersistIfChanged();
					}
				}
			}
			catch
			{
			}
		}

		private void PersistIfChanged()
		{
			if (lastSavedHP != shadeHP || lastSavedMax != shadeMaxHP || lastSavedSoul != shadeSoul)
			{
				SaveShadeState(shadeHP, shadeMaxHP, shadeSoul);
				lastSavedHP = shadeHP;
				lastSavedMax = shadeMaxHP;
				lastSavedSoul = shadeSoul;
			}
		}

		private void PushSoulToHud()
		{
			if ((bool)cachedHud)
			{
				try
				{
					cachedHud.SetShadeSoul(shadeSoul, shadeSoulMax);
				}
				catch
				{
				}
			}
		}

		private void PushShadeStatsToHud()
		{
			if ((bool)cachedHud)
			{
				try
				{
					cachedHud.SetShadeStats(shadeHP, shadeMaxHP);
				}
				catch
				{
				}
			}
		}

		private void HandleMovementAndFacing()
		{
			float num = (Input.GetKey(KeyCode.A) ? (-1f) : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
			float y = (Input.GetKey(KeyCode.S) ? (-1f) : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
			Vector2 vector = new Vector2(num, y);
			if (vector.sqrMagnitude > 1f)
			{
				vector.Normalize();
			}
			Vector2 vector2 = hornetTransform.position - base.transform.position;
			float magnitude = vector2.magnitude;
			if (magnitude > snapLeashRadius)
			{
				TeleportToHornet();
				inHardLeash = false;
				hardLeashTimer = 0f;
				EnableCollisions(enable: true);
				return;
			}
			Vector2 vector3 = Vector2.zero;
			if (magnitude > softLeashRadius && magnitude <= hardLeashRadius)
			{
				float t = Mathf.InverseLerp(softLeashRadius, hardLeashRadius, magnitude);
				Vector2 normalized = vector2.normalized;
				vector3 += normalized * Mathf.Lerp(softPullSpeed, softPullSpeed * 1.5f, t) * Time.deltaTime;
				inHardLeash = false;
				hardLeashTimer = 0f;
				EnableCollisions(enable: true);
			}
			if (magnitude > hardLeashRadius)
			{
				inHardLeash = true;
				hardLeashTimer += Time.deltaTime;
				EnableCollisions(enable: false);
				vector3 = vector2.normalized * hardPullSpeed * Time.deltaTime;
				if (hardLeashTimer >= hardLeashTimeout)
				{
					TeleportToHornet();
					inHardLeash = false;
					hardLeashTimer = 0f;
					EnableCollisions(enable: true);
					return;
				}
			}
			else if (inHardLeash)
			{
				inHardLeash = false;
				hardLeashTimer = 0f;
				EnableCollisions(enable: true);
			}
			if (!inHardLeash)
			{
				vector3 += vector * moveSpeed * Time.deltaTime;
			}
			if ((bool)rb)
			{
				rb.MovePosition(rb.position + vector3);
			}
			else
			{
				base.transform.position += (Vector3)vector3;
			}
			if (num > 0.1f)
			{
				facing = 1;
			}
			else if (num < -0.1f)
			{
				facing = -1;
			}
			else if (Mathf.Abs(vector2.x) > 0.1f)
			{
				facing = ((vector2.x >= 0f) ? 1 : (-1));
			}
			if (sr != null)
			{
				sr.flipX = facing == -1;
			}
			if (magnitude > maxDistance)
			{
				Vector3 vector4 = base.transform.position - hornetTransform.position;
				base.transform.position = hornetTransform.position + vector4.normalized * maxDistance;
			}
		}

		private void HandleFire()
		{
			fireTimer -= Time.deltaTime;
			if (Input.GetKey(KeyCode.Space) && !(fireTimer > 0f) && shadeSoul >= projectileSoulCost)
			{
				fireTimer = fireCooldown;
				shadeSoul = Mathf.Max(0, shadeSoul - projectileSoulCost);
				PushSoulToHud();
				CheckHazardOverlap();
				Vector2 dir = new Vector2(facing, 0f);
				SpawnProjectile(dir);
			}
		}

		private void SetupPhysics()
		{
			rb = GetComponent<Rigidbody2D>();
			if (!rb)
			{
				rb = base.gameObject.AddComponent<Rigidbody2D>();
			}
			rb.bodyType = RigidbodyType2D.Dynamic;
			rb.gravityScale = 0f;
			rb.freezeRotation = true;
			rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			bodyCol = GetComponent<Collider2D>();
			if (!bodyCol)
			{
				CapsuleCollider2D capsuleCollider2D = base.gameObject.AddComponent<CapsuleCollider2D>();
				capsuleCollider2D.direction = CapsuleDirection2D.Vertical;
				capsuleCollider2D.size = new Vector2(0.9f, 1.4f);
				bodyCol = capsuleCollider2D;
			}
			else
			{
				bodyCol.isTrigger = false;
			}
			try
			{
				HeroController instance = HeroController.instance;
				if (!instance)
				{
					return;
				}
				base.gameObject.layer = instance.gameObject.layer;
				Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>(includeInactive: true);
				Collider2D[] componentsInChildren2 = instance.GetComponentsInChildren<Collider2D>(includeInactive: true);
				Collider2D[] array = componentsInChildren;
				foreach (Collider2D collider2D in array)
				{
					Collider2D[] array2 = componentsInChildren2;
					foreach (Collider2D collider2D2 in array2)
					{
						if ((bool)collider2D && (bool)collider2D2)
						{
							Physics2D.IgnoreCollision(collider2D, collider2D2, ignore: true);
						}
					}
				}
			}
			catch
			{
			}
		}

		private void EnableCollisions(bool enable)
		{
			try
			{
				if ((bool)bodyCol)
				{
					bodyCol.enabled = enable;
				}
				Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>(includeInactive: true);
				foreach (Collider2D collider2D in componentsInChildren)
				{
					if ((bool)collider2D && collider2D != bodyCol)
					{
						collider2D.enabled = enable;
					}
				}
			}
			catch
			{
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			TryProcessDamageHero(collision.collider);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			TryProcessDamageHero(other);
		}

		private void TryProcessDamageHero(Collider2D col)
		{
			if (!col)
			{
				return;
			}
			try
			{
				if (((bool)bodyCol && (bool)col && !bodyCol.IsTouching(col)) || col.transform == base.transform || col.transform.IsChildOf(base.transform) || ((bool)hornetTransform && (col.transform == hornetTransform || col.transform.IsChildOf(hornetTransform))))
				{
					return;
				}
				DamageHero componentInParent = col.GetComponentInParent<DamageHero>();
				if (componentInParent != null)
				{
					if (IsTerrainHazard(GetHazardType(componentInParent)))
					{
						OnShadeHitHazard();
					}
					else
					{
						OnShadeHitEnemy(componentInParent);
					}
				}
			}
			catch
			{
			}
		}

		private void TeleportToHornet()
		{
			if ((bool)hornetTransform)
			{
				bool simulated = (bool)rb && rb.simulated;
				if ((bool)rb)
				{
					rb.simulated = false;
				}
				base.transform.position = hornetTransform.position;
				if ((bool)rb)
				{
					rb.linearVelocity = Vector2.zero;
					rb.simulated = simulated;
				}
			}
		}

		public void TeleportToPosition(Vector3 position)
		{
			bool simulated = (bool)rb && rb.simulated;
			if ((bool)rb)
			{
				rb.simulated = false;
			}
			base.transform.position = position;
			if ((bool)rb)
			{
				rb.linearVelocity = Vector2.zero;
				rb.simulated = simulated;
			}
		}

		private void CheckHazardOverlap()
		{
			if (hazardCooldown > 0f || !bodyCol)
			{
				return;
			}
			ContactFilter2D contactFilter = default(ContactFilter2D);
			contactFilter.useTriggers = true;
			Collider2D[] array = new Collider2D[16];
			int num = bodyCol.Overlap(contactFilter, array);
			for (int i = 0; i < num; i++)
			{
				Collider2D collider2D = array[i];
				if (!collider2D || collider2D.transform == base.transform || collider2D.transform.IsChildOf(base.transform) || ((bool)hornetTransform && (collider2D.transform == hornetTransform || collider2D.transform.IsChildOf(hornetTransform))))
				{
					continue;
				}
				DamageHero componentInParent = collider2D.GetComponentInParent<DamageHero>();
				if (componentInParent != null)
				{
					if (IsTerrainHazard(GetHazardType(componentInParent)))
					{
						OnShadeHitHazard();
					}
					else
					{
						OnShadeHitEnemy(componentInParent);
					}
					break;
				}
			}
		}

		private static HazardType GetHazardType(DamageHero dh)
		{
			try
			{
				FieldInfo field = typeof(DamageHero).GetField("hazardType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					return (HazardType)field.GetValue(dh);
				}
			}
			catch
			{
			}
			return HazardType.NON_HAZARD;
		}

		private void OnShadeHitHazard()
		{
			if (!(hazardCooldown > 0f))
			{
				TeleportToHornet();
				shadeHP = Mathf.Max(0, shadeHP - 1);
				PushShadeStatsToHud();
				hazardCooldown = 0.25f;
				PersistIfChanged();
			}
		}

		private void OnShadeHitEnemy(DamageHero dh)
		{
			if (hurtCooldown > 0f)
			{
				return;
			}
			int num = 1;
			try
			{
				if (dh != null)
				{
					num = Mathf.Max(1, dh.damageDealt);
				}
			}
			catch
			{
			}
			shadeHP = Mathf.Max(0, shadeHP - num);
			PushShadeStatsToHud();
			hurtCooldown = 1.35f;
			PersistIfChanged();
		}

		private void SetupShadeLight()
		{
			try
			{
				GameObject obj = new GameObject("ShadeLightSimple");
				obj.transform.SetParent(base.transform, worldPositionStays: false);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = Quaternion.identity;
				EnsureSimpleLightResources();
				obj.AddComponent<MeshFilter>().sharedMesh = s_simpleQuadMesh;
				MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = s_simpleAdditiveMat;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer.receiveShadows = false;
				SpriteRenderer component = GetComponent<SpriteRenderer>();
				meshRenderer.sortingLayerID = (component ? component.sortingLayerID : 0);
				meshRenderer.sortingOrder = (component ? (component.sortingOrder - 1) : (-1));
				obj.transform.localScale = new Vector3(simpleLightSize, simpleLightSize, 1f);
				shadeLightRenderers = new Renderer[1] { meshRenderer };
			}
			catch
			{
			}
		}

		private void SyncShadeLight()
		{
			try
			{
				if (shadeLightRenderers == null)
				{
					return;
				}
				SpriteRenderer component = GetComponent<SpriteRenderer>();
				int sortingLayerID = (component ? component.sortingLayerID : 0);
				int num = (component ? component.sortingOrder : 0);
				Renderer[] array = shadeLightRenderers;
				foreach (Renderer renderer in array)
				{
					if ((bool)renderer)
					{
						renderer.enabled = true;
						renderer.sortingLayerID = sortingLayerID;
						renderer.sortingOrder = num - 1;
					}
				}
			}
			catch
			{
			}
		}

		private IEnumerator EnableShadeLightNextFrame()
		{
			yield return null;
		}

		private static void EnsureSimpleLightResources()
		{
			try
			{
				if (s_simpleQuadMesh == null)
				{
					s_simpleQuadMesh = new Mesh();
					s_simpleQuadMesh.name = "ShadeLightQuad";
					s_simpleQuadMesh.vertices = new Vector3[4]
					{
						new Vector3(-0.5f, -0.5f, 0f),
						new Vector3(0.5f, -0.5f, 0f),
						new Vector3(-0.5f, 0.5f, 0f),
						new Vector3(0.5f, 0.5f, 0f)
					};
					s_simpleQuadMesh.uv = new Vector2[4]
					{
						new Vector2(0f, 0f),
						new Vector2(1f, 0f),
						new Vector2(0f, 1f),
						new Vector2(1f, 1f)
					};
					s_simpleQuadMesh.triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
					s_simpleQuadMesh.RecalculateNormals();
				}
				if (s_simpleLightTex == null)
				{
					int num = 128;
					s_simpleLightTex = new Texture2D(num, num, TextureFormat.ARGB32, mipChain: false);
					s_simpleLightTex.filterMode = FilterMode.Bilinear;
					for (int i = 0; i < num; i++)
					{
						for (int j = 0; j < num; j++)
						{
							float num2 = ((float)j + 0.5f) / (float)num * 2f - 1f;
							float num3 = ((float)i + 0.5f) / (float)num * 2f - 1f;
							float num4 = Mathf.Sqrt(num2 * num2 + num3 * num3);
							float f = Mathf.Clamp01(1f - num4);
							f = Mathf.Pow(f, 3.5f) * 0.55f;
							s_simpleLightTex.SetPixel(j, i, new Color(1f, 1f, 1f, f));
						}
					}
					s_simpleLightTex.Apply();
				}
				if (s_simpleAdditiveMat == null)
				{
					s_simpleAdditiveMat = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Transparent"))
					{
						name = "ShadeLightAdditiveMat",
						mainTexture = s_simpleLightTex,
						renderQueue = 3000
					};
					try
					{
						s_simpleAdditiveMat.SetColor("_Color", new Color(1f, 1f, 1f, 0.35f));
						return;
					}
					catch
					{
						return;
					}
				}
			}
			catch
			{
			}
		}

		private static bool IsTerrainHazard(HazardType hz)
		{
			if ((uint)(hz - 2) <= 5u || (uint)(hz - 9) <= 3u)
			{
				return true;
			}
			return false;
		}

		private void HandleNailAttack()
		{
			nailTimer -= Time.deltaTime;
			if (!(nailTimer > 0f) && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)))
			{
				nailTimer = nailCooldown;
				PerformNailSlash();
			}
		}

		private void PerformNailSlash()
		{
			HeroController instance = HeroController.instance;
			if (instance == null)
			{
				return;
			}
			GameObject gameObject = null;
			float num = (Input.GetKey(KeyCode.S) ? (-1f) : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);
			FieldInfo field = instance.GetType().GetField("UpSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo field2 = instance.GetType().GetField("DownSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo field3 = instance.GetType().GetField("NormalSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo field4 = instance.GetType().GetField("AlternateSlashObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			GameObject gameObject2 = field?.GetValue(instance) as GameObject;
			GameObject gameObject3 = field2?.GetValue(instance) as GameObject;
			GameObject gameObject4 = field3?.GetValue(instance) as GameObject;
			GameObject gameObject5 = field4?.GetValue(instance) as GameObject;
			NailSlash nailSlash = null;
			NailSlash nailSlash2 = null;
			NailSlash nailSlash3 = null;
			NailSlash nailSlash4 = null;
			try
			{
				nailSlash = instance.GetType().GetProperty("UpSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			try
			{
				nailSlash2 = instance.GetType().GetProperty("DownSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			try
			{
				nailSlash3 = instance.GetType().GetProperty("NormalSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			try
			{
				nailSlash4 = instance.GetType().GetProperty("AlternateSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			GameObject gameObject6 = (nailSlash ? nailSlash.gameObject : null);
			GameObject gameObject7 = (nailSlash2 ? nailSlash2.gameObject : null);
			GameObject gameObject8 = (nailSlash3 ? nailSlash3.gameObject : null);
			GameObject gameObject9 = (nailSlash4 ? nailSlash4.gameObject : null);
			GameObject gameObject10 = gameObject2 ?? gameObject6;
			GameObject gameObject11 = gameObject3 ?? gameObject7;
			GameObject gameObject12 = gameObject4 ?? gameObject8;
			GameObject gameObject13 = gameObject5 ?? gameObject9;
			gameObject = ((num > 0.35f && (bool)gameObject10) ? gameObject10 : ((!(num < -0.35f) || !gameObject11) ? ((facing >= 0) ? (gameObject13 ?? gameObject12) : (gameObject12 ?? gameObject13)) : gameObject11));
			if (gameObject == null)
			{
				try
				{
					NailSlash[] array = instance.GetComponentsInChildren<NailSlash>(includeInactive: true);
					NailSlash nailSlash5 = null;
					if (array == null || array.Length == 0)
					{
						array = Resources.FindObjectsOfTypeAll<NailSlash>();
					}
					if (array != null && array.Length != 0)
					{
						nailSlash5 = ((num > 0.35f) ? Array.Find(array, (NailSlash s) => MatchUp(s)) : ((!(num < -0.35f)) ? ((facing >= 0) ? (Array.Find(array, (NailSlash s) => MatchNormal(s) && MatchRight(s)) ?? Array.Find(array, (NailSlash s) => MatchRight(s))) : (Array.Find(array, (NailSlash s) => MatchNormal(s) && MatchLeft(s)) ?? Array.Find(array, (NailSlash s) => MatchLeft(s)))) : Array.Find(array, (NailSlash s) => MatchDown(s))));
						if (nailSlash5 == null)
						{
							nailSlash5 = Array.Find(array, (NailSlash s) => MatchNormal(s)) ?? array[0];
						}
						gameObject = (nailSlash5 ? nailSlash5.gameObject : null);
					}
				}
				catch
				{
				}
				if (gameObject == null)
				{
					return;
				}
			}
			GameObject gameObject14 = UnityEngine.Object.Instantiate(gameObject, instance.transform);
			gameObject14.transform.position = base.transform.position;
			gameObject14.transform.SetParent(base.transform, worldPositionStays: true);
			Collider2D[] componentsInChildren = gameObject14.GetComponentsInChildren<Collider2D>(includeInactive: true);
			Collider2D[] array2 = componentsInChildren;
			foreach (Collider2D collider2D in array2)
			{
				if ((bool)collider2D)
				{
					collider2D.enabled = false;
				}
			}
			DamageEnemies[] componentsInChildren2 = gameObject14.GetComponentsInChildren<DamageEnemies>(includeInactive: true);
			DamageEnemies[] array3 = componentsInChildren2;
			foreach (DamageEnemies damageEnemies in array3)
			{
				if ((bool)damageEnemies)
				{
					damageEnemies.enabled = false;
				}
			}
			try
			{
				int layer = gameObject.layer;
				Transform[] componentsInChildren3 = gameObject14.GetComponentsInChildren<Transform>(includeInactive: true);
				foreach (Transform transform in componentsInChildren3)
				{
					if ((bool)transform)
					{
						transform.gameObject.layer = layer;
						transform.gameObject.tag = "Untagged";
					}
				}
			}
			catch
			{
			}
			try
			{
				Transform obj7 = gameObject14.transform;
				Vector3 localScale = obj7.localScale;
				localScale.x = Mathf.Abs(localScale.x) * ((facing >= 0) ? 1f : (-1f));
				obj7.localScale = localScale;
			}
			catch
			{
			}
			NailSlash component = gameObject14.GetComponent<NailSlash>();
			if (component != null)
			{
				typeof(NailAttackBase).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(component, instance);
				try
				{
					NailSlashTravel component2 = gameObject14.GetComponent<NailSlashTravel>();
					if (component2 != null)
					{
						typeof(NailSlashTravel).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(component2, instance);
					}
				}
				catch
				{
				}
				try
				{
					NailSlashRecoil[] componentsInChildren4 = gameObject14.GetComponentsInChildren<NailSlashRecoil>(includeInactive: true);
					foreach (NailSlashRecoil nailSlashRecoil in componentsInChildren4)
					{
						if ((bool)nailSlashRecoil)
						{
							UnityEngine.Object.Destroy(nailSlashRecoil);
						}
					}
				}
				catch
				{
				}
				try
				{
					DamageEnemies[] componentsInChildren5 = gameObject14.GetComponentsInChildren<DamageEnemies>(includeInactive: true);
					FieldInfo field5 = typeof(DamageEnemies).GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field6 = typeof(DamageEnemies).GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field7 = typeof(DamageEnemies).GetField("direction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					FieldInfo field8 = typeof(DamageEnemies).GetField("moveDirection", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field9 = typeof(DamageEnemies).GetField("flipDirectionIfBehind", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field10 = typeof(DamageEnemies).GetField("forwardVector", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field11 = typeof(DamageEnemies).GetField("onlyDamageEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
					MethodInfo method = typeof(DamageEnemies).GetMethod("setOnlyDamageEnemies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					FieldInfo field12 = typeof(DamageEnemies).GetField("ignoreNailPosition", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field13 = typeof(DamageEnemies).GetField("silkGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field14 = typeof(DamageEnemies).GetField("doesNotGenerateSilk", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					FieldInfo field15 = typeof(DamageEnemies).GetField("attackType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					float num2 = 0f;
					num2 = ((num > 0.35f) ? 90f : ((!(num < -0.35f)) ? ((facing >= 0) ? 0f : 180f) : 270f));
					array3 = componentsInChildren5;
					foreach (DamageEnemies damageEnemies2 in array3)
					{
						if (!damageEnemies2)
						{
							continue;
						}
						try
						{
							field5?.SetValue(damageEnemies2, false);
						}
						catch
						{
						}
						try
						{
							field6?.SetValue(damageEnemies2, false);
						}
						catch
						{
						}
						try
						{
							field15?.SetValue(damageEnemies2, AttackTypes.Generic);
						}
						catch
						{
						}
						try
						{
							field7?.SetValue(damageEnemies2, num2);
						}
						catch
						{
						}
						try
						{
							field8?.SetValue(damageEnemies2, false);
						}
						catch
						{
						}
						try
						{
							field9?.SetValue(damageEnemies2, false);
						}
						catch
						{
						}
						try
						{
							field10?.SetValue(damageEnemies2, Vector2.zero);
						}
						catch
						{
						}
						try
						{
							if (method != null)
							{
								method.Invoke(damageEnemies2, new object[1] { false });
							}
							else
							{
								field11?.SetValue(damageEnemies2, false);
							}
						}
						catch
						{
						}
						try
						{
							field12?.SetValue(damageEnemies2, true);
						}
						catch
						{
						}
						try
						{
							if (field13 != null)
							{
								object value = Enum.ToObject(field13.FieldType, 0);
								field13.SetValue(damageEnemies2, value);
							}
						}
						catch
						{
						}
						try
						{
							field14?.SetValue(damageEnemies2, true);
						}
						catch
						{
						}
					}
					try
					{
						DamageEnemies[] componentsInChildren6 = gameObject14.GetComponentsInChildren<DamageEnemies>(includeInactive: true);
						bool flag = false;
						array3 = componentsInChildren6;
						foreach (DamageEnemies damageEnemies3 in array3)
						{
							if ((bool)damageEnemies3)
							{
								if (!flag)
								{
									flag = true;
								}
								else
								{
									damageEnemies3.enabled = false;
								}
							}
						}
					}
					catch
					{
					}
					try
					{
						if (Mathf.Abs(num) < 0.35f && facing >= 0)
						{
							NailSlash nailSlash6 = instance.GetType().GetProperty("AlternateSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
							if (nailSlash6 != null && !string.IsNullOrEmpty(nailSlash6.animName))
							{
								component.animName = nailSlash6.animName;
							}
						}
					}
					catch
					{
					}
					array3 = componentsInChildren2;
					foreach (DamageEnemies damageEnemies4 in array3)
					{
						if ((bool)damageEnemies4)
						{
							damageEnemies4.enabled = true;
						}
					}
					array2 = componentsInChildren;
					foreach (Collider2D collider2D2 in array2)
					{
						if ((bool)collider2D2)
						{
							collider2D2.enabled = true;
						}
					}
					component.StartSlash();
				}
				catch
				{
				}
			}
			StartCoroutine(PostConfigureSlash(gameObject14, num, facing, instance));
			static bool MatchDown(NailSlash ns)
			{
				if ((bool)ns)
				{
					if (!(ns.name ?? "").ToLowerInvariant().Contains("down"))
					{
						return (ns.animName ?? "").ToLowerInvariant().Contains("down");
					}
					return true;
				}
				return false;
			}
			static bool MatchLeft(NailSlash ns)
			{
				if (!ns)
				{
					return false;
				}
				string text = (ns.name ?? "").ToLowerInvariant();
				string text2 = (ns.animName ?? "").ToLowerInvariant();
				if (!text.Contains("left"))
				{
					return text2.Contains("left");
				}
				return true;
			}
			static bool MatchNormal(NailSlash ns)
			{
				if ((bool)ns && !MatchUp(ns))
				{
					return !MatchDown(ns);
				}
				return false;
			}
			static bool MatchRight(NailSlash ns)
			{
				if (!ns)
				{
					return false;
				}
				string text3 = (ns.name ?? "").ToLowerInvariant();
				string text4 = (ns.animName ?? "").ToLowerInvariant();
				if (!text3.Contains("alt") && !text3.Contains("right") && !text4.Contains("alt"))
				{
					return text4.Contains("right");
				}
				return true;
			}
			static bool MatchUp(NailSlash ns)
			{
				if ((bool)ns)
				{
					if (!(ns.name ?? "").ToLowerInvariant().Contains("up"))
					{
						return (ns.animName ?? "").ToLowerInvariant().Contains("up");
					}
					return true;
				}
				return false;
			}
		}

		private IEnumerator PostConfigureSlash(GameObject slash, float v, int facingSign, HeroController hc)
		{
			yield return null;
			if (!slash)
			{
				yield break;
			}
			try
			{
				NailSlashRecoil[] componentsInChildren = slash.GetComponentsInChildren<NailSlashRecoil>(includeInactive: true);
				foreach (NailSlashRecoil nailSlashRecoil in componentsInChildren)
				{
					if ((bool)nailSlashRecoil)
					{
						UnityEngine.Object.Destroy(nailSlashRecoil);
					}
				}
				DamageEnemies[] componentsInChildren2 = slash.GetComponentsInChildren<DamageEnemies>(includeInactive: true);
				FieldInfo field = typeof(DamageEnemies).GetField("sourceIsHero", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field2 = typeof(DamageEnemies).GetField("isHeroDamage", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field3 = typeof(DamageEnemies).GetField("direction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo field4 = typeof(DamageEnemies).GetField("moveDirection", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field5 = typeof(DamageEnemies).GetField("flipDirectionIfBehind", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field6 = typeof(DamageEnemies).GetField("forwardVector", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field7 = typeof(DamageEnemies).GetField("onlyDamageEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
				MethodInfo method = typeof(DamageEnemies).GetMethod("setOnlyDamageEnemies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo field8 = typeof(DamageEnemies).GetField("ignoreNailPosition", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field9 = typeof(DamageEnemies).GetField("silkGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
				FieldInfo field10 = typeof(DamageEnemies).GetField("doesNotGenerateSilk", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo field11 = typeof(DamageEnemies).GetField("attackType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				float num = ((v > 0.35f) ? 90f : ((!(v < -0.35f)) ? ((facingSign >= 0) ? 0f : 180f) : 270f));
				DamageEnemies[] array = componentsInChildren2;
				foreach (DamageEnemies damageEnemies in array)
				{
					if (!damageEnemies)
					{
						continue;
					}
					try
					{
						field?.SetValue(damageEnemies, false);
					}
					catch
					{
					}
					try
					{
						field2?.SetValue(damageEnemies, false);
					}
					catch
					{
					}
					try
					{
						field11?.SetValue(damageEnemies, AttackTypes.Generic);
					}
					catch
					{
					}
					try
					{
						field3?.SetValue(damageEnemies, num);
					}
					catch
					{
					}
					try
					{
						field4?.SetValue(damageEnemies, false);
					}
					catch
					{
					}
					try
					{
						field5?.SetValue(damageEnemies, false);
					}
					catch
					{
					}
					try
					{
						field6?.SetValue(damageEnemies, Vector2.zero);
					}
					catch
					{
					}
					try
					{
						if (method != null)
						{
							method.Invoke(damageEnemies, new object[1] { false });
						}
						else
						{
							field7?.SetValue(damageEnemies, false);
						}
					}
					catch
					{
					}
					try
					{
						field8?.SetValue(damageEnemies, true);
					}
					catch
					{
					}
					try
					{
						if (field9 != null)
						{
							object value = Enum.ToObject(field9.FieldType, 0);
							field9.SetValue(damageEnemies, value);
						}
					}
					catch
					{
					}
					try
					{
						field10?.SetValue(damageEnemies, true);
					}
					catch
					{
					}
				}
				Collider2D[] componentsInChildren3 = slash.GetComponentsInChildren<Collider2D>(includeInactive: true);
				Collider2D[] array2 = new Collider2D[0];
				if ((bool)hc)
				{
					array2 = hc.GetComponentsInChildren<Collider2D>(includeInactive: true);
				}
				Collider2D[] array3 = componentsInChildren3;
				foreach (Collider2D collider2D in array3)
				{
					Collider2D[] array4 = array2;
					foreach (Collider2D collider2D2 in array4)
					{
						if ((bool)collider2D && (bool)collider2D2)
						{
							Physics2D.IgnoreCollision(collider2D, collider2D2, ignore: true);
						}
					}
				}
				try
				{
					Transform obj12 = slash.transform;
					Vector3 localScale = obj12.localScale;
					localScale.x = Mathf.Abs(localScale.x) * ((facingSign >= 0) ? (-1f) : 1f);
					obj12.localScale = localScale;
				}
				catch
				{
				}
			}
			catch
			{
			}
		}

		private void SpawnProjectile(Vector2 dir)
		{
			GameObject gameObject = new GameObject("ShadeProjectile");
			gameObject.transform.position = base.transform.position + (Vector3)new Vector2(muzzleOffset.x * (float)facing, muzzleOffset.y);
			gameObject.tag = "Hero Spell";
			int num = LayerMask.NameToLayer("Hero Spell");
			int num2 = LayerMask.NameToLayer("Hero Attack");
			if (num >= 0)
			{
				gameObject.layer = num;
			}
			else if (num2 >= 0)
			{
				gameObject.layer = num2;
			}
			gameObject.AddComponent<SpriteRenderer>().sprite = MakeDotSprite();
			CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
			circleCollider2D.isTrigger = true;
			ShadeProjectile[] array = UnityEngine.Object.FindObjectsByType<ShadeProjectile>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				Collider2D component = array[i].GetComponent<Collider2D>();
				if ((bool)component)
				{
					Physics2D.IgnoreCollision(circleCollider2D, component, ignore: true);
				}
			}
			Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.gravityScale = 0f;
			rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
			rigidbody2D.linearVelocity = dir.normalized * projectileSpeed;
			if (hornetTransform != null)
			{
				Collider2D[] componentsInChildren = hornetTransform.GetComponentsInChildren<Collider2D>(includeInactive: true);
				foreach (Collider2D collider2D in componentsInChildren)
				{
					if ((bool)collider2D && (bool)circleCollider2D)
					{
						Physics2D.IgnoreCollision(circleCollider2D, collider2D, ignore: true);
					}
				}
			}
			ShadeProjectile shadeProjectile = gameObject.AddComponent<ShadeProjectile>();
			shadeProjectile.damage = 20;
			shadeProjectile.hornetRoot = hornetTransform;
			shadeProjectile.lifeSeconds = 1.5f;
		}

		private Sprite MakeDotSprite()
		{
			Texture2D texture2D = new Texture2D(6, 6);
			for (int i = 0; i < texture2D.width; i++)
			{
				for (int j = 0; j < texture2D.height; j++)
				{
					texture2D.SetPixel(i, j, Color.black);
				}
			}
			texture2D.Apply();
			return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 16f);
		}
	}

	private static GameObject helper;

	private static bool loggedStartupFields;

	private static SimpleHUD hud;

	private static bool registeredEnterSceneHandler;

	internal static int savedShadeHP = -1;

	internal static int savedShadeMax = -1;

	internal static int savedShadeSoul = -1;

	internal static bool HasSavedShadeState => savedShadeMax > 0;

	internal static void SaveShadeState(int curHp, int maxHp, int soul)
	{
		savedShadeMax = Mathf.Max(1, maxHp);
		savedShadeHP = Mathf.Clamp(curHp, 0, savedShadeMax);
		savedShadeSoul = Mathf.Max(0, soul);
	}

	private void Awake()
	{
		new Harmony("com.legacyoftheabyss.helper").PatchAll();
		SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode mode)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				string text = gameObject.name.ToLowerInvariant();
				if (text.Contains("team cherry") || (text.Contains("save") && text.Contains("reminder")))
				{
					gameObject.SetActive(value: false);
				}
			}
		};
	}

	private static void HandleFinishedEnteringScene()
	{
		try
		{
			GameManager instance = GameManager.instance;
			if (!(instance == null) && !(instance.hero_ctrl == null))
			{
				Vector3 position = instance.hero_ctrl.transform.position;
				instance.StartCoroutine(SpawnShadeAfterDelay(position, 0.5f));
			}
		}
		catch
		{
		}
	}

	private static IEnumerator SpawnShadeAfterDelay(Vector3 pos, float delay)
	{
		yield return new WaitForSeconds(delay);
		GameManager instance = GameManager.instance;
		if (instance == null || instance.hero_ctrl == null)
		{
			yield break;
		}
		if (helper != null)
		{
			try
			{
				ShadeController component = helper.GetComponent<ShadeController>();
				if (component != null)
				{
					component.TeleportToPosition(pos);
					SaveShadeState(component.GetCurrentHP(), component.GetMaxHP(), component.GetShadeSoul());
				}
				else
				{
					helper.transform.position = pos;
				}
				yield break;
			}
			catch
			{
				yield break;
			}
		}
		helper = new GameObject("HelperShade");
		helper.transform.position = pos;
		ShadeController shadeController = helper.AddComponent<ShadeController>();
		shadeController.Init(instance.hero_ctrl.transform);
		if (HasSavedShadeState)
		{
			shadeController.RestorePersistentState(savedShadeHP, savedShadeMax, savedShadeSoul);
		}
		SpriteRenderer spriteRenderer = helper.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = GenerateDebugSprite();
		spriteRenderer.color = Color.black;
		SpriteRenderer componentInChildren = instance.hero_ctrl.GetComponentInChildren<SpriteRenderer>();
		if (componentInChildren != null)
		{
			spriteRenderer.sortingLayerID = componentInChildren.sortingLayerID;
			spriteRenderer.sortingOrder = componentInChildren.sortingOrder + 1;
		}
	}

	private static Sprite GenerateDebugSprite()
	{
		Texture2D texture2D = new Texture2D(160, 160);
		for (int i = 0; i < 160; i++)
		{
			for (int j = 0; j < 160; j++)
			{
				texture2D.SetPixel(i, j, Color.white);
			}
		}
		texture2D.Apply();
		return Sprite.Create(texture2D, new Rect(0f, 0f, 160f, 160f), new Vector2(0.5f, 0.5f));
	}

	internal static void DisableStartup(GameManager gm)
	{
		if (gm == null)
		{
			return;
		}
		FieldInfo[] fields = gm.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		_ = loggedStartupFields;
		loggedStartupFields = true;
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType != typeof(bool))
			{
				continue;
			}
			string text = fieldInfo.Name.ToLower();
			if (text.Contains("logo") || (text.Contains("save") && text.Contains("reminder")))
			{
				try
				{
					fieldInfo.SetValue(gm, false);
				}
				catch
				{
				}
			}
		}
	}
}
