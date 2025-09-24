using System;
using UnityEngine;

// Token: 0x0200025C RID: 604
public class PassColour : MonoBehaviour
{
	// Token: 0x060015C2 RID: 5570 RVA: 0x0006241C File Offset: 0x0006061C
	public void SetColour(Color color)
	{
		foreach (PassColour.Other other in this.passTo)
		{
			Color color2 = color.MultiplyElements(other.MultiplyColor);
			if (other.Sprite)
			{
				other.Sprite.color = color2;
			}
			if (other.tk2dSprite)
			{
				other.tk2dSprite.color = color2;
			}
			if (other.ParticleSystem)
			{
				other.ParticleSystem.main.startColor = color2;
			}
		}
	}

	// Token: 0x04001460 RID: 5216
	[SerializeField]
	private PassColour.Other[] passTo;

	// Token: 0x02001551 RID: 5457
	[Serializable]
	private class Other
	{
		// Token: 0x040086B5 RID: 34485
		public SpriteRenderer Sprite;

		// Token: 0x040086B6 RID: 34486
		public tk2dSprite tk2dSprite;

		// Token: 0x040086B7 RID: 34487
		public ParticleSystem ParticleSystem;

		// Token: 0x040086B8 RID: 34488
		public Color MultiplyColor;
	}
}
