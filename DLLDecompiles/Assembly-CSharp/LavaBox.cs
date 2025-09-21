using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class LavaBox : MonoBehaviour
{
	// Token: 0x060006DB RID: 1755 RVA: 0x000227DC File Offset: 0x000209DC
	private void Awake()
	{
		BoxCollider2D component = base.GetComponent<BoxCollider2D>();
		if (component == null)
		{
			Debug.Log(string.Format("{0} is missing box collider", this), this);
			base.enabled = false;
			return;
		}
		this.effectPos_y = base.transform.position.y + component.offset.y + component.size.y / 2f + 0.5f;
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x0002284C File Offset: 0x00020A4C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (LavaSurfaceSplasher.TrySplash(gameObject) || gameObject.layer == 18 || gameObject.layer == 19 || gameObject.layer == 26)
		{
			Vector3 position = default(Vector3);
			if (this.isLavaFall)
			{
				position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0.003f);
			}
			else
			{
				position = new Vector3(gameObject.transform.position.x, this.effectPos_y, 0.003f);
			}
			this.lavaSplashSmallPrefab.Spawn(position);
		}
	}

	// Token: 0x040006B9 RID: 1721
	public GameObject lavaSplashSmallPrefab;

	// Token: 0x040006BA RID: 1722
	public bool isLavaFall;

	// Token: 0x040006BB RID: 1723
	private const float EFFECT_POS_Z = 0.003f;

	// Token: 0x040006BC RID: 1724
	private float effectPos_y;
}
