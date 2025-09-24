using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012AC RID: 4780
	[ActionCategory("Hollow Knight")]
	public class DirectionalInvincibility : FsmStateAction
	{
		// Token: 0x06007D4B RID: 32075 RVA: 0x00255F19 File Offset: 0x00254119
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.xPositiveDirection = null;
			this.xNegativeDirection = null;
		}

		// Token: 0x06007D4C RID: 32076 RVA: 0x00255F34 File Offset: 0x00254134
		public override void OnEnter()
		{
			this.targetGo = this.target.GetSafe(this);
			if (this.targetGo != null)
			{
				this.tform = this.targetGo.transform;
				this.healthManager = this.targetGo.GetComponent<HealthManager>();
				if (this.healthManager != null)
				{
					this.healthManager.IsInvincible = true;
				}
			}
			if (this.tform.localScale.x > 0f)
			{
				this.healthManager.InvincibleFromDirection = this.xPositiveDirection.Value;
				this.shieldingPositive = true;
				return;
			}
			this.healthManager.InvincibleFromDirection = this.xNegativeDirection.Value;
			this.shieldingPositive = false;
		}

		// Token: 0x06007D4D RID: 32077 RVA: 0x00255FF0 File Offset: 0x002541F0
		public override void OnUpdate()
		{
			float x = this.tform.localScale.x;
			if (x > 0f && !this.shieldingPositive)
			{
				this.healthManager.InvincibleFromDirection = this.xPositiveDirection.Value;
				this.shieldingPositive = true;
				return;
			}
			if (x < 0f && this.shieldingPositive)
			{
				this.healthManager.InvincibleFromDirection = this.xNegativeDirection.Value;
				this.shieldingPositive = false;
			}
		}

		// Token: 0x06007D4E RID: 32078 RVA: 0x00256069 File Offset: 0x00254269
		public override void OnExit()
		{
			if (this.healthManager != null)
			{
				this.healthManager.InvincibleFromDirection = 0;
				this.healthManager.IsInvincible = false;
			}
		}

		// Token: 0x04007D49 RID: 32073
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D4A RID: 32074
		public FsmInt xPositiveDirection;

		// Token: 0x04007D4B RID: 32075
		public FsmInt xNegativeDirection;

		// Token: 0x04007D4C RID: 32076
		private bool shieldingPositive;

		// Token: 0x04007D4D RID: 32077
		private GameObject targetGo;

		// Token: 0x04007D4E RID: 32078
		private Transform tform;

		// Token: 0x04007D4F RID: 32079
		private HealthManager healthManager;
	}
}
