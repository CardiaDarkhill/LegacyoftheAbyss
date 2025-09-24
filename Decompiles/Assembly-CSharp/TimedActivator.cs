using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000574 RID: 1396
public class TimedActivator : MonoBehaviour
{
	// Token: 0x060031F3 RID: 12787 RVA: 0x000DDFE9 File Offset: 0x000DC1E9
	private void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			this.Awake();
			this.DoSetup(true);
		}
	}

	// Token: 0x060031F4 RID: 12788 RVA: 0x000DDFFF File Offset: 0x000DC1FF
	private void OnValidate()
	{
		if (this.duration < 0f)
		{
			this.duration = 0f;
		}
		if (this.deactivateWarningDuration > this.duration)
		{
			this.deactivateWarningDuration = this.duration;
		}
	}

	// Token: 0x060031F5 RID: 12789 RVA: 0x000DE034 File Offset: 0x000DC234
	private void Awake()
	{
		this.OnValidate();
		this.activateObjects = (this.activateObjectsParent ? this.activateObjectsParent.GetComponentsInChildren<ActivatingBase>() : Array.Empty<ActivatingBase>());
		this.interactiveObjects = (this.interactiveObjectsParent ? this.interactiveObjectsParent.GetComponentsInChildren<ActivatingBase>() : Array.Empty<ActivatingBase>());
		this.UpdateSiblingLists();
	}

	// Token: 0x060031F6 RID: 12790 RVA: 0x000DE097 File Offset: 0x000DC297
	private void OnEnable()
	{
		TimedActivator._timedActivators.Add(this);
	}

	// Token: 0x060031F7 RID: 12791 RVA: 0x000DE0A4 File Offset: 0x000DC2A4
	private void Start()
	{
		this.DoSetup(false);
	}

	// Token: 0x060031F8 RID: 12792 RVA: 0x000DE0AD File Offset: 0x000DC2AD
	private void OnDisable()
	{
		TimedActivator._timedActivators.Remove(this);
	}

	// Token: 0x060031F9 RID: 12793 RVA: 0x000DE0BC File Offset: 0x000DC2BC
	private void DoSetup(bool drawingGizmos)
	{
		if (this.allActivators == null)
		{
			this.allActivators = new List<TimedActivator.Activator>(this.activateObjects.Length);
		}
		else
		{
			this.allActivators.Clear();
		}
		Vector2 vector = base.transform.position;
		foreach (KeyValuePair<Transform, Dictionary<int, List<TimedActivator.Activator>>> keyValuePair in this.activateSiblings)
		{
			foreach (KeyValuePair<int, List<TimedActivator.Activator>> keyValuePair2 in keyValuePair.Value)
			{
				List<TimedActivator.Activator> value = keyValuePair2.Value;
				float num = 0f;
				for (int i = 0; i < value.Count; i++)
				{
					TimedActivator.Activator activator = value[i];
					Vector2 a = activator.Activate.transform.position;
					Vector2 b;
					if (i == 0)
					{
						TimedActivator.Activator activator2 = null;
						float num2 = Vector2.Distance(a, vector);
						if (keyValuePair2.Key > 0)
						{
							foreach (KeyValuePair<int, List<TimedActivator.Activator>> keyValuePair3 in keyValuePair.Value)
							{
								if (keyValuePair3.Key != keyValuePair2.Key)
								{
									foreach (TimedActivator.Activator activator3 in keyValuePair3.Value)
									{
										float num3 = Vector2.Distance(a, activator3.Activate.transform.position);
										if (num3 <= num2)
										{
											activator2 = activator3;
											num2 = num3;
										}
									}
								}
							}
						}
						if (activator2 != null)
						{
							b = activator2.Activate.transform.position;
							num = activator2.Distance;
						}
						else
						{
							b = vector;
						}
					}
					else
					{
						b = value[i - 1].Activate.transform.position;
					}
					num += Vector2.Distance(a, b);
					activator.Distance = num;
					activator.DeactivateDelay = this.interactiveParentDelay;
					this.AddActivator(activator);
				}
			}
		}
		foreach (ActivatingBase activatingBase in this.interactiveObjects)
		{
			Vector2 a2 = activatingBase.transform.position;
			TimedActivator.Activator activator4 = null;
			float num4 = float.MaxValue;
			foreach (KeyValuePair<Transform, Dictionary<int, List<TimedActivator.Activator>>> keyValuePair4 in this.activateSiblings)
			{
				foreach (KeyValuePair<int, List<TimedActivator.Activator>> keyValuePair5 in keyValuePair4.Value)
				{
					foreach (TimedActivator.Activator activator5 in keyValuePair5.Value)
					{
						float num5 = Vector2.Distance(a2, activator5.Activate.transform.position);
						if (num5 <= num4)
						{
							activator4 = activator5;
							num4 = num5;
						}
					}
				}
			}
			float num6;
			if (activator4 == null)
			{
				base.transform.position;
				num6 = 0f;
				num4 = Vector2.Distance(a2, vector);
			}
			else
			{
				activator4.Activate.transform.position;
				num6 = activator4.Distance;
			}
			float distance = num6 + num4;
			this.AddActivator(new TimedActivator.Activator
			{
				Activate = activatingBase,
				Distance = distance,
				ActivateDelay = this.interactiveParentDelay,
				DeactivateDelay = -this.interactiveParentDelay
			});
		}
	}

	// Token: 0x060031FA RID: 12794 RVA: 0x000DE538 File Offset: 0x000DC738
	private void AddActivator(TimedActivator.Activator activator)
	{
		Animator component = activator.Activate.GetComponent<Animator>();
		activator.Animator = component;
		activator.HasAnimatorParam = (component && component.HasParameter(TimedActivator._growSpeedProp, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Float)));
		if (activator.HasAnimatorParam)
		{
			activator.CurrentParamValue = component.GetFloat(TimedActivator._growSpeedProp);
		}
		this.allActivators.Add(activator);
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x000DE5A0 File Offset: 0x000DC7A0
	private void Update()
	{
		float num = this.growCurve.Evaluate(Time.time);
		float deltaTime = Time.deltaTime;
		for (int i = 0; i < this.allActivators.Count; i++)
		{
			TimedActivator.Activator activator = this.allActivators[i];
			if (activator.HasAnimatorParam && Math.Abs(activator.CurrentParamValue - num) > 1E-45f)
			{
				activator.Animator.SetFloat(TimedActivator._growSpeedProp, num);
				activator.CurrentParamValue = num;
			}
		}
		this.ProcessTrackerOnActivatingComplete(this.activatingTrackers, deltaTime);
		this.ProcessTrackerOnWarningComplete(this.warningTrackers, deltaTime);
		this.ProcessTrackerOnDeactivatingComplete(this.deactivatingTrackers, deltaTime);
	}

	// Token: 0x060031FC RID: 12796 RVA: 0x000DE644 File Offset: 0x000DC844
	private void ProcessTracker(List<TimedActivator.Tracker> trackerList, float deltaTime, Action<TimedActivator.Tracker> onTrackerComplete)
	{
		if (trackerList == null)
		{
			return;
		}
		bool flag = onTrackerComplete != null;
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			TimedActivator.Tracker tracker = trackerList[i];
			if (tracker.Activator.Activate.IsPaused)
			{
				trackerList[num] = tracker;
				num++;
			}
			else
			{
				tracker.TimeLeft -= deltaTime;
				if (tracker.TimeLeft > 0f)
				{
					trackerList[num] = tracker;
					num++;
				}
				else if (flag)
				{
					onTrackerComplete(tracker);
				}
			}
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	// Token: 0x060031FD RID: 12797 RVA: 0x000DE6E0 File Offset: 0x000DC8E0
	private void ProcessTrackerOnActivatingComplete(List<TimedActivator.Tracker> trackerList, float deltaTime)
	{
		if (trackerList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			TimedActivator.Tracker tracker = trackerList[i];
			if (tracker.Activator.Activate.IsPaused)
			{
				trackerList[num] = tracker;
				num++;
			}
			else
			{
				tracker.TimeLeft -= deltaTime;
				if (tracker.TimeLeft > 0f)
				{
					trackerList[num] = tracker;
					num++;
				}
				else
				{
					TimedActivator.Activator activator = tracker.Activator;
					activator.lastState = TimedActivator.LastActivatorState.Activate;
					activator.Activate.SetActive(true, false);
				}
			}
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x000DE784 File Offset: 0x000DC984
	private void ProcessTrackerOnWarningComplete(List<TimedActivator.Tracker> trackerList, float deltaTime)
	{
		if (trackerList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			TimedActivator.Tracker tracker = trackerList[i];
			if (tracker.Activator.Activate.IsPaused)
			{
				trackerList[num] = tracker;
				num++;
			}
			else
			{
				tracker.TimeLeft -= deltaTime;
				if (tracker.TimeLeft > 0f)
				{
					trackerList[num] = tracker;
					num++;
				}
				else
				{
					TimedActivator.Activator activator = tracker.Activator;
					if (activator.lastState == TimedActivator.LastActivatorState.Deactivate || activator.lastState == TimedActivator.LastActivatorState.Warning)
					{
						return;
					}
					activator.lastState = TimedActivator.LastActivatorState.Warning;
					activator.Activate.DeactivateWarning();
				}
			}
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	// Token: 0x060031FF RID: 12799 RVA: 0x000DE840 File Offset: 0x000DCA40
	private void ProcessTrackerOnDeactivatingComplete(List<TimedActivator.Tracker> trackerList, float deltaTime)
	{
		if (trackerList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < trackerList.Count; i++)
		{
			TimedActivator.Tracker tracker = trackerList[i];
			if (tracker.Activator.Activate.IsPaused)
			{
				trackerList[num] = tracker;
				num++;
			}
			else
			{
				tracker.TimeLeft -= deltaTime;
				if (tracker.TimeLeft > 0f)
				{
					trackerList[num] = tracker;
					num++;
				}
				else
				{
					TimedActivator.Activator activator = tracker.Activator;
					if (activator.lastState == TimedActivator.LastActivatorState.NotActive)
					{
						return;
					}
					if (activator.lastState == TimedActivator.LastActivatorState.Deactivate)
					{
						return;
					}
					activator.lastState = TimedActivator.LastActivatorState.Deactivate;
					activator.Activate.SetActive(false, false);
				}
			}
		}
		if (num < trackerList.Count)
		{
			trackerList.RemoveRange(num, trackerList.Count - num);
		}
	}

	// Token: 0x06003200 RID: 12800 RVA: 0x000DE900 File Offset: 0x000DCB00
	public void StartActiveTimer()
	{
		this.durationLeft = this.duration;
		if (this.activationRoutine != null)
		{
			this.OnAlreadyActivated.Invoke();
			base.StopCoroutine(this.activationRoutine);
		}
		this.interrupted = false;
		this.activationRoutine = base.StartCoroutine(this.Activation());
	}

	// Token: 0x06003201 RID: 12801 RVA: 0x000DE951 File Offset: 0x000DCB51
	private IEnumerator Activation()
	{
		if (!this.interrupted)
		{
			this.OnActivated.Invoke();
		}
		if (this.delay > 0f)
		{
			yield return new WaitForSeconds(this.delay);
		}
		if (!this.interrupted)
		{
			this.OnActivatedDelay.Invoke();
			this.StartActivation(true);
			this.interrupted = false;
		}
		if (this.deactivateDelay > 0f)
		{
			yield return new WaitForSeconds(this.deactivateDelay);
			this.OnDeactivate.Invoke();
		}
		bool hasWarned = false;
		while (this.durationLeft > 0f)
		{
			yield return null;
			this.durationLeft -= Time.deltaTime;
			if (!this.interrupted && !hasWarned && this.durationLeft <= this.deactivateWarningDuration)
			{
				hasWarned = true;
				this.SendDeactivateWarning();
			}
		}
		if (this.deactivateDelay <= 0f)
		{
			this.OnDeactivate.Invoke();
		}
		if (!this.interrupted)
		{
			this.StartActivation(false);
		}
		this.activationRoutine = null;
		yield break;
	}

	// Token: 0x06003202 RID: 12802 RVA: 0x000DE960 File Offset: 0x000DCB60
	[ContextMenu("Test Activate", true)]
	[ContextMenu("Test Deactivate", true)]
	[ContextMenu("Test Deactivate Warning", true)]
	private bool CanDoContextMenu()
	{
		return Application.isPlaying;
	}

	// Token: 0x06003203 RID: 12803 RVA: 0x000DE967 File Offset: 0x000DCB67
	[ContextMenu("Test Activate")]
	public void ForceActivate()
	{
		this.StartActivation(true);
	}

	// Token: 0x06003204 RID: 12804 RVA: 0x000DE970 File Offset: 0x000DCB70
	[ContextMenu("Test Deactivate")]
	public void ForceDeactivate()
	{
		this.StartActivation(false);
	}

	// Token: 0x06003205 RID: 12805 RVA: 0x000DE97C File Offset: 0x000DCB7C
	private void StartActivation(bool setActive)
	{
		List<TimedActivator.Tracker> list;
		float num;
		if (setActive)
		{
			if (this.deactivateOthers)
			{
				foreach (TimedActivator timedActivator in TimedActivator._timedActivators)
				{
					timedActivator.StartedActivating(timedActivator == this);
				}
			}
			list = this.activatingTrackers;
			num = this.distanceActivateDelay;
		}
		else
		{
			this.deactivatingTrackers.Clear();
			list = this.deactivatingTrackers;
			num = this.distanceDeactivateDelay;
		}
		foreach (TimedActivator.Activator activator in this.allActivators)
		{
			float timeLeft = activator.Distance * num + (setActive ? activator.ActivateDelay : activator.DeactivateDelay);
			list.Add(new TimedActivator.Tracker
			{
				Activator = activator,
				TimeLeft = timeLeft
			});
		}
	}

	// Token: 0x06003206 RID: 12806 RVA: 0x000DEA84 File Offset: 0x000DCC84
	[ContextMenu("Test Deactivate Warning")]
	public void SendDeactivateWarning()
	{
		this.warningTrackers.Clear();
		foreach (TimedActivator.Activator activator in this.allActivators)
		{
			this.warningTrackers.Add(new TimedActivator.Tracker
			{
				Activator = activator,
				TimeLeft = activator.Distance * this.distanceDeactivateDelay + activator.DeactivateDelay
			});
		}
	}

	// Token: 0x06003207 RID: 12807 RVA: 0x000DEB14 File Offset: 0x000DCD14
	private void StartedActivating(bool isFromHit)
	{
		this.interrupted = true;
		if (isFromHit && !this.deactivateOnReHit)
		{
			this.deactivatingTrackers.Clear();
			this.warningTrackers.Clear();
			this.activatingTrackers.Clear();
			return;
		}
		this.deactivatingTrackers.Clear();
		foreach (TimedActivator.Activator activator in this.allActivators)
		{
			if (activator.Activate.IsActive && activator.lastState != TimedActivator.LastActivatorState.NotActive && activator.lastState != TimedActivator.LastActivatorState.Deactivate)
			{
				this.deactivatingTrackers.Add(new TimedActivator.Tracker
				{
					Activator = activator,
					TimeLeft = 0f
				});
			}
		}
		this.warningTrackers.Clear();
		this.activatingTrackers.Clear();
	}

	// Token: 0x06003208 RID: 12808 RVA: 0x000DEBFC File Offset: 0x000DCDFC
	private void UpdateSiblingLists()
	{
		if (this.activateSiblings == null)
		{
			this.activateSiblings = new Dictionary<Transform, Dictionary<int, List<TimedActivator.Activator>>>();
		}
		else
		{
			foreach (KeyValuePair<Transform, Dictionary<int, List<TimedActivator.Activator>>> keyValuePair in this.activateSiblings)
			{
				foreach (KeyValuePair<int, List<TimedActivator.Activator>> keyValuePair2 in keyValuePair.Value)
				{
					keyValuePair2.Value.Clear();
				}
			}
		}
		foreach (ActivatingBase activatingBase in this.activateObjects)
		{
			Transform parent = activatingBase.transform.parent;
			Dictionary<int, List<TimedActivator.Activator>> dictionary;
			if (!this.activateSiblings.TryGetValue(parent, out dictionary))
			{
				dictionary = (this.activateSiblings[parent] = new Dictionary<int, List<TimedActivator.Activator>>());
			}
			List<TimedActivator.Activator> list;
			if (dictionary.TryGetValue(activatingBase.BranchIndex, out list))
			{
				list.Capacity = activatingBase.transform.parent.childCount;
			}
			else
			{
				list = (dictionary[activatingBase.BranchIndex] = new List<TimedActivator.Activator>(activatingBase.transform.parent.childCount));
			}
			list.Add(new TimedActivator.Activator
			{
				Activate = activatingBase
			});
		}
	}

	// Token: 0x04003586 RID: 13702
	[SerializeField]
	private Transform activateObjectsParent;

	// Token: 0x04003587 RID: 13703
	[SerializeField]
	private Transform interactiveObjectsParent;

	// Token: 0x04003588 RID: 13704
	[SerializeField]
	private float delay;

	// Token: 0x04003589 RID: 13705
	[SerializeField]
	private float deactivateDelay;

	// Token: 0x0400358A RID: 13706
	[SerializeField]
	private bool deactivateOnReHit;

	// Token: 0x0400358B RID: 13707
	[SerializeField]
	private float duration;

	// Token: 0x0400358C RID: 13708
	[SerializeField]
	private AnimationCurve growCurve = AnimationCurve.Linear(1f, 1f, 1f, 1f);

	// Token: 0x0400358D RID: 13709
	[SerializeField]
	private float deactivateWarningDuration;

	// Token: 0x0400358E RID: 13710
	[SerializeField]
	private float distanceActivateDelay;

	// Token: 0x0400358F RID: 13711
	[SerializeField]
	private float distanceDeactivateDelay;

	// Token: 0x04003590 RID: 13712
	[SerializeField]
	private float interactiveParentDelay;

	// Token: 0x04003591 RID: 13713
	[SerializeField]
	private bool deactivateOthers = true;

	// Token: 0x04003592 RID: 13714
	[Space]
	public UnityEvent OnActivated;

	// Token: 0x04003593 RID: 13715
	public UnityEvent OnActivatedDelay;

	// Token: 0x04003594 RID: 13716
	public UnityEvent OnAlreadyActivated;

	// Token: 0x04003595 RID: 13717
	public UnityEvent OnDeactivate;

	// Token: 0x04003596 RID: 13718
	private ActivatingBase[] activateObjects;

	// Token: 0x04003597 RID: 13719
	private ActivatingBase[] interactiveObjects;

	// Token: 0x04003598 RID: 13720
	private Dictionary<Transform, Dictionary<int, List<TimedActivator.Activator>>> activateSiblings;

	// Token: 0x04003599 RID: 13721
	private List<TimedActivator.Activator> allActivators;

	// Token: 0x0400359A RID: 13722
	private readonly List<TimedActivator.Tracker> activatingTrackers = new List<TimedActivator.Tracker>();

	// Token: 0x0400359B RID: 13723
	private readonly List<TimedActivator.Tracker> warningTrackers = new List<TimedActivator.Tracker>();

	// Token: 0x0400359C RID: 13724
	private readonly List<TimedActivator.Tracker> deactivatingTrackers = new List<TimedActivator.Tracker>();

	// Token: 0x0400359D RID: 13725
	private float durationLeft;

	// Token: 0x0400359E RID: 13726
	private Coroutine activationRoutine;

	// Token: 0x0400359F RID: 13727
	private static readonly int _growSpeedProp = Animator.StringToHash("Grow Speed");

	// Token: 0x040035A0 RID: 13728
	private static readonly List<TimedActivator> _timedActivators = new List<TimedActivator>();

	// Token: 0x040035A1 RID: 13729
	private bool interrupted;

	// Token: 0x0200187C RID: 6268
	private class Activator
	{
		// Token: 0x04009235 RID: 37429
		public ActivatingBase Activate;

		// Token: 0x04009236 RID: 37430
		public float Distance;

		// Token: 0x04009237 RID: 37431
		public float ActivateDelay;

		// Token: 0x04009238 RID: 37432
		public float DeactivateDelay;

		// Token: 0x04009239 RID: 37433
		public Animator Animator;

		// Token: 0x0400923A RID: 37434
		public bool HasAnimatorParam;

		// Token: 0x0400923B RID: 37435
		public float CurrentParamValue;

		// Token: 0x0400923C RID: 37436
		public TimedActivator.LastActivatorState lastState;
	}

	// Token: 0x0200187D RID: 6269
	private struct Tracker
	{
		// Token: 0x0400923D RID: 37437
		public TimedActivator.Activator Activator;

		// Token: 0x0400923E RID: 37438
		public float TimeLeft;
	}

	// Token: 0x0200187E RID: 6270
	private enum LastActivatorState
	{
		// Token: 0x04009240 RID: 37440
		NotActive,
		// Token: 0x04009241 RID: 37441
		Activate,
		// Token: 0x04009242 RID: 37442
		Warning,
		// Token: 0x04009243 RID: 37443
		Deactivate
	}
}
