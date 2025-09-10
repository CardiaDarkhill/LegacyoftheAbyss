using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000789 RID: 1929
public static class StaticVariableList
{
	// Token: 0x0600446E RID: 17518 RVA: 0x0012BBB2 File Offset: 0x00129DB2
	public static void Clear()
	{
		Dictionary<string, StaticVariableList.StaticVariable> variables = StaticVariableList._variables;
		if (variables == null)
		{
			return;
		}
		variables.Clear();
	}

	// Token: 0x0600446F RID: 17519 RVA: 0x0012BBC4 File Offset: 0x00129DC4
	public static void ClearSceneTransitions()
	{
		if (StaticVariableList._variables == null)
		{
			return;
		}
		if (StaticVariableList._removeKeys == null)
		{
			StaticVariableList._removeKeys = new List<string>();
		}
		foreach (KeyValuePair<string, StaticVariableList.StaticVariable> keyValuePair in StaticVariableList._variables)
		{
			if (keyValuePair.Value.SceneTransitionsLeft > 0 && !(keyValuePair.Key == "doubleJumpNpcPresent"))
			{
				StaticVariableList._removeKeys.Add(keyValuePair.Key);
			}
		}
		foreach (string key in StaticVariableList._removeKeys)
		{
			StaticVariableList._variables.Remove(key);
		}
		StaticVariableList._removeKeys.Clear();
	}

	// Token: 0x06004470 RID: 17520 RVA: 0x0012BCAC File Offset: 0x00129EAC
	public static void SetValue(string variableName, object value, int sceneTransitionsLimit = 0)
	{
		if (StaticVariableList._variables == null)
		{
			StaticVariableList._variables = new Dictionary<string, StaticVariableList.StaticVariable>();
		}
		StaticVariableList._variables[variableName] = new StaticVariableList.StaticVariable
		{
			Value = value,
			SceneTransitionsLeft = sceneTransitionsLimit
		};
	}

	// Token: 0x06004471 RID: 17521 RVA: 0x0012BCE0 File Offset: 0x00129EE0
	public static T GetValue<T>(string variableName)
	{
		if (StaticVariableList._variables != null && StaticVariableList._variables.ContainsKey(variableName))
		{
			return (T)((object)StaticVariableList._variables[variableName].Value);
		}
		Debug.LogError("Attempt to get " + variableName + " from static variable list failed!");
		return default(T);
	}

	// Token: 0x06004472 RID: 17522 RVA: 0x0012BD35 File Offset: 0x00129F35
	public static object GetValue(string variableName)
	{
		if (StaticVariableList._variables != null && StaticVariableList._variables.ContainsKey(variableName))
		{
			return StaticVariableList._variables[variableName].Value;
		}
		Debug.LogError("Attempt to get " + variableName + " from static variable list failed!");
		return null;
	}

	// Token: 0x06004473 RID: 17523 RVA: 0x0012BD72 File Offset: 0x00129F72
	public static T GetValue<T>(string variableName, T defaultValue)
	{
		if (StaticVariableList._variables == null || !StaticVariableList._variables.ContainsKey(variableName))
		{
			return defaultValue;
		}
		return (T)((object)StaticVariableList._variables[variableName].Value);
	}

	// Token: 0x06004474 RID: 17524 RVA: 0x0012BD9F File Offset: 0x00129F9F
	public static bool Exists(string variableName)
	{
		return StaticVariableList._variables != null && StaticVariableList._variables.ContainsKey(variableName);
	}

	// Token: 0x06004475 RID: 17525 RVA: 0x0012BDB8 File Offset: 0x00129FB8
	public static void ReportSceneTransition()
	{
		if (StaticVariableList._variables == null)
		{
			return;
		}
		if (StaticVariableList._removeKeys == null)
		{
			StaticVariableList._removeKeys = new List<string>();
		}
		foreach (KeyValuePair<string, StaticVariableList.StaticVariable> keyValuePair in StaticVariableList._variables)
		{
			StaticVariableList.StaticVariable value = keyValuePair.Value;
			if (value.SceneTransitionsLeft > 0)
			{
				value.SceneTransitionsLeft--;
				if (value.SceneTransitionsLeft == 0)
				{
					StaticVariableList._removeKeys.Add(keyValuePair.Key);
				}
			}
		}
		foreach (string key in StaticVariableList._removeKeys)
		{
			StaticVariableList._variables.Remove(key);
		}
		StaticVariableList._removeKeys.Clear();
	}

	// Token: 0x04004581 RID: 17793
	private static Dictionary<string, StaticVariableList.StaticVariable> _variables;

	// Token: 0x04004582 RID: 17794
	private static List<string> _removeKeys;

	// Token: 0x02001A69 RID: 6761
	private class StaticVariable
	{
		// Token: 0x04009962 RID: 39266
		public object Value;

		// Token: 0x04009963 RID: 39267
		public int SceneTransitionsLeft;
	}
}
