using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000616 RID: 1558
public class CheatCodeListener : MonoBehaviour
{
	// Token: 0x06003783 RID: 14211 RVA: 0x000F4BF1 File Offset: 0x000F2DF1
	private void Awake()
	{
		this.ia = ManagerSingleton<InputHandler>.Instance.inputActions;
	}

	// Token: 0x06003784 RID: 14212 RVA: 0x000F4C03 File Offset: 0x000F2E03
	private void OnEnable()
	{
		this.currentSequence.Clear();
	}

	// Token: 0x06003785 RID: 14213 RVA: 0x000F4C10 File Offset: 0x000F2E10
	private void Update()
	{
		int count = this.currentSequence.Count;
		if (this.ia.Up.WasPressed)
		{
			this.currentSequence.Add(CheatCodeListener.InputAction.Up);
		}
		if (this.ia.Down.WasPressed)
		{
			this.currentSequence.Add(CheatCodeListener.InputAction.Down);
		}
		if (this.ia.Left.WasPressed)
		{
			this.currentSequence.Add(CheatCodeListener.InputAction.Left);
		}
		if (this.ia.Right.WasPressed)
		{
			this.currentSequence.Add(CheatCodeListener.InputAction.Right);
		}
		if (this.currentSequence.Count <= count)
		{
			return;
		}
		CheatCodeListener.CheatCode cheatCode = null;
		foreach (CheatCodeListener.CheatCode cheatCode2 in this.cheatCodes)
		{
			if (cheatCode2.Sequence.Length == this.currentSequence.Count)
			{
				bool flag = false;
				for (int j = 0; j < cheatCode2.Sequence.Length; j++)
				{
					if (cheatCode2.Sequence[j] != this.currentSequence[j])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					cheatCode = cheatCode2;
					break;
				}
			}
		}
		if (cheatCode == null)
		{
			return;
		}
		cheatCode.Response(this);
		this.currentSequence.Clear();
	}

	// Token: 0x06003786 RID: 14214 RVA: 0x000F4D40 File Offset: 0x000F2F40
	public CheatCodeListener()
	{
		CheatCodeListener.CheatCode[] array = new CheatCodeListener.CheatCode[1];
		int num = 0;
		CheatCodeListener.CheatCode cheatCode = new CheatCodeListener.CheatCode();
		cheatCode.Sequence = new CheatCodeListener.InputAction[]
		{
			CheatCodeListener.InputAction.Up,
			CheatCodeListener.InputAction.Down,
			CheatCodeListener.InputAction.Up,
			CheatCodeListener.InputAction.Down,
			CheatCodeListener.InputAction.Left,
			CheatCodeListener.InputAction.Right,
			CheatCodeListener.InputAction.Left,
			CheatCodeListener.InputAction.Right
		};
		cheatCode.Response = delegate(CheatCodeListener listener)
		{
			if (GameManager.instance.GetStatusRecordInt("RecPermadeathMode") != 0)
			{
				return;
			}
			PermadeathUnlock.Unlock();
			Object.Instantiate<GameObject>(listener.permadeathUnlockEffect);
		};
		array[num] = cheatCode;
		this.cheatCodes = array;
		this.currentSequence = new List<CheatCodeListener.InputAction>();
		base..ctor();
	}

	// Token: 0x04003A7A RID: 14970
	[SerializeField]
	private GameObject permadeathUnlockEffect;

	// Token: 0x04003A7B RID: 14971
	private readonly CheatCodeListener.CheatCode[] cheatCodes;

	// Token: 0x04003A7C RID: 14972
	private readonly List<CheatCodeListener.InputAction> currentSequence;

	// Token: 0x04003A7D RID: 14973
	private HeroActions ia;

	// Token: 0x02001926 RID: 6438
	private enum InputAction
	{
		// Token: 0x04009491 RID: 38033
		Up,
		// Token: 0x04009492 RID: 38034
		Down,
		// Token: 0x04009493 RID: 38035
		Left,
		// Token: 0x04009494 RID: 38036
		Right
	}

	// Token: 0x02001927 RID: 6439
	private class CheatCode
	{
		// Token: 0x04009495 RID: 38037
		public CheatCodeListener.InputAction[] Sequence;

		// Token: 0x04009496 RID: 38038
		public Action<CheatCodeListener> Response;
	}
}
