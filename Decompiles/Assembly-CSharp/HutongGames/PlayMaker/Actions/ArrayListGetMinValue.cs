using System;
using System.Runtime.CompilerServices;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5A RID: 2906
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the minimum value within an arrayList. It can use float, int, vector2 and vector3 ( uses magnitude), rect ( uses surface), gameobject ( using bounding box volume), and string ( use lenght)")]
	public class ArrayListGetMinValue : ArrayListActions
	{
		// Token: 0x06005A69 RID: 23145 RVA: 0x001C94FE File Offset: 0x001C76FE
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.minimumValue = null;
			this.minimumValueIndex = null;
			this.everyframe = true;
		}

		// Token: 0x06005A6A RID: 23146 RVA: 0x001C9523 File Offset: 0x001C7723
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoFindMinimumValue();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A6B RID: 23147 RVA: 0x001C9563 File Offset: 0x001C7763
		public override void OnUpdate()
		{
			this.DoFindMinimumValue();
		}

		// Token: 0x06005A6C RID: 23148 RVA: 0x001C956C File Offset: 0x001C776C
		private void DoFindMinimumValue()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			VariableType type = this.minimumValue.Type;
			if (!ArrayListGetMinValue.supportedTypes.Contains(this.minimumValue.Type))
			{
				return;
			}
			float num = float.PositiveInfinity;
			int num2 = 0;
			int num3 = 0;
			foreach (object obj in this.proxy.arrayList)
			{
				float floatFromObject = PlayMakerUtils.GetFloatFromObject(obj, type, true);
				if (num > floatFromObject)
				{
					num = floatFromObject;
					num2 = num3;
				}
				num3++;
			}
			this.minimumValueIndex.Value = num2;
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.minimumValue, this.proxy.arrayList[num2]);
		}

		// Token: 0x06005A6D RID: 23149 RVA: 0x001C9644 File Offset: 0x001C7844
		public override string ErrorCheck()
		{
			if (!ArrayListGetMinValue.supportedTypes.Contains(this.minimumValue.Type))
			{
				return "A " + this.minimumValue.Type.ToString() + " can not be processed as a minimum";
			}
			return "";
		}

		// Token: 0x06005A6F RID: 23151 RVA: 0x001C969E File Offset: 0x001C789E
		// Note: this type is marked as 'beforefieldinit'.
		static ArrayListGetMinValue()
		{
			VariableType[] array = new VariableType[7];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.D18B72E3104D17471D27BD26FBD89EA6685E4AE36B0FDF2562CD7B7B8F692B9C).FieldHandle);
			ArrayListGetMinValue.supportedTypes = array;
		}

		// Token: 0x0400560B RID: 22027
		private static VariableType[] supportedTypes;

		// Token: 0x0400560C RID: 22028
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400560D RID: 22029
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400560E RID: 22030
		[Tooltip("Performs every frame. WARNING, it could be affecting performances.")]
		public bool everyframe;

		// Token: 0x0400560F RID: 22031
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Minimum Value")]
		public FsmVar minimumValue;

		// Token: 0x04005610 RID: 22032
		[UIHint(UIHint.Variable)]
		[Tooltip("The index of the Maximum Value within that arrayList")]
		public FsmInt minimumValueIndex;
	}
}
