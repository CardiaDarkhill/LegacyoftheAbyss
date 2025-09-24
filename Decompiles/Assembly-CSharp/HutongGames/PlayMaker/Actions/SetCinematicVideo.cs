using System;
using TeamCherry.Cinematics;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F5 RID: 4597
	public class SetCinematicVideo : FsmStateAction
	{
		// Token: 0x06007A7C RID: 31356 RVA: 0x0024C911 File Offset: 0x0024AB11
		public override void Reset()
		{
			this.CinematicPlayer = null;
			this.VideoRef = null;
		}

		// Token: 0x06007A7D RID: 31357 RVA: 0x0024C924 File Offset: 0x0024AB24
		public override void OnEnter()
		{
			GameObject safe = this.CinematicPlayer.GetSafe(this);
			if (safe)
			{
				CinematicPlayer component = safe.GetComponent<CinematicPlayer>();
				if (component)
				{
					component.VideoClip = (this.VideoRef.Value as CinematicVideoReference);
				}
			}
			base.Finish();
		}

		// Token: 0x04007AC1 RID: 31425
		[CheckForComponent(typeof(CinematicPlayer))]
		public FsmOwnerDefault CinematicPlayer;

		// Token: 0x04007AC2 RID: 31426
		[ObjectType(typeof(CinematicVideoReference))]
		public FsmObject VideoRef;
	}
}
