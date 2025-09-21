using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F9D RID: 3997
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Applies an explosion Force to all Game Objects with a Rigid Body inside a Radius.")]
	public class Explosion : FsmStateAction
	{
		// Token: 0x06006E73 RID: 28275 RVA: 0x00223531 File Offset: 0x00221731
		public override void Reset()
		{
			this.center = null;
			this.upwardsModifier = 0f;
			this.forceMode = ForceMode.Force;
			this.everyFrame = false;
		}

		// Token: 0x06006E74 RID: 28276 RVA: 0x00223558 File Offset: 0x00221758
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006E75 RID: 28277 RVA: 0x00223566 File Offset: 0x00221766
		public override void OnEnter()
		{
			this.DoExplosion();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E76 RID: 28278 RVA: 0x0022357C File Offset: 0x0022177C
		public override void OnFixedUpdate()
		{
			this.DoExplosion();
		}

		// Token: 0x06006E77 RID: 28279 RVA: 0x00223584 File Offset: 0x00221784
		private void DoExplosion()
		{
			foreach (Collider collider in Physics.OverlapSphere(this.center.Value, this.radius.Value))
			{
				Rigidbody component = collider.gameObject.GetComponent<Rigidbody>();
				if (component != null && this.ShouldApplyForce(collider.gameObject))
				{
					component.AddExplosionForce(this.force.Value, this.center.Value, this.radius.Value, this.upwardsModifier.Value, this.forceMode);
				}
			}
		}

		// Token: 0x06006E78 RID: 28280 RVA: 0x0022361C File Offset: 0x0022181C
		private bool ShouldApplyForce(GameObject go)
		{
			int num = ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value);
			return (1 << go.layer & num) > 0;
		}

		// Token: 0x04006E12 RID: 28178
		[RequiredField]
		[Tooltip("The world position of the center of the explosion.")]
		public FsmVector3 center;

		// Token: 0x04006E13 RID: 28179
		[RequiredField]
		[Tooltip("The strength of the explosion.")]
		public FsmFloat force;

		// Token: 0x04006E14 RID: 28180
		[RequiredField]
		[Tooltip("The radius of the explosion. Force falls of linearly with distance.")]
		public FsmFloat radius;

		// Token: 0x04006E15 RID: 28181
		[Tooltip("Applies the force as if it was applied from beneath the object. This is useful because explosions that throw things up instead of pushing things to the side look cooler. A value of 2 will apply a force as if it is applied from 2 meters below while not changing the actual explosion position.")]
		public FsmFloat upwardsModifier;

		// Token: 0x04006E16 RID: 28182
		[Tooltip("The type of force to apply.")]
		public ForceMode forceMode;

		// Token: 0x04006E17 RID: 28183
		[UIHint(UIHint.Layer)]
		[NonSerialized]
		public FsmInt layer;

		// Token: 0x04006E18 RID: 28184
		[UIHint(UIHint.Layer)]
		[Tooltip("Layers to effect.")]
		public FsmInt[] layerMask;

		// Token: 0x04006E19 RID: 28185
		[Tooltip("Invert the mask, so you effect all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006E1A RID: 28186
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
