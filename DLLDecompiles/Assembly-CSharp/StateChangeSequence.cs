using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000561 RID: 1377
public class StateChangeSequence : MonoBehaviour
{
	// Token: 0x06003129 RID: 12585 RVA: 0x000DA5DC File Offset: 0x000D87DC
	private void Awake()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out int value)
			{
				value = this.stateValue;
			};
			this.persistent.OnSetSaveState += delegate(int value)
			{
				if (!this.CheckCompleteBool())
				{
					this.SetState(value, true);
				}
			};
		}
	}

	// Token: 0x0600312A RID: 12586 RVA: 0x000DA619 File Offset: 0x000D8819
	private void Start()
	{
		if (!this.CheckCompleteBool() && this.stateValue < 0)
		{
			this.SetState(this.initialState, true);
		}
	}

	// Token: 0x0600312B RID: 12587 RVA: 0x000DA639 File Offset: 0x000D8839
	private bool CheckCompleteBool()
	{
		if (!string.IsNullOrEmpty(this.isCompleteBool) && PlayerData.instance.GetVariable(this.isCompleteBool))
		{
			this.SetState(this.states.Length - 1, true);
			return true;
		}
		return false;
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x000DA670 File Offset: 0x000D8870
	private void SetState(int index, bool isReturning)
	{
		if (index >= this.states.Length)
		{
			return;
		}
		int num = this.stateValue;
		this.stateValue = index;
		if (this.stateValue <= num)
		{
			return;
		}
		if (this.stateValue == this.states.Length - 1)
		{
			this.SetIsCompleteBool();
		}
		for (int i = Mathf.Max(0, num + 1); i <= this.stateValue; i++)
		{
			StateChangeSequence.State state = this.states[i];
			UnityEvent unityEvent;
			string text;
			if (isReturning && state.UseOnReturn)
			{
				unityEvent = state.OnReturn;
				text = state.OnReturnEventRegister;
			}
			else
			{
				unityEvent = state.OnEnter;
				text = state.OnEnterEventRegister;
			}
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			if (!string.IsNullOrEmpty(text))
			{
				EventRegister.SendEvent(text, null);
			}
		}
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x000DA720 File Offset: 0x000D8920
	public void SetIsCompleteBool()
	{
		if (!string.IsNullOrEmpty(this.isCompleteBool))
		{
			PlayerData.instance.SetVariable(this.isCompleteBool, true);
		}
		if (!string.IsNullOrWhiteSpace(this.awardAchievement))
		{
			if (this.queueAchievement)
			{
				GameManager.instance.QueueAchievement(this.awardAchievement);
				return;
			}
			GameManager.instance.AwardAchievement(this.awardAchievement);
		}
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x000DA781 File Offset: 0x000D8981
	public void SetState(int index)
	{
		this.SetState(index, false);
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x000DA78B File Offset: 0x000D898B
	public void SetStateReturn(int index)
	{
		this.SetState(index, true);
	}

	// Token: 0x06003130 RID: 12592 RVA: 0x000DA795 File Offset: 0x000D8995
	public void IncrementState()
	{
		this.SetState(this.stateValue + 1);
	}

	// Token: 0x06003131 RID: 12593 RVA: 0x000DA7A5 File Offset: 0x000D89A5
	public void SetLastState()
	{
		this.SetState(this.states.Length - 1);
	}

	// Token: 0x04003491 RID: 13457
	[SerializeField]
	private PersistentIntItem persistent;

	// Token: 0x04003492 RID: 13458
	[SerializeField]
	private int initialState;

	// Token: 0x04003493 RID: 13459
	[Space]
	[SerializeField]
	private StateChangeSequence.State[] states = new StateChangeSequence.State[1];

	// Token: 0x04003494 RID: 13460
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string isCompleteBool;

	// Token: 0x04003495 RID: 13461
	[SerializeField]
	private string awardAchievement;

	// Token: 0x04003496 RID: 13462
	[SerializeField]
	private bool queueAchievement;

	// Token: 0x04003497 RID: 13463
	private int stateValue = -1;

	// Token: 0x02001864 RID: 6244
	[Serializable]
	private class State
	{
		// Token: 0x040091D8 RID: 37336
		public UnityEvent OnEnter;

		// Token: 0x040091D9 RID: 37337
		public string OnEnterEventRegister;

		// Token: 0x040091DA RID: 37338
		[Space]
		public bool UseOnReturn;

		// Token: 0x040091DB RID: 37339
		public UnityEvent OnReturn;

		// Token: 0x040091DC RID: 37340
		public string OnReturnEventRegister;
	}
}
