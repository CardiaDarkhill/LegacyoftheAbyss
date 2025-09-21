using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F2B RID: 3883
	[ActionCategory("iTween")]
	[Tooltip("Instantly changes a GameObject's scale then returns it to it's starting scale over time.")]
	public class iTweenScaleFrom : iTweenFsmAction
	{
		// Token: 0x06006C4C RID: 27724 RVA: 0x0021CBA4 File Offset: 0x0021ADA4
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.transformScale = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorScale = new FsmVector3
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
		}

		// Token: 0x06006C4D RID: 27725 RVA: 0x0021CC26 File Offset: 0x0021AE26
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006C4E RID: 27726 RVA: 0x0021CC49 File Offset: 0x0021AE49
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006C4F RID: 27727 RVA: 0x0021CC58 File Offset: 0x0021AE58
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = this.vectorScale.IsNone ? Vector3.zero : this.vectorScale.Value;
			if (!this.transformScale.IsNone && this.transformScale.Value)
			{
				vector = this.transformScale.Value.transform.localScale + vector;
			}
			this.itweenType = "scale";
			iTween.ScaleFrom(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"scale",
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
				!this.realTime.IsNone && this.realTime.Value
			}));
		}

		// Token: 0x04006C07 RID: 27655
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C08 RID: 27656
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04006C09 RID: 27657
		[Tooltip("Scale From a transform scale.")]
		public FsmGameObject transformScale;

		// Token: 0x04006C0A RID: 27658
		[Tooltip("A scale vector the GameObject will animate From.")]
		public FsmVector3 vectorScale;

		// Token: 0x04006C0B RID: 27659
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04006C0C RID: 27660
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x04006C0D RID: 27661
		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		// Token: 0x04006C0E RID: 27662
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x04006C0F RID: 27663
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;
	}
}
