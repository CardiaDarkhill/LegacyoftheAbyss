using System;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000524 RID: 1316
public class NoiseResponder : MonoBehaviour
{
	// Token: 0x14000095 RID: 149
	// (add) Token: 0x06002F4E RID: 12110 RVA: 0x000D09A8 File Offset: 0x000CEBA8
	// (remove) Token: 0x06002F4F RID: 12111 RVA: 0x000D09E0 File Offset: 0x000CEBE0
	public event Action NoiseStarted;

	// Token: 0x14000096 RID: 150
	// (add) Token: 0x06002F50 RID: 12112 RVA: 0x000D0A18 File Offset: 0x000CEC18
	// (remove) Token: 0x06002F51 RID: 12113 RVA: 0x000D0A50 File Offset: 0x000CEC50
	public event Action NoiseEnded;

	// Token: 0x06002F52 RID: 12114 RVA: 0x000D0A85 File Offset: 0x000CEC85
	[UsedImplicitly]
	private bool? ValidateFsmEvent(string fsmEvent)
	{
		return this.eventTarget.IsEventValid(fsmEvent, false);
	}

	// Token: 0x06002F53 RID: 12115 RVA: 0x000D0A94 File Offset: 0x000CEC94
	private void OnEnable()
	{
		NoiseMaker.NoiseCreated += this.OnNoiseCreated;
	}

	// Token: 0x06002F54 RID: 12116 RVA: 0x000D0AA7 File Offset: 0x000CECA7
	private void OnDisable()
	{
		NoiseMaker.NoiseCreated -= this.OnNoiseCreated;
	}

	// Token: 0x06002F55 RID: 12117 RVA: 0x000D0ABC File Offset: 0x000CECBC
	private void Update()
	{
		bool flag = this.wasNoiseMade && Time.timeAsDouble >= this.noiseRespondTime;
		if (!flag && !this.ignoreNeedolin && HeroPerformanceRegion.GetAffectedState(base.transform, this.ignoreRange) != HeroPerformanceRegion.AffectedState.None)
		{
			flag = true;
		}
		if (flag && !this.wasNoiseResponded)
		{
			this.wasNoiseMade = false;
			this.wasNoiseResponded = true;
			this.OnNoiseStarted();
		}
		if (!flag && this.wasNoiseResponded)
		{
			this.wasNoiseResponded = false;
			this.OnNoiseEnded();
		}
	}

	// Token: 0x06002F56 RID: 12118 RVA: 0x000D0B3C File Offset: 0x000CED3C
	private void OnNoiseCreated(Vector2 _, NoiseMaker.NoiseEventCheck isNoiseInRange, NoiseMaker.Intensities intensity, bool allowOffScreen)
	{
		if (intensity < this.minIntensity)
		{
			return;
		}
		if (!isNoiseInRange(base.transform.position))
		{
			return;
		}
		if (Time.timeAsDouble < this.nextRespondTime)
		{
			return;
		}
		this.nextRespondTime = Time.timeAsDouble + (double)this.cooldown;
		this.wasNoiseMade = true;
		this.noiseRespondTime = Time.timeAsDouble + (double)this.respondDelay.GetRandomValue();
		if (this.eventTarget && !string.IsNullOrEmpty(this.noisePreDelayFsmEvent))
		{
			this.eventTarget.SendEvent(this.noisePreDelayFsmEvent);
		}
	}

	// Token: 0x06002F57 RID: 12119 RVA: 0x000D0BD7 File Offset: 0x000CEDD7
	private void OnNoiseStarted()
	{
		if (this.NoiseStarted != null)
		{
			this.NoiseStarted();
		}
		if (this.eventTarget && !string.IsNullOrEmpty(this.noiseStartedFsmEvent))
		{
			this.eventTarget.SendEvent(this.noiseStartedFsmEvent);
		}
	}

	// Token: 0x06002F58 RID: 12120 RVA: 0x000D0C17 File Offset: 0x000CEE17
	private void OnNoiseEnded()
	{
		if (this.NoiseEnded != null)
		{
			this.NoiseEnded();
		}
		if (this.eventTarget && !string.IsNullOrEmpty(this.noiseEndedFsmEvent))
		{
			this.eventTarget.SendEvent(this.noiseEndedFsmEvent);
		}
	}

	// Token: 0x04003212 RID: 12818
	[SerializeField]
	[FormerlySerializedAs("IgnoreNeedolin")]
	private bool ignoreNeedolin;

	// Token: 0x04003213 RID: 12819
	[SerializeField]
	private bool ignoreRange;

	// Token: 0x04003214 RID: 12820
	[SerializeField]
	private NoiseMaker.Intensities minIntensity;

	// Token: 0x04003215 RID: 12821
	[SerializeField]
	private MinMaxFloat respondDelay;

	// Token: 0x04003216 RID: 12822
	[SerializeField]
	private float cooldown;

	// Token: 0x04003217 RID: 12823
	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	// Token: 0x04003218 RID: 12824
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string noisePreDelayFsmEvent;

	// Token: 0x04003219 RID: 12825
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string noiseStartedFsmEvent;

	// Token: 0x0400321A RID: 12826
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string noiseEndedFsmEvent;

	// Token: 0x0400321B RID: 12827
	private bool wasNoiseMade;

	// Token: 0x0400321C RID: 12828
	private double noiseRespondTime;

	// Token: 0x0400321D RID: 12829
	private bool wasNoiseResponded;

	// Token: 0x0400321E RID: 12830
	private double nextRespondTime;
}
