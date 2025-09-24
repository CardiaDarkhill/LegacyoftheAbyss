using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F53 RID: 3923
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if a GameObject Variable has a null value. E.g., If the FindGameObject action failed to find an object.")]
	public class GameObjectIsNull : FsmStateAction
	{
		// Token: 0x06006D07 RID: 27911 RVA: 0x0021F33F File Offset: 0x0021D53F
		public override void Reset()
		{
			this.gameObject = null;
			this.isNull = null;
			this.isNotNull = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D08 RID: 27912 RVA: 0x0021F364 File Offset: 0x0021D564
		public override void OnEnter()
		{
			this.DoIsGameObjectNull();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D09 RID: 27913 RVA: 0x0021F37A File Offset: 0x0021D57A
		public override void OnUpdate()
		{
			this.DoIsGameObjectNull();
		}

		// Token: 0x06006D0A RID: 27914 RVA: 0x0021F384 File Offset: 0x0021D584
		private void DoIsGameObjectNull()
		{
			bool flag = this.gameObject.Value == null;
			if (this.storeResult != null)
			{
				this.storeResult.Value = flag;
			}
			base.Fsm.Event(flag ? this.isNull : this.isNotNull);
		}

		// Token: 0x04006CCA RID: 27850
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The GameObject variable to test.")]
		public FsmGameObject gameObject;

		// Token: 0x04006CCB RID: 27851
		[Tooltip("Event to send if the GamObject is null.")]
		public FsmEvent isNull;

		// Token: 0x04006CCC RID: 27852
		[Tooltip("Event to send if the GamObject is NOT null.")]
		public FsmEvent isNotNull;

		// Token: 0x04006CCD RID: 27853
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006CCE RID: 27854
		[Tooltip("Repeat every frame. Useful if you want to wait for a GameObject variable to be not null (or null) then send an event.")]
		public bool everyFrame;
	}
}
