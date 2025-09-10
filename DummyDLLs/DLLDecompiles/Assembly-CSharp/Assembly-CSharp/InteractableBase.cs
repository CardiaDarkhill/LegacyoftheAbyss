using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003CD RID: 973
public abstract class InteractableBase : DebugDrawColliderRuntimeAdder
{
	// Token: 0x17000366 RID: 870
	// (get) Token: 0x06002110 RID: 8464 RVA: 0x000995CB File Offset: 0x000977CB
	// (set) Token: 0x06002111 RID: 8465 RVA: 0x000995D3 File Offset: 0x000977D3
	public Transform PromptMarker
	{
		get
		{
			return this.promptMarker;
		}
		set
		{
			this.promptMarker = value;
		}
	}

	// Token: 0x17000367 RID: 871
	// (get) Token: 0x06002112 RID: 8466 RVA: 0x000995DC File Offset: 0x000977DC
	// (set) Token: 0x06002113 RID: 8467 RVA: 0x000995E4 File Offset: 0x000977E4
	public InteractableBase.PromptLabels InteractLabel
	{
		get
		{
			return this.interactLabel;
		}
		set
		{
			InteractableBase.PromptLabels promptLabels = this.interactLabel;
			this.interactLabel = value;
			if (this.interactLabel == promptLabels || !Application.isPlaying)
			{
				return;
			}
			if (this.HideInteraction())
			{
				this.ShowInteraction();
			}
		}
	}

	// Token: 0x17000368 RID: 872
	// (get) Token: 0x06002114 RID: 8468 RVA: 0x0009961E File Offset: 0x0009781E
	public virtual string InteractLabelDisplay
	{
		get
		{
			this.UpgradeOldSerializedFields();
			return this.interactLabel.ToString();
		}
	}

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x06002115 RID: 8469 RVA: 0x00099637 File Offset: 0x00097837
	public InteractPriority Priority
	{
		get
		{
			return this.priority;
		}
	}

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06002116 RID: 8470 RVA: 0x0009963F File Offset: 0x0009783F
	// (set) Token: 0x06002117 RID: 8471 RVA: 0x00099647 File Offset: 0x00097847
	public bool IsDisabled { get; private set; }

	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06002118 RID: 8472 RVA: 0x00099650 File Offset: 0x00097850
	// (set) Token: 0x06002119 RID: 8473 RVA: 0x00099658 File Offset: 0x00097858
	public bool IsQueued { get; private set; }

	// Token: 0x1700036C RID: 876
	// (get) Token: 0x0600211A RID: 8474 RVA: 0x00099661 File Offset: 0x00097861
	protected virtual bool IsQueueingHandled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700036D RID: 877
	// (get) Token: 0x0600211B RID: 8475 RVA: 0x00099664 File Offset: 0x00097864
	protected virtual bool AutoQueueOnDeactivate
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700036E RID: 878
	// (get) Token: 0x0600211C RID: 8476 RVA: 0x00099667 File Offset: 0x00097867
	// (set) Token: 0x0600211D RID: 8477 RVA: 0x00099670 File Offset: 0x00097870
	public TriggerEnterEvent EnterDetector
	{
		get
		{
			return this.enterDetector;
		}
		set
		{
			if (this.enterDetector)
			{
				this.enterDetector.OnTriggerEntered -= this.DetectorTriggerEnter;
			}
			if (this.ExitDetector == null || this.enterDetector == this.exitDetector)
			{
				this.ExitDetector = value;
			}
			this.enterDetector = value;
			if (this.enterDetector)
			{
				this.enterDetector.OnTriggerEntered += this.DetectorTriggerEnter;
				return;
			}
			if (!this.HasDefinedDetector() && this.previousInsideCount == 0 && this.localInsideColliders.Count != 0)
			{
				this.previousInsideCount = 1;
				if (base.enabled)
				{
					this.ShowInteraction();
				}
				this.previousInsideCount = 0;
			}
		}
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x0600211E RID: 8478 RVA: 0x0009972E File Offset: 0x0009792E
	// (set) Token: 0x0600211F RID: 8479 RVA: 0x00099738 File Offset: 0x00097938
	public TriggerEnterEvent ExitDetector
	{
		get
		{
			return this.exitDetector;
		}
		set
		{
			if (this.exitDetector)
			{
				this.exitDetector.OnTriggerExited -= this.DetectorTriggerExit;
			}
			this.exitDetector = value;
			if (this.exitDetector)
			{
				this.exitDetector.OnTriggerExited += this.DetectorTriggerExit;
			}
		}
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x00099794 File Offset: 0x00097994
	protected virtual bool EnableInteractableFields()
	{
		return true;
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x00099797 File Offset: 0x00097997
	private bool DisplayExitDetectorField()
	{
		return this.EnableInteractableFields() && this.HasDefinedDetector();
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x000997A9 File Offset: 0x000979A9
	private bool HasDefinedDetector()
	{
		return this.enterDetector;
	}

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06002123 RID: 8483 RVA: 0x000997B8 File Offset: 0x000979B8
	public bool IsBlocked
	{
		get
		{
			using (List<IInteractableBlocker>.Enumerator enumerator = this.blockers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsBlocking)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x00099814 File Offset: 0x00097A14
	protected virtual void OnValidate()
	{
		this.UpgradeOldSerializedFields();
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x0009981C File Offset: 0x00097A1C
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
		this.ExitDetector = this.exitDetector;
		this.EnterDetector = this.enterDetector;
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x00099842 File Offset: 0x00097A42
	protected virtual void OnDisable()
	{
		if (!this.HasDefinedDetector())
		{
			this.insideColliders.Clear();
			this.previousInsideCount = 0;
		}
		InteractManager.RemoveInteractible(this);
	}

	// Token: 0x06002127 RID: 8487 RVA: 0x00099868 File Offset: 0x00097A68
	private void UpgradeOldSerializedFields()
	{
		if (!string.IsNullOrEmpty(this.promptLabel))
		{
			try
			{
				this.interactLabel = (InteractableBase.PromptLabels)Enum.Parse(typeof(InteractableBase.PromptLabels), this.promptLabel, true);
				this.promptLabel = null;
			}
			catch
			{
				Debug.LogError("Could not upgrade serialized string promptLabel (" + this.promptLabel + ") to PromptLabels enum");
			}
		}
	}

	// Token: 0x06002128 RID: 8488 RVA: 0x000998DC File Offset: 0x00097ADC
	protected virtual void OnTriggerEnter2D(Collider2D collision)
	{
		this.LocalAddInside(collision);
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000998E5 File Offset: 0x00097AE5
	protected virtual void OnTriggerExit2D(Collider2D collision)
	{
		this.LocalRemoveInside(collision);
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000998EE File Offset: 0x00097AEE
	private void DetectorTriggerEnter(Collider2D collision, GameObject sender)
	{
		this.AddInside(collision);
	}

	// Token: 0x0600212B RID: 8491 RVA: 0x000998F7 File Offset: 0x00097AF7
	private void DetectorTriggerExit(Collider2D collision, GameObject sender)
	{
		this.RemoveInside(collision);
	}

	// Token: 0x0600212C RID: 8492 RVA: 0x00099900 File Offset: 0x00097B00
	private void AddInside(Collider2D col)
	{
		if (col.gameObject.layer != 9)
		{
			return;
		}
		this.insideColliders.Add(col);
		this.previousInsideCount = this.insideColliders.Count;
		if (base.enabled)
		{
			this.ShowInteraction();
		}
	}

	// Token: 0x0600212D RID: 8493 RVA: 0x00099940 File Offset: 0x00097B40
	private void RemoveInside(Collider2D col)
	{
		this.insideColliders.Remove(col);
		int count = this.insideColliders.Count;
		if (count <= 0 && this.previousInsideCount > 0)
		{
			this.HideInteraction();
		}
		this.previousInsideCount = count;
	}

	// Token: 0x0600212E RID: 8494 RVA: 0x00099984 File Offset: 0x00097B84
	private void LocalAddInside(Collider2D col)
	{
		if (col.gameObject.layer != 9)
		{
			return;
		}
		if (this.localInsideColliders.Add(col) && !this.HasDefinedDetector())
		{
			int num = this.previousInsideCount;
			this.previousInsideCount = this.localInsideColliders.Count;
			if (base.enabled)
			{
				this.ShowInteraction();
			}
			this.previousInsideCount = num;
		}
	}

	// Token: 0x0600212F RID: 8495 RVA: 0x000999E4 File Offset: 0x00097BE4
	private void LocalRemoveInside(Collider2D col)
	{
		if (this.localInsideColliders.Remove(col) && !this.HasDefinedDetector() && this.localInsideColliders.Count <= 0)
		{
			this.HideInteraction();
		}
	}

	// Token: 0x06002130 RID: 8496 RVA: 0x00099A14 File Offset: 0x00097C14
	private void ShowInteraction()
	{
		if (this.IsDisabled || this.isShowingInteraction)
		{
			return;
		}
		if (this.previousInsideCount <= 0 && (this.HasDefinedDetector() || this.localInsideColliders.Count <= 0))
		{
			return;
		}
		this.isShowingInteraction = true;
		InteractManager.AddInteractible(this, this.promptMarker ? this.promptMarker : base.transform, Vector3.zero);
	}

	// Token: 0x06002131 RID: 8497 RVA: 0x00099A7F File Offset: 0x00097C7F
	private bool HideInteraction()
	{
		this.isShowingInteraction = false;
		return InteractManager.RemoveInteractible(this);
	}

	// Token: 0x06002132 RID: 8498 RVA: 0x00099A8E File Offset: 0x00097C8E
	protected void DisableInteraction()
	{
		InteractManager.BlockingInteractable = this;
		HeroController.instance.RelinquishControl();
		this.HideInteraction();
		this.checkControlVersion = false;
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x00099AAE File Offset: 0x00097CAE
	protected void RecordControlVersion()
	{
		this.checkControlVersion = true;
		this.expectedControlVersion = HeroController.ControlVersion;
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x00099AC4 File Offset: 0x00097CC4
	protected void EnableInteraction()
	{
		if (InteractManager.BlockingInteractable == this)
		{
			InteractManager.BlockingInteractable = null;
			if (!this.checkControlVersion || this.expectedControlVersion == HeroController.ControlVersion)
			{
				HeroController.instance.RegainControl(false);
			}
		}
		this.checkControlVersion = false;
		this.ShowInteraction();
	}

	// Token: 0x06002135 RID: 8501 RVA: 0x00099B14 File Offset: 0x00097D14
	public void Activate()
	{
		if (this.delayRoutine != null)
		{
			base.StopCoroutine(this.delayRoutine);
			this.delayRoutine = null;
		}
		this.IsDisabled = false;
		this.isQueueing = false;
		this.ShowInteraction();
		FSMUtility.SetBoolOnGameObjectFSMs(base.gameObject, "Was Interactable Active", true);
		FSMUtility.SendEventToGameObject(base.gameObject, "INTERACTIBLE ENABLED", true);
		if (this.IsQueued)
		{
			this.IsQueued = false;
			if (!this.IsQueueingHandled)
			{
				this.Interact();
			}
		}
		this.OnActivated();
	}

	// Token: 0x06002136 RID: 8502 RVA: 0x00099B95 File Offset: 0x00097D95
	protected virtual void OnActivated()
	{
	}

	// Token: 0x06002137 RID: 8503 RVA: 0x00099B97 File Offset: 0x00097D97
	public void ActivateDelayed(float delay)
	{
		if (this.delayRoutine != null)
		{
			base.StopCoroutine(this.delayRoutine);
			this.delayRoutine = null;
		}
		this.delayRoutine = this.ExecuteDelayed(delay, new Action(this.Activate));
	}

	// Token: 0x06002138 RID: 8504 RVA: 0x00099BD0 File Offset: 0x00097DD0
	public void Deactivate(bool allowQueueing)
	{
		if (this.delayRoutine != null)
		{
			base.StopCoroutine(this.delayRoutine);
			this.delayRoutine = null;
		}
		this.isQueueing = allowQueueing;
		if (!allowQueueing)
		{
			this.IsDisabled = true;
			this.HideInteraction();
		}
		else
		{
			this.IsDisabled = false;
			if (this.isQueueing && !this.IsQueued && this.AutoQueueOnDeactivate)
			{
				this.IsQueued = true;
			}
		}
		FSMUtility.SetBoolOnGameObjectFSMs(base.gameObject, "Was Interactable Active", false);
	}

	// Token: 0x06002139 RID: 8505 RVA: 0x00099C49 File Offset: 0x00097E49
	public void QueueInteraction()
	{
		if (this.isQueueing)
		{
			this.IsQueued = true;
		}
		if (this.isQueueing && !this.IsQueueingHandled)
		{
			this.HideInteraction();
			return;
		}
		this.Interact();
	}

	// Token: 0x0600213A RID: 8506
	public abstract void Interact();

	// Token: 0x0600213B RID: 8507 RVA: 0x00099C78 File Offset: 0x00097E78
	public virtual void CanInteract()
	{
	}

	// Token: 0x0600213C RID: 8508 RVA: 0x00099C7A File Offset: 0x00097E7A
	public virtual void CanNotInteract()
	{
	}

	// Token: 0x0600213D RID: 8509 RVA: 0x00099C7C File Offset: 0x00097E7C
	protected static bool TrySendStateChangeEvent(PlayMakerFSM fsm, string eventName, bool logError = true)
	{
		if (!string.IsNullOrEmpty(eventName) && fsm)
		{
			string activeStateName = fsm.ActiveStateName;
			fsm.SendEvent(eventName);
			return !fsm.ActiveStateName.Equals(activeStateName);
		}
		return false;
	}

	// Token: 0x0600213E RID: 8510 RVA: 0x00099CBA File Offset: 0x00097EBA
	public void AddBlocker(IInteractableBlocker blocker)
	{
		this.blockers.AddIfNotPresent(blocker);
	}

	// Token: 0x0600213F RID: 8511 RVA: 0x00099CC9 File Offset: 0x00097EC9
	public void RemoveBlocker(IInteractableBlocker blocker)
	{
		this.blockers.Remove(blocker);
	}

	// Token: 0x06002140 RID: 8512 RVA: 0x00099CD8 File Offset: 0x00097ED8
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x0400201D RID: 8221
	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private Transform promptMarker;

	// Token: 0x0400201E RID: 8222
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string promptLabel = "Speak";

	// Token: 0x0400201F RID: 8223
	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private InteractableBase.PromptLabels interactLabel;

	// Token: 0x04002020 RID: 8224
	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private InteractPriority priority;

	// Token: 0x04002021 RID: 8225
	[SerializeField]
	[ModifiableProperty]
	[Conditional("EnableInteractableFields", true, true, false)]
	private TriggerEnterEvent enterDetector;

	// Token: 0x04002022 RID: 8226
	[SerializeField]
	[ModifiableProperty]
	[Conditional("DisplayExitDetectorField", true, true, false)]
	private TriggerEnterEvent exitDetector;

	// Token: 0x04002023 RID: 8227
	private readonly HashSet<Collider2D> insideColliders = new HashSet<Collider2D>();

	// Token: 0x04002024 RID: 8228
	private readonly HashSet<Collider2D> localInsideColliders = new HashSet<Collider2D>();

	// Token: 0x04002025 RID: 8229
	private int previousInsideCount;

	// Token: 0x04002026 RID: 8230
	private bool isShowingInteraction;

	// Token: 0x04002027 RID: 8231
	private bool isQueueing;

	// Token: 0x04002028 RID: 8232
	private Coroutine delayRoutine;

	// Token: 0x04002029 RID: 8233
	private readonly List<IInteractableBlocker> blockers = new List<IInteractableBlocker>();

	// Token: 0x0400202C RID: 8236
	private bool checkControlVersion;

	// Token: 0x0400202D RID: 8237
	private int expectedControlVersion;

	// Token: 0x02001684 RID: 5764
	public enum PromptLabels
	{
		// Token: 0x04008B14 RID: 35604
		Inspect,
		// Token: 0x04008B15 RID: 35605
		Speak,
		// Token: 0x04008B16 RID: 35606
		Listen,
		// Token: 0x04008B17 RID: 35607
		Enter,
		// Token: 0x04008B18 RID: 35608
		Ascend,
		// Token: 0x04008B19 RID: 35609
		Rest,
		// Token: 0x04008B1A RID: 35610
		Shop,
		// Token: 0x04008B1B RID: 35611
		Travel,
		// Token: 0x04008B1C RID: 35612
		Challenge,
		// Token: 0x04008B1D RID: 35613
		Exit,
		// Token: 0x04008B1E RID: 35614
		Descend,
		// Token: 0x04008B1F RID: 35615
		Sit,
		// Token: 0x04008B20 RID: 35616
		Trade,
		// Token: 0x04008B21 RID: 35617
		Accept,
		// Token: 0x04008B22 RID: 35618
		Watch,
		// Token: 0x04008B23 RID: 35619
		Ascend_GG,
		// Token: 0x04008B24 RID: 35620
		Consume,
		// Token: 0x04008B25 RID: 35621
		Track,
		// Token: 0x04008B26 RID: 35622
		TurnIn,
		// Token: 0x04008B27 RID: 35623
		Attack,
		// Token: 0x04008B28 RID: 35624
		Give,
		// Token: 0x04008B29 RID: 35625
		Take,
		// Token: 0x04008B2A RID: 35626
		Claim,
		// Token: 0x04008B2B RID: 35627
		Call,
		// Token: 0x04008B2C RID: 35628
		Play,
		// Token: 0x04008B2D RID: 35629
		Dive,
		// Token: 0x04008B2E RID: 35630
		Take_Living
	}
}
