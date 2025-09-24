using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class AmbientSway : MonoBehaviour
{
	// Token: 0x06000257 RID: 599 RVA: 0x0000E334 File Offset: 0x0000C534
	private void Start()
	{
		this.initialPosition = base.transform.localPosition;
		if (this.active)
		{
			this.timeOffset = Random.Range(-10f, 10f);
		}
		this.updateOffset = Random.Range(0f, 1f / this.profile.Fps);
		this.started = true;
		ComponentSingleton<AmbientSwayCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
	}

	// Token: 0x06000258 RID: 600 RVA: 0x0000E3AD File Offset: 0x0000C5AD
	private void OnEnable()
	{
		if (this.started)
		{
			ComponentSingleton<AmbientSwayCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		}
		if (!this.profile)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000259 RID: 601 RVA: 0x0000E3E1 File Offset: 0x0000C5E1
	private void OnDisable()
	{
		ComponentSingleton<AmbientSwayCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0000E3FC File Offset: 0x0000C5FC
	private void OnUpdate()
	{
		if (!this.active)
		{
			return;
		}
		this.time += Time.deltaTime;
		float num = this.time + this.updateOffset;
		if (this.profile.Fps > 0f)
		{
			if (num < this.nextUpdateTime)
			{
				return;
			}
			this.nextUpdateTime = num + 1f / this.profile.Fps;
		}
		base.transform.localPosition = this.initialPosition + this.profile.GetOffset(this.time, this.timeOffset);
	}

	// Token: 0x0600025B RID: 603 RVA: 0x0000E494 File Offset: 0x0000C694
	public void RecordInitialPosition()
	{
		this.initialPosition = base.transform.localPosition;
		this.time = 0f;
		this.nextUpdateTime = 0f;
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000E4BD File Offset: 0x0000C6BD
	public void StartSway()
	{
		this.initialPosition = base.transform.localPosition;
		this.ContinueSway();
	}

	// Token: 0x0600025D RID: 605 RVA: 0x0000E4D6 File Offset: 0x0000C6D6
	public void ContinueSway()
	{
		this.time = 0f;
		this.timeOffset = 0f;
		this.active = true;
		this.initialPosition = base.transform.localPosition;
		this.nextUpdateTime = 0f;
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0000E511 File Offset: 0x0000C711
	public void ResumeSway()
	{
		this.active = true;
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0000E51A File Offset: 0x0000C71A
	public void StopSway()
	{
		this.active = false;
	}

	// Token: 0x04000203 RID: 515
	[SerializeField]
	[AssetPickerDropdown]
	private AmbientSwayProfile profile;

	// Token: 0x04000204 RID: 516
	[SerializeField]
	private bool active = true;

	// Token: 0x04000205 RID: 517
	private float time;

	// Token: 0x04000206 RID: 518
	private float updateOffset;

	// Token: 0x04000207 RID: 519
	private float timeOffset;

	// Token: 0x04000208 RID: 520
	private float nextUpdateTime;

	// Token: 0x04000209 RID: 521
	private Vector3 initialPosition;

	// Token: 0x0400020A RID: 522
	private bool started;
}
