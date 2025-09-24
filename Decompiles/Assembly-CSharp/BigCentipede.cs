using System;
using UnityEngine;

// Token: 0x0200029B RID: 667
public class BigCentipede : MonoBehaviour
{
	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001752 RID: 5970 RVA: 0x000691ED File Offset: 0x000673ED
	public Vector2 EntryPoint
	{
		get
		{
			return this.entryPoint;
		}
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001753 RID: 5971 RVA: 0x000691F5 File Offset: 0x000673F5
	public Vector2 ExitPoint
	{
		get
		{
			return this.exitPoint;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06001754 RID: 5972 RVA: 0x000691FD File Offset: 0x000673FD
	public Vector2 Direction
	{
		get
		{
			return this.direction;
		}
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x00069208 File Offset: 0x00067408
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.audioSource = base.GetComponent<AudioSource>();
		this.sections = base.GetComponentsInChildren<BigCentipedeSection>();
		if (this.audioSource)
		{
			this.audioSource.pitch = Random.Range(0.8f, 1.15f);
		}
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x0006926C File Offset: 0x0006746C
	protected void Start()
	{
		this.direction = base.transform.right.normalized;
		this.entryDust.transform.parent = null;
		if (this.entry != null)
		{
			this.entryPoint = this.entry.transform.position;
			this.entryDust.transform.SetPosition2D(this.entry.transform.position);
		}
		else
		{
			this.entryPoint = base.transform.position - this.direction * 12f;
		}
		this.exitDust.transform.parent = null;
		if (this.exit != null)
		{
			this.exitPoint = this.exit.transform.position;
			this.exitDust.transform.SetPosition2D(this.exit.transform.position);
		}
		else
		{
			this.exitPoint = base.transform.position + this.direction * 6f;
		}
		this.UnBurrow(false);
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x000693B8 File Offset: 0x000675B8
	private void UnBurrow(bool changePosition)
	{
		this.entryDust.Play();
		this.isBurrowing = false;
		if (changePosition)
		{
			base.transform.SetPosition2D(this.entryPoint - this.direction * 2.6f);
		}
		this.exitDust.Stop();
		this.meshRenderer.enabled = true;
		this.audioSource.volume = 0f;
		this.fadingAudio = true;
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x0006942E File Offset: 0x0006762E
	private void Burrow()
	{
		this.exitDust.Play();
		this.isBurrowing = true;
		this.burrowTimer = 0f;
		this.meshRenderer.enabled = false;
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x00069459 File Offset: 0x00067659
	protected void FixedUpdate()
	{
		this.body.MovePosition(this.body.position + this.direction * this.moveSpeed * Time.fixedDeltaTime);
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x00069494 File Offset: 0x00067694
	protected void Update()
	{
		Vector2 lhs = base.transform.position;
		if (!this.isBurrowing)
		{
			if (Vector2.Dot(lhs, this.direction) > Vector2.Dot(this.exitPoint, this.direction))
			{
				this.Burrow();
			}
		}
		else
		{
			this.burrowTimer += Time.deltaTime;
			if (this.burrowTimer > this.burrowTime)
			{
				this.UnBurrow(true);
			}
		}
		if (this.fadingAudio)
		{
			this.audioSource.volume += Time.deltaTime * 1.5f;
			if (this.audioSource.volume > 1f)
			{
				this.audioSource.volume = 1f;
				this.fadingAudio = false;
			}
		}
	}

	// Token: 0x040015F7 RID: 5623
	private Rigidbody2D body;

	// Token: 0x040015F8 RID: 5624
	private MeshRenderer meshRenderer;

	// Token: 0x040015F9 RID: 5625
	private AudioSource audioSource;

	// Token: 0x040015FA RID: 5626
	private BigCentipedeSection[] sections;

	// Token: 0x040015FB RID: 5627
	[SerializeField]
	private ParticleSystem entryDust;

	// Token: 0x040015FC RID: 5628
	[SerializeField]
	private ParticleSystem exitDust;

	// Token: 0x040015FD RID: 5629
	private Vector2 entryPoint;

	// Token: 0x040015FE RID: 5630
	private Vector2 exitPoint;

	// Token: 0x040015FF RID: 5631
	[SerializeField]
	private float burrowTime;

	// Token: 0x04001600 RID: 5632
	[SerializeField]
	private float moveSpeed;

	// Token: 0x04001601 RID: 5633
	private Vector2 direction;

	// Token: 0x04001602 RID: 5634
	private bool fadingAudio;

	// Token: 0x04001603 RID: 5635
	private bool isBurrowing;

	// Token: 0x04001604 RID: 5636
	private float burrowTimer;

	// Token: 0x04001605 RID: 5637
	[SerializeField]
	private Transform entry;

	// Token: 0x04001606 RID: 5638
	[SerializeField]
	private Transform exit;
}
