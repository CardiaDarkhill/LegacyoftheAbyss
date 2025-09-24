using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002B2 RID: 690
public class CorpseLandTint : MonoBehaviour
{
	// Token: 0x06001884 RID: 6276 RVA: 0x000709F0 File Offset: 0x0006EBF0
	private void Awake()
	{
		List<tk2dSprite> list = new List<tk2dSprite>();
		foreach (tk2dSprite tk2dSprite in base.GetComponentsInChildren<tk2dSprite>(true))
		{
			if (!(tk2dSprite == null) && NonTinter.CanTint(tk2dSprite.gameObject, NonTinter.TintFlag.CorpseLand))
			{
				list.Add(tk2dSprite);
			}
		}
		this.childSprites = list.ToArray();
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x00070A48 File Offset: 0x0006EC48
	public void Landed(bool desaturate)
	{
		if (this.hasLanded)
		{
			return;
		}
		this.hasLanded = true;
		int count = this.childSprites.Length;
		Color[] startColors = new Color[count];
		for (int i = 0; i < count; i++)
		{
			startColors[i] = this.childSprites[i].color;
		}
		MeshRenderer[] renderers = null;
		MaterialPropertyBlock matPropBlock = null;
		if (desaturate)
		{
			matPropBlock = new MaterialPropertyBlock();
			renderers = new MeshRenderer[count];
			int count2 = count;
			int num = 0;
			for (int j = 0; j < count; j++)
			{
				renderers[num] = this.childSprites[j].GetComponent<MeshRenderer>();
			}
		}
		this.StartTimerRoutine(GlobalSettings.Corpse.LandTintWaitTime, GlobalSettings.Corpse.LandTintFadeTime, delegate(float time)
		{
			float value = GlobalSettings.Corpse.LandDesaturationCurve.Evaluate(time);
			time = GlobalSettings.Corpse.LandTintCurve.Evaluate(time);
			for (int k = 0; k < count; k++)
			{
				Color a = startColors[k];
				Color b = a * GlobalSettings.Corpse.LandTint;
				this.childSprites[k].color = Color.Lerp(a, b, time);
				if (desaturate)
				{
					MeshRenderer meshRenderer = renderers[k];
					meshRenderer.GetPropertyBlock(matPropBlock);
					matPropBlock.SetFloat(CorpseLandTint._desaturationProp, value);
					meshRenderer.SetPropertyBlock(matPropBlock);
				}
			}
		}, null, null, false);
	}

	// Token: 0x0400177F RID: 6015
	private tk2dSprite[] childSprites;

	// Token: 0x04001780 RID: 6016
	private bool hasLanded;

	// Token: 0x04001781 RID: 6017
	private static readonly int _desaturationProp = Shader.PropertyToID("_Desaturation");
}
