using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200138E RID: 5006
	public class SpawnSkillGetMsg : FsmStateAction
	{
		// Token: 0x0600809E RID: 32926 RVA: 0x0025ECBD File Offset: 0x0025CEBD
		public override void Reset()
		{
			this.MsgPrefab = null;
			this.Skill = null;
		}

		// Token: 0x0600809F RID: 32927 RVA: 0x0025ECCD File Offset: 0x0025CECD
		public override void OnEnter()
		{
			SkillGetMsg.Spawn(this.MsgPrefab.Value.GetComponent<SkillGetMsg>(), this.Skill.Value as ToolItemSkill, new Action(base.Finish));
		}

		// Token: 0x04007FEB RID: 32747
		[RequiredField]
		[CheckForComponent(typeof(SkillGetMsg))]
		public FsmGameObject MsgPrefab;

		// Token: 0x04007FEC RID: 32748
		[RequiredField]
		[ObjectType(typeof(ToolItemSkill))]
		public FsmObject Skill;
	}
}
