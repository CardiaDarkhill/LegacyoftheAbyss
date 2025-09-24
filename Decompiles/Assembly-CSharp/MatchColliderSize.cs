using System;
using UnityEngine;

// Token: 0x0200024D RID: 589
public class MatchColliderSize : MonoBehaviour
{
	// Token: 0x0600156C RID: 5484 RVA: 0x00061065 File Offset: 0x0005F265
	private void Start()
	{
		this.hasStarted = true;
		this.DoMatch();
	}

	// Token: 0x0600156D RID: 5485 RVA: 0x00061074 File Offset: 0x0005F274
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.DoMatch();
	}

	// Token: 0x0600156E RID: 5486 RVA: 0x00061088 File Offset: 0x0005F288
	private void DoMatch()
	{
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		if (!component)
		{
			return;
		}
		component.offset = this.target.offset;
		component.size = this.target.size;
	}

	// Token: 0x0400140F RID: 5135
	public BoxCollider2D target;

	// Token: 0x04001410 RID: 5136
	private bool hasStarted;
}
