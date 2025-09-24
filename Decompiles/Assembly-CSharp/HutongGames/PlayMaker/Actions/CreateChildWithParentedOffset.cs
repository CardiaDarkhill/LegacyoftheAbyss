using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C05 RID: 3077
	[Tooltip("Instantiate a child with a new parent to maintain localPosition while being centred (fix for some cases where object is animated at some arbitrary world position)")]
	public class CreateChildWithParentedOffset : FsmStateAction
	{
		// Token: 0x06005DFD RID: 24061 RVA: 0x001D9E20 File Offset: 0x001D8020
		public override void Reset()
		{
			this.Parent = null;
			this.Offset = null;
			this.Prefab = null;
			this.StoreObject = null;
		}

		// Token: 0x06005DFE RID: 24062 RVA: 0x001D9E40 File Offset: 0x001D8040
		public override void OnEnter()
		{
			GameObject safe = this.Parent.GetSafe(this);
			GameObject value = this.Prefab.Value;
			if (value == null)
			{
				base.Finish();
				return;
			}
			GameObject gameObject = new GameObject(string.Format("{0} Offset Parent", value.name));
			if (safe != null)
			{
				gameObject.transform.SetParent(safe.transform);
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(value);
			Animator component = gameObject2.GetComponent<Animator>();
			if (component)
			{
				component.Update(0f);
			}
			Vector3 localPosition = gameObject2.transform.localPosition;
			gameObject2.transform.SetParent(gameObject.transform);
			gameObject2.transform.localPosition = localPosition;
			gameObject.transform.localPosition = -localPosition + this.Offset.Value;
			this.StoreObject.Value = gameObject2;
			base.Finish();
		}

		// Token: 0x04005A55 RID: 23125
		public FsmOwnerDefault Parent;

		// Token: 0x04005A56 RID: 23126
		public FsmVector3 Offset;

		// Token: 0x04005A57 RID: 23127
		public FsmGameObject Prefab;

		// Token: 0x04005A58 RID: 23128
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;
	}
}
