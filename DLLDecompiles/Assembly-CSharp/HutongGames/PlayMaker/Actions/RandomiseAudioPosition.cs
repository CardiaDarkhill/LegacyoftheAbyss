using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE8 RID: 3304
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Randomise the current time of an audiosource. Leave TimeMin and TimeMax at 0 to use clip length.")]
	public class RandomiseAudioPosition : FsmStateAction
	{
		// Token: 0x06006234 RID: 25140 RVA: 0x001F09AB File Offset: 0x001EEBAB
		public override void Reset()
		{
			this.gameObject = null;
			this.timeMin = 0f;
			this.timeMax = 0f;
		}

		// Token: 0x06006235 RID: 25141 RVA: 0x001F09D4 File Offset: 0x001EEBD4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					float time;
					if (this.timeMin.Value != 0f || this.timeMax.Value != 0f)
					{
						time = Random.Range(this.timeMin.Value, this.timeMax.Value);
					}
					else
					{
						time = Random.Range(0f, component.clip.length);
					}
					component.time = time;
				}
			}
			base.Finish();
		}

		// Token: 0x0400604C RID: 24652
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400604D RID: 24653
		public FsmFloat timeMin;

		// Token: 0x0400604E RID: 24654
		public FsmFloat timeMax;
	}
}
