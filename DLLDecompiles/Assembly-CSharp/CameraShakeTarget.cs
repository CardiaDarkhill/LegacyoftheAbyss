using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000158 RID: 344
[Serializable]
public class CameraShakeTarget
{
	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000A77 RID: 2679 RVA: 0x0002EF65 File Offset: 0x0002D165
	public CameraManagerReference Camera
	{
		get
		{
			return this.camera;
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0002EF70 File Offset: 0x0002D170
	public void DoShake(Object source, bool shouldFreeze = true)
	{
		if (this.cached)
		{
			if (this.isValid)
			{
				this.camera.DoShake(this.profile, source, shouldFreeze && this.doFreeze, this.vibrate, true);
			}
			return;
		}
		if (this.camera && this.profile)
		{
			this.camera.DoShake(this.profile, source, shouldFreeze && this.doFreeze, this.vibrate, true);
		}
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0002EFF4 File Offset: 0x0002D1F4
	public bool TryShake(Object source, bool shouldFreeze = true)
	{
		if (this.cached)
		{
			if (this.isValid)
			{
				this.DoShake(source, shouldFreeze);
				return true;
			}
			return false;
		}
		else
		{
			if (!this.camera || !this.profile)
			{
				return false;
			}
			this.DoShake(source, shouldFreeze);
			return true;
		}
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x0002F044 File Offset: 0x0002D244
	public void DoShakeInRange(Object source, Vector2 range, Vector2 sourcePos, bool shouldFreeze = true)
	{
		if (this.camera && this.profile)
		{
			this.camera.DoShakeInRange(this.profile, source, range, sourcePos, shouldFreeze && this.doFreeze, this.vibrate);
		}
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0002F094 File Offset: 0x0002D294
	public void CancelShake()
	{
		if (this.cached)
		{
			if (this.isValid)
			{
				this.camera.CancelShake(this.profile);
			}
			return;
		}
		if (this.camera && this.profile)
		{
			this.camera.CancelShake(this.profile);
		}
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0002F0EE File Offset: 0x0002D2EE
	public CameraShakeTarget Duplicate()
	{
		return (CameraShakeTarget)base.MemberwiseClone();
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0002F0FB File Offset: 0x0002D2FB
	public void Cache()
	{
		this.cached = true;
		this.isValid = (this.camera && this.profile);
	}

	// Token: 0x040009F2 RID: 2546
	[SerializeField]
	[FormerlySerializedAs("Camera")]
	private CameraManagerReference camera;

	// Token: 0x040009F3 RID: 2547
	[SerializeField]
	[FormerlySerializedAs("Profile")]
	private CameraShakeProfile profile;

	// Token: 0x040009F4 RID: 2548
	[SerializeField]
	private bool doFreeze;

	// Token: 0x040009F5 RID: 2549
	[SerializeField]
	private bool vibrate = true;

	// Token: 0x040009F6 RID: 2550
	private bool cached;

	// Token: 0x040009F7 RID: 2551
	private bool isValid;
}
