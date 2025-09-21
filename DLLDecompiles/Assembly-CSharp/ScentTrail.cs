using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class ScentTrail : MonoBehaviour
{
	// Token: 0x0600166E RID: 5742 RVA: 0x00064EF4 File Offset: 0x000630F4
	private void Start()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.scentClouds.Add(transform.gameObject);
		}
		float num = 0f;
		for (int i = 0; i < this.scentClouds.Count; i++)
		{
			if (i < this.scentClouds.Count - 1)
			{
				Transform transform2 = this.scentClouds[i].transform;
				Transform transform3 = this.scentClouds[i + 1].transform;
				float y = transform3.position.y - transform2.position.y;
				float x = transform3.position.x - transform2.position.x;
				float num2;
				for (num2 = Mathf.Atan2(y, x) * 57.295776f; num2 < 0f; num2 += 360f)
				{
				}
				transform2.SetRotationZ(num2);
				this.scentClouds[i].GetComponent<ParticleSystem>().main.startDelay = num;
				this.scentClouds[i].transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main.startDelay = num;
				num += 0.1f;
			}
			else
			{
				this.scentClouds[i].SetActive(false);
			}
		}
	}

	// Token: 0x0600166F RID: 5743 RVA: 0x00065094 File Offset: 0x00063294
	public void StartTrail()
	{
		for (int i = 0; i < this.scentClouds.Count; i++)
		{
			this.scentClouds[i].GetComponent<ParticleSystem>().Play();
		}
	}

	// Token: 0x06001670 RID: 5744 RVA: 0x000650D0 File Offset: 0x000632D0
	public void StopTrail()
	{
		for (int i = 0; i < this.scentClouds.Count; i++)
		{
			this.scentClouds[i].GetComponent<ParticleSystem>().Stop();
		}
	}

	// Token: 0x040014E3 RID: 5347
	public List<GameObject> scentClouds;
}
