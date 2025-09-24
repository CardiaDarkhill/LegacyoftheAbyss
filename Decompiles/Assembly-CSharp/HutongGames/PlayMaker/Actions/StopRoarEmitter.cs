using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D82 RID: 3458
	[ActionCategory("Hollow Knight")]
	public class StopRoarEmitter : FsmStateAction
	{
		// Token: 0x060064B6 RID: 25782 RVA: 0x001FCB60 File Offset: 0x001FAD60
		public override void Reset()
		{
			this.delay = null;
			this.didRoar = false;
			this.timer = 0f;
		}

		// Token: 0x060064B7 RID: 25783 RVA: 0x001FCB7B File Offset: 0x001FAD7B
		public override void OnEnter()
		{
			if (this.delay.Value < 0.001f)
			{
				this.StopRoar();
			}
		}

		// Token: 0x060064B8 RID: 25784 RVA: 0x001FCB98 File Offset: 0x001FAD98
		public override void OnUpdate()
		{
			if (!this.didRoar)
			{
				if (this.timer < this.delay.Value)
				{
					this.timer += Time.deltaTime;
				}
				if (this.timer >= this.delay.Value)
				{
					this.StopRoar();
				}
			}
		}

		// Token: 0x060064B9 RID: 25785 RVA: 0x001FCBEB File Offset: 0x001FADEB
		private void StopRoar()
		{
			this.StopEmitter("Roar Wave Emitter");
			this.StopEmitter("Roar Wave Emitter Small");
			FSMUtility.SendEventToGameObject(GameManager.instance.hero_ctrl.gameObject, "ROAR EXIT", false);
			this.didRoar = true;
			base.Finish();
		}

		// Token: 0x060064BA RID: 25786 RVA: 0x001FCC2A File Offset: 0x001FAE2A
		private void StopEmitter(string name)
		{
			FSMUtility.SendEventToGameObject(GameManager.instance.gameCams.gameObject.transform.Find(name).gameObject, "END", false);
		}

		// Token: 0x040063B1 RID: 25521
		public FsmFloat delay;

		// Token: 0x040063B2 RID: 25522
		private float timer;

		// Token: 0x040063B3 RID: 25523
		private bool didRoar;
	}
}
