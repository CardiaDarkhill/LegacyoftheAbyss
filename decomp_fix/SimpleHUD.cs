using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SimpleHUD : MonoBehaviour
{
	private Sprite[] _slashFramesCache;

	private AudioSource sfxSource;

	private List<AudioClip> shadeHurtCandidates;

	private int shadeHurtIdx;

	private AudioClip pinnedHurtSingle;

	private AudioClip pinnedHurtDouble;

	private PlayerData playerData;

	private Image[] maskImages;

	private Sprite maskSprite;

	private readonly Color missingMaskColor = new Color(0.2f, 0.2f, 0.2f, 0.45f);

	private Sprite soulOrbSprite;

	private RectTransform soulOrbRoot;

	private RectTransform soulRevealMask;

	private Image soulImage;

	private Image soulBgImage;

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
			}
			if (shadeHurtCandidates != null && shadeHurtCandidates.Count > 0)
			{
				AudioClip audioClip = shadeHurtCandidates[shadeHurtIdx % shadeHurtCandidates.Count];
				shadeHurtIdx++;
				if (audioClip != null)
				{
					sfxSource.PlayOneShot(audioClip);
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
					orderby x.n.IndexOf("take", StringComparison.OrdinalIgnoreCase) >= 0 && x.n.IndexOf("hit", StringComparison.OrdinalIgnoreCase) >= 0 descending, x.n.IndexOf("hornet", StringComparison.OrdinalIgnoreCase) >= 0 descending, x.n.IndexOf("hurt", StringComparison.OrdinalIgnoreCase) >= 0 descending, x.n.IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0 descending
					select x.clip)
				{
					if (item2 != null && hashSet.Add(item2))
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
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				object obj = instance.GetType().GetField("HeroFSM", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(instance);
				if (obj != null)
				{
					PropertyInfo property = obj.GetType().GetProperty("States", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					IEnumerable enumerable = ((property != null) ? (property.GetValue(obj, null) as IEnumerable) : null);
					if (enumerable != null)
					{
						foreach (object item in enumerable)
						{
							PropertyInfo property2 = item.GetType().GetProperty("Actions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
							IEnumerable enumerable2 = ((property2 != null) ? (property2.GetValue(item, null) as IEnumerable) : null);
							if (enumerable2 == null)
							{
								continue;
							}
							foreach (object item2 in enumerable2)
							{
								if (item2 == null)
								{
									continue;
								}
								try
								{
									PropertyInfo[] properties = item2.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
									foreach (PropertyInfo propertyInfo in properties)
									{
										if (propertyInfo.PropertyType == typeof(AudioClip))
										{
											AudioClip audioClip = propertyInfo.GetValue(item2, null) as AudioClip;
											if (audioClip != null && !list.Contains(audioClip))
											{
												list.Add(audioClip);
											}
										}
										Type propertyType = propertyInfo.PropertyType;
										if (propertyType != null && propertyType.FullName != null && propertyType.FullName.Contains("UnityEngine.AudioSource"))
										{
											AudioSource audioSource = propertyInfo.GetValue(item2, null) as AudioSource;
											if (audioSource != null && audioSource.clip != null && !list.Contains(audioSource.clip))
											{
												list.Add(audioSource.clip);
											}
										}
										if (!(propertyType != null) || propertyType.FullName == null || !propertyType.FullName.Contains("HutongGames.PlayMaker.FsmObject"))
										{
											continue;
										}
										object value = propertyInfo.GetValue(item2, null);
										if (value != null)
										{
											PropertyInfo property3 = value.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
											AudioClip audioClip2 = ((property3 != null) ? property3.GetValue(value, null) : null) as AudioClip;
											if (audioClip2 != null && !list.Contains(audioClip2))
											{
												list.Add(audioClip2);
											}
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
		}
		catch
		{
		}
		return list;
	}

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

	private void Update()
	{
		if (playerData == null)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Minus))
		{
			shadeHealth = Mathf.Max(0, shadeHealth - 1);
			try
			{
				Debug.Log("[SimpleHUD] Debug: Shade HP -1");
			}
			catch
			{
			}
		}
		if (Input.GetKeyDown(KeyCode.Equals))
		{
			shadeHealth = Mathf.Min(shadeMax, shadeHealth + 1);
			try
			{
				Debug.Log("[SimpleHUD] Debug: Shade HP +1");
			}
			catch
			{
			}
		}
		float num = (shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax));
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
				try
				{
					Debug.Log("[SimpleHUD] Debug: Shade Soul +11");
				}
				catch
				{
				}
			}
			else
			{
				float num3 = (debugUseCustomSilk ? debugSilk : ((float)playerData.silk));
				debugUseCustomSilk = true;
				debugSilk = Mathf.Min(num3 + num2, num);
				try
				{
					Debug.Log("[SimpleHUD] Debug: Hornet Silk +step");
				}
				catch
				{
				}
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
				try
				{
					Debug.Log("[SimpleHUD] Debug: Shade Soul -11");
				}
				catch
				{
				}
			}
			else
			{
				float num4 = (debugUseCustomSilk ? debugSilk : ((float)playerData.silk));
				debugUseCustomSilk = true;
				debugSilk = Mathf.Max(num4 - num2, 0f);
				try
				{
					Debug.Log("[SimpleHUD] Debug: Hornet Silk -step");
				}
				catch
				{
				}
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
				try
				{
					Debug.Log("[SimpleHUD] Debug: Shade Soul reset");
				}
				catch
				{
				}
			}
			else
			{
				debugUseCustomSilk = false;
				debugSilk = playerData.silk;
				try
				{
					Debug.Log("[SimpleHUD] Debug: Hornet Silk reset");
				}
				catch
				{
				}
			}
		}
		SyncShadeFromPlayer();
		RefreshHealth();
		RefreshSoul();
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
		healthContainer = new GameObject("HealthContainer");
		healthContainer.transform.SetParent(canvas.transform, worldPositionStays: false);
		RectTransform rectTransform4 = healthContainer.AddComponent<RectTransform>();
		anchorMin = (rectTransform4.anchorMax = new Vector2(1f, 1f));
		rectTransform4.anchorMin = anchorMin;
		rectTransform4.pivot = new Vector2(1f, 1f);
		Vector2 obj = ((maskSprite != null) ? new Vector2(maskSprite.rect.width, maskSprite.rect.height) : new Vector2(33f, 41f));
		float num = obj.y * uIScale * 0.88f;
		float num2 = soulOrbRoot.anchoredPosition.y - soulOrbRoot.sizeDelta.y * 0.5f;
		rectTransform4.anchoredPosition = new Vector2(soulOrbRoot.anchoredPosition.x, num2 + num * 0.5f);
		BuildMasks(rectTransform4, uIScale);
	}

	private void RefreshHealth()
	{
		int num = shadeHealth;
		if (maskImages == null)
		{
			return;
		}
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
		if (!(img == null))
		{
			for (int i = 0; i < 2; i++)
			{
				img.color = Color.white;
				yield return new WaitForSeconds(0.05f);
				img.color = Color.red;
				yield return new WaitForSeconds(0.05f);
			}
			img.color = missingMaskColor;
		}
	}

	private void RefreshSoul()
	{
		if (!(soulOrbRoot == null))
		{
			float num = (shadeSoulOverride ? Mathf.Max(1f, shadeSoulMax) : Mathf.Max(1f, playerData.silkMax));
			float fillAmount = Mathf.Clamp01((shadeSoulOverride ? Mathf.Clamp(shadeSoul, 0f, num) : (debugUseCustomSilk ? debugSilk : ((float)playerData.silk))) / num);
			if (soulImage != null)
			{
				soulImage.fillAmount = fillAmount;
			}
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
}
