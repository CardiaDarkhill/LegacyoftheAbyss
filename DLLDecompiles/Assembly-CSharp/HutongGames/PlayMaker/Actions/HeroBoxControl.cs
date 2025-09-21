using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001233 RID: 4659
	public class HeroBoxControl : FsmStateAction
	{
		// Token: 0x17000C17 RID: 3095
		// (get) Token: 0x06007B64 RID: 31588 RVA: 0x0024F7AF File Offset: 0x0024D9AF
		protected virtual bool IsEveryFrame
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06007B65 RID: 31589 RVA: 0x0024F7B2 File Offset: 0x0024D9B2
		public override void Reset()
		{
			this.heroBoxState = null;
		}

		// Token: 0x06007B66 RID: 31590 RVA: 0x0024F7BC File Offset: 0x0024D9BC
		public override void OnEnter()
		{
			bool flag = this.heroBox;
			if (!flag)
			{
				HeroController instance = HeroController.instance;
				if (instance)
				{
					this.heroBox = instance.heroBox;
					flag = this.heroBox;
				}
			}
			if (flag)
			{
				HeroBoxControl.HeroBoxState boxState = (HeroBoxControl.HeroBoxState)this.heroBoxState.Value;
				this.SetBoxState(boxState);
			}
			else
			{
				Debug.LogError("Failed to find hero box.");
			}
			if (!this.IsEveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007B67 RID: 31591 RVA: 0x0024F834 File Offset: 0x0024DA34
		protected void SetBoxState(HeroBoxControl.HeroBoxState boxState)
		{
			switch (boxState)
			{
			case HeroBoxControl.HeroBoxState.Off:
				this.heroBox.HeroBoxOff();
				return;
			case HeroBoxControl.HeroBoxState.Normal:
				this.heroBox.HeroBoxNormal();
				return;
			case HeroBoxControl.HeroBoxState.Downspike:
				this.heroBox.HeroBoxDownspike();
				return;
			case HeroBoxControl.HeroBoxState.DownDrill:
				this.heroBox.HeroBoxDownDrill();
				return;
			case HeroBoxControl.HeroBoxState.Scuttle:
				this.heroBox.HeroBoxScuttle();
				return;
			case HeroBoxControl.HeroBoxState.Sprint:
				this.heroBox.HeroBoxSprint();
				return;
			case HeroBoxControl.HeroBoxState.Airdash:
				this.heroBox.HeroBoxAirdash();
				return;
			case HeroBoxControl.HeroBoxState.ReaperDownSlash:
				this.heroBox.HeroBoxReaperDownSlash();
				return;
			case HeroBoxControl.HeroBoxState.WallSlide:
				this.heroBox.HeroBoxWallSlide();
				return;
			case HeroBoxControl.HeroBoxState.ParryStance:
				this.heroBox.HeroBoxParryStance();
				return;
			case HeroBoxControl.HeroBoxState.WallScramble:
				this.heroBox.HeroBoxWallScramble();
				return;
			case HeroBoxControl.HeroBoxState.Harpoon:
				this.heroBox.HeroBoxHarpoon();
				return;
			default:
				this.heroBox.HeroBoxNormal();
				Debug.LogError(string.Format("Encountered unsupported box state {0}. Setting to normal.", boxState));
				return;
			}
		}

		// Token: 0x04007BA9 RID: 31657
		[ObjectType(typeof(HeroBoxControl.HeroBoxState))]
		public FsmEnum heroBoxState;

		// Token: 0x04007BAA RID: 31658
		private HeroBox heroBox;

		// Token: 0x02001BDA RID: 7130
		[Serializable]
		public enum HeroBoxState
		{
			// Token: 0x04009F1F RID: 40735
			Off,
			// Token: 0x04009F20 RID: 40736
			Normal,
			// Token: 0x04009F21 RID: 40737
			Downspike,
			// Token: 0x04009F22 RID: 40738
			DownDrill,
			// Token: 0x04009F23 RID: 40739
			Scuttle,
			// Token: 0x04009F24 RID: 40740
			Sprint,
			// Token: 0x04009F25 RID: 40741
			Airdash,
			// Token: 0x04009F26 RID: 40742
			ReaperDownSlash,
			// Token: 0x04009F27 RID: 40743
			WallSlide,
			// Token: 0x04009F28 RID: 40744
			ParryStance,
			// Token: 0x04009F29 RID: 40745
			WallScramble,
			// Token: 0x04009F2A RID: 40746
			Harpoon
		}
	}
}
