using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class UmbrellaWindObject : MonoBehaviour, ITrackTriggerObject
{
	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000789 RID: 1929 RVA: 0x00024A86 File Offset: 0x00022C86
	// (set) Token: 0x0600078A RID: 1930 RVA: 0x00024A8E File Offset: 0x00022C8E
	public bool IsActive { get; set; }

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x0600078B RID: 1931 RVA: 0x00024A97 File Offset: 0x00022C97
	// (set) Token: 0x0600078C RID: 1932 RVA: 0x00024A9F File Offset: 0x00022C9F
	public float SelfXSpeed { get; set; }

	// Token: 0x0600078D RID: 1933 RVA: 0x00024AA8 File Offset: 0x00022CA8
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.hc = base.GetComponent<HeroController>();
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00024AC4 File Offset: 0x00022CC4
	private void FixedUpdate()
	{
		if (!this.IsActive)
		{
			this.currentAddSpeed = 0f;
			return;
		}
		float num;
		float num2;
		if (this.hc && this.hc.cState.inUpdraft)
		{
			num = 0f;
			num2 = 10f;
		}
		else
		{
			num = 0f;
			foreach (UmbrellaWindRegion umbrellaWindRegion in this.insideRegions)
			{
				num += umbrellaWindRegion.SpeedX;
			}
			num2 = ((num > this.currentAddSpeed) ? 5f : 2f);
		}
		this.currentAddSpeed = ((Math.Abs(num - this.currentAddSpeed) < 0.001f) ? num : Mathf.Lerp(this.currentAddSpeed, num, Time.deltaTime * num2));
		Vector2 linearVelocity = this.body.linearVelocity;
		linearVelocity.x = this.SelfXSpeed + this.currentAddSpeed;
		this.body.linearVelocity = linearVelocity;
		if (this.hc)
		{
			this.hc.DoRecoilMovement();
		}
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x00024BEC File Offset: 0x00022DEC
	public void OnTrackTriggerEntered(TrackTriggerObjects enteredRange)
	{
		UmbrellaWindRegion umbrellaWindRegion = enteredRange as UmbrellaWindRegion;
		if (umbrellaWindRegion)
		{
			this.insideRegions.AddIfNotPresent(umbrellaWindRegion);
		}
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00024C18 File Offset: 0x00022E18
	public void OnTrackTriggerExited(TrackTriggerObjects exitedRange)
	{
		UmbrellaWindRegion umbrellaWindRegion = exitedRange as UmbrellaWindRegion;
		if (umbrellaWindRegion)
		{
			this.insideRegions.Remove(umbrellaWindRegion);
		}
	}

	// Token: 0x0400075B RID: 1883
	private const float LERP_SPEED_UP = 5f;

	// Token: 0x0400075C RID: 1884
	private const float LERP_SPEED_DOWN = 2f;

	// Token: 0x0400075D RID: 1885
	private const float LERP_SPEED_UPDRAFT = 10f;

	// Token: 0x0400075E RID: 1886
	private float currentAddSpeed;

	// Token: 0x0400075F RID: 1887
	private Rigidbody2D body;

	// Token: 0x04000760 RID: 1888
	private HeroController hc;

	// Token: 0x04000761 RID: 1889
	private readonly List<UmbrellaWindRegion> insideRegions = new List<UmbrellaWindRegion>();
}
