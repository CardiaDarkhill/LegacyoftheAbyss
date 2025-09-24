using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF3 RID: 3315
	[ActionCategory("Combat")]
	[Tooltip("Detect 2D entry collisions or triggers between the Owner of this FSM and other Game Objects that have a Damager FSM.")]
	public class ReceivedNailDamage : ReceivedDamageBase
	{
		// Token: 0x06006262 RID: 25186 RVA: 0x001F21B0 File Offset: 0x001F03B0
		public override bool CustomCanRespond(HitInstance damageInstance)
		{
			return base.CustomCanRespond(damageInstance) && damageInstance.IsNailDamage;
		}
	}
}
