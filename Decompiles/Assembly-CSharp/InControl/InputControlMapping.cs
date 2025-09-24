using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008F4 RID: 2292
	[Serializable]
	public class InputControlMapping
	{
		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06005045 RID: 20549 RVA: 0x0017290C File Offset: 0x00170B0C
		// (set) Token: 0x06005046 RID: 20550 RVA: 0x00172941 File Offset: 0x00170B41
		public string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(this.name))
				{
					return this.name;
				}
				return this.Target.ToString();
			}
			set
			{
				this.name = value;
			}
		}

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06005047 RID: 20551 RVA: 0x0017294A File Offset: 0x00170B4A
		// (set) Token: 0x06005048 RID: 20552 RVA: 0x00172952 File Offset: 0x00170B52
		public bool Invert
		{
			get
			{
				return this.invert;
			}
			set
			{
				this.invert = value;
			}
		}

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x06005049 RID: 20553 RVA: 0x0017295B File Offset: 0x00170B5B
		// (set) Token: 0x0600504A RID: 20554 RVA: 0x00172963 File Offset: 0x00170B63
		public float Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				this.scale = value;
			}
		}

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x0600504B RID: 20555 RVA: 0x0017296C File Offset: 0x00170B6C
		// (set) Token: 0x0600504C RID: 20556 RVA: 0x00172974 File Offset: 0x00170B74
		public bool Raw
		{
			get
			{
				return this.raw;
			}
			set
			{
				this.raw = value;
			}
		}

		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x0600504D RID: 20557 RVA: 0x0017297D File Offset: 0x00170B7D
		// (set) Token: 0x0600504E RID: 20558 RVA: 0x00172985 File Offset: 0x00170B85
		public bool Passive
		{
			get
			{
				return this.passive;
			}
			set
			{
				this.passive = value;
			}
		}

		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x0600504F RID: 20559 RVA: 0x0017298E File Offset: 0x00170B8E
		// (set) Token: 0x06005050 RID: 20560 RVA: 0x00172996 File Offset: 0x00170B96
		public bool IgnoreInitialZeroValue
		{
			get
			{
				return this.ignoreInitialZeroValue;
			}
			set
			{
				this.ignoreInitialZeroValue = value;
			}
		}

		// Token: 0x17000A88 RID: 2696
		// (get) Token: 0x06005051 RID: 20561 RVA: 0x0017299F File Offset: 0x00170B9F
		// (set) Token: 0x06005052 RID: 20562 RVA: 0x001729A7 File Offset: 0x00170BA7
		public float Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
			set
			{
				this.sensitivity = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x06005053 RID: 20563 RVA: 0x001729B5 File Offset: 0x00170BB5
		// (set) Token: 0x06005054 RID: 20564 RVA: 0x001729BD File Offset: 0x00170BBD
		public float LowerDeadZone
		{
			get
			{
				return this.lowerDeadZone;
			}
			set
			{
				this.lowerDeadZone = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x06005055 RID: 20565 RVA: 0x001729CB File Offset: 0x00170BCB
		// (set) Token: 0x06005056 RID: 20566 RVA: 0x001729D3 File Offset: 0x00170BD3
		public float UpperDeadZone
		{
			get
			{
				return this.upperDeadZone;
			}
			set
			{
				this.upperDeadZone = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000A8B RID: 2699
		// (get) Token: 0x06005057 RID: 20567 RVA: 0x001729E1 File Offset: 0x00170BE1
		// (set) Token: 0x06005058 RID: 20568 RVA: 0x001729E9 File Offset: 0x00170BE9
		public InputControlSource Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x06005059 RID: 20569 RVA: 0x001729F2 File Offset: 0x00170BF2
		// (set) Token: 0x0600505A RID: 20570 RVA: 0x001729FA File Offset: 0x00170BFA
		public InputControlType Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}

		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x0600505B RID: 20571 RVA: 0x00172A03 File Offset: 0x00170C03
		// (set) Token: 0x0600505C RID: 20572 RVA: 0x00172A0B File Offset: 0x00170C0B
		public InputRangeType SourceRange
		{
			get
			{
				return this.sourceRange;
			}
			set
			{
				this.sourceRange = value;
			}
		}

		// Token: 0x17000A8E RID: 2702
		// (get) Token: 0x0600505D RID: 20573 RVA: 0x00172A14 File Offset: 0x00170C14
		// (set) Token: 0x0600505E RID: 20574 RVA: 0x00172A1C File Offset: 0x00170C1C
		public InputRangeType TargetRange
		{
			get
			{
				return this.targetRange;
			}
			set
			{
				this.targetRange = value;
			}
		}

		// Token: 0x0600505F RID: 20575 RVA: 0x00172A28 File Offset: 0x00170C28
		public float ApplyToValue(float value)
		{
			if (this.Raw)
			{
				value *= this.Scale;
				value = (InputRange.Excludes(this.sourceRange, value) ? 0f : value);
			}
			else
			{
				value = Mathf.Clamp(value * this.Scale, -1f, 1f);
				value = InputRange.Remap(value, this.sourceRange, this.targetRange);
			}
			if (this.Invert)
			{
				value = -value;
			}
			return value;
		}

		// Token: 0x040050BF RID: 20671
		[SerializeField]
		private string name = "";

		// Token: 0x040050C0 RID: 20672
		[SerializeField]
		private bool invert;

		// Token: 0x040050C1 RID: 20673
		[SerializeField]
		private float scale = 1f;

		// Token: 0x040050C2 RID: 20674
		[SerializeField]
		private bool raw;

		// Token: 0x040050C3 RID: 20675
		[SerializeField]
		private bool passive;

		// Token: 0x040050C4 RID: 20676
		[SerializeField]
		private bool ignoreInitialZeroValue;

		// Token: 0x040050C5 RID: 20677
		[SerializeField]
		private float sensitivity = 1f;

		// Token: 0x040050C6 RID: 20678
		[SerializeField]
		private float lowerDeadZone;

		// Token: 0x040050C7 RID: 20679
		[SerializeField]
		private float upperDeadZone = 1f;

		// Token: 0x040050C8 RID: 20680
		[SerializeField]
		private InputControlSource source;

		// Token: 0x040050C9 RID: 20681
		[SerializeField]
		private InputControlType target;

		// Token: 0x040050CA RID: 20682
		[SerializeField]
		private InputRangeType sourceRange = InputRangeType.MinusOneToOne;

		// Token: 0x040050CB RID: 20683
		[SerializeField]
		private InputRangeType targetRange = InputRangeType.MinusOneToOne;
	}
}
