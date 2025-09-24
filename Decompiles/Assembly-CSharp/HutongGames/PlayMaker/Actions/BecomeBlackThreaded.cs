using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200129D RID: 4765
	public class BecomeBlackThreaded : FsmStateAction
	{
		// Token: 0x06007D11 RID: 32017 RVA: 0x00255599 File Offset: 0x00253799
		public override void Reset()
		{
			this.Target = null;
			this.WaitForSing = true;
		}

		// Token: 0x06007D12 RID: 32018 RVA: 0x002555B0 File Offset: 0x002537B0
		public override void Awake()
		{
			BlackThreadState blackThreadState = this.GetBlackThreadState();
			if (blackThreadState)
			{
				blackThreadState.ReportWillBeThreaded();
			}
		}

		// Token: 0x06007D13 RID: 32019 RVA: 0x002555D4 File Offset: 0x002537D4
		public override void OnEnter()
		{
			BlackThreadState blackThreadState = this.GetBlackThreadState();
			if (blackThreadState)
			{
				if (this.WaitForSing.Value)
				{
					blackThreadState.BecomeThreaded();
				}
				else
				{
					blackThreadState.BecomeThreadedNoSing();
				}
			}
			base.Finish();
		}

		// Token: 0x06007D14 RID: 32020 RVA: 0x00255614 File Offset: 0x00253814
		private BlackThreadState GetBlackThreadState()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				BlackThreadState componentInParent = safe.GetComponentInParent<BlackThreadState>(true);
				if (componentInParent)
				{
					return componentInParent;
				}
				Debug.LogError("Object \"" + safe.name + "\" does not have a BlackThreadState component", base.Owner);
			}
			else
			{
				Debug.LogError("Target is null", base.Owner);
			}
			return null;
		}

		// Token: 0x04007D22 RID: 32034
		public FsmOwnerDefault Target;

		// Token: 0x04007D23 RID: 32035
		public FsmBool WaitForSing;
	}
}
