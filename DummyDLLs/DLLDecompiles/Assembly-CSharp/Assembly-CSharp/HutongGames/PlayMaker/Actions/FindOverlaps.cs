using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F9E RID: 3998
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Find overlaps with GameObject colliders in the scene.")]
	public class FindOverlaps : ComponentAction<Transform>
	{
		// Token: 0x17000BFD RID: 3069
		// (get) Token: 0x06006E7A RID: 28282 RVA: 0x00223658 File Offset: 0x00221858
		// (set) Token: 0x06006E7B RID: 28283 RVA: 0x00223660 File Offset: 0x00221860
		public Vector3 center { get; private set; }

		// Token: 0x17000BFE RID: 3070
		// (get) Token: 0x06006E7C RID: 28284 RVA: 0x00223669 File Offset: 0x00221869
		// (set) Token: 0x06006E7D RID: 28285 RVA: 0x00223671 File Offset: 0x00221871
		public Quaternion orientation { get; private set; }

		// Token: 0x17000BFF RID: 3071
		// (get) Token: 0x06006E7E RID: 28286 RVA: 0x0022367A File Offset: 0x0022187A
		// (set) Token: 0x06006E7F RID: 28287 RVA: 0x00223682 File Offset: 0x00221882
		public Vector3 capsulePoint1 { get; private set; }

		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x06006E80 RID: 28288 RVA: 0x0022368B File Offset: 0x0022188B
		// (set) Token: 0x06006E81 RID: 28289 RVA: 0x00223693 File Offset: 0x00221893
		public Vector3 capsulePoint2 { get; private set; }

		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x06006E82 RID: 28290 RVA: 0x0022369C File Offset: 0x0022189C
		// (set) Token: 0x06006E83 RID: 28291 RVA: 0x002236A4 File Offset: 0x002218A4
		public int targetMask { get; private set; }

		// Token: 0x06006E84 RID: 28292 RVA: 0x002236B0 File Offset: 0x002218B0
		public override void Reset()
		{
			this.position = null;
			this.offset = null;
			this.shape = FindOverlaps.Shape.Box;
			this.radius = new FsmFloat
			{
				Value = 1f
			};
			this.box = new FsmVector3
			{
				Value = new Vector3(1f, 1f, 1f)
			};
			this.height = new FsmFloat
			{
				Value = 1f
			};
			this.storeOverlapping = null;
			this.maxOverlaps = new FsmInt
			{
				Value = 50
			};
			this.repeatInterval = new FsmInt
			{
				Value = 1
			};
			this.foundOverlaps = null;
			this.includeSelf = null;
			this.layerMask = null;
			this.invertMask = null;
			this.noOverlaps = null;
			this.debugColor = new FsmColor
			{
				Value = Color.yellow
			};
			this.debug = null;
		}

		// Token: 0x06006E85 RID: 28293 RVA: 0x0022378F File Offset: 0x0022198F
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006E86 RID: 28294 RVA: 0x0022379D File Offset: 0x0022199D
		public bool HideBox()
		{
			return this.shape > FindOverlaps.Shape.Box;
		}

		// Token: 0x06006E87 RID: 28295 RVA: 0x002237A8 File Offset: 0x002219A8
		public bool HideRadius()
		{
			return this.shape != FindOverlaps.Shape.Sphere && this.shape != FindOverlaps.Shape.Capsule;
		}

		// Token: 0x06006E88 RID: 28296 RVA: 0x002237C1 File Offset: 0x002219C1
		public bool HideCapsule()
		{
			return this.shape != FindOverlaps.Shape.Capsule;
		}

		// Token: 0x06006E89 RID: 28297 RVA: 0x002237CF File Offset: 0x002219CF
		public override void OnEnter()
		{
			this.colliders = new Collider[Mathf.Clamp(this.maxOverlaps.Value, 0, int.MaxValue)];
			this.DoGetOverlap();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E8A RID: 28298 RVA: 0x0022380B File Offset: 0x00221A0B
		public override void OnFixedUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoGetOverlap();
			}
		}

		// Token: 0x06006E8B RID: 28299 RVA: 0x0022382C File Offset: 0x00221A2C
		private void DoGetOverlap()
		{
			this.repeat = this.repeatInterval.Value;
			this.InitShapeCenter();
			this.targetMask = this.layerMask.Value;
			if (this.invertMask.Value)
			{
				this.targetMask = ~this.targetMask;
			}
			int num = 0;
			switch (this.shape)
			{
			case FindOverlaps.Shape.Box:
				num = Physics.OverlapBoxNonAlloc(this.center, this.box.Value / 2f, this.colliders, this.orientation, this.targetMask);
				break;
			case FindOverlaps.Shape.Sphere:
				num = Physics.OverlapSphereNonAlloc(this.center, this.radius.Value, this.colliders, this.targetMask);
				break;
			case FindOverlaps.Shape.Capsule:
				num = Physics.OverlapCapsuleNonAlloc(this.capsulePoint1, this.capsulePoint2, this.radius.Value, this.colliders, this.targetMask);
				break;
			}
			if (num == 0)
			{
				this.storeOverlapping.Values = new object[0];
			}
			else if (this.includeSelf.Value)
			{
				this.storeOverlapping.Values = new object[num];
				for (int i = 0; i < num; i++)
				{
					this.storeOverlapping.Values[i] = this.colliders[i].gameObject;
				}
			}
			else
			{
				List<object> list = new List<object>();
				for (int j = 0; j < num; j++)
				{
					GameObject gameObject = this.colliders[j].gameObject;
					if (gameObject != this.cachedGameObject)
					{
						list.Add(gameObject);
					}
				}
				this.storeOverlapping.Values = list.ToArray();
			}
			base.Fsm.Event((num > 0) ? this.foundOverlaps : this.noOverlaps);
		}

		// Token: 0x06006E8C RID: 28300 RVA: 0x002239E8 File Offset: 0x00221BE8
		public void InitShapeCenter()
		{
			this.center = this.offset.Value;
			this.orientation = Quaternion.identity;
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.position);
			if (base.UpdateCachedTransform(ownerDefaultTarget))
			{
				this.center = base.cachedTransform.TransformPoint(this.offset.Value);
				this.orientation = base.cachedTransform.rotation;
				if (this.shape == FindOverlaps.Shape.Capsule)
				{
					float num = this.height.Value / 2f - this.radius.Value;
					this.capsulePoint1 = base.cachedTransform.TransformPoint(new Vector3(0f, -num, 0f));
					this.capsulePoint2 = base.cachedTransform.TransformPoint(new Vector3(0f, num, 0f));
				}
			}
		}

		// Token: 0x04006E1B RID: 28187
		[Tooltip("GameObject position to use for the test shape. Set to none to use world origin (0,0,0).")]
		public FsmOwnerDefault position;

		// Token: 0x04006E1C RID: 28188
		[Tooltip("Offset position of the shape.")]
		public FsmVector3 offset;

		// Token: 0x04006E1D RID: 28189
		[Tooltip("Shape to find overlaps against.")]
		public FindOverlaps.Shape shape;

		// Token: 0x04006E1E RID: 28190
		[HideIf("HideRadius")]
		[Tooltip("Radius of sphere/capsule.")]
		public FsmFloat radius;

		// Token: 0x04006E1F RID: 28191
		[HideIf("HideBox")]
		[Tooltip("Size of box.")]
		public FsmVector3 box;

		// Token: 0x04006E20 RID: 28192
		[HideIf("HideCapsule")]
		[Tooltip("The height of the capsule.")]
		public FsmFloat height;

		// Token: 0x04006E21 RID: 28193
		[Tooltip("Maximum number of overlaps to detect.")]
		public FsmInt maxOverlaps;

		// Token: 0x04006E22 RID: 28194
		[ActionSection("Filter")]
		[UIHint(UIHint.LayerMask)]
		[Tooltip("LayerMask name to filter the overlapping objects")]
		public FsmInt layerMask;

		// Token: 0x04006E23 RID: 28195
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006E24 RID: 28196
		[Tooltip("Include self in the array.")]
		public FsmBool includeSelf;

		// Token: 0x04006E25 RID: 28197
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause Overlaps can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x04006E26 RID: 28198
		[ActionSection("Output")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		[Tooltip("Store overlapping GameObjects in an array.")]
		public FsmArray storeOverlapping;

		// Token: 0x04006E27 RID: 28199
		[Tooltip("Event to send if overlaps were found.")]
		public FsmEvent foundOverlaps;

		// Token: 0x04006E28 RID: 28200
		[Tooltip("Event to send if no overlaps were found.")]
		public FsmEvent noOverlaps;

		// Token: 0x04006E29 RID: 28201
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x04006E2A RID: 28202
		[Tooltip("Draw a gizmo in the scene view to visualize the shape.")]
		public FsmBool debug;

		// Token: 0x04006E30 RID: 28208
		private Collider[] colliders;

		// Token: 0x04006E31 RID: 28209
		private int repeat;

		// Token: 0x02001BB3 RID: 7091
		public enum Shape
		{
			// Token: 0x04009E4B RID: 40523
			Box,
			// Token: 0x04009E4C RID: 40524
			Sphere,
			// Token: 0x04009E4D RID: 40525
			Capsule
		}
	}
}
