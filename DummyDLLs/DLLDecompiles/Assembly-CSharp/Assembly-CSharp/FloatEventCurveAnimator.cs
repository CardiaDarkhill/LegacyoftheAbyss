using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000649 RID: 1609
public class FloatEventCurveAnimator : FloatCurveAnimator
{
	// Token: 0x1700068A RID: 1674
	// (get) Token: 0x060039A5 RID: 14757 RVA: 0x000FD12A File Offset: 0x000FB32A
	// (set) Token: 0x060039A6 RID: 14758 RVA: 0x000FD132 File Offset: 0x000FB332
	protected override float Value
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
			this.OnValueChanged.Invoke(value);
		}
	}

	// Token: 0x04003C61 RID: 15457
	[SerializeField]
	private float value;

	// Token: 0x04003C62 RID: 15458
	public FloatEventCurveAnimator.UnityFloatEvent OnValueChanged;

	// Token: 0x02001966 RID: 6502
	[Serializable]
	public class UnityFloatEvent : UnityEvent<float>
	{
	}
}
