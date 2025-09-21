using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004F7 RID: 1271
public class HarpoonRingSlider : MonoBehaviour
{
	// Token: 0x06002D7C RID: 11644 RVA: 0x000C680C File Offset: 0x000C4A0C
	private void Awake()
	{
		this.spawnedRing = Object.Instantiate<GameObject>(this.ringPrefab, this.ring.parent).transform;
		this.spawnedRing.localPosition = this.ring.localPosition;
		this.spawnedRing.SetPositionZ(this.ringPrefab.transform.position.z);
		Transform transform = this.spawnedRing.Find("Backing");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
		this.spawnedRingCollider = this.spawnedRing.GetComponent<Collider2D>();
		while (this.ring.childCount > 0)
		{
			this.ring.GetChild(0).SetParent(this.spawnedRing, true);
		}
		this.ring.gameObject.SetActive(false);
		this.ringTarget.gameObject.SetActive(false);
		if (this.camLockOnRing)
		{
			this.camLockOnRing.SetActive(false);
		}
		if (this.activeMovingDown)
		{
			this.activeMovingDown.SetActive(false);
		}
		if (this.activeMovingUp)
		{
			this.activeMovingUp.SetActive(false);
		}
	}

	// Token: 0x06002D7D RID: 11645 RVA: 0x000C693B File Offset: 0x000C4B3B
	private void Start()
	{
		if (!this.isRingActivated)
		{
			this.spawnedRingCollider.enabled = false;
			this.spawnedRing.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmBool("Collider Start Enabled").Value = false;
		}
	}

	// Token: 0x06002D7E RID: 11646 RVA: 0x000C6974 File Offset: 0x000C4B74
	public void HeroOnRing()
	{
		if (!this.isRingActivated)
		{
			return;
		}
		this.isOnRing = true;
		if (this.camLockOnRing)
		{
			this.camLockOnRing.SetActive(true);
		}
		if (this.isInDelay)
		{
			base.StopCoroutine(this.moveRoutine);
			this.moveRoutine = null;
		}
		if (this.moveRoutine == null)
		{
			this.StartMoveUp();
		}
	}

	// Token: 0x06002D7F RID: 11647 RVA: 0x000C69D4 File Offset: 0x000C4BD4
	private void StartMoveUp()
	{
		if (this.activeMovingDown)
		{
			this.activeMovingDown.SetActive(false);
		}
		if (this.activeMovingUp)
		{
			this.activeMovingUp.SetActive(true);
		}
		this.moveRoutine = base.StartCoroutine(this.MoveRoutine(this.ringTarget.position, this.moveSpeed, this.moveDelay, delegate
		{
			if (this.isOnRing)
			{
				FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "RING DROP IMPACT", false);
			}
			else
			{
				FSMUtility.SendEventToGameObject(this.spawnedRing.gameObject, "IMPACT", false);
			}
			this.Stopped();
			this.impactUpSound.SpawnAndPlayOneShot(this.spawnedRing.transform.position, null);
		}));
	}

	// Token: 0x06002D80 RID: 11648 RVA: 0x000C6A50 File Offset: 0x000C4C50
	public void HeroOffRing()
	{
		if (!this.isRingActivated)
		{
			return;
		}
		this.isOnRing = false;
		if (this.camLockOnRing)
		{
			this.camLockOnRing.SetActive(false);
		}
		if (this.isInDelay)
		{
			base.StopCoroutine(this.moveRoutine);
			this.moveRoutine = null;
		}
		if (this.moveRoutine == null)
		{
			this.StartMoveDown();
		}
	}

	// Token: 0x06002D81 RID: 11649 RVA: 0x000C6AAF File Offset: 0x000C4CAF
	private void Stopped()
	{
		if (this.activeMovingDown)
		{
			this.activeMovingDown.SetActive(false);
		}
		if (this.activeMovingUp)
		{
			this.activeMovingUp.SetActive(false);
		}
	}

	// Token: 0x06002D82 RID: 11650 RVA: 0x000C6AE4 File Offset: 0x000C4CE4
	private void StartMoveDown()
	{
		if (this.activeMovingUp)
		{
			this.activeMovingUp.SetActive(false);
		}
		if (this.activeMovingDown)
		{
			this.activeMovingDown.SetActive(true);
		}
		this.moveRoutine = base.StartCoroutine(this.MoveRoutine(this.ring.position, this.returnSpeed, this.returnDelay, delegate
		{
			FSMUtility.SendEventToGameObject(this.spawnedRing.gameObject, "IMPACT", false);
			this.Stopped();
			this.impactDownSound.SpawnAndPlayOneShot(this.spawnedRing.transform.position, null);
		}));
	}

	// Token: 0x06002D83 RID: 11651 RVA: 0x000C6B5D File Offset: 0x000C4D5D
	private IEnumerator MoveRoutine(Vector2 toPos, float speed, float delay, Action onEnd)
	{
		bool wasOnRing = this.isOnRing;
		this.isInDelay = true;
		yield return new WaitForSeconds(delay);
		this.isInDelay = false;
		Vector2 fromPos = this.spawnedRing.position;
		float num = Vector2.Distance(fromPos, toPos);
		float time = num / speed;
		float speedMultiplier = 0f;
		float elapsed = 0f;
		float unscaledElapsed = 0f;
		while (elapsed < time)
		{
			float t = elapsed / time;
			Vector2 position = Vector2.Lerp(fromPos, toPos, t);
			this.spawnedRing.SetPosition2D(position);
			yield return null;
			elapsed += Time.deltaTime * speedMultiplier;
			unscaledElapsed += Time.deltaTime;
			speedMultiplier = Mathf.Clamp01(unscaledElapsed / this.accelTime);
		}
		this.spawnedRing.SetPosition2D(toPos);
		if (onEnd != null)
		{
			onEnd();
		}
		this.moveRoutine = null;
		if (wasOnRing)
		{
			if (!this.isOnRing)
			{
				this.StartMoveDown();
			}
		}
		else if (this.isOnRing)
		{
			this.StartMoveUp();
		}
		yield break;
	}

	// Token: 0x06002D84 RID: 11652 RVA: 0x000C6B89 File Offset: 0x000C4D89
	public void ActivateRing()
	{
		this.isRingActivated = true;
		this.spawnedRingCollider.enabled = true;
	}

	// Token: 0x04002F35 RID: 12085
	[SerializeField]
	private GameObject ringPrefab;

	// Token: 0x04002F36 RID: 12086
	[SerializeField]
	private Transform ring;

	// Token: 0x04002F37 RID: 12087
	[SerializeField]
	private Transform ringTarget;

	// Token: 0x04002F38 RID: 12088
	[SerializeField]
	private GameObject camLockOnRing;

	// Token: 0x04002F39 RID: 12089
	[Space]
	[SerializeField]
	private GameObject activeMovingUp;

	// Token: 0x04002F3A RID: 12090
	[SerializeField]
	private AudioEvent impactUpSound;

	// Token: 0x04002F3B RID: 12091
	[SerializeField]
	private GameObject activeMovingDown;

	// Token: 0x04002F3C RID: 12092
	[SerializeField]
	private AudioEvent impactDownSound;

	// Token: 0x04002F3D RID: 12093
	[Space]
	[SerializeField]
	private float moveDelay;

	// Token: 0x04002F3E RID: 12094
	[SerializeField]
	private float moveSpeed;

	// Token: 0x04002F3F RID: 12095
	[SerializeField]
	private float returnDelay;

	// Token: 0x04002F40 RID: 12096
	[SerializeField]
	private float returnSpeed;

	// Token: 0x04002F41 RID: 12097
	[SerializeField]
	private float accelTime;

	// Token: 0x04002F42 RID: 12098
	private Transform spawnedRing;

	// Token: 0x04002F43 RID: 12099
	private Collider2D spawnedRingCollider;

	// Token: 0x04002F44 RID: 12100
	private Coroutine moveRoutine;

	// Token: 0x04002F45 RID: 12101
	private bool isRingActivated;

	// Token: 0x04002F46 RID: 12102
	private bool isOnRing;

	// Token: 0x04002F47 RID: 12103
	private bool isInDelay;
}
