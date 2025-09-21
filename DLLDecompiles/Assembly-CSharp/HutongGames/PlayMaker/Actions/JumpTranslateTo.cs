using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C95 RID: 3221
	[UsedImplicitly]
	public class JumpTranslateTo : FsmStateAction
	{
		// Token: 0x060060BF RID: 24767 RVA: 0x001EA968 File Offset: 0x001E8B68
		public override void Reset()
		{
			this.TranslateObject = null;
			this.StartPos = null;
			this.EndPos = null;
			this.JumpHeight = 1f;
			this.JumpCurve = new FsmAnimationCurve
			{
				curve = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f, 0f, 3f),
					new Keyframe(0.5f, 1f),
					new Keyframe(1f, 0f, -3f, 0f)
				})
			};
			this.Duration = 1f;
		}

		// Token: 0x060060C0 RID: 24768 RVA: 0x001EAA20 File Offset: 0x001E8C20
		public override void OnEnter()
		{
			GameObject safe = this.TranslateObject.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.enterTime = Time.timeAsDouble;
			this.translateObject = safe.transform;
			this.startPos = this.StartPos.Value;
			this.endPos = this.EndPos.Value;
			this.DoAction();
		}

		// Token: 0x060060C1 RID: 24769 RVA: 0x001EAA88 File Offset: 0x001E8C88
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x060060C2 RID: 24770 RVA: 0x001EAA90 File Offset: 0x001E8C90
		private void DoAction()
		{
			float num = (float)(Time.timeAsDouble - this.enterTime);
			float value = this.Duration.Value;
			bool flag;
			if (num > value)
			{
				num = value;
				flag = true;
			}
			else
			{
				flag = false;
			}
			float num2 = num / value;
			Vector3 position = Vector3.Lerp(this.startPos, this.endPos, num2);
			float num3 = this.JumpHeight.Value * this.JumpCurve.curve.Evaluate(num2);
			position.y += num3;
			this.translateObject.position = position;
			if (flag)
			{
				base.Finish();
			}
		}

		// Token: 0x04005E48 RID: 24136
		[RequiredField]
		public FsmOwnerDefault TranslateObject;

		// Token: 0x04005E49 RID: 24137
		public FsmVector3 StartPos;

		// Token: 0x04005E4A RID: 24138
		public FsmVector3 EndPos;

		// Token: 0x04005E4B RID: 24139
		public FsmFloat JumpHeight;

		// Token: 0x04005E4C RID: 24140
		public FsmAnimationCurve JumpCurve;

		// Token: 0x04005E4D RID: 24141
		public FsmFloat Duration;

		// Token: 0x04005E4E RID: 24142
		private Transform translateObject;

		// Token: 0x04005E4F RID: 24143
		private double enterTime;

		// Token: 0x04005E50 RID: 24144
		private Vector3 startPos;

		// Token: 0x04005E51 RID: 24145
		private Vector3 endPos;
	}
}
