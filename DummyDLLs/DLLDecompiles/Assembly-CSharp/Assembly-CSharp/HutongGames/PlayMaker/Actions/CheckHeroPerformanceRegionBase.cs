using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012DF RID: 4831
	public abstract class CheckHeroPerformanceRegionBase : FsmStateAction
	{
		// Token: 0x17000C1D RID: 3101
		// (get) Token: 0x06007DF4 RID: 32244
		protected abstract bool IsActive { get; }

		// Token: 0x17000C1E RID: 3102
		// (get) Token: 0x06007DF5 RID: 32245
		protected abstract Transform TargetTransform { get; }

		// Token: 0x17000C1F RID: 3103
		// (get) Token: 0x06007DF6 RID: 32246 RVA: 0x00257B6C File Offset: 0x00255D6C
		protected virtual float TargetRadius
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000C20 RID: 3104
		// (get) Token: 0x06007DF7 RID: 32247
		protected abstract float NewDelay { get; }

		// Token: 0x17000C21 RID: 3105
		// (get) Token: 0x06007DF8 RID: 32248
		protected abstract bool UseNeedolinRange { get; }

		// Token: 0x17000C22 RID: 3106
		// (get) Token: 0x06007DF9 RID: 32249
		protected abstract bool IsNoiseResponder { get; }

		// Token: 0x06007DFA RID: 32250 RVA: 0x00257B74 File Offset: 0x00255D74
		public override void OnEnter()
		{
			this.respondingToNoise = false;
			if (this.IsNoiseResponder)
			{
				NoiseMaker.NoiseCreatedInScene += this.OnNoiseCreated;
				this.subscribedNoiseRespond = true;
			}
			this.affectedState = HeroPerformanceRegion.AffectedState.None;
			this.delay = 0f;
			this.blackThreadState = this.TargetTransform.GetComponent<BlackThreadState>();
			this.hasBlackThreadState = (this.blackThreadState != null);
			this.enemySingDuration = this.TargetTransform.GetComponent<EnemySingDuration>();
		}

		// Token: 0x06007DFB RID: 32251 RVA: 0x00257BEE File Offset: 0x00255DEE
		public override void OnExit()
		{
			if (this.subscribedNoiseRespond)
			{
				NoiseMaker.NoiseCreatedInScene -= this.OnNoiseCreated;
				this.subscribedNoiseRespond = true;
			}
			this.blackThreadState = null;
			this.hasBlackThreadState = false;
			this.enemySingDuration = null;
		}

		// Token: 0x06007DFC RID: 32252 RVA: 0x00257C28 File Offset: 0x00255E28
		public override void OnUpdate()
		{
			this.DoAction(true);
			if (this.delay <= 0f)
			{
				return;
			}
			this.delay -= Time.deltaTime;
			if (this.delay <= 0f)
			{
				this.SendEvents(this.affectedState);
			}
		}

		// Token: 0x06007DFD RID: 32253 RVA: 0x00257C78 File Offset: 0x00255E78
		protected void DoAction(bool isDelayed)
		{
			if (!this.IsActive)
			{
				return;
			}
			if (this.hasBlackThreadState && this.blackThreadState.IsInForcedSing)
			{
				this.affectedState = HeroPerformanceRegion.AffectedState.ActiveInner;
				this.delay = 0f;
			}
			else
			{
				HeroPerformanceRegion.AffectedState affectedState = this.affectedState;
				this.affectedState = ((this.UseNeedolinRange && this.TargetRadius > Mathf.Epsilon) ? HeroPerformanceRegion.GetAffectedStateWithRadius(this.TargetTransform, this.TargetRadius) : HeroPerformanceRegion.GetAffectedState(this.TargetTransform, !this.UseNeedolinRange));
				if (this.hasBlackThreadState && this.blackThreadState.IsVisiblyThreaded)
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.None;
				}
				if (this.affectedState == HeroPerformanceRegion.AffectedState.ActiveInner && this.hasBlackThreadState && this.blackThreadState.IsVisiblyThreaded)
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.ActiveOuter;
				}
				if (affectedState == HeroPerformanceRegion.AffectedState.None && this.affectedState != HeroPerformanceRegion.AffectedState.None && this.enemySingDuration && !this.enemySingDuration.CheckCanSing())
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.None;
				}
				if (this.affectedState == HeroPerformanceRegion.AffectedState.None && this.respondingToNoise)
				{
					this.affectedState = HeroPerformanceRegion.AffectedState.ActiveOuter;
				}
				if ((this.affectedState != HeroPerformanceRegion.AffectedState.None && affectedState == HeroPerformanceRegion.AffectedState.None) || (this.affectedState == HeroPerformanceRegion.AffectedState.None && affectedState != HeroPerformanceRegion.AffectedState.None) || (this.affectedState == HeroPerformanceRegion.AffectedState.ActiveInner && affectedState != HeroPerformanceRegion.AffectedState.ActiveInner))
				{
					this.delay = (isDelayed ? this.NewDelay : 0f);
				}
			}
			this.OnAffectedState(this.affectedState);
			if (this.delay > 0f)
			{
				return;
			}
			this.respondingToNoise = false;
			this.SendEvents(this.affectedState);
		}

		// Token: 0x06007DFE RID: 32254
		protected abstract void OnAffectedState(HeroPerformanceRegion.AffectedState affectedState);

		// Token: 0x06007DFF RID: 32255
		protected abstract void SendEvents(HeroPerformanceRegion.AffectedState affectedState);

		// Token: 0x06007E00 RID: 32256 RVA: 0x00257DF0 File Offset: 0x00255FF0
		private void OnNoiseCreated(Vector2 noiseSourcePos, NoiseMaker.NoiseEventCheck isNoiseInRange, NoiseMaker.Intensities intensity, bool allowOffscreen)
		{
			if (!this.IsActive)
			{
				return;
			}
			Transform targetTransform = this.TargetTransform;
			if (!targetTransform)
			{
				return;
			}
			Vector2 vector = (this.TargetRadius > Mathf.Epsilon) ? HeroPerformanceRegion.GetPosInRadius(noiseSourcePos, targetTransform.position, this.TargetRadius) : targetTransform.position;
			if (!isNoiseInRange(vector))
			{
				return;
			}
			Vector2 vector2 = new Vector2(8.3f * ForceCameraAspect.CurrentViewportAspect, 8.3f);
			Vector2 vector3 = GameCameras.instance.mainCamera.transform.position;
			if (!allowOffscreen)
			{
				if (Mathf.Abs(vector.x - vector3.x) > vector2.x + 6f)
				{
					return;
				}
				if (Mathf.Abs(vector.y - vector3.y) > vector2.y + 6f)
				{
					return;
				}
			}
			this.respondingToNoise = true;
		}

		// Token: 0x04007DCD RID: 32205
		private HeroPerformanceRegion.AffectedState affectedState;

		// Token: 0x04007DCE RID: 32206
		private float delay;

		// Token: 0x04007DCF RID: 32207
		private bool respondingToNoise;

		// Token: 0x04007DD0 RID: 32208
		private bool subscribedNoiseRespond;

		// Token: 0x04007DD1 RID: 32209
		private BlackThreadState blackThreadState;

		// Token: 0x04007DD2 RID: 32210
		private EnemySingDuration enemySingDuration;

		// Token: 0x04007DD3 RID: 32211
		protected bool hasBlackThreadState;
	}
}
