using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public static class ScreenFaderUtils
{
	// Token: 0x0600015E RID: 350 RVA: 0x000080BC File Offset: 0x000062BC
	public static void Fade(Color startColour, Color endColour, float duration)
	{
		GameManager instance = GameManager.instance;
		if (instance == null)
		{
			return;
		}
		PlayMakerFSM screenFader_fsm = instance.screenFader_fsm;
		screenFader_fsm.FsmVariables.GetFsmColor("Start Colour").Value = startColour;
		screenFader_fsm.FsmVariables.GetFsmColor("End Colour").Value = endColour;
		screenFader_fsm.FsmVariables.GetFsmFloat("Duration").Value = duration;
		screenFader_fsm.SendEvent("CUSTOM FADE");
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000812B File Offset: 0x0000632B
	public static void SetColour(Color colour)
	{
		ScreenFaderUtils.Fade(colour, colour, 0f);
	}

	// Token: 0x06000160 RID: 352 RVA: 0x0000813C File Offset: 0x0000633C
	public static Color GetColour()
	{
		GameManager instance = GameManager.instance;
		if (instance == null)
		{
			Debug.Log("GameManager could not be found");
			return Color.clear;
		}
		return instance.screenFader_fsm.FsmVariables.GetFsmGameObject("Screen Fader").Value.GetComponent<SpriteRenderer>().color;
	}
}
