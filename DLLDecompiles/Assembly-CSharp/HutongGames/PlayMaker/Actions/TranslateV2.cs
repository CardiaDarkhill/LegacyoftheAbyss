using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA0 RID: 3488
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Translates a Game Object. Use a Vector3 variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class TranslateV2 : FsmStateAction
	{
		// Token: 0x06006545 RID: 25925 RVA: 0x001FF430 File Offset: 0x001FD630
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
			this.alwaysOnStart = false;
		}

		// Token: 0x06006546 RID: 25926 RVA: 0x001FF4AB File Offset: 0x001FD6AB
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006547 RID: 25927 RVA: 0x001FF4B9 File Offset: 0x001FD6B9
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006548 RID: 25928 RVA: 0x001FF4C7 File Offset: 0x001FD6C7
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate && !this.fixedUpdate)
			{
				this.DoTranslate();
				base.Finish();
			}
			if (this.alwaysOnStart)
			{
				this.DoTranslate();
			}
		}

		// Token: 0x06006549 RID: 25929 RVA: 0x001FF4FB File Offset: 0x001FD6FB
		public override void OnUpdate()
		{
			if (!this.lateUpdate && !this.fixedUpdate)
			{
				this.DoTranslate();
			}
		}

		// Token: 0x0600654A RID: 25930 RVA: 0x001FF513 File Offset: 0x001FD713
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

		// Token: 0x0600654B RID: 25931 RVA: 0x001FF531 File Offset: 0x001FD731
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

		// Token: 0x0600654C RID: 25932 RVA: 0x001FF550 File Offset: 0x001FD750
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
			}
			else
			{
				ownerDefaultTarget.transform.Translate(vector * Time.deltaTime, this.space);
			}
			if (!this.yMin.IsNone && ownerDefaultTarget.transform.position.y < this.yMin.Value)
			{
				ownerDefaultTarget.transform.position = new Vector3(ownerDefaultTarget.transform.position.x, this.yMin.Value, ownerDefaultTarget.transform.position.z);
			}
			if (!this.yMax.IsNone && ownerDefaultTarget.transform.position.y > this.yMax.Value)
			{
				ownerDefaultTarget.transform.position = new Vector3(ownerDefaultTarget.transform.position.x, this.yMax.Value, ownerDefaultTarget.transform.position.z);
			}
		}

		// Token: 0x04006442 RID: 25666
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006443 RID: 25667
		[UIHint(UIHint.Variable)]
		[Tooltip("A translation vector. NOTE: You can override individual axis below.")]
		public FsmVector3 vector;

		// Token: 0x04006444 RID: 25668
		[Tooltip("Translation along x axis.")]
		public FsmFloat x;

		// Token: 0x04006445 RID: 25669
		[Tooltip("Translation along y axis.")]
		public FsmFloat y;

		// Token: 0x04006446 RID: 25670
		[Tooltip("Translation along z axis.")]
		public FsmFloat z;

		// Token: 0x04006447 RID: 25671
		[Tooltip("Translate in local or world space.")]
		public Space space;

		// Token: 0x04006448 RID: 25672
		[Tooltip("Translate over one second")]
		public bool perSecond;

		// Token: 0x04006449 RID: 25673
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400644A RID: 25674
		[Tooltip("Perform the translate in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		// Token: 0x0400644B RID: 25675
		[Tooltip("Perform the translate in FixedUpdate. This is useful when working with rigid bodies and physics.")]
		public bool fixedUpdate;

		// Token: 0x0400644C RID: 25676
		public bool alwaysOnStart;

		// Token: 0x0400644D RID: 25677
		public FsmFloat yMin;

		// Token: 0x0400644E RID: 25678
		public FsmFloat yMax;
	}
}
