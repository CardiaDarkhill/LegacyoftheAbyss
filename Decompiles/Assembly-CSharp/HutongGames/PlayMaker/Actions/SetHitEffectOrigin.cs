using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A8 RID: 4776
	[ActionCategory("Hollow Knight")]
	public class SetHitEffectOrigin : FsmStateAction
	{
		// Token: 0x06007D39 RID: 32057 RVA: 0x00255B69 File Offset: 0x00253D69
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.effectOrigin = new FsmVector3();
		}

		// Token: 0x06007D3A RID: 32058 RVA: 0x00255B84 File Offset: 0x00253D84
		public override void OnEnter()
		{
			this.targetObj = ((this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value);
			if (this.targetObj != null)
			{
				this.enemyHitEffects = this.targetObj.GetComponent<EnemyHitEffectsRegular>();
				if (this.enemyHitEffects != null)
				{
					this.SetOrigin();
				}
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007D3B RID: 32059 RVA: 0x00255BFD File Offset: 0x00253DFD
		public override void OnUpdate()
		{
			this.SetOrigin();
		}

		// Token: 0x06007D3C RID: 32060 RVA: 0x00255C05 File Offset: 0x00253E05
		private void SetOrigin()
		{
			if (this.enemyHitEffects == null)
			{
				base.Finish();
			}
			this.enemyHitEffects.SetEffectOrigin(this.effectOrigin.Value);
		}

		// Token: 0x04007D38 RID: 32056
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D39 RID: 32057
		public FsmVector3 effectOrigin;

		// Token: 0x04007D3A RID: 32058
		public bool everyFrame;

		// Token: 0x04007D3B RID: 32059
		private GameObject targetObj;

		// Token: 0x04007D3C RID: 32060
		private EnemyHitEffectsRegular enemyHitEffects;
	}
}
