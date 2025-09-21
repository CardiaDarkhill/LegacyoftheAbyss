using System;
using System.Collections;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000496 RID: 1174
public class BellShrineTuningForkGroup : MonoBehaviour
{
	// Token: 0x06002A6C RID: 10860 RVA: 0x000B7DF8 File Offset: 0x000B5FF8
	private void Start()
	{
		uint variable = PlayerData.instance.GetVariable(this.pdActivatedBitmask);
		if (variable == 4294967295U)
		{
			BellShrineTuningForkGroup.BellShrine[] array = this.bellShrines;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].TuningFork.SetInitialState(BellShrineTuningFork.States.AllComplete);
			}
		}
		else
		{
			for (int j = 0; j < this.bellShrines.Length; j++)
			{
				this.bellShrines[j].TuningFork.SetInitialState(variable.IsBitSet(j) ? BellShrineTuningFork.States.Activated : BellShrineTuningFork.States.Dormant);
			}
		}
		this.DoReset();
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x000B7E77 File Offset: 0x000B6077
	public void DoReset()
	{
		this.finalIcon.gameObject.SetActive(false);
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x000B7E8C File Offset: 0x000B608C
	public void DoActivation()
	{
		uint num = 0U;
		PlayerData instance = PlayerData.instance;
		uint num2 = instance.GetVariable(this.pdActivatedBitmask);
		int num3 = 0;
		for (int i = 0; i < this.bellShrines.Length; i++)
		{
			if (this.bellShrines[i].TuningFork.IsBellShrineCompleted)
			{
				num3++;
				if (!num2.IsBitSet(i))
				{
					num2 = num2.SetBitAtIndex(i);
					num = num.SetBitAtIndex(i);
				}
			}
		}
		if (num3 == this.bellShrines.Length)
		{
			num2 = uint.MaxValue;
			if (!string.IsNullOrWhiteSpace(this.awardAchievementOnComplete))
			{
				GameManager.instance.QueueAchievement(this.awardAchievementOnComplete);
			}
		}
		if (num != 0U)
		{
			instance.SetVariable(this.pdActivatedBitmask, num2);
		}
		base.StartCoroutine(this.ActivationRoutine(num));
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x000B7F44 File Offset: 0x000B6144
	private IEnumerator ActivationRoutine(uint newMask)
	{
		PlayerData instance = PlayerData.instance;
		uint activatedMask = instance.GetVariable(this.pdActivatedBitmask);
		uint existingMask = activatedMask & ~newMask;
		yield return new WaitForSeconds(this.iconsBeginDelay);
		WaitForSeconds wait = new WaitForSeconds(this.iconsActivateDelay);
		int num;
		for (int i = 0; i < this.bellShrines.Length; i = num + 1)
		{
			BellShrineTuningForkGroup.BellShrine bellShrine = this.bellShrines[i];
			bellShrine.Icon.gameObject.SetActive(true);
			if (existingMask.IsBitSet(i))
			{
				bellShrine.Icon.Play(BellShrineTuningForkGroup._appearActiveId);
				this.iconActivateSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, base.transform.position, null);
			}
			else
			{
				bellShrine.Icon.Play(BellShrineTuningForkGroup._appearInactiveId);
				this.iconAppearSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, base.transform.position, null);
			}
			if (existingMask.IsBitSet(i))
			{
				bellShrine.TuningFork.DoPulse();
			}
			yield return wait;
			num = i;
		}
		wait = null;
		yield return new WaitForSeconds(this.tuningForksBeginDelay);
		wait = new WaitForSeconds(this.tuningForksActivateDelay);
		for (int i = 0; i < this.bellShrines.Length; i = num + 1)
		{
			if (newMask.IsBitSet(i))
			{
				BellShrineTuningForkGroup.BellShrine shrine = this.bellShrines[i];
				shrine.Icon.Play(BellShrineTuningForkGroup._preActivateId);
				Vector3 pos = base.transform.position;
				this.iconPreActivateSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, pos, null);
				yield return base.StartCoroutine(shrine.TuningFork.DoActivate());
				this.iconActivateSound.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, pos, null);
				shrine.Icon.Play(BellShrineTuningForkGroup._activateId);
				yield return null;
				yield return new WaitForSeconds(shrine.Icon.GetCurrentAnimatorStateInfo(0).length);
				yield return wait;
				shrine = null;
				pos = default(Vector3);
			}
			num = i;
		}
		wait = null;
		bool isFinished = activatedMask == uint.MaxValue;
		if (isFinished)
		{
			this.finalIcon.gameObject.SetActive(true);
			this.finalIcon.Play(BellShrineTuningForkGroup._activateId);
			yield return null;
			yield return new WaitForSeconds(this.finalIcon.GetCurrentAnimatorStateInfo(0).length);
		}
		this.eventFsm.SendEvent(isFinished ? "ACTIVATED ALL" : "ACTIVATION COMPLETE");
		yield break;
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x000B7F5A File Offset: 0x000B615A
	public void DoCompletion()
	{
		base.StartCoroutine(this.CompletionRoutine());
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x000B7F69 File Offset: 0x000B6169
	private IEnumerator CompletionRoutine()
	{
		float num = 0f;
		foreach (BellShrineTuningForkGroup.BellShrine bellShrine in this.bellShrines)
		{
			float randomValue = this.tuningForksCompleteDelay.GetRandomValue();
			bellShrine.TuningFork.DoComplete(randomValue);
			if (randomValue > num)
			{
				num = randomValue;
			}
		}
		yield return new WaitForSeconds(num);
		this.eventFsm.SendEvent("COMPLETE ANIMS STARTED");
		yield break;
	}

	// Token: 0x04002B03 RID: 11011
	[SerializeField]
	[PlayerDataField(typeof(uint), true)]
	private string pdActivatedBitmask;

	// Token: 0x04002B04 RID: 11012
	[SerializeField]
	private string awardAchievementOnComplete;

	// Token: 0x04002B05 RID: 11013
	[SerializeField]
	private BellShrineTuningForkGroup.BellShrine[] bellShrines;

	// Token: 0x04002B06 RID: 11014
	[SerializeField]
	private Animator finalIcon;

	// Token: 0x04002B07 RID: 11015
	[SerializeField]
	private PlayMakerFSM eventFsm;

	// Token: 0x04002B08 RID: 11016
	[SerializeField]
	private float iconsBeginDelay;

	// Token: 0x04002B09 RID: 11017
	[SerializeField]
	private float iconsActivateDelay;

	// Token: 0x04002B0A RID: 11018
	[SerializeField]
	private float tuningForksBeginDelay;

	// Token: 0x04002B0B RID: 11019
	[SerializeField]
	private float tuningForksActivateDelay;

	// Token: 0x04002B0C RID: 11020
	[SerializeField]
	private MinMaxFloat tuningForksCompleteDelay;

	// Token: 0x04002B0D RID: 11021
	[SerializeField]
	private AudioEvent iconAppearSound;

	// Token: 0x04002B0E RID: 11022
	[SerializeField]
	private AudioEvent iconPreActivateSound;

	// Token: 0x04002B0F RID: 11023
	[SerializeField]
	private AudioEvent iconActivateSound;

	// Token: 0x04002B10 RID: 11024
	private static readonly int _appearInactiveId = Animator.StringToHash("Appear Inactive");

	// Token: 0x04002B11 RID: 11025
	private static readonly int _appearActiveId = Animator.StringToHash("Appear Active");

	// Token: 0x04002B12 RID: 11026
	private static readonly int _preActivateId = Animator.StringToHash("Pre Activate");

	// Token: 0x04002B13 RID: 11027
	private static readonly int _activateId = Animator.StringToHash("Activate");

	// Token: 0x020017A2 RID: 6050
	[Serializable]
	private class BellShrine
	{
		// Token: 0x04008EC4 RID: 36548
		public Animator Icon;

		// Token: 0x04008EC5 RID: 36549
		public BellShrineTuningFork TuningFork;
	}
}
