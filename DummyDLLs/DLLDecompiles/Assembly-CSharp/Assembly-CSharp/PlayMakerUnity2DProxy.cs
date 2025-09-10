using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class PlayMakerUnity2DProxy : MonoBehaviour
{
	// Token: 0x060001A5 RID: 421 RVA: 0x00008E0A File Offset: 0x0000700A
	public void AddOnCollisionEnter2dDelegate(PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate del)
	{
		this.OnCollisionEnter2dDelegates = (PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate)Delegate.Combine(this.OnCollisionEnter2dDelegates, del);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00008E23 File Offset: 0x00007023
	public void RemoveOnCollisionEnter2dDelegate(PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate del)
	{
		this.OnCollisionEnter2dDelegates = (PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate)Delegate.Remove(this.OnCollisionEnter2dDelegates, del);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x00008E3C File Offset: 0x0000703C
	public void AddOnCollisionStay2dDelegate(PlayMakerUnity2DProxy.OnCollisionStay2dDelegate del)
	{
		this.OnCollisionStay2dDelegates = (PlayMakerUnity2DProxy.OnCollisionStay2dDelegate)Delegate.Combine(this.OnCollisionStay2dDelegates, del);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x00008E55 File Offset: 0x00007055
	public void RemoveOnCollisionStay2dDelegate(PlayMakerUnity2DProxy.OnCollisionStay2dDelegate del)
	{
		this.OnCollisionStay2dDelegates = (PlayMakerUnity2DProxy.OnCollisionStay2dDelegate)Delegate.Remove(this.OnCollisionStay2dDelegates, del);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x00008E6E File Offset: 0x0000706E
	public void AddOnCollisionExit2dDelegate(PlayMakerUnity2DProxy.OnCollisionExit2dDelegate del)
	{
		this.OnCollisionExit2dDelegates = (PlayMakerUnity2DProxy.OnCollisionExit2dDelegate)Delegate.Combine(this.OnCollisionExit2dDelegates, del);
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00008E87 File Offset: 0x00007087
	public void RemoveOnCollisionExit2dDelegate(PlayMakerUnity2DProxy.OnCollisionExit2dDelegate del)
	{
		this.OnCollisionExit2dDelegates = (PlayMakerUnity2DProxy.OnCollisionExit2dDelegate)Delegate.Remove(this.OnCollisionExit2dDelegates, del);
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00008EA0 File Offset: 0x000070A0
	public void AddOnTriggerEnter2dDelegate(PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate del)
	{
		this.OnTriggerEnter2dDelegates = (PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate)Delegate.Combine(this.OnTriggerEnter2dDelegates, del);
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00008EB9 File Offset: 0x000070B9
	public void RemoveOnTriggerEnter2dDelegate(PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate del)
	{
		this.OnTriggerEnter2dDelegates = (PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate)Delegate.Remove(this.OnTriggerEnter2dDelegates, del);
	}

	// Token: 0x060001AD RID: 429 RVA: 0x00008ED2 File Offset: 0x000070D2
	public void AddOnTriggerStay2dDelegate(PlayMakerUnity2DProxy.OnTriggerStay2dDelegate del)
	{
		this.OnTriggerStay2dDelegates = (PlayMakerUnity2DProxy.OnTriggerStay2dDelegate)Delegate.Combine(this.OnTriggerStay2dDelegates, del);
	}

	// Token: 0x060001AE RID: 430 RVA: 0x00008EEB File Offset: 0x000070EB
	public void RemoveOnTriggerStay2dDelegate(PlayMakerUnity2DProxy.OnTriggerStay2dDelegate del)
	{
		this.OnTriggerStay2dDelegates = (PlayMakerUnity2DProxy.OnTriggerStay2dDelegate)Delegate.Remove(this.OnTriggerStay2dDelegates, del);
	}

	// Token: 0x060001AF RID: 431 RVA: 0x00008F04 File Offset: 0x00007104
	public void AddOnTriggerExit2dDelegate(PlayMakerUnity2DProxy.OnTriggerExit2dDelegate del)
	{
		this.OnTriggerExit2dDelegates = (PlayMakerUnity2DProxy.OnTriggerExit2dDelegate)Delegate.Combine(this.OnTriggerExit2dDelegates, del);
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x00008F1D File Offset: 0x0000711D
	public void RemoveOnTriggerExit2dDelegate(PlayMakerUnity2DProxy.OnTriggerExit2dDelegate del)
	{
		this.OnTriggerExit2dDelegates = (PlayMakerUnity2DProxy.OnTriggerExit2dDelegate)Delegate.Remove(this.OnTriggerExit2dDelegates, del);
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x00008F36 File Offset: 0x00007136
	[ContextMenu("Help")]
	public void help()
	{
		Application.OpenURL("https://hutonggames.fogbugz.com/default.asp?W1150");
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x00008F42 File Offset: 0x00007142
	public void Start()
	{
		if (!PlayMakerUnity2d.isAvailable())
		{
			Debug.LogError("PlayMakerUnity2DProxy requires the 'PlayMaker Unity 2D' Prefab in the Scene.\nUse the menu 'PlayMaker/Addons/Unity 2D/Components/Add PlayMakerUnity2D to Scene' to correct the situation", this);
			base.enabled = false;
			return;
		}
		this.RefreshImplementation();
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x00008F64 File Offset: 0x00007164
	public void RefreshImplementation()
	{
		this.CheckGameObjectEventsImplementation();
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x00008F6C File Offset: 0x0000716C
	private void OnCollisionEnter2D(Collision2D coll)
	{
		if (this.HandleCollisionEnter2D)
		{
			this.lastCollision2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnCollisionEnter2DEvent);
		}
		if (this.OnCollisionEnter2dDelegates != null)
		{
			this.OnCollisionEnter2dDelegates(coll);
		}
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x00008FA4 File Offset: 0x000071A4
	private void OnCollisionStay2D(Collision2D coll)
	{
		if (this.debug)
		{
			Debug.Log("OnCollisionStay2D " + this.HandleCollisionStay2D.ToString(), base.gameObject);
		}
		if (this.HandleCollisionStay2D)
		{
			this.lastCollision2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnCollisionStay2DEvent);
		}
		if (this.OnCollisionStay2dDelegates != null)
		{
			this.OnCollisionStay2dDelegates(coll);
		}
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000900C File Offset: 0x0000720C
	private void OnCollisionExit2D(Collision2D coll)
	{
		if (this.debug)
		{
			Debug.Log("OnCollisionExit2D " + this.HandleCollisionExit2D.ToString(), base.gameObject);
		}
		if (this.HandleCollisionExit2D)
		{
			this.lastCollision2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnCollisionExit2DEvent);
		}
		if (this.OnCollisionExit2dDelegates != null)
		{
			this.OnCollisionExit2dDelegates(coll);
		}
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00009074 File Offset: 0x00007274
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (this.debug)
		{
			Debug.Log(base.gameObject.name + " OnTriggerEnter2D " + coll.gameObject.name, base.gameObject);
		}
		if (this.HandleTriggerEnter2D)
		{
			this.lastTrigger2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnTriggerEnter2DEvent);
		}
		if (this.OnTriggerEnter2dDelegates != null)
		{
			this.OnTriggerEnter2dDelegates(coll);
		}
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x000090E8 File Offset: 0x000072E8
	private void OnTriggerStay2D(Collider2D coll)
	{
		if (this.debug)
		{
			Debug.Log(base.gameObject.name + " OnTriggerStay2D " + coll.gameObject.name, base.gameObject);
		}
		if (this.HandleTriggerStay2D)
		{
			this.lastTrigger2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnTriggerStay2DEvent);
		}
		if (this.OnTriggerStay2dDelegates != null)
		{
			this.OnTriggerStay2dDelegates(coll);
		}
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000915C File Offset: 0x0000735C
	private void OnTriggerExit2D(Collider2D coll)
	{
		if (this.debug)
		{
			Debug.Log(base.gameObject.name + " OnTriggerExit2D " + coll.gameObject.name, base.gameObject);
		}
		if (this.HandleTriggerExit2D)
		{
			this.lastTrigger2DInfo = coll;
			PlayMakerUnity2d.ForwardEventToGameObject(base.gameObject, PlayMakerUnity2d.OnTriggerExit2DEvent);
		}
		if (this.OnTriggerExit2dDelegates != null)
		{
			this.OnTriggerExit2dDelegates(coll);
		}
	}

	// Token: 0x060001BA RID: 442 RVA: 0x000091D0 File Offset: 0x000073D0
	private void CheckGameObjectEventsImplementation()
	{
		foreach (PlayMakerFSM fsm in base.GetComponents<PlayMakerFSM>())
		{
			this.CheckFsmEventsImplementation(fsm);
		}
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00009200 File Offset: 0x00007400
	private void CheckFsmEventsImplementation(PlayMakerFSM fsm)
	{
		foreach (FsmTransition fsmTransition in fsm.FsmGlobalTransitions)
		{
			this.CheckTransition(fsmTransition.EventName);
		}
		FsmState[] fsmStates = fsm.FsmStates;
		for (int i = 0; i < fsmStates.Length; i++)
		{
			foreach (FsmTransition fsmTransition2 in fsmStates[i].Transitions)
			{
				this.CheckTransition(fsmTransition2.EventName);
			}
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00009278 File Offset: 0x00007478
	private void CheckTransition(string transitionName)
	{
		if (transitionName.Equals(PlayMakerUnity2d.OnCollisionEnter2DEvent))
		{
			this.HandleCollisionEnter2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnCollisionExit2DEvent))
		{
			this.HandleCollisionExit2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnCollisionStay2DEvent))
		{
			this.HandleCollisionStay2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnTriggerEnter2DEvent))
		{
			this.HandleTriggerEnter2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnTriggerExit2DEvent))
		{
			this.HandleTriggerExit2D = true;
		}
		if (transitionName.Equals(PlayMakerUnity2d.OnTriggerStay2DEvent))
		{
			this.HandleTriggerStay2D = true;
		}
	}

	// Token: 0x04000177 RID: 375
	public bool debug;

	// Token: 0x04000178 RID: 376
	[HideInInspector]
	public bool HandleCollisionEnter2D;

	// Token: 0x04000179 RID: 377
	[HideInInspector]
	public bool HandleCollisionExit2D;

	// Token: 0x0400017A RID: 378
	[HideInInspector]
	public bool HandleCollisionStay2D;

	// Token: 0x0400017B RID: 379
	[HideInInspector]
	public bool HandleTriggerEnter2D;

	// Token: 0x0400017C RID: 380
	[HideInInspector]
	public bool HandleTriggerExit2D;

	// Token: 0x0400017D RID: 381
	[HideInInspector]
	public bool HandleTriggerStay2D;

	// Token: 0x0400017E RID: 382
	[HideInInspector]
	public Collision2D lastCollision2DInfo;

	// Token: 0x0400017F RID: 383
	[HideInInspector]
	public Collider2D lastTrigger2DInfo;

	// Token: 0x04000180 RID: 384
	private PlayMakerUnity2DProxy.OnCollisionEnter2dDelegate OnCollisionEnter2dDelegates;

	// Token: 0x04000181 RID: 385
	private PlayMakerUnity2DProxy.OnCollisionStay2dDelegate OnCollisionStay2dDelegates;

	// Token: 0x04000182 RID: 386
	private PlayMakerUnity2DProxy.OnCollisionExit2dDelegate OnCollisionExit2dDelegates;

	// Token: 0x04000183 RID: 387
	private PlayMakerUnity2DProxy.OnTriggerEnter2dDelegate OnTriggerEnter2dDelegates;

	// Token: 0x04000184 RID: 388
	private PlayMakerUnity2DProxy.OnTriggerStay2dDelegate OnTriggerStay2dDelegates;

	// Token: 0x04000185 RID: 389
	private PlayMakerUnity2DProxy.OnTriggerExit2dDelegate OnTriggerExit2dDelegates;

	// Token: 0x020013CB RID: 5067
	// (Invoke) Token: 0x0600817D RID: 33149
	public delegate void OnCollisionEnter2dDelegate(Collision2D collisionInfo);

	// Token: 0x020013CC RID: 5068
	// (Invoke) Token: 0x06008181 RID: 33153
	public delegate void OnCollisionStay2dDelegate(Collision2D collisionInfo);

	// Token: 0x020013CD RID: 5069
	// (Invoke) Token: 0x06008185 RID: 33157
	public delegate void OnCollisionExit2dDelegate(Collision2D collisionInfo);

	// Token: 0x020013CE RID: 5070
	// (Invoke) Token: 0x06008189 RID: 33161
	public delegate void OnTriggerEnter2dDelegate(Collider2D collisionInfo);

	// Token: 0x020013CF RID: 5071
	// (Invoke) Token: 0x0600818D RID: 33165
	public delegate void OnTriggerStay2dDelegate(Collider2D collisionInfo);

	// Token: 0x020013D0 RID: 5072
	// (Invoke) Token: 0x06008191 RID: 33169
	public delegate void OnTriggerExit2dDelegate(Collider2D collisionInfo);
}
