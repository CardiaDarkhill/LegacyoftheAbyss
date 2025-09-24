using System;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008A1 RID: 2209
	public sealed class PlaystationGamePadManager : MonoBehaviour
	{
		// Token: 0x06004C72 RID: 19570 RVA: 0x001681EC File Offset: 0x001663EC
		private void Start()
		{
			GamePad[] gamePads = GamePad.gamePads;
			for (int i = 0; i < gamePads.Length; i++)
			{
				gamePads[i].InitGamepad();
			}
		}

		// Token: 0x06004C73 RID: 19571 RVA: 0x00168218 File Offset: 0x00166418
		private void Update()
		{
			GamePad[] gamePads = GamePad.gamePads;
			for (int i = 0; i < gamePads.Length; i++)
			{
				gamePads[i].Update();
			}
		}
	}
}
