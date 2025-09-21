using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F17 RID: 3863
	[ActionCategory("iTween")]
	[Tooltip("Instantly rotates a GameObject to look at the supplied Vector3 then returns it to it's starting rotation over time.")]
	public class iTweenLookFrom : iTweenFsmAction
	{
		// Token: 0x06006BE6 RID: 27622 RVA: 0x00218BD4 File Offset: 0x00216DD4
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.transformTarget = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorTarget = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.delay = 0f;
			this.loopType = iTween.LoopType.none;
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.axis = iTweenFsmAction.AxisRestriction.none;
		}

		// Token: 0x06006BE7 RID: 27623 RVA: 0x00218C5D File Offset: 0x00216E5D
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006BE8 RID: 27624 RVA: 0x00218C80 File Offset: 0x00216E80
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006BE9 RID: 27625 RVA: 0x00218C90 File Offset: 0x00216E90
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = this.vectorTarget.IsNone ? Vector3.zero : this.vectorTarget.Value;
			if (!this.transformTarget.IsNone && this.transformTarget.Value)
			{
				vector = this.transformTarget.Value.transform.position + vector;
			}
			this.itweenType = "rotate";
			iTween.LookFrom(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"looktarget",
				vector,
				"name",
				this.id.IsNone ? "" : this.id.Value,
				this.speed.IsNone ? "time" : "speed",
				this.speed.IsNone ? (this.time.IsNone ? 1f : this.time.Value) : this.speed.Value,
				"delay",
				this.delay.IsNone ? 0f : this.delay.Value,
				"easetype",
				this.easeType,
				"looptype",
				this.loopType,
				"oncomplete",
				"iTweenOnComplete",
				"oncompleteparams",
				this.itweenID,
				"onstart",
				"iTweenOnStart",
				"onstartparams",
				this.itweenID,
				"ignoretimescale",
				!this.realTime.IsNone && this.realTime.Value,
				"axis",
				(this.axis == iTweenFsmAction.AxisRestriction.none) ? "" : Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis)
			}));
		}

		// Token: 0x04006B46 RID: 27462
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006B47 RID: 27463
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04006B48 RID: 27464
		[Tooltip("Look from a transform position.")]
		public FsmGameObject transformTarget;

		// Token: 0x04006B49 RID: 27465
		[Tooltip("A target position the GameObject will look at. If Transform Target is defined this is used as a local offset.")]
		public FsmVector3 vectorTarget;

		// Token: 0x04006B4A RID: 27466
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04006B4B RID: 27467
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x04006B4C RID: 27468
		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		// Token: 0x04006B4D RID: 27469
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x04006B4E RID: 27470
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		// Token: 0x04006B4F RID: 27471
		[Tooltip("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis;
	}
}
