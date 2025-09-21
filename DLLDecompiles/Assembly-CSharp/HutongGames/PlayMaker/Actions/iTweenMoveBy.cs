using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F1B RID: 3867
	[ActionCategory("iTween")]
	[Tooltip("Adds the supplied vector to a GameObject's position.")]
	public class iTweenMoveBy : iTweenFsmAction
	{
		// Token: 0x06006BFB RID: 27643 RVA: 0x002198B0 File Offset: 0x00217AB0
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.time = 1f;
			this.delay = 0f;
			this.loopType = iTween.LoopType.none;
			this.vector = new FsmVector3
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.orientToPath = false;
			this.lookAtObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.lookAtVector = new FsmVector3
			{
				UseVariable = true
			};
			this.lookTime = 0f;
			this.axis = iTweenFsmAction.AxisRestriction.none;
		}

		// Token: 0x06006BFC RID: 27644 RVA: 0x0021996E File Offset: 0x00217B6E
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006BFD RID: 27645 RVA: 0x00219991 File Offset: 0x00217B91
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006BFE RID: 27646 RVA: 0x002199A0 File Offset: 0x00217BA0
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("amount", this.vector.IsNone ? Vector3.zero : this.vector.Value);
			hashtable.Add(this.speed.IsNone ? "time" : "speed", this.speed.IsNone ? (this.time.IsNone ? 1f : this.time.Value) : this.speed.Value);
			hashtable.Add("delay", this.delay.IsNone ? 0f : this.delay.Value);
			hashtable.Add("easetype", this.easeType);
			hashtable.Add("looptype", this.loopType);
			hashtable.Add("oncomplete", "iTweenOnComplete");
			hashtable.Add("oncompleteparams", this.itweenID);
			hashtable.Add("onstart", "iTweenOnStart");
			hashtable.Add("onstartparams", this.itweenID);
			hashtable.Add("ignoretimescale", !this.realTime.IsNone && this.realTime.Value);
			hashtable.Add("space", this.space);
			hashtable.Add("name", this.id.IsNone ? "" : this.id.Value);
			hashtable.Add("axis", (this.axis == iTweenFsmAction.AxisRestriction.none) ? "" : Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis));
			if (!this.orientToPath.IsNone)
			{
				hashtable.Add("orienttopath", this.orientToPath.Value);
			}
			if (!this.lookAtObject.IsNone)
			{
				hashtable.Add("looktarget", this.lookAtVector.IsNone ? this.lookAtObject.Value.transform.position : (this.lookAtObject.Value.transform.position + this.lookAtVector.Value));
			}
			else if (!this.lookAtVector.IsNone)
			{
				hashtable.Add("looktarget", this.lookAtVector.Value);
			}
			if (!this.lookAtObject.IsNone || !this.lookAtVector.IsNone)
			{
				hashtable.Add("looktime", this.lookTime.IsNone ? 0f : this.lookTime.Value);
			}
			this.itweenType = "move";
			iTween.MoveBy(ownerDefaultTarget, hashtable);
		}

		// Token: 0x04006B6F RID: 27503
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006B70 RID: 27504
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04006B71 RID: 27505
		[RequiredField]
		[Tooltip("The vector to add to the GameObject's position.")]
		public FsmVector3 vector;

		// Token: 0x04006B72 RID: 27506
		[Tooltip("For the time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04006B73 RID: 27507
		[Tooltip("For the time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x04006B74 RID: 27508
		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		// Token: 0x04006B75 RID: 27509
		[Tooltip("For the shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x04006B76 RID: 27510
		[Tooltip("For the type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		// Token: 0x04006B77 RID: 27511
		public Space space;

		// Token: 0x04006B78 RID: 27512
		[ActionSection("LookAt")]
		[Tooltip("For whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		// Token: 0x04006B79 RID: 27513
		[Tooltip("For a target the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		// Token: 0x04006B7A RID: 27514
		[Tooltip("For a target the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		// Token: 0x04006B7B RID: 27515
		[Tooltip("For the time in seconds the object will take to look at either the 'looktarget' or 'orienttopath'. 0 by default")]
		public FsmFloat lookTime;

		// Token: 0x04006B7C RID: 27516
		[Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
		public iTweenFsmAction.AxisRestriction axis;
	}
}
