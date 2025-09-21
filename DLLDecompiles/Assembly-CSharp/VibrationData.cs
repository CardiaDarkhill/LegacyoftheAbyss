using System;
using TeamCherry.PS5;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020007AD RID: 1965
[Serializable]
public struct VibrationData
{
	// Token: 0x170007CA RID: 1994
	// (get) Token: 0x06004581 RID: 17793 RVA: 0x0012EFB3 File Offset: 0x0012D1B3
	public LowFidelityVibrations LowFidelityVibration
	{
		get
		{
			return this.lowFidelityVibration;
		}
	}

	// Token: 0x170007CB RID: 1995
	// (get) Token: 0x06004582 RID: 17794 RVA: 0x0012EFBB File Offset: 0x0012D1BB
	public TextAsset HighFidelityVibration
	{
		get
		{
			return this.highFidelityVibration;
		}
	}

	// Token: 0x170007CC RID: 1996
	// (get) Token: 0x06004583 RID: 17795 RVA: 0x0012EFC3 File Offset: 0x0012D1C3
	public GamepadVibration GamepadVibration
	{
		get
		{
			return this.gamepadVibration;
		}
	}

	// Token: 0x170007CD RID: 1997
	// (get) Token: 0x06004584 RID: 17796 RVA: 0x0012EFCC File Offset: 0x0012D1CC
	public float Strength
	{
		get
		{
			OverrideFloat overrideFloat = this.strength;
			if (overrideFloat == null || !overrideFloat.IsEnabled)
			{
				return 1f;
			}
			return this.strength.Value;
		}
	}

	// Token: 0x06004585 RID: 17797 RVA: 0x0012EFFC File Offset: 0x0012D1FC
	public static VibrationData Create(LowFidelityVibrations lowFidelityVibration = LowFidelityVibrations.None, TextAsset highFidelityVibration = null, GamepadVibration gamepadVibration = null, PS5VibrationData ps5Vibration = null)
	{
		return new VibrationData
		{
			lowFidelityVibration = lowFidelityVibration,
			highFidelityVibration = highFidelityVibration,
			gamepadVibration = gamepadVibration,
			ps5Vibration = ps5Vibration
		};
	}

	// Token: 0x170007CE RID: 1998
	// (get) Token: 0x06004586 RID: 17798 RVA: 0x0012F032 File Offset: 0x0012D232
	public AudioClip PS5Vibration
	{
		get
		{
			return this.GetPS5Vibration();
		}
	}

	// Token: 0x170007CF RID: 1999
	// (get) Token: 0x06004587 RID: 17799 RVA: 0x0012F03A File Offset: 0x0012D23A
	public PS5VibrationData PS5VibrationAsset
	{
		get
		{
			return this.ps5Vibration;
		}
	}

	// Token: 0x06004588 RID: 17800 RVA: 0x0012F042 File Offset: 0x0012D242
	public AudioClip GetPS5Vibration()
	{
		if (this.ps5Vibration)
		{
			return this.ps5Vibration;
		}
		return null;
	}

	// Token: 0x06004589 RID: 17801 RVA: 0x0012F05E File Offset: 0x0012D25E
	public void SetVibrationData(PS5VibrationData ps5VibrationData)
	{
		this.ps5Vibration = ps5VibrationData;
	}

	// Token: 0x04004628 RID: 17960
	[SerializeField]
	private LowFidelityVibrations lowFidelityVibration;

	// Token: 0x04004629 RID: 17961
	[SerializeField]
	private TextAsset highFidelityVibration;

	// Token: 0x0400462A RID: 17962
	[SerializeField]
	private GamepadVibration gamepadVibration;

	// Token: 0x0400462B RID: 17963
	[SerializeField]
	private PS5VibrationData ps5Vibration;

	// Token: 0x0400462C RID: 17964
	[SerializeField]
	private OverrideFloat strength;
}
