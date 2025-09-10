using System;
using UnityEngine;

// Token: 0x02000239 RID: 569
[RequireComponent(typeof(PolygonCollider2D))]
public class GradeTrigger : MonoBehaviour
{
	// Token: 0x060014D7 RID: 5335 RVA: 0x0005E160 File Offset: 0x0005C360
	private void Start()
	{
		if (this.gradeMarker)
		{
			this.gradeMarker.SetStartSizeForTrigger();
			this.gradeMarker.EaseDuration = this.easeTime;
			this.gradeMarker.Deactivate();
			return;
		}
		Debug.LogError("No grade marker set for this grade trigger: " + base.name);
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x0005E1B7 File Offset: 0x0005C3B7
	private void OnTriggerEnter2D(Collider2D triggerObject)
	{
		if (triggerObject.tag == "Player")
		{
			if (this.instantActivate)
			{
				this.gradeMarker.Activate();
				return;
			}
			this.gradeMarker.ActivateGradual();
		}
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x0005E1EA File Offset: 0x0005C3EA
	private void OnTriggerExit2D(Collider2D triggerObject)
	{
		if (triggerObject.tag == "Player")
		{
			if (this.instantActivate)
			{
				this.gradeMarker.Deactivate();
				return;
			}
			this.gradeMarker.DeactivateGradual();
		}
	}

	// Token: 0x04001356 RID: 4950
	public GradeMarker gradeMarker;

	// Token: 0x04001357 RID: 4951
	public bool instantActivate;

	// Token: 0x04001358 RID: 4952
	[Range(0.5f, 2f)]
	public float easeTime = 0.8f;
}
