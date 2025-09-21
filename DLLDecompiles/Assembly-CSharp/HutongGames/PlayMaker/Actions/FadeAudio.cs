using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E98 RID: 3736
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class FadeAudio : ComponentAction<AudioSource>
	{
		// Token: 0x06006A06 RID: 27142 RVA: 0x00212729 File Offset: 0x00210929
		public override void Reset()
		{
			this.gameObject = null;
			this.startVolume = 1f;
			this.endVolume = 0f;
			this.time = 1f;
		}

		// Token: 0x06006A07 RID: 27143 RVA: 0x00212764 File Offset: 0x00210964
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.UpdateBlendCache();
				if (this.startVolume.IsNone)
				{
					this.startVolume.Value = this.GetVolume();
				}
				else
				{
					this.SetVolume(this.startVolume.Value);
				}
			}
			if (this.startVolume.Value > this.endVolume.Value)
			{
				this.fadingDown = true;
				return;
			}
			this.fadingDown = false;
		}

		// Token: 0x06006A08 RID: 27144 RVA: 0x002127EC File Offset: 0x002109EC
		public override void OnExit()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.UpdateBlendCache();
				this.SetVolume(this.endVolume.Value);
			}
		}

		// Token: 0x06006A09 RID: 27145 RVA: 0x0021282B File Offset: 0x00210A2B
		public override void OnUpdate()
		{
			this.DoSetAudioVolume();
		}

		// Token: 0x06006A0A RID: 27146 RVA: 0x00212834 File Offset: 0x00210A34
		private void DoSetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.UpdateBlendCache();
				this.timeElapsed += Time.deltaTime;
				this.timePercentage = this.timeElapsed / this.time.Value * 100f;
				float num = (this.endVolume.Value - this.startVolume.Value) * (this.timePercentage / 100f);
				this.SetVolume(this.GetVolume() + num);
				if (this.fadingDown && base.audio.volume <= this.endVolume.Value)
				{
					this.SetVolume(this.endVolume.Value);
					base.Finish();
				}
				else if (!this.fadingDown && base.audio.volume >= this.endVolume.Value)
				{
					this.SetVolume(this.endVolume.Value);
					base.Finish();
				}
				this.timeElapsed = 0f;
			}
		}

		// Token: 0x06006A0B RID: 27147 RVA: 0x00212943 File Offset: 0x00210B43
		private float GetVolume()
		{
			if (this.hasBlendController)
			{
				return this.volumeModifier.Volume;
			}
			return base.audio.volume;
		}

		// Token: 0x06006A0C RID: 27148 RVA: 0x00212964 File Offset: 0x00210B64
		private void SetVolume(float volume)
		{
			if (this.hasBlendController)
			{
				this.volumeModifier.Volume = volume;
				return;
			}
			base.audio.volume = volume;
		}

		// Token: 0x06006A0D RID: 27149 RVA: 0x00212988 File Offset: 0x00210B88
		private void UpdateBlendCache()
		{
			if (this.lastCacheVersion != this.cacheVersion)
			{
				this.lastCacheVersion = this.cacheVersion;
				VolumeBlendController component = base.audio.GetComponent<VolumeBlendController>();
				if (component != null)
				{
					this.hasBlendController = true;
					this.volumeModifier = component.GetSharedFSMModifier();
				}
			}
		}

		// Token: 0x0400695E RID: 26974
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400695F RID: 26975
		public FsmFloat startVolume;

		// Token: 0x04006960 RID: 26976
		public FsmFloat endVolume;

		// Token: 0x04006961 RID: 26977
		public FsmFloat time;

		// Token: 0x04006962 RID: 26978
		private float timeElapsed;

		// Token: 0x04006963 RID: 26979
		private float timePercentage;

		// Token: 0x04006964 RID: 26980
		private bool fadingDown;

		// Token: 0x04006965 RID: 26981
		private bool hasBlendController;

		// Token: 0x04006966 RID: 26982
		private VolumeModifier volumeModifier;

		// Token: 0x04006967 RID: 26983
		private int lastCacheVersion;
	}
}
