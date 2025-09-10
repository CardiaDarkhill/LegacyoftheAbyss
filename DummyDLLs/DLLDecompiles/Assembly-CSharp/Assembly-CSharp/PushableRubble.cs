using System;
using UnityEngine;

// Token: 0x0200058B RID: 1419
[RequireComponent(typeof(Rigidbody2D))]
public class PushableRubble : MonoBehaviour
{
	// Token: 0x060032C7 RID: 12999 RVA: 0x000E1F91 File Offset: 0x000E0191
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060032C8 RID: 13000 RVA: 0x000E1FA0 File Offset: 0x000E01A0
	private void OnEnable()
	{
		Transform transform = base.transform;
		transform.SetRotation2D(Random.Range(0f, 360f));
		if (!this.dontPositionToGround)
		{
			Vector3 position = transform.position;
			RaycastHit2D hit = Helper.Raycast2D(position, Vector2.down, 10f, 256);
			if (hit)
			{
				Vector2 point = hit.point;
				Collider2D component = base.GetComponent<Collider2D>();
				if (component)
				{
					float num = position.y - component.bounds.min.y;
					point.y += num;
				}
				transform.SetPosition2D(point);
			}
			this.body.Sleep();
		}
	}

	// Token: 0x060032C9 RID: 13001 RVA: 0x000E2054 File Offset: 0x000E0254
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == 17)
		{
			this.hitAudio.SpawnAndPlayOneShot(base.transform.position, null);
		}
		this.Push();
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x000E2084 File Offset: 0x000E0284
	private void Push()
	{
		this.body.AddForce(new Vector2((float)Random.Range(-100, 100) * this.forceMultiplier.x, (float)Random.Range(0, 40) * this.forceMultiplier.y), ForceMode2D.Force);
		this.body.AddTorque((float)Random.Range(-50, 50) * this.torqueMultiplier, ForceMode2D.Force);
		this.pushAudio.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.pushAudioClipTable)
		{
			this.pushAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
	}

	// Token: 0x060032CB RID: 13003 RVA: 0x000E2127 File Offset: 0x000E0327
	public void EndRubble()
	{
		base.Invoke("EndRubbleContinuation", 5f);
	}

	// Token: 0x060032CC RID: 13004 RVA: 0x000E213C File Offset: 0x000E033C
	private void EndRubbleContinuation()
	{
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector2.zero;
		Collider2D component = base.GetComponent<Collider2D>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	// Token: 0x040036B5 RID: 14005
	[SerializeField]
	private Vector2 forceMultiplier = Vector2.one;

	// Token: 0x040036B6 RID: 14006
	[SerializeField]
	private float torqueMultiplier = 1f;

	// Token: 0x040036B7 RID: 14007
	[SerializeField]
	private bool dontPositionToGround;

	// Token: 0x040036B8 RID: 14008
	[Space]
	[SerializeField]
	private AudioEventRandom pushAudio;

	// Token: 0x040036B9 RID: 14009
	[SerializeField]
	private RandomAudioClipTable pushAudioClipTable;

	// Token: 0x040036BA RID: 14010
	[Space]
	[SerializeField]
	private AudioEventRandom hitAudio;

	// Token: 0x040036BB RID: 14011
	private Rigidbody2D body;

	// Token: 0x040036BC RID: 14012
	private const int GROUND_LAYER_MASK = 256;
}
