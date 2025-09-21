using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001278 RID: 4728
	[ActionCategory("Particle System")]
	[Tooltip("Waits for particle system to stop playing")]
	public sealed class WaitParticleSystemsInChildren : FsmStateAction
	{
		// Token: 0x06007C89 RID: 31881 RVA: 0x00253A9A File Offset: 0x00251C9A
		public override void Reset()
		{
			this.target = null;
			this.stop = null;
			this.stopLoops = null;
			this.withChildren = null;
			this.timeOut = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007C8A RID: 31882 RVA: 0x00253ACC File Offset: 0x00251CCC
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			bool flag = true;
			if (safe != null)
			{
				this.particleSystems.Clear();
				this.particleSystems.AddRange(safe.GetComponentsInChildren<ParticleSystem>());
				if (this.particleSystems.Count > 0)
				{
					flag = false;
					if (this.stop.Value)
					{
						using (List<ParticleSystem>.Enumerator enumerator = this.particleSystems.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ParticleSystem particleSystem = enumerator.Current;
								particleSystem.Stop(this.withChildren.Value, ParticleSystemStopBehavior.StopEmitting);
							}
							goto IL_F5;
						}
					}
					if (this.stopLoops.Value)
					{
						foreach (ParticleSystem particleSystem2 in this.particleSystems)
						{
							if (particleSystem2.main.loop)
							{
								particleSystem2.Stop(this.withChildren.Value, ParticleSystemStopBehavior.StopEmitting);
							}
						}
					}
				}
			}
			IL_F5:
			if (flag)
			{
				base.Finish();
			}
		}

		// Token: 0x06007C8B RID: 31883 RVA: 0x00253BF4 File Offset: 0x00251DF4
		public override void OnExit()
		{
			this.particleSystems.Clear();
		}

		// Token: 0x06007C8C RID: 31884 RVA: 0x00253C04 File Offset: 0x00251E04
		public override void OnUpdate()
		{
			if (!this.timeOut.IsNone)
			{
				this.timer += Time.deltaTime;
				if (this.timer >= this.timeOut.Value)
				{
					base.Finish();
					return;
				}
			}
			this.particleSystems.RemoveAll((ParticleSystem o) => o == null || !o.isPlaying);
			if (this.particleSystems.Count == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x04007C9C RID: 31900
		[RequiredField]
		public FsmOwnerDefault target;

		// Token: 0x04007C9D RID: 31901
		public FsmBool stop;

		// Token: 0x04007C9E RID: 31902
		public FsmBool stopLoops;

		// Token: 0x04007C9F RID: 31903
		public FsmBool withChildren;

		// Token: 0x04007CA0 RID: 31904
		public FsmFloat timeOut;

		// Token: 0x04007CA1 RID: 31905
		private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

		// Token: 0x04007CA2 RID: 31906
		private float timer;
	}
}
