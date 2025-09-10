using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000608 RID: 1544
public class AreaTitleController : MonoBehaviour
{
	// Token: 0x0600371A RID: 14106 RVA: 0x000F314B File Offset: 0x000F134B
	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(this.area.Identifier))
		{
			this.orderedAreas = new AreaTitleController.Area[]
			{
				this.area
			};
			this.area = default(AreaTitleController.Area);
		}
	}

	// Token: 0x0600371B RID: 14107 RVA: 0x000F3184 File Offset: 0x000F1384
	private void Awake()
	{
		if (!string.IsNullOrEmpty(this.waitForEvent))
		{
			EventRegister register = EventRegister.GetRegisterGuaranteed(base.gameObject, this.waitForEvent);
			Action temp = null;
			temp = delegate()
			{
				this.waitingForEvent = false;
				this.FindAreaTitle();
				this.DoPlay();
				register.ReceivedEvent -= temp;
			};
			register.ReceivedEvent += temp;
			this.waitingForEvent = true;
		}
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x000F31F4 File Offset: 0x000F13F4
	private void Start()
	{
		if (!this.triggerEntered && !string.IsNullOrEmpty(this.waitForEvent))
		{
			return;
		}
		this.hc = HeroController.instance;
		if (!this.hc.isHeroInPosition)
		{
			this.heroInPositionResponder = delegate(bool <p0>)
			{
				this.waitingForHero = false;
				if (base.isActiveAndEnabled)
				{
					this.FindAreaTitle();
					if (this.triggerEntered)
					{
						this.Play();
					}
					else
					{
						this.DoPlay();
					}
				}
				this.hc.heroInPositionDelayed -= this.heroInPositionResponder;
				this.heroInPositionResponder = null;
			};
			this.hc.heroInPositionDelayed += this.heroInPositionResponder;
			this.waitingForHero = true;
		}
		else if (!this.waitingForEvent)
		{
			this.FindAreaTitle();
			if (this.triggerEntered)
			{
				this.Play();
			}
			else
			{
				this.DoPlay();
			}
		}
		else if (this.waitForTrigger && this.triggerEntered)
		{
			this.Play();
		}
		this.started = true;
	}

	// Token: 0x0600371D RID: 14109 RVA: 0x000F329E File Offset: 0x000F149E
	private void OnDisable()
	{
		this.triggerEntered = false;
	}

	// Token: 0x0600371E RID: 14110 RVA: 0x000F32A7 File Offset: 0x000F14A7
	private void FindAreaTitle()
	{
		if (ManagerSingleton<AreaTitle>.Instance)
		{
			this.areaTitle = ManagerSingleton<AreaTitle>.Instance.gameObject;
		}
	}

	// Token: 0x0600371F RID: 14111 RVA: 0x000F32C5 File Offset: 0x000F14C5
	private void DoPlay()
	{
		if (this.waitForTrigger || this.waitingForEvent || this.waitingForHero)
		{
			return;
		}
		this.Play();
	}

	// Token: 0x06003720 RID: 14112 RVA: 0x000F32E6 File Offset: 0x000F14E6
	protected void OnDestroy()
	{
		if (this.hc != null && this.heroInPositionResponder != null)
		{
			this.hc.heroInPositionDelayed -= this.heroInPositionResponder;
			this.hc = null;
			this.heroInPositionResponder = null;
		}
	}

	// Token: 0x06003721 RID: 14113 RVA: 0x000F331D File Offset: 0x000F151D
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!base.isActiveAndEnabled || this.played)
		{
			return;
		}
		if (collision.CompareTag("Player"))
		{
			this.triggerEntered = true;
			if (this.waitingForHero || !this.started)
			{
				return;
			}
			this.Play();
		}
	}

	// Token: 0x06003722 RID: 14114 RVA: 0x000F335C File Offset: 0x000F155C
	public void Play()
	{
		if (this.played)
		{
			return;
		}
		this.played = true;
		this.currentAreaData = this.GetAreaTitle();
		if (!string.IsNullOrEmpty(this.doorTrigger))
		{
			if (HeroController.instance.GetEntryGateName() == this.doorTrigger)
			{
				this.CheckArea();
				return;
			}
			base.gameObject.SetActive(false);
			return;
		}
		else
		{
			if (string.IsNullOrEmpty(this.doorException))
			{
				this.CheckArea();
				return;
			}
			if (HeroController.instance.GetEntryGateName() != this.doorException)
			{
				this.CheckArea();
				return;
			}
			base.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x06003723 RID: 14115 RVA: 0x000F33FC File Offset: 0x000F15FC
	private AreaTitleController.Area GetAreaTitle()
	{
		if (this.orderedAreas != null)
		{
			for (int i = 0; i < this.orderedAreas.Length; i++)
			{
				AreaTitleController.Area area = this.orderedAreas[i];
				if (area.Test.IsFulfilled)
				{
					return area;
				}
			}
		}
		return default(AreaTitleController.Area);
	}

	// Token: 0x06003724 RID: 14116 RVA: 0x000F3449 File Offset: 0x000F1649
	private void CheckArea()
	{
		this.Finish();
	}

	// Token: 0x06003725 RID: 14117 RVA: 0x000F3454 File Offset: 0x000F1654
	private void Finish()
	{
		GameManager instance = GameManager.instance;
		if (string.IsNullOrEmpty(this.currentAreaData.Identifier))
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (instance != null && instance.IsFirstLevelForPlayer)
		{
			PlayerData playerData2 = GameManager.instance.playerData;
			if (!string.IsNullOrEmpty(this.currentAreaData.VisitedBool) && playerData2.GetBool(this.currentAreaData.VisitedBool))
			{
				if (!string.IsNullOrEmpty(this.currentAreaData.Identifier))
				{
					playerData2.currentArea = this.currentAreaData.Identifier;
				}
				base.gameObject.SetActive(false);
				return;
			}
			if (!this.ignoreFirstSceneCheckIfUnvisited)
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		if (this.currentAreaData.IsSubArea)
		{
			base.StartCoroutine(this.VisitPause(null));
			return;
		}
		PlayerData playerData = GameManager.instance.playerData;
		string currentArea = playerData.currentArea;
		bool flag = !string.IsNullOrEmpty(this.currentAreaData.VisitedBool) && playerData.GetBool(this.currentAreaData.VisitedBool);
		if (this.onlyIfUnvisited && flag)
		{
			return;
		}
		if ((!flag && this.onlyOnRevisit) || this.currentAreaData.Identifier == currentArea)
		{
			if (this.recordVisitedOnSkip && !string.IsNullOrEmpty(this.currentAreaData.VisitedBool))
			{
				GameManager.instance.playerData.SetBool(this.currentAreaData.VisitedBool, true);
				playerData.currentArea = this.currentAreaData.Identifier;
			}
			base.gameObject.SetActive(false);
			return;
		}
		Action afterDelay = delegate()
		{
			playerData.currentArea = this.currentAreaData.Identifier;
		};
		base.StartCoroutine((flag || this.currentAreaData.AlwaysSmallTitle) ? this.VisitPause(afterDelay) : this.UnvisitPause(afterDelay));
	}

	// Token: 0x06003726 RID: 14118 RVA: 0x000F363C File Offset: 0x000F183C
	public void ForcePlay()
	{
		if (this.areaTitle)
		{
			this.areaTitle.SetActive(true);
			PlayMakerFSM fsm = FSMUtility.GetFSM(this.areaTitle);
			if (fsm)
			{
				FSMUtility.SetBool(fsm, "Visited", true);
				FSMUtility.SetBool(fsm, "NPC Title", false);
				FSMUtility.SetBool(fsm, "City Title", this.IsCityTitle());
				FSMUtility.SetBool(fsm, "Display Right", this.displayRight);
				FSMUtility.SetString(fsm, "Area Event", this.currentAreaData.Identifier);
			}
		}
	}

	// Token: 0x06003727 RID: 14119 RVA: 0x000F36C8 File Offset: 0x000F18C8
	public void ForcePlayLarge()
	{
		if (this.areaTitle)
		{
			this.areaTitle.SetActive(true);
			PlayMakerFSM fsm = FSMUtility.GetFSM(this.areaTitle);
			if (fsm)
			{
				FSMUtility.SetBool(fsm, "Visited", false);
				FSMUtility.SetBool(fsm, "NPC Title", false);
				FSMUtility.SetBool(fsm, "City Title", this.IsCityTitle());
				FSMUtility.SetBool(fsm, "Display Right", this.displayRight);
				FSMUtility.SetString(fsm, "Area Event", this.currentAreaData.Identifier);
			}
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003728 RID: 14120 RVA: 0x000F375E File Offset: 0x000F195E
	private IEnumerator VisitPause(Action afterDelay)
	{
		yield return new WaitForSeconds(this.visitedPause);
		if (afterDelay != null)
		{
			afterDelay();
		}
		if (this.areaTitle)
		{
			if (this.areaTitle.gameObject.activeInHierarchy)
			{
				while (InteractManager.BlockingInteractable)
				{
					yield return null;
				}
				float timeOut = 5f;
				while (this.areaTitle.gameObject.activeInHierarchy && timeOut > 0f)
				{
					timeOut -= Time.deltaTime;
					yield return null;
				}
				this.areaTitle.gameObject.SetActive(false);
			}
			this.areaTitle.SetActive(true);
			PlayMakerFSM fsm = FSMUtility.GetFSM(this.areaTitle);
			if (fsm)
			{
				FSMUtility.SetBool(fsm, "Visited", true);
				FSMUtility.SetBool(fsm, "NPC Title", false);
				FSMUtility.SetBool(fsm, "City Title", this.IsCityTitle());
				FSMUtility.SetBool(fsm, "Display Right", this.displayRight);
				FSMUtility.SetString(fsm, "Area Event", this.currentAreaData.Identifier);
			}
			if (!string.IsNullOrEmpty(this.currentAreaData.VisitedBool))
			{
				GameManager.instance.playerData.SetBool(this.currentAreaData.VisitedBool, true);
			}
		}
		yield break;
	}

	// Token: 0x06003729 RID: 14121 RVA: 0x000F3774 File Offset: 0x000F1974
	private IEnumerator UnvisitPause(Action afterDelay)
	{
		if (InteractManager.BlockingInteractable || this.alwaysBlockInteractIfUnvisited)
		{
			InteractManager.IsDisabled = true;
		}
		yield return new WaitForSeconds(this.unvisitedPause);
		if (afterDelay != null)
		{
			afterDelay();
		}
		if (!this.areaTitle)
		{
			yield break;
		}
		InteractManager.IsDisabled = true;
		while (InteractManager.BlockingInteractable)
		{
			while (InteractManager.BlockingInteractable)
			{
				yield return null;
			}
			float timeOut = 5f;
			while (this.areaTitle.gameObject.activeInHierarchy && timeOut > 0f)
			{
				timeOut -= Time.deltaTime;
				yield return null;
			}
		}
		this.areaTitle.SetActive(false);
		this.areaTitle.SetActive(true);
		PlayMakerFSM fsm = FSMUtility.GetFSM(this.areaTitle);
		if (!fsm)
		{
			yield break;
		}
		FSMUtility.SetBool(fsm, "Visited", false);
		FSMUtility.SetBool(fsm, "NPC Title", false);
		FSMUtility.SetBool(fsm, "City Title", this.IsCityTitle());
		FSMUtility.SetString(fsm, "Area Event", this.currentAreaData.Identifier);
		if (!string.IsNullOrEmpty(this.currentAreaData.VisitedBool))
		{
			GameManager.instance.playerData.SetBool(this.currentAreaData.VisitedBool, true);
		}
		yield break;
	}

	// Token: 0x0600372A RID: 14122 RVA: 0x000F378C File Offset: 0x000F198C
	private bool IsCityTitle()
	{
		MapZone currentMapZoneEnum = GameManager.instance.GetCurrentMapZoneEnum();
		return currentMapZoneEnum == MapZone.CITY_OF_SONG || currentMapZoneEnum - MapZone.UNDERSTORE <= 1 || currentMapZoneEnum - MapZone.WARD <= 1;
	}

	// Token: 0x040039EA RID: 14826
	[SerializeField]
	private string waitForEvent;

	// Token: 0x040039EB RID: 14827
	[Space]
	[SerializeField]
	private AreaTitleController.Area[] orderedAreas;

	// Token: 0x040039EC RID: 14828
	[SerializeField]
	[HideInInspector]
	private AreaTitleController.Area area;

	// Token: 0x040039ED RID: 14829
	[SerializeField]
	private bool displayRight;

	// Token: 0x040039EE RID: 14830
	[SerializeField]
	private string doorTrigger = "";

	// Token: 0x040039EF RID: 14831
	[SerializeField]
	private string doorException = "";

	// Token: 0x040039F0 RID: 14832
	[SerializeField]
	private bool onlyOnRevisit;

	// Token: 0x040039F1 RID: 14833
	[SerializeField]
	private bool recordVisitedOnSkip;

	// Token: 0x040039F2 RID: 14834
	[SerializeField]
	private float unvisitedPause = 2f;

	// Token: 0x040039F3 RID: 14835
	[SerializeField]
	private float visitedPause = 2f;

	// Token: 0x040039F4 RID: 14836
	[SerializeField]
	private bool waitForTrigger;

	// Token: 0x040039F5 RID: 14837
	[SerializeField]
	private bool ignoreFirstSceneCheckIfUnvisited;

	// Token: 0x040039F6 RID: 14838
	[SerializeField]
	private bool alwaysBlockInteractIfUnvisited;

	// Token: 0x040039F7 RID: 14839
	[SerializeField]
	private bool triggerEnterWaitForHeroInPosition;

	// Token: 0x040039F8 RID: 14840
	[SerializeField]
	private bool onlyIfUnvisited;

	// Token: 0x040039F9 RID: 14841
	private GameObject areaTitle;

	// Token: 0x040039FA RID: 14842
	private bool played;

	// Token: 0x040039FB RID: 14843
	private bool waitingForEvent;

	// Token: 0x040039FC RID: 14844
	private bool waitingForHero;

	// Token: 0x040039FD RID: 14845
	private AreaTitleController.Area currentAreaData;

	// Token: 0x040039FE RID: 14846
	private HeroController hc;

	// Token: 0x040039FF RID: 14847
	private HeroController.HeroInPosition heroInPositionResponder;

	// Token: 0x04003A00 RID: 14848
	private bool started;

	// Token: 0x04003A01 RID: 14849
	private bool triggerEntered;

	// Token: 0x02001911 RID: 6417
	[Serializable]
	private struct Area
	{
		// Token: 0x04009438 RID: 37944
		public PlayerDataTest Test;

		// Token: 0x04009439 RID: 37945
		public string Identifier;

		// Token: 0x0400943A RID: 37946
		public bool IsSubArea;

		// Token: 0x0400943B RID: 37947
		[ModifiableProperty]
		[Conditional("IsSubArea", false, false, false)]
		public bool AlwaysSmallTitle;

		// Token: 0x0400943C RID: 37948
		[PlayerDataField(typeof(bool), false)]
		public string VisitedBool;
	}
}
