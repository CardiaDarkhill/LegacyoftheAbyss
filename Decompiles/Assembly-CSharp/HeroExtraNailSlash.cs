using System;
using UnityEngine;

// Token: 0x020003B7 RID: 951
public class HeroExtraNailSlash : MonoBehaviour
{
	// Token: 0x06001FF2 RID: 8178 RVA: 0x000915A9 File Offset: 0x0008F7A9
	private void Awake()
	{
		this.damagers = base.GetComponentsInChildren<DamageEnemies>(true);
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000915B8 File Offset: 0x0008F7B8
	private void OnEnable()
	{
		HeroController instance = HeroController.instance;
		NailImbuementConfig currentImbuement = instance.NailImbuement.CurrentImbuement;
		if (currentImbuement == null)
		{
			return;
		}
		NailElements currentElement = instance.NailImbuement.CurrentElement;
		foreach (SpriteRenderer spriteRenderer in this.tintSprites)
		{
			if (spriteRenderer)
			{
				spriteRenderer.color = currentImbuement.NailTintColor;
			}
		}
		foreach (tk2dSprite tk2dSprite in this.tintTk2dSprites)
		{
			if (tk2dSprite)
			{
				tk2dSprite.color = currentImbuement.NailTintColor;
			}
		}
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			if (damageEnemies)
			{
				damageEnemies.NailElement = currentElement;
				damageEnemies.NailImbuement = currentImbuement;
			}
		}
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x00091698 File Offset: 0x0008F898
	private void OnDisable()
	{
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			if (damageEnemies)
			{
				damageEnemies.NailElement = NailElements.None;
				damageEnemies.NailImbuement = null;
			}
		}
		foreach (SpriteRenderer spriteRenderer in this.tintSprites)
		{
			if (spriteRenderer)
			{
				spriteRenderer.color = Color.white;
			}
		}
		foreach (tk2dSprite tk2dSprite in this.tintTk2dSprites)
		{
			if (tk2dSprite)
			{
				tk2dSprite.color = Color.white;
			}
		}
	}

	// Token: 0x04001EF7 RID: 7927
	[SerializeField]
	private SpriteRenderer[] tintSprites;

	// Token: 0x04001EF8 RID: 7928
	[SerializeField]
	private tk2dSprite[] tintTk2dSprites;

	// Token: 0x04001EF9 RID: 7929
	private DamageEnemies[] damagers;
}
