using System;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BD9 RID: 3033
	[ActionCategory(ActionCategory.ScriptControl)]
	public class CallMethodOnExit : FsmStateAction
	{
		// Token: 0x06005CF9 RID: 23801 RVA: 0x001D3619 File Offset: 0x001D1819
		public override void OnExit()
		{
			this.parametersArray = new object[this.parameters.Length];
			this.DoMethodCall();
			base.Finish();
		}

		// Token: 0x06005CFA RID: 23802 RVA: 0x001D363C File Offset: 0x001D183C
		private void DoMethodCall()
		{
			if (this.behaviour.Value == null)
			{
				base.Finish();
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.component = (ownerDefaultTarget.GetComponent(this.behaviour.Value) as MonoBehaviour);
			if (this.component == null)
			{
				base.LogWarning("CallMethodProper: " + ownerDefaultTarget.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
			if (this.cachedMethodInfo == null)
			{
				this.errorString = string.Empty;
				if (!this.DoCache())
				{
					Debug.LogError(this.errorString);
					base.Finish();
					return;
				}
			}
			object value = null;
			if (this.cachedParameterInfo.Length == 0)
			{
				value = this.cachedMethodInfo.Invoke(this.cachedBehaviour, null);
			}
			else
			{
				for (int i = 0; i < this.parameters.Length; i++)
				{
					FsmVar fsmVar = this.parameters[i];
					fsmVar.UpdateValue();
					this.parametersArray[i] = fsmVar.GetValue();
				}
				try
				{
					value = this.cachedMethodInfo.Invoke(this.cachedBehaviour, this.parametersArray);
				}
				catch (Exception ex)
				{
					string str = "CallMethodProper error on ";
					string ownerName = base.Fsm.OwnerName;
					string str2 = " -> ";
					Exception ex2 = ex;
					Debug.LogError(str + ownerName + str2 + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
			if (this.storeResult.Type != VariableType.Unknown)
			{
				this.storeResult.SetValue(value);
			}
		}

		// Token: 0x06005CFB RID: 23803 RVA: 0x001D37C4 File Offset: 0x001D19C4
		private bool DoCache()
		{
			this.cachedBehaviour = this.component;
			this.cachedType = this.component.GetType();
			this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value);
			if (this.cachedMethodInfo == null)
			{
				this.errorString = this.errorString + "Method Name is invalid: " + this.methodName.Value + "\n";
				base.Finish();
				return false;
			}
			this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
			return true;
		}

		// Token: 0x04005895 RID: 22677
		[RequiredField]
		[Tooltip("The game object that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005896 RID: 22678
		[RequiredField]
		[UIHint(UIHint.Behaviour)]
		[Tooltip("The Behaviour that contains the method to start as a coroutine.")]
		public FsmString behaviour;

		// Token: 0x04005897 RID: 22679
		[UIHint(UIHint.Method)]
		[Tooltip("Name of the method to call on the component")]
		public FsmString methodName;

		// Token: 0x04005898 RID: 22680
		[Tooltip("Method paramters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		// Token: 0x04005899 RID: 22681
		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the method call.")]
		public FsmVar storeResult;

		// Token: 0x0400589A RID: 22682
		private Object cachedBehaviour;

		// Token: 0x0400589B RID: 22683
		private Type cachedType;

		// Token: 0x0400589C RID: 22684
		private MethodInfo cachedMethodInfo;

		// Token: 0x0400589D RID: 22685
		private ParameterInfo[] cachedParameterInfo;

		// Token: 0x0400589E RID: 22686
		private object[] parametersArray;

		// Token: 0x0400589F RID: 22687
		private string errorString;

		// Token: 0x040058A0 RID: 22688
		private MonoBehaviour component;
	}
}
