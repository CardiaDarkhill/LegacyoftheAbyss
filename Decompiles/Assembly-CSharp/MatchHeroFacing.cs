using System;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class MatchHeroFacing : MonoBehaviour
{
	// Token: 0x06001570 RID: 5488 RVA: 0x000610CF File Offset: 0x0005F2CF
	private void Start()
	{
		this.hasStarted = true;
		this.DoMatch();
	}

	// Token: 0x06001571 RID: 5489 RVA: 0x000610DE File Offset: 0x0005F2DE
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.DoMatch();
	}

	// Token: 0x06001572 RID: 5490 RVA: 0x000610F0 File Offset: 0x0005F2F0
	private void DoMatch()
	{
		Transform transform = GameManager.instance.hero_ctrl.transform;
		if (!this.reverse)
		{
			if ((transform.localScale.x < 0f && base.transform.localScale.x > 0f) || (transform.localScale.x < 0f && base.transform.localScale.x < 0f))
			{
				base.transform.localScale = new Vector2(-base.transform.localScale.x, base.transform.localScale.y);
				return;
			}
		}
		else if ((transform.localScale.x < 0f && base.transform.localScale.x < 0f) || (transform.localScale.x > 0f && base.transform.localScale.x > 0f))
		{
			base.transform.localScale = new Vector2(-base.transform.localScale.x, base.transform.localScale.y);
		}
	}

	// Token: 0x04001411 RID: 5137
	public bool reverse;

	// Token: 0x04001412 RID: 5138
	private bool hasStarted;
}
