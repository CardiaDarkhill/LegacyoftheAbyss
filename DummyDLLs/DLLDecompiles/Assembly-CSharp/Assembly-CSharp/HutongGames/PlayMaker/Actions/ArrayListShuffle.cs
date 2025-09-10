using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B31 RID: 2865
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Shuffle elements from an ArrayList Proxy component")]
	public class ArrayListShuffle : ArrayListActions
	{
		// Token: 0x060059B9 RID: 22969 RVA: 0x001C6B42 File Offset: 0x001C4D42
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = 0;
			this.shufflingRange = 0;
		}

		// Token: 0x060059BA RID: 22970 RVA: 0x001C6B6A File Offset: 0x001C4D6A
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListShuffle(this.proxy.arrayList);
			}
			base.Finish();
		}

		// Token: 0x060059BB RID: 22971 RVA: 0x001C6BA8 File Offset: 0x001C4DA8
		public void DoArrayListShuffle(ArrayList source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			int num = 0;
			int num2 = this.proxy.arrayList.Count - 1;
			if (this.startIndex.Value > 0)
			{
				num = Mathf.Min(this.startIndex.Value, num2);
			}
			if (this.shufflingRange.Value > 0)
			{
				num2 = Mathf.Min(this.proxy.arrayList.Count - 1, num + this.shufflingRange.Value);
			}
			Debug.Log(num);
			Debug.Log(num2);
			for (int i = num2; i > num; i--)
			{
				int index = Random.Range(num, i + 1);
				object value = this.proxy.arrayList[i];
				this.proxy.arrayList[i] = this.proxy.arrayList[index];
				this.proxy.arrayList[index] = value;
			}
		}

		// Token: 0x04005534 RID: 21812
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to shuffle")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005535 RID: 21813
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy from ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005536 RID: 21814
		[Tooltip("Optional start Index for the shuffling. Leave it to 0 for no effect")]
		public FsmInt startIndex;

		// Token: 0x04005537 RID: 21815
		[Tooltip("Optional range for the shuffling, starting at the start index if greater than 0. Leave it to 0 for no effect, that is will shuffle the whole array")]
		public FsmInt shufflingRange;
	}
}
