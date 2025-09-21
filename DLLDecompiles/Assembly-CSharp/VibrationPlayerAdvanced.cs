using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007B7 RID: 1975
public sealed class VibrationPlayerAdvanced : MonoBehaviour
{
	// Token: 0x170007D6 RID: 2006
	// (get) Token: 0x060045AB RID: 17835 RVA: 0x0012F49B File Offset: 0x0012D69B
	// (set) Token: 0x060045AC RID: 17836 RVA: 0x0012F4A3 File Offset: 0x0012D6A3
	public bool IsLooping
	{
		get
		{
			return this.isLooping;
		}
		set
		{
			this.isLooping = value;
			if (this.emission != null)
			{
				this.emission.IsLooping = this.isLooping;
			}
		}
	}

	// Token: 0x170007D7 RID: 2007
	// (get) Token: 0x060045AD RID: 17837 RVA: 0x0012F4C5 File Offset: 0x0012D6C5
	// (set) Token: 0x060045AE RID: 17838 RVA: 0x0012F4CD File Offset: 0x0012D6CD
	public string VibrationTag
	{
		get
		{
			return this.vibrationTag;
		}
		set
		{
			this.vibrationTag = value;
			if (this.emission != null)
			{
				this.emission.Tag = this.vibrationTag;
			}
		}
	}

	// Token: 0x060045AF RID: 17839 RVA: 0x0012F4EF File Offset: 0x0012D6EF
	private void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x060045B0 RID: 17840 RVA: 0x0012F4FF File Offset: 0x0012D6FF
	private void OnDisable()
	{
		if (this.stopOnDisable)
		{
			this.Stop();
			return;
		}
		if (this.isLooping && this.emission != null)
		{
			this.emission.IsLooping = false;
			this.emission = null;
		}
	}

	// Token: 0x060045B1 RID: 17841 RVA: 0x0012F534 File Offset: 0x0012D734
	public void Play()
	{
		if (this.emission != null)
		{
			this.emission.Stop();
		}
		VibrationData vibrationData = this.vibrationDataAsset;
		bool flag = this.isLooping;
		string tag = base.tag;
		bool flag2 = this.isRealtime;
		this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, flag, tag, flag2);
		if (this.doTimedStrength)
		{
			this.StartStrengthRoutine();
			return;
		}
		this.emission.SetStrength(this.strength);
	}

	// Token: 0x060045B2 RID: 17842 RVA: 0x0012F5AB File Offset: 0x0012D7AB
	public void Stop()
	{
		if (this.emission != null)
		{
			this.emission.Stop();
			this.emission = null;
		}
	}

	// Token: 0x060045B3 RID: 17843 RVA: 0x0012F5C8 File Offset: 0x0012D7C8
	private void StartStrengthRoutine()
	{
		if (this.coroutine != null)
		{
			base.StopCoroutine(this.coroutine);
			this.coroutine = null;
		}
		VibrationEmission vibrationEmission = this.emission;
		if (vibrationEmission != null)
		{
			vibrationEmission.SetStrength(this.curve.Evaluate(0f) * this.strength);
		}
		this.coroutine = base.StartCoroutine(this.TimePlayRoutine());
	}

	// Token: 0x060045B4 RID: 17844 RVA: 0x0012F62A File Offset: 0x0012D82A
	private IEnumerator TimePlayRoutine()
	{
		if (this.duration > 0f)
		{
			float t = 0f;
			float inverse = 1f / this.duration;
			VibrationEmission vibrationEmission = this.emission;
			if (vibrationEmission != null)
			{
				vibrationEmission.SetStrength(this.curve.Evaluate(t) * this.strength);
			}
			while (t < 1f)
			{
				VibrationEmission vibrationEmission2 = this.emission;
				if (vibrationEmission2 == null || !vibrationEmission2.IsPlaying)
				{
					break;
				}
				yield return null;
				float num = this.isRealtime ? Time.unscaledDeltaTime : Time.deltaTime;
				t += inverse * num;
				VibrationEmission vibrationEmission3 = this.emission;
				if (vibrationEmission3 != null)
				{
					vibrationEmission3.SetStrength(this.curve.Evaluate(Mathf.Clamp01(t)) * this.strength);
				}
			}
		}
		VibrationEmission vibrationEmission4 = this.emission;
		if (vibrationEmission4 != null)
		{
			vibrationEmission4.SetStrength(this.curve.Evaluate(this.duration) * this.strength);
		}
		this.Stop();
		this.coroutine = null;
		yield break;
	}

	// Token: 0x04004646 RID: 17990
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x04004647 RID: 17991
	[Space]
	[SerializeField]
	private float strength = 1f;

	// Token: 0x04004648 RID: 17992
	[Space]
	[SerializeField]
	private bool playOnEnable;

	// Token: 0x04004649 RID: 17993
	[SerializeField]
	private bool stopOnDisable;

	// Token: 0x0400464A RID: 17994
	[Space]
	[SerializeField]
	private bool isLooping;

	// Token: 0x0400464B RID: 17995
	[SerializeField]
	private bool isRealtime;

	// Token: 0x0400464C RID: 17996
	[SerializeField]
	private string vibrationTag;

	// Token: 0x0400464D RID: 17997
	[Space]
	[SerializeField]
	private bool doTimedStrength;

	// Token: 0x0400464E RID: 17998
	[SerializeField]
	private float duration = 1f;

	// Token: 0x0400464F RID: 17999
	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04004650 RID: 18000
	private VibrationEmission emission;

	// Token: 0x04004651 RID: 18001
	private Coroutine coroutine;
}
