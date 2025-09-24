using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x02000633 RID: 1587
public class MemoryMsgBox : MonoBehaviour
{
	// Token: 0x0600389D RID: 14493 RVA: 0x000F9EA4 File Offset: 0x000F80A4
	private void Awake()
	{
		if (!MemoryMsgBox._instance)
		{
			MemoryMsgBox._instance = this;
		}
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = 0f;
		}
		this.gm = GameManager.instance;
		this.gm.NextSceneWillActivate += this.ClearAllText;
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x000F9F04 File Offset: 0x000F8104
	private void OnDestroy()
	{
		if (MemoryMsgBox._instance == this)
		{
			MemoryMsgBox._instance = null;
		}
		if (!this.gm)
		{
			return;
		}
		this.gm.NextSceneWillActivate -= this.ClearAllText;
		this.gm = null;
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x000F9F50 File Offset: 0x000F8150
	public static void AddText(Object source, string text)
	{
		if (!MemoryMsgBox._instance)
		{
			return;
		}
		MemoryMsgBox._instance.InternalAddText(source, text);
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x000F9F6B File Offset: 0x000F816B
	public static void RemoveText(Object source, float disappearDelay)
	{
		if (!MemoryMsgBox._instance)
		{
			return;
		}
		MemoryMsgBox._instance.InternalRemoveText(source, disappearDelay);
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x000F9F88 File Offset: 0x000F8188
	private void InternalAddText(Object source, string text)
	{
		for (int i = this.messageStack.Count - 1; i >= 0; i--)
		{
			if (this.messageStack[i].Source == source)
			{
				this.messageStack.RemoveAt(i);
			}
		}
		this.messageStack.Add(new MemoryMsgBox.Message
		{
			Source = source,
			Text = text
		});
		if (this.delayedDisappearRoutine != null)
		{
			base.StopCoroutine(this.delayedDisappearRoutine);
			this.delayedDisappearRoutine = null;
		}
		if (this.appearRoutine == null)
		{
			this.appearRoutine = base.StartCoroutine(this.Appear());
			return;
		}
		this.queuedRefresh = true;
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x000FA034 File Offset: 0x000F8234
	private void InternalRemoveText(Object source, float disappearDelay)
	{
		for (int i = this.messageStack.Count - 1; i >= 0; i--)
		{
			if (this.messageStack[i].Source == source)
			{
				this.messageStack.RemoveAt(i);
			}
		}
		if (this.delayedDisappearRoutine != null)
		{
			base.StopCoroutine(this.delayedDisappearRoutine);
			this.delayedDisappearRoutine = null;
		}
		if (this.appearRoutine != null)
		{
			if (disappearDelay > 0f)
			{
				this.delayedDisappearRoutine = base.StartCoroutine(this.DelayRefresh(disappearDelay));
				return;
			}
			this.queuedRefresh = true;
		}
	}

	// Token: 0x060038A3 RID: 14499 RVA: 0x000FA0C4 File Offset: 0x000F82C4
	private void ClearAllText()
	{
		this.messageStack.Clear();
		if (this.delayedDisappearRoutine != null)
		{
			base.StopCoroutine(this.delayedDisappearRoutine);
			this.delayedDisappearRoutine = null;
		}
		if (this.appearRoutine != null)
		{
			base.StopCoroutine(this.appearRoutine);
			this.appearRoutine = null;
		}
		this.fadeGroup.AlphaSelf = 0f;
	}

	// Token: 0x060038A4 RID: 14500 RVA: 0x000FA122 File Offset: 0x000F8322
	private IEnumerator DelayRefresh(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (this.appearRoutine != null)
		{
			this.queuedRefresh = true;
		}
		this.delayedDisappearRoutine = null;
		yield break;
	}

	// Token: 0x060038A5 RID: 14501 RVA: 0x000FA138 File Offset: 0x000F8338
	private IEnumerator Appear()
	{
		Color colour = ScreenFaderUtils.GetColour();
		bool flag = colour.a < 0.5f || colour.r < 0.5f;
		this.regularFolder.SetActive(flag);
		this.whiteBackFolder.SetActive(!flag);
		while (this.messageStack.Count > 0)
		{
			MemoryMsgBox.Message msg = this.messageStack[this.messageStack.Count - 1];
			TMP_Text[] array = this.textDisplays;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = msg.Text;
			}
			float elapsed = 0f;
			while (elapsed < this.fadeUpTime && !this.queuedRefresh)
			{
				this.fadeGroup.AlphaSelf = this.fadeUpCurve.Evaluate(elapsed / this.fadeUpTime);
				yield return null;
				elapsed += Time.deltaTime;
			}
			this.fadeGroup.AlphaSelf = 1f;
			for (;;)
			{
				if (this.queuedRefresh)
				{
					this.queuedRefresh = false;
					if (this.messageStack.Count <= 0 || !(this.messageStack[this.messageStack.Count - 1].Text == msg.Text))
					{
						break;
					}
				}
				else
				{
					yield return null;
				}
			}
			elapsed = 0f;
			float startAlpha = this.fadeGroup.AlphaSelf;
			while (elapsed < this.fadeDownTime)
			{
				this.fadeGroup.AlphaSelf = Mathf.Lerp(0f, startAlpha, this.fadeDownCurve.Evaluate(elapsed / this.fadeDownTime));
				yield return null;
				elapsed += Time.deltaTime;
			}
			this.queuedRefresh = false;
			msg = default(MemoryMsgBox.Message);
		}
		this.fadeGroup.AlphaSelf = 0f;
		this.appearRoutine = null;
		yield break;
	}

	// Token: 0x04003B92 RID: 15250
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04003B93 RID: 15251
	[SerializeField]
	private TMP_Text[] textDisplays;

	// Token: 0x04003B94 RID: 15252
	[SerializeField]
	private float fadeUpTime;

	// Token: 0x04003B95 RID: 15253
	[SerializeField]
	private AnimationCurve fadeUpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003B96 RID: 15254
	[SerializeField]
	private float fadeDownTime;

	// Token: 0x04003B97 RID: 15255
	[SerializeField]
	private AnimationCurve fadeDownCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04003B98 RID: 15256
	[SerializeField]
	private GameObject regularFolder;

	// Token: 0x04003B99 RID: 15257
	[SerializeField]
	private GameObject whiteBackFolder;

	// Token: 0x04003B9A RID: 15258
	private readonly List<MemoryMsgBox.Message> messageStack = new List<MemoryMsgBox.Message>();

	// Token: 0x04003B9B RID: 15259
	private Coroutine appearRoutine;

	// Token: 0x04003B9C RID: 15260
	private bool queuedRefresh;

	// Token: 0x04003B9D RID: 15261
	private Coroutine delayedDisappearRoutine;

	// Token: 0x04003B9E RID: 15262
	private int currentBackboardSpriteIndex;

	// Token: 0x04003B9F RID: 15263
	private bool hasAppeared;

	// Token: 0x04003BA0 RID: 15264
	private GameManager gm;

	// Token: 0x04003BA1 RID: 15265
	private static MemoryMsgBox _instance;

	// Token: 0x0200194C RID: 6476
	private struct Message
	{
		// Token: 0x04009537 RID: 38199
		public Object Source;

		// Token: 0x04009538 RID: 38200
		public string Text;
	}
}
