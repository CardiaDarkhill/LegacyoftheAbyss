using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F26 RID: 3878
	[ActionCategory("iTween")]
	[Tooltip("Instantly changes a GameObject's Euler angles in degrees then returns it to it's starting rotation over time.")]
	public class iTweenRotateFrom : iTweenFsmAction
	{
		// Token: 0x06006C32 RID: 27698 RVA: 0x0021BD34 File Offset: 0x00219F34
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.transformRotation = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorRotation = new FsmVector3
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
			this.space = Space.World;
		}

		// Token: 0x06006C33 RID: 27699 RVA: 0x0021BDBD File Offset: 0x00219FBD
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006C34 RID: 27700 RVA: 0x0021BDE0 File Offset: 0x00219FE0
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006C35 RID: 27701 RVA: 0x0021BDF0 File Offset: 0x00219FF0
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = this.vectorRotation.IsNone ? Vector3.zero : this.vectorRotation.Value;
			if (!this.transformRotation.IsNone && this.transformRotation.Value)
			{
				vector = ((this.space == Space.World) ? (this.transformRotation.Value.transform.eulerAngles + vector) : (this.transformRotation.Value.transform.localEulerAngles + vector));
			}
			this.itweenType = "rotate";
			iTween.RotateFrom(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"rotation",
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
				"islocal",
				this.space == Space.Self
			}));
		}

		// Token: 0x04006BDC RID: 27612
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006BDD RID: 27613
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04006BDE RID: 27614
		[Tooltip("A rotation from a GameObject.")]
		public FsmGameObject transformRotation;

		// Token: 0x04006BDF RID: 27615
		[Tooltip("A rotation vector the GameObject will animate from.")]
		public FsmVector3 vectorRotation;

		// Token: 0x04006BE0 RID: 27616
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04006BE1 RID: 27617
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x04006BE2 RID: 27618
		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		// Token: 0x04006BE3 RID: 27619
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x04006BE4 RID: 27620
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		// Token: 0x04006BE5 RID: 27621
		[Tooltip("Whether to animate in local or world space.")]
		public Space space;
	}
}
