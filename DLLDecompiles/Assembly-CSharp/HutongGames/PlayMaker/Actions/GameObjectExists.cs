using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C53 RID: 3155
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Checks to see if a Game Object exists in the scene by Name and/or Tag.")]
	public class GameObjectExists : FsmStateAction
	{
		// Token: 0x06005F96 RID: 24470 RVA: 0x001E53DD File Offset: 0x001E35DD
		public override void Reset()
		{
			this.objectName = "";
			this.withTag = "Untagged";
			this.result = null;
		}

		// Token: 0x06005F97 RID: 24471 RVA: 0x001E5408 File Offset: 0x001E3608
		public override void OnEnter()
		{
			base.Finish();
			if (!(this.withTag.Value != "Untagged"))
			{
				if (GameObject.Find(this.objectName.Value) == null)
				{
					this.result.Value = true;
				}
				return;
			}
			if (!string.IsNullOrEmpty(this.objectName.Value))
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(this.withTag.Value);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].name == this.objectName.Value)
					{
						this.result.Value = true;
						return;
					}
				}
				this.result.Value = false;
				return;
			}
			if (GameObject.FindGameObjectWithTag(this.withTag.Value) != null)
			{
				this.result.Value = true;
				return;
			}
			this.result.Value = false;
		}

		// Token: 0x06005F98 RID: 24472 RVA: 0x001E54F0 File Offset: 0x001E36F0
		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(this.objectName.Value) && string.IsNullOrEmpty(this.withTag.Value))
			{
				return "Please specify Name, Tag, or both for the object you are looking for.";
			}
			return null;
		}

		// Token: 0x04005CEF RID: 23791
		[Tooltip("The name of the GameObject to find. You can leave this empty if you specify a Tag.")]
		public FsmString objectName;

		// Token: 0x04005CF0 RID: 23792
		[UIHint(UIHint.Tag)]
		[Tooltip("Find a GameObject with this tag. If Object Name is specified then both name and Tag must match.")]
		public FsmString withTag;

		// Token: 0x04005CF1 RID: 23793
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a boolean variable.")]
		public FsmBool result;
	}
}
