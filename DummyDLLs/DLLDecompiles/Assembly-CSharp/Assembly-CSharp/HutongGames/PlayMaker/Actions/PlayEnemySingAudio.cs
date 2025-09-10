using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC9 RID: 3273
	[ActionCategory(ActionCategory.Audio)]
	public class PlayEnemySingAudio : ComponentAction<AudioSource>
	{
		// Token: 0x060061AB RID: 25003 RVA: 0x001EEF05 File Offset: 0x001ED105
		public override void Reset()
		{
			this.gameObject = null;
			this.singAudioTable = null;
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x001EEF18 File Offset: 0x001ED118
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.audio.clip = this.singAudioTable.SelectClip(true);
				base.audio.pitch = this.singAudioTable.SelectPitch();
				base.audio.Stop();
				base.audio.time = Random.Range(0f, 0.2f);
				base.audio.Play();
			}
			base.Finish();
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x001EEFA3 File Offset: 0x001ED1A3
		public override void OnExit()
		{
			base.audio.Stop();
		}

		// Token: 0x04005FDE RID: 24542
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with the AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005FDF RID: 24543
		public RandomAudioClipTable singAudioTable;
	}
}
