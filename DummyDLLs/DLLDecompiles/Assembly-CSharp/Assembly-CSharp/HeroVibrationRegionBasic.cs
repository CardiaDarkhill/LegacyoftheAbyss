using System;
using UnityEngine;

// Token: 0x020007A4 RID: 1956
public sealed class HeroVibrationRegionBasic : MonoBehaviour
{
	// Token: 0x06004525 RID: 17701 RVA: 0x0012E4D0 File Offset: 0x0012C6D0
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			this.Enter();
		}
	}

	// Token: 0x06004526 RID: 17702 RVA: 0x0012E4E5 File Offset: 0x0012C6E5
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			this.Exit();
		}
	}

	// Token: 0x06004527 RID: 17703 RVA: 0x0012E4FC File Offset: 0x0012C6FC
	private void Enter()
	{
		if (this.isInside)
		{
			return;
		}
		this.isInside = true;
		VibrationData vibrationData = this.vibrationDataAsset;
		bool isLooping = this.loop;
		bool isRealtime = this.isRealTime;
		string tag = this.vibrationTag;
		this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, tag, isRealtime);
		VibrationEmission vibrationEmission = this.emission;
		if (vibrationEmission != null)
		{
			vibrationEmission.SetStrength(this.strength);
		}
		VibrationEmission vibrationEmission2 = this.emission;
		if (vibrationEmission2 == null)
		{
			return;
		}
		vibrationEmission2.SetSpeed(this.speed);
	}

	// Token: 0x06004528 RID: 17704 RVA: 0x0012E57D File Offset: 0x0012C77D
	private void Exit()
	{
		if (!this.isInside)
		{
			return;
		}
		this.isInside = false;
		VibrationEmission vibrationEmission = this.emission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.emission = null;
	}

	// Token: 0x040045FF RID: 17919
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x04004600 RID: 17920
	[SerializeField]
	private float strength = 1f;

	// Token: 0x04004601 RID: 17921
	[SerializeField]
	private float speed = 1f;

	// Token: 0x04004602 RID: 17922
	[SerializeField]
	private bool loop = true;

	// Token: 0x04004603 RID: 17923
	[SerializeField]
	private bool isRealTime;

	// Token: 0x04004604 RID: 17924
	[SerializeField]
	private string vibrationTag;

	// Token: 0x04004605 RID: 17925
	private bool isInside;

	// Token: 0x04004606 RID: 17926
	private VibrationEmission emission;
}
