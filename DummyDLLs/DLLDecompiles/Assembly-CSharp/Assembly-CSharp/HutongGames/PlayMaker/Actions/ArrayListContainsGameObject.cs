using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B50 RID: 2896
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Check if a GameObject ( by name and/or tag) is within an arrayList.")]
	public class ArrayListContainsGameObject : ArrayListActions
	{
		// Token: 0x06005A35 RID: 23093 RVA: 0x001C8546 File Offset: 0x001C6746
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.gameObjectName = null;
			this.result = null;
			this.resultIndex = null;
			this.isContained = null;
			this.isContainedEvent = null;
			this.isNotContainedEvent = null;
		}

		// Token: 0x06005A36 RID: 23094 RVA: 0x001C8580 File Offset: 0x001C6780
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			int num = this.DoContainsGo();
			if (num >= 0)
			{
				this.isContained.Value = true;
				this.result.Value = (GameObject)this.proxy.arrayList[num];
				this.resultIndex.Value = num;
				base.Fsm.Event(this.isContainedEvent);
			}
			else
			{
				this.isContained.Value = false;
				base.Fsm.Event(this.isNotContainedEvent);
			}
			base.Finish();
		}

		// Token: 0x06005A37 RID: 23095 RVA: 0x001C8634 File Offset: 0x001C6834
		private int DoContainsGo()
		{
			if (!base.isProxyValid())
			{
				return -1;
			}
			int num = 0;
			string value = this.gameObjectName.Value;
			string value2 = this.withTag.Value;
			foreach (object obj in this.proxy.arrayList)
			{
				GameObject gameObject = (GameObject)obj;
				if (gameObject != null)
				{
					if (value2 == "Untagged" || this.withTag.IsNone)
					{
						if (gameObject.name.Equals(value))
						{
							return num;
						}
					}
					else if (string.IsNullOrEmpty(value))
					{
						if (gameObject.tag.Equals(value2))
						{
							return num;
						}
					}
					else if (gameObject.name.Equals(value) && gameObject.tag.Equals(value2))
					{
						return num;
					}
				}
				num++;
			}
			return -1;
		}

		// Token: 0x040055C2 RID: 21954
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055C3 RID: 21955
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055C4 RID: 21956
		[Tooltip("The name of the GameObject to find in the arrayList. You can leave this empty if you specify a Tag.")]
		public FsmString gameObjectName;

		// Token: 0x040055C5 RID: 21957
		[UIHint(UIHint.Tag)]
		[Tooltip("Find a GameObject in this arrayList with this tag. If GameObject Name is specified then both name and Tag must match.")]
		public FsmString withTag;

		// Token: 0x040055C6 RID: 21958
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject variable.")]
		public FsmGameObject result;

		// Token: 0x040055C7 RID: 21959
		[UIHint(UIHint.Variable)]
		public FsmInt resultIndex;

		// Token: 0x040055C8 RID: 21960
		[Tooltip("Store in a bool wether it contains or not that GameObject")]
		[UIHint(UIHint.Variable)]
		public FsmBool isContained;

		// Token: 0x040055C9 RID: 21961
		[Tooltip("Event sent if this arraList contains that GameObject")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isContainedEvent;

		// Token: 0x040055CA RID: 21962
		[Tooltip("Event sent if this arraList does not contains that GameObject")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent isNotContainedEvent;
	}
}
