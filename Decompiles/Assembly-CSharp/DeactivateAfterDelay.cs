using System;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class DeactivateAfterDelay : MonoBehaviour
{
	// Token: 0x06001358 RID: 4952 RVA: 0x000586E2 File Offset: 0x000568E2
	private void Awake()
	{
		if (this.stayInPlace)
		{
			this.startPos = base.transform.localPosition;
		}
		this.hasVisibility = (this.visibilityCheck != null);
	}

	// Token: 0x06001359 RID: 4953 RVA: 0x00058710 File Offset: 0x00056910
	private void OnEnable()
	{
		this.timer = this.time;
		if (this.stayInPlace)
		{
			if (this.getPositionOnEnable)
			{
				this.startPos = base.transform.localPosition;
			}
			base.transform.localPosition = this.startPos;
			this.worldPos = base.transform.position;
		}
		if (this.deparent)
		{
			base.transform.parent = null;
		}
		if (this.time == 0f)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600135A RID: 4954 RVA: 0x0005879C File Offset: 0x0005699C
	private void Update()
	{
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			if (this.stayInPlace)
			{
				base.transform.position = this.worldPos;
				return;
			}
		}
		else
		{
			if (this.requireNotVisible && this.hasVisibility && this.visibilityCheck.IsVisible)
			{
				return;
			}
			base.gameObject.SetActive(false);
			if (this.deparentOnDeactivate)
			{
				base.transform.parent = null;
			}
		}
	}

	// Token: 0x0600135B RID: 4955 RVA: 0x00058820 File Offset: 0x00056A20
	public void SetTime(float newTime)
	{
		this.time = newTime;
		this.timer = this.time;
	}

	// Token: 0x040011C1 RID: 4545
	public float time;

	// Token: 0x040011C2 RID: 4546
	public bool stayInPlace;

	// Token: 0x040011C3 RID: 4547
	public bool deparent;

	// Token: 0x040011C4 RID: 4548
	public bool getPositionOnEnable;

	// Token: 0x040011C5 RID: 4549
	public bool deparentOnDeactivate;

	// Token: 0x040011C6 RID: 4550
	[SerializeField]
	private bool requireNotVisible;

	// Token: 0x040011C7 RID: 4551
	[SerializeField]
	private VisibilityEvent visibilityCheck;

	// Token: 0x040011C8 RID: 4552
	private float timer;

	// Token: 0x040011C9 RID: 4553
	private Vector3 worldPos;

	// Token: 0x040011CA RID: 4554
	private Vector3 startPos;

	// Token: 0x040011CB RID: 4555
	private bool hasVisibility;
}
