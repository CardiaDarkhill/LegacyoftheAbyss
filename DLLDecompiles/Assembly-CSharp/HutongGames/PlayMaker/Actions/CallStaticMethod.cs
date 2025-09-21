using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200105B RID: 4187
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Call a static method in a class.\nNOTE: This is an advanced action - you need to know the full method signature to use this action.")]
	public class CallStaticMethod : FsmStateAction
	{
		// Token: 0x06007282 RID: 29314 RVA: 0x00232E8C File Offset: 0x0023108C
		public override void OnEnter()
		{
			this.parametersArray = new object[this.parameters.Length];
			this.DoMethodCall();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007283 RID: 29315 RVA: 0x00232EB5 File Offset: 0x002310B5
		public override void OnUpdate()
		{
			this.DoMethodCall();
		}

		// Token: 0x06007284 RID: 29316 RVA: 0x00232EC0 File Offset: 0x002310C0
		private void DoMethodCall()
		{
			if (this.className == null || string.IsNullOrEmpty(this.className.Value))
			{
				base.Finish();
				return;
			}
			if (this.cachedClassName != this.className.Value || this.cachedMethodName != this.methodName.Value)
			{
				this.errorString = string.Empty;
				if (!this.DoCache())
				{
					Debug.LogError(this.errorString);
					base.Finish();
					return;
				}
			}
			object value;
			if (this.cachedParameterInfo.Length == 0)
			{
				value = this.cachedMethodInfo.Invoke(null, null);
			}
			else
			{
				for (int i = 0; i < this.parameters.Length; i++)
				{
					FsmVar fsmVar = this.parameters[i];
					fsmVar.UpdateValue();
					this.parametersArray[i] = fsmVar.GetValue();
				}
				value = this.cachedMethodInfo.Invoke(null, this.parametersArray);
			}
			if (!this.storeResult.IsNone)
			{
				this.storeResult.SetValue(value);
			}
		}

		// Token: 0x06007285 RID: 29317 RVA: 0x00232FBC File Offset: 0x002311BC
		private bool DoCache()
		{
			this.cachedType = ReflectionUtils.GetGlobalType(this.className.Value);
			if (this.cachedType == null)
			{
				this.errorString = this.errorString + "Class is invalid: " + this.className.Value + "\n";
				base.Finish();
				return false;
			}
			this.cachedClassName = this.className.Value;
			List<Type> list = new List<Type>(this.parameters.Length);
			foreach (FsmVar fsmVar in this.parameters)
			{
				if (fsmVar != null && fsmVar.RealType != null)
				{
					list.Add(fsmVar.RealType);
				}
			}
			try
			{
				this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, list.ToArray());
			}
			catch (Exception ex)
			{
				this.errorString = this.errorString + ex.Message + "\n";
			}
			if (this.cachedMethodInfo == null)
			{
				this.errorString = this.errorString + "Invalid Method Name or Parameters: " + this.methodName.Value + "\n";
				base.Finish();
				return false;
			}
			this.cachedMethodName = this.methodName.Value;
			this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
			return true;
		}

		// Token: 0x06007286 RID: 29318 RVA: 0x00233128 File Offset: 0x00231328
		public override string ErrorCheck()
		{
			this.errorString = string.Empty;
			this.DoCache();
			if (!string.IsNullOrEmpty(this.errorString))
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

		// Token: 0x04007277 RID: 29303
		[Tooltip("Full path to the class that contains the static method.")]
		public FsmString className;

		// Token: 0x04007278 RID: 29304
		[Tooltip("The static method to call.")]
		public FsmString methodName;

		// Token: 0x04007279 RID: 29305
		[Tooltip("Method parameters. NOTE: these must match the method's signature!")]
		public FsmVar[] parameters;

		// Token: 0x0400727A RID: 29306
		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the method call.")]
		public FsmVar storeResult;

		// Token: 0x0400727B RID: 29307
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400727C RID: 29308
		private Type cachedType;

		// Token: 0x0400727D RID: 29309
		private string cachedClassName;

		// Token: 0x0400727E RID: 29310
		private string cachedMethodName;

		// Token: 0x0400727F RID: 29311
		private MethodInfo cachedMethodInfo;

		// Token: 0x04007280 RID: 29312
		private ParameterInfo[] cachedParameterInfo;

		// Token: 0x04007281 RID: 29313
		private object[] parametersArray;

		// Token: 0x04007282 RID: 29314
		private string errorString;
	}
}
