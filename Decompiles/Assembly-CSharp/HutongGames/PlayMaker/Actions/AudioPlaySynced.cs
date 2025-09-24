using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC2 RID: 3010
	public class AudioPlaySynced : FsmStateAction
	{
		// Token: 0x06005C93 RID: 23699 RVA: 0x001D253A File Offset: 0x001D073A
		public override void Reset()
		{
			this.Target = null;
			this.ReadSource = null;
		}

		// Token: 0x06005C94 RID: 23700 RVA: 0x001D254A File Offset: 0x001D074A
		public override void OnEnter()
		{
			this.DoAction();
			base.Finish();
		}

		// Token: 0x06005C95 RID: 23701 RVA: 0x001D2558 File Offset: 0x001D0758
		private void DoAction()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			GameObject value = this.ReadSource.Value;
			if (!value)
			{
				return;
			}
			AudioSource component = safe.GetComponent<AudioSource>();
			AudioSource component2 = value.GetComponent<AudioSource>();
			if (!component || !component2)
			{
				return;
			}
			if (!component.isPlaying)
			{
				component.Play();
			}
			if (component2.isPlaying)
			{
				component.timeSamples = component2.timeSamples;
			}
		}

		// Token: 0x04005827 RID: 22567
		public FsmOwnerDefault Target;

		// Token: 0x04005828 RID: 22568
		public FsmGameObject ReadSource;
	}
}
