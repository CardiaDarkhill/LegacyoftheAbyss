using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000555 RID: 1365
public class SmashableTile : MonoBehaviour
{
	// Token: 0x060030DC RID: 12508 RVA: 0x000D84E4 File Offset: 0x000D66E4
	private void Awake()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isSmashed;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isSmashed = value;
				if (this.isSmashed)
				{
					this.SetSmashed();
				}
			};
		}
		if (this.sprite && this.smashedSprites.Length == 0)
		{
			this.sprite.enabled = false;
		}
		if (this.activateOnSmash)
		{
			this.activateOnSmash.SetActive(false);
		}
	}

	// Token: 0x060030DD RID: 12509 RVA: 0x000D8567 File Offset: 0x000D6767
	private void Start()
	{
		if (this.startSmashedCondition.IsDefined && this.startSmashedCondition.IsFulfilled)
		{
			this.SetSmashed();
		}
	}

	// Token: 0x060030DE RID: 12510 RVA: 0x000D858C File Offset: 0x000D678C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.isSmashed)
		{
			return;
		}
		if (!collision.CompareTag("Tile Smasher"))
		{
			return;
		}
		if (Random.Range(0f, 1f) > this.smashChance)
		{
			return;
		}
		this.SetSmashed();
		if (this.activateOnSmash)
		{
			this.activateOnSmash.SetActive(true);
		}
	}

	// Token: 0x060030DF RID: 12511 RVA: 0x000D85E8 File Offset: 0x000D67E8
	private void SetSmashed()
	{
		this.isSmashed = true;
		if (!this.sprite)
		{
			return;
		}
		this.sprite.enabled = true;
		if (this.smashedSprites.Length == 0)
		{
			return;
		}
		this.sprite.sprite = this.smashedSprites[Random.Range(0, this.smashedSprites.Length)];
		float z = base.transform.position.z;
		if (z < this.preventRotationZRange.Start || z > this.preventRotationZRange.End)
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			localEulerAngles.z += this.breakRotation.GetRandomValue();
			base.transform.localEulerAngles = localEulerAngles;
		}
	}

	// Token: 0x04003417 RID: 13335
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003418 RID: 13336
	[SerializeField]
	private PlayerDataTest startSmashedCondition;

	// Token: 0x04003419 RID: 13337
	[Space]
	[SerializeField]
	[Range(0f, 1f)]
	private float smashChance = 1f;

	// Token: 0x0400341A RID: 13338
	[SerializeField]
	private SpriteRenderer sprite;

	// Token: 0x0400341B RID: 13339
	[SerializeField]
	private Sprite[] smashedSprites;

	// Token: 0x0400341C RID: 13340
	[SerializeField]
	private GameObject activateOnSmash;

	// Token: 0x0400341D RID: 13341
	[Space]
	[SerializeField]
	private MinMaxFloat breakRotation;

	// Token: 0x0400341E RID: 13342
	[SerializeField]
	private MinMaxFloat preventRotationZRange;

	// Token: 0x0400341F RID: 13343
	private bool isSmashed;
}
