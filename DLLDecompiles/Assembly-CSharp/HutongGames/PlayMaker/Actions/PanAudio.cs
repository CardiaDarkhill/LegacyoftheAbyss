using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F98 RID: 3992
	[ActionCategory(ActionCategory.Audio)]
	public class PanAudio : ComponentAction<AudioSource>
	{
		// Token: 0x06006E46 RID: 28230 RVA: 0x0022299B File Offset: 0x00220B9B
		public override void Reset()
		{
			this.gameObject = null;
			this.targetPan = 0f;
			this.overTime = 1f;
		}

		// Token: 0x06006E47 RID: 28231 RVA: 0x002229C4 File Offset: 0x00220BC4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.startPan = base.audio.panStereo;
			}
			if (this.startPan > this.targetPan.Value)
			{
				this.panningLeft = true;
				return;
			}
			this.panningLeft = false;
		}

		// Token: 0x06006E48 RID: 28232 RVA: 0x00222A20 File Offset: 0x00220C20
		public override void OnExit()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.audio.panStereo = this.targetPan.Value;
			}
		}

		// Token: 0x06006E49 RID: 28233 RVA: 0x00222A5E File Offset: 0x00220C5E
		public override void OnUpdate()
		{
			this.DoPan();
		}

		// Token: 0x06006E4A RID: 28234 RVA: 0x00222A68 File Offset: 0x00220C68
		private void DoPan()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.timeElapsed += Time.deltaTime;
				this.timePercentage = this.timeElapsed / this.overTime.Value * 100f;
				float num = (this.targetPan.Value - this.startPan) * (this.timePercentage / 100f);
				base.audio.panStereo = base.audio.panStereo + num;
				if (this.panningLeft && base.audio.panStereo <= this.targetPan.Value)
				{
					base.audio.panStereo = this.targetPan.Value;
					base.Finish();
				}
				else if (!this.panningLeft && base.audio.panStereo >= this.targetPan.Value)
				{
					base.audio.panStereo = this.targetPan.Value;
					base.Finish();
				}
				this.timeElapsed = 0f;
			}
		}

		// Token: 0x04006DEC RID: 28140
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006DED RID: 28141
		public FsmFloat targetPan;

		// Token: 0x04006DEE RID: 28142
		public FsmFloat overTime;

		// Token: 0x04006DEF RID: 28143
		private float startPan;

		// Token: 0x04006DF0 RID: 28144
		private float timeElapsed;

		// Token: 0x04006DF1 RID: 28145
		private float timePercentage;

		// Token: 0x04006DF2 RID: 28146
		private bool panningLeft;
	}
}
