using System;
using UnityEngine;

// Token: 0x02000450 RID: 1104
public class InputDebugger : MonoBehaviour
{
	// Token: 0x060026CE RID: 9934 RVA: 0x000AF564 File Offset: 0x000AD764
	private void Update()
	{
		foreach (object obj in Enum.GetValues(typeof(KeyCode)))
		{
			KeyCode key = (KeyCode)obj;
			if (Input.GetKeyDown(key))
			{
				Debug.Log("KeyCode down: " + key.ToString());
			}
		}
	}
}
