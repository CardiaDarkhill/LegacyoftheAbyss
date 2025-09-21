using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200105A RID: 4186
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Lets you call a method in a script on a Game Object.\nUnlike {{Invoke Method}}, or {{Send Message}}, you can use multiple parameters and get a return value.")]
	public class CallMethod : FsmStateAction
	{
		// Token: 0x06007279 RID: 29305 RVA: 0x0023295D File Offset: 0x00230B5D
		public override void Reset()
		{
			this.behaviour = null;
			this.methodName = null;
			this.parameters = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600727A RID: 29306 RVA: 0x00232982 File Offset: 0x00230B82
		public override void OnEnter()
		{
			this.parametersArray = new object[this.parameters.Length];
			this.DoMethodCall();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600727B RID: 29307 RVA: 0x002329AB File Offset: 0x00230BAB
		public override void OnUpdate()
		{
			this.DoMethodCall();
		}

		// Token: 0x0600727C RID: 29308 RVA: 0x002329B4 File Offset: 0x00230BB4
		private void DoMethodCall()
		{
			if (this.behaviour.Value == null)
			{
				base.Finish();
				return;
			}
			if (this.NeedToUpdateCache() && !this.DoCache())
			{
				Debug.LogError(this.errorString);
				base.Finish();
				return;
			}
			object value;
			if (this.cachedParameterInfo.Length == 0)
			{
				value = this.cachedMethodInfo.Invoke(this.cachedBehaviour.Value, null);
			}
			else
			{
				for (int i = 0; i < this.parameters.Length; i++)
				{
					FsmVar fsmVar = this.parameters[i];
					fsmVar.UpdateValue();
					if (fsmVar.Type == VariableType.Array)
					{
						fsmVar.UpdateValue();
						object[] array = fsmVar.GetValue() as object[];
						Array array2 = Array.CreateInstance(this.cachedParameterInfo[i].ParameterType.GetElementType(), array.Length);
						for (int j = 0; j < array.Length; j++)
						{
							array2.SetValue(array[j], j);
						}
						this.parametersArray[i] = array2;
					}
					else
					{
						fsmVar.UpdateValue();
						this.parametersArray[i] = fsmVar.GetValue();
					}
				}
				value = this.cachedMethodInfo.Invoke(this.cachedBehaviour.Value, this.parametersArray);
			}
			if (this.storeResult != null && !this.storeResult.IsNone && this.storeResult.Type != VariableType.Unknown)
			{
				this.storeResult.SetValue(value);
			}
		}

		// Token: 0x0600727D RID: 29309 RVA: 0x00232B10 File Offset: 0x00230D10
		private bool NeedToUpdateCache()
		{
			return this.cachedBehaviour == null || this.cachedMethodName == null || this.cachedBehaviour.Value != this.behaviour.Value || this.cachedBehaviour.Name != this.behaviour.Name || this.cachedMethodName.Value != this.methodName.Value || this.cachedMethodName.Name != this.methodName.Name;
		}

		// Token: 0x0600727E RID: 29310 RVA: 0x00232BA1 File Offset: 0x00230DA1
		private void ClearCache()
		{
			this.cachedBehaviour = null;
			this.cachedMethodName = null;
			this.cachedType = null;
			this.cachedMethodInfo = null;
			this.cachedParameterInfo = null;
		}

		// Token: 0x0600727F RID: 29311 RVA: 0x00232BC8 File Offset: 0x00230DC8
		private bool DoCache()
		{
			this.ClearCache();
			this.errorString = string.Empty;
			this.cachedBehaviour = new FsmObject(this.behaviour);
			this.cachedMethodName = new FsmString(this.methodName);
			if (this.cachedBehaviour.Value == null)
			{
				if (!this.behaviour.UsesVariable || Application.isPlaying)
				{
					this.errorString += "Behaviour is invalid!\n";
				}
				base.Finish();
				return false;
			}
			this.cachedType = this.behaviour.Value.GetType();
			List<Type> list = new List<Type>(this.parameters.Length);
			foreach (FsmVar fsmVar in this.parameters)
			{
				list.Add(fsmVar.RealType);
			}
			this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, list.ToArray());
			if (this.cachedMethodInfo == null)
			{
				this.errorString = this.errorString + "Invalid Method Name or Parameters: " + this.methodName.Value + "\n";
				base.Finish();
				return false;
			}
			this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
			return true;
		}

		// Token: 0x06007280 RID: 29312 RVA: 0x00232D08 File Offset: 0x00230F08
		public override string ErrorCheck()
		{
			if (Application.isPlaying)
			{
				return this.errorString;
			}
			if (!this.DoCache())
			{
				return this.errorString;
			}
			if (this.parameters.Length != this.cachedParameterInfo.Length)
			{
				return string.Concat(new string[]
				{
					"Parameter count does not match method.\nMethod has ",
					this.cachedParameterInfo.Length.ToString(),
					" parameters.\nYou specified ",
					this.parameters.Length.ToString(),
					" paramaters."
				});
			}
			for (int i = 0; i < this.parameters.Length; i++)
			{
				Type realType = this.parameters[i].RealType;
				Type parameterType = this.cachedParameterInfo[i].ParameterType;
				if (realType != parameterType)
				{
					string[] array = new string[6];
					array[0] = "Parameters do not match method signature.\nParameter ";
					array[1] = (i + 1).ToString();
					array[2] = " (";
					int num = 3;
					Type type = realType;
					array[num] = ((type != null) ? type.ToString() : null);
					array[4] = ") should be of type: ";
					int num2 = 5;
					Type type2 = parameterType;
					array[num2] = ((type2 != null) ? type2.ToString() : null);
					return string.Concat(array);
				}
			}
			if (this.cachedMethodInfo.ReturnType == typeof(void))
			{
				if (!string.IsNullOrEmpty(this.storeResult.variableName))
				{
					return "Method does not have return.\nSpecify 'none' in Store Result.";
				}
			}
			else if (this.cachedMethodInfo.ReturnType != this.storeResult.RealType)
			{
				string str = "Store Result is of the wrong type.\nIt should be of type: ";
				Type returnType = this.cachedMethodInfo.ReturnType;
				return str + ((returnType != null) ? returnType.ToString() : null);
			}
			return string.Empty;
		}

		// Token: 0x0400726A RID: 29290
		[ObjectType(typeof(Component))]
		[Tooltip("The behaviour on a Game Object that has the method you want to call. Drag the script component from the Unity inspector into this slot. HINT: Use Lock if the script is on another GameObject.\n\nNOTE: Unity Object fields show the GameObject name, so for clarity we show the Behaviour name as well below.")]
		public FsmObject behaviour;

		// Token: 0x0400726B RID: 29291
		[Tooltip("Select from a list of available methods.\n\nNOTE: The full method signature is visible below.")]
		public FsmString methodName;

		// Token: 0x0400726C RID: 29292
		[Tooltip("Method parameters.\n\nNOTE: This UI is built automatically when you select the method.")]
		public FsmVar[] parameters;

		// Token: 0x0400726D RID: 29293
		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("If the Method has a return, use this to store it in a variable.")]
		public FsmVar storeResult;

		// Token: 0x0400726E RID: 29294
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400726F RID: 29295
		[Tooltip("Revert to the old Manual UI where all parameters had to be configured manually.")]
		public bool manualUI;

		// Token: 0x04007270 RID: 29296
		private FsmObject cachedBehaviour;

		// Token: 0x04007271 RID: 29297
		private FsmString cachedMethodName;

		// Token: 0x04007272 RID: 29298
		private Type cachedType;

		// Token: 0x04007273 RID: 29299
		private MethodInfo cachedMethodInfo;

		// Token: 0x04007274 RID: 29300
		private ParameterInfo[] cachedParameterInfo;

		// Token: 0x04007275 RID: 29301
		private object[] parametersArray;

		// Token: 0x04007276 RID: 29302
		private string errorString;
	}
}
