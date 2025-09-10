using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200056B RID: 1387
public class SuspendedSwayPlat : SuspendedPlatformBase
{
	// Token: 0x06003193 RID: 12691 RVA: 0x000DC249 File Offset: 0x000DA449
	private void OnValidate()
	{
		if (this.fallLerpAcceleration < 0f)
		{
			this.fallLerpAcceleration = 0f;
		}
	}

	// Token: 0x06003194 RID: 12692 RVA: 0x000DC264 File Offset: 0x000DA464
	protected override void Awake()
	{
		if (this.hangingParent)
		{
			this.hangingParent.SetActive(true);
		}
		if (this.fallingParent)
		{
			this.fallingParent.SetActive(false);
		}
		if (this.landedParent)
		{
			this.landedParent.SetActive(false);
		}
		this.landEnable.SetAllActive(false);
		this.fallEnable.SetAllActive(false);
		this.landedEnable.SetAllActive(false);
		base.Awake();
		if (this.landDetector)
		{
			this.landDetector.CollisionEntered += delegate(Collision2D collision)
			{
				if (this.activated)
				{
					return;
				}
				if (collision.gameObject.layer != 9)
				{
					return;
				}
				if (collision.GetSafeContact().Normal.y < 0f)
				{
					base.StartCoroutine(this.BreakDrop(this.landBreakDelay));
				}
			};
		}
	}

	// Token: 0x06003195 RID: 12693 RVA: 0x000DC30A File Offset: 0x000DA50A
	public override void CutDown()
	{
		if (this.activated)
		{
			return;
		}
		base.CutDown();
		base.StartCoroutine(this.BreakDrop(0f));
	}

	// Token: 0x06003196 RID: 12694 RVA: 0x000DC32D File Offset: 0x000DA52D
	protected override void OnStartActivated()
	{
		base.OnStartActivated();
		if (this.hangingParent)
		{
			this.hangingParent.SetActive(false);
		}
		if (this.landedParent)
		{
			this.landedParent.SetActive(true);
		}
	}

	// Token: 0x06003197 RID: 12695 RVA: 0x000DC367 File Offset: 0x000DA567
	private IEnumerator BreakDrop(float delay)
	{
		this.activated = true;
		this.landShake.DoShake(this, true);
		this.landOnAudio.SpawnAndPlayOneShot(base.transform.position, null);
		this.landEnable.SetAllActive(true);
		if (delay > 0f)
		{
			if (this.landBreakJitter)
			{
				this.landBreakJitter.StartJitter();
			}
			yield return new WaitForSeconds(delay);
			if (this.landBreakJitter)
			{
				this.landBreakJitter.StopJitter();
			}
		}
		if (this.hangingParent)
		{
			this.hangingParent.SetActive(false);
		}
		if (this.fallingParent)
		{
			if (this.hangingParent)
			{
				this.fallingParent.transform.SetPosition2D(this.hangingParent.transform.position);
			}
			this.fallingParent.SetActive(true);
			this.fallEnable.SetAllActive(true);
			this.fallShake.DoShake(this, true);
			this.fallAudio.SpawnAndPlayOneShot(base.transform.position, null);
			if (this.landedParent)
			{
				Vector2 initialPos = this.fallingParent.transform.position;
				Quaternion initialRotation = this.fallingParent.transform.rotation;
				Vector2 targetPos = this.landedParent.transform.position;
				Quaternion targetRotation = this.landedParent.transform.rotation;
				Vector2 normalized = (targetPos - initialPos).normalized;
				float num = Vector2.Distance(targetPos, initialPos);
				float speed = 0f;
				float speedMultiplier = 1f / num;
				float t = 0f;
				while (t <= 1f)
				{
					speed += this.fallLerpAcceleration * Time.deltaTime;
					if (this.fallLerpMaxSpeed > 0f && speed > this.fallLerpMaxSpeed)
					{
						speed = this.fallLerpMaxSpeed;
					}
					t += speed * speedMultiplier * Time.deltaTime;
					Vector2 position = Vector2.Lerp(initialPos, targetPos, t);
					Quaternion rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
					this.fallingParent.transform.SetPosition2D(position);
					this.fallingParent.transform.rotation = rotation;
					yield return null;
				}
				this.fallingParent.transform.SetPosition2D(targetPos);
				this.fallingParent.transform.rotation = targetRotation;
				initialPos = default(Vector2);
				initialRotation = default(Quaternion);
				targetPos = default(Vector2);
				targetRotation = default(Quaternion);
			}
			this.fallingParent.SetActive(false);
		}
		if (this.landedParent)
		{
			this.landedParent.SetActive(true);
			this.impactAudio.SpawnAndPlayOneShot(this.landedParent.transform.position, null);
		}
		else
		{
			this.impactAudio.SpawnAndPlayOneShot(base.transform.position, null);
		}
		this.landedEnable.SetAllActive(true);
		this.impactShake.DoShake(this, true);
		yield break;
	}

	// Token: 0x040034F2 RID: 13554
	[SerializeField]
	private GameObject hangingParent;

	// Token: 0x040034F3 RID: 13555
	[SerializeField]
	private CollisionEnterEvent landDetector;

	// Token: 0x040034F4 RID: 13556
	[SerializeField]
	private float landBreakDelay;

	// Token: 0x040034F5 RID: 13557
	[SerializeField]
	private JitterSelf landBreakJitter;

	// Token: 0x040034F6 RID: 13558
	[SerializeField]
	private GameObject[] landEnable;

	// Token: 0x040034F7 RID: 13559
	[SerializeField]
	private CameraShakeTarget landShake;

	// Token: 0x040034F8 RID: 13560
	[SerializeField]
	private AudioEvent landOnAudio;

	// Token: 0x040034F9 RID: 13561
	[Space]
	[SerializeField]
	private GameObject fallingParent;

	// Token: 0x040034FA RID: 13562
	[SerializeField]
	private CameraShakeTarget fallShake;

	// Token: 0x040034FB RID: 13563
	[SerializeField]
	private AudioEvent fallAudio;

	// Token: 0x040034FC RID: 13564
	[SerializeField]
	private float fallLerpAcceleration;

	// Token: 0x040034FD RID: 13565
	[SerializeField]
	private float fallLerpMaxSpeed;

	// Token: 0x040034FE RID: 13566
	[SerializeField]
	private GameObject[] fallEnable;

	// Token: 0x040034FF RID: 13567
	[Space]
	[SerializeField]
	private GameObject landedParent;

	// Token: 0x04003500 RID: 13568
	[SerializeField]
	private GameObject[] landedEnable;

	// Token: 0x04003501 RID: 13569
	[SerializeField]
	private CameraShakeTarget impactShake;

	// Token: 0x04003502 RID: 13570
	[SerializeField]
	private AudioEvent impactAudio;
}
