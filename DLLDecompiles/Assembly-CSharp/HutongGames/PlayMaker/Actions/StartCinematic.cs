using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F4 RID: 4596
	public class StartCinematic : FsmStateAction
	{
		// Token: 0x06007A79 RID: 31353 RVA: 0x0024C8C3 File Offset: 0x0024AAC3
		public override void Reset()
		{
			this.CinematicPlayer = null;
		}

		// Token: 0x06007A7A RID: 31354 RVA: 0x0024C8CC File Offset: 0x0024AACC
		public override void OnEnter()
		{
			GameObject safe = this.CinematicPlayer.GetSafe(this);
			if (safe)
			{
				CinematicPlayer component = safe.GetComponent<CinematicPlayer>();
				if (component)
				{
					component.TriggerStartVideo();
				}
			}
			base.Finish();
		}

		// Token: 0x04007AC0 RID: 31424
		[CheckForComponent(typeof(CinematicPlayer))]
		public FsmOwnerDefault CinematicPlayer;
	}
}
