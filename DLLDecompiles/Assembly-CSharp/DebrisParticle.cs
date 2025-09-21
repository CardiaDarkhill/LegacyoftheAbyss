using System;
using UnityEngine;

// Token: 0x02000203 RID: 515
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(tk2dSprite))]
public class DebrisParticle : MonoBehaviour
{
	// Token: 0x06001376 RID: 4982 RVA: 0x00058B18 File Offset: 0x00056D18
	protected void Reset()
	{
		this.startZ = 0.019f;
		this.scaleMin = 1.25f;
		this.scaleMax = 2f;
		this.blackChance = 0.33333334f;
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x00058B46 File Offset: 0x00056D46
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.sprite = base.GetComponent<tk2dSprite>();
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x00058B60 File Offset: 0x00056D60
	protected void OnEnable()
	{
		if (this.randomSpriteIds.Length != 0)
		{
			this.sprite.SetSprite(this.sprite.Collection, this.randomSpriteIds[Random.Range(0, this.randomSpriteIds.Length)]);
		}
		Vector3 position = base.transform.position;
		position.z = this.startZ;
		base.transform.position = position;
		float num = Random.Range(this.scaleMin, this.scaleMax);
		Vector3 localScale = base.transform.localScale;
		localScale.x = num;
		localScale.y = num;
		base.transform.localScale = localScale;
		if (Random.Range(0f, 1f) < this.blackChance)
		{
			this.sprite.color = Color.black;
			position.z -= 0.05f;
			base.transform.position = position;
		}
		else
		{
			this.sprite.color = Color.white;
		}
		this.didSpin = false;
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x00058C5D File Offset: 0x00056E5D
	protected void Update()
	{
		if (!this.didSpin)
		{
			this.didSpin = true;
			this.body.AddTorque(-this.body.linearVelocity.x, ForceMode2D.Force);
		}
	}

	// Token: 0x040011DF RID: 4575
	private Rigidbody2D body;

	// Token: 0x040011E0 RID: 4576
	private tk2dSprite sprite;

	// Token: 0x040011E1 RID: 4577
	[SerializeField]
	private string[] randomSpriteIds;

	// Token: 0x040011E2 RID: 4578
	[SerializeField]
	private float startZ;

	// Token: 0x040011E3 RID: 4579
	[SerializeField]
	private float scaleMin;

	// Token: 0x040011E4 RID: 4580
	[SerializeField]
	private float scaleMax;

	// Token: 0x040011E5 RID: 4581
	[SerializeField]
	private float blackChance;

	// Token: 0x040011E6 RID: 4582
	private bool didSpin;
}
