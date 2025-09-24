using System;
using System.Collections;
using HutongGames;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000573 RID: 1395
public class TickedPendulum : MonoBehaviour
{
	// Token: 0x060031E8 RID: 12776 RVA: 0x000DDD50 File Offset: 0x000DBF50
	private void Awake()
	{
		PlayMakerFSM component = base.GetComponent<PlayMakerFSM>();
		if (component)
		{
			FsmBool fsmBool = component.FsmVariables.FindFsmBool("Dont Play Sound");
			if (fsmBool != null)
			{
				this.playSound = !fsmBool.Value;
				Object.Destroy(component);
			}
		}
		this.tickRegister.ReceivedEvent += this.Ticked;
		this.angleStart = base.transform.eulerAngles.z;
		this.angleEnd = Helper.GetReflectedAngle(this.angleStart, true, false, true);
		this.initialParentX = this.spikeRotate.parent.position.x;
		this.UpdateSpikeRotation();
		this.ResetPosition();
	}

	// Token: 0x060031E9 RID: 12777 RVA: 0x000DDDFD File Offset: 0x000DBFFD
	private void Start()
	{
		this.mainCamera = GameCameras.instance.mainCamera.transform;
		this.started = true;
		if (this.tickState == TickedPendulum.TickState.OnStart)
		{
			this.AutoTick();
		}
		this.StartTick();
	}

	// Token: 0x060031EA RID: 12778 RVA: 0x000DDE30 File Offset: 0x000DC030
	private void OnEnable()
	{
		if (this.tickState == TickedPendulum.TickState.OnEnabled)
		{
			this.AutoTick();
		}
		if (this.started)
		{
			if (this.ticking)
			{
				this.tickQueued = true;
			}
			this.ResetPosition();
			this.StartTick();
		}
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x000DDE64 File Offset: 0x000DC064
	private void ResetPosition()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.z = this.angleStart;
		base.transform.eulerAngles = eulerAngles;
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x000DDE98 File Offset: 0x000DC098
	private void UpdateSpikeRotation()
	{
		float num = this.spikeRotate.position.x - this.initialParentX;
		num *= 2f;
		this.spikeRotate.SetLocalRotation2D(num);
	}

	// Token: 0x060031ED RID: 12781 RVA: 0x000DDED1 File Offset: 0x000DC0D1
	private void AutoTick()
	{
		if (this.ticking)
		{
			return;
		}
		this.Ticked();
		this.tickTime = Time.timeAsDouble + (double)(this.swingTime * 0.4f);
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x000DDEFB File Offset: 0x000DC0FB
	private void Ticked()
	{
		if (Time.timeAsDouble < this.tickTime)
		{
			return;
		}
		this.tickQueued = true;
	}

	// Token: 0x060031EF RID: 12783 RVA: 0x000DDF14 File Offset: 0x000DC114
	private void PlaySound(bool isIn)
	{
		if (!this.playSound)
		{
			return;
		}
		Vector3 position = this.mainCamera.position;
		float num = Vector3.Distance(position, base.transform.position);
		float num2 = Vector3.Distance(position, this.spikeRotate.position);
		if (num > 45f && num2 > 45f)
		{
			return;
		}
		(isIn ? this.inSound : this.outSound).SpawnAndPlayOneShot(this.audioPrefab, base.transform.position, false, 1f, null);
	}

	// Token: 0x060031F0 RID: 12784 RVA: 0x000DDF98 File Offset: 0x000DC198
	private void StartTick()
	{
		if (this.tickRoutine != null)
		{
			base.StopCoroutine(this.tickRoutine);
		}
		this.tickRoutine = base.StartCoroutine(this.TickRoutine());
	}

	// Token: 0x060031F1 RID: 12785 RVA: 0x000DDFC0 File Offset: 0x000DC1C0
	private IEnumerator TickRoutine()
	{
		for (;;)
		{
			if (this.tickQueued)
			{
				this.tickQueued = false;
				this.ticking = true;
				this.PlaySound(this.outSound);
				float startRotation = base.transform.eulerAngles.z;
				float elapsed = 0f;
				while (elapsed < this.swingTime && !this.tickQueued)
				{
					float rotation = EasingFunction.EaseInOutQuad(startRotation, this.angleEnd, elapsed / this.swingTime);
					base.transform.SetRotation2D(rotation);
					this.UpdateSpikeRotation();
					yield return null;
					elapsed += Time.deltaTime;
				}
				this.ticking = false;
				while (!this.tickQueued)
				{
					yield return null;
				}
				this.tickQueued = false;
				this.ticking = true;
				this.PlaySound(this.outSound);
				elapsed = base.transform.eulerAngles.z;
				startRotation = 0f;
				while (startRotation < this.swingTime && !this.tickQueued)
				{
					float rotation2 = EasingFunction.EaseInOutQuad(elapsed, this.angleStart, startRotation / this.swingTime);
					base.transform.SetRotation2D(rotation2);
					this.UpdateSpikeRotation();
					yield return null;
					startRotation += Time.deltaTime;
				}
				this.ticking = false;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x04003575 RID: 13685
	[SerializeField]
	private EventRegister tickRegister;

	// Token: 0x04003576 RID: 13686
	[SerializeField]
	private float swingTime = 1f;

	// Token: 0x04003577 RID: 13687
	[SerializeField]
	private Transform spikeRotate;

	// Token: 0x04003578 RID: 13688
	[SerializeField]
	private AudioSource audioPrefab;

	// Token: 0x04003579 RID: 13689
	[SerializeField]
	private RandomAudioClipTable outSound;

	// Token: 0x0400357A RID: 13690
	[SerializeField]
	private RandomAudioClipTable inSound;

	// Token: 0x0400357B RID: 13691
	[Space]
	[SerializeField]
	private bool playSound = true;

	// Token: 0x0400357C RID: 13692
	[Space]
	[SerializeField]
	private TickedPendulum.TickState tickState;

	// Token: 0x0400357D RID: 13693
	private bool ticking;

	// Token: 0x0400357E RID: 13694
	private bool tickQueued;

	// Token: 0x0400357F RID: 13695
	private float angleStart;

	// Token: 0x04003580 RID: 13696
	private float angleEnd;

	// Token: 0x04003581 RID: 13697
	private float initialParentX;

	// Token: 0x04003582 RID: 13698
	private double tickTime;

	// Token: 0x04003583 RID: 13699
	private Transform mainCamera;

	// Token: 0x04003584 RID: 13700
	private bool started;

	// Token: 0x04003585 RID: 13701
	private Coroutine tickRoutine;

	// Token: 0x0200187A RID: 6266
	private enum TickState
	{
		// Token: 0x0400922D RID: 37421
		None,
		// Token: 0x0400922E RID: 37422
		OnStart,
		// Token: 0x0400922F RID: 37423
		OnEnabled
	}
}
