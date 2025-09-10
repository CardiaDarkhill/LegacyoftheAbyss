using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E4 RID: 4324
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the 2d Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetPosition2d : FsmStateAction
	{
		// Token: 0x06007502 RID: 29954 RVA: 0x0023C7B0 File Offset: 0x0023A9B0
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
			this.space = Space.Self;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x06007503 RID: 29955 RVA: 0x0023C804 File Offset: 0x0023AA04
		public override void OnPreprocess()
		{
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x06007504 RID: 29956 RVA: 0x0023C81A File Offset: 0x0023AA1A
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoSetPosition();
				base.Finish();
			}
		}

		// Token: 0x06007505 RID: 29957 RVA: 0x0023C838 File Offset: 0x0023AA38
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetPosition();
			}
		}

		// Token: 0x06007506 RID: 29958 RVA: 0x0023C848 File Offset: 0x0023AA48
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

		// Token: 0x06007507 RID: 29959 RVA: 0x0023C868 File Offset: 0x0023AA68
		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector2 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition);
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
			if (this.space == Space.World)
			{
				ownerDefaultTarget.transform.position = new Vector3(vector.x, vector.y, ownerDefaultTarget.transform.position.z);
				return;
			}
			ownerDefaultTarget.transform.localPosition = new Vector3(vector.x, vector.y, ownerDefaultTarget.transform.localPosition.z);
		}

		// Token: 0x0400755A RID: 30042
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400755B RID: 30043
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored Vector2 position, and/or set individual axis below.")]
		public FsmVector2 vector;

		// Token: 0x0400755C RID: 30044
		[Tooltip("Set the X position.")]
		public FsmFloat x;

		// Token: 0x0400755D RID: 30045
		[Tooltip("Set the Y position.")]
		public FsmFloat y;

		// Token: 0x0400755E RID: 30046
		[Tooltip("Use local or world space.")]
		public Space space;

		// Token: 0x0400755F RID: 30047
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007560 RID: 30048
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
