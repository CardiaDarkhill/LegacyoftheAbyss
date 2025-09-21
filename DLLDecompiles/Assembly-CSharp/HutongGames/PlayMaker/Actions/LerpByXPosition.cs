using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C97 RID: 3223
	public class LerpByXPosition : FsmStateAction
	{
		// Token: 0x060060C9 RID: 24777 RVA: 0x001EAB86 File Offset: 0x001E8D86
		public bool IsStartPointDefined()
		{
			return !this.StartPoint.IsNone;
		}

		// Token: 0x060060CA RID: 24778 RVA: 0x001EAB96 File Offset: 0x001E8D96
		public bool IsEndPointDefined()
		{
			return !this.EndPoint.IsNone;
		}

		// Token: 0x060060CB RID: 24779 RVA: 0x001EABA6 File Offset: 0x001E8DA6
		public override void Reset()
		{
			this.StartPoint = null;
			this.StartPosition = null;
			this.EndPoint = null;
			this.EndPosition = null;
			this.TargetPosition = null;
			this.EveryFrame = false;
		}

		// Token: 0x060060CC RID: 24780 RVA: 0x001EABD2 File Offset: 0x001E8DD2
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060060CD RID: 24781 RVA: 0x001EABE8 File Offset: 0x001E8DE8
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x060060CE RID: 24782 RVA: 0x001EABF0 File Offset: 0x001E8DF0
		private void DoAction()
		{
			float num = this.StartPoint.Value ? this.StartPoint.Value.transform.position.x : this.StartPosition.Value;
			float num2 = this.EndPoint.Value ? this.EndPoint.Value.transform.position.x : this.EndPosition.Value;
			float num3 = Mathf.Clamp(this.TargetPosition.Value.x, (num < num2) ? num : num2, (num > num2) ? num : num2);
			float num4 = num2 - num;
			float num5 = (num2 - num3) / num4;
			float t = 1f - num5;
			foreach (LerpByXPosition.FsmFloatMinMax fsmFloatMinMax in this.Values)
			{
				fsmFloatMinMax.StoreValue.Value = Mathf.Lerp(fsmFloatMinMax.MinValue.Value, fsmFloatMinMax.MaxValue.Value, t);
			}
		}

		// Token: 0x04005E54 RID: 24148
		public FsmGameObject StartPoint;

		// Token: 0x04005E55 RID: 24149
		[HideIf("IsStartPointDefined")]
		public FsmFloat StartPosition;

		// Token: 0x04005E56 RID: 24150
		public FsmGameObject EndPoint;

		// Token: 0x04005E57 RID: 24151
		[HideIf("IsEndPointDefined")]
		public FsmFloat EndPosition;

		// Token: 0x04005E58 RID: 24152
		public FsmVector2 TargetPosition;

		// Token: 0x04005E59 RID: 24153
		public LerpByXPosition.FsmFloatMinMax[] Values;

		// Token: 0x04005E5A RID: 24154
		public bool EveryFrame;

		// Token: 0x02001B84 RID: 7044
		[Serializable]
		public class FsmFloatMinMax
		{
			// Token: 0x04009D76 RID: 40310
			public FsmFloat MinValue;

			// Token: 0x04009D77 RID: 40311
			public FsmFloat MaxValue;

			// Token: 0x04009D78 RID: 40312
			[UIHint(UIHint.Variable)]
			public FsmFloat StoreValue;
		}
	}
}
