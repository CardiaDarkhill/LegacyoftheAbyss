using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7C RID: 3452
	[ActionCategory("Hollow Knight")]
	public class StartRoarEmitter : FsmStateAction
	{
		// Token: 0x0600649E RID: 25758 RVA: 0x001FC39C File Offset: 0x001FA59C
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

		// Token: 0x0600649F RID: 25759 RVA: 0x001FC3F8 File Offset: 0x001FA5F8
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

		// Token: 0x060064A0 RID: 25760 RVA: 0x001FC4F0 File Offset: 0x001FA6F0
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

		// Token: 0x060064A1 RID: 25761 RVA: 0x001FC544 File Offset: 0x001FA744
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

		// Token: 0x060064A2 RID: 25762 RVA: 0x001FC58C File Offset: 0x001FA78C
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

		// Token: 0x04006393 RID: 25491
		public FsmOwnerDefault spawnPoint;

		// Token: 0x04006394 RID: 25492
		public FsmFloat delay;

		// Token: 0x04006395 RID: 25493
		public FsmBool stunHero;

		// Token: 0x04006396 RID: 25494
		public FsmBool roarBurst;

		// Token: 0x04006397 RID: 25495
		public FsmBool isSmall;

		// Token: 0x04006398 RID: 25496
		public FsmBool noVisualEffect;

		// Token: 0x04006399 RID: 25497
		public FsmBool forceThroughBind;

		// Token: 0x0400639A RID: 25498
		public bool stopOnExit;

		// Token: 0x0400639B RID: 25499
		private float timer;

		// Token: 0x0400639C RID: 25500
		private bool didRoar;

		// Token: 0x0400639D RID: 25501
		private GameObject roarEmitter;

		// Token: 0x0400639E RID: 25502
		private GameObject spawnPointGo;
	}
}
