using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200019F RID: 415
public abstract class CustomPlayMakerPhysicsEvent<T> : MonoBehaviour
{
	// Token: 0x0600102A RID: 4138 RVA: 0x0004E084 File Offset: 0x0004C284
	protected void SendEvent(T other)
	{
		if (this.eventResponders.Count > 0)
		{
			if (this.listDirty)
			{
				this.runList.Clear();
				if (this.runList.Capacity < this.eventResponders.Count)
				{
					this.runList.Capacity = this.eventResponders.Count;
				}
				this.runList.AddRange(this.eventResponders);
				this.listDirty = false;
			}
			this.sendingEvents = true;
			for (int i = this.runList.Count - 1; i >= 0; i--)
			{
				CustomPlayMakerPhysicsEvent<T>.EventResponder eventResponder = this.runList[i];
				if (!eventResponder.SendEvent(other))
				{
					this.eventResponders.Remove(eventResponder);
				}
			}
			if (this.eventResponders.Count == 0)
			{
				this.runList.Clear();
			}
			this.sendingEvents = false;
		}
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x0004E15C File Offset: 0x0004C35C
	public CustomPlayMakerPhysicsEvent<T>.EventResponder Add(FsmStateAction fsmStateAction, Action<T> collisionCallback)
	{
		if (fsmStateAction == null)
		{
			return null;
		}
		if (collisionCallback == null)
		{
			return null;
		}
		CustomPlayMakerPhysicsEvent<T>.EventResponder eventResponder = new CustomPlayMakerPhysicsEvent<T>.EventResponder(fsmStateAction, collisionCallback);
		if (!this.eventResponders.Add(eventResponder))
		{
			if (this.eventResponders.Remove(eventResponder))
			{
				this.eventResponders.Add(eventResponder);
				this.listDirty = true;
			}
			else
			{
				eventResponder = null;
			}
		}
		else
		{
			this.listDirty = true;
		}
		return eventResponder;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x0004E1BC File Offset: 0x0004C3BC
	public void Remove(CustomPlayMakerPhysicsEvent<T>.EventResponder eventResponder)
	{
		if (eventResponder == null)
		{
			return;
		}
		eventResponder.pendingRemoval = true;
		if (this.eventResponders.Remove(eventResponder))
		{
			if (this.eventResponders.Count == 0)
			{
				if (!this.sendingEvents)
				{
					this.runList.Clear();
					return;
				}
			}
			else
			{
				this.listDirty = true;
			}
		}
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x0004E20C File Offset: 0x0004C40C
	public void Remove(FsmStateAction fsmStateAction)
	{
		this.removalHelper.stateAction = fsmStateAction;
		if (this.eventResponders.Remove(this.removalHelper))
		{
			if (this.eventResponders.Count == 0)
			{
				if (!this.sendingEvents)
				{
					this.runList.Clear();
					return;
				}
			}
			else
			{
				this.listDirty = true;
			}
		}
	}

	// Token: 0x04000FB9 RID: 4025
	private HashSet<CustomPlayMakerPhysicsEvent<T>.EventResponder> eventResponders = new HashSet<CustomPlayMakerPhysicsEvent<T>.EventResponder>();

	// Token: 0x04000FBA RID: 4026
	private List<CustomPlayMakerPhysicsEvent<T>.EventResponder> runList = new List<CustomPlayMakerPhysicsEvent<T>.EventResponder>();

	// Token: 0x04000FBB RID: 4027
	private bool listDirty;

	// Token: 0x04000FBC RID: 4028
	private bool sendingEvents;

	// Token: 0x04000FBD RID: 4029
	private CustomPlayMakerPhysicsEvent<T>.EventResponder removalHelper = new CustomPlayMakerPhysicsEvent<T>.EventResponder(null, null);

	// Token: 0x020014DE RID: 5342
	public sealed class EventResponder
	{
		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x0600851A RID: 34074 RVA: 0x0026ED54 File Offset: 0x0026CF54
		public bool IsValid
		{
			get
			{
				if (this.stateAction != null && this.stateAction.Active)
				{
					Fsm fsm = this.stateAction.Fsm;
					if (fsm != null && !fsm.Finished && fsm.ActiveState != null)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x0600851B RID: 34075 RVA: 0x0026ED98 File Offset: 0x0026CF98
		public bool SendEvent(T other)
		{
			if (this.pendingRemoval)
			{
				return false;
			}
			if (!this.IsValid)
			{
				return false;
			}
			Action<T> action = this.callback;
			if (action != null)
			{
				action(other);
			}
			return !this.pendingRemoval;
		}

		// Token: 0x0600851C RID: 34076 RVA: 0x0026EDC9 File Offset: 0x0026CFC9
		public EventResponder(FsmStateAction stateAction, Action<T> callback)
		{
			this.stateAction = stateAction;
			this.callback = callback;
		}

		// Token: 0x0600851D RID: 34077 RVA: 0x0026EDE0 File Offset: 0x0026CFE0
		public override bool Equals(object obj)
		{
			CustomPlayMakerPhysicsEvent<T>.EventResponder eventResponder = obj as CustomPlayMakerPhysicsEvent<T>.EventResponder;
			return eventResponder != null && this.stateAction == eventResponder.stateAction;
		}

		// Token: 0x0600851E RID: 34078 RVA: 0x0026EE07 File Offset: 0x0026D007
		public bool Equals(FsmStateAction stateAction)
		{
			return stateAction != null && this.stateAction.Equals(stateAction);
		}

		// Token: 0x0600851F RID: 34079 RVA: 0x0026EE1A File Offset: 0x0026D01A
		public override int GetHashCode()
		{
			FsmStateAction fsmStateAction = this.stateAction;
			if (fsmStateAction == null)
			{
				return 0;
			}
			return fsmStateAction.GetHashCode();
		}

		// Token: 0x04008505 RID: 34053
		public FsmStateAction stateAction;

		// Token: 0x04008506 RID: 34054
		public Action<T> callback;

		// Token: 0x04008507 RID: 34055
		public bool pendingRemoval;
	}
}
