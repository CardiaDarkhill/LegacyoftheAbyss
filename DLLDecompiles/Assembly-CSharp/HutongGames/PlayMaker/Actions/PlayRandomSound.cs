using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E3B RID: 3643
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays a Random Audio Clip at a position defined by a Game Object or a Vector3. If a position is defined, it takes priority over the game object. You can set the relative weight of the clips to control how often they are selected.")]
	public class PlayRandomSound : FsmStateAction
	{
		// Token: 0x0600685E RID: 26718 RVA: 0x0020CE50 File Offset: 0x0020B050
		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.audioClips = new FsmObject[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.volume = 1f;
			this.noRepeat = false;
		}

		// Token: 0x0600685F RID: 26719 RVA: 0x0020CED1 File Offset: 0x0020B0D1
		public override void OnEnter()
		{
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x06006860 RID: 26720 RVA: 0x0020CEE0 File Offset: 0x0020B0E0
		private void DoPlayRandomClip()
		{
			if (this.audioClips.Length == 0)
			{
				return;
			}
			if (!this.noRepeat.Value || this.weights.Length == 1)
			{
				this.randomIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			}
			else
			{
				do
				{
					this.randomIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				}
				while (this.randomIndex == this.lastIndex && this.randomIndex != -1);
				this.lastIndex = this.randomIndex;
			}
			if (this.randomIndex != -1)
			{
				AudioClip audioClip = this.audioClips[this.randomIndex].Value as AudioClip;
				if (audioClip != null)
				{
					if (!this.position.IsNone)
					{
						AudioSource.PlayClipAtPoint(audioClip, this.position.Value, this.volume.Value);
						return;
					}
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
					if (ownerDefaultTarget == null)
					{
						return;
					}
					AudioSource.PlayClipAtPoint(audioClip, ownerDefaultTarget.transform.position, this.volume.Value);
				}
			}
		}

		// Token: 0x0400678C RID: 26508
		[Tooltip("The GameObject to play the sound.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400678D RID: 26509
		[Tooltip("Use world position instead of GameObject.")]
		public FsmVector3 position;

		// Token: 0x0400678E RID: 26510
		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		[ObjectType(typeof(AudioClip))]
		[Tooltip("A possible Audio Clip choice.")]
		public FsmObject[] audioClips;

		// Token: 0x0400678F RID: 26511
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this sound being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x04006790 RID: 26512
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Volume to play the sound at.")]
		public FsmFloat volume = 1f;

		// Token: 0x04006791 RID: 26513
		[Tooltip("Don't play the same sound twice in a row")]
		public FsmBool noRepeat;

		// Token: 0x04006792 RID: 26514
		private int randomIndex;

		// Token: 0x04006793 RID: 26515
		private int lastIndex = -1;
	}
}
