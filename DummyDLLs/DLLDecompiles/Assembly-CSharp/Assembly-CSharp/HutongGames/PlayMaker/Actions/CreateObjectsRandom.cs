using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C06 RID: 3078
	public class CreateObjectsRandom : FsmStateAction
	{
		// Token: 0x06005E00 RID: 24064 RVA: 0x001D9F31 File Offset: 0x001D8131
		public override void Reset()
		{
			this.SpawnPool = null;
			this.xSpread = null;
			this.ySpread = null;
			this.AmountMin = null;
			this.AmountMax = null;
			this.SpawnPoint = null;
			this.Activated = true;
			this.StoreArray = null;
		}

		// Token: 0x06005E01 RID: 24065 RVA: 0x001D9F70 File Offset: 0x001D8170
		public override void OnEnter()
		{
			int length = this.SpawnPool.Length;
			int num = Random.Range(this.AmountMin.Value, this.AmountMax.Value + 1);
			this.StoreArray.Resize(num);
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(0, length);
				GameObject gameObject = this.SpawnPool.Get(index) as GameObject;
				if (gameObject)
				{
					GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
					if (this.SpawnPoint.Value != null)
					{
						gameObject2.transform.position = this.SpawnPoint.Value.transform.position;
					}
					gameObject2.transform.position = new Vector3(gameObject2.transform.position.x + Random.Range(-this.xSpread.Value, this.xSpread.Value), gameObject2.transform.position.y + Random.Range(-this.ySpread.Value, this.ySpread.Value), gameObject2.transform.position.z);
					gameObject2.SetActive(this.Activated.Value);
					this.StoreArray.Set(i, gameObject2);
				}
			}
			base.Finish();
		}

		// Token: 0x04005A59 RID: 23129
		[ArrayEditor(typeof(GameObject), "", 0, 0, 65536)]
		public FsmArray SpawnPool;

		// Token: 0x04005A5A RID: 23130
		public FsmInt AmountMin;

		// Token: 0x04005A5B RID: 23131
		public FsmInt AmountMax;

		// Token: 0x04005A5C RID: 23132
		public FsmGameObject SpawnPoint;

		// Token: 0x04005A5D RID: 23133
		public FsmFloat xSpread;

		// Token: 0x04005A5E RID: 23134
		public FsmFloat ySpread;

		// Token: 0x04005A5F RID: 23135
		public FsmBool Activated;

		// Token: 0x04005A60 RID: 23136
		[ArrayEditor(typeof(GameObject), "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray StoreArray;
	}
}
