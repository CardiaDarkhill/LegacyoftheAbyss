using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013AF RID: 5039
	public class StartPreloadingScene : FsmStateAction
	{
		// Token: 0x0600811C RID: 33052 RVA: 0x002605EC File Offset: 0x0025E7EC
		public override void Reset()
		{
			this.SceneName = null;
			this.LoadMode = null;
		}

		// Token: 0x0600811D RID: 33053 RVA: 0x002605FC File Offset: 0x0025E7FC
		public override void OnEnter()
		{
			ScenePreloader.SpawnPreloader(this.SceneName.Value, (LoadSceneMode)this.LoadMode.Value);
			base.Finish();
		}

		// Token: 0x04008057 RID: 32855
		public FsmString SceneName;

		// Token: 0x04008058 RID: 32856
		[ObjectType(typeof(LoadSceneMode))]
		public FsmEnum LoadMode;
	}
}
