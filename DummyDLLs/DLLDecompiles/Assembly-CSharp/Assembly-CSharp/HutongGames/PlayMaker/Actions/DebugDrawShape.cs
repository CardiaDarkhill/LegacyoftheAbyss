using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E78 RID: 3704
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Draw a debug Gizmo.\nNote: you can enable/disable Gizmos in the Game View toolbar.")]
	public class DebugDrawShape : ComponentAction<Transform>
	{
		// Token: 0x06006983 RID: 27011 RVA: 0x00210BA7 File Offset: 0x0020EDA7
		public bool HideRadius()
		{
			return this.shape != DebugDrawShape.ShapeType.Sphere && this.shape != DebugDrawShape.ShapeType.WireSphere;
		}

		// Token: 0x06006984 RID: 27012 RVA: 0x00210BBF File Offset: 0x0020EDBF
		public bool HideSize()
		{
			return this.shape != DebugDrawShape.ShapeType.Cube && this.shape != DebugDrawShape.ShapeType.WireCube;
		}

		// Token: 0x06006985 RID: 27013 RVA: 0x00210BD8 File Offset: 0x0020EDD8
		public override void Reset()
		{
			this.gameObject = null;
			this.shape = DebugDrawShape.ShapeType.Sphere;
			this.color = new FsmColor
			{
				Value = Color.grey
			};
			this.radius = new FsmFloat
			{
				Value = 1f
			};
			this.size = new Vector3(1f, 1f, 1f);
		}

		// Token: 0x06006986 RID: 27014 RVA: 0x00210C3E File Offset: 0x0020EE3E
		public override void Awake()
		{
			base.BlocksFinish = false;
		}

		// Token: 0x06006987 RID: 27015 RVA: 0x00210C48 File Offset: 0x0020EE48
		public override void OnDrawActionGizmos()
		{
			if (base.Fsm == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCachedTransform(ownerDefaultTarget))
			{
				return;
			}
			Gizmos.color = this.color.Value;
			switch (this.shape)
			{
			case DebugDrawShape.ShapeType.Sphere:
				Gizmos.DrawSphere(base.cachedTransform.position, this.radius.Value);
				return;
			case DebugDrawShape.ShapeType.Cube:
				Gizmos.DrawCube(base.cachedTransform.position, this.size.Value);
				return;
			case DebugDrawShape.ShapeType.WireSphere:
				Gizmos.DrawWireSphere(base.cachedTransform.position, this.radius.Value);
				return;
			case DebugDrawShape.ShapeType.WireCube:
				Gizmos.DrawWireCube(base.cachedTransform.position, this.size.Value);
				return;
			default:
				return;
			}
		}

		// Token: 0x040068BB RID: 26811
		[RequiredField]
		[Tooltip("Draw the Gizmo at a GameObject's position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040068BC RID: 26812
		[Tooltip("The type of Gizmo to draw:\nSphere, Cube, WireSphere, or WireCube.")]
		public DebugDrawShape.ShapeType shape;

		// Token: 0x040068BD RID: 26813
		[Tooltip("The color to use.")]
		public FsmColor color;

		// Token: 0x040068BE RID: 26814
		[HideIf("HideRadius")]
		[Tooltip("Use this for sphere gizmos")]
		public FsmFloat radius;

		// Token: 0x040068BF RID: 26815
		[HideIf("HideSize")]
		[Tooltip("Use this for cube gizmos")]
		public FsmVector3 size;

		// Token: 0x02001BA5 RID: 7077
		public enum ShapeType
		{
			// Token: 0x04009E09 RID: 40457
			Sphere,
			// Token: 0x04009E0A RID: 40458
			Cube,
			// Token: 0x04009E0B RID: 40459
			WireSphere,
			// Token: 0x04009E0C RID: 40460
			WireCube
		}
	}
}
