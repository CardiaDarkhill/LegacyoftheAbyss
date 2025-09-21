using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003D7 RID: 983
public abstract class CurrencyObject<T> : CurrencyObjectBase where T : CurrencyObject<T>, IBreakOnContact
{
	// Token: 0x0600218D RID: 8589 RVA: 0x0009AECB File Offset: 0x000990CB
	protected override void OnStartOrEnable()
	{
		base.OnStartOrEnable();
		if (base.isActiveAndEnabled)
		{
			this.VerifySpawn();
		}
	}

	// Token: 0x0600218E RID: 8590 RVA: 0x0009AEE1 File Offset: 0x000990E1
	protected override void OnDisable()
	{
		if (this.activeObjectNode != null)
		{
			CurrencyObject<T>._activeObjects.Remove(this.activeObjectNode);
			this.activeObjectNode = null;
		}
		base.OnDisable();
	}

	// Token: 0x0600218F RID: 8591 RVA: 0x0009AF08 File Offset: 0x00099108
	private void VerifySpawn()
	{
		if (this.CurrencyType != null)
		{
			int frameCount = Time.frameCount;
			if (frameCount != CurrencyObject<T>._lastUpdateFrame)
			{
				CurrencyObject<T>._maxObjects = CurrencyObjectLimitRegion.GetLimit(base.transform.position, this.CurrencyType.Value);
				CurrencyObject<T>._lastUpdateFrame = frameCount;
			}
			if (CurrencyObject<T>._activeObjects.Count >= CurrencyObject<T>._maxObjects)
			{
				CurrencyObject<T>._activeObjects.First.Value.gameObject.Recycle();
			}
		}
		this.activeObjectNode = CurrencyObject<T>._activeObjects.AddLast((T)((object)this));
	}

	// Token: 0x0400204B RID: 8267
	private LinkedListNode<T> activeObjectNode;

	// Token: 0x0400204C RID: 8268
	private static readonly LinkedList<T> _activeObjects = new LinkedList<T>();

	// Token: 0x0400204D RID: 8269
	private static int _lastUpdateFrame = -1;

	// Token: 0x0400204E RID: 8270
	private static int _maxObjects;
}
