using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B56 RID: 2902
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Returns the Gameobject within an arrayList which have the max float value in its FSM")]
	public class ArrayListGetGameobjectMaxFsmFloatIndex : ArrayListActions
	{
		// Token: 0x06005A53 RID: 23123 RVA: 0x001C8EDD File Offset: 0x001C70DD
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.maxGameObject = null;
			this.maxIndex = null;
			this.everyframe = true;
			this.fsmName = "";
			this.storeMaxValue = null;
		}

		// Token: 0x06005A54 RID: 23124 RVA: 0x001C8F19 File Offset: 0x001C7119
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoFindMaxGo();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A55 RID: 23125 RVA: 0x001C8F59 File Offset: 0x001C7159
		public override void OnUpdate()
		{
			this.DoFindMaxGo();
		}

		// Token: 0x06005A56 RID: 23126 RVA: 0x001C8F64 File Offset: 0x001C7164
		private void DoFindMaxGo()
		{
			float num = 0f;
			if (this.storeMaxValue.IsNone)
			{
				return;
			}
			if (!base.isProxyValid())
			{
				return;
			}
			int num2 = 0;
			foreach (object obj in this.proxy.arrayList)
			{
				GameObject gameObject = (GameObject)obj;
				if (gameObject != null)
				{
					this.fsm = ActionHelpers.GetGameObjectFsm(gameObject, this.fsmName.Value);
					if (this.fsm == null)
					{
						break;
					}
					FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
					if (fsmFloat == null)
					{
						break;
					}
					if (fsmFloat.Value > num)
					{
						this.storeMaxValue.Value = fsmFloat.Value;
						num = fsmFloat.Value;
						this.maxGameObject.Value = gameObject;
						this.maxIndex.Value = num2;
					}
				}
				num2++;
			}
		}

		// Token: 0x040055F0 RID: 22000
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055F1 RID: 22001
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055F2 RID: 22002
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x040055F3 RID: 22003
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		public FsmString variableName;

		// Token: 0x040055F4 RID: 22004
		public bool everyframe;

		// Token: 0x040055F5 RID: 22005
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeMaxValue;

		// Token: 0x040055F6 RID: 22006
		[UIHint(UIHint.Variable)]
		public FsmGameObject maxGameObject;

		// Token: 0x040055F7 RID: 22007
		[UIHint(UIHint.Variable)]
		public FsmInt maxIndex;

		// Token: 0x040055F8 RID: 22008
		private GameObject goLastFrame;

		// Token: 0x040055F9 RID: 22009
		private PlayMakerFSM fsm;
	}
}
