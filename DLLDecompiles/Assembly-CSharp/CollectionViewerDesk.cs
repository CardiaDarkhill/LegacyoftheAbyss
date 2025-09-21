using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class CollectionViewerDesk : NPCControlBase
{
	// Token: 0x170001FA RID: 506
	// (get) Token: 0x06001251 RID: 4689 RVA: 0x00055306 File Offset: 0x00053506
	public IReadOnlyList<CollectionViewerDesk.Section> Sections
	{
		get
		{
			return this.sections;
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x06001252 RID: 4690 RVA: 0x0005530E File Offset: 0x0005350E
	public bool HasGramaphone
	{
		get
		{
			return this.gramaphone && this.gramaphone.gameObject.activeInHierarchy;
		}
	}

	// Token: 0x170001FC RID: 508
	// (get) Token: 0x06001253 RID: 4691 RVA: 0x0005532F File Offset: 0x0005352F
	public IEnumerable<Transform> MementosChildren
	{
		get
		{
			if (this.mementosParent)
			{
				foreach (object obj in this.mementosParent)
				{
					Transform transform = (Transform)obj;
					if (!(transform.gameObject == this.heartMementosGroup))
					{
						yield return transform;
					}
				}
				IEnumerator enumerator = null;
			}
			if (this.heartMementosParent)
			{
				foreach (object obj2 in this.heartMementosParent)
				{
					Transform transform2 = (Transform)obj2;
					yield return transform2;
				}
				IEnumerator enumerator = null;
			}
			yield break;
			yield break;
		}
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x00055340 File Offset: 0x00053540
	protected override void Awake()
	{
		base.Awake();
		EventRegister.GetRegisterGuaranteed(base.gameObject, "ACTIVATE DISPLAYS").ReceivedEvent += this.ActivateDisplays;
		if (this.sitDownHornet)
		{
			this.sitDownHornet.gameObject.SetActive(false);
		}
		if (this.inertSeat)
		{
			this.inertSeat.SetActive(true);
		}
		if (this.mementoAppearEffect)
		{
			this.mementoAppearEffect.SetActive(false);
		}
		this.didStartLoad = true;
		this.DoForEachRelic(delegate(CollectableRelic relic)
		{
			relic.LoadClips();
		});
	}

	// Token: 0x06001255 RID: 4693 RVA: 0x000553F0 File Offset: 0x000535F0
	protected override void Start()
	{
		base.Start();
		this.ActivateDisplays();
		PlayerData instance = PlayerData.instance;
		foreach (Transform transform in this.MementosChildren)
		{
			transform.gameObject.SetActive(instance.MementosDeposited.GetData(transform.name).IsDeposited);
		}
		this.ActivateHeartsGroup();
	}

	// Token: 0x06001256 RID: 4694 RVA: 0x00055470 File Offset: 0x00053670
	private void OnDestroy()
	{
		if (!this.didStartLoad)
		{
			return;
		}
		this.didStartLoad = false;
		this.DoForEachRelic(delegate(CollectableRelic relic)
		{
			relic.FreeClips();
		});
	}

	// Token: 0x06001257 RID: 4695 RVA: 0x000554A8 File Offset: 0x000536A8
	private void DoForEachRelic(Action<CollectableRelic> action)
	{
		foreach (CollectionViewerDesk.Section section in this.Sections)
		{
			IEnumerable enumerable = section.List as IEnumerable;
			if (enumerable != null)
			{
				foreach (object obj in enumerable)
				{
					CollectableItemRelicType collectableItemRelicType = obj as CollectableItemRelicType;
					CollectableRelic collectableRelic;
					if (collectableItemRelicType == null)
					{
						collectableRelic = (obj as CollectableRelic);
						if (collectableRelic == null)
						{
							continue;
						}
					}
					else
					{
						using (IEnumerator<CollectableRelic> enumerator3 = collectableItemRelicType.Relics.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								CollectableRelic obj2 = enumerator3.Current;
								action(obj2);
							}
							continue;
						}
					}
					action(collectableRelic);
				}
			}
		}
	}

	// Token: 0x06001258 RID: 4696 RVA: 0x000555A4 File Offset: 0x000537A4
	protected override void OnStartDialogue()
	{
		CollectionViewerDesk.<>c__DisplayClass29_0 CS$<>8__locals1 = new CollectionViewerDesk.<>c__DisplayClass29_0();
		CS$<>8__locals1.<>4__this = this;
		base.DisableInteraction();
		CS$<>8__locals1.constructPrompt = default(LocalisedString);
		CS$<>8__locals1.constructIndex = -1;
		CS$<>8__locals1.takeItem = null;
		CS$<>8__locals1.<OnStartDialogue>g__FindNext|0();
	}

	// Token: 0x06001259 RID: 4697 RVA: 0x000555D7 File Offset: 0x000537D7
	protected override void OnEndDialogue()
	{
		base.EnableInteraction();
	}

	// Token: 0x0600125A RID: 4698 RVA: 0x000555E0 File Offset: 0x000537E0
	private void GetIsActive(bool canConstruct, out bool isAnyActive, out bool isAnyUnlockable, out bool isJustMementos)
	{
		isAnyActive = false;
		isAnyUnlockable = false;
		isJustMementos = false;
		foreach (CollectionViewerDesk.Section section in this.Sections)
		{
			if (!isAnyActive && section.IsActive && section.CheckIsListActive(true, null))
			{
				if (!isAnyActive && section.List == Gameplay.MementoList)
				{
					isJustMementos = true;
				}
				else if (isJustMementos)
				{
					isJustMementos = false;
				}
				isAnyActive = true;
			}
			if ((canConstruct || section.ConstructPrompt.IsEmpty) && !isAnyUnlockable && section.IsUnlockable)
			{
				isAnyUnlockable = true;
			}
		}
		PlayerData instance = PlayerData.instance;
		foreach (Transform transform in this.MementosChildren)
		{
			string name = transform.name;
			if (instance.Collectables.GetData(name).Amount > 0)
			{
				CollectableItem itemByName = CollectableItemManager.GetItemByName(name);
				if (itemByName == null || ((ICollectionViewerItem)itemByName).CanDeposit)
				{
					isAnyUnlockable = true;
					isJustMementos = true;
				}
			}
		}
	}

	// Token: 0x0600125B RID: 4699 RVA: 0x00055700 File Offset: 0x00053900
	private void OnBoardClosed()
	{
		this.board.BoardClosed -= this.OnBoardClosed;
		base.StartCoroutine(this.GetUpSequence());
	}

	// Token: 0x0600125C RID: 4700 RVA: 0x00055726 File Offset: 0x00053926
	public void PlayOnGramaphone(CollectableRelic playingRelic)
	{
		this.StopPlayingRelic();
		this.gramaphone.Play(playingRelic, false, null);
	}

	// Token: 0x0600125D RID: 4701 RVA: 0x0005573C File Offset: 0x0005393C
	public void StopPlayingRelic()
	{
		this.gramaphone.Stop();
	}

	// Token: 0x0600125E RID: 4702 RVA: 0x00055749 File Offset: 0x00053949
	public CollectableRelic GetPlayingRelic()
	{
		if (!this.HasGramaphone)
		{
			return null;
		}
		return this.gramaphone.PlayingRelic;
	}

	// Token: 0x0600125F RID: 4703 RVA: 0x00055760 File Offset: 0x00053960
	private IEnumerator SitDownSequence(int constructIndex)
	{
		if (this.sitDownHornet)
		{
			HeroController instance = HeroController.instance;
			instance.GetComponent<MeshRenderer>().enabled = false;
			if (this.inertSeat)
			{
				this.inertSeat.SetActive(false);
			}
			this.hornetMoveSfx.SpawnAndPlayOneShot(instance.transform.position, false);
			this.sitDownHornet.gameObject.SetActive(true);
			yield return new WaitForSeconds(this.sitDownHornet.PlayAnimGetTime("Hornet Desk Sit"));
		}
		bool flag = false;
		CollectionViewerDesk.Section constructingSection = null;
		for (int i = 0; i < this.sections.Count; i++)
		{
			CollectionViewerDesk.Section section = this.sections[i];
			if (section.IsUnlockable)
			{
				if (!section.ConstructPrompt.IsEmpty)
				{
					if (constructIndex != i)
					{
						goto IL_110;
					}
					constructingSection = section;
				}
				section.Unlock();
				flag = true;
			}
			IL_110:;
		}
		if (flag)
		{
			ScreenFaderUtils.Fade(ScreenFaderUtils.GetColour(), Color.black, this.screenFadeTime);
			yield return new WaitForSeconds(this.screenFadeTime);
			if (constructingSection != null && constructingSection.ConstructAudio.Clip)
			{
				constructingSection.ConstructAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero, null);
				yield return new WaitForSeconds(constructingSection.ConstructAudio.Clip.length);
			}
			this.ActivateDisplays();
			yield return new WaitForSeconds(this.fadeHoldTime);
			Color colour;
			Color startColour = colour = ScreenFaderUtils.GetColour();
			colour.a = 0f;
			ScreenFaderUtils.Fade(startColour, colour, this.screenFadeTime);
			yield return new WaitForSeconds(this.screenFadeTime);
		}
		this.board.BoardClosed += this.OnBoardClosed;
		this.board.OpenBoard(this);
		yield break;
	}

	// Token: 0x06001260 RID: 4704 RVA: 0x00055778 File Offset: 0x00053978
	private void ActivateDisplays()
	{
		foreach (CollectionViewerDesk.Section section in this.Sections)
		{
			section.DisplayObjects.SetAllActive(section.IsActive);
		}
		bool flag;
		bool flag2;
		bool flag3;
		this.GetIsActive(true, out flag, out flag2, out flag3);
		bool flag4 = flag || flag2;
		if (!flag4)
		{
			base.Deactivate(false);
		}
		if (this.activeWhileInteractable)
		{
			this.activeWhileInteractable.SetActive(flag4);
		}
		if (this.activeWhileNew)
		{
			this.activeWhileNew.SetActive(flag2 && !flag3);
		}
		if (flag3 && this.onlyMementosInspect)
		{
			base.PromptMarker = this.onlyMementosInspect.PromptMarker;
			base.HeroAnimation = this.onlyMementosInspect.HeroAnimation;
			base.TalkPosition = this.onlyMementosInspect.TalkPosition;
			base.CentreOffset = this.onlyMementosInspect.CentreOffset;
			base.TargetDistance = this.onlyMementosInspect.TargetDistance;
		}
	}

	// Token: 0x06001261 RID: 4705 RVA: 0x00055894 File Offset: 0x00053A94
	private void ActivateHeartsGroup()
	{
		if (!this.heartMementosParent)
		{
			return;
		}
		bool active = false;
		using (IEnumerator enumerator = this.heartMementosParent.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (((Transform)enumerator.Current).gameObject.activeSelf)
				{
					active = true;
				}
			}
		}
		if (this.heartMementosGroup)
		{
			this.heartMementosGroup.SetActive(active);
		}
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x0005591C File Offset: 0x00053B1C
	private IEnumerator GetUpSequence()
	{
		HeroController hc = HeroController.instance;
		HeroAnimationController heroAnim = hc.GetComponent<HeroAnimationController>();
		if (this.sitDownHornet)
		{
			this.hornetMoveSfx.SpawnAndPlayOneShot(hc.transform.position, false);
			yield return new WaitForSeconds(this.sitDownHornet.PlayAnimGetTime("Hornet Desk Stand"));
			this.sitDownHornet.gameObject.SetActive(false);
			if (this.inertSeat)
			{
				this.inertSeat.SetActive(true);
			}
			hc.GetComponent<MeshRenderer>().enabled = true;
			heroAnim.SetPlayRunToIdle();
		}
		else if (heroAnim.animator.IsPlaying("Abyss Kneel"))
		{
			this.hornetMoveSfx.SpawnAndPlayOneShot(hc.transform.position, false);
			yield return new WaitForSeconds(heroAnim.animator.PlayAnimGetTime(heroAnim.GetClip("Abyss Kneel to Stand")));
		}
		GameCameras.instance.HUDIn();
		base.EndDialogue();
		yield break;
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x0005592C File Offset: 0x00053B2C
	public void DoMementoDeposit(string mementoName)
	{
		foreach (Transform transform in this.MementosChildren)
		{
			if (!(transform.name != mementoName))
			{
				if (this.mementoAppearEffect)
				{
					this.mementoAppearEffect.SetActive(false);
					this.mementoAppearEffect.transform.SetPosition2D(transform.transform.position);
					this.mementoAppearEffect.SetActive(true);
				}
				transform.gameObject.SetActive(true);
				this.ActivateHeartsGroup();
			}
		}
	}

	// Token: 0x04001111 RID: 4369
	[Space]
	[SerializeField]
	private CollectionViewBoard board;

	// Token: 0x04001112 RID: 4370
	[Space]
	[SerializeField]
	private List<CollectionViewerDesk.Section> sections;

	// Token: 0x04001113 RID: 4371
	[Space]
	[SerializeField]
	private GameObject activeWhileInteractable;

	// Token: 0x04001114 RID: 4372
	[SerializeField]
	private GameObject activeWhileNew;

	// Token: 0x04001115 RID: 4373
	[SerializeField]
	private CollectionGramaphone gramaphone;

	// Token: 0x04001116 RID: 4374
	[Space]
	[SerializeField]
	private tk2dSpriteAnimator sitDownHornet;

	// Token: 0x04001117 RID: 4375
	[SerializeField]
	private RandomAudioClipTable hornetMoveSfx;

	// Token: 0x04001118 RID: 4376
	[SerializeField]
	private GameObject inertSeat;

	// Token: 0x04001119 RID: 4377
	[SerializeField]
	private float screenFadeTime;

	// Token: 0x0400111A RID: 4378
	[SerializeField]
	private float fadeHoldTime;

	// Token: 0x0400111B RID: 4379
	[SerializeField]
	private Transform mementosParent;

	// Token: 0x0400111C RID: 4380
	[SerializeField]
	private GameObject heartMementosGroup;

	// Token: 0x0400111D RID: 4381
	[SerializeField]
	private Transform heartMementosParent;

	// Token: 0x0400111E RID: 4382
	[SerializeField]
	private float mementoAppearDelay;

	// Token: 0x0400111F RID: 4383
	[SerializeField]
	private GameObject mementoAppearEffect;

	// Token: 0x04001120 RID: 4384
	[SerializeField]
	private float mementoAppearEndDelay;

	// Token: 0x04001121 RID: 4385
	[Space]
	[SerializeField]
	private NPCControlBase onlyMementosInspect;

	// Token: 0x04001122 RID: 4386
	private bool didStartLoad;

	// Token: 0x02001512 RID: 5394
	[Serializable]
	public class Section
	{
		// Token: 0x17000D53 RID: 3411
		// (get) Token: 0x060085A6 RID: 34214 RVA: 0x002707A5 File Offset: 0x0026E9A5
		public bool IsActive
		{
			get
			{
				return string.IsNullOrWhiteSpace(this.UnlockSaveBool) || PlayerData.instance.GetBool(this.UnlockSaveBool);
			}
		}

		// Token: 0x17000D54 RID: 3412
		// (get) Token: 0x060085A7 RID: 34215 RVA: 0x002707C8 File Offset: 0x0026E9C8
		public bool IsUnlockable
		{
			get
			{
				return !this.IsActive && this.UnlockTest.IsFulfilled && (!this.UnlockItem || this.UnlockItem.CollectedAmount > 0) && (!this.List || this.CheckIsListActive(true, null));
			}
		}

		// Token: 0x060085A8 RID: 34216 RVA: 0x00270825 File Offset: 0x0026EA25
		public void Unlock()
		{
			if (this.UnlockItem)
			{
				this.UnlockItem.Take(1, false);
			}
			PlayerData.instance.SetBool(this.UnlockSaveBool, true);
		}

		// Token: 0x060085A9 RID: 34217 RVA: 0x00270854 File Offset: 0x0026EA54
		public bool CheckIsListActive(bool checkVisible, Action<ICollectionViewerItem> addFunc)
		{
			IEnumerable enumerable = this.List as IEnumerable;
			if (enumerable == null)
			{
				return false;
			}
			if (this.tempDict == null)
			{
				this.tempDict = new Dictionary<object, object>();
			}
			foreach (object obj in enumerable)
			{
				CollectableItemMemento collectableItemMemento = obj as CollectableItemMemento;
				if (collectableItemMemento != null)
				{
					Object countKey = collectableItemMemento.CountKey;
					object obj2;
					if (this.tempDict.TryGetValue(countKey, out obj2))
					{
						CollectableItemMemento collectableItemMemento2 = obj2 as CollectableItemMemento;
						if (collectableItemMemento2 != null && collectableItemMemento2.IsListedInCollection())
						{
							continue;
						}
					}
					if (countKey)
					{
						this.tempDict[countKey] = obj;
					}
					else
					{
						this.tempDict[obj] = obj;
					}
				}
				else
				{
					this.tempDict[obj] = obj;
				}
			}
			bool result = false;
			foreach (KeyValuePair<object, object> keyValuePair in this.tempDict)
			{
				object obj3;
				object obj4;
				keyValuePair.Deconstruct(out obj3, out obj4);
				object obj5 = obj4;
				ICollectionViewerItemList collectionViewerItemList = obj5 as ICollectionViewerItemList;
				ICollectionViewerItem collectionViewerItem;
				if (collectionViewerItemList == null)
				{
					collectionViewerItem = (obj5 as ICollectionViewerItem);
					if (collectionViewerItem == null)
					{
						continue;
					}
				}
				else
				{
					using (IEnumerator<ICollectionViewerItem> enumerator3 = collectionViewerItemList.GetCollectionViewerItems().GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							ICollectionViewerItem collectionViewerItem2 = enumerator3.Current;
							if (!checkVisible || collectionViewerItem2.IsListedInCollection())
							{
								result = true;
								if (addFunc == null)
								{
									return true;
								}
								addFunc(collectionViewerItem2);
							}
						}
						continue;
					}
				}
				if (collectionViewerItem.IsListedInCollection() || (!checkVisible && collectionViewerItem.IsRequiredInCollection()))
				{
					result = true;
					if (addFunc == null)
					{
						return true;
					}
					addFunc(collectionViewerItem);
				}
			}
			return result;
		}

		// Token: 0x040085CB RID: 34251
		public NamedScriptableObjectListDummy List;

		// Token: 0x040085CC RID: 34252
		[ModifiableProperty]
		[Conditional("List", true, false, false)]
		public LocalisedString Heading;

		// Token: 0x040085CD RID: 34253
		public GameObject[] DisplayObjects;

		// Token: 0x040085CE RID: 34254
		[Space]
		public PlayerDataTest UnlockTest;

		// Token: 0x040085CF RID: 34255
		public CollectableItem UnlockItem;

		// Token: 0x040085D0 RID: 34256
		[PlayerDataField(typeof(bool), false)]
		public string UnlockSaveBool;

		// Token: 0x040085D1 RID: 34257
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString ConstructPrompt;

		// Token: 0x040085D2 RID: 34258
		public AudioEvent ConstructAudio = AudioEvent.Default;

		// Token: 0x040085D3 RID: 34259
		public string ConstructEventRegister;

		// Token: 0x040085D4 RID: 34260
		private Dictionary<object, object> tempDict;
	}
}
