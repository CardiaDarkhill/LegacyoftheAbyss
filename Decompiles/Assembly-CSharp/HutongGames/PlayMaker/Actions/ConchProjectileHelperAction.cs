using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001231 RID: 4657
	public sealed class ConchProjectileHelperAction : FsmStateAction
	{
		// Token: 0x06007B59 RID: 31577 RVA: 0x0024F615 File Offset: 0x0024D815
		public override void Reset()
		{
			this.target = null;
		}

		// Token: 0x06007B5A RID: 31578 RVA: 0x0024F61E File Offset: 0x0024D81E
		public override void OnEnter()
		{
			this.conchProjectileCollision = this.target.GetSafe(this);
			if (this.conchProjectileCollision != null)
			{
				this.conchProjectileCollision.SetDirection(this.direction.Value);
			}
		}

		// Token: 0x06007B5B RID: 31579 RVA: 0x0024F656 File Offset: 0x0024D856
		public override void OnExit()
		{
			if (this.conchProjectileCollision != null)
			{
				this.conchProjectileCollision.StateExited();
			}
		}

		// Token: 0x04007B9D RID: 31645
		[ObjectType(typeof(ConchProjectileCollision))]
		public FsmOwnerDefault target;

		// Token: 0x04007B9E RID: 31646
		public FsmVector2 direction;

		// Token: 0x04007B9F RID: 31647
		private ConchProjectileCollision conchProjectileCollision;
	}
}
