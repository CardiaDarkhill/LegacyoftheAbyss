using System;
using UnityEngine;

// Token: 0x020001D7 RID: 471
public interface ITagDamageTakerOwner
{
	// Token: 0x06001274 RID: 4724
	bool ApplyTagDamage(DamageTag.DamageTagInstance damageTagInstance);

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001275 RID: 4725
	SpriteFlash SpriteFlash { get; }

	// Token: 0x17000205 RID: 517
	// (get) Token: 0x06001276 RID: 4726
	Vector2 TagDamageEffectPos { get; }

	// Token: 0x17000206 RID: 518
	// (get) Token: 0x06001277 RID: 4727
	Transform transform { get; }
}
