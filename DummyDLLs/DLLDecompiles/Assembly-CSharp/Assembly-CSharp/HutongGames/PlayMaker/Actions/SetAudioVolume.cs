using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E41 RID: 3649
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioVolume : ComponentAction<AudioSource>
	{
		// Token: 0x06006875 RID: 26741 RVA: 0x0020D2EE File Offset: 0x0020B4EE
		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006876 RID: 26742 RVA: 0x0020D30E File Offset: 0x0020B50E
		public override void OnEnter()
		{
			this.DoSetAudioVolume();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006877 RID: 26743 RVA: 0x0020D324 File Offset: 0x0020B524
		public override void OnUpdate()
		{
			this.DoSetAudioVolume();
		}

		// Token: 0x06006878 RID: 26744 RVA: 0x0020D32C File Offset: 0x0020B52C
		private void DoSetAudioVolume()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)) && !this.volume.IsNone)
			{
				if (this.lastCacheVersion != this.cacheVersion)
				{
					this.lastCacheVersion = this.cacheVersion;
					this.hasModifier = false;
					VolumeBlendController component = base.audio.GetComponent<VolumeBlendController>();
					this.hasModifier = (component != null);
					if (this.hasModifier)
					{
						this.modifier = component.GetSharedFSMModifier();
					}
				}
				if (this.hasModifier)
				{
					this.modifier.Volume = this.volume.Value;
					return;
				}
				base.audio.volume = this.volume.Value;
			}
		}

		// Token: 0x040067A3 RID: 26531
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("A GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067A4 RID: 26532
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the volume.")]
		public FsmFloat volume;

		// Token: 0x040067A5 RID: 26533
		[Tooltip("Repeat every frame. Useful if you're driving the volume with a float variable.")]
		public bool everyFrame;

		// Token: 0x040067A6 RID: 26534
		private bool hasModifier;

		// Token: 0x040067A7 RID: 26535
		private VolumeModifier modifier;

		// Token: 0x040067A8 RID: 26536
		private int lastCacheVersion;
	}
}
