using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003AD RID: 941
public class GodfinderGateIconManager : MonoBehaviour
{
	// Token: 0x06001FA7 RID: 8103 RVA: 0x00090BB6 File Offset: 0x0008EDB6
	private void OnValidate()
	{
		this.DoLayout();
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x00090BC0 File Offset: 0x0008EDC0
	private void OnEnable()
	{
		GodfinderGateIcon[] array = this.gateIcons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Evaluate();
		}
		this.DoLayout();
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x00090BF0 File Offset: 0x0008EDF0
	private void DoLayout()
	{
		Vector3 v = base.transform.position + new Vector3(-this.offsetX / 2f, 0f);
		Vector3 v2 = base.transform.position + new Vector3(this.offsetX / 2f, 0f);
		List<GodfinderGateIcon> list = new List<GodfinderGateIcon>();
		foreach (GodfinderGateIcon godfinderGateIcon in this.gateIcons)
		{
			if (godfinderGateIcon.gameObject.activeSelf)
			{
				list.Add(godfinderGateIcon);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j])
			{
				list[j].transform.position = Vector2.Lerp(v, v2, (float)j / (float)(list.Count - 1));
			}
		}
	}

	// Token: 0x04001EB6 RID: 7862
	public GodfinderGateIcon[] gateIcons;

	// Token: 0x04001EB7 RID: 7863
	public float offsetX = 8f;
}
