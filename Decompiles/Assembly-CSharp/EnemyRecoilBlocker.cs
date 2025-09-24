using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class EnemyRecoilBlocker : MonoBehaviour
{
	// Token: 0x06001A1F RID: 6687 RVA: 0x00078613 File Offset: 0x00076813
	private void Awake()
	{
		this.selfCollider = base.GetComponent<Collider2D>();
	}

	// Token: 0x06001A20 RID: 6688 RVA: 0x00078624 File Offset: 0x00076824
	private void OnCollisionEnter2D(Collision2D other)
	{
		Recoil component = other.gameObject.GetComponent<Recoil>();
		if (!component)
		{
			return;
		}
		if (this.touchingTrackRoutines.ContainsKey(component))
		{
			return;
		}
		this.touchingTrackRoutines[component] = base.StartCoroutine(this.TouchingTrack(component));
	}

	// Token: 0x06001A21 RID: 6689 RVA: 0x00078670 File Offset: 0x00076870
	private void OnCollisionExit2D(Collision2D other)
	{
		Recoil component = other.gameObject.GetComponent<Recoil>();
		if (!component)
		{
			return;
		}
		if (Physics2D.GetIgnoreCollision(this.selfCollider, other.collider))
		{
			return;
		}
		Coroutine routine;
		if (!this.touchingTrackRoutines.Remove(component, out routine))
		{
			return;
		}
		base.StopCoroutine(routine);
	}

	// Token: 0x06001A22 RID: 6690 RVA: 0x000786BE File Offset: 0x000768BE
	private IEnumerator TouchingTrack(Recoil recoil)
	{
		Collider2D recoilCollider = recoil.GetComponent<Collider2D>();
		WaitForFixedUpdate loopWait = new WaitForFixedUpdate();
		WaitForSeconds recoilWait = new WaitForSeconds(0.1f);
		while (!(recoilCollider == null))
		{
			if (recoil.IsRecoiling)
			{
				if (!Physics2D.GetIgnoreCollision(this.selfCollider, recoilCollider))
				{
					Physics2D.IgnoreCollision(this.selfCollider, recoilCollider);
					yield return recoilWait;
				}
			}
			else if (Physics2D.GetIgnoreCollision(this.selfCollider, recoilCollider))
			{
				if (this.results == null)
				{
					this.results = new Collider2D[10];
				}
				int b = Physics2D.OverlapCollider(this.selfCollider, new ContactFilter2D
				{
					useLayerMask = true,
					layerMask = 1 << recoilCollider.gameObject.layer
				}, this.results);
				bool flag = false;
				for (int i = 0; i < Mathf.Min(this.results.Length, b); i++)
				{
					if (!(this.results[i] != recoilCollider))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.touchingTrackRoutines.Remove(recoil);
					Physics2D.IgnoreCollision(this.selfCollider, recoilCollider, false);
					yield break;
				}
			}
			yield return loopWait;
		}
		yield break;
	}

	// Token: 0x0400190F RID: 6415
	private List<Recoil> touchingRecoilers;

	// Token: 0x04001910 RID: 6416
	private readonly Dictionary<Recoil, Coroutine> touchingTrackRoutines = new Dictionary<Recoil, Coroutine>();

	// Token: 0x04001911 RID: 6417
	private Collider2D[] results;

	// Token: 0x04001912 RID: 6418
	private Collider2D selfCollider;
}
