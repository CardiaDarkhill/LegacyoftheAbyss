using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D45 RID: 3397
	[ActionCategory(ActionCategory.Transform)]
	public class SetPosition2D : FsmStateAction
	{
		// Token: 0x060063A8 RID: 25512 RVA: 0x001F6D60 File Offset: 0x001F4F60
		public bool UsingVector()
		{
			return !this.Vector.IsNone;
		}

		// Token: 0x060063A9 RID: 25513 RVA: 0x001F6D70 File Offset: 0x001F4F70
		public override void Reset()
		{
			this.GameObject = null;
			this.Vector = new FsmVector2
			{
				UseVariable = true
			};
			this.X = null;
			this.Y = null;
			this.Space = Space.World;
			this.EveryFrame = false;
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x001F6DA7 File Offset: 0x001F4FA7
		public override void OnEnter()
		{
			this.DoSetPosition();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x001F6DBD File Offset: 0x001F4FBD
		public override void OnUpdate()
		{
			this.DoSetPosition();
		}

		// Token: 0x060063AC RID: 25516 RVA: 0x001F6DC8 File Offset: 0x001F4FC8
		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.GameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector2 vector = (this.Space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition;
			Vector2 position = this.UsingVector() ? this.Vector.Value : new Vector2(this.X.IsNone ? vector.x : this.X.Value, this.Y.IsNone ? vector.y : this.Y.Value);
			if (this.Space == Space.World)
			{
				ownerDefaultTarget.transform.SetPosition2D(position);
				return;
			}
			ownerDefaultTarget.transform.SetLocalPosition2D(position);
		}

		// Token: 0x040061FB RID: 25083
		[RequiredField]
		public FsmOwnerDefault GameObject;

		// Token: 0x040061FC RID: 25084
		public FsmVector2 Vector;

		// Token: 0x040061FD RID: 25085
		[HideIf("UsingVector")]
		public FsmFloat X;

		// Token: 0x040061FE RID: 25086
		[HideIf("UsingVector")]
		public FsmFloat Y;

		// Token: 0x040061FF RID: 25087
		public Space Space;

		// Token: 0x04006200 RID: 25088
		public bool EveryFrame;
	}
}
