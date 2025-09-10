using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEB RID: 3051
	public class CheckGroundClearance : FsmStateAction
	{
		// Token: 0x06005D74 RID: 23924 RVA: 0x001D7661 File Offset: 0x001D5861
		public override void Reset()
		{
			this.RelativeTo = null;
			this.Position = null;
			this.GroundWidth = null;
			this.IsClearEvent = null;
			this.NotClearEvent = null;
		}

		// Token: 0x06005D75 RID: 23925 RVA: 0x001D7686 File Offset: 0x001D5886
		public override void OnEnter()
		{
			base.Fsm.Event(this.IsClear() ? this.IsClearEvent : this.NotClearEvent);
			base.Finish();
		}

		// Token: 0x06005D76 RID: 23926 RVA: 0x001D76B0 File Offset: 0x001D58B0
		private bool IsClear()
		{
			GameObject safe = this.RelativeTo.GetSafe(this);
			RaycastHit2D hit = Helper.Raycast2D(safe ? safe.transform.TransformPoint(this.Position.Value) : this.Position.Value, Vector2.down, 10f, 256);
			if (!hit)
			{
				return false;
			}
			Vector2 point = hit.point;
			point.y += 0.3f;
			float length = this.GroundWidth.Value / 2f;
			return CheckGroundClearance.IsClearDirectional(point, -1f, length, 256) && CheckGroundClearance.IsClearDirectional(point, 1f, length, 256);
		}

		// Token: 0x06005D77 RID: 23927 RVA: 0x001D7774 File Offset: 0x001D5974
		private static bool IsClearDirectional(Vector2 origin, float direction, float length, int layerMask)
		{
			Vector2 vector = new Vector2(direction, 0f);
			if (Helper.Raycast2D(origin, vector, length, layerMask))
			{
				return false;
			}
			while (length > 0f)
			{
				if (!Helper.Raycast2D(origin + vector * length, Vector2.down, 0.6f, 256))
				{
					return false;
				}
				length -= 0.5f;
			}
			return true;
		}

		// Token: 0x0400599A RID: 22938
		private const int LAYER_MASK = 256;

		// Token: 0x0400599B RID: 22939
		private const float GROUND_PADDING = 0.3f;

		// Token: 0x0400599C RID: 22940
		public FsmOwnerDefault RelativeTo;

		// Token: 0x0400599D RID: 22941
		public FsmVector2 Position;

		// Token: 0x0400599E RID: 22942
		[RequiredField]
		public FsmFloat GroundWidth;

		// Token: 0x0400599F RID: 22943
		public FsmEvent IsClearEvent;

		// Token: 0x040059A0 RID: 22944
		public FsmEvent NotClearEvent;
	}
}
