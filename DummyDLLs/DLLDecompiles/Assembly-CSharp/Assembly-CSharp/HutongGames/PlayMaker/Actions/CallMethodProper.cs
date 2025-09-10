using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDA RID: 3034
	[ActionCategory(ActionCategory.ScriptControl)]
	public class CallMethodProper : FsmStateAction
	{
		// Token: 0x06005CFD RID: 23805 RVA: 0x001D3860 File Offset: 0x001D1A60
		public override void Awake()
		{
			if (Application.isPlaying)
			{
				this.PreCache();
			}
		}

		// Token: 0x06005CFE RID: 23806 RVA: 0x001D386F File Offset: 0x001D1A6F
		public override void OnEnter()
		{
			this.parametersArray = new object[this.parameters.Length];
			this.DoMethodCall();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CFF RID: 23807 RVA: 0x001D3898 File Offset: 0x001D1A98
		public override void OnUpdate()
		{
			this.DoMethodCall();
		}

		// Token: 0x06005D00 RID: 23808 RVA: 0x001D38A0 File Offset: 0x001D1AA0
		private void DoMethodCall()
		{
			if (string.IsNullOrEmpty(this.behaviour.Value) || string.IsNullOrEmpty(this.methodName.Value))
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
			if (this.cachedMethodInfo == null || !this.cachedMethodInfo.Name.Equals(this.methodName.Value))
			{
				this.errorString = string.Empty;
				if (!this.DoCache())
				{
					Debug.LogError(this.errorString, base.Owner);
					base.Finish();
					return;
				}
			}
			object value = null;
			if (this.cachedParameterInfo.Length == 0)
			{
				value = this.cachedMethodInfo.Invoke(this.component, null);
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
					value = this.cachedMethodInfo.Invoke(this.component, this.parametersArray);
				}
				catch (TargetParameterCountException)
				{
					ParameterInfo[] array = this.cachedMethodInfo.GetParameters();
					Debug.LogErrorFormat(base.Owner, "Count did not match. Required: {0}, Was: {1}, Method: {2}", new object[]
					{
						array.Length,
						this.parametersArray.Length,
						this.cachedMethodInfo.Name
					});
				}
				catch (Exception ex)
				{
					string str = "CallMethodProper error on ";
					string ownerName = base.Fsm.OwnerName;
					string str2 = " -> ";
					Exception ex2 = ex;
					Debug.LogError(str + ownerName + str2 + ((ex2 != null) ? ex2.ToString() : null), base.Owner);
				}
			}
			if (this.storeResult.Type != VariableType.Unknown)
			{
				this.storeResult.SetValue(value);
			}
		}

		// Token: 0x06005D01 RID: 23809 RVA: 0x001D3ACC File Offset: 0x001D1CCC
		private bool DoCache()
		{
			this.cachedType = this.component.GetType();
			try
			{
				this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value);
			}
			catch (AmbiguousMatchException)
			{
				Type[] types = (from fsmVar in this.parameters
				select fsmVar.RealType).ToArray<Type>();
				this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, types);
			}
			if (this.cachedMethodInfo == null)
			{
				this.errorString = this.errorString + "Method Name is invalid: " + this.methodName.Value + "\n";
				base.Finish();
				return false;
			}
			this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
			return true;
		}

		// Token: 0x06005D02 RID: 23810 RVA: 0x001D3BB8 File Offset: 0x001D1DB8
		private void PreCache()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.component = (ownerDefaultTarget.GetComponent(this.behaviour.Value) as MonoBehaviour);
			if (this.component == null)
			{
				return;
			}
			this.cachedType = this.component.GetType();
			try
			{
				this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value);
			}
			catch (AmbiguousMatchException)
			{
				Type[] types = (from fsmVar in this.parameters
				select fsmVar.RealType).ToArray<Type>();
				this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, types);
			}
			if (this.cachedMethodInfo == null)
			{
				this.errorString = this.errorString + "Method Name is invalid: " + this.methodName.Value + "\n";
				return;
			}
			this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
		}

		// Token: 0x040058A1 RID: 22689
		[RequiredField]
		[Tooltip("The game object that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040058A2 RID: 22690
		[RequiredField]
		[UIHint(UIHint.Behaviour)]
		[Tooltip("The Behaviour that contains the method to start as a coroutine.")]
		public FsmString behaviour;

		// Token: 0x040058A3 RID: 22691
		[UIHint(UIHint.Method)]
		[Tooltip("Name of the method to call on the component")]
		public FsmString methodName;

		// Token: 0x040058A4 RID: 22692
		[Tooltip("Method paramters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		// Token: 0x040058A5 RID: 22693
		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the method call.")]
		public FsmVar storeResult;

		// Token: 0x040058A6 RID: 22694
		public bool EveryFrame;

		// Token: 0x040058A7 RID: 22695
		private Type cachedType;

		// Token: 0x040058A8 RID: 22696
		private MethodInfo cachedMethodInfo;

		// Token: 0x040058A9 RID: 22697
		private ParameterInfo[] cachedParameterInfo;

		// Token: 0x040058AA RID: 22698
		private object[] parametersArray;

		// Token: 0x040058AB RID: 22699
		private string errorString;

		// Token: 0x040058AC RID: 22700
		private MonoBehaviour component;
	}
}
