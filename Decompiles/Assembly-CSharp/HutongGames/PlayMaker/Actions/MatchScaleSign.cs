using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC0 RID: 3264
	public class MatchScaleSign : FsmStateAction
	{
		// Token: 0x0600617B RID: 24955 RVA: 0x001EE16D File Offset: 0x001EC36D
		public override void Reset()
		{
			this.Target = null;
			this.MatchTo = null;
			this.EveryFrame = false;
		}

		// Token: 0x0600617C RID: 24956 RVA: 0x001EE184 File Offset: 0x001EC384
		public override void OnEnter()
		{
			this.target = this.Target.GetSafe(this);
			if (this.target && this.MatchTo.Value)
			{
				this.DoAction();
			}
			else
			{
				base.Finish();
			}
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600617D RID: 24957 RVA: 0x001EE1DE File Offset: 0x001EC3DE
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x0600617E RID: 24958 RVA: 0x001EE1E8 File Offset: 0x001EC3E8
		private void DoAction()
		{
			Vector3 localScale = this.target.transform.localScale;
			Vector3 lossyScale = this.target.transform.lossyScale;
			Vector3 lossyScale2 = this.MatchTo.Value.transform.lossyScale;
			Vector3 vector = localScale;
			if ((lossyScale.x > 0f && lossyScale2.x < 0f) || (lossyScale.x < 0f && lossyScale2.x > 0f))
			{
				vector.x = -vector.x;
			}
			if ((lossyScale.y > 0f && lossyScale2.y < 0f) || (lossyScale.y < 0f && lossyScale2.y > 0f))
			{
				vector.y = -vector.y;
			}
			this.target.transform.localScale = vector;
		}

		// Token: 0x04005FA7 RID: 24487
		public FsmOwnerDefault Target;

		// Token: 0x04005FA8 RID: 24488
		public FsmGameObject MatchTo;

		// Token: 0x04005FA9 RID: 24489
		public bool EveryFrame;

		// Token: 0x04005FAA RID: 24490
		private GameObject target;
	}
}
