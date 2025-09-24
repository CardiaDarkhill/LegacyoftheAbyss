using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004F8 RID: 1272
public class HealthCocoon : MonoBehaviour, IHitResponder
{
	// Token: 0x06002D88 RID: 11656 RVA: 0x000C6C48 File Offset: 0x000C4E48
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = this.activated;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				this.activated = value;
				if (this.activated)
				{
					this.SetBroken();
				}
			};
		}
	}

	// Token: 0x06002D89 RID: 11657 RVA: 0x000C6CA0 File Offset: 0x000C4EA0
	private void Start()
	{
		this.animRoutine = base.StartCoroutine(this.Animate());
		HealthCocoon.FlingPrefab[] array = this.flingPrefabs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetupPool(base.transform);
		}
	}

	// Token: 0x06002D8A RID: 11658 RVA: 0x000C6CE2 File Offset: 0x000C4EE2
	private void PlaySound(AudioClip clip)
	{
		if (this.source && clip)
		{
			this.source.PlayOneShot(clip);
		}
	}

	// Token: 0x06002D8B RID: 11659 RVA: 0x000C6D05 File Offset: 0x000C4F05
	private IEnumerator Animate()
	{
		for (;;)
		{
			yield return new WaitForSeconds(Random.Range(this.waitMin, this.waitMax));
			this.PlaySound(this.moveSound);
			if (this.animator)
			{
				tk2dSpriteAnimationClip clipByName = this.animator.GetClipByName(this.sweatAnimation);
				this.animator.Play(clipByName);
				yield return new WaitForSeconds((float)clipByName.frames.Length / clipByName.fps);
				this.animator.Play(this.idleAnimation);
			}
		}
		yield break;
	}

	// Token: 0x06002D8C RID: 11660 RVA: 0x000C6D14 File Offset: 0x000C4F14
	private void SetBroken()
	{
		base.StopCoroutine(this.animRoutine);
		base.GetComponent<MeshRenderer>().enabled = false;
		GameObject[] array = this.disableChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		Collider2D[] array2 = this.disableColliders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].enabled = false;
		}
	}

	// Token: 0x06002D8D RID: 11661 RVA: 0x000C6D78 File Offset: 0x000C4F78
	private void FlingObjects(HealthCocoon.FlingPrefab fling)
	{
		if (fling.Prefab)
		{
			int num = Random.Range(fling.MinAmount, fling.MaxAmount + 1);
			for (int i = 1; i <= num; i++)
			{
				GameObject gameObject = fling.Spawn();
				gameObject.transform.position += new Vector3(fling.OriginVariation.x * Random.Range(-1f, 1f), fling.OriginVariation.y * Random.Range(-1f, 1f));
				float num2 = Random.Range(fling.MinSpeed, fling.MaxSpeed);
				float num3 = Random.Range(fling.MinAngle, fling.MaxAngle);
				float x = num2 * Mathf.Cos(num3 * 0.017453292f);
				float y = num2 * Mathf.Sin(num3 * 0.017453292f);
				Vector2 linearVelocity;
				linearVelocity.x = x;
				linearVelocity.y = y;
				Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
				if (component)
				{
					component.linearVelocity = linearVelocity;
				}
			}
		}
	}

	// Token: 0x06002D8E RID: 11662 RVA: 0x000C6E80 File Offset: 0x000C5080
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.activated)
		{
			return IHitResponder.Response.None;
		}
		bool flag = false;
		if (damageInstance.IsNailDamage)
		{
			flag = true;
			float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			float z = 0f;
			Vector2 v = new Vector2(1.5f, 1.5f);
			if (overriddenDirection < 45f)
			{
				z = (float)Random.Range(340, 380);
			}
			else if (overriddenDirection < 135f)
			{
				z = (float)Random.Range(340, 380);
			}
			else if (overriddenDirection < 225f)
			{
				v.x *= -1f;
				z = (float)Random.Range(70, 110);
			}
			else if (overriddenDirection < 360f)
			{
				z = (float)Random.Range(250, 290);
			}
			GameObject[] array = this.slashEffects;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i].Spawn(base.transform.position + this.effectOrigin);
				gameObject.transform.eulerAngles = new Vector3(0f, 0f, z);
				gameObject.transform.localScale = v;
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Spell)
		{
			flag = true;
			GameObject[] array = this.spellEffects;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject2 = array[i].Spawn(base.transform.position + this.effectOrigin);
				Vector3 position = gameObject2.transform.position;
				position.z = 0.0031f;
				gameObject2.transform.position = position;
			}
		}
		if (flag)
		{
			this.activated = true;
			GameObject[] array = this.enableChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (this.cap)
			{
				this.cap.gameObject.SetActive(true);
				Vector2 hitDirectionAsVector = damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular);
				this.cap.AddForce(this.capHitForce * hitDirectionAsVector, ForceMode2D.Impulse);
			}
			foreach (HealthCocoon.FlingPrefab fling in this.flingPrefabs)
			{
				this.FlingObjects(fling);
			}
			this.PlaySound(this.deathSound);
			this.SetBroken();
			GameManager.instance.AddToCocoonList();
			this.deathCameraShake.DoShake(this, true);
		}
		return flag ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	// Token: 0x04002F48 RID: 12104
	[Header("Behaviour")]
	[SerializeField]
	private GameObject[] slashEffects;

	// Token: 0x04002F49 RID: 12105
	[SerializeField]
	private GameObject[] spellEffects;

	// Token: 0x04002F4A RID: 12106
	[SerializeField]
	private Vector3 effectOrigin = new Vector3(0f, 0.8f, 0f);

	// Token: 0x04002F4B RID: 12107
	[Space]
	[SerializeField]
	private HealthCocoon.FlingPrefab[] flingPrefabs;

	// Token: 0x04002F4C RID: 12108
	[Space]
	[SerializeField]
	private GameObject[] enableChildren;

	// Token: 0x04002F4D RID: 12109
	[SerializeField]
	private GameObject[] disableChildren;

	// Token: 0x04002F4E RID: 12110
	[SerializeField]
	private Collider2D[] disableColliders;

	// Token: 0x04002F4F RID: 12111
	[Space]
	[SerializeField]
	private Rigidbody2D cap;

	// Token: 0x04002F50 RID: 12112
	[SerializeField]
	private float capHitForce = 10f;

	// Token: 0x04002F51 RID: 12113
	[Space]
	[SerializeField]
	private AudioClip deathSound;

	// Token: 0x04002F52 RID: 12114
	[Space]
	[SerializeField]
	private CameraShakeTarget deathCameraShake;

	// Token: 0x04002F53 RID: 12115
	[Header("Animation")]
	[SerializeField]
	private string idleAnimation = "Cocoon Idle";

	// Token: 0x04002F54 RID: 12116
	[SerializeField]
	private string sweatAnimation = "Cocoon Sweat";

	// Token: 0x04002F55 RID: 12117
	[SerializeField]
	private AudioClip moveSound;

	// Token: 0x04002F56 RID: 12118
	[SerializeField]
	private float waitMin = 2f;

	// Token: 0x04002F57 RID: 12119
	[SerializeField]
	private float waitMax = 6f;

	// Token: 0x04002F58 RID: 12120
	private bool activated;

	// Token: 0x04002F59 RID: 12121
	private Coroutine animRoutine;

	// Token: 0x04002F5A RID: 12122
	private AudioSource source;

	// Token: 0x04002F5B RID: 12123
	private tk2dSpriteAnimator animator;

	// Token: 0x020017FC RID: 6140
	[Serializable]
	private class FlingPrefab
	{
		// Token: 0x06008F8E RID: 36750 RVA: 0x00291470 File Offset: 0x0028F670
		public void SetupPool(Transform parent)
		{
			if (this.Prefab)
			{
				this.pool.Capacity = this.MaxAmount;
				for (int i = this.pool.Count; i < this.MaxAmount; i++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.Prefab, parent);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.SetActive(false);
					this.pool.Add(gameObject);
				}
			}
		}

		// Token: 0x06008F8F RID: 36751 RVA: 0x002914E8 File Offset: 0x0028F6E8
		public GameObject Spawn()
		{
			foreach (GameObject gameObject in this.pool)
			{
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
					return gameObject;
				}
			}
			return null;
		}

		// Token: 0x0400903F RID: 36927
		public GameObject Prefab;

		// Token: 0x04009040 RID: 36928
		public int MinAmount;

		// Token: 0x04009041 RID: 36929
		public int MaxAmount;

		// Token: 0x04009042 RID: 36930
		public Vector2 OriginVariation = new Vector2(0.5f, 0.5f);

		// Token: 0x04009043 RID: 36931
		public float MinSpeed;

		// Token: 0x04009044 RID: 36932
		public float MaxSpeed;

		// Token: 0x04009045 RID: 36933
		public float MinAngle;

		// Token: 0x04009046 RID: 36934
		public float MaxAngle;

		// Token: 0x04009047 RID: 36935
		private List<GameObject> pool = new List<GameObject>();
	}
}
