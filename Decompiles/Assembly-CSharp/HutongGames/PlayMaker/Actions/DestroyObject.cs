using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EA4 RID: 3748
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroys a Game Object.")]
	public class DestroyObject : FsmStateAction
	{
		// Token: 0x06006A3F RID: 27199 RVA: 0x002135F6 File Offset: 0x002117F6
		public override void Reset()
		{
			this.gameObject = null;
			this.delay = 0f;
		}

		// Token: 0x06006A40 RID: 27200 RVA: 0x00213610 File Offset: 0x00211810
		public override void OnEnter()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				if (this.delay.Value <= 0f)
				{
					Object.Destroy(value);
				}
				else
				{
					Object.Destroy(value, this.delay.Value);
				}
				if (this.detachChildren.Value)
				{
					value.transform.DetachChildren();
				}
			}
			base.Finish();
		}

		// Token: 0x06006A41 RID: 27201 RVA: 0x0021367B File Offset: 0x0021187B
		public override void OnUpdate()
		{
		}

		// Token: 0x0400699C RID: 27036
		[RequiredField]
		[Tooltip("The GameObject to destroy.")]
		public FsmGameObject gameObject;

		// Token: 0x0400699D RID: 27037
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Optional delay before destroying the Game Object.")]
		public FsmFloat delay;

		// Token: 0x0400699E RID: 27038
		[Tooltip("Detach children before destroying the Game Object.")]
		public FsmBool detachChildren;
	}
}
