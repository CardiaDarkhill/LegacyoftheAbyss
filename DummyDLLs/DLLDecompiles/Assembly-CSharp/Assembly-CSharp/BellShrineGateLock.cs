using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000494 RID: 1172
public class BellShrineGateLock : MonoBehaviour
{
	// Token: 0x06002A59 RID: 10841 RVA: 0x000B7B6F File Offset: 0x000B5D6F
	private void Start()
	{
		this.UpdateActivation();
	}

	// Token: 0x06002A5A RID: 10842 RVA: 0x000B7B78 File Offset: 0x000B5D78
	private void UpdateActivation()
	{
		PlayerData instance = PlayerData.instance;
		foreach (BellShrineGateLock.BellLock bellLock in this.bellLocks)
		{
			bellLock.Root.SetActive(instance.GetVariable(bellLock.PdBool));
		}
	}

	// Token: 0x06002A5B RID: 10843 RVA: 0x000B7BBC File Offset: 0x000B5DBC
	public bool GetIsAllUnlocked()
	{
		PlayerData instance = PlayerData.instance;
		foreach (BellShrineGateLock.BellLock bellLock in this.bellLocks)
		{
			if (!instance.GetVariable(bellLock.PdBool))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002A5C RID: 10844 RVA: 0x000B7BF9 File Offset: 0x000B5DF9
	public void StartedPlaying()
	{
		if (this.playRoutine != null)
		{
			return;
		}
		this.UpdateActivation();
		this.isPlaying = true;
		this.playRoutine = base.StartCoroutine(this.PlayRoutine());
	}

	// Token: 0x06002A5D RID: 10845 RVA: 0x000B7C23 File Offset: 0x000B5E23
	public void StoppedPlaying()
	{
		this.isPlaying = false;
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x000B7C2C File Offset: 0x000B5E2C
	private IEnumerator PlayRoutine()
	{
		yield return new WaitForSeconds(this.startDelay);
		if (!this.isPlaying)
		{
			this.playRoutine = null;
			yield break;
		}
		WaitForSeconds bellWait = new WaitForSeconds(this.bellDelay);
		PlayerData pd = PlayerData.instance;
		bool allDone = true;
		int playedCount = 0;
		int num;
		for (int i = 0; i < this.bellLocks.Length; i = num + 1)
		{
			if (i > 0)
			{
				yield return bellWait;
			}
			if (!this.isPlaying)
			{
				break;
			}
			BellShrineGateLock.BellLock bellLock = this.bellLocks[i];
			if (!pd.GetVariable(bellLock.PdBool))
			{
				allDone = false;
			}
			else
			{
				bellLock.ActivateJitter.StartTimedJitter();
				bellLock.LoopJitter.StartJitter();
				num = playedCount;
				playedCount = num + 1;
			}
			num = i;
		}
		if (playedCount >= this.bellLocks.Length)
		{
			EventRegister.SendEvent("NEEDOLIN LOCK", null);
		}
		if (!this.isPlaying || !allDone)
		{
			if (this.isPlaying)
			{
				yield return bellWait;
			}
			this.playRoutine = null;
			this.EndAll();
			yield break;
		}
		this.eventFsm.SendEvent("GATE LOCKS PRE COMPLETE");
		yield return new WaitForSeconds(this.endDelay);
		this.EndAll();
		this.eventFsm.SendEvent("GATE LOCKS COMPLETE");
		yield break;
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x000B7C3C File Offset: 0x000B5E3C
	private void EndAll()
	{
		BellShrineGateLock.BellLock[] array = this.bellLocks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LoopJitter.StopJitterWithDecay();
		}
	}

	// Token: 0x04002AF5 RID: 10997
	[SerializeField]
	private BellShrineGateLock.BellLock[] bellLocks;

	// Token: 0x04002AF6 RID: 10998
	[SerializeField]
	private float startDelay;

	// Token: 0x04002AF7 RID: 10999
	[SerializeField]
	private float bellDelay;

	// Token: 0x04002AF8 RID: 11000
	[SerializeField]
	private float endDelay;

	// Token: 0x04002AF9 RID: 11001
	[SerializeField]
	private PlayMakerFSM eventFsm;

	// Token: 0x04002AFA RID: 11002
	private bool isPlaying;

	// Token: 0x04002AFB RID: 11003
	private Coroutine playRoutine;

	// Token: 0x0200179D RID: 6045
	[Serializable]
	private class BellLock
	{
		// Token: 0x04008EAD RID: 36525
		public GameObject Root;

		// Token: 0x04008EAE RID: 36526
		public JitterSelfForTime ActivateJitter;

		// Token: 0x04008EAF RID: 36527
		public JitterSelf LoopJitter;

		// Token: 0x04008EB0 RID: 36528
		[PlayerDataField(typeof(bool), true)]
		public string PdBool;
	}
}
