using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EB6 RID: 3766
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
	public class GetRandomObject : FsmStateAction
	{
		// Token: 0x06006A8B RID: 27275 RVA: 0x0021440F File Offset: 0x0021260F
		public override void Reset()
		{
			this.withTag = "Untagged";
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A8C RID: 27276 RVA: 0x0021442F File Offset: 0x0021262F
		public override void OnEnter()
		{
			this.DoGetRandomObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A8D RID: 27277 RVA: 0x00214445 File Offset: 0x00212645
		public override void OnUpdate()
		{
			this.DoGetRandomObject();
		}

		// Token: 0x06006A8E RID: 27278 RVA: 0x00214450 File Offset: 0x00212650
		private void DoGetRandomObject()
		{
			GameObject[] array;
			if (this.withTag.Value != "Untagged")
			{
				array = GameObject.FindGameObjectsWithTag(this.withTag.Value);
			}
			else
			{
				array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			}
			if (array.Length != 0)
			{
				this.storeResult.Value = array[Random.Range(0, array.Length)];
				return;
			}
			this.storeResult.Value = null;
		}

		// Token: 0x040069DB RID: 27099
		[UIHint(UIHint.Tag)]
		[Tooltip("Only select from Game Objects with this Tag.")]
		public FsmString withTag;

		// Token: 0x040069DC RID: 27100
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a GameObject Variable.")]
		public FsmGameObject storeResult;

		// Token: 0x040069DD RID: 27101
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
