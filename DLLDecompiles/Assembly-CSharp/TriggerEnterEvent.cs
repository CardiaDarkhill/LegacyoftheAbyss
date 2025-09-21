using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005FF RID: 1535
public class TriggerEnterEvent : EventBase
{
	// Token: 0x140000AD RID: 173
	// (add) Token: 0x060036D1 RID: 14033 RVA: 0x000F1E18 File Offset: 0x000F0018
	// (remove) Token: 0x060036D2 RID: 14034 RVA: 0x000F1E50 File Offset: 0x000F0050
	public event TriggerEnterEvent.CollisionEvent OnTriggerEntered;

	// Token: 0x140000AE RID: 174
	// (add) Token: 0x060036D3 RID: 14035 RVA: 0x000F1E88 File Offset: 0x000F0088
	// (remove) Token: 0x060036D4 RID: 14036 RVA: 0x000F1EC0 File Offset: 0x000F00C0
	public event TriggerEnterEvent.CollisionEvent OnTriggerExited;

	// Token: 0x140000AF RID: 175
	// (add) Token: 0x060036D5 RID: 14037 RVA: 0x000F1EF8 File Offset: 0x000F00F8
	// (remove) Token: 0x060036D6 RID: 14038 RVA: 0x000F1F30 File Offset: 0x000F0130
	private event TriggerEnterEvent.CollisionEvent _OnTriggerStayed;

	// Token: 0x140000B0 RID: 176
	// (add) Token: 0x060036D7 RID: 14039 RVA: 0x000F1F65 File Offset: 0x000F0165
	// (remove) Token: 0x060036D8 RID: 14040 RVA: 0x000F1F92 File Offset: 0x000F0192
	public event TriggerEnterEvent.CollisionEvent OnTriggerStayed
	{
		add
		{
			if (this._OnTriggerStayed == null)
			{
				base.gameObject.AddComponentIfNotPresent<TriggerStaySubEvent>().OnTriggerStayed += this.CallOnTriggerStay2D;
			}
			this._OnTriggerStayed += value;
		}
		remove
		{
			this._OnTriggerStayed -= value;
		}
	}

	// Token: 0x17000650 RID: 1616
	// (get) Token: 0x060036D9 RID: 14041 RVA: 0x000F1F9B File Offset: 0x000F019B
	public override string InspectorInfo
	{
		get
		{
			return "Trigger Enter";
		}
	}

	// Token: 0x17000651 RID: 1617
	// (get) Token: 0x060036DA RID: 14042 RVA: 0x000F1FA2 File Offset: 0x000F01A2
	// (set) Token: 0x060036DB RID: 14043 RVA: 0x000F1FAA File Offset: 0x000F01AA
	public bool DelayUpdateGrounded
	{
		get
		{
			return this.delayUpdateGrounded;
		}
		set
		{
			this.delayUpdateGrounded = value;
		}
	}

	// Token: 0x17000652 RID: 1618
	// (get) Token: 0x060036DC RID: 14044 RVA: 0x000F1FB3 File Offset: 0x000F01B3
	private bool ShouldDelay
	{
		get
		{
			return (this.delayUpdateGrounded && !this.hc.cState.onGround) || this.gm.IsInSceneTransition;
		}
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x000F1FDC File Offset: 0x000F01DC
	protected override void Awake()
	{
		base.Awake();
		this.gm = GameManager.instance;
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x000F1FF0 File Offset: 0x000F01F0
	protected virtual void Start()
	{
		this.active = false;
		this.hc = HeroController.instance;
		if (!this.waitForHeroInPosition)
		{
			this.HeroInPosition();
			return;
		}
		if (this.hc.isHeroInPosition)
		{
			this.HeroInPosition();
			return;
		}
		this.hc.heroInPosition += this.OnHeroInPosition;
		this.subscribedHc = this.hc;
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x000F2055 File Offset: 0x000F0255
	private void OnDisable()
	{
		if (this.delayRoutine != null)
		{
			base.StopCoroutine(this.delayRoutine);
			this.delayRoutine = null;
		}
		List<Collider2D> list = this.enteredWhileInactive;
		if (list != null)
		{
			list.Clear();
		}
		List<Collider2D> list2 = this.exitedWhileDelayed;
		if (list2 == null)
		{
			return;
		}
		list2.Clear();
	}

	// Token: 0x060036E0 RID: 14048 RVA: 0x000F2093 File Offset: 0x000F0293
	private void OnDestroy()
	{
		this.UnSubHeroInPosition();
	}

	// Token: 0x060036E1 RID: 14049 RVA: 0x000F209B File Offset: 0x000F029B
	private void OnHeroInPosition(bool forceDirect)
	{
		this.HeroInPosition();
	}

	// Token: 0x060036E2 RID: 14050 RVA: 0x000F20A3 File Offset: 0x000F02A3
	private void UnSubHeroInPosition()
	{
		if (!this.subscribedHc)
		{
			return;
		}
		this.subscribedHc.heroInPosition -= this.OnHeroInPosition;
		this.subscribedHc = null;
	}

	// Token: 0x060036E3 RID: 14051 RVA: 0x000F20D1 File Offset: 0x000F02D1
	private void HeroInPosition()
	{
		this.UnSubHeroInPosition();
		if (this.readPersistent)
		{
			this.readPersistent.OnSetSaveState += delegate(bool value)
			{
				if (value)
				{
					this.Activate();
				}
			};
			this.RefreshActivate();
			return;
		}
		this.Activate();
	}

	// Token: 0x060036E4 RID: 14052 RVA: 0x000F210A File Offset: 0x000F030A
	public void RefreshActivate()
	{
		if (this.readPersistent.GetCurrentValue())
		{
			this.Activate();
		}
	}

	// Token: 0x060036E5 RID: 14053 RVA: 0x000F2120 File Offset: 0x000F0320
	private void Activate()
	{
		if (this.active)
		{
			return;
		}
		this.active = true;
		if (this.enteredWhileInactive == null || this.enteredWhileInactive.Count == 0)
		{
			return;
		}
		if (this.ShouldDelay)
		{
			return;
		}
		TriggerEnterEvent.ProcessList(this.enteredWhileInactive, new Action<Collider2D>(this.OnTriggerEnter2D));
		if (this.isDelayingProcessList)
		{
			this.isDelayingProcessList = false;
			TriggerEnterEvent.ProcessList(this.exitedWhileDelayed, new Action<Collider2D>(this.OnTriggerExit2D));
		}
	}

	// Token: 0x060036E6 RID: 14054 RVA: 0x000F219C File Offset: 0x000F039C
	private static void ProcessList(List<Collider2D> list, Action<Collider2D> action)
	{
		if (list == null)
		{
			return;
		}
		TriggerEnterEvent.processedColliders.Clear();
		while (list.Count > 0)
		{
			int index = list.Count - 1;
			Collider2D collider2D = list[index];
			list.RemoveAt(index);
			if (collider2D && TriggerEnterEvent.processedColliders.Add(collider2D))
			{
				action(collider2D);
			}
		}
		TriggerEnterEvent.processedColliders.Clear();
	}

	// Token: 0x060036E7 RID: 14055 RVA: 0x000F2200 File Offset: 0x000F0400
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Particle"))
		{
			return;
		}
		if (this.checkLayer != 0 && (this.checkLayer & 1 << other.gameObject.layer) == 0)
		{
			return;
		}
		string[] array = this.checkTags;
		if (array != null && array.Length > 0)
		{
			foreach (string tag in this.checkTags)
			{
				if (!other.CompareTag(tag))
				{
					return;
				}
			}
		}
		if (this.active && !this.ShouldDelay)
		{
			TriggerEnterEvent.CollisionEvent onTriggerEntered = this.OnTriggerEntered;
			if (onTriggerEntered != null)
			{
				onTriggerEntered(other, base.gameObject);
			}
			if (this.fsmTarget)
			{
				if (this.enableFsmOnSend && !this.fsmTarget.enabled)
				{
					this.fsmTarget.enabled = true;
				}
				this.fsmTarget.SendEvent(this.fsmEvent);
			}
			if (!this.callEventOnExit)
			{
				base.CallReceivedEvent();
			}
			return;
		}
		List<Collider2D> list = this.exitedWhileDelayed;
		if (list != null && list.Remove(other))
		{
			return;
		}
		if (this.enteredWhileInactive == null)
		{
			this.enteredWhileInactive = new List<Collider2D>();
		}
		this.enteredWhileInactive.Add(other);
		if (this.ShouldDelay && this.delayRoutine == null)
		{
			this.delayRoutine = base.StartCoroutine(this.WaitForNotDelayed());
		}
	}

	// Token: 0x060036E8 RID: 14056 RVA: 0x000F2348 File Offset: 0x000F0548
	private void OnTriggerExit2D(Collider2D other)
	{
		if (this.isDelayingProcessList)
		{
			List<Collider2D> list = this.enteredWhileInactive;
			if (list != null && list.Remove(other))
			{
				return;
			}
		}
		if (this.active && !this.ShouldDelay)
		{
			TriggerEnterEvent.CollisionEvent onTriggerExited = this.OnTriggerExited;
			if (onTriggerExited != null)
			{
				onTriggerExited(other, base.gameObject);
			}
			if (this.callEventOnExit)
			{
				base.CallReceivedEvent();
			}
			return;
		}
		List<Collider2D> list2 = this.enteredWhileInactive;
		if ((list2 != null && list2.Remove(other)) || !this.ShouldDelay)
		{
			return;
		}
		if (this.exitedWhileDelayed == null)
		{
			this.exitedWhileDelayed = new List<Collider2D>();
		}
		this.exitedWhileDelayed.Add(other);
		if (base.gameObject.activeInHierarchy && this.delayRoutine == null)
		{
			this.delayRoutine = base.StartCoroutine(this.WaitForNotDelayed());
		}
	}

	// Token: 0x060036E9 RID: 14057 RVA: 0x000F240E File Offset: 0x000F060E
	public void CallOnTriggerStay2D(Collider2D other)
	{
		if (!this.active)
		{
			return;
		}
		TriggerEnterEvent.CollisionEvent onTriggerStayed = this._OnTriggerStayed;
		if (onTriggerStayed == null)
		{
			return;
		}
		onTriggerStayed(other, base.gameObject);
	}

	// Token: 0x060036EA RID: 14058 RVA: 0x000F2430 File Offset: 0x000F0630
	private IEnumerator WaitForNotDelayed()
	{
		this.isDelayingProcessList = true;
		while (this.ShouldDelay)
		{
			yield return null;
		}
		if (!this.active)
		{
			if (this.readPersistent != null && !this.readPersistent.GetCurrentValue())
			{
				yield break;
			}
			while (!this.active)
			{
				yield return null;
			}
		}
		if (this.isDelayingProcessList)
		{
			this.isDelayingProcessList = false;
			TriggerEnterEvent.ProcessList(this.enteredWhileInactive, new Action<Collider2D>(this.OnTriggerEnter2D));
			TriggerEnterEvent.ProcessList(this.exitedWhileDelayed, new Action<Collider2D>(this.OnTriggerExit2D));
		}
		this.delayRoutine = null;
		yield break;
	}

	// Token: 0x060036EB RID: 14059 RVA: 0x000F243F File Offset: 0x000F063F
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x040039A4 RID: 14756
	[SerializeField]
	private PersistentBoolItem readPersistent;

	// Token: 0x040039A5 RID: 14757
	[SerializeField]
	private bool waitForHeroInPosition = true;

	// Token: 0x040039A6 RID: 14758
	[SerializeField]
	private bool callEventOnExit;

	// Token: 0x040039A7 RID: 14759
	[SerializeField]
	private bool delayUpdateGrounded;

	// Token: 0x040039A8 RID: 14760
	[Space]
	[SerializeField]
	private LayerMask checkLayer;

	// Token: 0x040039A9 RID: 14761
	[SerializeField]
	private string[] checkTags;

	// Token: 0x040039AA RID: 14762
	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	// Token: 0x040039AB RID: 14763
	[SerializeField]
	private string fsmEvent;

	// Token: 0x040039AC RID: 14764
	[SerializeField]
	private bool enableFsmOnSend;

	// Token: 0x040039AD RID: 14765
	private GameManager gm;

	// Token: 0x040039AE RID: 14766
	private HeroController hc;

	// Token: 0x040039AF RID: 14767
	private HeroController subscribedHc;

	// Token: 0x040039B0 RID: 14768
	private bool active;

	// Token: 0x040039B1 RID: 14769
	private bool isDelayingProcessList;

	// Token: 0x040039B2 RID: 14770
	private List<Collider2D> enteredWhileInactive;

	// Token: 0x040039B3 RID: 14771
	private List<Collider2D> exitedWhileDelayed;

	// Token: 0x040039B4 RID: 14772
	private Coroutine delayRoutine;

	// Token: 0x040039B5 RID: 14773
	private static readonly HashSet<Collider2D> processedColliders = new HashSet<Collider2D>();

	// Token: 0x0200190A RID: 6410
	// (Invoke) Token: 0x060092F2 RID: 37618
	public delegate void CollisionEvent(Collider2D collider, GameObject sender);
}
