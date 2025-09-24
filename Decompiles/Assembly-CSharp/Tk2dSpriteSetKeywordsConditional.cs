using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200036E RID: 878
public sealed class Tk2dSpriteSetKeywordsConditional : MonoBehaviour
{
	// Token: 0x06001E24 RID: 7716 RVA: 0x0008B2C5 File Offset: 0x000894C5
	private void Reset()
	{
		this.sprite = base.GetComponent<tk2dSprite>();
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x0008B2D4 File Offset: 0x000894D4
	private void Awake()
	{
		using (List<PlayerDataTest>.Enumerator enumerator = this.tests.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.IsFulfilled)
				{
					return;
				}
			}
		}
		bool flag = this.sprite != null;
		if (!flag)
		{
			this.sprite = base.GetComponent<tk2dSprite>();
			flag = (this.sprite != null);
		}
		if (flag)
		{
			this.sprite.ForceBuild();
			foreach (string keyword in this.keywords)
			{
				this.sprite.EnableKeyword(keyword);
			}
		}
		if (this.includeChildren)
		{
			foreach (tk2dSprite tk2dSprite in base.GetComponentsInChildren<tk2dSprite>(true))
			{
				if (!flag || !(tk2dSprite == this.sprite))
				{
					foreach (string keyword2 in this.keywords)
					{
						tk2dSprite.ForceBuild();
						tk2dSprite.EnableKeyword(keyword2);
					}
				}
			}
		}
		base.enabled = false;
	}

	// Token: 0x04001D3D RID: 7485
	[SerializeField]
	private List<PlayerDataTest> tests = new List<PlayerDataTest>();

	// Token: 0x04001D3E RID: 7486
	[SerializeField]
	private tk2dSprite sprite;

	// Token: 0x04001D3F RID: 7487
	[SerializeField]
	private bool includeChildren;

	// Token: 0x04001D40 RID: 7488
	[SerializeField]
	private string[] keywords;
}
