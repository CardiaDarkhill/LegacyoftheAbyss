using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010EE RID: 4334
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Translates a Game Object. Use a Vector3 variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class Translate : FsmStateAction
	{
		// Token: 0x0600753E RID: 30014 RVA: 0x0023D928 File Offset: 0x0023BB28
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
			this.perSecond = true;
			this.everyFrame = true;
			this.lateUpdate = false;
			this.fixedUpdate = false;
		}

		// Token: 0x0600753F RID: 30015 RVA: 0x0023D99C File Offset: 0x0023BB9C
		public override void OnPreprocess()
		{
			if (this.fixedUpdate)
			{
				base.Fsm.HandleFixedUpdate = true;
			}
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x06007540 RID: 30016 RVA: 0x0023D9C6 File Offset: 0x0023BBC6
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate && !this.fixedUpdate)
			{
				this.DoTranslate();
				base.Finish();
			}
		}

		// Token: 0x06007541 RID: 30017 RVA: 0x0023D9EC File Offset: 0x0023BBEC
		public override void OnUpdate()
		{
			if (!this.lateUpdate && !this.fixedUpdate)
			{
				this.DoTranslate();
			}
		}

		// Token: 0x06007542 RID: 30018 RVA: 0x0023DA04 File Offset: 0x0023BC04
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoTranslate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007543 RID: 30019 RVA: 0x0023DA22 File Offset: 0x0023BC22
		public override void OnFixedUpdate()
		{
			if (this.fixedUpdate)
			{
				this.DoTranslate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007544 RID: 30020 RVA: 0x0023DA40 File Offset: 0x0023BC40
		private void DoTranslate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = this.vector.IsNone ? new Vector3(this.x.Value, this.y.Value, this.z.Value) : this.vector.Value;
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
			if (!this.perSecond)
			{
				ownerDefaultTarget.transform.Translate(vector, this.space);
				return;
			}
			ownerDefaultTarget.transform.Translate(vector * Time.deltaTime, this.space);
		}

		// Token: 0x040075AD RID: 30125
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040075AE RID: 30126
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("A translation vector. NOTE: You can override individual axis below.")]
		public FsmVector3 vector;

		// Token: 0x040075AF RID: 30127
		[Tooltip("Translation along x axis.")]
		public FsmFloat x;

		// Token: 0x040075B0 RID: 30128
		[Tooltip("Translation along y axis.")]
		public FsmFloat y;

		// Token: 0x040075B1 RID: 30129
		[Tooltip("Translation along z axis.")]
		public FsmFloat z;

		// Token: 0x040075B2 RID: 30130
		[Tooltip("Translate in local or world space.")]
		public Space space;

		// Token: 0x040075B3 RID: 30131
		[Tooltip("Translate over one second")]
		public bool perSecond;

		// Token: 0x040075B4 RID: 30132
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040075B5 RID: 30133
		[Tooltip("Perform the translate in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		// Token: 0x040075B6 RID: 30134
		[Tooltip("Perform the translate in FixedUpdate. This is useful when working with rigid bodies and physics.")]
		public bool fixedUpdate;
	}
}
