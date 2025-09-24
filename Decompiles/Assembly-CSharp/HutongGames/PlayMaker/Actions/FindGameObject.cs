using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EAA RID: 3754
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Finds a Game Object by Name and/or Tag.")]
	public class FindGameObject : FsmStateAction
	{
		// Token: 0x06006A56 RID: 27222 RVA: 0x00213A17 File Offset: 0x00211C17
		public override void Reset()
		{
			this.objectName = "";
			this.withTag = "Untagged";
			this.store = null;
		}

		// Token: 0x06006A57 RID: 27223 RVA: 0x00213A40 File Offset: 0x00211C40
		public override void OnEnter()
		{
			this.Find();
			base.Finish();
		}

		// Token: 0x06006A58 RID: 27224 RVA: 0x00213A50 File Offset: 0x00211C50
		private void Find()
		{
			if (!(this.withTag.Value != "Untagged"))
			{
				this.store.Value = GameObject.Find(this.objectName.Value);
				return;
			}
			if (!string.IsNullOrEmpty(this.objectName.Value))
			{
				foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(this.withTag.Value))
				{
					if (gameObject.name == this.objectName.Value)
					{
						this.store.Value = gameObject;
						return;
					}
				}
				this.store.Value = null;
				return;
			}
			this.store.Value = GameObject.FindGameObjectWithTag(this.withTag.Value);
		}

		// Token: 0x06006A59 RID: 27225 RVA: 0x00213B15 File Offset: 0x00211D15
		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(this.objectName.Value) && string.IsNullOrEmpty(this.withTag.Value))
			{
				return "Specify Name, Tag, or both.";
			}
			return null;
		}

		// Token: 0x040069AE RID: 27054
		[Tooltip("The name of the GameObject to find. You can leave this empty if you specify a Tag.")]
		public FsmString objectName;

		// Token: 0x040069AF RID: 27055
		[UIHint(UIHint.Tag)]
		[Tooltip("Find a GameObject with this tag. If Object Name is specified then both name and Tag must match.")]
		public FsmString withTag;

		// Token: 0x040069B0 RID: 27056
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject variable.")]
		public FsmGameObject store;
	}
}
