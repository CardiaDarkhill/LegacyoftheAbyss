using System;
using UnityEngine;

// Token: 0x02000306 RID: 774
public class LineOfSightDetector : MonoBehaviour
{
	// Token: 0x170002DC RID: 732
	// (get) Token: 0x06001B8A RID: 7050 RVA: 0x0008096D File Offset: 0x0007EB6D
	public bool CanSeeHero
	{
		get
		{
			return this.canSeeHero;
		}
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x00080975 File Offset: 0x0007EB75
	protected void Awake()
	{
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x00080978 File Offset: 0x0007EB78
	protected void Update()
	{
		bool flag = false;
		for (int i = 0; i < this.alertRanges.Length; i++)
		{
			AlertRange alertRange = this.alertRanges[i];
			if (!(alertRange == null) && alertRange.IsHeroInRange())
			{
				flag = true;
			}
		}
		if (this.alertRanges.Length != 0 && !flag)
		{
			this.canSeeHero = false;
			return;
		}
		HeroController instance = HeroController.instance;
		if (instance == null)
		{
			this.canSeeHero = false;
			return;
		}
		Vector2 vector = base.transform.position;
		Vector2 vector2 = instance.transform.position;
		Vector2 vector3 = vector2 - vector;
		if (Helper.Raycast2D(vector, vector3.normalized, vector3.magnitude, 256))
		{
			this.canSeeHero = false;
		}
		else
		{
			this.canSeeHero = true;
		}
		Debug.DrawLine(vector, vector2, this.canSeeHero ? Color.green : Color.yellow);
	}

	// Token: 0x04001A8B RID: 6795
	[SerializeField]
	private AlertRange[] alertRanges;

	// Token: 0x04001A8C RID: 6796
	private bool canSeeHero;
}
