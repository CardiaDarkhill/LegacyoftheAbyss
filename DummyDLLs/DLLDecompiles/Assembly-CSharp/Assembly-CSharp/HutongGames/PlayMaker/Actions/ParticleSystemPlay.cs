using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E96 RID: 3734
	[ActionCategory(ActionCategory.Effects)]
	[Tooltip("Plays a ParticleSystem. Optionally destroy the GameObject when the ParticleSystem is finished.")]
	public class ParticleSystemPlay : ComponentAction<ParticleSystem>
	{
		// Token: 0x060069FA RID: 27130 RVA: 0x0021247C File Offset: 0x0021067C
		public override void Reset()
		{
			this.gameObject = null;
			this.withChildren = null;
			this.cacheChildren = null;
			this.destroyOnFinish = null;
		}

		// Token: 0x060069FB RID: 27131 RVA: 0x0021249C File Offset: 0x0021069C
		public override void Awake()
		{
			if (!this.withChildren.Value || !this.cacheChildren.Value)
			{
				return;
			}
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(this.go))
			{
				this.childParticleSystems = this.go.GetComponentsInChildren<ParticleSystem>();
			}
		}

		// Token: 0x060069FC RID: 27132 RVA: 0x002124FA File Offset: 0x002106FA
		public override void OnEnter()
		{
			this.DoParticleSystemPlay();
			if (!this.destroyOnFinish.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x060069FD RID: 27133 RVA: 0x00212518 File Offset: 0x00210718
		public override void OnExit()
		{
			if (!this.stopOnExit.Value)
			{
				return;
			}
			if (this.withChildren.Value && this.cacheChildren.Value)
			{
				this.cachedComponent.Stop(false);
				for (int i = 0; i < this.childParticleSystems.Length; i++)
				{
					ParticleSystem particleSystem = this.childParticleSystems[i];
					if (particleSystem != null)
					{
						particleSystem.Stop(false);
					}
				}
				return;
			}
			this.cachedComponent.Stop(this.withChildren.Value);
		}

		// Token: 0x060069FE RID: 27134 RVA: 0x0021259C File Offset: 0x0021079C
		public override void OnUpdate()
		{
			if (this.withChildren.Value && this.cacheChildren.Value)
			{
				if (this.cachedComponent.IsAlive(false))
				{
					return;
				}
				for (int i = 0; i < this.childParticleSystems.Length; i++)
				{
					ParticleSystem particleSystem = this.childParticleSystems[i];
					if (particleSystem != null && particleSystem.IsAlive(false))
					{
						return;
					}
				}
			}
			else if (this.cachedComponent.IsAlive(this.withChildren.Value))
			{
				return;
			}
			Object.Destroy(this.go);
			base.Finish();
		}

		// Token: 0x060069FF RID: 27135 RVA: 0x00212630 File Offset: 0x00210830
		private void DoParticleSystemPlay()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(this.go))
			{
				return;
			}
			if (this.withChildren.Value && this.cacheChildren.Value)
			{
				this.cachedComponent.Play(false);
				for (int i = 0; i < this.childParticleSystems.Length; i++)
				{
					ParticleSystem particleSystem = this.childParticleSystems[i];
					if (particleSystem != null)
					{
						particleSystem.Play(false);
					}
				}
				return;
			}
			this.cachedComponent.Play(this.withChildren.Value);
		}

		// Token: 0x04006954 RID: 26964
		[RequiredField]
		[Tooltip("The GameObject with the ParticleSystem.")]
		[CheckForComponent(typeof(ParticleSystem))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006955 RID: 26965
		[Tooltip("Play ParticleSystems on all child GameObjects too.")]
		public FsmBool withChildren;

		// Token: 0x04006956 RID: 26966
		[Tooltip("''With Children'' can be quite expensive since it has to find Particle Systems in all children. If the hierarchy doesn't change, use Cache Children to speed this up.")]
		public FsmBool cacheChildren;

		// Token: 0x04006957 RID: 26967
		[Tooltip("Stop playing when state exits")]
		public FsmBool stopOnExit;

		// Token: 0x04006958 RID: 26968
		[Tooltip("Destroy the GameObject when the ParticleSystem has finished playing. 'With Children' means all child particle systems also need to finish.")]
		public FsmBool destroyOnFinish;

		// Token: 0x04006959 RID: 26969
		private GameObject go;

		// Token: 0x0400695A RID: 26970
		private ParticleSystem[] childParticleSystems;
	}
}
