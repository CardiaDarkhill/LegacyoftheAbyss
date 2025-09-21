using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001277 RID: 4727
	[ActionCategory("Particle System")]
	[Tooltip("Waits for particle system to stop playing")]
	public sealed class WaitParticleSystem : FsmStateAction
	{
		// Token: 0x06007C85 RID: 31877 RVA: 0x00253982 File Offset: 0x00251B82
		public override void Reset()
		{
			this.target = null;
			this.stop = null;
			this.withChildren = null;
			this.timeOut = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007C86 RID: 31878 RVA: 0x002539AC File Offset: 0x00251BAC
		public override void OnEnter()
		{
			this.particleSystem = this.target.GetSafe(this);
			bool flag = true;
			if (this.particleSystem != null && this.particleSystem.isPlaying)
			{
				if (this.stop.Value)
				{
					this.particleSystem.Stop(this.withChildren.Value, ParticleSystemStopBehavior.StopEmitting);
				}
				this.timer = 0f;
				flag = false;
			}
			if (flag)
			{
				base.Finish();
			}
		}

		// Token: 0x06007C87 RID: 31879 RVA: 0x00253A24 File Offset: 0x00251C24
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
			if (this.particleSystem == null)
			{
				base.Finish();
				return;
			}
			if (!this.particleSystem.isPlaying)
			{
				base.Finish();
			}
		}

		// Token: 0x04007C96 RID: 31894
		[RequiredField]
		[CheckForComponent(typeof(ParticleSystem))]
		public FsmOwnerDefault target;

		// Token: 0x04007C97 RID: 31895
		public FsmBool stop;

		// Token: 0x04007C98 RID: 31896
		public FsmBool withChildren;

		// Token: 0x04007C99 RID: 31897
		public FsmFloat timeOut;

		// Token: 0x04007C9A RID: 31898
		private ParticleSystem particleSystem;

		// Token: 0x04007C9B RID: 31899
		private float timer;
	}
}
