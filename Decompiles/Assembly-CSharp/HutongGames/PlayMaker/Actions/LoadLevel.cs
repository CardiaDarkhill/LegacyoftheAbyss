using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F33 RID: 3891
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Loads a Level by Name. NOTE: Before you can load a level, you have to add it to the list of levels defined in File->Build Settings...")]
	public class LoadLevel : FsmStateAction
	{
		// Token: 0x06006C72 RID: 27762 RVA: 0x0021DBF3 File Offset: 0x0021BDF3
		public override void Reset()
		{
			this.levelName = "";
			this.additive = false;
			this.async = false;
			this.loadedEvent = null;
			this.dontDestroyOnLoad = false;
		}

		// Token: 0x06006C73 RID: 27763 RVA: 0x0021DC28 File Offset: 0x0021BE28
		public override void OnEnter()
		{
			if (!Application.CanStreamedLevelBeLoaded(this.levelName.Value))
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
				if (this.async)
				{
					this.asyncOperation = SceneManager.LoadSceneAsync(this.levelName.Value, LoadSceneMode.Additive);
					Debug.Log("LoadLevelAdditiveAsyc: " + this.levelName.Value);
					return;
				}
				SceneManager.LoadScene(this.levelName.Value, LoadSceneMode.Additive);
				Debug.Log("LoadLevelAdditive: " + this.levelName.Value);
			}
			else
			{
				if (this.async)
				{
					this.asyncOperation = SceneManager.LoadSceneAsync(this.levelName.Value, LoadSceneMode.Single);
					Debug.Log("LoadLevelAsync: " + this.levelName.Value);
					return;
				}
				SceneManager.LoadScene(this.levelName.Value, LoadSceneMode.Single);
				Debug.Log("LoadLevel: " + this.levelName.Value);
			}
			base.Log("LOAD COMPLETE");
			base.Fsm.Event(this.loadedEvent);
			base.Finish();
		}

		// Token: 0x06006C74 RID: 27764 RVA: 0x0021DD7C File Offset: 0x0021BF7C
		public override void OnUpdate()
		{
			if (this.asyncOperation.isDone)
			{
				base.Fsm.Event(this.loadedEvent);
				base.Finish();
			}
		}

		// Token: 0x04006C3A RID: 27706
		[RequiredField]
		[Tooltip("The name of the level to load. NOTE: Must be in the list of levels defined in File->Build Settings... ")]
		public FsmString levelName;

		// Token: 0x04006C3B RID: 27707
		[Tooltip("Load the level additively, keeping the current scene.")]
		public bool additive;

		// Token: 0x04006C3C RID: 27708
		[Tooltip("Load the level asynchronously in the background.")]
		public bool async;

		// Token: 0x04006C3D RID: 27709
		[Tooltip("Event to send when the level has loaded. NOTE: This only makes sense if the FSM is still in the scene!")]
		public FsmEvent loadedEvent;

		// Token: 0x04006C3E RID: 27710
		[Tooltip("Keep this GameObject in the new level. NOTE: The GameObject and components is disabled then enabled on load; uncheck Reset On Disable to keep the active state.")]
		public FsmBool dontDestroyOnLoad;

		// Token: 0x04006C3F RID: 27711
		[Tooltip("Event to send if the level cannot be loaded.")]
		public FsmEvent failedEvent;

		// Token: 0x04006C40 RID: 27712
		private AsyncOperation asyncOperation;
	}
}
