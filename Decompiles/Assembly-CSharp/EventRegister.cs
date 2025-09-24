using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000332 RID: 818
public class EventRegister : EventBase
{
	// Token: 0x170002EA RID: 746
	// (get) Token: 0x06001C9F RID: 7327 RVA: 0x00085374 File Offset: 0x00083574
	public override string InspectorInfo
	{
		get
		{
			return this.subscribedEvent;
		}
	}

	// Token: 0x170002EB RID: 747
	// (get) Token: 0x06001CA0 RID: 7328 RVA: 0x0008537C File Offset: 0x0008357C
	// (set) Token: 0x06001CA1 RID: 7329 RVA: 0x00085384 File Offset: 0x00083584
	public string SubscribedEvent
	{
		get
		{
			return this.subscribedEvent;
		}
		set
		{
			this.SwitchEvent(value);
		}
	}

	// Token: 0x06001CA2 RID: 7330 RVA: 0x00085390 File Offset: 0x00083590
	private bool? CheckAlias(string eventName)
	{
		if (this.aliasEventMode == EventRegister.AliasEventMode.None && string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		return this.IsEventValid(eventName);
	}

	// Token: 0x06001CA3 RID: 7331 RVA: 0x000853C0 File Offset: 0x000835C0
	private bool? CheckSubscribedEvent(string eventName)
	{
		if (this.aliasEventMode != EventRegister.AliasEventMode.None)
		{
			return null;
		}
		return this.IsEventValid(eventName);
	}

	// Token: 0x06001CA4 RID: 7332 RVA: 0x000853E8 File Offset: 0x000835E8
	private bool? IsEventValid(string eventName)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return new bool?(false);
		}
		if (string.IsNullOrEmpty(this.setFsmBool))
		{
			return this.targetFsm.IsEventValid(eventName, true);
		}
		bool? flag = this.targetFsm.IsEventValid(eventName, true);
		bool flag2 = true;
		if (flag.GetValueOrDefault() == flag2 & flag != null)
		{
			return new bool?(true);
		}
		return null;
	}

	// Token: 0x06001CA5 RID: 7333 RVA: 0x00085454 File Offset: 0x00083654
	private bool? IsFsmBoolValid(string boolName)
	{
		if (!this.targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return new bool?(this.targetFsm.FsmVariables.FindFsmBool(boolName) != null);
	}

	// Token: 0x06001CA6 RID: 7334 RVA: 0x00085499 File Offset: 0x00083699
	private void Reset()
	{
		this.targetFsm = base.GetComponent<PlayMakerFSM>();
	}

	// Token: 0x06001CA7 RID: 7335 RVA: 0x000854A7 File Offset: 0x000836A7
	protected override void Awake()
	{
		base.Awake();
		this.didAwake = true;
		this.UpdateEventHash();
		EventRegister.SubscribeEvent(this);
	}

	// Token: 0x06001CA8 RID: 7336 RVA: 0x000854C2 File Offset: 0x000836C2
	private void OnDestroy()
	{
		EventRegister.UnsubscribeEvent(this);
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x000854CA File Offset: 0x000836CA
	private void UpdateEventHash()
	{
		this.subscribedEventHash = EventRegister.GetEventHashCode(this.subscribedEvent);
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x000854DD File Offset: 0x000836DD
	public static int GetEventHashCode(string eventName)
	{
		if (!string.IsNullOrWhiteSpace(eventName))
		{
			return eventName.GetHashCode();
		}
		return 0;
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x000854EF File Offset: 0x000836EF
	[ContextMenu("Test Receive", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x000854F8 File Offset: 0x000836F8
	[ContextMenu("Test Receive", false)]
	public void ReceiveEvent()
	{
		if (string.IsNullOrEmpty(this.subscribedEvent))
		{
			return;
		}
		string eventName = this.subscribedEvent;
		if (this.aliasEventMode == EventRegister.AliasEventMode.SendAlias && !string.IsNullOrEmpty(this.aliasEvent))
		{
			eventName = this.aliasEvent;
		}
		if (this.targetFsm)
		{
			bool enabled = this.targetFsm.enabled;
			if (!enabled && this.enableFsmBeforeSend)
			{
				this.targetFsm.enabled = true;
			}
			bool flag = this.targetFsm.SendEventRecursive(eventName);
			if (this.enableFsmBeforeSend && !enabled && !flag)
			{
				this.targetFsm.enabled = false;
			}
			if (!string.IsNullOrEmpty(this.setFsmBool))
			{
				this.targetFsm.FsmVariables.FindFsmBool(this.setFsmBool).Value = this.setFsmBoolValue;
			}
		}
		else
		{
			PlayMakerFSM[] components = base.gameObject.GetComponents<PlayMakerFSM>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].SendEventRecursive(eventName);
			}
		}
		base.CallReceivedEvent();
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x000855F0 File Offset: 0x000837F0
	public void SwitchEvent(string eventName)
	{
		if (!this.didAwake && !EventRegister.auditListsOnUnload)
		{
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				EventRegister.auditListsOnUnload = true;
				instance.NextSceneWillActivate += EventRegister.AuditEventRegisterLists;
			}
		}
		EventRegister.UnsubscribeEvent(this);
		this.subscribedEvent = eventName;
		this.UpdateEventHash();
		EventRegister.SubscribeEvent(this);
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x0008564C File Offset: 0x0008384C
	private static void AuditEventRegisterLists()
	{
		EventRegister.auditListsOnUnload = false;
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.NextSceneWillActivate -= EventRegister.AuditEventRegisterLists;
		}
		using (Dictionary<int, List<EventRegister>>.ValueCollection.Enumerator enumerator = EventRegister._eventRegister.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.RemoveAll((EventRegister o) => o == null);
			}
		}
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x000856E8 File Offset: 0x000838E8
	public static void SendEvent(string eventName, GameObject excludeGameObject = null)
	{
		EventRegister.SendEvent(eventName.GetHashCode(), excludeGameObject);
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x000856F8 File Offset: 0x000838F8
	public static void SendEvent(int eventNameHash, GameObject excludeGameObject = null)
	{
		List<EventRegister> list;
		if (!EventRegister._eventRegister.TryGetValue(eventNameHash, out list))
		{
			return;
		}
		if (excludeGameObject)
		{
			using (List<EventRegister>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					EventRegister eventRegister = enumerator.Current;
					if (!(eventRegister.gameObject == excludeGameObject))
					{
						eventRegister.ReceiveEvent();
					}
				}
				return;
			}
		}
		foreach (EventRegister eventRegister2 in list)
		{
			eventRegister2.ReceiveEvent();
		}
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x000857A8 File Offset: 0x000839A8
	private static void SubscribeEvent(EventRegister register)
	{
		if (!register.didAwake)
		{
			return;
		}
		int num = register.subscribedEventHash;
		if (num == 0)
		{
			return;
		}
		List<EventRegister> list;
		if (!EventRegister._eventRegister.TryGetValue(num, out list))
		{
			list = new List<EventRegister>();
			EventRegister._eventRegister.Add(num, list);
		}
		list.Add(register);
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x000857F4 File Offset: 0x000839F4
	private static void UnsubscribeEvent(EventRegister register)
	{
		int num = register.subscribedEventHash;
		if (num == 0)
		{
			return;
		}
		List<EventRegister> list;
		if (!EventRegister._eventRegister.TryGetValue(num, out list))
		{
			return;
		}
		if (!list.Remove(register))
		{
			return;
		}
		if (list.Count <= 0)
		{
			EventRegister._eventRegister.Remove(num);
		}
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x0008583C File Offset: 0x00083A3C
	public static EventRegister GetRegisterGuaranteed(GameObject gameObject, string eventName)
	{
		foreach (EventRegister eventRegister in gameObject.GetComponents<EventRegister>())
		{
			if (eventRegister.subscribedEvent.Equals(eventName))
			{
				return eventRegister;
			}
		}
		EventRegister eventRegister2 = gameObject.AddComponent<EventRegister>();
		eventRegister2.SwitchEvent(eventName);
		return eventRegister2;
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x00085880 File Offset: 0x00083A80
	public static void RemoveRegister(GameObject gameObject, string eventName)
	{
		foreach (EventRegister eventRegister in gameObject.GetComponents<EventRegister>())
		{
			if (eventRegister.subscribedEvent.Equals(eventName))
			{
				Object.Destroy(eventRegister);
			}
		}
	}

	// Token: 0x04001BC3 RID: 7107
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckSubscribedEvent")]
	private string subscribedEvent;

	// Token: 0x04001BC4 RID: 7108
	[SerializeField]
	private PlayMakerFSM targetFsm;

	// Token: 0x04001BC5 RID: 7109
	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	[InspectorValidation("IsFsmBoolValid")]
	private string setFsmBool;

	// Token: 0x04001BC6 RID: 7110
	[SerializeField]
	[ModifiableProperty]
	[Conditional("targetFsm", true, false, false)]
	private bool setFsmBoolValue;

	// Token: 0x04001BC7 RID: 7111
	[Space]
	[SerializeField]
	private bool enableFsmBeforeSend;

	// Token: 0x04001BC8 RID: 7112
	[Space]
	[SerializeField]
	private EventRegister.AliasEventMode aliasEventMode;

	// Token: 0x04001BC9 RID: 7113
	[Tooltip("Alias event not added to register.")]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("CheckAlias")]
	private string aliasEvent;

	// Token: 0x04001BCA RID: 7114
	private new bool didAwake;

	// Token: 0x04001BCB RID: 7115
	private int subscribedEventHash;

	// Token: 0x04001BCC RID: 7116
	private static readonly Dictionary<int, List<EventRegister>> _eventRegister = new Dictionary<int, List<EventRegister>>();

	// Token: 0x04001BCD RID: 7117
	private static bool auditListsOnUnload;

	// Token: 0x020015FC RID: 5628
	[Serializable]
	private enum AliasEventMode
	{
		// Token: 0x0400895D RID: 35165
		None,
		// Token: 0x0400895E RID: 35166
		SendAlias
	}
}
