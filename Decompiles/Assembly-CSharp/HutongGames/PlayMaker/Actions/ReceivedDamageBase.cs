using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF4 RID: 3316
	public abstract class ReceivedDamageBase : FsmStateAction
	{
		// Token: 0x06006264 RID: 25188 RVA: 0x001F21CC File Offset: 0x001F03CC
		public override void Reset()
		{
			this.Target = null;
			this.collideTag = new FsmString
			{
				UseVariable = true
			};
			this.sendEvent = null;
			this.sendEventHeavy = null;
			this.sendEventSpikes = null;
			this.sendEventLightning = null;
			this.storeGameObject = null;
			this.ignoreAcid = null;
			this.ignoreLava = null;
			this.ignoreWater = null;
			this.ignoreHunterWeapon = null;
			this.ignoreTraps = null;
			this.ignoreNail = null;
			this.ignoreSpikes = null;
			this.storeDamageDealt = null;
			this.storeDirection = null;
			this.storeMagnitudeMultiplier = null;
			this.firstHitOnly = null;
		}

		// Token: 0x06006265 RID: 25189 RVA: 0x001F2264 File Offset: 0x001F0464
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.proxy = (safe.GetComponent<ReceivedDamageProxy>() ?? safe.AddComponent<ReceivedDamageProxy>());
			this.proxy.AddHandler(this);
		}

		// Token: 0x06006266 RID: 25190 RVA: 0x001F22A0 File Offset: 0x001F04A0
		public override void OnExit()
		{
			this.proxy.RemoveHandler(this);
		}

		// Token: 0x06006267 RID: 25191 RVA: 0x001F22B0 File Offset: 0x001F04B0
		public bool CanRespondToHit(HitInstance damageInstance)
		{
			AttackTypes attackType = damageInstance.AttackType;
			GameObject source = damageInstance.Source;
			return source && (!this.firstHitOnly.Value || damageInstance.IsFirstHit) && (this.collideTag.IsNone || string.IsNullOrEmpty(this.collideTag.Value) || source.CompareTag(this.collideTag.Value)) && (!this.ignoreHunterWeapon.Value || attackType != AttackTypes.Hunter) && (!this.ignoreTraps.Value || attackType != AttackTypes.Trap) && (!this.ignoreLava.Value || attackType != AttackTypes.Lava) && (!this.ignoreNail.Value || attackType != AttackTypes.Nail) && (!this.ignoreSpikes.Value || attackType != AttackTypes.Spikes) && (!this.ignoreAcid.Value || !source.CompareTag("Acid")) && (!this.ignoreWater.Value || !source.CompareTag("Water Surface")) && this.CustomCanRespond(damageInstance);
		}

		// Token: 0x06006268 RID: 25192 RVA: 0x001F23B9 File Offset: 0x001F05B9
		public virtual bool CustomCanRespond(HitInstance damageInstance)
		{
			return damageInstance.DamageDealt > 0 && (!damageInstance.IsManualTrigger || damageInstance.IsNailDamage);
		}

		// Token: 0x06006269 RID: 25193 RVA: 0x001F23D8 File Offset: 0x001F05D8
		public bool RespondToHit(HitInstance damageInstance)
		{
			if (!this.CanRespondToHit(damageInstance))
			{
				return false;
			}
			AttackTypes attackType = damageInstance.AttackType;
			GameObject source = damageInstance.Source;
			this.storeGameObject.Value = source;
			this.storeDamageDealt.Value = damageInstance.DamageDealt;
			this.storeDirection.Value = damageInstance.Direction;
			this.storeMagnitudeMultiplier.Value = damageInstance.MagnitudeMultiplier;
			bool flag = false;
			if (attackType <= AttackTypes.Lava)
			{
				if (attackType != AttackTypes.Spell)
				{
					if (attackType != AttackTypes.Lava)
					{
						goto IL_B4;
					}
					base.Fsm.Event(this.sendEventLava);
					goto IL_B4;
				}
			}
			else
			{
				if (attackType == AttackTypes.Lightning)
				{
					flag = true;
					goto IL_B4;
				}
				if (attackType != AttackTypes.Heavy)
				{
					if (attackType != AttackTypes.Spikes)
					{
						goto IL_B4;
					}
					base.Fsm.Event(this.sendEventSpikes);
					goto IL_B4;
				}
			}
			base.Fsm.Event(this.sendEventHeavy);
			IL_B4:
			if (damageInstance.ZapDamageTicks > 0)
			{
				flag = true;
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEventLightning);
			}
			base.Fsm.Event(this.sendEvent);
			return true;
		}

		// Token: 0x0600626A RID: 25194 RVA: 0x001F24CC File Offset: 0x001F06CC
		public override string ErrorCheck()
		{
			string text = string.Empty;
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null && safe.GetComponent<Collider2D>() == null && safe.GetComponent<Rigidbody2D>() == null)
			{
				text += "Target requires a RigidBody2D or Collider2D!\n";
			}
			return text;
		}

		// Token: 0x040060C4 RID: 24772
		public FsmOwnerDefault Target;

		// Token: 0x040060C5 RID: 24773
		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString collideTag;

		// Token: 0x040060C6 RID: 24774
		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x040060C7 RID: 24775
		public FsmEvent sendEventHeavy;

		// Token: 0x040060C8 RID: 24776
		public FsmEvent sendEventSpikes;

		// Token: 0x040060C9 RID: 24777
		public FsmEvent sendEventLava;

		// Token: 0x040060CA RID: 24778
		public FsmEvent sendEventLightning;

		// Token: 0x040060CB RID: 24779
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		public FsmGameObject storeGameObject;

		// Token: 0x040060CC RID: 24780
		public FsmBool ignoreAcid;

		// Token: 0x040060CD RID: 24781
		public FsmBool ignoreLava;

		// Token: 0x040060CE RID: 24782
		public FsmBool ignoreWater;

		// Token: 0x040060CF RID: 24783
		public FsmBool ignoreHunterWeapon;

		// Token: 0x040060D0 RID: 24784
		public FsmBool ignoreTraps;

		// Token: 0x040060D1 RID: 24785
		public FsmBool ignoreNail;

		// Token: 0x040060D2 RID: 24786
		public FsmBool ignoreSpikes;

		// Token: 0x040060D3 RID: 24787
		[UIHint(UIHint.Variable)]
		public FsmInt storeDamageDealt;

		// Token: 0x040060D4 RID: 24788
		[UIHint(UIHint.Variable)]
		public FsmFloat storeDirection;

		// Token: 0x040060D5 RID: 24789
		[UIHint(UIHint.Variable)]
		public FsmFloat storeMagnitudeMultiplier;

		// Token: 0x040060D6 RID: 24790
		public FsmBool firstHitOnly;

		// Token: 0x040060D7 RID: 24791
		private ReceivedDamageProxy proxy;
	}
}
