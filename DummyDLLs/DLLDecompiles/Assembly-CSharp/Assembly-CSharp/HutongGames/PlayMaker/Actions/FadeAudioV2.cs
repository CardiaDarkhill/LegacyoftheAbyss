using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C38 RID: 3128
	[ActionCategory(ActionCategory.Audio)]
	public class FadeAudioV2 : ComponentAction<AudioSource>
	{
		// Token: 0x06005F15 RID: 24341 RVA: 0x001E2303 File Offset: 0x001E0503
		public override void Reset()
		{
			this.Target = null;
			this.StartVolume = 1f;
			this.EndVolume = 0f;
			this.Time = 1f;
			this.SetOnExit = null;
		}

		// Token: 0x06005F16 RID: 24342 RVA: 0x001E2344 File Offset: 0x001E0544
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				if (this.StartVolume.IsNone)
				{
					this.StartVolume.Value = base.audio.volume;
				}
				else
				{
					base.audio.volume = this.StartVolume.Value;
				}
			}
			if (this.StartVolume.Value > this.EndVolume.Value)
			{
				this.fadingDown = true;
				return;
			}
			this.fadingDown = false;
		}

		// Token: 0x06005F17 RID: 24343 RVA: 0x001E23D0 File Offset: 0x001E05D0
		public override void OnExit()
		{
			if (!this.SetOnExit.Value)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.audio.volume = this.EndVolume.Value;
			}
		}

		// Token: 0x06005F18 RID: 24344 RVA: 0x001E241C File Offset: 0x001E061C
		public override void OnUpdate()
		{
			this.DoSetAudioVolume();
		}

		// Token: 0x06005F19 RID: 24345 RVA: 0x001E2424 File Offset: 0x001E0624
		private void DoSetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.timeElapsed += UnityEngine.Time.deltaTime;
				this.timePercentage = this.timeElapsed / this.Time.Value * 100f;
				float num = (this.EndVolume.Value - this.StartVolume.Value) * (this.timePercentage / 100f);
				base.audio.volume = base.audio.volume + num;
				if (this.fadingDown && base.audio.volume <= this.EndVolume.Value)
				{
					base.audio.volume = this.EndVolume.Value;
					base.Finish();
				}
				else if (!this.fadingDown && base.audio.volume >= this.EndVolume.Value)
				{
					base.audio.volume = this.EndVolume.Value;
					base.Finish();
				}
				this.timeElapsed = 0f;
			}
		}

		// Token: 0x04005BFD RID: 23549
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault Target;

		// Token: 0x04005BFE RID: 23550
		public FsmFloat StartVolume;

		// Token: 0x04005BFF RID: 23551
		public FsmFloat EndVolume;

		// Token: 0x04005C00 RID: 23552
		public FsmFloat Time;

		// Token: 0x04005C01 RID: 23553
		public FsmBool SetOnExit;

		// Token: 0x04005C02 RID: 23554
		private float timeElapsed;

		// Token: 0x04005C03 RID: 23555
		private float timePercentage;

		// Token: 0x04005C04 RID: 23556
		private bool fadingDown;
	}
}
