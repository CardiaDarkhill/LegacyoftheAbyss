using System;
using UnityEngine;

// Token: 0x0200013E RID: 318
public sealed class VolumeModifier
{
	// Token: 0x1400000F RID: 15
	// (add) Token: 0x060009D8 RID: 2520 RVA: 0x0002CA3C File Offset: 0x0002AC3C
	// (remove) Token: 0x060009D9 RID: 2521 RVA: 0x0002CA74 File Offset: 0x0002AC74
	public event Action OnValueChanged;

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060009DA RID: 2522 RVA: 0x0002CAA9 File Offset: 0x0002ACA9
	// (set) Token: 0x060009DB RID: 2523 RVA: 0x0002CAB1 File Offset: 0x0002ACB1
	public bool IsValid { get; private set; }

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060009DC RID: 2524 RVA: 0x0002CABA File Offset: 0x0002ACBA
	// (set) Token: 0x060009DD RID: 2525 RVA: 0x0002CAC4 File Offset: 0x0002ACC4
	public float Volume
	{
		get
		{
			return this.volume;
		}
		set
		{
			float b = Mathf.Clamp01(value);
			if (!Mathf.Approximately(this.volume, b))
			{
				this.volume = b;
				Action onValueChanged = this.OnValueChanged;
				if (onValueChanged == null)
				{
					return;
				}
				onValueChanged();
			}
		}
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x0002CAFD File Offset: 0x0002ACFD
	public VolumeModifier(float initial)
	{
		this.volume = Mathf.Clamp01(initial);
		this.IsValid = true;
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x0002CB18 File Offset: 0x0002AD18
	public void SetInvalid()
	{
		this.IsValid = false;
	}

	// Token: 0x04000968 RID: 2408
	private float volume;
}
