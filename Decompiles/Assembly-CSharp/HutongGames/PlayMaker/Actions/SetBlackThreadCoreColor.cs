using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200129B RID: 4763
	public sealed class SetBlackThreadCoreColor : FsmStateAction
	{
		// Token: 0x06007D08 RID: 32008 RVA: 0x00255420 File Offset: 0x00253620
		public bool IsOriginal()
		{
			return (SetBlackThreadCoreColor.LerpType)this.lerpType.Value == SetBlackThreadCoreColor.LerpType.OriginalColor;
		}

		// Token: 0x06007D09 RID: 32009 RVA: 0x00255435 File Offset: 0x00253635
		public override void Reset()
		{
			this.Target = null;
			this.lerpType = null;
			this.targetColor = null;
			this.duration = null;
		}

		// Token: 0x06007D0A RID: 32010 RVA: 0x00255454 File Offset: 0x00253654
		public override void OnEnter()
		{
			BlackThreadCore safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				SetBlackThreadCoreColor.LerpType lerpType = (SetBlackThreadCoreColor.LerpType)this.lerpType.Value;
				if (lerpType != SetBlackThreadCoreColor.LerpType.TargetColor)
				{
					if (lerpType == SetBlackThreadCoreColor.LerpType.OriginalColor)
					{
						safe.LerpToOriginal(this.duration.Value);
					}
				}
				else
				{
					safe.LerpToColor(this.targetColor.Value, this.duration.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D18 RID: 32024
		[RequiredField]
		[CheckForComponent(typeof(BlackThreadCore))]
		public FsmOwnerDefault Target;

		// Token: 0x04007D19 RID: 32025
		[ObjectType(typeof(SetBlackThreadCoreColor.LerpType))]
		public FsmEnum lerpType;

		// Token: 0x04007D1A RID: 32026
		[HideIf("IsOriginal")]
		public FsmColor targetColor;

		// Token: 0x04007D1B RID: 32027
		[Tooltip("Applies instantly if duration <= 0")]
		public FsmFloat duration;

		// Token: 0x02001BEC RID: 7148
		[Serializable]
		private enum LerpType
		{
			// Token: 0x04009F92 RID: 40850
			TargetColor,
			// Token: 0x04009F93 RID: 40851
			OriginalColor
		}
	}
}
