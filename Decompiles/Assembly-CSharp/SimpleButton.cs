using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000551 RID: 1361
public class SimpleButton : MonoBehaviour
{
	// Token: 0x17000553 RID: 1363
	// (get) Token: 0x060030AA RID: 12458 RVA: 0x000D6E50 File Offset: 0x000D5050
	public bool IsDepressed
	{
		get
		{
			return this.isDepressed;
		}
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x000D6E58 File Offset: 0x000D5058
	private void Awake()
	{
		this.trigger.OnTriggerEntered += this.OnTriggerEntered;
		this.trigger.OnTriggerExited += this.OnTriggerExited;
		if (this.art)
		{
			this.initialArtPos = this.art.localPosition;
		}
		if (this.startLocked)
		{
			this.SetLocked(true);
		}
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x000D6EC5 File Offset: 0x000D50C5
	private void OnTriggerEntered(Collider2D other, GameObject sender)
	{
		this.SetDepressed(true);
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x000D6ECE File Offset: 0x000D50CE
	private void OnTriggerExited(Collider2D other, GameObject sender)
	{
		this.SetDepressed(false);
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x000D6ED8 File Offset: 0x000D50D8
	private void SetDepressed(bool value)
	{
		if (this.isDepressed == value)
		{
			return;
		}
		this.isDepressed = value;
		if (this.isLocked)
		{
			return;
		}
		this.playAudio = true;
		this.SetDepressedPosition(value);
		this.playAudio = false;
		if (this.DepressedChange != null)
		{
			this.DepressedChange(this.isDepressed);
		}
		if (this.isDepressed)
		{
			this.OnDepress.Invoke();
		}
	}

	// Token: 0x060030AF RID: 12463 RVA: 0x000D6F40 File Offset: 0x000D5140
	private void SetDepressedPosition(bool value)
	{
		if (this.animatePosRoutine != null)
		{
			base.StopCoroutine(this.animatePosRoutine);
			this.animatePosRoutine = null;
		}
		if (!this.art)
		{
			return;
		}
		this.animateStartPos = this.art.localPosition;
		this.animateEndPos = (value ? (this.initialArtPos + this.artOffset) : this.initialArtPos);
		if (!value)
		{
			if (this.playAudio)
			{
				this.riseAudio.SpawnAndPlayOneShot(base.transform.position, null);
			}
			this.animatePosRoutine = this.StartTimerRoutine(0f, this.riseDuration, delegate(float t)
			{
				Vector2 position = Vector2.Lerp(this.animateStartPos, this.animateEndPos, t);
				this.art.SetLocalPosition2D(position);
			}, null, null, false);
			return;
		}
		this.art.SetLocalPosition2D(this.animateEndPos);
	}

	// Token: 0x060030B0 RID: 12464 RVA: 0x000D7009 File Offset: 0x000D5209
	public void SetLocked(bool value)
	{
		this.isLocked = value;
		this.SetDepressedPosition(this.isLocked || this.isDepressed);
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x000D7029 File Offset: 0x000D5229
	public void UnlockActivate()
	{
		this.SetLocked(false);
		if (!this.isDepressed && this.DepressedChange != null)
		{
			this.DepressedChange(this.isDepressed);
		}
	}

	// Token: 0x040033B0 RID: 13232
	public Action<bool> DepressedChange;

	// Token: 0x040033B1 RID: 13233
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private TriggerEnterEvent trigger;

	// Token: 0x040033B2 RID: 13234
	[SerializeField]
	private Transform art;

	// Token: 0x040033B3 RID: 13235
	[SerializeField]
	private Vector2 artOffset;

	// Token: 0x040033B4 RID: 13236
	[SerializeField]
	private float riseDuration;

	// Token: 0x040033B5 RID: 13237
	[SerializeField]
	private AudioEvent riseAudio;

	// Token: 0x040033B6 RID: 13238
	[SerializeField]
	private bool startLocked;

	// Token: 0x040033B7 RID: 13239
	[Space]
	public UnityEvent OnDepress;

	// Token: 0x040033B8 RID: 13240
	private Vector2 initialArtPos;

	// Token: 0x040033B9 RID: 13241
	private bool isDepressed;

	// Token: 0x040033BA RID: 13242
	private bool isLocked;

	// Token: 0x040033BB RID: 13243
	private Vector2 animateStartPos;

	// Token: 0x040033BC RID: 13244
	private Vector2 animateEndPos;

	// Token: 0x040033BD RID: 13245
	private Coroutine animatePosRoutine;

	// Token: 0x040033BE RID: 13246
	private bool playAudio;
}
