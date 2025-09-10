using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000727 RID: 1831
public class SilkSpool : MonoBehaviour
{
	// Token: 0x17000775 RID: 1909
	// (get) Token: 0x06004158 RID: 16728 RVA: 0x0011F007 File Offset: 0x0011D207
	// (set) Token: 0x06004159 RID: 16729 RVA: 0x0011F00E File Offset: 0x0011D20E
	public static SilkSpool Instance { get; private set; }

	// Token: 0x17000776 RID: 1910
	// (get) Token: 0x0600415A RID: 16730 RVA: 0x0011F016 File Offset: 0x0011D216
	public static float BindCost
	{
		get
		{
			if (PlayerData.instance.IsAnyCursed)
			{
				return float.MaxValue;
			}
			bool isEquipped = Gameplay.WitchCrest.IsEquipped;
			return 9f;
		}
	}

	// Token: 0x0600415B RID: 16731 RVA: 0x0011F03C File Offset: 0x0011D23C
	private void Awake()
	{
		if (SilkSpool.Instance)
		{
			return;
		}
		SilkSpool.Instance = this;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += delegate()
		{
			this.RefreshSilk();
			ToolItem mossCreep1Tool = Gameplay.MossCreep1Tool;
			if (!mossCreep1Tool || !mossCreep1Tool.IsEquipped)
			{
				this.CancelMossChunk();
			}
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "BIND FAILED NOT ENOUGH").ReceivedEvent += delegate()
		{
			if (this.silkFailedAnimator)
			{
				this.silkFailedAnimator.SetTrigger(this.animFailedTrigger);
			}
			this.silkFailedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, false, 1f, null);
		};
	}

	// Token: 0x0600415C RID: 16732 RVA: 0x0011F09E File Offset: 0x0011D29E
	private void OnDestroy()
	{
		if (SilkSpool.Instance == this)
		{
			SilkSpool.Instance = null;
		}
	}

	// Token: 0x0600415D RID: 16733 RVA: 0x0011F0B3 File Offset: 0x0011D2B3
	public void Start()
	{
		if (this.spoolParent)
		{
			this.spoolParent.SetActive(false);
		}
	}

	// Token: 0x0600415E RID: 16734 RVA: 0x0011F0CE File Offset: 0x0011D2CE
	public void DrawSpool()
	{
		this.DrawSpool(0);
	}

	// Token: 0x0600415F RID: 16735 RVA: 0x0011F0D8 File Offset: 0x0011D2D8
	public void DrawSpool(int maxOffset)
	{
		PlayerData instance = PlayerData.instance;
		int num = instance.CurrentSilkMaxBasic + maxOffset;
		int num2 = Mathf.Max(num, 9);
		float x = this.firstChunk_x + this.chunkDistance_x * (float)(num2 - 1) + 0.15f;
		Vector3 localPosition = this.capR.localPosition;
		Transform transform = this.capR;
		localPosition = new Vector3(x, localPosition.y, localPosition.z);
		transform.localPosition = localPosition;
		this.capRAnchored.localPosition = localPosition;
		Vector3 localScale = this.seg1.localScale;
		this.seg1.localScale = new Vector3(x, localScale.y, localScale.z);
		bool flag = instance.IsSilkSpoolBroken && !instance.UnlockSilkFinalCutscene;
		bool isAnyCursed = instance.IsAnyCursed;
		if (this.activeParent)
		{
			bool flag2 = !flag && !isAnyCursed;
			int activeInHierarchy = this.activeParent.activeInHierarchy ? 1 : 0;
			this.activeParent.SetActive(flag2);
			if (activeInHierarchy == 0 && flag2 && !Gameplay.SpoolExtenderTool.Status.IsEquipped)
			{
				EventRegister.SendEvent("SPOOL CAP FIX", null);
			}
		}
		if (this.brokenParent)
		{
			this.brokenParent.SetActive(flag && !isAnyCursed);
		}
		if (this.cursedParent)
		{
			bool activeInHierarchy2 = this.cursedParent.activeInHierarchy;
			this.cursedParent.SetActive(isAnyCursed);
			if (isAnyCursed && !activeInHierarchy2)
			{
				this.cursedAnimator.Play("Spool Cursed Appear");
				this.cursedAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			}
		}
		if (this.spoolParent && !this.spoolParent.activeSelf)
		{
			this.spoolParent.SetActive(true);
		}
		this.RefreshBindNotch(num);
		this.hasDrawnSpool = true;
		this.RefreshSilk(this.queuedAddSource.GetValueOrDefault(), this.queuedTakeSource.GetValueOrDefault());
		this.queuedAddSource = null;
		this.queuedTakeSource = null;
		if (this.act3EndingParent.activeSelf)
		{
			float num3 = (float)num2 / 9f;
			this.act3EndingBarScaler.SetScaleX(num3);
			float newXScale = 1f / num3;
			Transform[] array = this.act3EndingBarInverseScalers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetScaleX(newXScale);
			}
		}
	}

	// Token: 0x06004160 RID: 16736 RVA: 0x0011F334 File Offset: 0x0011D534
	public void ChangeSilk(int silk, int silkParts, SilkSpool.SilkAddSource addSource, SilkSpool.SilkTakeSource takeSource)
	{
		PlayerData instance = PlayerData.instance;
		if ((float)silk >= SilkSpool.BindCost)
		{
			for (int i = 0; i < this.silkChunks.Count; i++)
			{
				SilkChunk silkChunk = this.silkChunks[i];
				if ((float)i < SilkSpool.BindCost)
				{
					if (silkChunk.IsRegen)
					{
						silkChunk.Add(true);
					}
					else
					{
						silkChunk.StartGlow();
					}
				}
				else if (silkChunk.IsRegen)
				{
					silkChunk.Add(false);
				}
				else
				{
					silkChunk.EndGlow();
				}
			}
			this.bindOrb.enabled = true;
			this.bindOrb.SendEvent("BIND ACTIVE");
		}
		else if ((float)silk < SilkSpool.BindCost)
		{
			for (int j = 0; j < this.silkChunks.Count; j++)
			{
				if (j + 1 <= silk)
				{
					SilkChunk silkChunk2 = this.silkChunks[j];
					if (silkChunk2.IsRegen)
					{
						silkChunk2.Add(false);
					}
					else
					{
						silkChunk2.EndGlow();
					}
				}
			}
			this.bindOrb.enabled = true;
			this.bindOrb.SendEvent("BIND INACTIVE");
		}
		if (silk > this.silkChunks.Count)
		{
			for (int k = silk - this.silkChunks.Count; k > 0; k--)
			{
				if (this.mossChunks != null && k <= this.mossChunks.Count && addSource == SilkSpool.SilkAddSource.Moss)
				{
					int index = k - 1;
					SilkChunk silkChunk3 = this.mossChunks[index];
					silkChunk3.Add(this.IsGlowing(silk));
					silkChunk3.FinishMossState();
					this.silkChunks.Add(silkChunk3);
					this.mossChunks.RemoveAt(index);
				}
				else if (k == 1 && addSource == SilkSpool.SilkAddSource.Normal && silkParts == 0 && this.partsChunk != null)
				{
					bool flag = this.IsGlowing(silk);
					this.partsChunk.Add(flag);
					if (!flag)
					{
						this.partsChunk.FinishPartsState();
					}
					this.silkChunks.Add(this.partsChunk);
					this.partsChunk = null;
				}
				else
				{
					SilkChunk silkChunk4 = this.SpawnNewChunk(true);
					if (silkChunk4)
					{
						silkChunk4.Add(this.IsGlowing(silk));
					}
				}
			}
		}
		if (silk < this.silkChunks.Count)
		{
			int l = this.silkChunks.Count - silk;
			int num = 1;
			if (l > 0)
			{
				List<SilkChunk> list = this.silkChunks;
				int num2 = num;
				if (list[list.Count - num2].IsRegen)
				{
					l--;
					num++;
				}
			}
			while (l > 0)
			{
				List<SilkChunk> list2 = this.silkChunks;
				int num2 = num;
				list2[list2.Count - num2].Remove(takeSource);
				this.silkChunks.RemoveAt(this.silkChunks.Count - num);
				l--;
			}
		}
		if (silkParts > 0)
		{
			if (this.partsChunk == null)
			{
				this.partsChunk = this.SpawnNewChunk(false);
			}
			this.partsChunk.SetPartsState(silkParts - 1);
		}
		else if (this.partsChunk)
		{
			this.partsChunk.Remove(SilkSpool.SilkTakeSource.Parts);
			this.partsChunk = null;
			this.EvaluatePositions();
		}
		if (addSource == SilkSpool.SilkAddSource.Moss)
		{
			this.CancelMossChunk();
		}
		this.EvaluatePositions();
		if (instance.UnlockSilkFinalCutscene && !this.wasInSilkFinalCutscene && this.silkFinalCutsceneBurst)
		{
			this.silkFinalCutsceneBurst.SetActive(false);
			this.silkFinalCutsceneBurst.SetActive(true);
		}
		this.wasInSilkFinalCutscene = instance.UnlockSilkFinalCutscene;
	}

	// Token: 0x06004161 RID: 16737 RVA: 0x0011F67D File Offset: 0x0011D87D
	private bool IsGlowing(int silk)
	{
		return (float)silk >= SilkSpool.BindCost && (float)this.silkChunks.Count <= SilkSpool.BindCost;
	}

	// Token: 0x06004162 RID: 16738 RVA: 0x0011F6A0 File Offset: 0x0011D8A0
	public void RefreshSilk()
	{
		this.RefreshSilk(SilkSpool.SilkAddSource.Normal, SilkSpool.SilkTakeSource.Normal);
		this.RefreshBindNotch(PlayerData.instance.CurrentSilkMaxBasic);
	}

	// Token: 0x06004163 RID: 16739 RVA: 0x0011F6BC File Offset: 0x0011D8BC
	public void RefreshSilk(SilkSpool.SilkAddSource addSource, SilkSpool.SilkTakeSource takeSource)
	{
		EventRegister.SendEvent("SILK REFRESHED", null);
		if (!this.hasDrawnSpool)
		{
			this.queuedAddSource = new SilkSpool.SilkAddSource?(addSource);
			this.queuedTakeSource = new SilkSpool.SilkTakeSource?(takeSource);
			return;
		}
		if (this.wasUsingChunk)
		{
			this.wasUsingChunk.PlayIdle();
			this.wasUsingChunk = null;
		}
		PlayerData instance = PlayerData.instance;
		this.ChangeSilk(instance.silk, instance.silkParts, addSource, takeSource);
		if (this.silkChunks.Count == 0)
		{
			return;
		}
		SilkSpool.SilkUsingFlags silkUsingFlags = SilkSpool.SilkUsingFlags.None;
		foreach (SilkSpool.SilkUsingFlags silkUsingFlags2 in this.usingSilk)
		{
			silkUsingFlags |= silkUsingFlags2;
		}
		if (silkUsingFlags == SilkSpool.SilkUsingFlags.None)
		{
			return;
		}
		for (int i = this.silkChunks.Count - 1; i >= 0; i--)
		{
			SilkChunk silkChunk = this.silkChunks[i];
			if (!silkChunk.IsRegen)
			{
				silkChunk.SetUsing(silkUsingFlags);
				this.wasUsingChunk = silkChunk;
				return;
			}
		}
	}

	// Token: 0x06004164 RID: 16740 RVA: 0x0011F7CC File Offset: 0x0011D9CC
	private void RefreshBindNotch(int currentSilkMax)
	{
		if (!this.bindNotch)
		{
			return;
		}
		float bindCost = SilkSpool.BindCost;
		if ((float)currentSilkMax > bindCost)
		{
			this.bindNotch.SetActive(true);
			this.bindNotch.transform.SetLocalPositionX(this.firstChunk_x + this.chunkDistance_x * bindCost);
			return;
		}
		this.bindNotch.SetActive(false);
	}

	// Token: 0x06004165 RID: 16741 RVA: 0x0011F82C File Offset: 0x0011DA2C
	private SilkChunk SpawnNewChunk(bool addToList)
	{
		if (!this.silkChunkPrefab)
		{
			return null;
		}
		SilkChunk component = this.silkChunkPrefab.Spawn(this.chunkParent).GetComponent<SilkChunk>();
		if (addToList)
		{
			this.silkChunks.Add(component);
		}
		return component;
	}

	// Token: 0x06004166 RID: 16742 RVA: 0x0011F870 File Offset: 0x0011DA70
	public bool AddUsing(SilkSpool.SilkUsingFlags usingFlags, int amount = 1)
	{
		for (int i = 0; i < amount; i++)
		{
			this.usingSilk.Add(usingFlags);
		}
		if ((usingFlags & SilkSpool.SilkUsingFlags.Curse) == SilkSpool.SilkUsingFlags.Curse)
		{
			this.cursedAnimator.Play("Spool Cursed Bobbing");
		}
		this.RefreshSilk();
		return true;
	}

	// Token: 0x06004167 RID: 16743 RVA: 0x0011F8B4 File Offset: 0x0011DAB4
	public bool RemoveUsing(SilkSpool.SilkUsingFlags usingFlags, int amount = 1)
	{
		bool result = false;
		for (int i = 0; i < amount; i++)
		{
			if (this.usingSilk.Remove(usingFlags))
			{
				result = true;
			}
		}
		if ((usingFlags & SilkSpool.SilkUsingFlags.Curse) == SilkSpool.SilkUsingFlags.Curse)
		{
			this.cursedAnimator.Play("Spool Cursed");
		}
		this.RefreshSilk();
		return result;
	}

	// Token: 0x06004168 RID: 16744 RVA: 0x0011F900 File Offset: 0x0011DB00
	public void SetRegen(int amount, bool isUpgraded)
	{
		for (int i = 0; i < this.silkChunks.Count; i++)
		{
			SilkChunk silkChunk = this.silkChunks[i];
			if (silkChunk.IsRegen)
			{
				silkChunk.EndedRegen();
				silkChunk.gameObject.Recycle();
				this.silkChunks.RemoveAt(i);
			}
		}
		for (int j = 0; j < amount; j++)
		{
			SilkChunk silkChunk2 = this.SpawnNewChunk(true);
			if (silkChunk2)
			{
				silkChunk2.SetRegen(isUpgraded);
			}
		}
		this.EvaluatePositions();
	}

	// Token: 0x06004169 RID: 16745 RVA: 0x0011F980 File Offset: 0x0011DB80
	public static void ResumeSilkAudio()
	{
		SilkSpool instance = SilkSpool.Instance;
		if (instance == null)
		{
			return;
		}
		foreach (SilkChunk silkChunk in instance.silkChunks)
		{
			silkChunk.ResumeRegenAudioLoop();
		}
	}

	// Token: 0x0600416A RID: 16746 RVA: 0x0011F9E0 File Offset: 0x0011DBE0
	public static void EndSilkAudio()
	{
		SilkSpool instance = SilkSpool.Instance;
		if (instance == null)
		{
			return;
		}
		foreach (SilkChunk silkChunk in instance.silkChunks)
		{
			silkChunk.StopRegenAudioLoop();
		}
	}

	// Token: 0x0600416B RID: 16747 RVA: 0x0011FA40 File Offset: 0x0011DC40
	private void EvaluatePositions()
	{
		float num = this.firstChunk_x;
		for (int i = 0; i < this.silkChunks.Count; i++)
		{
			SilkChunk silkChunk = this.silkChunks[i];
			silkChunk.transform.localPosition = new Vector3(num, 0f, -0.002f - (float)i * 1E-05f);
			if (!silkChunk.IsRegen)
			{
				num += this.chunkDistance_x;
			}
		}
		if (this.partsChunk)
		{
			this.partsChunk.transform.localPosition = new Vector3(num, 0f, -0.0015f);
			num += this.chunkDistance_x;
		}
		if (this.mossChunks != null)
		{
			foreach (SilkChunk silkChunk2 in this.mossChunks)
			{
				silkChunk2.transform.localPosition = new Vector3(num, 0f, -0.001f);
				num += this.chunkDistance_x;
			}
		}
	}

	// Token: 0x0600416C RID: 16748 RVA: 0x0011FB4C File Offset: 0x0011DD4C
	public void SetMossState(int state, int count = 1)
	{
		int num = state - 1;
		if (num < 0)
		{
			this.CancelMossChunk();
			return;
		}
		if (this.mossChunks == null)
		{
			this.mossChunks = new List<SilkChunk>(count);
		}
		while (this.mossChunks.Count < count)
		{
			SilkChunk item = this.SpawnNewChunk(false);
			this.mossChunks.Add(item);
			this.EvaluatePositions();
		}
		foreach (SilkChunk silkChunk in this.mossChunks)
		{
			silkChunk.SetMossState(num);
		}
	}

	// Token: 0x0600416D RID: 16749 RVA: 0x0011FBEC File Offset: 0x0011DDEC
	private void CancelMossChunk()
	{
		if (this.mossChunks == null)
		{
			return;
		}
		foreach (SilkChunk silkChunk in this.mossChunks)
		{
			silkChunk.gameObject.Recycle();
		}
		this.mossChunks.Clear();
		this.EvaluatePositions();
	}

	// Token: 0x040042CC RID: 17100
	public const string SILK_REFRESHED_EVENT = "SILK REFRESHED";

	// Token: 0x040042CE RID: 17102
	public float firstChunk_x = 0.27f;

	// Token: 0x040042CF RID: 17103
	public float chunkDistance_x = 0.18f;

	// Token: 0x040042D0 RID: 17104
	[SerializeField]
	private Transform chunkParent;

	// Token: 0x040042D1 RID: 17105
	[SerializeField]
	private Transform capR;

	// Token: 0x040042D2 RID: 17106
	[SerializeField]
	private Transform capRAnchored;

	// Token: 0x040042D3 RID: 17107
	[SerializeField]
	private Transform seg1;

	// Token: 0x040042D4 RID: 17108
	[SerializeField]
	private GameObject bindNotch;

	// Token: 0x040042D5 RID: 17109
	[SerializeField]
	private GameObject silkChunkPrefab;

	// Token: 0x040042D6 RID: 17110
	[SerializeField]
	private PlayMakerFSM bindOrb;

	// Token: 0x040042D7 RID: 17111
	[Space]
	[SerializeField]
	private Animator silkFailedAnimator;

	// Token: 0x040042D8 RID: 17112
	[SerializeField]
	private RandomAudioClipTable silkFailedAudioTable;

	// Token: 0x040042D9 RID: 17113
	[Space]
	[SerializeField]
	private GameObject spoolParent;

	// Token: 0x040042DA RID: 17114
	[SerializeField]
	private GameObject activeParent;

	// Token: 0x040042DB RID: 17115
	[SerializeField]
	private GameObject brokenParent;

	// Token: 0x040042DC RID: 17116
	[SerializeField]
	private GameObject cursedParent;

	// Token: 0x040042DD RID: 17117
	[SerializeField]
	private tk2dSpriteAnimator cursedAnimator;

	// Token: 0x040042DE RID: 17118
	[SerializeField]
	private AudioEvent cursedAudio;

	// Token: 0x040042DF RID: 17119
	[Space]
	[SerializeField]
	private GameObject silkFinalCutsceneBurst;

	// Token: 0x040042E0 RID: 17120
	[Space]
	[SerializeField]
	private GameObject act3EndingParent;

	// Token: 0x040042E1 RID: 17121
	[SerializeField]
	private Transform act3EndingBarScaler;

	// Token: 0x040042E2 RID: 17122
	[SerializeField]
	private Transform[] act3EndingBarInverseScalers;

	// Token: 0x040042E3 RID: 17123
	private readonly List<SilkChunk> silkChunks = new List<SilkChunk>();

	// Token: 0x040042E4 RID: 17124
	private bool hasDrawnSpool;

	// Token: 0x040042E5 RID: 17125
	private SilkSpool.SilkAddSource? queuedAddSource;

	// Token: 0x040042E6 RID: 17126
	private SilkSpool.SilkTakeSource? queuedTakeSource;

	// Token: 0x040042E7 RID: 17127
	private bool wasInSilkFinalCutscene;

	// Token: 0x040042E8 RID: 17128
	private readonly List<SilkSpool.SilkUsingFlags> usingSilk = new List<SilkSpool.SilkUsingFlags>();

	// Token: 0x040042E9 RID: 17129
	private SilkChunk wasUsingChunk;

	// Token: 0x040042EA RID: 17130
	private List<SilkChunk> mossChunks;

	// Token: 0x040042EB RID: 17131
	private SilkChunk partsChunk;

	// Token: 0x040042EC RID: 17132
	private readonly int animFailedTrigger = Animator.StringToHash("Failed");

	// Token: 0x040042ED RID: 17133
	private readonly int appearAnimId = Animator.StringToHash("Appear");

	// Token: 0x040042EE RID: 17134
	private readonly int disappearAnimId = Animator.StringToHash("Disappear");

	// Token: 0x02001A0D RID: 6669
	public enum SilkAddSource
	{
		// Token: 0x0400983C RID: 38972
		Normal,
		// Token: 0x0400983D RID: 38973
		Moss
	}

	// Token: 0x02001A0E RID: 6670
	public enum SilkTakeSource
	{
		// Token: 0x0400983F RID: 38975
		Normal,
		// Token: 0x04009840 RID: 38976
		Wisp,
		// Token: 0x04009841 RID: 38977
		Curse,
		// Token: 0x04009842 RID: 38978
		Drain,
		// Token: 0x04009843 RID: 38979
		ActiveUse,
		// Token: 0x04009844 RID: 38980
		Parts
	}

	// Token: 0x02001A0F RID: 6671
	[Flags]
	public enum SilkUsingFlags
	{
		// Token: 0x04009846 RID: 38982
		None = 0,
		// Token: 0x04009847 RID: 38983
		Normal = 1,
		// Token: 0x04009848 RID: 38984
		Acid = 2,
		// Token: 0x04009849 RID: 38985
		Maggot = 4,
		// Token: 0x0400984A RID: 38986
		Void = 8,
		// Token: 0x0400984B RID: 38987
		Curse = 16,
		// Token: 0x0400984C RID: 38988
		Drain = 32
	}
}
