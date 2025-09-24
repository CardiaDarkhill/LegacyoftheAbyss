using System;
using UnityEngine;
using UnityEngine.Events;

namespace TMProOld
{
	// Token: 0x02000808 RID: 2056
	internal struct ColorTween : ITweenValue
	{
		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x060047EE RID: 18414 RVA: 0x0014F01E File Offset: 0x0014D21E
		// (set) Token: 0x060047EF RID: 18415 RVA: 0x0014F026 File Offset: 0x0014D226
		public Color startColor
		{
			get
			{
				return this.m_StartColor;
			}
			set
			{
				this.m_StartColor = value;
			}
		}

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x060047F0 RID: 18416 RVA: 0x0014F02F File Offset: 0x0014D22F
		// (set) Token: 0x060047F1 RID: 18417 RVA: 0x0014F037 File Offset: 0x0014D237
		public Color targetColor
		{
			get
			{
				return this.m_TargetColor;
			}
			set
			{
				this.m_TargetColor = value;
			}
		}

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x060047F2 RID: 18418 RVA: 0x0014F040 File Offset: 0x0014D240
		// (set) Token: 0x060047F3 RID: 18419 RVA: 0x0014F048 File Offset: 0x0014D248
		public ColorTween.ColorTweenMode tweenMode
		{
			get
			{
				return this.m_TweenMode;
			}
			set
			{
				this.m_TweenMode = value;
			}
		}

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x060047F4 RID: 18420 RVA: 0x0014F051 File Offset: 0x0014D251
		// (set) Token: 0x060047F5 RID: 18421 RVA: 0x0014F059 File Offset: 0x0014D259
		public float duration
		{
			get
			{
				return this.m_Duration;
			}
			set
			{
				this.m_Duration = value;
			}
		}

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x060047F6 RID: 18422 RVA: 0x0014F062 File Offset: 0x0014D262
		// (set) Token: 0x060047F7 RID: 18423 RVA: 0x0014F06A File Offset: 0x0014D26A
		public bool ignoreTimeScale
		{
			get
			{
				return this.m_IgnoreTimeScale;
			}
			set
			{
				this.m_IgnoreTimeScale = value;
			}
		}

		// Token: 0x060047F8 RID: 18424 RVA: 0x0014F074 File Offset: 0x0014D274
		public void TweenValue(float floatPercentage)
		{
			if (!this.ValidTarget())
			{
				return;
			}
			Color arg = Color.Lerp(this.m_StartColor, this.m_TargetColor, floatPercentage);
			if (this.m_TweenMode == ColorTween.ColorTweenMode.Alpha)
			{
				arg.r = this.m_StartColor.r;
				arg.g = this.m_StartColor.g;
				arg.b = this.m_StartColor.b;
			}
			else if (this.m_TweenMode == ColorTween.ColorTweenMode.RGB)
			{
				arg.a = this.m_StartColor.a;
			}
			this.m_Target.Invoke(arg);
		}

		// Token: 0x060047F9 RID: 18425 RVA: 0x0014F105 File Offset: 0x0014D305
		public void AddOnChangedCallback(UnityAction<Color> callback)
		{
			if (this.m_Target == null)
			{
				this.m_Target = new ColorTween.ColorTweenCallback();
			}
			this.m_Target.AddListener(callback);
		}

		// Token: 0x060047FA RID: 18426 RVA: 0x0014F126 File Offset: 0x0014D326
		public bool GetIgnoreTimescale()
		{
			return this.m_IgnoreTimeScale;
		}

		// Token: 0x060047FB RID: 18427 RVA: 0x0014F12E File Offset: 0x0014D32E
		public float GetDuration()
		{
			return this.m_Duration;
		}

		// Token: 0x060047FC RID: 18428 RVA: 0x0014F136 File Offset: 0x0014D336
		public bool ValidTarget()
		{
			return this.m_Target != null;
		}

		// Token: 0x0400487F RID: 18559
		private ColorTween.ColorTweenCallback m_Target;

		// Token: 0x04004880 RID: 18560
		private Color m_StartColor;

		// Token: 0x04004881 RID: 18561
		private Color m_TargetColor;

		// Token: 0x04004882 RID: 18562
		private ColorTween.ColorTweenMode m_TweenMode;

		// Token: 0x04004883 RID: 18563
		private float m_Duration;

		// Token: 0x04004884 RID: 18564
		private bool m_IgnoreTimeScale;

		// Token: 0x02001AB3 RID: 6835
		public enum ColorTweenMode
		{
			// Token: 0x04009A40 RID: 39488
			All,
			// Token: 0x04009A41 RID: 39489
			RGB,
			// Token: 0x04009A42 RID: 39490
			Alpha
		}

		// Token: 0x02001AB4 RID: 6836
		public class ColorTweenCallback : UnityEvent<Color>
		{
		}
	}
}
