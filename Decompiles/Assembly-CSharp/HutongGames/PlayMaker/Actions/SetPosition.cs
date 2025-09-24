using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E3 RID: 4323
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetPosition : ComponentAction<Transform>
	{
		// Token: 0x060074FB RID: 29947 RVA: 0x0023C5FC File Offset: 0x0023A7FC
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.Self;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x060074FC RID: 29948 RVA: 0x0023C662 File Offset: 0x0023A862
		public override void OnPreprocess()
		{
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x060074FD RID: 29949 RVA: 0x0023C678 File Offset: 0x0023A878
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoSetPosition();
				base.Finish();
			}
		}

		// Token: 0x060074FE RID: 29950 RVA: 0x0023C696 File Offset: 0x0023A896
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetPosition();
			}
		}

		// Token: 0x060074FF RID: 29951 RVA: 0x0023C6A6 File Offset: 0x0023A8A6
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetPosition();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007500 RID: 29952 RVA: 0x0023C6C4 File Offset: 0x0023A8C4
		private void DoSetPosition()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space == Space.World) ? base.cachedTransform.position : base.cachedTransform.localPosition);
			}
			else
			{
				vector = this.vector.Value;
			}
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				vector.z = this.z.Value;
			}
			if (this.space == Space.World)
			{
				base.cachedTransform.position = vector;
				return;
			}
			base.cachedTransform.localPosition = vector;
		}

		// Token: 0x04007552 RID: 30034
		[RequiredField]
		[Tooltip("The Game Object to position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007553 RID: 30035
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored Vector3 position, and/or set individual axis below.")]
		public FsmVector3 vector;

		// Token: 0x04007554 RID: 30036
		[Tooltip("Set the X position.")]
		public FsmFloat x;

		// Token: 0x04007555 RID: 30037
		[Tooltip("Set the Y position.")]
		public FsmFloat y;

		// Token: 0x04007556 RID: 30038
		[Tooltip("Set the Z position.")]
		public FsmFloat z;

		// Token: 0x04007557 RID: 30039
		[Tooltip("Set position in local (relative to parent) or world space.")]
		public Space space;

		// Token: 0x04007558 RID: 30040
		[Tooltip("Perform this action every frame. Useful if position is changing.")]
		public bool everyFrame;

		// Token: 0x04007559 RID: 30041
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
