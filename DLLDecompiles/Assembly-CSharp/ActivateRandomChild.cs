using System;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class ActivateRandomChild : MonoBehaviour
{
	// Token: 0x060013B2 RID: 5042 RVA: 0x00059C86 File Offset: 0x00057E86
	private void OnEnable()
	{
		this.DoActivateRandomChildren();
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x00059C90 File Offset: 0x00057E90
	public void DoActivateRandomChildren()
	{
		if (this.getChildren)
		{
			this.children = new Transform[base.transform.childCount];
			for (int i = 0; i < base.transform.childCount; i++)
			{
				this.children[i] = base.transform.GetChild(i);
			}
		}
		int num = Random.Range(0, this.children.Length);
		if (this.disableOthers)
		{
			for (int j = 0; j < this.children.Length; j++)
			{
				Transform transform = this.children[j];
				if (transform)
				{
					transform.gameObject.SetActive(num == j);
				}
			}
			return;
		}
		Transform transform2 = this.children[num];
		if (transform2)
		{
			transform2.gameObject.SetActive(true);
		}
	}

	// Token: 0x04001222 RID: 4642
	[SerializeField]
	private Transform[] children;

	// Token: 0x04001223 RID: 4643
	[SerializeField]
	private bool getChildren;

	// Token: 0x04001224 RID: 4644
	[SerializeField]
	private bool disableOthers;
}
