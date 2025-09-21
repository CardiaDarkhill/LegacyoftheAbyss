using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5B RID: 2907
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the average value within an arrayList.")]
	public class ArrayListGetNearestFloatValue : ArrayListActions
	{
		// Token: 0x06005A70 RID: 23152 RVA: 0x001C96B6 File Offset: 0x001C78B6
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.floatValue = null;
			this.nearestIndex = null;
			this.nearestValue = null;
			this.everyframe = true;
		}

		// Token: 0x06005A71 RID: 23153 RVA: 0x001C96E2 File Offset: 0x001C78E2
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoGetNearestValue();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A72 RID: 23154 RVA: 0x001C9722 File Offset: 0x001C7922
		public override void OnUpdate()
		{
			this.DoGetNearestValue();
		}

		// Token: 0x06005A73 RID: 23155 RVA: 0x001C972C File Offset: 0x001C792C
		private void DoGetNearestValue()
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
			float value2 = this.floatValue.Value;
			if (this._floats.Count > 0)
			{
				float value3 = float.MaxValue;
				float num = float.MaxValue;
				int value4 = 0;
				int num2 = 0;
				foreach (float num3 in this._floats)
				{
					float num4 = Mathf.Abs(num3 - value2);
					if (num > num4)
					{
						num = num4;
						value3 = num3;
						value4 = num2;
					}
					num2++;
				}
				this.nearestIndex.Value = value4;
				this.nearestValue.Value = value3;
			}
		}

		// Token: 0x04005611 RID: 22033
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005612 RID: 22034
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005613 RID: 22035
		[Tooltip("The target Value")]
		public FsmFloat floatValue;

		// Token: 0x04005614 RID: 22036
		[Tooltip("Performs every frame. WARNING, it could be affecting performances.")]
		public bool everyframe;

		// Token: 0x04005615 RID: 22037
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The index of the nearest Value")]
		public FsmInt nearestIndex;

		// Token: 0x04005616 RID: 22038
		[UIHint(UIHint.Variable)]
		[Tooltip("The nearest Value")]
		public FsmFloat nearestValue;

		// Token: 0x04005617 RID: 22039
		private List<float> _floats;
	}
}
