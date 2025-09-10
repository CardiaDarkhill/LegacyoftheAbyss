using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A0 RID: 416
public sealed class CustomPlayMakerTriggerStay2D : CustomPlayMakerPhysicsEvent<Collider2D>
{
	// Token: 0x0600102F RID: 4143 RVA: 0x0004E28B File Offset: 0x0004C48B
	private void Awake()
	{
		CustomPlayMakerTriggerStay2D.lookup[base.gameObject] = this;
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x0004E29E File Offset: 0x0004C49E
	private void OnDestroy()
	{
		CustomPlayMakerTriggerStay2D.lookup.Remove(base.gameObject);
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0004E2B4 File Offset: 0x0004C4B4
	public static CustomPlayMakerTriggerStay2D GetEventSender(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return null;
		}
		CustomPlayMakerTriggerStay2D result;
		if (!CustomPlayMakerTriggerStay2D.lookup.TryGetValue(gameObject, out result))
		{
			result = gameObject.AddComponentIfNotPresent<CustomPlayMakerTriggerStay2D>();
		}
		return result;
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x0004E2E3 File Offset: 0x0004C4E3
	private void OnTriggerStay2D(Collider2D other)
	{
		base.SendEvent(other);
	}

	// Token: 0x04000FBE RID: 4030
	private static Dictionary<GameObject, CustomPlayMakerTriggerStay2D> lookup = new Dictionary<GameObject, CustomPlayMakerTriggerStay2D>();
}
