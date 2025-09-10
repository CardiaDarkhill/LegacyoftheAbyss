using System;
using UnityEngine;

// Token: 0x02000532 RID: 1330
public class PressurePlateBinarySwitcher : MonoBehaviour
{
	// Token: 0x06002FBA RID: 12218 RVA: 0x000D21DC File Offset: 0x000D03DC
	private void Awake()
	{
		this.switchables = base.GetComponentsInChildren<IBinarySwitchable>();
	}

	// Token: 0x06002FBB RID: 12219 RVA: 0x000D21EC File Offset: 0x000D03EC
	private void Start()
	{
		if (this.defaultPlate)
		{
			this.defaultPlate.Activated += this.Toggle;
		}
		if (this.altPlate)
		{
			this.altPlate.Activated += this.Toggle;
		}
		this.SetPlatesActivated(false);
	}

	// Token: 0x06002FBC RID: 12220 RVA: 0x000D2248 File Offset: 0x000D0448
	[ContextMenu("Test", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x000D224F File Offset: 0x000D044F
	[ContextMenu("Test")]
	public void Toggle()
	{
		this.SetPlatesActivated(!this.isCurrentlyAlt);
	}

	// Token: 0x06002FBE RID: 12222 RVA: 0x000D2260 File Offset: 0x000D0460
	private void SetPlatesActivated(bool isAlt)
	{
		this.isCurrentlyAlt = isAlt;
		if (isAlt)
		{
			if (this.defaultPlate)
			{
				this.defaultPlate.Deactivate();
			}
			if (this.altPlate)
			{
				this.altPlate.ActivateSilent();
			}
		}
		else
		{
			if (this.defaultPlate)
			{
				this.defaultPlate.ActivateSilent();
			}
			if (this.altPlate)
			{
				this.altPlate.Deactivate();
			}
		}
		IBinarySwitchable[] array = this.switchables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SwitchBinaryState(this.isCurrentlyAlt);
		}
	}

	// Token: 0x0400327D RID: 12925
	[SerializeField]
	private TempPressurePlate defaultPlate;

	// Token: 0x0400327E RID: 12926
	[SerializeField]
	private TempPressurePlate altPlate;

	// Token: 0x0400327F RID: 12927
	private bool isCurrentlyAlt;

	// Token: 0x04003280 RID: 12928
	private IBinarySwitchable[] switchables;
}
