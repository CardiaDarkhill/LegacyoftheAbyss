using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000211 RID: 529
public class ActivateChildrenOnContact : MonoBehaviour
{
	// Token: 0x060013AE RID: 5038 RVA: 0x00059B0C File Offset: 0x00057D0C
	private void Awake()
	{
		if (this.initChildren)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				bool activeSelf = transform.gameObject.activeSelf;
				this.children[transform] = activeSelf;
				if (!activeSelf)
				{
					transform.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x060013AF RID: 5039 RVA: 0x00059B90 File Offset: 0x00057D90
	private IEnumerator Start()
	{
		if (this.initChildren)
		{
			yield return null;
			if (!this.activated)
			{
				foreach (object obj in base.transform)
				{
					Transform transform = (Transform)obj;
					bool flag;
					if (this.children.TryGetValue(transform, out flag) && !flag)
					{
						transform.gameObject.SetActive(false);
					}
				}
			}
			this.children.Clear();
		}
		yield break;
	}

	// Token: 0x060013B0 RID: 5040 RVA: 0x00059BA0 File Offset: 0x00057DA0
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.onlyReactToPlayer && !collision.gameObject.CompareTag("Player"))
		{
			return;
		}
		this.activated = true;
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(this.setActive);
		}
		if (this.circleCollider != null)
		{
			this.circleCollider.enabled = false;
		}
		if (this.boxCollider != null)
		{
			this.boxCollider.enabled = false;
		}
		if (this.onContact != null)
		{
			this.onContact.Invoke();
		}
	}

	// Token: 0x0400121A RID: 4634
	public CircleCollider2D circleCollider;

	// Token: 0x0400121B RID: 4635
	public BoxCollider2D boxCollider;

	// Token: 0x0400121C RID: 4636
	public bool setActive = true;

	// Token: 0x0400121D RID: 4637
	[SerializeField]
	private bool initChildren;

	// Token: 0x0400121E RID: 4638
	[SerializeField]
	private bool onlyReactToPlayer;

	// Token: 0x0400121F RID: 4639
	[SerializeField]
	private UnityEvent onContact;

	// Token: 0x04001220 RID: 4640
	private Dictionary<Transform, bool> children = new Dictionary<Transform, bool>();

	// Token: 0x04001221 RID: 4641
	private bool activated;
}
