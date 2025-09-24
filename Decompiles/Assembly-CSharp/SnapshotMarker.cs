using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000133 RID: 307
public abstract class SnapshotMarker : MonoBehaviour
{
	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000981 RID: 2433 RVA: 0x0002BB13 File Offset: 0x00029D13
	public AudioMixerSnapshot Snapshot
	{
		get
		{
			return this.snapshot;
		}
	}

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000982 RID: 2434 RVA: 0x0002BB1B File Offset: 0x00029D1B
	public float TransitionTime
	{
		get
		{
			return this.transitionTime;
		}
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x0002BB24 File Offset: 0x00029D24
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(position, this.maxIntensityRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(position, this.cutoffRadius);
		if (!Application.isPlaying)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(position, Mathf.Lerp(this.cutoffRadius, this.maxIntensityRadius, this.GetBlendAmountRaw()));
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(position, Mathf.Lerp(this.cutoffRadius, this.maxIntensityRadius, this.GetBlendAmount()));
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x0002BBBF File Offset: 0x00029DBF
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.AddMarker();
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x0002BBD0 File Offset: 0x00029DD0
	private void Start()
	{
		this.hasStarted = true;
		this.AddMarker();
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x0002BBDF File Offset: 0x00029DDF
	private void OnDisable()
	{
		this.RemoveMarker();
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x0002BBE8 File Offset: 0x00029DE8
	public float GetBlendAmount()
	{
		float blendAmountRaw = this.GetBlendAmountRaw();
		return this.blendCurve.Evaluate(blendAmountRaw);
	}

	// Token: 0x06000988 RID: 2440 RVA: 0x0002BC08 File Offset: 0x00029E08
	private float GetBlendAmountRaw()
	{
		Vector2 vector = GameCameras.instance.mainCamera.transform.position - base.transform.position;
		float num = this.maxIntensityRadius * this.maxIntensityRadius;
		float num2 = this.cutoffRadius * this.cutoffRadius - num;
		float num3 = (vector.sqrMagnitude - num) / num2;
		return Mathf.Clamp01(1f - num3);
	}

	// Token: 0x06000989 RID: 2441
	protected abstract void AddMarker();

	// Token: 0x0600098A RID: 2442
	protected abstract void RemoveMarker();

	// Token: 0x0400092A RID: 2346
	[SerializeField]
	private float maxIntensityRadius;

	// Token: 0x0400092B RID: 2347
	[SerializeField]
	private float cutoffRadius;

	// Token: 0x0400092C RID: 2348
	[SerializeField]
	private AnimationCurve blendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400092D RID: 2349
	[Space]
	[SerializeField]
	private AudioMixerSnapshot snapshot;

	// Token: 0x0400092E RID: 2350
	[SerializeField]
	private float transitionTime = 0.2f;

	// Token: 0x0400092F RID: 2351
	private bool hasStarted;
}
