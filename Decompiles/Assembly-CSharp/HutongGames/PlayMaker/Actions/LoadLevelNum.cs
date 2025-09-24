using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F34 RID: 3892
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Loads a Level by Index number. Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
	public class LoadLevelNum : FsmStateAction
	{
		// Token: 0x06006C76 RID: 27766 RVA: 0x0021DDAA File Offset: 0x0021BFAA
		public override void Reset()
		{
			this.levelIndex = null;
			this.additive = false;
			this.loadedEvent = null;
			this.dontDestroyOnLoad = false;
		}

		// Token: 0x06006C77 RID: 27767 RVA: 0x0021DDD0 File Offset: 0x0021BFD0
		public override void OnEnter()
		{
			if (!Application.CanStreamedLevelBeLoaded(this.levelIndex.Value))
			{
				base.Fsm.Event(this.failedEvent);
				base.Finish();
				return;
			}
			if (this.dontDestroyOnLoad.Value)
			{
				Object.DontDestroyOnLoad(base.Owner.transform.root.gameObject);
			}
			if (this.additive)
			{
				SceneManager.LoadScene(this.levelIndex.Value, LoadSceneMode.Additive);
			}
			else
			{
				SceneManager.LoadScene(this.levelIndex.Value, LoadSceneMode.Single);
			}
			base.Fsm.Event(this.loadedEvent);
			base.Finish();
		}

		// Token: 0x04006C41 RID: 27713
		[RequiredField]
		[Tooltip("The level index in File->Build Settings")]
		public FsmInt levelIndex;

		// Token: 0x04006C42 RID: 27714
		[Tooltip("Load the level additively, keeping the current scene.")]
		public bool additive;

		// Token: 0x04006C43 RID: 27715
		[Tooltip("Event to send after the level is loaded.")]
		public FsmEvent loadedEvent;

		// Token: 0x04006C44 RID: 27716
		[Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
		public FsmBool dontDestroyOnLoad;

		// Token: 0x04006C45 RID: 27717
		[Tooltip("Event to send if the level cannot be loaded.")]
		public FsmEvent failedEvent;
	}
}
