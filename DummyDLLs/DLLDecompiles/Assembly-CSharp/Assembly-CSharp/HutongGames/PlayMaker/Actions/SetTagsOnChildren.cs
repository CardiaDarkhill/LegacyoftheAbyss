using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC2 RID: 3778
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Set the Tag on all children of a GameObject. Optionally filter by component.")]
	public class SetTagsOnChildren : FsmStateAction
	{
		// Token: 0x06006ABD RID: 27325 RVA: 0x00214AF3 File Offset: 0x00212CF3
		public override void Reset()
		{
			this.gameObject = null;
			this.tag = null;
			this.filterByComponent = null;
		}

		// Token: 0x06006ABE RID: 27326 RVA: 0x00214B0A File Offset: 0x00212D0A
		public override void OnEnter()
		{
			this.SetTag(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x06006ABF RID: 27327 RVA: 0x00214B2C File Offset: 0x00212D2C
		private void SetTag(GameObject parent)
		{
			if (parent == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.filterByComponent.Value))
			{
				using (IEnumerator enumerator = parent.transform.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						((Transform)obj).gameObject.tag = this.tag.Value;
					}
					goto IL_AC;
				}
			}
			this.UpdateComponentFilter();
			if (this.componentFilter != null)
			{
				Component[] componentsInChildren = parent.GetComponentsInChildren(this.componentFilter);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].gameObject.tag = this.tag.Value;
				}
			}
			IL_AC:
			base.Finish();
		}

		// Token: 0x06006AC0 RID: 27328 RVA: 0x00214BFC File Offset: 0x00212DFC
		private void UpdateComponentFilter()
		{
			this.componentFilter = ReflectionUtils.GetGlobalType(this.filterByComponent.Value);
			if (this.componentFilter == null)
			{
				this.componentFilter = ReflectionUtils.GetGlobalType("UnityEngine." + this.filterByComponent.Value);
			}
			if (this.componentFilter == null)
			{
				Debug.LogWarning("Couldn't get type: " + this.filterByComponent.Value);
			}
		}

		// Token: 0x04006A00 RID: 27136
		[RequiredField]
		[Tooltip("GameObject Parent")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006A01 RID: 27137
		[RequiredField]
		[UIHint(UIHint.Tag)]
		[Tooltip("Set Tag To...")]
		public FsmString tag;

		// Token: 0x04006A02 RID: 27138
		[UIHint(UIHint.ScriptComponent)]
		[Tooltip("Only set the Tag on children with this component.")]
		public FsmString filterByComponent;

		// Token: 0x04006A03 RID: 27139
		private Type componentFilter;
	}
}
