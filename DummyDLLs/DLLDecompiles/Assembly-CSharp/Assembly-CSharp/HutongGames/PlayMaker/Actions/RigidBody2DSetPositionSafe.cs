using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D01 RID: 3329
	[UsedImplicitly]
	public class RigidBody2DSetPositionSafe : FsmStateAction
	{
		// Token: 0x06006296 RID: 25238 RVA: 0x001F2C6D File Offset: 0x001F0E6D
		public override void Reset()
		{
			this.Target = null;
			this.SetPosition = null;
			this.EveryFrame = false;
		}

		// Token: 0x06006297 RID: 25239 RVA: 0x001F2C84 File Offset: 0x001F0E84
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.body = safe.GetComponent<Rigidbody2D>();
				if (this.body)
				{
					this.DoSet();
					if (this.EveryFrame)
					{
						return;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06006298 RID: 25240 RVA: 0x001F2CD4 File Offset: 0x001F0ED4
		public override void OnUpdate()
		{
			this.DoSet();
		}

		// Token: 0x06006299 RID: 25241 RVA: 0x001F2CDC File Offset: 0x001F0EDC
		private void DoSet()
		{
			this.body.MovePosition(this.SetPosition.Value);
		}

		// Token: 0x040060FE RID: 24830
		[CheckForComponent(typeof(Rigidbody2D))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x040060FF RID: 24831
		[RequiredField]
		public FsmVector2 SetPosition;

		// Token: 0x04006100 RID: 24832
		public bool EveryFrame;

		// Token: 0x04006101 RID: 24833
		private Rigidbody2D body;
	}
}
