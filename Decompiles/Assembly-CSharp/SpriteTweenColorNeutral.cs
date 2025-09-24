using System;
using System.Collections;
using UnityEngine;

// Token: 0x020005CB RID: 1483
public class SpriteTweenColorNeutral : MonoBehaviour
{
	// Token: 0x060034D8 RID: 13528 RVA: 0x000EA994 File Offset: 0x000E8B94
	private void ColorReturnNeutral()
	{
		tk2dSprite component = base.GetComponent<tk2dSprite>();
		Hashtable hashtable = new Hashtable();
		hashtable.Add("from", component.color);
		hashtable.Add("to", this.Color);
		hashtable.Add("time", this.Duration);
		hashtable.Add("OnUpdate", "updateSpriteColor");
		hashtable.Add("looptype", iTween.LoopType.none);
		hashtable.Add("easetype", iTween.EaseType.linear);
		iTween.ValueTo(base.gameObject, hashtable);
	}

	// Token: 0x060034D9 RID: 13529 RVA: 0x000EAA2F File Offset: 0x000E8C2F
	private void updateSpriteColor(Color color)
	{
		base.GetComponent<tk2dSprite>().color = color;
	}

	// Token: 0x060034DA RID: 13530 RVA: 0x000EAA3D File Offset: 0x000E8C3D
	private void onEnable()
	{
	}

	// Token: 0x0400384E RID: 14414
	private Color Color = new Color(1f, 1f, 1f, 1f);

	// Token: 0x0400384F RID: 14415
	private float Duration = 0.25f;
}
