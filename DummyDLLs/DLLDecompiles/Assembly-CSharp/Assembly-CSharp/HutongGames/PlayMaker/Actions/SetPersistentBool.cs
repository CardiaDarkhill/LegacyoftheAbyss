using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A9 RID: 5033
	public class SetPersistentBool : FsmStateAction
	{
		// Token: 0x06008109 RID: 33033 RVA: 0x0026027C File Offset: 0x0025E47C
		public override void Reset()
		{
			this.Target = null;
			this.SetValue = null;
		}

		// Token: 0x0600810A RID: 33034 RVA: 0x0026028C File Offset: 0x0025E48C
		public override void OnEnter()
		{
			PersistentBoolItem safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				safe.SetValueOverride(this.SetValue.Value);
			}
			base.Finish();
		}

		// Token: 0x04008048 RID: 32840
		[RequiredField]
		[ObjectType(typeof(PersistentBoolItem))]
		public FsmOwnerDefault Target;

		// Token: 0x04008049 RID: 32841
		public FsmBool SetValue;
	}
}
