using System;
using System.Collections.Generic;
using GlobalSettings;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005EA RID: 1514
public class ToolItemLimiter : MonoBehaviour
{
	// Token: 0x060035EA RID: 13802 RVA: 0x000ED8DF File Offset: 0x000EBADF
	public static void ClearStatic()
	{
		ToolItemLimiter._tempInts.Clear();
		ToolItemLimiter._lookup.Clear();
	}

	// Token: 0x060035EB RID: 13803 RVA: 0x000ED8F5 File Offset: 0x000EBAF5
	[UsedImplicitly]
	private bool? ValidateFsmEvent(string eventName)
	{
		return this.targetFsm.IsEventValid(eventName, false);
	}

	// Token: 0x060035EC RID: 13804 RVA: 0x000ED904 File Offset: 0x000EBB04
	[UsedImplicitly]
	private bool? ValidateFsmBool(string boolName)
	{
		if (!this.targetFsm || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return new bool?(this.targetFsm.FsmVariables.FindFsmBool(boolName) != null);
	}

	// Token: 0x060035ED RID: 13805 RVA: 0x000ED949 File Offset: 0x000EBB49
	private void OnEnable()
	{
		this.throwNum = ToolItemLimiter._lastThrowNum;
		this.RecordSpawn();
	}

	// Token: 0x060035EE RID: 13806 RVA: 0x000ED95C File Offset: 0x000EBB5C
	private void OnDisable()
	{
		this.RemoveNode();
	}

	// Token: 0x060035EF RID: 13807 RVA: 0x000ED964 File Offset: 0x000EBB64
	private void RecordSpawn()
	{
		if (this.representingTool == null)
		{
			return;
		}
		ToolItemLimiter.ToolLimiterTracker toolLimiterTracker;
		if (!ToolItemLimiter._lookup.TryGetValue(this.representingTool, out toolLimiterTracker))
		{
			toolLimiterTracker = (ToolItemLimiter._lookup[this.representingTool] = new ToolItemLimiter.ToolLimiterTracker());
		}
		toolLimiterTracker.Add(this);
	}

	// Token: 0x060035F0 RID: 13808 RVA: 0x000ED9B4 File Offset: 0x000EBBB4
	private void RemoveNode()
	{
		if (this.representingTool == null)
		{
			return;
		}
		ToolItemLimiter.ToolLimiterTracker toolLimiterTracker;
		if (!ToolItemLimiter._lookup.TryGetValue(this.representingTool, out toolLimiterTracker))
		{
			return;
		}
		toolLimiterTracker.Remove(this);
	}

	// Token: 0x060035F1 RID: 13809 RVA: 0x000ED9EC File Offset: 0x000EBBEC
	private void Break()
	{
		this.BreakInternal();
		if (this.representingTool == null)
		{
			return;
		}
		ToolItemLimiter.ToolLimiterTracker toolLimiterTracker;
		if (!ToolItemLimiter._lookup.TryGetValue(this.representingTool, out toolLimiterTracker))
		{
			return;
		}
		int num = this.throwNum;
		this.RemoveNode();
		while (toolLimiterTracker.ActiveLimiters.Count > 0)
		{
			LinkedListNode<ToolItemLimiter> first = toolLimiterTracker.ActiveLimiters.First;
			if (first.Value.throwNum != num)
			{
				break;
			}
			first.Value.BreakInternal();
			toolLimiterTracker.ActiveLimiters.RemoveFirst();
		}
	}

	// Token: 0x060035F2 RID: 13810 RVA: 0x000EDA70 File Offset: 0x000EBC70
	private void BreakInternal()
	{
		this.OnBreak.Invoke();
		if (!this.targetFsm)
		{
			return;
		}
		this.targetFsm.SendEvent(this.breakEvent);
		if (string.IsNullOrEmpty(this.breakSetBool))
		{
			return;
		}
		FsmBool fsmBool = this.targetFsm.FsmVariables.FindFsmBool(this.breakSetBool);
		if (fsmBool != null)
		{
			fsmBool.Value = true;
		}
	}

	// Token: 0x060035F3 RID: 13811 RVA: 0x000EDAD8 File Offset: 0x000EBCD8
	public static void ReportToolUsed(ToolItem thrownTool)
	{
		ToolItemLimiter._lastThrowNum++;
		int num = (thrownTool.Usage.UseAltForQuickSling && Gameplay.QuickSlingTool.IsEquipped) ? thrownTool.Usage.MaxActiveAlt : thrownTool.Usage.MaxActive;
		if (num <= 0)
		{
			return;
		}
		ToolItemLimiter.ToolLimiterTracker toolLimiterTracker;
		if (!ToolItemLimiter._lookup.TryGetValue(thrownTool, out toolLimiterTracker))
		{
			return;
		}
		if (toolLimiterTracker.ActiveLimiters.Count > 0)
		{
			ToolItemLimiter._tempInts.Add(toolLimiterTracker.ActiveLimiters.First.Value.throwNum);
			int num2 = toolLimiterTracker.ActiveLimiters.Last.Value.throwNum;
			ToolItemLimiter._tempInts.Add(num2);
			if (ToolItemLimiter._tempInts.Count >= 2)
			{
				LinkedListNode<ToolItemLimiter> next = toolLimiterTracker.ActiveLimiters.First.Next;
				while (next != null && next.Value.throwNum != num2)
				{
					ToolItemLimiter._tempInts.Add(next.Value.throwNum);
					next = next.Next;
				}
			}
		}
		int count = ToolItemLimiter._tempInts.Count;
		while (count-- >= num && toolLimiterTracker.ActiveLimiters.First != null)
		{
			toolLimiterTracker.ActiveLimiters.First.Value.Break();
		}
		ToolItemLimiter._tempInts.Clear();
	}

	// Token: 0x04003912 RID: 14610
	[SerializeField]
	private ToolItem representingTool;

	// Token: 0x04003913 RID: 14611
	[Space]
	[SerializeField]
	private PlayMakerFSM targetFsm;

	// Token: 0x04003914 RID: 14612
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string breakEvent;

	// Token: 0x04003915 RID: 14613
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmBool")]
	private string breakSetBool;

	// Token: 0x04003916 RID: 14614
	[Space]
	public UnityEvent OnBreak;

	// Token: 0x04003917 RID: 14615
	private float activateTime;

	// Token: 0x04003918 RID: 14616
	private static int _lastThrowNum;

	// Token: 0x04003919 RID: 14617
	private int throwNum;

	// Token: 0x0400391A RID: 14618
	private static readonly HashSet<int> _tempInts = new HashSet<int>();

	// Token: 0x0400391B RID: 14619
	private static readonly Dictionary<ToolItem, ToolItemLimiter.ToolLimiterTracker> _lookup = new Dictionary<ToolItem, ToolItemLimiter.ToolLimiterTracker>();

	// Token: 0x0400391C RID: 14620
	private LinkedListNode<ToolItemLimiter> activeNode;

	// Token: 0x020018F3 RID: 6387
	private sealed class ToolLimiterTracker
	{
		// Token: 0x060092B5 RID: 37557 RVA: 0x0029C71E File Offset: 0x0029A91E
		public void Add(ToolItemLimiter limiter)
		{
			this.Remove(limiter);
			limiter.activeNode = this.ActiveLimiters.AddLast(limiter);
		}

		// Token: 0x060092B6 RID: 37558 RVA: 0x0029C739 File Offset: 0x0029A939
		public void Remove(ToolItemLimiter limiter)
		{
			if (limiter.activeNode != null)
			{
				this.ActiveLimiters.Remove(limiter);
				limiter.activeNode = null;
			}
		}

		// Token: 0x040093E1 RID: 37857
		public readonly LinkedList<ToolItemLimiter> ActiveLimiters = new LinkedList<ToolItemLimiter>();
	}
}
