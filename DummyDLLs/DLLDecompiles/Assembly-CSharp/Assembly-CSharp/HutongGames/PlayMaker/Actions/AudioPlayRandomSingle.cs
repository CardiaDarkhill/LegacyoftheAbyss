using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBC RID: 3004
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomSingle : FsmStateAction
	{
		// Token: 0x06005C7B RID: 23675 RVA: 0x001D1F24 File Offset: 0x001D0124
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClip = null;
			this.pitchMin = 1f;
			this.pitchMax = 1f;
		}

		// Token: 0x06005C7C RID: 23676 RVA: 0x001D1F54 File Offset: 0x001D0154
		public override void OnEnter()
		{
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x06005C7D RID: 23677 RVA: 0x001D1F64 File Offset: 0x001D0164
		private void DoPlayRandomClip()
		{
			AudioClip clip = this.audioClip.Value as AudioClip;
			this.audio = this.gameObject.Value.GetComponent<AudioSource>();
			float pitch = Random.Range(this.pitchMin.Value, this.pitchMax.Value);
			this.audio.pitch = pitch;
			this.audio.PlayOneShot(clip);
		}

		// Token: 0x040057FB RID: 22523
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmGameObject gameObject;

		// Token: 0x040057FC RID: 22524
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		// Token: 0x040057FD RID: 22525
		public FsmFloat pitchMin = 1f;

		// Token: 0x040057FE RID: 22526
		public FsmFloat pitchMax = 2f;

		// Token: 0x040057FF RID: 22527
		private AudioSource audio;
	}
}
