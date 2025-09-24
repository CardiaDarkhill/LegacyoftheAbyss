using System;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class CollectableItemHeroReaction : MonoBehaviour
{
	// Token: 0x060010AF RID: 4271 RVA: 0x0004F324 File Offset: 0x0004D524
	private void Awake()
	{
		this.spriteFlash = base.GetComponent<SpriteFlash>();
	}

	// Token: 0x060010B0 RID: 4272 RVA: 0x0004F332 File Offset: 0x0004D532
	private void Start()
	{
		CollectableItemHeroReaction._instance = this;
	}

	// Token: 0x060010B1 RID: 4273 RVA: 0x0004F33A File Offset: 0x0004D53A
	private void OnDestroy()
	{
		if (CollectableItemHeroReaction._instance == this)
		{
			CollectableItemHeroReaction._instance = null;
		}
	}

	// Token: 0x170001B3 RID: 435
	// (get) Token: 0x060010B2 RID: 4274 RVA: 0x0004F34F File Offset: 0x0004D54F
	// (set) Token: 0x060010B3 RID: 4275 RVA: 0x0004F356 File Offset: 0x0004D556
	public static Vector2 NextEffectOffset { get; set; }

	// Token: 0x060010B4 RID: 4276 RVA: 0x0004F35E File Offset: 0x0004D55E
	public static void DoReaction()
	{
		if (!CollectableItemHeroReaction._instance)
		{
			return;
		}
		CollectableItemHeroReaction._instance.InternalDoReaction(new Vector2?(CollectableItemHeroReaction.NextEffectOffset), false);
		CollectableItemHeroReaction.NextEffectOffset = Vector2.zero;
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x0004F38C File Offset: 0x0004D58C
	public static void DoReaction(Vector2 effectOffset, bool smallEffect = false)
	{
		if (!CollectableItemHeroReaction._instance)
		{
			return;
		}
		CollectableItemHeroReaction._instance.InternalDoReaction(new Vector2?(effectOffset), smallEffect);
	}

	// Token: 0x060010B6 RID: 4278 RVA: 0x0004F3AC File Offset: 0x0004D5AC
	private void InternalDoReaction(Vector2? effectOffset, bool smallEffect)
	{
		if (this.spriteFlash)
		{
			this.spriteFlash.flashFocusHeal();
		}
		Vector2 vector = base.transform.position;
		if (effectOffset != null)
		{
			vector += effectOffset.Value;
		}
		else
		{
			vector += this.itemGetEffectOffset;
		}
		GameObject gameObject = smallEffect ? this.itemGetEffectPrefabSmall : this.itemGetEffectPrefab;
		if (gameObject)
		{
			gameObject.Spawn(vector.ToVector3(gameObject.transform.position.z)).transform.SetRotation2D(Random.Range(0f, 360f));
		}
		this.itemGetSound.SpawnAndPlayOneShot(vector, null);
	}

	// Token: 0x04000FEC RID: 4076
	private static CollectableItemHeroReaction _instance;

	// Token: 0x04000FED RID: 4077
	[SerializeField]
	private GameObject itemGetEffectPrefab;

	// Token: 0x04000FEE RID: 4078
	[SerializeField]
	private GameObject itemGetEffectPrefabSmall;

	// Token: 0x04000FEF RID: 4079
	[SerializeField]
	private Vector2 itemGetEffectOffset;

	// Token: 0x04000FF0 RID: 4080
	[SerializeField]
	private AudioEvent itemGetSound;

	// Token: 0x04000FF1 RID: 4081
	private SpriteFlash spriteFlash;
}
