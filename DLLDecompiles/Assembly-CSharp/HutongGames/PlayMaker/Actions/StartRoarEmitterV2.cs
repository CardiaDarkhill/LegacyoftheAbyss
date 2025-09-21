using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7D RID: 3453
	[ActionCategory("Hollow Knight")]
	public sealed class StartRoarEmitterV2 : FsmStateAction
	{
		// Token: 0x060064A4 RID: 25764 RVA: 0x001FC684 File Offset: 0x001FA884
		public override void Reset()
		{
			this.spawnPoint = null;
			this.delay = null;
			this.didRoar = false;
			this.noVisualEffect = false;
			this.isSmall = false;
			this.timer = 0f;
			this.roarBurst = null;
			this.stunHero = null;
			this.forceThroughBind = null;
		}

		// Token: 0x060064A5 RID: 25765 RVA: 0x001FC6E0 File Offset: 0x001FA8E0
		public override void OnEnter()
		{
			string n = this.isSmall.Value ? "Roar Wave Emitter Small" : "Roar Wave Emitter";
			this.roarEmitter = GameManager.instance.gameCams.gameObject.transform.Find(n).gameObject;
			if (this.roarEmitter == null)
			{
				base.Finish();
				return;
			}
			this.spawnPointGo = base.Fsm.GetOwnerDefaultTarget(this.spawnPoint);
			if (this.spawnPointGo == null)
			{
				base.Finish();
				return;
			}
			this.roarEmitter.transform.position = this.spawnPointGo.transform.position;
			GameObject gameObject = GameManager.instance.hero_ctrl.gameObject;
			if (gameObject != null && this.stunHero.Value)
			{
				FSMUtility.SendEventToGameObject(gameObject, "ROAR WOUND CANCEL", false);
			}
			if (this.delay.Value < 0.001f)
			{
				this.StartRoar();
			}
		}

		// Token: 0x060064A6 RID: 25766 RVA: 0x001FC7D8 File Offset: 0x001FA9D8
		public override void OnUpdate()
		{
			if (this.didRoar)
			{
				return;
			}
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer >= this.delay.Value)
			{
				this.StartRoar();
			}
		}

		// Token: 0x060064A7 RID: 25767 RVA: 0x001FC82C File Offset: 0x001FAA2C
		public override void OnExit()
		{
			if (!this.stopOnExit)
			{
				return;
			}
			FSMUtility.SendEventToGameObject(this.roarEmitter, "END", false);
			HeroController silentInstance = HeroController.SilentInstance;
			if (!silentInstance)
			{
				return;
			}
			FSMUtility.SendEventToGameObject(silentInstance.gameObject, "ROAR EXIT", false);
		}

		// Token: 0x060064A8 RID: 25768 RVA: 0x001FC874 File Offset: 0x001FAA74
		private void StartRoar()
		{
			Vector3 position = this.spawnPointGo.transform.position;
			this.roarEmitter.transform.position = new Vector3(position.x, position.y, 0f);
			if (!this.noVisualEffect.Value)
			{
				FSMUtility.SendEventToGameObject(this.roarEmitter, this.roarBurst.Value ? "BURST" : "START", false);
			}
			if (this.stunHero.Value)
			{
				GameObject gameObject = GameManager.instance.hero_ctrl.gameObject;
				if (this.noRecoil.Value)
				{
					PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(gameObject, "Roar and Wound States");
					if (playMakerFSM)
					{
						playMakerFSM.FsmVariables.FindFsmFloat("Push Strength").Value = 0f;
					}
				}
				if (this.forceThroughBind.Value)
				{
					FSMUtility.SendEventToGameObject(gameObject, this.roarBurst.Value ? "ROAR BURST ENTER FORCED" : "ROAR ENTER FORCED", false);
				}
				else
				{
					FSMUtility.SendEventToGameObject(gameObject, this.roarBurst.Value ? "ROAR BURST ENTER" : "ROAR ENTER", false);
				}
			}
			this.didRoar = true;
			base.Finish();
		}

		// Token: 0x0400639F RID: 25503
		public FsmOwnerDefault spawnPoint;

		// Token: 0x040063A0 RID: 25504
		public FsmFloat delay;

		// Token: 0x040063A1 RID: 25505
		public FsmBool stunHero;

		// Token: 0x040063A2 RID: 25506
		public FsmBool roarBurst;

		// Token: 0x040063A3 RID: 25507
		public FsmBool isSmall;

		// Token: 0x040063A4 RID: 25508
		public FsmBool noVisualEffect;

		// Token: 0x040063A5 RID: 25509
		public FsmBool forceThroughBind;

		// Token: 0x040063A6 RID: 25510
		public FsmBool noRecoil;

		// Token: 0x040063A7 RID: 25511
		public bool stopOnExit;

		// Token: 0x040063A8 RID: 25512
		private float timer;

		// Token: 0x040063A9 RID: 25513
		private bool didRoar;

		// Token: 0x040063AA RID: 25514
		private GameObject roarEmitter;

		// Token: 0x040063AB RID: 25515
		private GameObject spawnPointGo;
	}
}
