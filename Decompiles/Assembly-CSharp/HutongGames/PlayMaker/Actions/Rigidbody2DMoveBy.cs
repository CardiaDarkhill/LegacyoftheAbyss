using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D00 RID: 3328
	public class Rigidbody2DMoveBy : FsmStateAction
	{
		// Token: 0x06006293 RID: 25235 RVA: 0x001F2BD4 File Offset: 0x001F0DD4
		public override void Reset()
		{
			this.Target = null;
			this.Offset = null;
			this.Space = Space.World;
		}

		// Token: 0x06006294 RID: 25236 RVA: 0x001F2BEC File Offset: 0x001F0DEC
		public override void OnEnter()
		{
			Rigidbody2D component = this.Target.GetSafe(this).GetComponent<Rigidbody2D>();
			Vector2 vector = component.position;
			if (this.Space == Space.Self)
			{
				Vector2 b = component.transform.TransformVector(this.Offset.Value);
				vector += b;
			}
			else
			{
				vector += this.Offset.Value;
			}
			component.MovePosition(vector);
			base.Finish();
		}

		// Token: 0x040060FB RID: 24827
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		// Token: 0x040060FC RID: 24828
		public FsmVector2 Offset;

		// Token: 0x040060FD RID: 24829
		public Space Space;
	}
}
