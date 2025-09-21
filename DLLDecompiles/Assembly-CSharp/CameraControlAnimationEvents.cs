using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class CameraControlAnimationEvents : MonoBehaviour
{
	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060002BE RID: 702 RVA: 0x0000F50A File Offset: 0x0000D70A
	private CameraManagerReference CurrentCamera
	{
		get
		{
			if (!this.overrideCamera)
			{
				return GlobalSettings.Camera.MainCameraShakeManager;
			}
			return this.overrideCamera;
		}
	}

	// Token: 0x060002BF RID: 703 RVA: 0x0000F525 File Offset: 0x0000D725
	private void Awake()
	{
		this.childRenderers = base.GetComponentsInChildren<Renderer>();
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0000F533 File Offset: 0x0000D733
	private void Start()
	{
		this.StopRumble();
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0000F53B File Offset: 0x0000D73B
	private void OnDisable()
	{
		this.StopRumble();
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x0000F543 File Offset: 0x0000D743
	public void BigShake()
	{
		this.DoShake(GlobalSettings.Camera.BigShake);
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x0000F551 File Offset: 0x0000D751
	public void BigShakeQuick()
	{
		this.DoShake(GlobalSettings.Camera.BigShakeQuick);
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0000F55F File Offset: 0x0000D75F
	public void TinyShake()
	{
		this.DoShake(GlobalSettings.Camera.TinyShake);
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0000F56D File Offset: 0x0000D76D
	public void SmallShake()
	{
		this.DoShake(GlobalSettings.Camera.SmallShake);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x0000F57B File Offset: 0x0000D77B
	public void AverageShake()
	{
		this.DoShake(GlobalSettings.Camera.AverageShake);
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0000F589 File Offset: 0x0000D789
	public void AverageShakeQuick()
	{
		this.DoShake(GlobalSettings.Camera.AverageShakeQuick);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x0000F597 File Offset: 0x0000D797
	public void EnemyKillShake()
	{
		this.DoShake(GlobalSettings.Camera.EnemyKillShake);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x0000F5A5 File Offset: 0x0000D7A5
	public void TinyRumble()
	{
		if (this.DoShake(GlobalSettings.Camera.TinyRumble))
		{
			this.TrackRumble(GlobalSettings.Camera.TinyRumble);
		}
	}

	// Token: 0x060002CA RID: 714 RVA: 0x0000F5BF File Offset: 0x0000D7BF
	public void SmallRumble()
	{
		if (this.DoShake(GlobalSettings.Camera.SmallRumble))
		{
			this.TrackRumble(GlobalSettings.Camera.SmallRumble);
		}
	}

	// Token: 0x060002CB RID: 715 RVA: 0x0000F5D9 File Offset: 0x0000D7D9
	public void MedRumble()
	{
		if (this.DoShake(GlobalSettings.Camera.MedRumble))
		{
			this.TrackRumble(GlobalSettings.Camera.MedRumble);
		}
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0000F5F3 File Offset: 0x0000D7F3
	public void BigRumble()
	{
		if (this.DoShake(GlobalSettings.Camera.BigRumble))
		{
			this.TrackRumble(GlobalSettings.Camera.BigRumble);
		}
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0000F610 File Offset: 0x0000D810
	public void StopRumble()
	{
		if (this.startedRumbles == null)
		{
			return;
		}
		foreach (CameraShakeProfile profile in this.startedRumbles)
		{
			this.CancelShake(profile);
		}
		this.startedRumbles.Clear();
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0000F678 File Offset: 0x0000D878
	private void TrackRumble(CameraShakeProfile profile)
	{
		if (this.startedRumbles == null)
		{
			this.startedRumbles = new List<CameraShakeProfile>();
		}
		this.startedRumbles.Add(profile);
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0000F69C File Offset: 0x0000D89C
	private bool DoShake(CameraShakeProfile profile)
	{
		if (!this.IsActive)
		{
			return false;
		}
		if (this.requireVisible)
		{
			bool flag = false;
			foreach (Renderer renderer in this.childRenderers)
			{
				if (renderer && renderer.isVisible)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (!base.enabled)
		{
			return false;
		}
		this.CurrentCamera.DoShake(profile, this, false, this.vibrate, this.sendWorldForce);
		return true;
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0000F714 File Offset: 0x0000D914
	private void CancelShake(ICameraShake profile)
	{
		this.CurrentCamera.CancelShake(profile);
	}

	// Token: 0x0400025E RID: 606
	public bool IsActive = true;

	// Token: 0x0400025F RID: 607
	[SerializeField]
	private bool requireVisible;

	// Token: 0x04000260 RID: 608
	[SerializeField]
	private bool vibrate = true;

	// Token: 0x04000261 RID: 609
	[SerializeField]
	private CameraManagerReference overrideCamera;

	// Token: 0x04000262 RID: 610
	[SerializeField]
	private bool sendWorldForce = true;

	// Token: 0x04000263 RID: 611
	private Renderer[] childRenderers;

	// Token: 0x04000264 RID: 612
	private List<CameraShakeProfile> startedRumbles;
}
