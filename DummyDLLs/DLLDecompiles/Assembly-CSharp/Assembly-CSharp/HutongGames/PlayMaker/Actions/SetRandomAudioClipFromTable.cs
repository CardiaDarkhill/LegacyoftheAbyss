using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200120D RID: 4621
	public class SetRandomAudioClipFromTable : FsmStateAction
	{
		// Token: 0x06007ACB RID: 31435 RVA: 0x0024D86D File Offset: 0x0024BA6D
		public override void Reset()
		{
			this.Table = null;
			this.AudioSource = null;
			this.AutoPlay = null;
			this.timer = 0f;
		}

		// Token: 0x06007ACC RID: 31436 RVA: 0x0024D88F File Offset: 0x0024BA8F
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.delay.Value == 0f)
			{
				this.DoSet();
			}
		}

		// Token: 0x06007ACD RID: 31437 RVA: 0x0024D8B4 File Offset: 0x0024BAB4
		public override void OnUpdate()
		{
			if (this.timer >= this.delay.Value)
			{
				this.DoSet();
				return;
			}
			this.timer += Time.deltaTime;
		}

		// Token: 0x06007ACE RID: 31438 RVA: 0x0024D8E4 File Offset: 0x0024BAE4
		private void DoSet()
		{
			if (this.Table.Value == null)
			{
				base.Finish();
			}
			RandomAudioClipTable randomAudioClipTable = this.Table.Value as RandomAudioClipTable;
			GameObject value = this.AudioSource.Value;
			if (value)
			{
				AudioSource component = value.GetComponent<AudioSource>();
				if (randomAudioClipTable != null && component != null)
				{
					component.clip = randomAudioClipTable.SelectClip(true);
					component.volume = randomAudioClipTable.SelectVolume();
					component.pitch = randomAudioClipTable.SelectPitch();
					component.loop = true;
					if (this.AutoPlay.Value)
					{
						component.Play();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007B13 RID: 31507
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		// Token: 0x04007B14 RID: 31508
		[ObjectType(typeof(AudioSource))]
		public FsmGameObject AudioSource;

		// Token: 0x04007B15 RID: 31509
		public FsmBool AutoPlay;

		// Token: 0x04007B16 RID: 31510
		public FsmFloat delay;

		// Token: 0x04007B17 RID: 31511
		private float timer;
	}
}
