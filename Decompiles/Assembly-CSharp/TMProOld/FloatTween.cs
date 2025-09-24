using System;
using UnityEngine;
using UnityEngine.Events;

namespace TMProOld
{
	// Token: 0x02000809 RID: 2057
	internal struct FloatTween : ITweenValue
	{
		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x060047FD RID: 18429 RVA: 0x0014F141 File Offset: 0x0014D341
		// (set) Token: 0x060047FE RID: 18430 RVA: 0x0014F149 File Offset: 0x0014D349
		public float startValue
		{
			get
			{
				return this.m_StartValue;
			}
			set
			{
				this.m_StartValue = value;
			}
		}

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x060047FF RID: 18431 RVA: 0x0014F152 File Offset: 0x0014D352
		// (set) Token: 0x06004800 RID: 18432 RVA: 0x0014F15A File Offset: 0x0014D35A
		public float targetValue
		{
			get
			{
				return this.m_TargetValue;
			}
			set
			{
				this.m_TargetValue = value;
			}
		}

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x06004801 RID: 18433 RVA: 0x0014F163 File Offset: 0x0014D363
		// (set) Token: 0x06004802 RID: 18434 RVA: 0x0014F16B File Offset: 0x0014D36B
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

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06004803 RID: 18435 RVA: 0x0014F174 File Offset: 0x0014D374
		// (set) Token: 0x06004804 RID: 18436 RVA: 0x0014F17C File Offset: 0x0014D37C
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

		// Token: 0x06004805 RID: 18437 RVA: 0x0014F188 File Offset: 0x0014D388
		public void TweenValue(float floatPercentage)
		{
			if (!this.ValidTarget())
			{
				return;
			}
			float arg = Mathf.Lerp(this.m_StartValue, this.m_TargetValue, floatPercentage);
			this.m_Target.Invoke(arg);
		}

		// Token: 0x06004806 RID: 18438 RVA: 0x0014F1BD File Offset: 0x0014D3BD
		public void AddOnChangedCallback(UnityAction<float> callback)
		{
			if (this.m_Target == null)
			{
				this.m_Target = new FloatTween.FloatTweenCallback();
			}
			this.m_Target.AddListener(callback);
		}

		// Token: 0x06004807 RID: 18439 RVA: 0x0014F1DE File Offset: 0x0014D3DE
		public bool GetIgnoreTimescale()
		{
			return this.m_IgnoreTimeScale;
		}

		// Token: 0x06004808 RID: 18440 RVA: 0x0014F1E6 File Offset: 0x0014D3E6
		public float GetDuration()
		{
			return this.m_Duration;
		}

		// Token: 0x06004809 RID: 18441 RVA: 0x0014F1EE File Offset: 0x0014D3EE
		public bool ValidTarget()
		{
			return this.m_Target != null;
		}

		// Token: 0x04004885 RID: 18565
		private FloatTween.FloatTweenCallback m_Target;

		// Token: 0x04004886 RID: 18566
		private float m_StartValue;

		// Token: 0x04004887 RID: 18567
		private float m_TargetValue;

		// Token: 0x04004888 RID: 18568
		private float m_Duration;

		// Token: 0x04004889 RID: 18569
		private bool m_IgnoreTimeScale;

		// Token: 0x02001AB5 RID: 6837
		public class FloatTweenCallback : UnityEvent<float>
		{
		}
	}
}
