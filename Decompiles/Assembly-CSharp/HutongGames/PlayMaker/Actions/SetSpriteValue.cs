using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D56 RID: 3414
	public class SetSpriteValue : FsmStateAction
	{
		// Token: 0x060063F6 RID: 25590 RVA: 0x001F811B File Offset: 0x001F631B
		public override void Reset()
		{
			this.Variable = null;
			this.SetValue = null;
			this.EveryFrame = false;
		}

		// Token: 0x060063F7 RID: 25591 RVA: 0x001F8132 File Offset: 0x001F6332
		public override void OnEnter()
		{
			this.DoSet();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063F8 RID: 25592 RVA: 0x001F8148 File Offset: 0x001F6348
		public override void OnUpdate()
		{
			this.DoSet();
		}

		// Token: 0x060063F9 RID: 25593 RVA: 0x001F8150 File Offset: 0x001F6350
		private void DoSet()
		{
			this.Variable.Value = this.SetValue.Value;
		}

		// Token: 0x04006257 RID: 25175
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject Variable;

		// Token: 0x04006258 RID: 25176
		[RequiredField]
		[ObjectType(typeof(Sprite))]
		public FsmObject SetValue;

		// Token: 0x04006259 RID: 25177
		public bool EveryFrame;
	}
}
