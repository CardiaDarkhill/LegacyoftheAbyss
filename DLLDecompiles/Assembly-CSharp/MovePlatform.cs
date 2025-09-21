using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200051E RID: 1310
public class MovePlatform : UnlockablePropBase
{
	// Token: 0x06002F21 RID: 12065 RVA: 0x000CFFD4 File Offset: 0x000CE1D4
	private void Reset()
	{
		Vector2 a = base.transform.position;
		this.inactivePos = a;
		this.activePos = a + Vector2.down;
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x000D000C File Offset: 0x000CE20C
	private void OnDrawGizmosSelected()
	{
		float z = base.transform.position.z;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.inactivePos.ToVector3(z), 0.2f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.activePos.ToVector3(z), 0.2f);
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x000D006C File Offset: 0x000CE26C
	private void Start()
	{
		if (this.gearParent)
		{
			this.gears = this.gearParent.GetComponentsInChildren<LoopRotator>();
			this.gearParent.SetParent(null, true);
		}
		else
		{
			this.gears = new LoopRotator[0];
		}
		this.activateWhileMoving.SetAllActive(false);
		this.activateOnMove.SetAllActive(false);
		this.SetCanSway(false);
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x000D00D4 File Offset: 0x000CE2D4
	private void SetCanSway(bool value)
	{
		if (!this.swayPlatFsm)
		{
			return;
		}
		bool flag = !value;
		this.swayPlatFsm.FsmVariables.FindFsmBool("Dont Move").Value = flag;
		if (flag)
		{
			this.swayPlatFsm.SendEvent("STOP MOVE");
		}
	}

	// Token: 0x06002F25 RID: 12069 RVA: 0x000D0122 File Offset: 0x000CE322
	public override void Open()
	{
		if (this.activated)
		{
			return;
		}
		this.activated = true;
		this.moveRoutines = new Stack<Coroutine>(4);
		this.moveRoutines.Push(base.StartCoroutine(this.Move()));
	}

	// Token: 0x06002F26 RID: 12070 RVA: 0x000D0158 File Offset: 0x000CE358
	public override void Opened()
	{
		this.activated = true;
		if (this.moveRoutines != null)
		{
			while (this.moveRoutines.Count > 0)
			{
				base.StopCoroutine(this.moveRoutines.Pop());
			}
		}
		base.transform.SetPosition2D(this.activePos);
		LoopRotator[] array = this.gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		this.SetCanSway(true);
	}

	// Token: 0x06002F27 RID: 12071 RVA: 0x000D01CB File Offset: 0x000CE3CB
	private IEnumerator Move()
	{
		this.activateOnMove.SetAllActive(true);
		this.activateWhileMoving.SetAllActive(true);
		LoopRotator[] array = this.gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartRotation();
		}
		if (this.moveAudioSource)
		{
			this.moveAudioSource.Play();
			if (this.moveStartClip)
			{
				this.moveAudioSource.PlayOneShot(this.moveStartClip);
			}
		}
		if (this.beforeMoveDuration > 0f)
		{
			Vector2 initialPos = base.transform.position;
			Vector2 targetPos = initialPos + this.beforeMoveOffset;
			yield return this.moveRoutines.PushReturn(this.StartTimerRoutine(0f, this.beforeMoveDuration, delegate(float time)
			{
				this.transform.SetPosition2D(Vector2.LerpUnclamped(initialPos, targetPos, this.beforeMoveCurve.Evaluate(time)));
			}, null, null, false));
		}
		if (this.moveSpeed > 0f)
		{
			Vector2 initialPos = base.transform.position;
			float duration = (this.activePos - this.inactivePos).magnitude / this.moveSpeed;
			yield return this.moveRoutines.PushReturn(this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				this.transform.SetPosition2D(Vector2.LerpUnclamped(initialPos, this.activePos, time));
			}, null, null, false));
		}
		array = this.gears;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopRotation();
		}
		if (this.moveAudioSource)
		{
			this.moveAudioSource.Stop();
			if (this.moveEndClip)
			{
				this.moveAudioSource.PlayOneShot(this.moveEndClip);
			}
		}
		if (this.afterMoveDuration > 0f)
		{
			Vector2 initialPos = base.transform.position;
			Vector2 targetPos = initialPos + this.afterMoveOffset;
			yield return this.moveRoutines.PushReturn(this.StartTimerRoutine(0f, this.afterMoveDuration, delegate(float time)
			{
				this.transform.SetPosition2D(Vector2.LerpUnclamped(initialPos, targetPos, this.afterMoveCurve.Evaluate(time)));
			}, null, null, false));
		}
		this.activateWhileMoving.SetAllActive(false);
		this.SetCanSway(true);
		yield break;
	}

	// Token: 0x040031D7 RID: 12759
	[SerializeField]
	private Vector2 beforeMoveOffset;

	// Token: 0x040031D8 RID: 12760
	[SerializeField]
	private AnimationCurve beforeMoveCurve;

	// Token: 0x040031D9 RID: 12761
	[SerializeField]
	private float beforeMoveDuration;

	// Token: 0x040031DA RID: 12762
	[Space]
	[SerializeField]
	private Vector2 inactivePos;

	// Token: 0x040031DB RID: 12763
	[SerializeField]
	private Vector2 activePos;

	// Token: 0x040031DC RID: 12764
	[SerializeField]
	private float moveSpeed = 10f;

	// Token: 0x040031DD RID: 12765
	[Space]
	[SerializeField]
	private Vector2 afterMoveOffset;

	// Token: 0x040031DE RID: 12766
	[SerializeField]
	private AnimationCurve afterMoveCurve;

	// Token: 0x040031DF RID: 12767
	[SerializeField]
	private float afterMoveDuration;

	// Token: 0x040031E0 RID: 12768
	[Space]
	[SerializeField]
	private GameObject[] activateWhileMoving;

	// Token: 0x040031E1 RID: 12769
	[SerializeField]
	private GameObject[] activateOnMove;

	// Token: 0x040031E2 RID: 12770
	[SerializeField]
	private Transform gearParent;

	// Token: 0x040031E3 RID: 12771
	[SerializeField]
	private PlayMakerFSM swayPlatFsm;

	// Token: 0x040031E4 RID: 12772
	[Space]
	[SerializeField]
	private AudioClip moveStartClip;

	// Token: 0x040031E5 RID: 12773
	[SerializeField]
	private AudioSource moveAudioSource;

	// Token: 0x040031E6 RID: 12774
	[SerializeField]
	private AudioClip moveEndClip;

	// Token: 0x040031E7 RID: 12775
	private bool activated;

	// Token: 0x040031E8 RID: 12776
	private LoopRotator[] gears;

	// Token: 0x040031E9 RID: 12777
	private Stack<Coroutine> moveRoutines;
}
