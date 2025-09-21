using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005F7 RID: 1527
public class TrackTriggerEvent : MonoBehaviour
{
	// Token: 0x0600367F RID: 13951 RVA: 0x000F0801 File Offset: 0x000EEA01
	private void Awake()
	{
		if (this.triggerObjects)
		{
			this.triggerObjects.InsideStateChanged += this.OnInsideStateChanged;
		}
	}

	// Token: 0x06003680 RID: 13952 RVA: 0x000F0828 File Offset: 0x000EEA28
	private void Start()
	{
		bool flag = true;
		if (this.triggerObjects && this.triggerObjects.IsInside)
		{
			this.SetInside(true);
			flag = false;
		}
		if (flag)
		{
			this.OnOutside.Invoke();
		}
	}

	// Token: 0x06003681 RID: 13953 RVA: 0x000F0868 File Offset: 0x000EEA68
	private void OnInsideStateChanged(bool insideState)
	{
		this.SetInside(insideState);
	}

	// Token: 0x06003682 RID: 13954 RVA: 0x000F0871 File Offset: 0x000EEA71
	private void SetInside(bool isInside)
	{
		if (this.isInside == isInside)
		{
			return;
		}
		this.isInside = isInside;
		if (isInside)
		{
			this.OnInside.Invoke();
			return;
		}
		this.OnOutside.Invoke();
	}

	// Token: 0x04003960 RID: 14688
	[SerializeField]
	private TrackTriggerObjects triggerObjects;

	// Token: 0x04003961 RID: 14689
	public UnityEvent OnInside;

	// Token: 0x04003962 RID: 14690
	public UnityEvent OnOutside;

	// Token: 0x04003963 RID: 14691
	private bool isInside;
}
