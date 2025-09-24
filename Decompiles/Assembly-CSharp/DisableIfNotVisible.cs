using System;
using UnityEngine;

// Token: 0x02000348 RID: 840
public sealed class DisableIfNotVisible : MonoBehaviour
{
	// Token: 0x06001D32 RID: 7474 RVA: 0x000872C4 File Offset: 0x000854C4
	private void Awake()
	{
		if (this.visibilityEvent == null)
		{
			this.visibilityEvent = base.GetComponent<VisibilityEvent>();
			if (this.visibilityEvent == null)
			{
				this.visibilityEvent = base.gameObject.AddComponent<VisibilityGroup>();
			}
		}
		this.visibilityEvent.OnVisibilityChanged += this.OnVisibilityChanged;
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x00087321 File Offset: 0x00085521
	private void Start()
	{
		this.UpdateVisibility(this.visibilityEvent.IsVisible);
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x00087334 File Offset: 0x00085534
	private void OnValidate()
	{
		if (this.visibilityEvent == null)
		{
			this.visibilityEvent = base.GetComponent<VisibilityEvent>();
		}
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x00087350 File Offset: 0x00085550
	private void LateUpdate()
	{
		this.timer += Time.deltaTime;
		if (this.timer >= this.disableDelay)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x0008737E File Offset: 0x0008557E
	private void OnVisibilityChanged(bool visible)
	{
		this.UpdateVisibility(visible);
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x00087387 File Offset: 0x00085587
	private void UpdateVisibility(bool visible)
	{
		base.enabled = !visible;
		this.timer = 0f;
	}

	// Token: 0x04001C79 RID: 7289
	[SerializeField]
	private VisibilityEvent visibilityEvent;

	// Token: 0x04001C7A RID: 7290
	[SerializeField]
	private float disableDelay = 2f;

	// Token: 0x04001C7B RID: 7291
	private float timer;
}
