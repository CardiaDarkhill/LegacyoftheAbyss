using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AE4 RID: 2788
	public interface IFsmCollider2DStateAction
	{
		// Token: 0x0600588E RID: 22670
		void DoCollisionEnter2D(Collision2D collisionInfo);

		// Token: 0x0600588F RID: 22671
		void DoCollisionExit2D(Collision2D collisionInfo);

		// Token: 0x06005890 RID: 22672
		void DoCollisionStay2D(Collision2D collisionInfo);
	}
}
