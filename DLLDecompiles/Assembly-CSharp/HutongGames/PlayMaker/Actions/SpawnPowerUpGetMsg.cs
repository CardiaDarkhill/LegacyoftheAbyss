using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200137F RID: 4991
	public class SpawnPowerUpGetMsg : FsmStateAction
	{
		// Token: 0x0600806A RID: 32874 RVA: 0x0025E5EA File Offset: 0x0025C7EA
		public override void Reset()
		{
			this.MsgPrefab = null;
			this.PowerUp = null;
		}

		// Token: 0x0600806B RID: 32875 RVA: 0x0025E5FA File Offset: 0x0025C7FA
		public override void OnEnter()
		{
			PowerUpGetMsg.Spawn(this.MsgPrefab.Value.GetComponent<PowerUpGetMsg>(), (PowerUpGetMsg.PowerUps)this.PowerUp.Value, new Action(base.Finish));
		}

		// Token: 0x04007FCD RID: 32717
		[RequiredField]
		[CheckForComponent(typeof(PowerUpGetMsg))]
		public FsmGameObject MsgPrefab;

		// Token: 0x04007FCE RID: 32718
		[RequiredField]
		[ObjectType(typeof(PowerUpGetMsg.PowerUps))]
		public FsmEnum PowerUp;
	}
}
