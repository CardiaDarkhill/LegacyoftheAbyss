using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200105D RID: 4189
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Invokes a Method in a Behaviour attached to a Game Object. See Unity InvokeMethod docs.")]
	public class InvokeMethod : FsmStateAction
	{
		// Token: 0x0600728E RID: 29326 RVA: 0x00233470 File Offset: 0x00231670
		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.methodName = "";
			this.delay = null;
			this.repeating = false;
			this.repeatDelay = 1f;
			this.cancelOnExit = false;
		}

		// Token: 0x0600728F RID: 29327 RVA: 0x002334CA File Offset: 0x002316CA
		public override void OnEnter()
		{
			this.DoInvokeMethod(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x06007290 RID: 29328 RVA: 0x002334EC File Offset: 0x002316EC
		private void DoInvokeMethod(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			this.component = (go.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as MonoBehaviour);
			if (this.component == null)
			{
				base.LogWarning("InvokeMethod: " + go.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			if (this.repeating.Value)
			{
				this.component.InvokeRepeating(this.methodName.Value, this.delay.Value, this.repeatDelay.Value);
				return;
			}
			this.component.Invoke(this.methodName.Value, this.delay.Value);
		}

		// Token: 0x06007291 RID: 29329 RVA: 0x002335B4 File Offset: 0x002317B4
		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.cancelOnExit.Value)
			{
				this.component.CancelInvoke(this.methodName.Value);
			}
		}

		// Token: 0x04007289 RID: 29321
		[RequiredField]
		[Tooltip("The game object that owns the behaviour.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400728A RID: 29322
		[RequiredField]
		[UIHint(UIHint.Script)]
		[Tooltip("The behaviour that contains the method.")]
		public FsmString behaviour;

		// Token: 0x0400728B RID: 29323
		[RequiredField]
		[UIHint(UIHint.Method)]
		[Tooltip("The name of the method to invoke.")]
		public FsmString methodName;

		// Token: 0x0400728C RID: 29324
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Optional time delay in seconds.")]
		public FsmFloat delay;

		// Token: 0x0400728D RID: 29325
		[Tooltip("Call the method repeatedly.")]
		public FsmBool repeating;

		// Token: 0x0400728E RID: 29326
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Delay between repeated calls in seconds.")]
		public FsmFloat repeatDelay;

		// Token: 0x0400728F RID: 29327
		[Tooltip("Stop calling the method when the state is exited.")]
		public FsmBool cancelOnExit;

		// Token: 0x04007290 RID: 29328
		private MonoBehaviour component;
	}
}
