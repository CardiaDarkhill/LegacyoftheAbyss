using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200019E RID: 414
[DisallowMultipleComponent]
public sealed class CustomPlayMakerCollisionStay2D : CustomPlayMakerPhysicsEvent<Collision2D>
{
	// Token: 0x06001024 RID: 4132 RVA: 0x0004E010 File Offset: 0x0004C210
	private void Awake()
	{
		CustomPlayMakerCollisionStay2D.lookup[base.gameObject] = this;
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x0004E023 File Offset: 0x0004C223
	private void OnDestroy()
	{
		CustomPlayMakerCollisionStay2D.lookup.Remove(base.gameObject);
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0004E038 File Offset: 0x0004C238
	public static CustomPlayMakerCollisionStay2D GetEventSender(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return null;
		}
		CustomPlayMakerCollisionStay2D result;
		if (!CustomPlayMakerCollisionStay2D.lookup.TryGetValue(gameObject, out result))
		{
			result = gameObject.AddComponentIfNotPresent<CustomPlayMakerCollisionStay2D>();
		}
		return result;
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x0004E067 File Offset: 0x0004C267
	private void OnCollisionStay2D(Collision2D other)
	{
		base.SendEvent(other);
	}

	// Token: 0x04000FB8 RID: 4024
	private static Dictionary<GameObject, CustomPlayMakerCollisionStay2D> lookup = new Dictionary<GameObject, CustomPlayMakerCollisionStay2D>();
}
