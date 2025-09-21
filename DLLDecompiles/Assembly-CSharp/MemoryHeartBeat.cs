using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000251 RID: 593
public class MemoryHeartBeat : MonoBehaviour
{
	// Token: 0x1700024D RID: 589
	// (get) Token: 0x0600157E RID: 5502 RVA: 0x000615DB File Offset: 0x0005F7DB
	// (set) Token: 0x0600157F RID: 5503 RVA: 0x000615E3 File Offset: 0x0005F7E3
	public float Multiplier { get; set; }

	// Token: 0x06001580 RID: 5504 RVA: 0x000615EC File Offset: 0x0005F7EC
	private void OnValidate()
	{
		if (this.fovOffsetDuration > this.beatDelay.Start)
		{
			this.fovOffsetDuration = this.beatDelay.Start;
		}
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x00061614 File Offset: 0x0005F814
	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HEARTBEAT_SCENE_START").ReceivedEvent += delegate()
		{
			this.isInSpecialScene = true;
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HEARTBEAT_SCENE_END").ReceivedEvent += delegate()
		{
			this.isInSpecialScene = false;
		};
		this.beatEventId = EventRegister.GetEventHashCode(this.beatEvent);
		this.Multiplier = 1f;
		this.volume = this.audioSource.volume;
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x00061690 File Offset: 0x0005F890
	private void OnEnable()
	{
		this.beatRoutine = base.StartCoroutine(this.BeatRoutine());
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x000616A4 File Offset: 0x0005F8A4
	private void OnDisable()
	{
		base.StopCoroutine(this.beatRoutine);
		this.beatRoutine = null;
		GameCameras silentInstance = GameCameras.SilentInstance;
		if (silentInstance && silentInstance.forceCameraAspect)
		{
			silentInstance.forceCameraAspect.SetExtraFovOffset(0f);
		}
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000616EF File Offset: 0x0005F8EF
	private IEnumerator BeatRoutine()
	{
		for (;;)
		{
			yield return new WaitForSeconds(this.beatDelay.GetRandomValue());
			this.lowPassFilter.cutoffFrequency = (this.isInSpecialScene ? this.lowPassSceneCutoff : this.lowPassRegularCutoff);
			this.audioSource.volume = 1f;
			this.beatTable.PlayOneShot(this.audioSource, false);
			this.audioSource.volume *= this.volume * this.Multiplier;
			if (Mathf.Abs(this.fovOffset) > Mathf.Epsilon)
			{
				if (this.fovRoutine != null)
				{
					base.StopCoroutine(this.fovRoutine);
				}
				this.fovRoutine = base.StartCoroutine(this.TransitionFovOffset());
			}
			this.enableOnBeat.SetAllActive(true);
			EventRegister.SendEvent(this.beatEventId, null);
		}
		yield break;
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x000616FE File Offset: 0x0005F8FE
	private IEnumerator TransitionFovOffset()
	{
		ForceCameraAspect cam = GameCameras.instance.forceCameraAspect;
		for (float elapsed = 0f; elapsed < this.fovOffsetDuration; elapsed += Time.deltaTime)
		{
			float num = this.fovOffsetCurve.Evaluate(elapsed / this.fovOffsetDuration);
			cam.SetExtraFovOffset(this.fovOffset * num * this.Multiplier);
			yield return null;
		}
		cam.SetExtraFovOffset(this.fovOffset * this.fovOffsetCurve.Evaluate(1f) * this.Multiplier);
		this.fovRoutine = null;
		yield break;
	}

	// Token: 0x0400141F RID: 5151
	[SerializeField]
	private MinMaxFloat beatDelay;

	// Token: 0x04001420 RID: 5152
	[SerializeField]
	private RandomAudioClipTable beatTable;

	// Token: 0x04001421 RID: 5153
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001422 RID: 5154
	[Space]
	[SerializeField]
	private AudioLowPassFilter lowPassFilter;

	// Token: 0x04001423 RID: 5155
	[SerializeField]
	private float lowPassRegularCutoff;

	// Token: 0x04001424 RID: 5156
	[SerializeField]
	private float lowPassSceneCutoff;

	// Token: 0x04001425 RID: 5157
	[Space]
	[SerializeField]
	private float fovOffset;

	// Token: 0x04001426 RID: 5158
	[SerializeField]
	private AnimationCurve fovOffsetCurve;

	// Token: 0x04001427 RID: 5159
	[SerializeField]
	private float fovOffsetDuration;

	// Token: 0x04001428 RID: 5160
	[Space]
	[SerializeField]
	private GameObject[] enableOnBeat;

	// Token: 0x04001429 RID: 5161
	[Space]
	[SerializeField]
	private string beatEvent;

	// Token: 0x0400142A RID: 5162
	private int beatEventId;

	// Token: 0x0400142B RID: 5163
	private Coroutine beatRoutine;

	// Token: 0x0400142C RID: 5164
	private bool isInSpecialScene;

	// Token: 0x0400142D RID: 5165
	private Coroutine fovRoutine;

	// Token: 0x0400142E RID: 5166
	private float volume;
}
