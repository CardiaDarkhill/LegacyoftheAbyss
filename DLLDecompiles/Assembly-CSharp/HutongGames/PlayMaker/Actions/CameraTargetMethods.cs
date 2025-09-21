using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200122D RID: 4653
	public sealed class CameraTargetMethods : FsmStateAction
	{
		// Token: 0x06007B49 RID: 31561 RVA: 0x0024F373 File Offset: 0x0024D573
		public override void Reset()
		{
			this.parameters = null;
			this.everyFrame = null;
			this.storeValue = new FsmVar();
			this.isTrue = null;
			this.isFalse = null;
		}

		// Token: 0x06007B4A RID: 31562 RVA: 0x0024F39C File Offset: 0x0024D59C
		public override void OnEnter()
		{
			this.hasCameraTarget = this.heroController;
			if (!this.hasCameraTarget)
			{
				GameCameras instance = GameCameras.instance;
				if (instance)
				{
					this.heroController = instance.cameraTarget;
					this.hasCameraTarget = this.heroController;
				}
			}
			if (!this.hasCameraTarget || !this.everyFrame.Value)
			{
				this.SendMethod();
				base.Finish();
			}
			if (!this.hasCameraTarget)
			{
				Debug.LogError("Failed to find camera target.");
			}
		}

		// Token: 0x06007B4B RID: 31563 RVA: 0x0024F420 File Offset: 0x0024D620
		public override void OnUpdate()
		{
			this.SendMethod();
		}

		// Token: 0x06007B4C RID: 31564 RVA: 0x0024F428 File Offset: 0x0024D628
		private void SendMethod()
		{
			if (!this.hasCameraTarget)
			{
				return;
			}
			CameraTargetMethods.CameraMethod cameraMethod;
			if (CameraTargetMethods.HeroMethods.methodInfos.TryGetValue(this.method, out cameraMethod))
			{
				try
				{
					if (this.parameters != null)
					{
						for (int i = 0; i < this.parameters.Length; i++)
						{
							this.parameters[i].UpdateValue();
						}
					}
					cameraMethod.SendMethod(this);
					if (this.storeValue.Type == VariableType.Bool)
					{
						if (this.storeValue.boolValue)
						{
							base.Fsm.Event(this.isTrue);
						}
						else
						{
							base.Fsm.Event(this.isFalse);
						}
					}
					return;
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					base.Finish();
					return;
				}
			}
			Debug.LogError(string.Format("Failed to find hero method for \"{0}\".", this.method));
		}

		// Token: 0x06007B4D RID: 31565 RVA: 0x0024F4FC File Offset: 0x0024D6FC
		public bool ShouldHideParameters()
		{
			CameraTargetMethods.CameraMethod cameraMethod;
			return CameraTargetMethods.HeroMethods.methodInfos.TryGetValue(this.method, out cameraMethod) && (this.parameters == null || this.parameters.Length == 0);
		}

		// Token: 0x06007B4E RID: 31566 RVA: 0x0024F533 File Offset: 0x0024D733
		public bool ShouldHideStoreValue()
		{
			return this.GetReturnType() == VariableType.Unknown;
		}

		// Token: 0x06007B4F RID: 31567 RVA: 0x0024F53E File Offset: 0x0024D73E
		public bool ShouldHideTrueFalse()
		{
			return this.GetReturnType() != VariableType.Bool;
		}

		// Token: 0x06007B50 RID: 31568 RVA: 0x0024F54C File Offset: 0x0024D74C
		public VariableType GetReturnType()
		{
			CameraTargetMethods.CameraMethod cameraMethod;
			if (CameraTargetMethods.HeroMethods.methodInfos.TryGetValue(this.method, out cameraMethod))
			{
				return cameraMethod.returnType;
			}
			return VariableType.Unknown;
		}

		// Token: 0x04007B92 RID: 31634
		public CameraTargetMethods.Method method;

		// Token: 0x04007B93 RID: 31635
		[HideIf("ShouldHideParameters")]
		public FsmVar[] parameters;

		// Token: 0x04007B94 RID: 31636
		public FsmBool everyFrame;

		// Token: 0x04007B95 RID: 31637
		[HideIf("ShouldHideStoreValue")]
		[ActionSection("Store Result")]
		[UIHint(UIHint.Variable)]
		public FsmVar storeValue;

		// Token: 0x04007B96 RID: 31638
		[ActionSection("Events")]
		[HideIf("ShouldHideTrueFalse")]
		public FsmEvent isTrue;

		// Token: 0x04007B97 RID: 31639
		[HideIf("ShouldHideTrueFalse")]
		public FsmEvent isFalse;

		// Token: 0x04007B98 RID: 31640
		private bool hasCameraTarget;

		// Token: 0x04007B99 RID: 31641
		private CameraTarget heroController;

		// Token: 0x02001BD7 RID: 7127
		public enum Method
		{
			// Token: 0x04009F18 RID: 40728
			SetSprint,
			// Token: 0x04009F19 RID: 40729
			SetWallSprint
		}

		// Token: 0x02001BD8 RID: 7128
		public static class HeroMethods
		{
			// Token: 0x06009A6A RID: 39530 RVA: 0x002B3534 File Offset: 0x002B1734
			static HeroMethods()
			{
				CameraTargetMethods.HeroMethods.methodInfos.Add(CameraTargetMethods.Method.SetSprint, new CameraTargetMethods.CameraMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(CameraTargetMethods action)
				{
					if (action.hasCameraTarget)
					{
						action.heroController.SetSprint(action.parameters[0].boolValue);
					}
				}));
				CameraTargetMethods.HeroMethods.methodInfos.Add(CameraTargetMethods.Method.SetWallSprint, new CameraTargetMethods.CameraMethod(new VariableType[]
				{
					VariableType.Bool
				}, VariableType.Unknown, delegate(CameraTargetMethods action)
				{
					if (action.hasCameraTarget)
					{
						action.heroController.SetWallSprint(action.parameters[0].boolValue);
					}
				}));
			}

			// Token: 0x04009F1A RID: 40730
			public static readonly Dictionary<CameraTargetMethods.Method, CameraTargetMethods.CameraMethod> methodInfos = new Dictionary<CameraTargetMethods.Method, CameraTargetMethods.CameraMethod>();
		}

		// Token: 0x02001BD9 RID: 7129
		public sealed class CameraMethod
		{
			// Token: 0x170011AF RID: 4527
			// (get) Token: 0x06009A6B RID: 39531 RVA: 0x002B35A1 File Offset: 0x002B17A1
			public VariableType[] parameters { get; }

			// Token: 0x170011B0 RID: 4528
			// (get) Token: 0x06009A6C RID: 39532 RVA: 0x002B35A9 File Offset: 0x002B17A9
			public VariableType returnType { get; }

			// Token: 0x06009A6D RID: 39533 RVA: 0x002B35B1 File Offset: 0x002B17B1
			public void SendMethod(CameraTargetMethods action)
			{
				Action<CameraTargetMethods> action2 = this.fsmAction;
				if (action2 == null)
				{
					return;
				}
				action2(action);
			}

			// Token: 0x06009A6E RID: 39534 RVA: 0x002B35C4 File Offset: 0x002B17C4
			public CameraMethod(VariableType[] parameters = null, VariableType returnType = VariableType.Unknown, Action<CameraTargetMethods> fsmAction = null)
			{
				if (parameters == null)
				{
					this.parameters = new VariableType[0];
				}
				else
				{
					this.parameters = parameters;
				}
				this.returnType = returnType;
				this.fsmAction = fsmAction;
			}

			// Token: 0x04009F1D RID: 40733
			public Action<CameraTargetMethods> fsmAction;
		}
	}
}
