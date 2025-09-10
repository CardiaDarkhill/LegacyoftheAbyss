using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007A0 RID: 1952
public class CharmVibrations : MonoBehaviour
{
	// Token: 0x060044DA RID: 17626 RVA: 0x0012D2E8 File Offset: 0x0012B4E8
	public void PlayRegularPlace()
	{
		this.PlayDelayedVibration(this.regularPlace);
	}

	// Token: 0x060044DB RID: 17627 RVA: 0x0012D2F6 File Offset: 0x0012B4F6
	public void PlayFailedPlace()
	{
		this.PlayDelayedVibration(this.failedPlace);
	}

	// Token: 0x060044DC RID: 17628 RVA: 0x0012D304 File Offset: 0x0012B504
	public void PlayOvercharmPlace()
	{
		this.PlayDelayedVibration(this.overcharmPlace);
	}

	// Token: 0x060044DD RID: 17629 RVA: 0x0012D312 File Offset: 0x0012B512
	public void PlayOvercharmHit()
	{
		this.PlayDelayedVibration(this.overcharmHit);
	}

	// Token: 0x060044DE RID: 17630 RVA: 0x0012D320 File Offset: 0x0012B520
	public void PlayOvercharmFinalHit()
	{
		this.PlayDelayedVibration(this.overcharmFinalHit);
	}

	// Token: 0x060044DF RID: 17631 RVA: 0x0012D32E File Offset: 0x0012B52E
	protected void PlayDelayedVibration(VibrationData vibrationData)
	{
		base.StartCoroutine(this.PlayDelayedVibrationRoutine(vibrationData));
	}

	// Token: 0x060044E0 RID: 17632 RVA: 0x0012D33E File Offset: 0x0012B53E
	protected IEnumerator PlayDelayedVibrationRoutine(VibrationData vibrationData)
	{
		yield return null;
		VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget?(new VibrationTarget(VibrationMotors.All)), false, "", false);
		yield break;
	}

	// Token: 0x040045C1 RID: 17857
	[SerializeField]
	private VibrationData regularPlace;

	// Token: 0x040045C2 RID: 17858
	[SerializeField]
	private VibrationData failedPlace;

	// Token: 0x040045C3 RID: 17859
	[SerializeField]
	private VibrationData overcharmPlace;

	// Token: 0x040045C4 RID: 17860
	[SerializeField]
	private VibrationData overcharmHit;

	// Token: 0x040045C5 RID: 17861
	[SerializeField]
	private VibrationData overcharmFinalHit;
}
