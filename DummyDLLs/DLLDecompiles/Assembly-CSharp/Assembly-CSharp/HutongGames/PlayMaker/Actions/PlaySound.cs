using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E3C RID: 3644
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Plays an Audio Clip at a position defined by a Game Object or Vector3. If a position is defined, it takes priority over the game object. This action doesn't require an Audio Source component, but offers less control than Audio actions.")]
	public class PlaySound : FsmStateAction
	{
		// Token: 0x06006862 RID: 26722 RVA: 0x0020D004 File Offset: 0x0020B204
		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.clip = null;
			this.volume = 1f;
		}

		// Token: 0x06006863 RID: 26723 RVA: 0x0020D036 File Offset: 0x0020B236
		public override void OnEnter()
		{
			this.DoPlaySound();
			base.Finish();
		}

		// Token: 0x06006864 RID: 26724 RVA: 0x0020D044 File Offset: 0x0020B244
		private void DoPlaySound()
		{
			AudioClip x = this.clip.Value as AudioClip;
			if (x == null)
			{
				base.LogWarning("Missing Audio Clip!");
				return;
			}
			if (!this.position.IsNone)
			{
				AudioSource.PlayClipAtPoint(x, this.position.Value, this.volume.Value);
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AudioSource.PlayClipAtPoint(x, ownerDefaultTarget.transform.position, this.volume.Value);
		}

		// Token: 0x04006794 RID: 26516
		[Tooltip("A Game Object that defines a position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006795 RID: 26517
		[Tooltip("A Vector3 value that defines a world position (overrides Game Object).")]
		public FsmVector3 position;

		// Token: 0x04006796 RID: 26518
		[RequiredField]
		[Title("Audio Clip")]
		[ObjectType(typeof(AudioClip))]
		[Tooltip("The audio clip to play.")]
		public FsmObject clip;

		// Token: 0x04006797 RID: 26519
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Volume to play sound at.")]
		public FsmFloat volume = 1f;
	}
}
