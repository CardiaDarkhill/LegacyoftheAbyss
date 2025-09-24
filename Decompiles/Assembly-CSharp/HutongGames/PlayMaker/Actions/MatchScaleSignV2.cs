using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC1 RID: 3265
	public class MatchScaleSignV2 : FsmStateAction
	{
		// Token: 0x06006180 RID: 24960 RVA: 0x001EE2CA File Offset: 0x001EC4CA
		public override void Reset()
		{
			this.Target = null;
			this.MatchTo = null;
			this.EveryFrame = false;
			this.MatchX = null;
			this.MatchY = null;
			this.MatchZ = null;
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x001EE2F8 File Offset: 0x001EC4F8
		public override void OnEnter()
		{
			this.target = this.Target.GetSafe(this);
			this.matchTo = this.MatchTo.GetSafe(this);
			if (this.target && this.matchTo)
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

		// Token: 0x06006182 RID: 24962 RVA: 0x001EE35F File Offset: 0x001EC55F
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06006183 RID: 24963 RVA: 0x001EE368 File Offset: 0x001EC568
		private void DoAction()
		{
			Vector3 localScale = this.target.transform.localScale;
			Vector3 lossyScale = this.target.transform.lossyScale;
			Vector3 lossyScale2 = this.matchTo.transform.lossyScale;
			Vector3 vector = localScale;
			MatchScaleSignV2.MatchType matchType = (MatchScaleSignV2.MatchType)this.MatchX.Value;
			if (matchType != MatchScaleSignV2.MatchType.None)
			{
				if (matchType == MatchScaleSignV2.MatchType.InvertScaleSign)
				{
					lossyScale.x *= -1f;
				}
				if ((lossyScale.x > 0f && lossyScale2.x < 0f) || (lossyScale.x < 0f && lossyScale2.x > 0f))
				{
					vector.x = -vector.x;
				}
			}
			MatchScaleSignV2.MatchType matchType2 = (MatchScaleSignV2.MatchType)this.MatchY.Value;
			if (matchType2 != MatchScaleSignV2.MatchType.None)
			{
				if (matchType2 == MatchScaleSignV2.MatchType.InvertScaleSign)
				{
					lossyScale.y *= -1f;
				}
				if ((lossyScale.y > 0f && lossyScale2.y < 0f) || (lossyScale.y < 0f && lossyScale2.y > 0f))
				{
					vector.y = -vector.y;
				}
			}
			MatchScaleSignV2.MatchType matchType3 = (MatchScaleSignV2.MatchType)this.MatchZ.Value;
			if (matchType3 != MatchScaleSignV2.MatchType.None)
			{
				if (matchType3 == MatchScaleSignV2.MatchType.InvertScaleSign)
				{
					lossyScale.z *= -1f;
				}
				if ((lossyScale.z > 0f && lossyScale2.z < 0f) || (lossyScale.z < 0f && lossyScale2.z > 0f))
				{
					vector.z = -vector.z;
				}
			}
			this.target.transform.localScale = vector;
		}

		// Token: 0x04005FAB RID: 24491
		public FsmOwnerDefault Target;

		// Token: 0x04005FAC RID: 24492
		public FsmOwnerDefault MatchTo;

		// Token: 0x04005FAD RID: 24493
		[ObjectType(typeof(MatchScaleSignV2.MatchType))]
		public FsmEnum MatchX;

		// Token: 0x04005FAE RID: 24494
		[ObjectType(typeof(MatchScaleSignV2.MatchType))]
		public FsmEnum MatchY;

		// Token: 0x04005FAF RID: 24495
		[ObjectType(typeof(MatchScaleSignV2.MatchType))]
		public FsmEnum MatchZ;

		// Token: 0x04005FB0 RID: 24496
		public bool EveryFrame;

		// Token: 0x04005FB1 RID: 24497
		private GameObject target;

		// Token: 0x04005FB2 RID: 24498
		private GameObject matchTo;

		// Token: 0x02001B85 RID: 7045
		public enum MatchType
		{
			// Token: 0x04009D7A RID: 40314
			None,
			// Token: 0x04009D7B RID: 40315
			MatchScaleSign,
			// Token: 0x04009D7C RID: 40316
			InvertScaleSign
		}
	}
}
