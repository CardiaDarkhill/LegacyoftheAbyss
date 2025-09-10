using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000359 RID: 857
public sealed class NonTinter : MonoBehaviour
{
	// Token: 0x06001D9C RID: 7580 RVA: 0x00088A50 File Offset: 0x00086C50
	private void Awake()
	{
		NonTinter.lookup[base.gameObject] = this.blockedStates;
		if (this.includeChildren)
		{
			foreach (tk2dSprite tk2dSprite in base.GetComponentsInChildren<tk2dSprite>(true))
			{
				NonTinter.lookup[tk2dSprite.gameObject] = this.blockedStates;
			}
		}
	}

	// Token: 0x06001D9D RID: 7581 RVA: 0x00088AAB File Offset: 0x00086CAB
	public bool ShouldTint(NonTinter.TintFlag blockType)
	{
		return this.blockedStates.ShouldTint(blockType);
	}

	// Token: 0x06001D9E RID: 7582 RVA: 0x00088AB9 File Offset: 0x00086CB9
	public static void ClearNonTinters()
	{
		NonTinter.lookup.Clear();
	}

	// Token: 0x06001D9F RID: 7583 RVA: 0x00088AC8 File Offset: 0x00086CC8
	public static bool CanTint(GameObject gameObject, NonTinter.TintFlag source)
	{
		NonTinter.BlockedStates blockedStates;
		if (!NonTinter.lookup.TryGetValue(gameObject, out blockedStates))
		{
			NonTinter componentInParent = gameObject.GetComponentInParent<NonTinter>(true);
			if (componentInParent != null && (componentInParent.includeChildren || componentInParent.gameObject == gameObject))
			{
				blockedStates = componentInParent.blockedStates;
			}
			else
			{
				blockedStates = new NonTinter.BlockedStates(NonTinter.TintFlag.None);
			}
			NonTinter.lookup[gameObject] = blockedStates;
		}
		return blockedStates.ShouldTint(source);
	}

	// Token: 0x04001CD3 RID: 7379
	[SerializeField]
	private NonTinter.BlockedStates blockedStates = new NonTinter.BlockedStates(NonTinter.TintFlag.All);

	// Token: 0x04001CD4 RID: 7380
	[SerializeField]
	private bool includeChildren;

	// Token: 0x04001CD5 RID: 7381
	private static Dictionary<GameObject, NonTinter.BlockedStates> lookup = new Dictionary<GameObject, NonTinter.BlockedStates>();

	// Token: 0x0200160F RID: 5647
	[Serializable]
	private class BlockedStates
	{
		// Token: 0x060088CE RID: 35022 RVA: 0x0027B733 File Offset: 0x00279933
		public bool ShouldTint(NonTinter.TintFlag blockType)
		{
			return this.blockedTypes == NonTinter.TintFlag.None || (blockType & this.blockedTypes) == NonTinter.TintFlag.None;
		}

		// Token: 0x060088CF RID: 35023 RVA: 0x0027B74A File Offset: 0x0027994A
		public BlockedStates() : this(NonTinter.TintFlag.None)
		{
		}

		// Token: 0x060088D0 RID: 35024 RVA: 0x0027B753 File Offset: 0x00279953
		public BlockedStates(NonTinter.TintFlag blockedTypes)
		{
			this.blockedTypes = blockedTypes;
		}

		// Token: 0x04008995 RID: 35221
		public NonTinter.TintFlag blockedTypes = NonTinter.TintFlag.All;
	}

	// Token: 0x02001610 RID: 5648
	[Flags]
	public enum TintFlag
	{
		// Token: 0x04008997 RID: 35223
		None = 0,
		// Token: 0x04008998 RID: 35224
		CorpseLand = 1,
		// Token: 0x04008999 RID: 35225
		All = -1
	}
}
