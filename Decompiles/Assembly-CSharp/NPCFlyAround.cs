using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000527 RID: 1319
public class NPCFlyAround : MonoBehaviour
{
	// Token: 0x14000097 RID: 151
	// (add) Token: 0x06002F66 RID: 12134 RVA: 0x000D0D88 File Offset: 0x000CEF88
	// (remove) Token: 0x06002F67 RID: 12135 RVA: 0x000D0DC0 File Offset: 0x000CEFC0
	public event Action ArrivedAtPoint;

	// Token: 0x06002F68 RID: 12136 RVA: 0x000D0DF8 File Offset: 0x000CEFF8
	private void OnDrawGizmosSelected()
	{
		Vector2 vector = this.maxPointsPos - this.minPointsPos;
		Gizmos.DrawWireCube(this.minPointsPos + vector / 2f, vector);
	}

	// Token: 0x06002F69 RID: 12137 RVA: 0x000D0E3D File Offset: 0x000CF03D
	private bool? ValidateAnimName(string animName)
	{
		return this.animator.IsAnimValid(animName, false);
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x000D0E4C File Offset: 0x000CF04C
	private void Awake()
	{
		if (this.talkFlyPointsParent)
		{
			this.talkingFlyPoints = new List<Transform>(this.talkFlyPointsParent.childCount);
			for (int i = 0; i < this.talkFlyPointsParent.childCount; i++)
			{
				this.talkingFlyPoints.Add(this.talkFlyPointsParent.GetChild(i));
			}
			return;
		}
		this.talkingFlyPoints = new List<Transform>(0);
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x000D0EB6 File Offset: 0x000CF0B6
	private float PlayAnim(string animName)
	{
		if (!this.animator)
		{
			return 0f;
		}
		if (string.IsNullOrEmpty(animName))
		{
			return 0f;
		}
		return this.animator.PlayAnimGetTime(animName);
	}

	// Token: 0x06002F6C RID: 12140 RVA: 0x000D0EE8 File Offset: 0x000CF0E8
	private void StopFlyingRoutines()
	{
		foreach (Coroutine coroutine in this.flyRoutines)
		{
			if (coroutine != null)
			{
				base.StopCoroutine(coroutine);
			}
		}
		this.flyRoutines.Clear();
		this.flyingAnimState = NPCFlyAround.FlyingAnimStates.Inactive;
		this.nextFlyPointIndex = 0;
	}

	// Token: 0x06002F6D RID: 12141 RVA: 0x000D0F58 File Offset: 0x000CF158
	public void StartFlyToPoint(Vector2 point)
	{
		if (this.isFlyingAroundTalking)
		{
			return;
		}
		this.StopFlyingRoutines();
		this.flyRoutines.Add(base.StartCoroutine(this.FlyToPoint(point, null, true)));
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x000D0F83 File Offset: 0x000CF183
	private IEnumerator FlyToPoint(Vector2 targetPos, Func<bool> breakCondition = null, bool force = false)
	{
		bool wasLookAnimNpcActive = this.lookAnimNPC.State != LookAnimNPC.AnimState.Disabled;
		if (wasLookAnimNpcActive)
		{
			this.lookAnimNPC.Deactivate();
			this.reActivateLookAnimNpc = true;
		}
		this.shouldStopFlyingAnimRoutine = false;
		Coroutine animRoutine = base.StartCoroutine(this.FlyingAnimations());
		this.flyRoutines.Add(animRoutine);
		Vector2 startPos = base.transform.position;
		float distance = Vector2.Distance(startPos, targetPos);
		bool shouldMove = force || distance > this.minFlyDistance;
		if (shouldMove)
		{
			float duration = distance / this.flySpeed;
			if (distance > 0f && this.flyAudioFSM)
			{
				this.flyAudioFSM.SendEvent("FLY START");
			}
			float elapsed = 0f;
			while (elapsed < duration && (breakCondition == null || !breakCondition()))
			{
				float t = this.flyCurve.Evaluate(elapsed / duration);
				Vector2 position = Vector2.LerpUnclamped(startPos, targetPos, t);
				base.transform.SetPosition2D(position);
				yield return null;
				elapsed += Time.deltaTime;
			}
		}
		if (this.flyAudioFSM)
		{
			this.flyAudioFSM.SendEvent("FLY STOP");
		}
		if (shouldMove)
		{
			base.transform.SetPosition2D(targetPos);
		}
		this.shouldStopFlyingAnimRoutine = true;
		while (this.flyingAnimState != NPCFlyAround.FlyingAnimStates.Inactive)
		{
			yield return null;
		}
		base.StopCoroutine(animRoutine);
		if (distance >= this.minFlyDistance || (force && distance > 0.5f))
		{
			yield return new WaitForSeconds(this.PlayAnim(this.flyEndAnim));
		}
		this.PlayTalkOrFly();
		if (wasLookAnimNpcActive)
		{
			this.lookAnimNPC.Activate();
		}
		if (this.ArrivedAtPoint != null)
		{
			this.ArrivedAtPoint();
		}
		yield break;
	}

	// Token: 0x06002F6F RID: 12143 RVA: 0x000D0FA7 File Offset: 0x000CF1A7
	private IEnumerator FlyingAnimations()
	{
		bool wasFacingLeft = this.lookAnimNPC.WasFacingLeft;
		while (!this.shouldStopFlyingAnimRoutine)
		{
			yield return null;
			this.PlayTalkOrFly();
			this.flyingAnimState = NPCFlyAround.FlyingAnimStates.Flying;
			bool shouldFaceLeft = this.lookAnimNPC.ShouldFaceLeft();
			if (wasFacingLeft != shouldFaceLeft)
			{
				this.flyingAnimState = NPCFlyAround.FlyingAnimStates.Turning;
				this.lookAnimNPC.ForceShouldTurnChecking = true;
				this.lookAnimNPC.Activate();
				yield return null;
				for (;;)
				{
					LookAnimNPC.AnimState state = this.lookAnimNPC.State;
					if (state != LookAnimNPC.AnimState.TurningRight && state != LookAnimNPC.AnimState.TurningLeft)
					{
						break;
					}
					yield return null;
				}
				this.lookAnimNPC.ForceShouldTurnChecking = false;
				this.lookAnimNPC.Deactivate();
				wasFacingLeft = shouldFaceLeft;
			}
		}
		this.flyingAnimState = NPCFlyAround.FlyingAnimStates.Inactive;
		yield break;
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x000D0FB6 File Offset: 0x000CF1B6
	private void PlayTalkOrFly()
	{
		if (this.lookAnimNPC.IsNPCTalking)
		{
			this.PlayAnim(this.talkAnim);
			return;
		}
		this.PlayAnim(this.flyAnim);
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x000D0FE0 File Offset: 0x000CF1E0
	private IEnumerator FlyAroundTalking()
	{
		while (!this.lookAnimNPC.IsNPCTalking)
		{
			yield return null;
		}
		LookAnimNPC.AnimState state = this.lookAnimNPC.State;
		bool wasLookAnimNpcActive = state != LookAnimNPC.AnimState.Disabled;
		if (wasLookAnimNpcActive)
		{
			while (state == LookAnimNPC.AnimState.TurningLeft || state == LookAnimNPC.AnimState.TurningRight)
			{
				yield return null;
				state = this.lookAnimNPC.State;
			}
			this.lookAnimNPC.Deactivate();
		}
		if (wasLookAnimNpcActive)
		{
			this.reActivateLookAnimNpc = true;
		}
		bool wasNpcTalking = false;
		for (;;)
		{
			if (this.lookAnimNPC.IsNPCTalking)
			{
				if (!wasNpcTalking)
				{
					this.PlayAnim(this.talkAnim);
				}
			}
			else if (wasNpcTalking)
			{
				this.PlayAnim(this.flyAnim);
			}
			wasNpcTalking = this.lookAnimNPC.IsNPCTalking;
			int previousLineNumber = this.lookAnimNPC.CurrentLineNumber;
			if (this.talkingFlyPoints.Count > 0)
			{
				while (this.lookAnimNPC.CurrentLineNumber == previousLineNumber)
				{
					yield return null;
				}
			}
			else
			{
				while (this.lookAnimNPC.IsNPCTalking == wasNpcTalking)
				{
					yield return null;
				}
			}
			if (this.lookAnimNPC.IsNPCTalking && this.talkingFlyPoints.Count > 0)
			{
				Transform transform = this.talkingFlyPoints[this.nextFlyPointIndex];
				bool flag = true;
				for (int i = 0; i < this.talkingFlyPoints.Count; i++)
				{
					this.nextFlyPointIndex++;
					if (this.nextFlyPointIndex >= this.talkingFlyPoints.Count)
					{
						this.nextFlyPointIndex = 0;
					}
					transform = this.talkingFlyPoints[this.nextFlyPointIndex];
					Vector3 position = transform.position;
					if (position.x > this.minPointsPos.x && position.y > this.minPointsPos.y && position.x < this.maxPointsPos.x && position.y < this.maxPointsPos.y && flag)
					{
						flag = false;
						if (Vector2.Distance(position, base.transform.position) > this.minFlyDistance)
						{
							break;
						}
					}
				}
				Coroutine routine = base.StartCoroutine(this.FlyToPoint(transform.position, () => !this.lookAnimNPC.IsNPCTalking, false));
				this.flyRoutines.Add(routine);
				yield return routine;
				this.flyRoutines.Remove(routine);
				routine = null;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x000D0FEF File Offset: 0x000CF1EF
	public void EnableTalkingFlyAround()
	{
		if (this.isFlyingAroundTalking)
		{
			return;
		}
		if (!this.lookAnimNPC)
		{
			return;
		}
		this.isFlyingAroundTalking = true;
		this.StopFlyingRoutines();
		this.flyRoutines.Add(base.StartCoroutine(this.FlyAroundTalking()));
	}

	// Token: 0x06002F73 RID: 12147 RVA: 0x000D102C File Offset: 0x000CF22C
	public void DisableTalkingFlyAround()
	{
		if (!this.isFlyingAroundTalking)
		{
			return;
		}
		this.isFlyingAroundTalking = false;
		this.StopFlyingRoutines();
		if (this.reActivateLookAnimNpc)
		{
			this.lookAnimNPC.Activate();
			this.reActivateLookAnimNpc = false;
		}
	}

	// Token: 0x04003224 RID: 12836
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x04003225 RID: 12837
	[SerializeField]
	[InspectorValidation]
	private LookAnimNPC lookAnimNPC;

	// Token: 0x04003226 RID: 12838
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string flyAnim;

	// Token: 0x04003227 RID: 12839
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string turnAnim;

	// Token: 0x04003228 RID: 12840
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string flyEndAnim;

	// Token: 0x04003229 RID: 12841
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateAnimName")]
	private string talkAnim;

	// Token: 0x0400322A RID: 12842
	[Space]
	[SerializeField]
	private AnimationCurve flyCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400322B RID: 12843
	[SerializeField]
	private float flySpeed = 10f;

	// Token: 0x0400322C RID: 12844
	[Tooltip("Ignores new point if less than min distance away.")]
	[SerializeField]
	private float minFlyDistance = 1f;

	// Token: 0x0400322D RID: 12845
	[Space]
	[SerializeField]
	private Transform talkFlyPointsParent;

	// Token: 0x0400322E RID: 12846
	[SerializeField]
	private Vector2 minPointsPos = Vector2.negativeInfinity;

	// Token: 0x0400322F RID: 12847
	[SerializeField]
	private Vector2 maxPointsPos = Vector2.positiveInfinity;

	// Token: 0x04003230 RID: 12848
	[Space]
	[SerializeField]
	private PlayMakerFSM flyAudioFSM;

	// Token: 0x04003231 RID: 12849
	private int nextFlyPointIndex;

	// Token: 0x04003232 RID: 12850
	private List<Transform> talkingFlyPoints;

	// Token: 0x04003233 RID: 12851
	private readonly List<Coroutine> flyRoutines = new List<Coroutine>();

	// Token: 0x04003234 RID: 12852
	private bool isFlyingAroundTalking;

	// Token: 0x04003235 RID: 12853
	private bool reActivateLookAnimNpc;

	// Token: 0x04003236 RID: 12854
	private bool shouldStopFlyingAnimRoutine;

	// Token: 0x04003237 RID: 12855
	private NPCFlyAround.FlyingAnimStates flyingAnimState;

	// Token: 0x02001839 RID: 6201
	private enum FlyingAnimStates
	{
		// Token: 0x04009122 RID: 37154
		Inactive,
		// Token: 0x04009123 RID: 37155
		Flying,
		// Token: 0x04009124 RID: 37156
		Turning
	}
}
