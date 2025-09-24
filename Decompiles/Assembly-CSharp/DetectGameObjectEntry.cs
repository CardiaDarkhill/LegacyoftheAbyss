using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000346 RID: 838
public class DetectGameObjectEntry : MonoBehaviour
{
	// Token: 0x06001D28 RID: 7464 RVA: 0x00087128 File Offset: 0x00085328
	public void FindTarget(string targetName)
	{
		this.target = GameObject.Find(targetName);
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x00087136 File Offset: 0x00085336
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.target == null)
		{
			return;
		}
		if (this.IsTarget(collision.gameObject))
		{
			this.OnTargetEntered.Invoke();
		}
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x00087160 File Offset: 0x00085360
	private bool IsTarget(GameObject obj)
	{
		if (obj == this.target)
		{
			return true;
		}
		Transform parent = obj.transform.parent;
		return parent && this.IsTarget(parent.gameObject);
	}

	// Token: 0x04001C73 RID: 7283
	[SerializeField]
	private GameObject target;

	// Token: 0x04001C74 RID: 7284
	[Space]
	public UnityEvent OnTargetEntered;
}
