using System;
using System.Runtime.CompilerServices;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B59 RID: 2905
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the maximum value within an arrayList. It can use float, int, vector2 and vector3 ( uses magnitude), rect ( uses surface), gameobject ( using bounding box volume), and string ( use lenght)")]
	public class ArrayListGetMaxValue : ArrayListActions
	{
		// Token: 0x06005A62 RID: 23138 RVA: 0x001C9344 File Offset: 0x001C7544
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.maximumValue = null;
			this.maximumValueIndex = null;
			this.everyframe = true;
		}

		// Token: 0x06005A63 RID: 23139 RVA: 0x001C9369 File Offset: 0x001C7569
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoFindMaximumValue();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A64 RID: 23140 RVA: 0x001C93A9 File Offset: 0x001C75A9
		public override void OnUpdate()
		{
			this.DoFindMaximumValue();
		}

		// Token: 0x06005A65 RID: 23141 RVA: 0x001C93B4 File Offset: 0x001C75B4
		private void DoFindMaximumValue()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			VariableType type = this.maximumValue.Type;
			if (!ArrayListGetMaxValue.supportedTypes.Contains(this.maximumValue.Type))
			{
				return;
			}
			float num = float.NegativeInfinity;
			int num2 = 0;
			int num3 = 0;
			foreach (object obj in this.proxy.arrayList)
			{
				float floatFromObject = PlayMakerUtils.GetFloatFromObject(obj, type, true);
				if (num < floatFromObject)
				{
					num = floatFromObject;
					num2 = num3;
				}
				num3++;
			}
			this.maximumValueIndex.Value = num2;
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.maximumValue, this.proxy.arrayList[num2]);
		}

		// Token: 0x06005A66 RID: 23142 RVA: 0x001C948C File Offset: 0x001C768C
		public override string ErrorCheck()
		{
			if (!ArrayListGetMaxValue.supportedTypes.Contains(this.maximumValue.Type))
			{
				return "A " + this.maximumValue.Type.ToString() + " can not be processed as a minimum";
			}
			return "";
		}

		// Token: 0x06005A68 RID: 23144 RVA: 0x001C94E6 File Offset: 0x001C76E6
		// Note: this type is marked as 'beforefieldinit'.
		static ArrayListGetMaxValue()
		{
			VariableType[] array = new VariableType[7];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.D18B72E3104D17471D27BD26FBD89EA6685E4AE36B0FDF2562CD7B7B8F692B9C).FieldHandle);
			ArrayListGetMaxValue.supportedTypes = array;
		}

		// Token: 0x04005605 RID: 22021
		private static VariableType[] supportedTypes;

		// Token: 0x04005606 RID: 22022
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005607 RID: 22023
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005608 RID: 22024
		[Tooltip("Performs every frame. WARNING, it could be affecting performances.")]
		public bool everyframe;

		// Token: 0x04005609 RID: 22025
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Maximum Value")]
		public FsmVar maximumValue;

		// Token: 0x0400560A RID: 22026
		[UIHint(UIHint.Variable)]
		[Tooltip("The index of the Maximum Value within that arrayList")]
		public FsmInt maximumValueIndex;
	}
}
