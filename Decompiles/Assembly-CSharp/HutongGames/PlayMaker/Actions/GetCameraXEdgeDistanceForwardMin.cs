using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5F RID: 3167
	public class GetCameraXEdgeDistanceForwardMin : FsmStateAction
	{
		// Token: 0x06005FCC RID: 24524 RVA: 0x001E5C6C File Offset: 0x001E3E6C
		public override void Reset()
		{
			this.From = null;
			this.Min = new FsmFloat
			{
				UseVariable = true
			};
			this.StoreResult = null;
		}

		// Token: 0x06005FCD RID: 24525 RVA: 0x001E5C90 File Offset: 0x001E3E90
		public override void OnEnter()
		{
			GameObject safe = this.From.GetSafe(this);
			HeroController component = safe.GetComponent<HeroController>();
			bool flag;
			if (component)
			{
				flag = component.cState.facingRight;
			}
			else
			{
				flag = (Mathf.Sign(safe.transform.lossyScale.x) > 0f);
			}
			Vector2 vector = GameCameras.instance.mainCamera.transform.position;
			float num = 8.3f * ForceCameraAspect.CurrentViewportAspect;
			float num2 = vector.x - num;
			float num3 = vector.x + num;
			float x = safe.transform.position.x;
			float num4;
			if (flag)
			{
				num4 = num3 - x;
			}
			else
			{
				num4 = x - num2;
			}
			if (!this.Min.IsNone && num4 < this.Min.Value)
			{
				num4 = this.Min.Value;
			}
			this.StoreResult.Value = num4;
			base.Finish();
		}

		// Token: 0x04005D20 RID: 23840
		public FsmOwnerDefault From;

		// Token: 0x04005D21 RID: 23841
		public FsmFloat Min;

		// Token: 0x04005D22 RID: 23842
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreResult;
	}
}
