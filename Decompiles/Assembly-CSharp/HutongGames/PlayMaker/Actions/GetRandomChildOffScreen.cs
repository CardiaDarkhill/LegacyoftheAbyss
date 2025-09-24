using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7D RID: 3197
	public class GetRandomChildOffScreen : FsmStateAction
	{
		// Token: 0x06006048 RID: 24648 RVA: 0x001E7986 File Offset: 0x001E5B86
		public override void Reset()
		{
			this.Target = null;
			this.ViewportPadding = null;
			this.StoreObject = null;
		}

		// Token: 0x06006049 RID: 24649 RVA: 0x001E79A0 File Offset: 0x001E5BA0
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			Camera main = Camera.main;
			this.StoreObject.Value = null;
			if (safe && main)
			{
				Vector2 value = this.ViewportPadding.Value;
				Vector2 vector = new Vector2(-value.x, -value.y);
				Vector2 vector2 = new Vector2(1f + value.x, 1f + value.y);
				this.possibleTransforms.Clear();
				foreach (object obj in safe.transform)
				{
					Transform transform = (Transform)obj;
					Vector2 vector3 = main.WorldToViewportPoint(transform.position);
					if (vector3.x <= vector.x || vector3.x >= vector2.x || vector3.y <= vector.y || vector3.y >= vector2.y)
					{
						this.possibleTransforms.Add(transform);
					}
				}
				if (this.possibleTransforms.Count > 0)
				{
					this.StoreObject.Value = this.possibleTransforms[Random.Range(0, this.possibleTransforms.Count - 1)].gameObject;
				}
			}
			base.Finish();
		}

		// Token: 0x04005D9D RID: 23965
		public FsmOwnerDefault Target;

		// Token: 0x04005D9E RID: 23966
		public FsmVector2 ViewportPadding;

		// Token: 0x04005D9F RID: 23967
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreObject;

		// Token: 0x04005DA0 RID: 23968
		private List<Transform> possibleTransforms = new List<Transform>();
	}
}
