using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000781 RID: 1921
public class SceneLintLogger : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x0600442B RID: 17451 RVA: 0x0012B21C File Offset: 0x0012941C
	private void OnValidate()
	{
		if (this.logProviders == null)
		{
			return;
		}
		for (int i = this.logProviders.Count - 1; i >= 0; i--)
		{
			Object @object = this.logProviders[i];
			if (!(@object == null) && !(@object is SceneLintLogger.ILogProvider))
			{
				this.logProviders[i] = null;
			}
		}
	}

	// Token: 0x0600442C RID: 17452 RVA: 0x0012B278 File Offset: 0x00129478
	private void Awake()
	{
		if (this.logProviders == null)
		{
			return;
		}
		for (int i = this.logProviders.Count - 1; i >= 0; i--)
		{
			Object @object = this.logProviders[i];
			if (@object == null || !(@object is SceneLintLogger.ILogProvider))
			{
				this.logProviders.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600442D RID: 17453 RVA: 0x0012B2D0 File Offset: 0x001294D0
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		StringBuilder stringBuilder = null;
		string name = base.gameObject.scene.name;
		foreach (SceneLintLogger.ILogProvider logProvider in this.EnumerateLogProviders())
		{
			string sceneLintLog = logProvider.GetSceneLintLog(name);
			if (!string.IsNullOrEmpty(sceneLintLog))
			{
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder("SceneLintLogger: ");
					stringBuilder.Append(sceneLintLog);
				}
				else
				{
					stringBuilder.Append(", ");
					stringBuilder.Append(sceneLintLog);
				}
			}
		}
		if (stringBuilder == null)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600442E RID: 17454 RVA: 0x0012B378 File Offset: 0x00129578
	public IEnumerable<SceneLintLogger.ILogProvider> EnumerateLogProviders()
	{
		foreach (Object @object in this.logProviders)
		{
			SceneLintLogger.ILogProvider logProvider = @object as SceneLintLogger.ILogProvider;
			if (logProvider != null)
			{
				yield return logProvider;
			}
		}
		List<Object>.Enumerator enumerator = default(List<Object>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x04004561 RID: 17761
	[SerializeField]
	private List<Object> logProviders;

	// Token: 0x02001A59 RID: 6745
	public interface ILogProvider
	{
		// Token: 0x06009698 RID: 38552
		string GetSceneLintLog(string sceneName);
	}
}
