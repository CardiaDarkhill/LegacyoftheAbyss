using System;
using UnityEngine;

// Token: 0x020004FB RID: 1275
public class HeroPlatformCollisionSplit : MonoBehaviour
{
	// Token: 0x06002DA1 RID: 11681 RVA: 0x000C784C File Offset: 0x000C5A4C
	private void Start()
	{
		if (this.othersPlatform)
		{
			this.heroCol = HeroController.instance.GetComponent<Collider2D>();
			Physics2D.IgnoreCollision(this.heroCol, this.othersPlatform);
		}
		if (this.heroPlatform && this.othersEnterDetector)
		{
			this.othersEnterDetector.OnTriggerEntered += this.OnOthersEnterDetectorEntered;
		}
	}

	// Token: 0x06002DA2 RID: 11682 RVA: 0x000C78B8 File Offset: 0x000C5AB8
	private void OnOthersEnterDetectorEntered(Collider2D collider, GameObject sender)
	{
		if (collider == this.heroCol)
		{
			return;
		}
		Physics2D.IgnoreCollision(collider, this.heroPlatform);
	}

	// Token: 0x04002F79 RID: 12153
	[SerializeField]
	[Tooltip("The platform the Hero should collide with. Others will ignore collision with it.")]
	private Collider2D heroPlatform;

	// Token: 0x04002F7A RID: 12154
	[SerializeField]
	private TriggerEnterEvent othersEnterDetector;

	// Token: 0x04002F7B RID: 12155
	[SerializeField]
	[Tooltip("The platform that Others should collide with. Hero will ignore collision with it.")]
	private Collider2D othersPlatform;

	// Token: 0x04002F7C RID: 12156
	private Collider2D heroCol;
}
