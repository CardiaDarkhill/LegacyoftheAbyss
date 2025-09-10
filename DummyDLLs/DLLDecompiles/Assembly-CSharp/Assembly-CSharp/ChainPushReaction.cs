using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004B2 RID: 1202
public class ChainPushReaction : MonoBehaviour
{
	// Token: 0x14000088 RID: 136
	// (add) Token: 0x06002B62 RID: 11106 RVA: 0x000BE5F0 File Offset: 0x000BC7F0
	// (remove) Token: 0x06002B63 RID: 11107 RVA: 0x000BE628 File Offset: 0x000BC828
	public event Action<Vector3> OnTouched;

	// Token: 0x1700050D RID: 1293
	// (get) Token: 0x06002B64 RID: 11108 RVA: 0x000BE65D File Offset: 0x000BC85D
	// (set) Token: 0x06002B65 RID: 11109 RVA: 0x000BE665 File Offset: 0x000BC865
	private protected PlayChainSound Sound { protected get; private set; }

	// Token: 0x1700050E RID: 1294
	// (get) Token: 0x06002B66 RID: 11110 RVA: 0x000BE66E File Offset: 0x000BC86E
	// (set) Token: 0x06002B67 RID: 11111 RVA: 0x000BE676 File Offset: 0x000BC876
	public bool IsPushDisableStarted { get; private set; }

	// Token: 0x06002B68 RID: 11112 RVA: 0x000BE680 File Offset: 0x000BC880
	private void OnDrawGizmosSelected()
	{
		if (this.lowestLink)
		{
			Vector3 position = this.lowestLink.position;
			Gizmos.color = Color.green;
			Gizmos.DrawLine(position + Vector3.right * this.playerRangeEnable, position + Vector3.right * this.playerRangeEnable + Vector3.down);
			Gizmos.DrawLine(position - Vector3.right * this.playerRangeEnable, position - Vector3.right * this.playerRangeEnable + Vector3.down);
		}
	}

	// Token: 0x06002B69 RID: 11113 RVA: 0x000BE72C File Offset: 0x000BC92C
	protected virtual void Awake()
	{
		AutoGenerateHangingRope componentInChildren = base.GetComponentInChildren<AutoGenerateHangingRope>();
		if (componentInChildren && !componentInChildren.HasGenerated)
		{
			componentInChildren.Generated += this.SetupLinks;
			return;
		}
		this.SetupLinks();
	}

	// Token: 0x06002B6A RID: 11114 RVA: 0x000BE76C File Offset: 0x000BC96C
	private void SetupLinks()
	{
		ReplaceWithTemplate[] componentsInChildren = base.GetComponentsInChildren<ReplaceWithTemplate>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Awake();
		}
		this.links = base.GetComponentsInChildren<ChainLinkInteraction>(true);
		ChainLinkInteraction[] array = this.links;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Chain = this;
		}
		this.Sound = base.GetComponentInParent<PlayChainSound>();
		if (!base.transform.IsOnHeroPlane())
		{
			array = this.links;
			for (int i = 0; i < array.Length; i++)
			{
				Object.Destroy(array[i]);
			}
			Collider2D[] componentsInChildren2 = base.GetComponentsInChildren<Collider2D>(true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].gameObject.layer = 2;
			}
			Object.Destroy(this);
			HitRigidbody2D[] componentsInChildren3 = base.GetComponentsInChildren<HitRigidbody2D>(true);
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				Object.Destroy(componentsInChildren3[i]);
			}
			HitResponse[] componentsInChildren4 = base.GetComponentsInChildren<HitResponse>(true);
			for (int i = 0; i < componentsInChildren4.Length; i++)
			{
				Object.Destroy(componentsInChildren4[i]);
			}
			Breakable[] componentsInChildren5 = base.GetComponentsInChildren<Breakable>(true);
			for (int i = 0; i < componentsInChildren5.Length; i++)
			{
				Object.Destroy(componentsInChildren5[i]);
			}
			ChainAttackForce[] componentsInChildren6 = base.GetComponentsInChildren<ChainAttackForce>(true);
			for (int i = 0; i < componentsInChildren6.Length; i++)
			{
				Object.Destroy(componentsInChildren6[i]);
			}
		}
		if (!this.lowestLink)
		{
			this.GetLowestLink();
		}
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x000BE8BB File Offset: 0x000BCABB
	public void TouchedLink(Vector3 position)
	{
		if (this.Sound)
		{
			this.Sound.PlayTouchSound(position);
		}
		this.OnTouchedLink.Invoke();
		Action<Vector3> onTouched = this.OnTouched;
		if (onTouched == null)
		{
			return;
		}
		onTouched(position);
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x000BE8F2 File Offset: 0x000BCAF2
	public void DisableLinks(Transform trackObject)
	{
		this.AddDisableTracker(trackObject);
		if (!this.IsPushDisableStarted && this.lowestLink)
		{
			base.StartCoroutine(this.DisableLinksDelayed());
		}
	}

	// Token: 0x06002B6D RID: 11117 RVA: 0x000BE91D File Offset: 0x000BCB1D
	public void AddDisableTracker(Transform trackObject)
	{
		this.rangeEnableTracked.Add(trackObject);
	}

	// Token: 0x06002B6E RID: 11118 RVA: 0x000BE92B File Offset: 0x000BCB2B
	private IEnumerator DisableLinksDelayed()
	{
		this.IsPushDisableStarted = true;
		yield return new WaitForSeconds(this.touchTime);
		ChainLinkInteraction[] array = this.links;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		base.StartCoroutine(this.ReEnableLinks());
		yield break;
	}

	// Token: 0x06002B6F RID: 11119 RVA: 0x000BE93A File Offset: 0x000BCB3A
	private IEnumerator ReEnableLinks()
	{
		bool stillInRange = true;
		while (stillInRange)
		{
			stillInRange = false;
			yield return new WaitForSeconds(this.playerRangeEnableDelay);
			foreach (Transform transform in this.rangeEnableTracked)
			{
				if (transform && Mathf.Abs(transform.position.x - this.lowestLink.position.x) < this.playerRangeEnable)
				{
					stillInRange = true;
					break;
				}
			}
		}
		this.rangeEnableTracked.Clear();
		ChainLinkInteraction[] array = this.links;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(true);
		}
		this.IsPushDisableStarted = false;
		yield break;
	}

	// Token: 0x06002B70 RID: 11120 RVA: 0x000BE94C File Offset: 0x000BCB4C
	private void GetLowestLink()
	{
		float maxValue = float.MaxValue;
		foreach (ChainLinkInteraction chainLinkInteraction in this.links)
		{
			if (chainLinkInteraction.transform.position.y < maxValue)
			{
				this.lowestLink = chainLinkInteraction.transform;
			}
		}
	}

	// Token: 0x04002CB4 RID: 11444
	[SerializeField]
	private float touchTime = 0.25f;

	// Token: 0x04002CB5 RID: 11445
	[SerializeField]
	private float playerRangeEnable = 1f;

	// Token: 0x04002CB6 RID: 11446
	[SerializeField]
	private float playerRangeEnableDelay = 0.1f;

	// Token: 0x04002CB7 RID: 11447
	[Space]
	public UnityEvent OnTouchedLink;

	// Token: 0x04002CB9 RID: 11449
	private List<Transform> rangeEnableTracked = new List<Transform>();

	// Token: 0x04002CBA RID: 11450
	private ChainLinkInteraction[] links;

	// Token: 0x04002CBB RID: 11451
	private Transform lowestLink;
}
