using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D06 RID: 3334
	public class SceneTravelerTempEval : FsmStateAction
	{
		// Token: 0x060062AE RID: 25262 RVA: 0x001F3191 File Offset: 0x001F1391
		public override void Reset()
		{
			this.SeenPDBool = null;
			this.StillHereEvent = null;
			this.LeftEvent = null;
		}

		// Token: 0x060062AF RID: 25263 RVA: 0x001F31A8 File Offset: 0x001F13A8
		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			string value = this.SeenPDBool.Value;
			SceneTravelerTempEval.TravelerTypes key = (SceneTravelerTempEval.TravelerTypes)this.TravelerType.Value;
			if (this.travelerBools.ContainsKey(key))
			{
				List<SceneTravelerTempEval.TempAreaBools> list = this.travelerBools[key];
				foreach (SceneTravelerTempEval.TempAreaBools tempAreaBools in list)
				{
					if (tempAreaBools.SeenBool == value && instance.GetVariable(tempAreaBools.LeftBool))
					{
						base.Fsm.Event(this.LeftEvent);
						base.Finish();
						return;
					}
				}
				foreach (SceneTravelerTempEval.TempAreaBools tempAreaBools2 in list)
				{
					if (tempAreaBools2.SeenBool != value && instance.GetVariable(tempAreaBools2.SeenBool))
					{
						instance.SetVariable(tempAreaBools2.LeftBool, true);
					}
				}
			}
			base.Fsm.Event(this.StillHereEvent);
			base.Finish();
		}

		// Token: 0x04006119 RID: 24857
		[ObjectType(typeof(SceneTravelerTempEval.TravelerTypes))]
		[RequiredField]
		public FsmEnum TravelerType;

		// Token: 0x0400611A RID: 24858
		public FsmString SeenPDBool;

		// Token: 0x0400611B RID: 24859
		public FsmEvent StillHereEvent;

		// Token: 0x0400611C RID: 24860
		public FsmEvent LeftEvent;

		// Token: 0x0400611D RID: 24861
		private readonly Dictionary<SceneTravelerTempEval.TravelerTypes, List<SceneTravelerTempEval.TempAreaBools>> travelerBools = new Dictionary<SceneTravelerTempEval.TravelerTypes, List<SceneTravelerTempEval.TempAreaBools>>
		{
			{
				SceneTravelerTempEval.TravelerTypes.Mapper,
				new List<SceneTravelerTempEval.TempAreaBools>
				{
					new SceneTravelerTempEval.TempAreaBools("SeenMapperBoneForest", "MapperLeftBoneForest"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperDocks", "MapperLeftDocks"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperWilds", "MapperLeftWilds"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperCrawl", "MapperLeftCrawl"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperGreymoor", "MapperLeftGreymoor"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperBellhart", "MapperLeftBellhart"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperShellwood", "MapperLeftShellwood"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperHuntersNest", "MapperLeftHuntersNest"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperJudgeSteps", "MapperLeftJudgeSteps"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperDustpens", "MapperLeftDustpens"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperPeak", "MapperLeftPeak"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperShadow", "MapperLeftShadow"),
					new SceneTravelerTempEval.TempAreaBools("SeenMapperCoralCaverns", "MapperLeftCoralCaverns")
				}
			}
		};

		// Token: 0x02001B86 RID: 7046
		public enum TravelerTypes
		{
			// Token: 0x04009D7E RID: 40318
			Mapper
		}

		// Token: 0x02001B87 RID: 7047
		private class TempAreaBools
		{
			// Token: 0x06009A39 RID: 39481 RVA: 0x002B2F49 File Offset: 0x002B1149
			public TempAreaBools(string seenBool, string leftBool)
			{
				this.SeenBool = seenBool;
				this.LeftBool = leftBool;
			}

			// Token: 0x04009D7F RID: 40319
			public string SeenBool;

			// Token: 0x04009D80 RID: 40320
			public string LeftBool;
		}
	}
}
