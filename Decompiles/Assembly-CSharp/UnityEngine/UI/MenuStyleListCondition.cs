using System;

namespace UnityEngine.UI
{
	// Token: 0x0200087A RID: 2170
	public class MenuStyleListCondition : MenuButtonListCondition
	{
		// Token: 0x06004B8C RID: 19340 RVA: 0x00164EC4 File Offset: 0x001630C4
		public override bool IsFulfilled()
		{
			MenuStyles instance = MenuStyles.Instance;
			if (!instance)
			{
				return false;
			}
			int num = 0;
			MenuStyles.MenuStyle[] styles = instance.Styles;
			for (int i = 0; i < styles.Length; i++)
			{
				if (styles[i].IsAvailable)
				{
					num++;
				}
				if (num > 1)
				{
					return true;
				}
			}
			return false;
		}
	}
}
