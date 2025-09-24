using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000479 RID: 1145
public class PlayerDataTestResponse : MonoBehaviour
{
	// Token: 0x06002986 RID: 10630 RVA: 0x000B4DA2 File Offset: 0x000B2FA2
	private void Awake()
	{
		if (this.runOn != PlayerDataTestResponse.RunOnTypes.Awake)
		{
			return;
		}
		this.Evaluate();
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x000B4DB4 File Offset: 0x000B2FB4
	private void Start()
	{
		if (this.runOn != PlayerDataTestResponse.RunOnTypes.JustStart && this.runOn != PlayerDataTestResponse.RunOnTypes.StartThenOnEnable)
		{
			return;
		}
		this.hasStarted = true;
		this.Evaluate();
	}

	// Token: 0x06002988 RID: 10632 RVA: 0x000B4DD5 File Offset: 0x000B2FD5
	private void OnEnable()
	{
		if (this.runOn == PlayerDataTestResponse.RunOnTypes.StartThenOnEnable)
		{
			if (!this.hasStarted)
			{
				return;
			}
		}
		else if (this.runOn != PlayerDataTestResponse.RunOnTypes.OnEnable)
		{
			return;
		}
		this.Evaluate();
	}

	// Token: 0x06002989 RID: 10633 RVA: 0x000B4DF8 File Offset: 0x000B2FF8
	private void Evaluate()
	{
		if (this.test.IsFulfilled)
		{
			UnityEvent isFullfilled = this.IsFullfilled;
			if (isFullfilled == null)
			{
				return;
			}
			isFullfilled.Invoke();
			return;
		}
		else
		{
			UnityEvent isNotFulfilled = this.IsNotFulfilled;
			if (isNotFulfilled == null)
			{
				return;
			}
			isNotFulfilled.Invoke();
			return;
		}
	}

	// Token: 0x04002A19 RID: 10777
	[SerializeField]
	private PlayerDataTest test;

	// Token: 0x04002A1A RID: 10778
	[Space]
	[SerializeField]
	private PlayerDataTestResponse.RunOnTypes runOn;

	// Token: 0x04002A1B RID: 10779
	[Space]
	public UnityEvent IsFullfilled;

	// Token: 0x04002A1C RID: 10780
	[Space]
	public UnityEvent IsNotFulfilled;

	// Token: 0x04002A1D RID: 10781
	private bool hasStarted;

	// Token: 0x02001786 RID: 6022
	private enum RunOnTypes
	{
		// Token: 0x04008E5B RID: 36443
		StartThenOnEnable,
		// Token: 0x04008E5C RID: 36444
		JustStart,
		// Token: 0x04008E5D RID: 36445
		OnEnable,
		// Token: 0x04008E5E RID: 36446
		Awake
	}
}
