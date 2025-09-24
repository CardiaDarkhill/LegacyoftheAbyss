using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBF RID: 3263
	public class LoadSceneSingleCustom : FsmStateAction
	{
		// Token: 0x06006177 RID: 24951 RVA: 0x001EE04F File Offset: 0x001EC24F
		public override void Reset()
		{
			this.LevelName = null;
		}

		// Token: 0x06006178 RID: 24952 RVA: 0x001EE058 File Offset: 0x001EC258
		public override void OnEnter()
		{
			string text = "Scenes/" + this.LevelName.Value;
			this.asyncOperation = ScenePreloader.TakeSceneLoadOperation(text, LoadSceneMode.Single);
			if (this.asyncOperation != null)
			{
				if (this.asyncOperation.Value.IsDone)
				{
					this.asyncOperation.Value.Result.ActivateAsync();
				}
				else
				{
					this.asyncOperation.Value.Completed += delegate(AsyncOperationHandle<SceneInstance> _)
					{
						this.asyncOperation.Value.Result.ActivateAsync();
					};
				}
			}
			else
			{
				this.asyncOperation = new AsyncOperationHandle<SceneInstance>?(Addressables.LoadSceneAsync(text, LoadSceneMode.Single, true, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded));
			}
			GameManager.instance.LastSceneLoad = new SceneLoad(this.asyncOperation.Value, new GameManager.SceneLoadInfo
			{
				IsFirstLevelForPlayer = true,
				SceneName = this.LevelName.Value
			});
			base.Finish();
		}

		// Token: 0x04005FA5 RID: 24485
		[RequiredField]
		public FsmString LevelName;

		// Token: 0x04005FA6 RID: 24486
		private AsyncOperationHandle<SceneInstance>? asyncOperation;
	}
}
