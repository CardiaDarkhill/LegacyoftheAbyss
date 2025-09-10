using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFF RID: 3327
	[UsedImplicitly]
	public class RigidBody2DGetPosition : FsmStateAction
	{
		// Token: 0x0600628E RID: 25230 RVA: 0x001F2B43 File Offset: 0x001F0D43
		public override void Reset()
		{
			this.Target = null;
			this.Position = null;
			this.EveryFrame = false;
		}

		// Token: 0x0600628F RID: 25231 RVA: 0x001F2B5C File Offset: 0x001F0D5C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.body = safe.GetComponent<Rigidbody2D>();
				if (this.body)
				{
					this.DoGet();
					if (this.EveryFrame)
					{
						return;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06006290 RID: 25232 RVA: 0x001F2BAC File Offset: 0x001F0DAC
		public override void OnUpdate()
		{
			this.DoGet();
		}

		// Token: 0x06006291 RID: 25233 RVA: 0x001F2BB4 File Offset: 0x001F0DB4
		private void DoGet()
		{
			this.Position.Value = this.body.position;
		}

		// Token: 0x040060F7 RID: 24823
		[CheckForComponent(typeof(Rigidbody2D))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x040060F8 RID: 24824
		[UIHint(UIHint.Variable)]
		public FsmVector2 Position;

		// Token: 0x040060F9 RID: 24825
		public bool EveryFrame;

		// Token: 0x040060FA RID: 24826
		private Rigidbody2D body;
	}
}
