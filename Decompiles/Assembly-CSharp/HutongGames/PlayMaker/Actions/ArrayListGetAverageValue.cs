using System;
using System.Collections.Generic;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B58 RID: 2904
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the average value within an arrayList. It can use float, int, vector2 and vector3 ( uses magnitude), rect ( uses surface), gameobject ( using bounding box volume), and string ( use lenght)")]
	public class ArrayListGetAverageValue : ArrayListActions
	{
		// Token: 0x06005A5D RID: 23133 RVA: 0x001C91FB File Offset: 0x001C73FB
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.averageValue = null;
			this.everyframe = true;
		}

		// Token: 0x06005A5E RID: 23134 RVA: 0x001C9219 File Offset: 0x001C7419
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoGetAverageValue();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A5F RID: 23135 RVA: 0x001C9259 File Offset: 0x001C7459
		public override void OnUpdate()
		{
			this.DoGetAverageValue();
		}

		// Token: 0x06005A60 RID: 23136 RVA: 0x001C9264 File Offset: 0x001C7464
		private void DoGetAverageValue()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this._floats = new List<float>();
			foreach (object value in this.proxy.arrayList)
			{
				this._floats.Add(Convert.ToSingle(value));
			}
			if (this._floats.Count > 0)
			{
				this.averageValue.Value = this._floats.Aggregate((float acc, float cur) => acc + cur) / (float)this._floats.Count;
				return;
			}
			this.averageValue.Value = 0f;
		}

		// Token: 0x04005600 RID: 22016
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005601 RID: 22017
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005602 RID: 22018
		[Tooltip("Performs every frame. WARNING, it could be affecting performances.")]
		public bool everyframe;

		// Token: 0x04005603 RID: 22019
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The average Value")]
		public FsmFloat averageValue;

		// Token: 0x04005604 RID: 22020
		private List<float> _floats;
	}
}
