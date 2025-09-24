using System;

// Token: 0x020005E4 RID: 1508
public abstract class ToolBase : QuestTargetCounter
{
	// Token: 0x170005F1 RID: 1521
	// (get) Token: 0x06003585 RID: 13701 RVA: 0x000ECE3D File Offset: 0x000EB03D
	public override bool CanGetMultipleAtOnce
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170005F2 RID: 1522
	// (get) Token: 0x06003586 RID: 13702 RVA: 0x000ECE40 File Offset: 0x000EB040
	public override bool IsUnique
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170005F3 RID: 1523
	// (get) Token: 0x06003587 RID: 13703
	public abstract bool IsEquipped { get; }
}
