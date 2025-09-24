using System;
using UnityEngine;

// Token: 0x02000352 RID: 850
public sealed class HeroFallParticle : MonoBehaviour
{
	// Token: 0x06001D82 RID: 7554 RVA: 0x00088563 File Offset: 0x00086763
	private void Start()
	{
		this.hc = HeroController.instance;
		this.hasHC = (this.hc != null);
		base.enabled = this.hasHC;
	}

	// Token: 0x06001D83 RID: 7555 RVA: 0x00088590 File Offset: 0x00086790
	private void LateUpdate()
	{
		bool flag = this.hc.current_velocity.y < this.hc.MAX_FALL_VELOCITY * 0.75f;
		if (flag != this.isOn)
		{
			this.isOn = flag;
			if (this.isOn)
			{
				this.animator.Play(this.onAnimation);
				return;
			}
			this.animator.Play(this.offAnimation);
		}
	}

	// Token: 0x04001CB9 RID: 7353
	[SerializeField]
	private Animator animator;

	// Token: 0x04001CBA RID: 7354
	[SerializeField]
	private AnimatorHashCache offAnimation = new AnimatorHashCache("Off");

	// Token: 0x04001CBB RID: 7355
	[SerializeField]
	private AnimatorHashCache onAnimation = new AnimatorHashCache("On");

	// Token: 0x04001CBC RID: 7356
	private HeroController hc;

	// Token: 0x04001CBD RID: 7357
	private bool hasHC;

	// Token: 0x04001CBE RID: 7358
	private float threshold;

	// Token: 0x04001CBF RID: 7359
	private bool isOn;
}
