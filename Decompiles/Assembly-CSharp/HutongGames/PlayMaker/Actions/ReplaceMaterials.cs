using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFC RID: 3324
	public class ReplaceMaterials : FsmStateAction
	{
		// Token: 0x06006285 RID: 25221 RVA: 0x001F2A15 File Offset: 0x001F0C15
		public override void Reset()
		{
			this.Parent = null;
			this.From = null;
			this.To = null;
		}

		// Token: 0x06006286 RID: 25222 RVA: 0x001F2A2C File Offset: 0x001F0C2C
		public override void Awake()
		{
			GameObject safe = this.Parent.GetSafe(this);
			if (!safe)
			{
				return;
			}
			this.renderers = safe.GetComponentsInChildren<Renderer>(true);
		}

		// Token: 0x06006287 RID: 25223 RVA: 0x001F2A5C File Offset: 0x001F0C5C
		public override void OnEnter()
		{
			foreach (Renderer renderer in this.renderers)
			{
				for (int j = 0; j < this.From.Length; j++)
				{
					Material value = this.From[j].Value;
					Material value2 = this.To[j].Value;
					if (!(renderer.sharedMaterial != value))
					{
						renderer.sharedMaterial = value2;
						break;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040060F1 RID: 24817
		public FsmOwnerDefault Parent;

		// Token: 0x040060F2 RID: 24818
		[CompoundArray("Pairs", "From", "To")]
		public FsmMaterial[] From;

		// Token: 0x040060F3 RID: 24819
		public FsmMaterial[] To;

		// Token: 0x040060F4 RID: 24820
		private Renderer[] renderers;
	}
}
