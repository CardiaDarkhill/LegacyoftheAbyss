using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001321 RID: 4897
	[Tooltip("Needed to make an action to wrap this function because Playmaker CallMethod actions don't run on ScriptableObjects.")]
	public class UnlockCrest : FsmStateAction
	{
		// Token: 0x06007EF6 RID: 32502 RVA: 0x0025A36F File Offset: 0x0025856F
		public override void Reset()
		{
			base.Reset();
			this.Crest = null;
		}

		// Token: 0x06007EF7 RID: 32503 RVA: 0x0025A380 File Offset: 0x00258580
		public override void OnEnter()
		{
			ToolCrest toolCrest = this.Crest.Value as ToolCrest;
			if (toolCrest != null)
			{
				toolCrest.Unlock();
			}
			base.Finish();
		}

		// Token: 0x04007E98 RID: 32408
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;
	}
}
