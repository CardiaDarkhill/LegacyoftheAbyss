using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D62 RID: 3426
	[ActionCategory("Hollow Knight")]
	[Tooltip("Randomly shakes a GameObject's position by a diminishing amount over time.")]
	public class ShakePositionV2 : FsmStateAction
	{
		// Token: 0x0600642C RID: 25644 RVA: 0x001F8CE0 File Offset: 0x001F6EE0
		public override void Reset()
		{
			base.Reset();
			this.Target = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.UseOwner
			};
			this.Extents = null;
			this.Duration = 1f;
			this.IsLooping = false;
			this.StopEvent = null;
			this.FpsLimit = null;
			this.IsCameraShake = null;
		}

		// Token: 0x0600642D RID: 25645 RVA: 0x001F8D40 File Offset: 0x001F6F40
		public override void OnEnter()
		{
			base.OnEnter();
			this.timer = 0f;
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				this.target = safe.transform;
				this.startingWorldPosition = this.target.position;
			}
			else
			{
				this.target = null;
			}
			this.UpdateShaking();
		}

		// Token: 0x0600642E RID: 25646 RVA: 0x001F8DA0 File Offset: 0x001F6FA0
		public override void OnUpdate()
		{
			base.OnUpdate();
			this.UpdateShaking();
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x001F8DAE File Offset: 0x001F6FAE
		public override void OnExit()
		{
			this.StopAndReset();
			base.OnExit();
		}

		// Token: 0x06006430 RID: 25648 RVA: 0x001F8DBC File Offset: 0x001F6FBC
		private void UpdateShaking()
		{
			if (this.target != null)
			{
				this.timer += Time.deltaTime;
				if (this.FpsLimit.Value > 0f)
				{
					if (Time.unscaledTimeAsDouble < this.nextUpdateTime)
					{
						return;
					}
					this.nextUpdateTime = Time.unscaledTimeAsDouble + (double)(1f / this.FpsLimit.Value);
				}
				bool value = this.IsLooping.Value;
				float num = value ? 1f : Mathf.Clamp01(1f - this.timer / this.Duration.Value);
				if (this.IsCameraShake.Value)
				{
					num *= CameraShakeManager.ShakeMultiplier;
				}
				Vector3 a = Vector3.Scale(this.Extents.Value, new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
				this.target.position = this.startingWorldPosition + a * num;
				if (!value && this.timer > this.Duration.Value)
				{
					this.StopAndReset();
					base.Fsm.Event(this.StopEvent);
					base.Finish();
					return;
				}
			}
			else
			{
				this.StopAndReset();
				base.Fsm.Event(this.StopEvent);
				base.Finish();
			}
		}

		// Token: 0x06006431 RID: 25649 RVA: 0x001F8F22 File Offset: 0x001F7122
		private void StopAndReset()
		{
			if (this.target != null)
			{
				this.target.position = this.startingWorldPosition;
				this.target = null;
			}
		}

		// Token: 0x04006297 RID: 25239
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04006298 RID: 25240
		[RequiredField]
		public FsmVector3 Extents;

		// Token: 0x04006299 RID: 25241
		public FsmFloat Duration;

		// Token: 0x0400629A RID: 25242
		public FsmBool IsLooping;

		// Token: 0x0400629B RID: 25243
		public FsmEvent StopEvent;

		// Token: 0x0400629C RID: 25244
		public FsmFloat FpsLimit;

		// Token: 0x0400629D RID: 25245
		public FsmBool IsCameraShake;

		// Token: 0x0400629E RID: 25246
		private float timer;

		// Token: 0x0400629F RID: 25247
		private double nextUpdateTime;

		// Token: 0x040062A0 RID: 25248
		private Transform target;

		// Token: 0x040062A1 RID: 25249
		private Vector3 startingWorldPosition;
	}
}
