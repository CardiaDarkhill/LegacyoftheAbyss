using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D46 RID: 3398
	[ActionCategory(ActionCategory.Transform)]
	public class SetPosition2DV2 : FsmStateAction
	{
		// Token: 0x060063AE RID: 25518 RVA: 0x001F6E9B File Offset: 0x001F509B
		public bool UsingVector()
		{
			return !this.Vector.IsNone;
		}

		// Token: 0x060063AF RID: 25519 RVA: 0x001F6EAC File Offset: 0x001F50AC
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
			this.active = true;
		}

		// Token: 0x060063B0 RID: 25520 RVA: 0x001F6EFA File Offset: 0x001F50FA
		public override void OnEnter()
		{
			this.DoSetPosition();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063B1 RID: 25521 RVA: 0x001F6F10 File Offset: 0x001F5110
		public override void OnUpdate()
		{
			this.DoSetPosition();
		}

		// Token: 0x060063B2 RID: 25522 RVA: 0x001F6F18 File Offset: 0x001F5118
		private void DoSetPosition()
		{
			if (this.active.Value)
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
		}

		// Token: 0x04006201 RID: 25089
		[RequiredField]
		public FsmOwnerDefault GameObject;

		// Token: 0x04006202 RID: 25090
		public FsmVector2 Vector;

		// Token: 0x04006203 RID: 25091
		[HideIf("UsingVector")]
		public FsmFloat X;

		// Token: 0x04006204 RID: 25092
		[HideIf("UsingVector")]
		public FsmFloat Y;

		// Token: 0x04006205 RID: 25093
		public Space Space;

		// Token: 0x04006206 RID: 25094
		public bool EveryFrame;

		// Token: 0x04006207 RID: 25095
		public FsmBool active;
	}
}
