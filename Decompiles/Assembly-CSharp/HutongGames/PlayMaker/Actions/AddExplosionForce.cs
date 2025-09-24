using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F99 RID: 3993
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Applies a force to a Game Object that simulates explosion effects. The explosion force will fall off linearly with distance. Hint: Use the Explosion Action instead to apply an explosion force to all objects in a blast radius.")]
	public class AddExplosionForce : ComponentAction<Rigidbody>
	{
		// Token: 0x06006E4C RID: 28236 RVA: 0x00222B88 File Offset: 0x00220D88
		public override void Reset()
		{
			this.gameObject = null;
			this.center = new FsmVector3
			{
				UseVariable = true
			};
			this.upwardsModifier = 0f;
			this.forceMode = ForceMode.Force;
			this.everyFrame = false;
		}

		// Token: 0x06006E4D RID: 28237 RVA: 0x00222BC1 File Offset: 0x00220DC1
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006E4E RID: 28238 RVA: 0x00222BCF File Offset: 0x00220DCF
		public override void OnEnter()
		{
			this.DoAddExplosionForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E4F RID: 28239 RVA: 0x00222BE5 File Offset: 0x00220DE5
		public override void OnFixedUpdate()
		{
			this.DoAddExplosionForce();
		}

		// Token: 0x06006E50 RID: 28240 RVA: 0x00222BF0 File Offset: 0x00220DF0
		private void DoAddExplosionForce()
		{
			GameObject go = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (this.center == null || !base.UpdateCache(go))
			{
				return;
			}
			base.rigidbody.AddExplosionForce(this.force.Value, this.center.Value, this.radius.Value, this.upwardsModifier.Value, this.forceMode);
		}

		// Token: 0x04006DF3 RID: 28147
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject to add the explosion force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006DF4 RID: 28148
		[RequiredField]
		[Tooltip("The center of the explosion. Hint: this is often the position returned from a GetCollisionInfo action.")]
		public FsmVector3 center;

		// Token: 0x04006DF5 RID: 28149
		[RequiredField]
		[Tooltip("The strength of the explosion.")]
		public FsmFloat force;

		// Token: 0x04006DF6 RID: 28150
		[RequiredField]
		[Tooltip("The radius of the explosion. Force falls off linearly with distance.")]
		public FsmFloat radius;

		// Token: 0x04006DF7 RID: 28151
		[Tooltip("Applies the force as if it was applied from beneath the object. This is useful because explosions that throw things up instead of pushing things to the side look cooler. A value of 2 will apply a force as if it is applied from 2 meters below while not changing the actual explosion position.")]
		public FsmFloat upwardsModifier;

		// Token: 0x04006DF8 RID: 28152
		[Tooltip("The type of force to apply. See Unity Physics docs.")]
		public ForceMode forceMode;

		// Token: 0x04006DF9 RID: 28153
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
