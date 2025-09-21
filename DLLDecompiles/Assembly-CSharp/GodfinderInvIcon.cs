using System;
using UnityEngine;

// Token: 0x020003B1 RID: 945
public class GodfinderInvIcon : MonoBehaviour
{
	// Token: 0x06001FB9 RID: 8121 RVA: 0x00090F3C File Offset: 0x0008F13C
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x00090F4C File Offset: 0x0008F14C
	private void OnEnable()
	{
		if (this.spriteRenderer)
		{
			if (!GameManager.instance.playerData.bossRushMode)
			{
				this.spriteRenderer.sprite = (GameManager.instance.playerData.unlockedNewBossStatue ? this.newBossSprite : this.normalSprite);
				BossScene[] array = this.bosses;
				for (int i = 0; i < array.Length; i++)
				{
					if (!array[i].IsUnlocked(BossSceneCheckSource.Godfinder))
					{
						return;
					}
				}
				GodfinderInvIcon.BossSceneExtra[] array2 = this.extraBosses;
				for (int i = 0; i < array2.Length; i++)
				{
					if (!array2[i].IsUnlocked())
					{
						return;
					}
				}
			}
			this.spriteRenderer.sprite = this.allBossesSprite;
		}
	}

	// Token: 0x04001EC1 RID: 7873
	public Sprite normalSprite;

	// Token: 0x04001EC2 RID: 7874
	public Sprite newBossSprite;

	// Token: 0x04001EC3 RID: 7875
	public Sprite allBossesSprite;

	// Token: 0x04001EC4 RID: 7876
	[Tooltip("Once all listed bosses are unlocked, godfinder is in complete state.")]
	public BossScene[] bosses;

	// Token: 0x04001EC5 RID: 7877
	[Tooltip("Boss scenes with conditions as to whether they are counted or not.")]
	public GodfinderInvIcon.BossSceneExtra[] extraBosses;

	// Token: 0x04001EC6 RID: 7878
	private SpriteRenderer spriteRenderer;

	// Token: 0x02001668 RID: 5736
	[Serializable]
	public class BossSceneExtra
	{
		// Token: 0x06008A13 RID: 35347 RVA: 0x0027ED20 File Offset: 0x0027CF20
		public bool IsUnlocked()
		{
			if (this.extraTests != null)
			{
				BossScene.BossTest[] array = this.extraTests;
				for (int i = 0; i < array.Length; i++)
				{
					if (!array[i].IsUnlocked())
					{
						return true;
					}
				}
			}
			return this.bossScene && this.bossScene.IsUnlocked(BossSceneCheckSource.Godfinder);
		}

		// Token: 0x04008AB0 RID: 35504
		public BossScene bossScene;

		// Token: 0x04008AB1 RID: 35505
		[Tooltip("If any of these tests fail the boss scene will be skipped.")]
		public BossScene.BossTest[] extraTests;
	}
}
