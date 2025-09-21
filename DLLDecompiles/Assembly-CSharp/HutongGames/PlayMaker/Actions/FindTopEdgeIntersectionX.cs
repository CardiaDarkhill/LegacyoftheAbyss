using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001274 RID: 4724
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Find the intersection point between a given point and direction with the top edge of a BoxCollider2D, clamping to the closest point on the line if it isn't on the line.")]
	public sealed class FindTopEdgeIntersectionX : FsmStateAction
	{
		// Token: 0x06007C78 RID: 31864 RVA: 0x002533AF File Offset: 0x002515AF
		public override void Reset()
		{
			this.gameObject = null;
			this.xCoordinate = null;
			this.intersectionPoint = null;
			this.xIntersect = null;
			this.yIntersect = null;
			this.zIntersect = null;
		}

		// Token: 0x06007C79 RID: 31865 RVA: 0x002533DB File Offset: 0x002515DB
		public override void OnEnter()
		{
			this.DoFindIntersection();
			base.Finish();
		}

		// Token: 0x06007C7A RID: 31866 RVA: 0x002533EC File Offset: 0x002515EC
		private void DoFindIntersection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			BoxCollider2D component = ownerDefaultTarget.GetComponent<BoxCollider2D>();
			if (component == null)
			{
				return;
			}
			Vector2 vector = component.bounds.center;
			Vector2 size = component.size;
			float num = size.y / 2f;
			Vector2 vector2 = this.RotatePointAroundPivot(new Vector2(vector.x - size.x / 2f, vector.y + num), vector, component.transform.eulerAngles.z);
			Vector2 vector3 = this.RotatePointAroundPivot(new Vector2(vector.x + size.x / 2f, vector.y + num), vector, component.transform.eulerAngles.z);
			float num2 = Mathf.Min(vector2.x, vector3.x);
			float num3 = Mathf.Max(vector2.x, vector3.x);
			float value = this.xCoordinate.Value;
			Vector2 vector4;
			if (value >= num2 && value <= num3)
			{
				float t = (value - vector2.x) / (vector3.x - vector2.x);
				vector4 = Vector2.Lerp(vector2, vector3, t);
			}
			else if (Mathf.Abs(value - vector2.x) < Mathf.Abs(value - vector3.x))
			{
				vector4 = vector2;
			}
			else
			{
				vector4 = vector3;
			}
			this.intersectionPoint.Value = new Vector3(vector4.x, vector4.y, 0f);
			this.xIntersect.Value = vector4.x;
			this.yIntersect.Value = vector4.y;
		}

		// Token: 0x06007C7B RID: 31867 RVA: 0x002535A4 File Offset: 0x002517A4
		private Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float angle)
		{
			Vector2 vector = point - pivot;
			vector = Quaternion.Euler(0f, 0f, angle) * vector;
			point = vector + pivot;
			return point;
		}

		// Token: 0x04007C80 RID: 31872
		[RequiredField]
		[CheckForComponent(typeof(BoxCollider2D))]
		[Tooltip("The GameObject with the BoxCollider2D.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007C81 RID: 31873
		[RequiredField]
		[Tooltip("The starting point of the line.")]
		public FsmFloat xCoordinate;

		// Token: 0x04007C82 RID: 31874
		[Tooltip("The intersection point.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 intersectionPoint;

		// Token: 0x04007C83 RID: 31875
		[UIHint(UIHint.Variable)]
		public FsmFloat xIntersect;

		// Token: 0x04007C84 RID: 31876
		[UIHint(UIHint.Variable)]
		public FsmFloat yIntersect;

		// Token: 0x04007C85 RID: 31877
		[UIHint(UIHint.Variable)]
		public FsmFloat zIntersect;
	}
}
