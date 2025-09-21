using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000BA RID: 186
public class SpriteFlash : MonoBehaviour
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x0600059B RID: 1435 RVA: 0x0001D3FB File Offset: 0x0001B5FB
	// (set) Token: 0x0600059C RID: 1436 RVA: 0x0001D403 File Offset: 0x0001B603
	public Color ExtraMixColor { get; set; }

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600059D RID: 1437 RVA: 0x0001D40C File Offset: 0x0001B60C
	// (set) Token: 0x0600059E RID: 1438 RVA: 0x0001D414 File Offset: 0x0001B614
	public float ExtraMixAmount
	{
		get
		{
			return this.extraMixAmount;
		}
		set
		{
			this.extraMixAmount = Mathf.Clamp01(value);
			this.UpdateMaterial();
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x0600059F RID: 1439 RVA: 0x0001D428 File Offset: 0x0001B628
	// (set) Token: 0x060005A0 RID: 1440 RVA: 0x0001D430 File Offset: 0x0001B630
	public bool IsBlocked
	{
		get
		{
			return this.isBlocked;
		}
		set
		{
			this.isBlocked = value;
			this.UpdateMaterial();
		}
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x0001D43F File Offset: 0x0001B63F
	private void OnValidate()
	{
		if (this.doHitFlashOnEnable)
		{
			this.startFlashes |= SpriteFlash.StartFlashes.HitFlash;
			this.doHitFlashOnEnable = false;
		}
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x0001D460 File Offset: 0x0001B660
	private void Awake()
	{
		this.OnValidate();
		if (this.getParent && base.transform.parent)
		{
			SpriteFlash componentInParent = base.transform.parent.GetComponentInParent<SpriteFlash>();
			if (componentInParent)
			{
				this.parents.Add(componentInParent);
			}
		}
		foreach (SpriteFlash spriteFlash in this.parents)
		{
			if (spriteFlash)
			{
				spriteFlash.children.AddIfNotPresent(this);
			}
		}
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x0001D508 File Offset: 0x0001B708
	private void OnDisable()
	{
		ComponentSingleton<SpriteFlashCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
		this.CancelFlash();
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x0001D526 File Offset: 0x0001B726
	private void OnEnable()
	{
		ComponentSingleton<SpriteFlashCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		this.DoStartFlashes();
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x0001D544 File Offset: 0x0001B744
	private void DoStartFlashes()
	{
		if (this.startFlashes.HasFlag(SpriteFlash.StartFlashes.HitFlash))
		{
			this.FlashEnemyHit();
		}
		if (this.startFlashes.HasFlag(SpriteFlash.StartFlashes.QuestPickup))
		{
			this.flashWhiteLong();
			this.FlashingWhiteLong();
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0001D594 File Offset: 0x0001B794
	private void OnUpdate()
	{
		if (this.flashChanged)
		{
			this.UpdateMaterial();
		}
		if (this.geoFlash)
		{
			if (this.geoTimer > 0f)
			{
				this.geoTimer -= Time.deltaTime;
				return;
			}
			this.Flash(new Color(1f, 1f, 1f), 0.7f, 0.2f, 0.01f, 0.2f, 0.1f, true, 5, 0, false);
			this.geoFlash = false;
		}
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x0001D616 File Offset: 0x0001B816
	public void GeoFlash()
	{
		this.geoFlash = true;
		this.geoTimer = 0.25f;
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0001D62C File Offset: 0x0001B82C
	private void UpdateMaterial()
	{
		this.flashChanged = false;
		Coroutine coroutine;
		float num;
		Color color;
		if (this.currentRepeatingFlash != null)
		{
			coroutine = this.currentRepeatingFlash.Routine;
			num = this.currentRepeatingFlash.Amount;
			color = this.currentRepeatingFlash.Colour;
		}
		else
		{
			coroutine = null;
			num = 0f;
			color = Color.black;
		}
		float num2;
		Color color2;
		if (this.singleFlashRoutine != null && coroutine == null)
		{
			num2 = this.singleFlashAmount;
			color2 = this.singleFlashColour;
		}
		else if (this.singleFlashRoutine == null && coroutine != null)
		{
			num2 = num;
			color2 = color;
		}
		else if (this.singleFlashRoutine != null && coroutine != null)
		{
			num2 = Mathf.Lerp(num, this.singleFlashAmount, this.singleFlashAmount);
			color2 = Color.Lerp(color, this.singleFlashColour, this.singleFlashAmount);
		}
		else
		{
			num2 = 0f;
			color2 = Color.white;
		}
		if (this.extraMixAmount > 0f)
		{
			num2 = Mathf.Lerp(num2, 1f, this.extraMixAmount);
			color2 = Color.Lerp(color2, this.ExtraMixColor, this.extraMixAmount);
		}
		if (this.isBlocked)
		{
			num2 = 0f;
		}
		this.SetParams(num2, color2);
		SpriteFlash.SetParamsChildrenRecursive(this, num2, color2);
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0001D740 File Offset: 0x0001B940
	private static void SetParamsChildrenRecursive(SpriteFlash parent, float flashAmount, Color flashColour)
	{
		parent.children.RemoveAll((SpriteFlash o) => o == null);
		for (int i = parent.children.Count - 1; i >= 0; i--)
		{
			SpriteFlash spriteFlash = parent.children[i];
			if (!(spriteFlash == parent))
			{
				spriteFlash.SetParams(flashAmount, flashColour);
				SpriteFlash.SetParamsChildrenRecursive(spriteFlash, flashAmount, flashColour);
			}
		}
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x0001D7B8 File Offset: 0x0001B9B8
	private void SetParams(float flashAmount, Color flashColour)
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		else
		{
			this.block.Clear();
		}
		if (this.renderer == null)
		{
			this.renderer = base.GetComponent<Renderer>();
		}
		if (!this.renderer)
		{
			return;
		}
		this.GetPropertyBlock(this.block);
		this.block.SetFloat(SpriteFlash._flashAmountId, flashAmount);
		this.block.SetColor(SpriteFlash._flashColorId, flashColour);
		this.SetPropertyBlock(this.block);
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x0001D848 File Offset: 0x0001BA48
	public SpriteFlash.FlashHandle Flash(SpriteFlash.FlashConfig config)
	{
		return this.Flash(config.Colour, config.Amount, config.TimeUp, config.StayTime, config.TimeDown, 0f, false, 0, 0, false);
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x0001D884 File Offset: 0x0001BA84
	public SpriteFlash.FlashHandle Flash(Color flashColour, float amount, float timeUp, float stayTime, float timeDown, float stayDownTime = 0f, bool repeating = false, int repeatTimes = 0, int repeatingPriority = 0, bool requireExplicitCancel = false)
	{
		this.lastFlashId++;
		if (repeating)
		{
			SpriteFlash.RepeatingFlash repeatingFlash = new SpriteFlash.RepeatingFlash
			{
				Colour = flashColour,
				Handle = new SpriteFlash.FlashHandle(this.lastFlashId, this),
				Priority = repeatingPriority,
				RequireExplicitCancel = requireExplicitCancel
			};
			repeatingFlash.Routine = base.StartCoroutine(this.FlashRoutine(amount, timeUp, stayTime, timeDown, stayDownTime, true, repeatTimes, repeatingFlash));
			this.repeatingFlashes.Add(repeatingFlash);
			this.UpdateCurrentRepeatingFlash();
			return repeatingFlash.Handle;
		}
		this.CancelSingleFlash();
		this.singleFlashColour = flashColour;
		if (base.gameObject.activeInHierarchy)
		{
			this.singleFlashRoutine = base.StartCoroutine(this.FlashRoutine(amount, timeUp, stayTime, timeDown, stayDownTime, false, 0, null));
		}
		this.singleFlashHandle = new SpriteFlash.FlashHandle(this.lastFlashId, this);
		return this.singleFlashHandle;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0001D958 File Offset: 0x0001BB58
	public bool IsFlashing(bool repeating, SpriteFlash.FlashHandle flashHandle)
	{
		if (repeating)
		{
			using (List<SpriteFlash.RepeatingFlash>.Enumerator enumerator = this.repeatingFlashes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Handle.Equals(flashHandle))
					{
						return true;
					}
				}
			}
			return false;
		}
		return this.singleFlashRoutine != null && flashHandle.Equals(this.singleFlashHandle);
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0001D9D4 File Offset: 0x0001BBD4
	public bool IsFlashing(int ID)
	{
		using (List<SpriteFlash.RepeatingFlash>.Enumerator enumerator = this.repeatingFlashes.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Handle.ID == ID)
				{
					return true;
				}
			}
		}
		return this.singleFlashRoutine != null && this.singleFlashHandle.ID == ID;
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0001DA4C File Offset: 0x0001BC4C
	public void CancelFlashByID(int ID)
	{
		this.CancelSingleFlash(ID);
		this.CancelRepeatingFlash(ID);
		if (this.flashChanged)
		{
			this.flashChanged = false;
			this.UpdateMaterial();
		}
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0001DA71 File Offset: 0x0001BC71
	public void CancelFlash()
	{
		this.CancelSingleFlash();
		this.CancelRepeatingFlash();
		this.UpdateMaterial();
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x0001DA85 File Offset: 0x0001BC85
	public void CancelSingleFlash()
	{
		if (this.singleFlashRoutine != null)
		{
			base.StopCoroutine(this.singleFlashRoutine);
			this.singleFlashRoutine = null;
		}
		this.singleFlashAmount = 0f;
		this.flashChanged = true;
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0001DAB4 File Offset: 0x0001BCB4
	public void CancelSingleFlash(SpriteFlash.FlashHandle handle)
	{
		if (!this.singleFlashHandle.Equals(handle))
		{
			return;
		}
		this.CancelSingleFlash();
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x0001DACB File Offset: 0x0001BCCB
	public void CancelSingleFlash(int ID)
	{
		if (this.singleFlashHandle.ID != ID)
		{
			return;
		}
		this.CancelSingleFlash();
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x0001DAE4 File Offset: 0x0001BCE4
	public void CancelRepeatingFlash()
	{
		for (int i = this.repeatingFlashes.Count - 1; i >= 0; i--)
		{
			SpriteFlash.RepeatingFlash repeatingFlash = this.repeatingFlashes[i];
			if (!repeatingFlash.RequireExplicitCancel)
			{
				if (repeatingFlash.Routine != null)
				{
					base.StopCoroutine(repeatingFlash.Routine);
				}
				this.repeatingFlashes.RemoveAt(i);
			}
		}
		this.UpdateCurrentRepeatingFlash();
		this.flashChanged = true;
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x0001DB4C File Offset: 0x0001BD4C
	public void CancelRepeatingFlash(SpriteFlash.FlashHandle handle)
	{
		for (int i = this.repeatingFlashes.Count - 1; i >= 0; i--)
		{
			SpriteFlash.RepeatingFlash repeatingFlash = this.repeatingFlashes[i];
			if (repeatingFlash.Handle.Equals(handle))
			{
				if (repeatingFlash.Routine != null)
				{
					base.StopCoroutine(repeatingFlash.Routine);
				}
				this.repeatingFlashes.RemoveAt(i);
			}
		}
		this.UpdateCurrentRepeatingFlash();
		this.flashChanged = true;
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x0001DBBC File Offset: 0x0001BDBC
	public void CancelRepeatingFlash(int ID)
	{
		bool flag = false;
		for (int i = this.repeatingFlashes.Count - 1; i >= 0; i--)
		{
			SpriteFlash.RepeatingFlash repeatingFlash = this.repeatingFlashes[i];
			if (repeatingFlash.Handle.ID == ID)
			{
				if (repeatingFlash.Routine != null)
				{
					base.StopCoroutine(repeatingFlash.Routine);
				}
				this.repeatingFlashes.RemoveAt(i);
				flag = true;
			}
		}
		if (flag)
		{
			this.UpdateCurrentRepeatingFlash();
			this.flashChanged = true;
		}
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x0001DC30 File Offset: 0x0001BE30
	public SpriteFlash.FlashHandle FlashingSuperDashHandled()
	{
		return this.Flash(new Color(1f, 1f, 1f), 0.7f, 0.1f, 0.01f, 0.1f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x0001DC74 File Offset: 0x0001BE74
	public void FlashingSuperDash()
	{
		this.FlashingSuperDashHandled();
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x0001DC80 File Offset: 0x0001BE80
	public void FlashingGhostWounded()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.7f, 0.5f, 0.01f, 0.5f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x0001DCC8 File Offset: 0x0001BEC8
	public void FlashingWhiteStay()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.6f, 0.01f, 999f, 0.01f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x0001DD10 File Offset: 0x0001BF10
	public void FlashingWhiteStayMoth()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.6f, 2f, 9999f, 2f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x0001DD58 File Offset: 0x0001BF58
	public void FlashingFury()
	{
		this.Flash(new Color(0.71f, 0.18f, 0.18f), 0.75f, 0.25f, 0.01f, 0.25f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x0001DDA0 File Offset: 0x0001BFA0
	[ContextMenu("Test flashing")]
	public void FlashingOrange()
	{
		this.Flash(new Color(1f, 0.31f, 0f), 0.7f, 0.1f, 0.01f, 0.1f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x0001DDE8 File Offset: 0x0001BFE8
	public void FlashingWhite()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.7f, 0.1f, 0.01f, 0.1f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x0001DE30 File Offset: 0x0001C030
	public void FlashingWhiteLong()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.5f, 0.7f, 0.01f, 0.5f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x0001DE78 File Offset: 0x0001C078
	public void FlashingBomb()
	{
		this.Flash(new Color(0.99f, 0.89f, 0.09f), 0.7f, 0.3f, 0.1f, 0.3f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x0001DEC0 File Offset: 0x0001C0C0
	public void FlashingBombFast()
	{
		this.Flash(new Color(0.99f, 0.89f, 0.09f), 0.7f, 0.1f, 0.01f, 0.1f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x0001DF08 File Offset: 0x0001C108
	public void FlashingTarSlug()
	{
		this.Flash(new Color(1f, 0.5f, 0.24f), 0.7f, 0.1f, 0.01f, 0.1f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x0001DF50 File Offset: 0x0001C150
	public void FlashingAcid()
	{
		this.Flash(new Color(0.62f, 0.86f, 0.51f), 0.9f, 0.1f, 0.01f, 0.1f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x0001DF98 File Offset: 0x0001C198
	public void FlashingMossExtract()
	{
		this.Flash(new Color(0.5f, 1f, 0.49f), 0.9f, 0.2f, 0.01f, 0.2f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x0001DFE0 File Offset: 0x0001C1E0
	public void FlashingSwampExtract()
	{
		this.Flash(new Color(0.73f, 0.74f, 0.39f), 0.9f, 0.2f, 0.01f, 0.2f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x0001E028 File Offset: 0x0001C228
	public void FlashingBluebloodExtract()
	{
		this.Flash(new Color(0.55f, 0.9f, 1f), 0.9f, 0.2f, 0.01f, 0.2f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x0001E070 File Offset: 0x0001C270
	public void FlashingQuickened()
	{
		this.Flash(new Color(0.99f, 0.77f, 0.24f), 0.5f, 0.15f, 0.01f, 0.15f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x0001E0B8 File Offset: 0x0001C2B8
	public void flashInfected()
	{
		this.Flash(new Color(1f, 0.31f, 0f), 0.9f, 0.01f, 0.01f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x0001E100 File Offset: 0x0001C300
	public void flashDung()
	{
		this.Flash(new Color(0.45f, 0.27f, 0f), 0.9f, 0.01f, 0.01f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x0001E148 File Offset: 0x0001C348
	public void flashDungQuick()
	{
		this.Flash(new Color(0.45f, 0.27f, 0f), 0.75f, 0.001f, 0.05f, 0.1f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x0001E190 File Offset: 0x0001C390
	public void flashSporeQuick()
	{
		this.Flash(new Color(0.95f, 0.9f, 0.15f), 0.75f, 0.001f, 0.05f, 0.1f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x0001E1D8 File Offset: 0x0001C3D8
	public void flashWhiteQuick()
	{
		this.Flash(new Color(1f, 1f, 1f), 1f, 0.001f, 0.05f, 0.001f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x0001E220 File Offset: 0x0001C420
	public void FlashExtraRosary()
	{
		this.Flash(new Color(1f, 1f, 1f), 1f, 0.001f, 0.5f, 0.4f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x0001E268 File Offset: 0x0001C468
	public void flashInfectedLong()
	{
		this.Flash(new Color(1f, 0.31f, 0f), 0.9f, 0.01f, 0.25f, 0.35f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x0001E2B0 File Offset: 0x0001C4B0
	public void flashArmoured()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.9f, 0.01f, 0.01f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x0001E2F8 File Offset: 0x0001C4F8
	public void flashBenchRest()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.7f, 0.01f, 0.1f, 0.75f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x0001E340 File Offset: 0x0001C540
	public void flashDreamImpact()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.9f, 0.01f, 0.25f, 0.75f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x0001E388 File Offset: 0x0001C588
	public void flashMothDepart()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.75f, 1.9f, 1f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x0001E3D0 File Offset: 0x0001C5D0
	public void flashSoulGet()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.5f, 0.01f, 0.01f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x0001E418 File Offset: 0x0001C618
	public void flashShadeGet()
	{
		this.Flash(new Color(0f, 0f, 0f), 0.5f, 0.01f, 0.01f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x0001E460 File Offset: 0x0001C660
	public void flashWhiteLong()
	{
		this.Flash(new Color(1f, 1f, 1f), 1f, 0.01f, 0.01f, 2f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x0001E4A8 File Offset: 0x0001C6A8
	public void flashOvercharmed()
	{
		this.Flash(new Color(0.72f, 0.376f, 0.72f), 0.75f, 0.2f, 0.01f, 0.5f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x0001E4F0 File Offset: 0x0001C6F0
	public void flashFocusHeal()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.85f, 0.01f, 0.01f, 0.35f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x0001E535 File Offset: 0x0001C735
	public void FlashEnemyHit(HitInstance hitInstance)
	{
		if (hitInstance.AttackType == AttackTypes.Coal)
		{
			this.FlashCoal();
			return;
		}
		this.FlashEnemyHit();
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0001E54E File Offset: 0x0001C74E
	public void FlashEnemyHit()
	{
		this.FlashEnemyHit(Color.white);
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x0001E55B File Offset: 0x0001C75B
	public void FlashEnemyHitRage()
	{
		this.FlashEnemyHit(new Color(1f, 0.6f, 0.6f));
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0001E578 File Offset: 0x0001C778
	public void FlashEnemyHit(Color color)
	{
		this.Flash(color, 0.85f, 0f, 0.07f, 0.05f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x0001E5AC File Offset: 0x0001C7AC
	public void flashFocusGet()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.5f, 0.01f, 0.01f, 0.35f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x0001E5F4 File Offset: 0x0001C7F4
	public void flashFocusGetQuick()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.5f, 0.01f, 0.01f, 0.1f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x0001E63C File Offset: 0x0001C83C
	public void flashWhitePulse()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.35f, 0.5f, 0.01f, 0.75f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x0001E684 File Offset: 0x0001C884
	public void flashHealBlue()
	{
		this.Flash(new Color(0.55f, 0.9f, 1f), 0.85f, 0.01f, 0.01f, 0.5f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x0001E6CC File Offset: 0x0001C8CC
	public void flashHealPoison()
	{
		this.Flash(new Color(0.56f, 0.39f, 0.85f), 0.85f, 0.01f, 0.01f, 0.5f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x0001E714 File Offset: 0x0001C914
	public void FlashShadowRecharge()
	{
		this.Flash(new Color(0f, 0f, 0f), 0.75f, 0.01f, 0.01f, 0.35f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0001E75C File Offset: 0x0001C95C
	public void FlashLavaBellRecharge()
	{
		this.Flash(new Color(0.85882354f, 0.32156864f, 0.33333334f), 0.75f, 0.01f, 0.01f, 0.35f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x0001E7A4 File Offset: 0x0001C9A4
	public void flashInfectedLoop()
	{
		this.Flash(new Color(1f, 0.31f, 0f), 0.9f, 0.2f, 0.01f, 0.2f, 0f, true, 0, 0, false);
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x0001E7EC File Offset: 0x0001C9EC
	public void FlashGrimmflame()
	{
		this.Flash(new Color(1f, 0.25f, 0.25f), 0.75f, 0.01f, 0.01f, 1f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x0001E834 File Offset: 0x0001CA34
	public void FlashCoal()
	{
		this.Flash(new Color(1f, 0.55f, 0.1f), 0.75f, 0.01f, 0.01f, 0.2f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x0001E87C File Offset: 0x0001CA7C
	public void FlashAcid()
	{
		this.Flash(new Color(0.62f, 0.86f, 0.51f), 0.75f, 0.2f, 0.01f, 0.2f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x0001E8C4 File Offset: 0x0001CAC4
	public void FlashGrimmHit()
	{
		this.Flash(new Color(1f, 0.25f, 0.25f), 0.75f, 0.01f, 0.01f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0001E90C File Offset: 0x0001CB0C
	public void FlashBossFinalHit()
	{
		this.Flash(new Color(1f, 1f, 1f), 0.8f, 0.01f, 0.1f, 0.25f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0001E954 File Offset: 0x0001CB54
	public void FlashDazzleQuick()
	{
		this.Flash(new Color(1f, 0.7f, 1f), 0.5f, 0.001f, 0.05f, 0.1f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x0001E99C File Offset: 0x0001CB9C
	public void FlashWitchPoison()
	{
		this.Flash(Effects.EnemyWitchPoisonBloodBurst.Colour, 0.7f, 0.001f, 0.05f, 0.1f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x0001E9D8 File Offset: 0x0001CBD8
	public void FlashZapExplosion()
	{
		this.Flash(new Color(0.96f, 0.37f, 0.92f), 0.75f, 0.01f, 0.1f, 1f, 0f, false, 0, 0, false);
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x0001EA20 File Offset: 0x0001CC20
	public SpriteFlash.FlashHandle FlashingFrosted()
	{
		return this.Flash(new Color(0.6f, 0.8f, 1f), 0.5f, 0.2f, 0.01f, 0.3f, 1f, true, 0, 0, true);
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x0001EA64 File Offset: 0x0001CC64
	public SpriteFlash.FlashHandle FlashingFrostAntic()
	{
		return this.Flash(new Color(0.6f, 0.8f, 1f), 0.65f, 0.15f, 0.01f, 0.2f, 0f, true, 0, 2, false);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x0001EAA8 File Offset: 0x0001CCA8
	public SpriteFlash.FlashHandle FlashingMaggot()
	{
		return this.Flash(Effects.MossEffectsTintDust, 0.5f, 0.2f, 0.01f, 0.3f, 1f, true, 0, 0, true);
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x0001EADD File Offset: 0x0001CCDD
	private void SetPropertyBlock(MaterialPropertyBlock setBlock)
	{
		if (!this.renderer)
		{
			return;
		}
		this.renderer.SetPropertyBlock(setBlock);
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x0001EAF9 File Offset: 0x0001CCF9
	private void GetPropertyBlock(MaterialPropertyBlock getBlock)
	{
		if (!this.renderer)
		{
			return;
		}
		this.renderer.GetPropertyBlock(getBlock);
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x0001EB18 File Offset: 0x0001CD18
	private IEnumerator FlashRoutine(float amount, float timeUp, float stayTime, float timeDown, float stayDownTime, bool repeating, int repeatTimes, SpriteFlash.RepeatingFlash repeatingFlash)
	{
		bool hadRepeatTimes = repeatTimes > 0;
		do
		{
			int num = repeatTimes;
			repeatTimes = num - 1;
			for (float elapsed = 0f; elapsed < timeUp; elapsed += Time.deltaTime)
			{
				float t = elapsed / timeUp;
				float amount2 = Mathf.Lerp(0f, amount, t);
				if (repeating)
				{
					repeatingFlash.Amount = amount2;
				}
				else
				{
					this.singleFlashAmount = amount2;
				}
				this.flashChanged = true;
				yield return null;
			}
			if (repeating)
			{
				repeatingFlash.Amount = amount;
			}
			else
			{
				this.singleFlashAmount = amount;
			}
			this.flashChanged = true;
			yield return null;
			if (stayTime > 0f)
			{
				yield return new WaitForSeconds(stayTime);
			}
			else
			{
				yield return null;
			}
			for (float elapsed = 0f; elapsed < timeDown; elapsed += Time.deltaTime)
			{
				float t2 = elapsed / timeDown;
				float amount3 = Mathf.Lerp(amount, 0f, t2);
				if (repeating)
				{
					repeatingFlash.Amount = amount3;
				}
				else
				{
					this.singleFlashAmount = amount3;
				}
				this.flashChanged = true;
				yield return null;
			}
			if (repeating)
			{
				repeatingFlash.Amount = 0f;
			}
			else
			{
				this.singleFlashAmount = 0f;
			}
			this.flashChanged = true;
			yield return null;
			if (repeating && stayDownTime > 0f)
			{
				yield return new WaitForSeconds(stayDownTime);
			}
		}
		while (repeating && (!hadRepeatTimes || repeatTimes >= 0));
		this.singleFlashRoutine = null;
		yield break;
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x0001EB70 File Offset: 0x0001CD70
	private void UpdateCurrentRepeatingFlash()
	{
		if (this.repeatingFlashes.Count <= 0)
		{
			this.currentRepeatingFlash = null;
			return;
		}
		int num = int.MinValue;
		foreach (SpriteFlash.RepeatingFlash repeatingFlash in this.repeatingFlashes)
		{
			if (repeatingFlash.Priority > num)
			{
				num = repeatingFlash.Priority;
				this.currentRepeatingFlash = repeatingFlash;
			}
		}
	}

	// Token: 0x040005B0 RID: 1456
	[SerializeField]
	[Tooltip("Mirrors all flashes to children.")]
	private List<SpriteFlash> children;

	// Token: 0x040005B1 RID: 1457
	[SerializeField]
	[Tooltip("Add itself to parents children on Awake.")]
	private List<SpriteFlash> parents;

	// Token: 0x040005B2 RID: 1458
	[SerializeField]
	private bool getParent;

	// Token: 0x040005B3 RID: 1459
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool doHitFlashOnEnable;

	// Token: 0x040005B4 RID: 1460
	[SerializeField]
	private SpriteFlash.StartFlashes startFlashes;

	// Token: 0x040005B5 RID: 1461
	private float geoTimer;

	// Token: 0x040005B6 RID: 1462
	private bool geoFlash;

	// Token: 0x040005B7 RID: 1463
	private Coroutine singleFlashRoutine;

	// Token: 0x040005B8 RID: 1464
	private Renderer renderer;

	// Token: 0x040005B9 RID: 1465
	private MaterialPropertyBlock block;

	// Token: 0x040005BA RID: 1466
	private float singleFlashAmount;

	// Token: 0x040005BB RID: 1467
	private Color singleFlashColour;

	// Token: 0x040005BC RID: 1468
	private SpriteFlash.FlashHandle singleFlashHandle;

	// Token: 0x040005BD RID: 1469
	private readonly List<SpriteFlash.RepeatingFlash> repeatingFlashes = new List<SpriteFlash.RepeatingFlash>();

	// Token: 0x040005BE RID: 1470
	private SpriteFlash.RepeatingFlash currentRepeatingFlash;

	// Token: 0x040005BF RID: 1471
	private bool flashChanged;

	// Token: 0x040005C0 RID: 1472
	private int lastFlashId;

	// Token: 0x040005C2 RID: 1474
	private float extraMixAmount;

	// Token: 0x040005C3 RID: 1475
	private static readonly int _flashAmountId = Shader.PropertyToID("_FlashAmount");

	// Token: 0x040005C4 RID: 1476
	private static readonly int _flashColorId = Shader.PropertyToID("_FlashColor");

	// Token: 0x040005C5 RID: 1477
	private bool isBlocked;

	// Token: 0x02001424 RID: 5156
	public struct FlashHandle : IEquatable<SpriteFlash.FlashHandle>
	{
		// Token: 0x17000C9B RID: 3227
		// (get) Token: 0x060082A7 RID: 33447 RVA: 0x00266763 File Offset: 0x00264963
		public int ID
		{
			get
			{
				return this.id;
			}
		}

		// Token: 0x060082A8 RID: 33448 RVA: 0x0026676B File Offset: 0x0026496B
		public FlashHandle(int id, SpriteFlash owner)
		{
			this.id = id;
			this.owner = owner;
		}

		// Token: 0x060082A9 RID: 33449 RVA: 0x0026677B File Offset: 0x0026497B
		public bool Equals(SpriteFlash.FlashHandle other)
		{
			return this.id == other.id && this.owner == other.owner;
		}

		// Token: 0x04008211 RID: 33297
		private readonly int id;

		// Token: 0x04008212 RID: 33298
		private readonly SpriteFlash owner;
	}

	// Token: 0x02001425 RID: 5157
	private class RepeatingFlash
	{
		// Token: 0x04008213 RID: 33299
		public float Amount;

		// Token: 0x04008214 RID: 33300
		public Color Colour;

		// Token: 0x04008215 RID: 33301
		public SpriteFlash.FlashHandle Handle;

		// Token: 0x04008216 RID: 33302
		public bool RequireExplicitCancel;

		// Token: 0x04008217 RID: 33303
		public int Priority;

		// Token: 0x04008218 RID: 33304
		public Coroutine Routine;
	}

	// Token: 0x02001426 RID: 5158
	[Serializable]
	public struct FlashConfig
	{
		// Token: 0x04008219 RID: 33305
		public Color Colour;

		// Token: 0x0400821A RID: 33306
		[Range(0f, 1f)]
		public float Amount;

		// Token: 0x0400821B RID: 33307
		public float TimeUp;

		// Token: 0x0400821C RID: 33308
		public float StayTime;

		// Token: 0x0400821D RID: 33309
		public float TimeDown;
	}

	// Token: 0x02001427 RID: 5159
	[Flags]
	private enum StartFlashes
	{
		// Token: 0x0400821F RID: 33311
		[UsedImplicitly]
		None = 0,
		// Token: 0x04008220 RID: 33312
		HitFlash = 1,
		// Token: 0x04008221 RID: 33313
		QuestPickup = 2
	}
}
