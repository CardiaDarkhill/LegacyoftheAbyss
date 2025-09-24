using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000244 RID: 580
public class JumpEffects : MonoBehaviour
{
	// Token: 0x0600152A RID: 5418 RVA: 0x0005FC2C File Offset: 0x0005DE2C
	private void OnEnable()
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
		this.recycleTimer = 0f;
		this.fallTimer = 0.1f;
		this.dripTimer = 0f;
		this.dripEndTimer = 0f;
		this.dripping = false;
		this.checkForFall = false;
		this.trailAttached = false;
	}

	// Token: 0x0600152B RID: 5419 RVA: 0x0005FCCC File Offset: 0x0005DECC
	public void Play(GameObject owner, Vector2 velocity, Vector3 posOffset)
	{
		this.recycleTimer = 2f;
		this.ownerObject = owner;
		this.ownerOffset = posOffset;
		this.previousOwnerPos = this.ownerObject.transform.position;
		switch (owner.GetComponent<EnviroRegionListener>().CurrentEnvironmentType)
		{
		case EnvironmentTypes.Grass:
			this.grassEffects.SetActive(true);
			this.checkForFall = true;
			this.PlayJumpPuff(velocity);
			this.PlayTrail();
			return;
		case EnvironmentTypes.Bone:
			this.boneEffects.SetActive(true);
			this.checkForFall = true;
			this.PlayJumpPuff(velocity);
			this.PlayTrail();
			return;
		case EnvironmentTypes.ShallowWater:
			this.SplashOut();
			return;
		case EnvironmentTypes.Moss:
		case EnvironmentTypes.WetMetal:
		case EnvironmentTypes.WetWood:
		case EnvironmentTypes.RunningWater:
			this.PlaySplash();
			return;
		case EnvironmentTypes.PeakPuff:
			this.peakPuffEffects.SetActive(true);
			this.checkForFall = true;
			this.PlayJumpPuff(velocity);
			this.PlayTrail();
			return;
		}
		this.dustEffects.SetActive(true);
		this.checkForFall = true;
		this.PlayJumpPuff(velocity);
		this.PlayTrail();
	}

	// Token: 0x0600152C RID: 5420 RVA: 0x0005FDEC File Offset: 0x0005DFEC
	private void Update()
	{
		if (this.recycleTimer > 0f)
		{
			this.recycleTimer -= Time.deltaTime;
			if (this.recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
				return;
			}
		}
		if (!this.ownerObject)
		{
			base.gameObject.Recycle();
			return;
		}
		this.ownerPos = this.ownerObject.transform.TransformPoint(this.ownerOffset);
		if (this.checkForFall)
		{
			if (this.fallTimer >= 0f)
			{
				this.fallTimer -= Time.deltaTime;
			}
			else
			{
				this.CheckForFall();
			}
		}
		if (this.trailAttached)
		{
			this.dustTrail.transform.position = new Vector3(this.ownerPos.x, this.ownerPos.y - 1.5f, this.ownerPos.z + 0.001f);
		}
		if (this.dripping)
		{
			if (this.dripTimer <= 0f)
			{
				Vector3 position = new Vector3(this.ownerPos.x + Random.Range(-0.25f, 0.25f), this.ownerPos.y + Random.Range(-0.5f, 0.5f), this.ownerPos.z);
				this.spatterWhitePrefab.Spawn(position);
				this.dripTimer += 0.025f;
			}
			else
			{
				this.dripTimer -= Time.deltaTime;
			}
			if (this.dripEndTimer <= 0f)
			{
				this.dripping = false;
			}
			else
			{
				this.dripEndTimer -= Time.deltaTime;
			}
		}
		this.previousOwnerPos = this.ownerPos;
	}

	// Token: 0x0600152D RID: 5421 RVA: 0x0005FFA8 File Offset: 0x0005E1A8
	private void CheckForFall()
	{
		if ((this.ownerPos.y - this.previousOwnerPos.y) / Time.deltaTime > 0f)
		{
			return;
		}
		this.jumpPuff.SetActive(false);
		this.dustTrail.GetComponent<ParticleSystem>().Stop();
		this.checkForFall = false;
	}

	// Token: 0x0600152E RID: 5422 RVA: 0x0005FFFD File Offset: 0x0005E1FD
	private void PlayTrail()
	{
		this.dustTrail.SetActive(true);
		this.trailAttached = true;
	}

	// Token: 0x0600152F RID: 5423 RVA: 0x00060014 File Offset: 0x0005E214
	private void PlayJumpPuff(Vector2 velocity)
	{
		float z = velocity.x * -3f + 2.6f;
		this.jumpPuff.transform.localEulerAngles = new Vector3(0f, 0f, z);
		this.jumpPuff.SetActive(true);
		if (this.jumpPuffAnimator == null)
		{
			this.jumpPuffAnimator = this.jumpPuff.GetComponent<tk2dSpriteAnimator>();
		}
		this.jumpPuffAnimator.PlayFromFrame(0);
	}

	// Token: 0x06001530 RID: 5424 RVA: 0x0006008C File Offset: 0x0005E28C
	private void SplashOut()
	{
		this.dripEndTimer = 0.4f;
		this.dripping = true;
		Vector3 position = this.ownerObject.transform.position;
		for (int i = 1; i <= 11; i++)
		{
			GameObject gameObject = this.spatterWhitePrefab.Spawn(position);
			float num = Random.Range(5f, 12f);
			float num2 = Random.Range(80f, 110f);
			float x = num * Mathf.Cos(num2 * 0.017453292f);
			float y = num * Mathf.Sin(num2 * 0.017453292f);
			Vector2 linearVelocity;
			linearVelocity.x = x;
			linearVelocity.y = y;
			gameObject.GetComponent<Rigidbody2D>().linearVelocity = linearVelocity;
		}
	}

	// Token: 0x06001531 RID: 5425 RVA: 0x00060130 File Offset: 0x0005E330
	private void PlaySplash()
	{
		Color color;
		AreaEffectTint.IsActive(base.transform.position, out color);
		this.splash.color = color;
		this.splash.gameObject.SetActive(true);
		Transform transform = this.splash.transform;
		Vector3 localScale = transform.localScale;
		if (Random.Range(1, 100) > 50)
		{
			transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
		}
	}

	// Token: 0x040013BC RID: 5052
	[SerializeField]
	private GameObject dustEffects;

	// Token: 0x040013BD RID: 5053
	[SerializeField]
	private GameObject grassEffects;

	// Token: 0x040013BE RID: 5054
	[SerializeField]
	private GameObject boneEffects;

	// Token: 0x040013BF RID: 5055
	[SerializeField]
	private GameObject peakPuffEffects;

	// Token: 0x040013C0 RID: 5056
	[SerializeField]
	private SpriteRenderer splash;

	// Token: 0x040013C1 RID: 5057
	[SerializeField]
	private GameObject jumpPuff;

	// Token: 0x040013C2 RID: 5058
	[SerializeField]
	private GameObject dustTrail;

	// Token: 0x040013C3 RID: 5059
	[SerializeField]
	private GameObject spatterWhitePrefab;

	// Token: 0x040013C4 RID: 5060
	private GameObject ownerObject;

	// Token: 0x040013C5 RID: 5061
	private Vector3 ownerOffset;

	// Token: 0x040013C6 RID: 5062
	private Vector3 ownerPos;

	// Token: 0x040013C7 RID: 5063
	private Vector3 previousOwnerPos;

	// Token: 0x040013C8 RID: 5064
	private tk2dSpriteAnimator jumpPuffAnimator;

	// Token: 0x040013C9 RID: 5065
	private float recycleTimer;

	// Token: 0x040013CA RID: 5066
	private float fallTimer;

	// Token: 0x040013CB RID: 5067
	private float dripTimer;

	// Token: 0x040013CC RID: 5068
	private float dripEndTimer;

	// Token: 0x040013CD RID: 5069
	private bool dripping;

	// Token: 0x040013CE RID: 5070
	private bool checkForFall;

	// Token: 0x040013CF RID: 5071
	private bool trailAttached;
}
