using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using BepInEx;
using GlobalEnums;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETFramework,Version=v4.7.2", FrameworkDisplayName = ".NET Framework 4.7.2")]
[assembly: AssemblyCompany("LegacyoftheAbyss")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("LegacyoftheAbyss")]
[assembly: AssemblyTitle("LegacyoftheAbyss")]
[assembly: AssemblyVersion("1.0.0.0")]
[module: RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute(int P_0)
		{
			Version = P_0;
		}
	}
}
public class SimpleHUD : MonoBehaviour
{
	private PlayerData playerData;

	private Image[] maskImages;

	private Sprite maskSprite;

	private Color missingMaskColor = new Color(0.2f, 0.2f, 0.2f, 0.45f);

	private Sprite soulOrbSprite;

	private RectTransform soulOrbRoot;

	private RectTransform soulRevealMask;

	private Image soulImage;

	private Image soulBgImage;

	private float lastLoggedFill = -1f;

	private Sprite frameSprite;

	private Sprite[] slashFrames;

	private int shadeMax;

	private int shadeHealth;

	private int previousShadeHealth;

	private int prevHornetHealth;

	private int prevHornetMax;

	private GameObject healthContainer;

	private Canvas canvas;

	private CanvasScaler scaler;

	private const KeyCode DebugDamageKey = KeyCode.Minus;

	private const KeyCode DebugHealKey = KeyCode.Equals;

	private const KeyCode DebugSoulDecKey = KeyCode.LeftBracket;

	private const KeyCode DebugSoulIncKey = KeyCode.RightBracket;

	private const KeyCode DebugSoulResetKey = KeyCode.Backslash;

	private bool debugUseCustomSilk;

	private float debugSilk;

	private bool shadeSoulOverride;

	private float shadeSoul;

	private float shadeSoulMax;

	private const float MaskScale = 0.88f;

	private AudioSource sfxSource;

	private List<AudioClip> shadeHurtCandidates;

	private int shadeHurtIdx;

	private AudioClip pinnedHurtSingle;

	private AudioClip pinnedHurtDouble;

	private Sprite[] _slashFramesCache;

	public void Init(PlayerData pd)
	{
		playerData = pd;
		LoadSprites();
		ComputeShadeFromPlayer();
		CreateUI();
		previousShadeHealth = shadeHealth;
	}

	private float GetUIScale()
	{
		float num = Mathf.Max(0.1f, (float)Screen.height / 1080f);
		return 1f + (num - 1f) * 0.5f;
	}

	private void CreateUI()
	{
		canvas = base.gameObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		scaler = base.gameObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920f, 1080f);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 1f;
		base.gameObject.AddComponent<GraphicRaycaster>();
		soulOrbRoot = new GameObject("SoulOrb").AddComponent<RectTransform>();
		soulOrbRoot.SetParent(canvas.transform, worldPositionStays: false);
		RectTransform rectTransform = soulOrbRoot;
		Vector2 anchorMin = (soulOrbRoot.anchorMax = new Vector2(1f, 1f));
		rectTransform.anchorMin = anchorMin;
		soulOrbRoot.pivot = new Vector2(1f, 1f);
		Vector2 vector2 = ((soulOrbSprite != null) ? new Vector2(soulOrbSprite.rect.width, soulOrbSprite.rect.height) : new Vector2(96f, 96f));
		float uIScale = GetUIScale();
		soulOrbRoot.sizeDelta = vector2 * uIScale * 0.85f;
		soulOrbRoot.anchoredPosition = new Vector2(-200f * uIScale, -20f * uIScale);
		soulOrbRoot.localScale = new Vector3(-1f, 1f, 1f);
		GameObject gameObject = new GameObject("SoulBackground");
		gameObject.transform.SetParent(soulOrbRoot, worldPositionStays: false);
		soulBgImage = gameObject.AddComponent<Image>();
		soulBgImage.sprite = ((soulOrbSprite != null) ? soulOrbSprite : BuildCircleSprite());
		soulBgImage.preserveAspect = true;
		RectTransform rectTransform2 = soulBgImage.rectTransform;
		rectTransform2.anchorMin = new Vector2(0f, 0f);
		rectTransform2.anchorMax = new Vector2(1f, 1f);
		rectTransform2.pivot = new Vector2(0.5f, 0.5f);
		rectTransform2.anchoredPosition = Vector2.zero;
		rectTransform2.sizeDelta = Vector2.zero;
		soulBgImage.color = new Color(0.6f, 0.6f, 0.6f, 0.35f);
		GameObject gameObject2 = new GameObject("SoulRevealMask");
		gameObject2.transform.SetParent(soulOrbRoot, worldPositionStays: false);
		soulRevealMask = gameObject2.AddComponent<RectTransform>();
		soulRevealMask.anchorMin = new Vector2(0f, 0f);
		soulRevealMask.anchorMax = new Vector2(1f, 0f);
		soulRevealMask.pivot = new Vector2(0.5f, 0f);
		soulRevealMask.anchoredPosition = Vector2.zero;
		soulRevealMask.sizeDelta = new Vector2(0f, 0f);
		gameObject2.AddComponent<RectMask2D>();
		GameObject gameObject3 = new GameObject("SoulImage");
		gameObject3.transform.SetParent(soulOrbRoot, worldPositionStays: false);
		soulImage = gameObject3.AddComponent<Image>();
		soulImage.sprite = ((soulOrbSprite != null) ? soulOrbSprite : BuildCircleSprite());
		soulImage.preserveAspect = true;
		soulImage.maskable = true;
		soulImage.raycastTarget = false;
		soulImage.color = Color.white;
		soulImage.type = Image.Type.Filled;
		soulImage.fillMethod = Image.FillMethod.Vertical;
		soulImage.fillOrigin = 0;
		soulImage.fillAmount = 0f;
		RectTransform rectTransform3 = soulImage.rectTransform;
		rectTransform3.anchorMin = new Vector2(0.5f, 0f);
		rectTransform3.anchorMax = new Vector2(0.5f, 0f);
		rectTransform3.pivot = new Vector2(0.5f, 0f);
		rectTransform3.anchoredPosition = Vector2.zero;
		rectTransform3.sizeDelta = soulOrbRoot.sizeDelta;
		try
		{
			Debug.Log(string.Format("[HelperMod] Soul orb sprite={0} size={1} uiScale={2}", soulOrbSprite ? soulOrbSprite.name : "<generated>", vector2, uIScale));
		}
		catch
		{
		}
		healthContainer = new GameObject("HealthContainer");
		healthContainer.transform.SetParent(canvas.transform, worldPositionStays: false);
		RectTransform rectTransform4 = healthContainer.AddComponent<RectTransform>();
		anchorMin = (rectTransform4.anchorMax = new Vector2(1f, 1f));
		rectTransform4.anchorMin = anchorMin;
		rectTransform4.pivot = new Vector2(1f, 1f);
		Vector2 obj2 = ((maskSprite != null) ? new Vector2(maskSprite.rect.width, maskSprite.rect.height) : new Vector2(33f, 41f));
		float num = obj2.y * uIScale * 0.88f;
		float num2 = soulOrbRoot.anchoredPosition.y - soulOrbRoot.sizeDelta.y * 0.5f;
		rectTransform4.anchoredPosition = new Vector2(soulOrbRoot.anchoredPosition.x, num2 + num * 0.5f);
		BuildMasks(rectTransform4, uIScale);
	}

	private Sprite BuildMaskSprite()
	{
		Texture2D texture2D = new Texture2D(32, 32);
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				texture2D.SetPixel(i, j, Color.white);
			}
		}
		texture2D.Apply();
		return Sprite.Create(texture2D, new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f));
	}

	private void LoadSprites()
	{
		try
		{
			string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
			string path2 = Path.Combine(path, "select_game_HUD_0001_health.png");
			string path3 = Path.Combine(path, "select_game_HUD_0002_health_frame.png");
			string path4 = Path.Combine(path, "The Knight spells and items - atlas0 #00000309.png");
			string path5 = Path.Combine(path, "soul_orb_glow0000.png");
			maskSprite = LoadSprite(path2);
			if (maskSprite == null)
			{
				maskSprite = FindSpriteInGame("select_game_HUD_0001_health");
			}
			frameSprite = LoadSprite(path3);
			if (frameSprite == null)
			{
				frameSprite = FindSpriteInGame("select_game_HUD_0002_health_frame");
			}
			slashFrames = LoadSpriteSheet(path4, 8, 8);
			soulOrbSprite = LoadSprite(path5);
		}
		catch
		{
		}
	}

	private Sprite LoadSprite(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		byte[] bytes = File.ReadAllBytes(path);
		Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, mipChain: false);
		TryLoadImage(texture2D, bytes);
		texture2D.filterMode = FilterMode.Point;
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
	}

	private Sprite[] LoadSpriteSheet(string path, int cols, int rows)
	{
		if (!File.Exists(path))
		{
			return new Sprite[0];
		}
		byte[] bytes = File.ReadAllBytes(path);
		Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, mipChain: false);
		TryLoadImage(texture2D, bytes);
		texture2D.filterMode = FilterMode.Point;
		int num = texture2D.width / cols;
		int num2 = texture2D.height / rows;
		Sprite[] array = new Sprite[cols * rows];
		int num3 = 0;
		for (int num4 = rows - 1; num4 >= 0; num4--)
		{
			for (int i = 0; i < cols; i++)
			{
				array[num3++] = Sprite.Create(texture2D, new Rect(i * num, num4 * num2, num, num2), new Vector2(0.5f, 0.5f));
			}
		}
		return array;
	}

	private Sprite BuildCircleSprite()
	{
		int num = 64;
		Texture2D texture2D = new Texture2D(num, num);
		Vector2 b = new Vector2((float)num / 2f, (float)num / 2f);
		float num2 = (float)num / 2f;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				Color color = ((Vector2.Distance(new Vector2(i, j), b) <= num2) ? Color.white : Color.clear);
				texture2D.SetPixel(i, j, color);
			}
		}
		texture2D.Apply();
		return Sprite.Create(texture2D, new Rect(0f, 0f, num, num), new Vector2(0.5f, 0.5f));
	}

	private void Update()
	{
		if (playerData == null)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Minus))
		{
			shadeHealth = Mathf.Max(0, shadeHealth - 1);
		}
		if (Input.GetKeyDown(KeyCode.Equals))
		{
			shadeHealth = Mathf.Min(shadeMax, shadeHealth + 1);
		}
		float num = Mathf.Max(1f, playerData.silkMax);
		float num2 = Mathf.Max(1f, num * 0.1f);
		if (Input.GetKeyDown(KeyCode.RightBracket))
		{
			if (shadeSoulOverride)
			{
				try
				{
					LegacyHelper.ShadeController shadeController = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
					if (shadeController != null)
					{
						shadeController.shadeSoul = Mathf.Min(shadeController.shadeSoul + 11, shadeController.shadeSoulMax);
					}
				}
				catch
				{
				}
				shadeSoul = Mathf.Min(shadeSoul + 11f, Mathf.Max(1f, shadeSoulMax));
			}
			else
			{
				float num3 = (debugUseCustomSilk ? debugSilk : ((float)playerData.silk));
				debugUseCustomSilk = true;
				debugSilk = Mathf.Min(num3 + num2, num);
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			if (shadeSoulOverride)
			{
				try
				{
					LegacyHelper.ShadeController shadeController2 = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
					if (shadeController2 != null)
					{
						shadeController2.shadeSoul = Mathf.Max(shadeController2.shadeSoul - 11, 0);
					}
				}
				catch
				{
				}
				shadeSoul = Mathf.Max(shadeSoul - 11f, 0f);
			}
			else
			{
				float num4 = (debugUseCustomSilk ? debugSilk : ((float)playerData.silk));
				debugUseCustomSilk = true;
				debugSilk = Mathf.Max(num4 - num2, 0f);
			}
		}
		if (Input.GetKeyDown(KeyCode.Backslash))
		{
			if (shadeSoulOverride)
			{
				try
				{
					LegacyHelper.ShadeController shadeController3 = UnityEngine.Object.FindFirstObjectByType<LegacyHelper.ShadeController>();
					if (shadeController3 != null)
					{
						shadeController3.shadeSoul = 0;
					}
				}
				catch
				{
				}
				shadeSoul = 0f;
			}
			else
			{
				debugUseCustomSilk = false;
				debugSilk = playerData.silk;
			}
		}
		SyncShadeFromPlayer();
		RefreshHealth();
		RefreshSoul();
	}

	private void RefreshHealth()
	{
		int num = shadeHealth;
		if (num < previousShadeHealth)
		{
			int lost = previousShadeHealth - num;
			TryPlayPinnedHurtSfx(lost);
			for (int i = num; i < previousShadeHealth && i < maskImages.Length; i++)
			{
				StartCoroutine(LoseHealth(maskImages[i]));
			}
		}
		for (int j = 0; j < num && j < maskImages.Length; j++)
		{
			maskImages[j].sprite = ((maskSprite != null) ? maskSprite : maskImages[j].sprite);
			maskImages[j].color = Color.white;
		}
		for (int k = num; k < maskImages.Length; k++)
		{
			if (k >= previousShadeHealth)
			{
				maskImages[k].color = missingMaskColor;
			}
		}
		previousShadeHealth = num;
	}

	private IEnumerator LoseHealth(Image img)
	{
		if (img == null)
		{
			yield break;
		}
		for (int k = 0; k < 2; k++)
		{
			img.color = Color.white;
			yield return new WaitForSeconds(0.05f);
			img.color = Color.red;
			yield return new WaitForSeconds(0.05f);
		}
		img.color = Color.white;
		Sprite[] frames = GetSlashFrames();
		try
		{
			Debug.Log("[HelperMod] Slash frames: " + string.Join(", ", from f in frames.Take(6)
				select (!(f != null)) ? "<null>" : f.name));
		}
		catch
		{
		}
		if (frames != null && frames.Length != 0)
		{
			GameObject slash = new GameObject("Slash");
			slash.transform.SetParent(img.transform, worldPositionStays: false);
			Image sr = slash.AddComponent<Image>();
			sr.rectTransform.sizeDelta = img.rectTransform.sizeDelta * 2f;
			int k = Mathf.Min(6, frames.Length);
			for (int i = 0; i < k; i++)
			{
				sr.sprite = frames[i];
				yield return new WaitForSeconds(0.03f);
			}
			UnityEngine.Object.Destroy(slash);
		}
		float t = 0f;
		Color c = img.color;
		while (t < 0.3f)
		{
			t += Time.deltaTime;
			c.a = Mathf.Lerp(1f, 0f, t / 0.3f);
			img.color = c;
			yield return null;
		}
		img.color = missingMaskColor;
	}

	private void ComputeShadeFromPlayer()
	{
		if (playerData == null)
		{
			shadeMax = 0;
			shadeHealth = 0;
			return;
		}
		shadeMax = (playerData.maxHealth + 1) / 2;
		prevHornetMax = playerData.maxHealth;
		prevHornetHealth = playerData.health;
		if (previousShadeHealth == 0 && shadeHealth == 0)
		{
			shadeHealth = (playerData.health + 1) / 2;
		}
	}

	private void SyncShadeFromPlayer()
	{
		if (playerData != null)
		{
			int maxHealth = playerData.maxHealth;
			int health = playerData.health;
			int num = (maxHealth + 1) / 2;
			if (num != shadeMax)
			{
				shadeMax = num;
				RebuildMasks();
				previousShadeHealth = Mathf.Min(previousShadeHealth, shadeMax);
				shadeHealth = Mathf.Min(shadeHealth, shadeMax);
			}
			prevHornetHealth = health;
			prevHornetMax = maxHealth;
		}
	}

	private void RebuildMasks()
	{
		if (healthContainer == null)
		{
			return;
		}
		Image[] array = maskImages ?? Array.Empty<Image>();
		foreach (Image image in array)
		{
			if (image != null)
			{
				UnityEngine.Object.Destroy(image.gameObject);
			}
		}
		BuildMasks(healthContainer.GetComponent<RectTransform>(), GetUIScale());
	}

	private void BuildMasks(RectTransform container, float uiScale)
	{
		maskImages = new Image[shadeMax];
		Vector2 sizeDelta = ((maskSprite != null) ? new Vector2(maskSprite.rect.width, maskSprite.rect.height) : new Vector2(33f, 41f)) * uiScale * 0.88f;
		float num = 6f * uiScale;
		float num2 = 0f;
		for (int i = 0; i < shadeMax; i++)
		{
			GameObject obj = new GameObject($"Mask{i}");
			obj.transform.SetParent(container, worldPositionStays: false);
			RectTransform rectTransform = obj.AddComponent<RectTransform>();
			Vector2 anchorMin = (rectTransform.anchorMax = new Vector2(1f, 1f));
			rectTransform.anchorMin = anchorMin;
			rectTransform.pivot = new Vector2(1f, 1f);
			rectTransform.sizeDelta = sizeDelta;
			rectTransform.anchoredPosition = new Vector2(0f - num2, 0f);
			num2 += sizeDelta.x + num;
			Image image = obj.AddComponent<Image>();
			image.preserveAspect = true;
			image.sprite = ((maskSprite != null) ? maskSprite : BuildMaskSprite());
			image.color = Color.white;
			maskImages[i] = image;
		}
	}

	private void RefreshSoul()
	{
		if (soulOrbRoot == null)
		{
			return;
		}
		float num = (shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax));
		float num2 = (shadeSoulOverride ? Mathf.Clamp(shadeSoul, 0f, num) : (debugUseCustomSilk ? debugSilk : ((float)playerData.silk)));
		float num3 = Mathf.Clamp01(num2 / num);
		if (soulImage != null)
		{
			soulImage.fillAmount = num3;
		}
		float y = soulOrbRoot.sizeDelta.y;
		Vector2 vector = new Vector2(0f, y * num3);
		if (soulImage != null)
		{
			soulImage.fillAmount = num3;
		}
		if (!(Mathf.Abs(num3 - lastLoggedFill) > 0.01f))
		{
			return;
		}
		lastLoggedFill = num3;
		try
		{
			Debug.Log($"[HelperMod] Soul fill: silk={num2}/{num} fill={num3:F2} maskH={vector.y:F1} rootH={y:F1}");
		}
		catch
		{
		}
	}

	public void SetShadeSoul(int current, int max)
	{
		shadeSoulOverride = true;
		shadeSoul = Mathf.Max(0, current);
		shadeSoulMax = Mathf.Max(1, max);
	}

	public void SetShadeStats(int current, int max)
	{
		int num = Mathf.Max(1, max);
		int num2 = Mathf.Clamp(current, 0, num);
		bool num3 = num != shadeMax;
		shadeMax = num;
		shadeHealth = num2;
		if (num3)
		{
			RebuildMasks();
		}
		RefreshHealth();
	}

	public void ClearShadeSoulOverride()
	{
		shadeSoulOverride = false;
	}

	public void SetVisible(bool visible)
	{
		if (canvas != null)
		{
			canvas.enabled = visible;
		}
		else
		{
			base.gameObject.SetActive(visible);
		}
	}

	public void SetPlayerData(PlayerData pd)
	{
		if (pd != playerData)
		{
			playerData = pd;
			int num = shadeMax;
			ComputeShadeFromPlayer();
			if (shadeMax != num)
			{
				RebuildMasks();
				previousShadeHealth = Mathf.Min(previousShadeHealth, shadeMax);
			}
			RefreshHealth();
			RefreshSoul();
		}
	}

	private void TryPlayPinnedHurtSfx(int lost)
	{
		try
		{
			if (sfxSource == null)
			{
				GameObject gameObject = new GameObject("ShadeHUD_SFX");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				sfxSource = gameObject.AddComponent<AudioSource>();
				sfxSource.playOnAwake = false;
				sfxSource.spatialBlend = 0f;
				sfxSource.volume = 1f;
			}
			if (pinnedHurtSingle == null || pinnedHurtDouble == null)
			{
				AudioClip[] array = Resources.FindObjectsOfTypeAll<AudioClip>();
				if (array != null)
				{
					AudioClip[] array2 = array;
					foreach (AudioClip audioClip in array2)
					{
						if (!(audioClip == null))
						{
							string a = audioClip.name ?? string.Empty;
							if (pinnedHurtSingle == null && string.Equals(a, "hero_damage", StringComparison.OrdinalIgnoreCase))
							{
								pinnedHurtSingle = audioClip;
							}
							if (pinnedHurtDouble == null && string.Equals(a, "hero_double_damage", StringComparison.OrdinalIgnoreCase))
							{
								pinnedHurtDouble = audioClip;
							}
							if (pinnedHurtSingle != null && pinnedHurtDouble != null)
							{
								break;
							}
						}
					}
					if (pinnedHurtSingle == null)
					{
						array2 = array;
						foreach (AudioClip audioClip2 in array2)
						{
							string text = ((audioClip2 != null) ? audioClip2.name : null);
							if (!string.IsNullOrEmpty(text) && text.IndexOf("hero_damage", StringComparison.OrdinalIgnoreCase) >= 0)
							{
								pinnedHurtSingle = audioClip2;
								break;
							}
						}
					}
					if (pinnedHurtDouble == null)
					{
						array2 = array;
						foreach (AudioClip audioClip3 in array2)
						{
							string text2 = ((audioClip3 != null) ? audioClip3.name : null);
							if (!string.IsNullOrEmpty(text2) && text2.IndexOf("hero_double_damage", StringComparison.OrdinalIgnoreCase) >= 0)
							{
								pinnedHurtDouble = audioClip3;
								break;
							}
						}
					}
				}
			}
			AudioClip audioClip4 = ((lost >= 2 && pinnedHurtDouble != null) ? pinnedHurtDouble : pinnedHurtSingle);
			if (audioClip4 != null)
			{
				sfxSource.PlayOneShot(audioClip4);
				Debug.Log("[HelperMod] Shade hurt SFX (pinned): " + audioClip4.name + " (lost=" + lost + ")");
				return;
			}
		}
		catch
		{
		}
		TryPlayDamageSfx();
	}

	private void TryPlayDamageSfx()
	{
		try
		{
			if (sfxSource == null)
			{
				GameObject gameObject = new GameObject("ShadeHUD_SFX");
				gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				sfxSource = gameObject.AddComponent<AudioSource>();
				sfxSource.playOnAwake = false;
				sfxSource.spatialBlend = 0f;
				sfxSource.volume = 1f;
			}
			if (shadeHurtCandidates == null || shadeHurtCandidates.Count == 0)
			{
				shadeHurtCandidates = BuildShadeHurtCandidates();
				shadeHurtIdx = 0;
				try
				{
					Debug.Log("[HelperMod] Built shade hurt SFX list: " + string.Join(", ", shadeHurtCandidates.Select((AudioClip c) => (!(c != null)) ? "<null>" : c.name)));
				}
				catch
				{
				}
			}
			if (shadeHurtCandidates != null && shadeHurtCandidates.Count > 0)
			{
				AudioClip audioClip = shadeHurtCandidates[shadeHurtIdx % shadeHurtCandidates.Count];
				shadeHurtIdx++;
				if (audioClip != null)
				{
					sfxSource.PlayOneShot(audioClip);
					Debug.Log("[HelperMod] Shade hurt SFX: " + audioClip.name + " (" + shadeHurtIdx + "/" + shadeHurtCandidates.Count + ")");
				}
			}
		}
		catch
		{
		}
	}

	private List<AudioClip> BuildShadeHurtCandidates()
	{
		List<AudioClip> list = new List<AudioClip>();
		HashSet<AudioClip> hashSet = new HashSet<AudioClip>();
		try
		{
			List<AudioClip> list2 = FindHurtClipsFromHornetFSM();
			if (list2 != null)
			{
				foreach (AudioClip item in list2)
				{
					if (item != null && hashSet.Add(item))
					{
						list.Add(item);
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			AudioClip[] array = Resources.FindObjectsOfTypeAll<AudioClip>();
			if (array != null && array.Length != 0)
			{
				foreach (AudioClip item2 in from c in array
					where c != null
					select new
					{
						clip = c,
						n = (c.name ?? string.Empty)
					} into x
					where x.n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0 || x.n.IndexOf("take_hit", StringComparison.OrdinalIgnoreCase) >= 0 || x.n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0 || x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0 || x.n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0
					orderby (x.n.IndexOf("take", StringComparison.OrdinalIgnoreCase) >= 0 && x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0) ? 6 : 0 descending, (x.n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0) ? 1 : 0 descending, (x.n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0) ? 1 : 0 descending, (x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0) ? 1 : 0 descending, (x.n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0) ? 1 : 0 descending, x.n
					select x.clip)
				{
					string text = item2.name ?? string.Empty;
					if (text.IndexOf("deal_damage", StringComparison.OrdinalIgnoreCase) < 0 && text.IndexOf("attack", StringComparison.OrdinalIgnoreCase) < 0 && text.IndexOf("tool", StringComparison.OrdinalIgnoreCase) < 0 && text.IndexOf("slash", StringComparison.OrdinalIgnoreCase) < 0 && text.IndexOf("charge", StringComparison.OrdinalIgnoreCase) < 0 && hashSet.Add(item2))
					{
						list.Add(item2);
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	private List<AudioClip> FindHurtClipsFromHornetFSM()
	{
		List<AudioClip> list = new List<AudioClip>();
		try
		{
			HeroController heroController = ((HeroController.instance != null) ? HeroController.instance : ((GameManager.instance != null) ? GameManager.instance.hero_ctrl : null));
			if (heroController == null)
			{
				return list;
			}
			Type type = Type.GetType("HutongGames.PlayMaker.PlayMakerFSM, PlayMaker");
			if (type == null)
			{
				type = Type.GetType("PlayMakerFSM, PlayMaker");
			}
			if (type == null)
			{
				return list;
			}
			Component[] componentsInChildren = heroController.GetComponentsInChildren(type, includeInactive: true);
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				return list;
			}
			Component[] array = componentsInChildren;
			foreach (Component component in array)
			{
				if (component == null)
				{
					continue;
				}
				PropertyInfo property = component.GetType().GetProperty("Fsm", BindingFlags.Instance | BindingFlags.Public);
				object obj = ((property != null) ? property.GetValue(component, null) : null);
				if (obj == null)
				{
					continue;
				}
				Type type2 = obj.GetType();
				PropertyInfo property2 = type2.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
				string text = (((property2 != null) ? (property2.GetValue(obj, null) as string) : null) ?? string.Empty).ToLowerInvariant();
				if (!text.Contains("damage") && !text.Contains("hurt") && !text.Contains("hit") && !text.Contains("hero"))
				{
					continue;
				}
				PropertyInfo property3 = type2.GetProperty("States", BindingFlags.Instance | BindingFlags.Public);
				Array array2 = ((property3 != null) ? (property3.GetValue(obj, null) as Array) : null);
				if (array2 == null)
				{
					continue;
				}
				foreach (object item in array2)
				{
					if (item == null)
					{
						continue;
					}
					Type type3 = item.GetType();
					PropertyInfo property4 = type3.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
					string text2 = (((property4 != null) ? (property4.GetValue(item, null) as string) : null) ?? string.Empty).ToLowerInvariant();
					if (!text2.Contains("damage") && !text2.Contains("hurt") && !text2.Contains("hit") && !text2.Contains("impact"))
					{
						continue;
					}
					PropertyInfo property5 = type3.GetProperty("Actions", BindingFlags.Instance | BindingFlags.Public);
					Array array3 = ((property5 != null) ? (property5.GetValue(item, null) as Array) : null);
					if (array3 == null)
					{
						continue;
					}
					foreach (object item2 in array3)
					{
						if (item2 == null)
						{
							continue;
						}
						Type type4 = item2.GetType();
						FieldInfo[] fields = type4.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						foreach (FieldInfo fieldInfo in fields)
						{
							try
							{
								if (typeof(AudioClip).IsAssignableFrom(fieldInfo.FieldType))
								{
									AudioClip audioClip = fieldInfo.GetValue(item2) as AudioClip;
									if (audioClip != null && !list.Contains(audioClip))
									{
										list.Add(audioClip);
									}
								}
								if (typeof(AudioSource).IsAssignableFrom(fieldInfo.FieldType))
								{
									AudioSource audioSource = fieldInfo.GetValue(item2) as AudioSource;
									if (audioSource != null && audioSource.clip != null && !list.Contains(audioSource.clip))
									{
										list.Add(audioSource.clip);
									}
								}
								Type fieldType = fieldInfo.FieldType;
								if (!(fieldType != null) || fieldType.FullName == null || !fieldType.FullName.Contains("HutongGames.PlayMaker.FsmObject"))
								{
									continue;
								}
								object value = fieldInfo.GetValue(item2);
								if (value != null)
								{
									PropertyInfo property6 = value.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
									AudioClip audioClip2 = ((property6 != null) ? property6.GetValue(value, null) : null) as AudioClip;
									if (audioClip2 != null && !list.Contains(audioClip2))
									{
										list.Add(audioClip2);
									}
								}
							}
							catch
							{
							}
						}
						PropertyInfo[] properties = type4.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						foreach (PropertyInfo propertyInfo in properties)
						{
							try
							{
								if (!propertyInfo.CanRead)
								{
									continue;
								}
								Type propertyType = propertyInfo.PropertyType;
								if (typeof(AudioClip).IsAssignableFrom(propertyType))
								{
									AudioClip audioClip3 = propertyInfo.GetValue(item2, null) as AudioClip;
									if (audioClip3 != null && !list.Contains(audioClip3))
									{
										list.Add(audioClip3);
									}
								}
								if (typeof(AudioSource).IsAssignableFrom(propertyType))
								{
									AudioSource audioSource2 = propertyInfo.GetValue(item2, null) as AudioSource;
									if (audioSource2 != null && audioSource2.clip != null && !list.Contains(audioSource2.clip))
									{
										list.Add(audioSource2.clip);
									}
								}
								if (!(propertyType != null) || propertyType.FullName == null || !propertyType.FullName.Contains("HutongGames.PlayMaker.FsmObject"))
								{
									continue;
								}
								object value2 = propertyInfo.GetValue(item2, null);
								if (value2 != null)
								{
									PropertyInfo property7 = value2.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
									AudioClip audioClip4 = ((property7 != null) ? property7.GetValue(value2, null) : null) as AudioClip;
									if (audioClip4 != null && !list.Contains(audioClip4))
									{
										list.Add(audioClip4);
									}
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	private AudioClip FindHornetDamageClip()
	{
		try
		{
			AudioClip[] array = Resources.FindObjectsOfTypeAll<AudioClip>();
			if (array != null && array.Length != 0)
			{
				AudioClip audioClip = null;
				int num = int.MinValue;
				AudioClip[] array2 = array;
				foreach (AudioClip audioClip2 in array2)
				{
					if (!(audioClip2 == null))
					{
						string text = audioClip2.name ?? string.Empty;
						int num2 = 0;
						if (text.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 += 5;
						}
						if (text.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 += 4;
						}
						if (text.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 += 3;
						}
						if (text.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 += 3;
						}
						if (text.IndexOf("hero", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("player", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2++;
						}
						if (text.IndexOf("deal_damage", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 -= 5;
						}
						if (text.IndexOf("attack", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 -= 4;
						}
						if (text.IndexOf("tool", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 -= 3;
						}
						if (text.IndexOf("slash", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 -= 2;
						}
						if (text.IndexOf("charge", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							num2 -= 2;
						}
						if (num2 > num)
						{
							num = num2;
							audioClip = audioClip2;
						}
					}
				}
				if (audioClip != null)
				{
					Debug.Log("[HelperMod] Using damage SFX clip: " + audioClip.name);
					return audioClip;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private Sprite FindSpriteInGame(string namePart)
	{
		if (string.IsNullOrEmpty(namePart))
		{
			return null;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(namePart);
		Sprite[] array = Resources.FindObjectsOfTypeAll<Sprite>();
		Sprite result = null;
		int num = int.MinValue;
		Sprite[] array2 = array;
		foreach (Sprite sprite in array2)
		{
			if (!(sprite == null))
			{
				string obj = sprite.name ?? string.Empty;
				int num2 = 0;
				if (string.Equals(obj, fileNameWithoutExtension, StringComparison.OrdinalIgnoreCase))
				{
					num2 += 1000;
				}
				if (obj.IndexOf(fileNameWithoutExtension, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					num2 += 100;
				}
				num2 += (int)(sprite.rect.width + sprite.rect.height);
				if (num2 > num)
				{
					num = num2;
					result = sprite;
				}
			}
		}
		return result;
	}

	private Sprite[] GetSlashFrames()
	{
		if (_slashFramesCache != null && _slashFramesCache.Length != 0)
		{
			return _slashFramesCache;
		}
		Sprite[] array = Resources.FindObjectsOfTypeAll<Sprite>();
		List<Sprite> list = new List<Sprite>();
		Sprite[] array2 = array;
		foreach (Sprite sprite in array2)
		{
			if (!(sprite == null))
			{
				string text = sprite.name ?? string.Empty;
				if (text.IndexOf("charge_slash", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("slash", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("impact", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					list.Add(sprite);
				}
			}
		}
		if (list.Count > 0)
		{
			_slashFramesCache = (from s in list
				orderby s.name.IndexOf("charge_slash", StringComparison.OrdinalIgnoreCase) >= 0 descending, s.name.IndexOf("_0002_", StringComparison.OrdinalIgnoreCase) >= 0 descending, s.name
				select s).ToArray();
			return _slashFramesCache;
		}
		if (slashFrames != null && slashFrames.Length != 0)
		{
			_slashFramesCache = slashFrames;
			return _slashFramesCache;
		}
		return Array.Empty<Sprite>();
	}

	private static bool TryLoadImage(Texture2D tex, byte[] bytes)
	{
		try
		{
			Type type = Type.GetType("UnityEngine.ImageConversion, UnityEngine.ImageConversionModule");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("LoadImage", BindingFlags.Static | BindingFlags.Public, null, new Type[3]
				{
					typeof(Texture2D),
					typeof(byte[]),
					typeof(bool)
				}, null);
				if (method != null)
				{
					method.Invoke(null, new object[3] { tex, bytes, false });
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}
}
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
			Debug.Log("[HelperMod] Gameplay scene detected; scheduling shade spawn on control regain.");
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
					Debug.Log("[HelperMod] Skipping intro sequence");
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
					Debug.Log("[HelperMod] ShadeController.Start: Found Player tag -> set hornetTransform.");
				}
				else
				{
					Debug.Log("[HelperMod] ShadeController.Start: Could not find Player by tag.");
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
					if (shadeMaxHP <= 0)
					{
						shadeMaxHP = num;
					}
					else
					{
						shadeMaxHP = num;
					}
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
			lastSavedHP = -999;
			lastSavedMax = -999;
			lastSavedSoul = -999;
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
				if (Input.GetKeyDown(KeyCode.F9))
				{
					DumpNearestEnemyHealthManager();
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
			if (Input.GetKey(KeyCode.Space) && !(fireTimer > 0f))
			{
				if (shadeSoul < projectileSoulCost)
				{
					Debug.Log($"[HelperMod] Fire blocked: need {projectileSoulCost}, have {shadeSoul}.");
					return;
				}
				fireTimer = fireCooldown;
				shadeSoul = Mathf.Max(0, shadeSoul - projectileSoulCost);
				PushSoulToHud();
				CheckHazardOverlap();
				Vector2 dir = new Vector2(facing, 0f);
				SpawnProjectile(dir);
			}
		}

		private void HandleNailAttack()
		{
			nailTimer -= Time.deltaTime;
			if (!(nailTimer > 0f) && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J)))
			{
				nailTimer = nailCooldown;
				Debug.Log("[HelperMod] HandleNailAttack: Trigger PerformNailSlash().");
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
			Debug.Log("[HelperMod] PerformNailSlash: Begin.");
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
			NailSlash nailSlash2 = null;
			NailSlash nailSlash3 = null;
			NailSlash nailSlash4 = null;
			NailSlash nailSlash5 = null;
			try
			{
				nailSlash2 = instance.GetType().GetProperty("UpSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			try
			{
				nailSlash3 = instance.GetType().GetProperty("DownSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			try
			{
				nailSlash4 = instance.GetType().GetProperty("NormalSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			try
			{
				nailSlash5 = instance.GetType().GetProperty("AlternateSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
			}
			catch
			{
			}
			GameObject gameObject6 = (nailSlash2 ? nailSlash2.gameObject : null);
			GameObject gameObject7 = (nailSlash3 ? nailSlash3.gameObject : null);
			GameObject gameObject8 = (nailSlash4 ? nailSlash4.gameObject : null);
			GameObject gameObject9 = (nailSlash5 ? nailSlash5.gameObject : null);
			GameObject gameObject10 = gameObject2 ?? gameObject6;
			GameObject gameObject11 = gameObject3 ?? gameObject7;
			GameObject gameObject12 = gameObject4 ?? gameObject8;
			GameObject gameObject13 = gameObject5 ?? gameObject9;
			Debug.Log(string.Format("[HelperMod] PerformNailSlash: v={0:F2} up={1} down={2} normal={3} alt={4}.", num, gameObject10 ? gameObject10.name : "null", gameObject11 ? gameObject11.name : "null", gameObject12 ? gameObject12.name : "null", gameObject13 ? gameObject13.name : "null"));
			gameObject = ((num > 0.35f && (bool)gameObject10) ? gameObject10 : ((num < -0.35f && (bool)gameObject11) ? gameObject11 : ((facing < 0) ? (gameObject12 ?? gameObject13) : (gameObject13 ?? gameObject12))));
			if (gameObject == null)
			{
				try
				{
					NailSlash[] array = instance.GetComponentsInChildren<NailSlash>(includeInactive: true);
					Debug.Log($"[HelperMod] PerformNailSlash: Fallback search found {((array != null) ? array.Length : 0)} NailSlash components under Hero.");
					NailSlash nailSlash6 = null;
					if (array == null || array.Length == 0)
					{
						NailSlash[] array2 = Resources.FindObjectsOfTypeAll<NailSlash>();
						Debug.Log($"[HelperMod] PerformNailSlash: Global search found {((array2 != null) ? array2.Length : 0)} NailSlash assets.");
						array = array2;
					}
					if (array != null && array.Length != 0)
					{
						nailSlash6 = ((num > 0.35f) ? Array.Find(array, (NailSlash s) => MatchUp(s)) : ((num < -0.35f) ? Array.Find(array, (NailSlash s) => MatchDown(s)) : ((facing < 0) ? (Array.Find(array, (NailSlash s) => MatchNormal(s) && MatchLeft(s)) ?? Array.Find(array, (NailSlash s) => MatchLeft(s))) : (Array.Find(array, (NailSlash s) => MatchNormal(s) && MatchRight(s)) ?? Array.Find(array, (NailSlash s) => MatchRight(s))))));
						if (nailSlash6 == null)
						{
							nailSlash6 = Array.Find(array, (NailSlash s) => MatchNormal(s));
						}
						if (nailSlash6 == null)
						{
							nailSlash6 = array[0];
						}
						gameObject = (nailSlash6 ? nailSlash6.gameObject : null);
						Debug.Log("[HelperMod] PerformNailSlash: Fallback chose '" + (nailSlash6 ? nailSlash6.name : "<none>") + "'.");
					}
				}
				catch
				{
				}
				if (gameObject == null)
				{
					Debug.Log("[HelperMod] PerformNailSlash: No slash source found.");
					return;
				}
			}
			GameObject gameObject14 = UnityEngine.Object.Instantiate(gameObject, instance.transform);
			gameObject14.transform.position = base.transform.position;
			gameObject14.transform.SetParent(base.transform, worldPositionStays: true);
			Collider2D[] componentsInChildren = gameObject14.GetComponentsInChildren<Collider2D>(includeInactive: true);
			Collider2D[] array3 = componentsInChildren;
			foreach (Collider2D collider2D in array3)
			{
				if ((bool)collider2D)
				{
					collider2D.enabled = false;
				}
			}
			DamageEnemies[] componentsInChildren2 = gameObject14.GetComponentsInChildren<DamageEnemies>(includeInactive: true);
			DamageEnemies[] array4 = componentsInChildren2;
			foreach (DamageEnemies damageEnemies in array4)
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
			Debug.Log("[HelperMod] PerformNailSlash: Instantiated slash clone '" + gameObject14.name + "' under Hero then reparented to Shade (pre-patched).");
			try
			{
				Transform obj7 = gameObject14.transform;
				Vector3 localScale = obj7.localScale;
				localScale.x = Mathf.Abs(localScale.x) * ((facing >= 0) ? 1f : (-1f));
				obj7.localScale = localScale;
				Debug.Log($"[HelperMod] PerformNailSlash: Set facing={facing}.");
			}
			catch
			{
			}
			NailSlash nailSlash = gameObject14.GetComponent<NailSlash>();
			if (nailSlash != null)
			{
				typeof(NailAttackBase).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(nailSlash, instance);
				Debug.Log("[HelperMod] PerformNailSlash: Wired NailSlash.hc.");
				try
				{
					NailSlashTravel component = gameObject14.GetComponent<NailSlashTravel>();
					if (component != null)
					{
						typeof(NailSlashTravel).GetField("hc", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(component, instance);
						Debug.Log("[HelperMod] PerformNailSlash: Wired NailSlashTravel.hc.");
					}
				}
				catch
				{
				}
				try
				{
					NailSlashRecoil[] componentsInChildren4 = gameObject14.GetComponentsInChildren<NailSlashRecoil>(includeInactive: true);
					NailSlashRecoil[] array5 = componentsInChildren4;
					foreach (NailSlashRecoil nailSlashRecoil in array5)
					{
						if ((bool)nailSlashRecoil)
						{
							UnityEngine.Object.Destroy(nailSlashRecoil);
						}
					}
					Debug.Log($"[HelperMod] PerformNailSlash: Removed {((componentsInChildren4 != null) ? componentsInChildren4.Length : 0)} NailSlashRecoil components.");
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
					FieldInfo field11 = typeof(DamageEnemies).GetField("isNailAttack", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field12 = typeof(DamageEnemies).GetField("onlyDamageEnemies", BindingFlags.Instance | BindingFlags.NonPublic);
					MethodInfo method = typeof(DamageEnemies).GetMethod("setOnlyDamageEnemies", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					FieldInfo field13 = typeof(DamageEnemies).GetField("silkGeneration", BindingFlags.Instance | BindingFlags.NonPublic);
					FieldInfo field14 = typeof(DamageEnemies).GetField("doesNotGenerateSilk", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					FieldInfo field15 = typeof(DamageEnemies).GetField("attackType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					int num2 = 0;
					array4 = componentsInChildren5;
					foreach (DamageEnemies damageEnemies2 in array4)
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
							field11?.SetValue(damageEnemies2, false);
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
							float num3 = 0f;
							num3 = ((num > 0.35f) ? 90f : ((!(num < -0.35f)) ? ((facing >= 0) ? 0f : 180f) : 270f));
							field7?.SetValue(damageEnemies2, num3);
							field8?.SetValue(damageEnemies2, false);
							field9?.SetValue(damageEnemies2, false);
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
								field12?.SetValue(damageEnemies2, false);
							}
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
						num2++;
					}
					Debug.Log($"[HelperMod] PerformNailSlash: Patched DamageEnemies hero flags on {num2} component(s).");
				}
				catch
				{
				}
				try
				{
					Collider2D[] componentsInChildren6 = gameObject14.GetComponentsInChildren<Collider2D>(includeInactive: true);
					Collider2D[] componentsInChildren7 = instance.GetComponentsInChildren<Collider2D>(includeInactive: true);
					array3 = componentsInChildren6;
					foreach (Collider2D collider2D2 in array3)
					{
						Collider2D[] array6 = componentsInChildren7;
						foreach (Collider2D collider2D3 in array6)
						{
							if ((bool)collider2D2 && (bool)collider2D3)
							{
								Physics2D.IgnoreCollision(collider2D2, collider2D3, ignore: true);
							}
						}
					}
				}
				catch
				{
				}
				try
				{
					HeroExtraNailSlash[] componentsInChildren8 = gameObject14.GetComponentsInChildren<HeroExtraNailSlash>(includeInactive: true);
					foreach (HeroExtraNailSlash heroExtraNailSlash in componentsInChildren8)
					{
						if ((bool)heroExtraNailSlash)
						{
							UnityEngine.Object.Destroy(heroExtraNailSlash);
						}
					}
					NailSlashTravel[] componentsInChildren9 = gameObject14.GetComponentsInChildren<NailSlashTravel>(includeInactive: true);
					foreach (NailSlashTravel nailSlashTravel in componentsInChildren9)
					{
						if ((bool)nailSlashTravel)
						{
							UnityEngine.Object.Destroy(nailSlashTravel);
						}
					}
					NailSlashTerrainThunk[] componentsInChildren10 = gameObject14.GetComponentsInChildren<NailSlashTerrainThunk>(includeInactive: true);
					foreach (NailSlashTerrainThunk nailSlashTerrainThunk in componentsInChildren10)
					{
						if ((bool)nailSlashTerrainThunk)
						{
							UnityEngine.Object.Destroy(nailSlashTerrainThunk);
						}
					}
					Transform transform2 = gameObject14.transform.Find("Extra Damager");
					if ((bool)transform2)
					{
						transform2.gameObject.SetActive(value: false);
					}
					DamageEnemies[] componentsInChildren11 = gameObject14.GetComponentsInChildren<DamageEnemies>(includeInactive: true);
					bool flag = false;
					array4 = componentsInChildren11;
					foreach (DamageEnemies damageEnemies3 in array4)
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
						NailSlash nailSlash7 = instance.GetType().GetProperty("AlternateSlash", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance, null) as NailSlash;
						if (nailSlash7 != null && !string.IsNullOrEmpty(nailSlash7.animName))
						{
							nailSlash.animName = nailSlash7.animName;
							Debug.Log("[HelperMod] PerformNailSlash: Using AlternateSlash anim '" + nailSlash.animName + "' for right-facing side slash.");
						}
					}
				}
				catch
				{
				}
				array4 = componentsInChildren2;
				foreach (DamageEnemies damageEnemies4 in array4)
				{
					if ((bool)damageEnemies4)
					{
						damageEnemies4.enabled = true;
					}
				}
				array3 = componentsInChildren;
				foreach (Collider2D collider2D4 in array3)
				{
					if ((bool)collider2D4)
					{
						collider2D4.enabled = true;
					}
				}
				try
				{
					if (Mathf.Abs(num) < 0.35f)
					{
						gameObject14.AddComponent<SlashForwardFilter>().Init(fwd: (facing >= 0) ? Vector2.right : Vector2.left, shadeTransform: base.transform, duration: 0.25f);
					}
				}
				catch
				{
				}
				Debug.Log("[HelperMod] PerformNailSlash: Calling StartSlash().");
				nailSlash.StartSlash();
				try
				{
					DamageEnemies primaryDamager = nailSlash.EnemyDamager;
					if (primaryDamager != null)
					{
						Debug.Log("[HelperMod] PerformNailSlash: Hook DamagedEnemy for soul gain.");
						Action onDamaged = null;
						Action<bool> onEnded = null;
						onDamaged = delegate
						{
							Debug.Log("[HelperMod] DamagedEnemy: +11 soul.");
							shadeSoul = Mathf.Min(shadeSoulMax, shadeSoul + soulGainPerHit);
							PushSoulToHud();
							CheckHazardOverlap();
						};
						primaryDamager.DamagedEnemy += onDamaged;
						onEnded = delegate
						{
							try
							{
								primaryDamager.DamagedEnemy -= onDamaged;
							}
							catch
							{
							}
							try
							{
								nailSlash.EndedDamage -= onEnded;
							}
							catch
							{
							}
							Debug.Log("[HelperMod] EndedDamage: unsubscribed handlers.");
						};
						nailSlash.EndedDamage += onEnded;
					}
					else
					{
						Debug.Log("[HelperMod] PerformNailSlash: EnemyDamager was null.");
					}
				}
				catch
				{
				}
			}
			else
			{
				Debug.Log("[HelperMod] PerformNailSlash: NailSlash component not found on clone.");
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
				string text3 = (ns.name ?? "").ToLowerInvariant();
				string text4 = (ns.animName ?? "").ToLowerInvariant();
				if (!text3.Contains("left"))
				{
					return text4.Contains("left");
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
				string text = (ns.name ?? "").ToLowerInvariant();
				string text2 = (ns.animName ?? "").ToLowerInvariant();
				if (!text.Contains("alt") && !text.Contains("right") && !text2.Contains("alt"))
				{
					return text2.Contains("right");
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
			SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = MakeDotSprite();
			if (sr != null)
			{
				spriteRenderer.sortingLayerID = sr.sortingLayerID;
				spriteRenderer.sortingOrder = sr.sortingOrder + 1;
				spriteRenderer.color = Color.black;
			}
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

		private void DumpNearestEnemyHealthManager()
		{
			MonoBehaviour[] array = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			MonoBehaviour monoBehaviour = null;
			float num = float.PositiveInfinity;
			MonoBehaviour[] array2 = array;
			foreach (MonoBehaviour monoBehaviour2 in array2)
			{
				if (!(monoBehaviour2 == null) && !(monoBehaviour2.GetType().Name != "HealthManager"))
				{
					float num2 = Vector2.Distance(monoBehaviour2.transform.position, base.transform.position);
					if (num2 < num)
					{
						num = num2;
						monoBehaviour = monoBehaviour2;
					}
				}
			}
			if (monoBehaviour == null)
			{
				Debug.Log("[HelperMod] F9 dump: No HealthManager found nearby.");
				return;
			}
			Type type = monoBehaviour.GetType();
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			string arg = string.Join(", ", Array.ConvertAll(methods, (MethodInfo m) => m.Name + "(" + string.Join(",", Array.ConvertAll(m.GetParameters(), (ParameterInfo p) => p.ParameterType.Name + " " + p.Name)) + ")"));
			string text = string.Join(", ", Array.ConvertAll(fields, (FieldInfo f) => f.FieldType.Name + " " + f.Name));
			string text2 = string.Join(", ", Array.ConvertAll(properties, (PropertyInfo p) => p.PropertyType.Name + " " + p.Name));
			Debug.Log($"[HelperMod] F9 HealthManager @dist {num:F1} methods: {arg}");
			Debug.Log("[HelperMod] F9 HealthManager fields : " + text);
			Debug.Log("[HelperMod] F9 HealthManager props  : " + text2);
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
				try
				{
					Debug.Log($"[HelperMod] ShadeLight simple setup: size={simpleLightSize} layer={meshRenderer.sortingLayerID} order={meshRenderer.sortingOrder}");
				}
				catch
				{
				}
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
					Shader shader = Shader.Find("Sprites/Default");
					if (shader == null)
					{
						shader = Shader.Find("Unlit/Transparent");
					}
					s_simpleAdditiveMat = new Material(shader);
					s_simpleAdditiveMat.name = "ShadeLightAdditiveMat";
					s_simpleAdditiveMat.mainTexture = s_simpleLightTex;
					s_simpleAdditiveMat.renderQueue = 3000;
					Color value = new Color(1f, 1f, 1f, 0.35f);
					try
					{
						s_simpleAdditiveMat.SetColor("_Color", value);
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

	public class SlashForwardFilter : MonoBehaviour
	{
		private Transform shade;

		private Vector2 forward;

		private float timeLeft;

		private int lastEnabledCount;

		public void Init(Transform shadeTransform, Vector2 fwd, float duration)
		{
			shade = shadeTransform;
			forward = fwd.normalized;
			timeLeft = duration;
			lastEnabledCount = -1;
		}

		private void Update()
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0f)
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			Collider2D[] componentsInChildren = GetComponentsInChildren<Collider2D>(includeInactive: true);
			int num = 0;
			Collider2D[] array = componentsInChildren;
			foreach (Collider2D collider2D in array)
			{
				if ((bool)collider2D && collider2D.enabled)
				{
					num++;
				}
			}
			if (num != 0)
			{
				if (num != lastEnabledCount)
				{
					lastEnabledCount = num;
					FilterForward(componentsInChildren);
				}
				else
				{
					FilterForward(componentsInChildren);
				}
			}
		}

		private void FilterForward(Collider2D[] cols)
		{
			if (!shade)
			{
				return;
			}
			Vector2 vector = shade.position;
			foreach (Collider2D collider2D in cols)
			{
				if ((bool)collider2D && collider2D.enabled)
				{
					Vector2 normalized = ((Vector2)collider2D.bounds.center - vector).normalized;
					if (Vector2.Dot(forward, normalized) < 0f)
					{
						collider2D.enabled = false;
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(InputHandler), "MapKeyboardLayoutFromGameSettings")]
	private class BlockKeyboardRebinding
	{
		private static bool Prefix()
		{
			Debug.Log("[HelperMod] Prevented rebinding of Hornetâ€™s keyboard controls.");
			return false;
		}
	}

	[HarmonyPatch(typeof(InputHandler), "MapDefaultKeyboardLayout")]
	private static class BlockDefaultKeyboardMap
	{
		private static bool Prefix()
		{
			Debug.Log("[HelperMod] Blocked default keyboard layout for Hornet.");
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
		base.Logger.LogInfo("Patching GameManager.BeginScene...");
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
		bool flag = !loggedStartupFields;
		loggedStartupFields = true;
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType != typeof(bool))
			{
				continue;
			}
			if (flag)
			{
				bool flag2 = false;
				try
				{
					flag2 = (bool)fieldInfo.GetValue(gm);
				}
				catch
				{
				}
				Debug.Log($"[HelperMod] GameManager bool field {fieldInfo.Name}={flag2}");
			}
			string text = fieldInfo.Name.ToLower();
			if (text.Contains("logo") || (text.Contains("save") && text.Contains("reminder")))
			{
				try
				{
					fieldInfo.SetValue(gm, false);
					Debug.Log("[HelperMod] Disabled GameManager field " + fieldInfo.Name);
				}
				catch
				{
				}
			}
		}
	}
}
