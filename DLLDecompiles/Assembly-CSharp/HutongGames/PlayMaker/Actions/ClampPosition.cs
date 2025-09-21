using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D5 RID: 4309
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Clamps a position to min/max ranges. Set any limit to None to leave un-clamped.")]
	public class ClampPosition : ComponentAction<Transform>
	{
		// Token: 0x060074A7 RID: 29863 RVA: 0x0023AF68 File Offset: 0x00239168
		public override void Reset()
		{
			this.gameObject = null;
			this.minX = new FsmFloat
			{
				UseVariable = true
			};
			this.maxX = new FsmFloat
			{
				UseVariable = true
			};
			this.minY = new FsmFloat
			{
				UseVariable = true
			};
			this.maxY = new FsmFloat
			{
				UseVariable = true
			};
			this.minZ = new FsmFloat
			{
				UseVariable = true
			};
			this.maxZ = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.Self;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x060074A8 RID: 29864 RVA: 0x0023AFFD File Offset: 0x002391FD
		public override void OnPreprocess()
		{
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x060074A9 RID: 29865 RVA: 0x0023B013 File Offset: 0x00239213
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoClampPosition();
				base.Finish();
			}
		}

		// Token: 0x060074AA RID: 29866 RVA: 0x0023B031 File Offset: 0x00239231
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoClampPosition();
			}
		}

		// Token: 0x060074AB RID: 29867 RVA: 0x0023B041 File Offset: 0x00239241
		public override void OnLateUpdate()
		{
			this.DoClampPosition();
			if (this.lateUpdate)
			{
				this.DoClampPosition();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074AC RID: 29868 RVA: 0x0023B068 File Offset: 0x00239268
		private void DoClampPosition()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector = (this.space == Space.World) ? base.cachedTransform.position : base.cachedTransform.localPosition;
			if (!this.minX.IsNone)
			{
				vector.x = Mathf.Max(this.minX.Value, vector.x);
			}
			if (!this.maxX.IsNone)
			{
				vector.x = Mathf.Min(this.maxX.Value, vector.x);
			}
			if (!this.minY.IsNone)
			{
				vector.y = Mathf.Max(this.minY.Value, vector.y);
			}
			if (!this.maxY.IsNone)
			{
				vector.y = Mathf.Min(this.maxY.Value, vector.y);
			}
			if (!this.minZ.IsNone)
			{
				vector.z = Mathf.Max(this.minZ.Value, vector.z);
			}
			if (!this.maxZ.IsNone)
			{
				vector.z = Mathf.Min(this.maxZ.Value, vector.z);
			}
			if (this.space == Space.World)
			{
				base.cachedTransform.position = vector;
				return;
			}
			base.cachedTransform.localPosition = vector;
		}

		// Token: 0x040074E1 RID: 29921
		[RequiredField]
		[Tooltip("The GameObject to clamp position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040074E2 RID: 29922
		[Tooltip("Clamp the minimum value of x.")]
		public FsmFloat minX;

		// Token: 0x040074E3 RID: 29923
		[Tooltip("Clamp the maximum value of x.")]
		public FsmFloat maxX;

		// Token: 0x040074E4 RID: 29924
		[Tooltip("Clamp the minimum value of y.")]
		public FsmFloat minY;

		// Token: 0x040074E5 RID: 29925
		[Tooltip("Clamp the maximum value of y.")]
		public FsmFloat maxY;

		// Token: 0x040074E6 RID: 29926
		[Tooltip("Clamp the minimum value of z.")]
		public FsmFloat minZ;

		// Token: 0x040074E7 RID: 29927
		[Tooltip("Clamp the maximum value of z.")]
		public FsmFloat maxZ;

		// Token: 0x040074E8 RID: 29928
		[Tooltip("Clamp position in local (relative to parent) or world space.")]
		public Space space;

		// Token: 0x040074E9 RID: 29929
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040074EA RID: 29930
		[Tooltip("Perform in LateUpdate. This is useful if you want to clamp the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
