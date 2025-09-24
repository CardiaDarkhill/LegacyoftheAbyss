using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003F8 RID: 1016
public sealed class JitterSelfVibration : MonoBehaviour
{
	// Token: 0x060022AA RID: 8874 RVA: 0x0009F5F0 File Offset: 0x0009D7F0
	private void Awake()
	{
		if (this.jitterSelf)
		{
			this.jitterSelf.OnJitterStart.AddListener(new UnityAction(this.StartEmission));
			this.jitterSelf.OnJitterEnd.AddListener(new UnityAction(this.StopEmission));
		}
	}

	// Token: 0x060022AB RID: 8875 RVA: 0x0009F642 File Offset: 0x0009D842
	private void OnValidate()
	{
		if (!this.jitterSelf)
		{
			this.jitterSelf = base.GetComponent<JitterSelf>();
		}
	}

	// Token: 0x060022AC RID: 8876 RVA: 0x0009F65D File Offset: 0x0009D85D
	private void OnDisable()
	{
		this.StopEmission();
	}

	// Token: 0x060022AD RID: 8877 RVA: 0x0009F668 File Offset: 0x0009D868
	public void StartEmission()
	{
		this.StopEmission();
		VibrationData vibrationData = this.vibrationDataAsset;
		bool flag = this.isLooping;
		bool isRealtime = this.isRealTime;
		this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, flag, "", isRealtime);
	}

	// Token: 0x060022AE RID: 8878 RVA: 0x0009F6AF File Offset: 0x0009D8AF
	public void StopEmission()
	{
		VibrationEmission vibrationEmission = this.emission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.emission = null;
	}

	// Token: 0x04002180 RID: 8576
	[SerializeField]
	private JitterSelf jitterSelf;

	// Token: 0x04002181 RID: 8577
	[Space]
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x04002182 RID: 8578
	[SerializeField]
	private bool isLooping;

	// Token: 0x04002183 RID: 8579
	[SerializeField]
	private bool isRealTime;

	// Token: 0x04002184 RID: 8580
	private VibrationEmission emission;
}
