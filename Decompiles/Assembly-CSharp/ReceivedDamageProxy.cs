using System;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class ReceivedDamageProxy : MonoBehaviour, IHitResponder
{
	// Token: 0x06000156 RID: 342 RVA: 0x00007E9F File Offset: 0x0000609F
	public void AddHandler(ReceivedDamageBase handler)
	{
		this.handlers.Add(handler);
	}

	// Token: 0x06000157 RID: 343 RVA: 0x00007EAE File Offset: 0x000060AE
	public void RemoveHandler(ReceivedDamageBase handler)
	{
		if (this.handlers.Remove(handler))
		{
			this.dirty = true;
		}
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00007EC5 File Offset: 0x000060C5
	private void OnDisable()
	{
		this.hitting.Clear();
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00007ED4 File Offset: 0x000060D4
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.isHitting)
		{
			return IHitResponder.Response.None;
		}
		if (this.handlers.Count <= 0)
		{
			return IHitResponder.Response.None;
		}
		this.isHitting = true;
		this.hitting.AddRange(this.handlers);
		this.dirty = false;
		bool flag = false;
		for (int i = 0; i < this.hitting.Count; i++)
		{
			ReceivedDamageBase receivedDamageBase = this.hitting[i];
			if ((!this.dirty || this.handlers.Contains(receivedDamageBase)) && receivedDamageBase.RespondToHit(damageInstance))
			{
				flag = true;
			}
		}
		this.hitting.Clear();
		this.isHitting = false;
		return (flag && !this.dontReportHit) ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	// Token: 0x040000FB RID: 251
	[SerializeField]
	private bool dontReportHit;

	// Token: 0x040000FC RID: 252
	private readonly HashSet<ReceivedDamageBase> handlers = new HashSet<ReceivedDamageBase>();

	// Token: 0x040000FD RID: 253
	private readonly List<ReceivedDamageBase> hitting = new List<ReceivedDamageBase>();

	// Token: 0x040000FE RID: 254
	private int hittingIndex;

	// Token: 0x040000FF RID: 255
	private bool isHitting;

	// Token: 0x04000100 RID: 256
	private bool dirty;
}
