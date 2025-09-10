using System;
using UnityEngine;

// Token: 0x02000240 RID: 576
public class GroundSplat : MonoBehaviour
{
	// Token: 0x06001516 RID: 5398 RVA: 0x0005F7F8 File Offset: 0x0005D9F8
	private void Awake()
	{
		if (this.collisionDetector)
		{
			this.collisionDetector.CollisionEnteredDirectional += this.OnCollisionEnteredDirectional;
		}
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x0005F81E File Offset: 0x0005DA1E
	private void OnEnable()
	{
		this.isActive = true;
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x0005F828 File Offset: 0x0005DA28
	private void OnCollisionEnteredDirectional(CollisionEnterEvent.Direction direction, Collision2D collision)
	{
		if (!this.isActive)
		{
			return;
		}
		Vector3 position = base.transform.position;
		BloodSpawner.SpawnBlood(this.blood, position);
		if (this.animator)
		{
			this.animator.Play(GroundSplat._splatAnim, 0, 0f);
		}
		this.splatSound.SpawnAndPlayOneShot(position, null);
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x0005F888 File Offset: 0x0005DA88
	public void Stop()
	{
		this.isActive = false;
	}

	// Token: 0x040013A2 RID: 5026
	private static readonly int _splatAnim = Animator.StringToHash("Splat");

	// Token: 0x040013A3 RID: 5027
	[SerializeField]
	private CollisionEnterEvent collisionDetector;

	// Token: 0x040013A4 RID: 5028
	[SerializeField]
	private BloodSpawner.GeneralConfig blood;

	// Token: 0x040013A5 RID: 5029
	[SerializeField]
	private Animator animator;

	// Token: 0x040013A6 RID: 5030
	[SerializeField]
	private AudioEventRandom splatSound;

	// Token: 0x040013A7 RID: 5031
	private bool isActive;
}
