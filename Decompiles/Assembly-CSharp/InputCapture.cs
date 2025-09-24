using System;
using System.Text;
using UnityEngine;

// Token: 0x02000770 RID: 1904
public sealed class InputCapture
{
	// Token: 0x060043F0 RID: 17392 RVA: 0x0012A4D4 File Offset: 0x001286D4
	public void Update()
	{
		if (!this.isCapturing)
		{
			return;
		}
		if (this.keyboard == null)
		{
			foreach (char c in Input.inputString)
			{
				if (c == '\b')
				{
					if (this.capturedInput.Length > 0)
					{
						this.capturedInput.Remove(this.capturedInput.Length - 1, 1);
						this.dirty = true;
					}
				}
				else if (c == '\n' || c == '\r')
				{
					if (!this.justStarted)
					{
						this.StopCapturing();
					}
				}
				else
				{
					this.capturedInput.Append(c);
					this.dirty = true;
				}
			}
			this.justStarted = false;
			return;
		}
		if (this.keyboard.status == TouchScreenKeyboard.Status.Done)
		{
			this.capturedInput.Clear();
			this.capturedInput.Append(this.keyboard.text);
			this.keyboard = null;
			this.dirty = true;
			this.StopCapturing();
		}
	}

	// Token: 0x060043F1 RID: 17393 RVA: 0x0012A5C4 File Offset: 0x001287C4
	public void StartCapturing(string startingText)
	{
		this.isCapturing = true;
		this.dirty = true;
		this.capturedInput.Clear();
		this.justStarted = true;
		if (!string.IsNullOrEmpty(startingText))
		{
			this.capturedInput.Append(startingText);
		}
		if (TouchScreenKeyboard.isSupported)
		{
			this.OpenTouchScreenKeyboard();
		}
	}

	// Token: 0x060043F2 RID: 17394 RVA: 0x0012A614 File Offset: 0x00128814
	public void StopCapturing()
	{
		this.isCapturing = false;
		if (this.keyboard != null)
		{
			this.capturedInput.Clear();
			this.capturedInput.Append(this.keyboard.text);
			this.keyboard = null;
			this.dirty = true;
		}
		Action<string> onKeyboardClosed = this.OnKeyboardClosed;
		if (onKeyboardClosed == null)
		{
			return;
		}
		onKeyboardClosed(this.GetCapturedInput());
	}

	// Token: 0x060043F3 RID: 17395 RVA: 0x0012A677 File Offset: 0x00128877
	public string GetCapturedInput()
	{
		if (this.dirty)
		{
			this.dirty = false;
			this.cachedString = this.capturedInput.ToString();
		}
		return this.cachedString;
	}

	// Token: 0x060043F4 RID: 17396 RVA: 0x0012A69F File Offset: 0x0012889F
	public void ClearCapturedInput()
	{
		this.dirty = true;
		this.capturedInput.Clear();
	}

	// Token: 0x060043F5 RID: 17397 RVA: 0x0012A6B4 File Offset: 0x001288B4
	private void OpenTouchScreenKeyboard()
	{
		this.keyboard = TouchScreenKeyboard.Open(this.GetCapturedInput(), TouchScreenKeyboardType.Default, false, false, false, false);
	}

	// Token: 0x0400453C RID: 17724
	private bool isCapturing;

	// Token: 0x0400453D RID: 17725
	private StringBuilder capturedInput = new StringBuilder();

	// Token: 0x0400453E RID: 17726
	private TouchScreenKeyboard keyboard;

	// Token: 0x0400453F RID: 17727
	public Action<string> OnKeyboardClosed;

	// Token: 0x04004540 RID: 17728
	private string cachedString;

	// Token: 0x04004541 RID: 17729
	private bool dirty = true;

	// Token: 0x04004542 RID: 17730
	private bool justStarted;
}
