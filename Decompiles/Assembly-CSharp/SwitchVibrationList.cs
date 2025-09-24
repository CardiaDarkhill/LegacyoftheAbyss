using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000451 RID: 1105
[CreateAssetMenu(menuName = "Debug/Switch Vibration List")]
public sealed class SwitchVibrationList : ScriptableObject
{
	// Token: 0x04002413 RID: 9235
	private const string VIBRATION_DIRECTORY = "Assets/Audio/Vibration Files";

	// Token: 0x04002414 RID: 9236
	public List<TextAsset> vibrations = new List<TextAsset>();
}
