using System;
using System.Collections;
using UnityEngine;

// Token: 0x020005B9 RID: 1465
public class SendEventToRegisterOnEnable : MonoBehaviour
{
	// Token: 0x06003482 RID: 13442 RVA: 0x000E9624 File Offset: 0x000E7824
	private void OnEnable()
	{
		if (string.IsNullOrEmpty(this.sendEvent))
		{
			return;
		}
		switch (this.delayType)
		{
		case SendEventToRegisterOnEnable.DelayTypes.None:
			EventRegister.SendEvent(this.sendEvent, null);
			return;
		case SendEventToRegisterOnEnable.DelayTypes.Frame:
			base.StartCoroutine(this.SendEventDelayedFrame());
			return;
		case SendEventToRegisterOnEnable.DelayTypes.HalfSecond:
			base.StartCoroutine(this.SendEventDelayedTime(0.5f));
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06003483 RID: 13443 RVA: 0x000E968D File Offset: 0x000E788D
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06003484 RID: 13444 RVA: 0x000E9695 File Offset: 0x000E7895
	private IEnumerator SendEventDelayedFrame()
	{
		yield return null;
		EventRegister.SendEvent(this.sendEvent, null);
		yield break;
	}

	// Token: 0x06003485 RID: 13445 RVA: 0x000E96A4 File Offset: 0x000E78A4
	private IEnumerator SendEventDelayedTime(float delay)
	{
		yield return new WaitForSeconds(delay);
		EventRegister.SendEvent(this.sendEvent, null);
		yield break;
	}

	// Token: 0x040037F6 RID: 14326
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private string sendEvent;

	// Token: 0x040037F7 RID: 14327
	[SerializeField]
	private SendEventToRegisterOnEnable.DelayTypes delayType;

	// Token: 0x020018D9 RID: 6361
	private enum DelayTypes
	{
		// Token: 0x0400937F RID: 37759
		None,
		// Token: 0x04009380 RID: 37760
		Frame,
		// Token: 0x04009381 RID: 37761
		HalfSecond
	}
}
