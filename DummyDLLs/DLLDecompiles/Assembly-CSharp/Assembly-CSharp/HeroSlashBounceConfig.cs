using System;
using UnityEngine;

// Token: 0x020003BE RID: 958
[CreateAssetMenu(menuName = "Profiles/Hero Slash Bounce")]
public class HeroSlashBounceConfig : ScriptableObject
{
	// Token: 0x17000357 RID: 855
	// (get) Token: 0x0600203E RID: 8254 RVA: 0x00093157 File Offset: 0x00091357
	public int JumpSteps
	{
		get
		{
			return this.jumpSteps;
		}
	}

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x0600203F RID: 8255 RVA: 0x0009315F File Offset: 0x0009135F
	public int JumpedSteps
	{
		get
		{
			return this.jumpedSteps;
		}
	}

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x06002040 RID: 8256 RVA: 0x00093167 File Offset: 0x00091367
	public bool HideSlashOnBounceCancel
	{
		get
		{
			return this.hideSlashOnBounceCancel;
		}
	}

	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06002041 RID: 8257 RVA: 0x0009316F File Offset: 0x0009136F
	public static HeroSlashBounceConfig Default
	{
		get
		{
			if (!HeroSlashBounceConfig._default)
			{
				HeroSlashBounceConfig._default = ScriptableObject.CreateInstance<HeroSlashBounceConfig>();
			}
			return HeroSlashBounceConfig._default;
		}
	}

	// Token: 0x04001F3D RID: 7997
	[SerializeField]
	private int jumpSteps = 4;

	// Token: 0x04001F3E RID: 7998
	[SerializeField]
	private int jumpedSteps = -20;

	// Token: 0x04001F3F RID: 7999
	[SerializeField]
	private bool hideSlashOnBounceCancel = true;

	// Token: 0x04001F40 RID: 8000
	private static HeroSlashBounceConfig _default;
}
