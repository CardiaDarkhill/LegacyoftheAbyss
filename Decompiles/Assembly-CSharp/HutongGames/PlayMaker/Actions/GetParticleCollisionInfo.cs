using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA2 RID: 4002
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets info on the last particle collision event. See Unity Particle System docs.")]
	public class GetParticleCollisionInfo : FsmStateAction
	{
		// Token: 0x06006E99 RID: 28313 RVA: 0x00223CB2 File Offset: 0x00221EB2
		public override void Reset()
		{
			this.gameObjectHit = null;
		}

		// Token: 0x06006E9A RID: 28314 RVA: 0x00223CBB File Offset: 0x00221EBB
		private void StoreCollisionInfo()
		{
			this.gameObjectHit.Value = base.Fsm.ParticleCollisionGO;
		}

		// Token: 0x06006E9B RID: 28315 RVA: 0x00223CD3 File Offset: 0x00221ED3
		public override void OnEnter()
		{
			this.StoreCollisionInfo();
			base.Finish();
		}

		// Token: 0x04006E3B RID: 28219
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit.")]
		public FsmGameObject gameObjectHit;
	}
}
