using System;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x0200081E RID: 2078
	public interface ITextElement
	{
		// Token: 0x1700089B RID: 2203
		// (get) Token: 0x0600499C RID: 18844
		Material sharedMaterial { get; }

		// Token: 0x0600499D RID: 18845
		void Rebuild(CanvasUpdate update);

		// Token: 0x0600499E RID: 18846
		int GetInstanceID();
	}
}
