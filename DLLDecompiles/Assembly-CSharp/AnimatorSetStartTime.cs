using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class AnimatorSetStartTime : MonoBehaviour
{
	// Token: 0x06000299 RID: 665 RVA: 0x0000EF66 File Offset: 0x0000D166
	private void Reset()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0000EF74 File Offset: 0x0000D174
	private void OnEnable()
	{
		if (!this.onEnable)
		{
			return;
		}
		this.SetTime();
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0000EF85 File Offset: 0x0000D185
	private void Start()
	{
		this.OnEnable();
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0000EF90 File Offset: 0x0000D190
	public void SetTime()
	{
		if (!this.animator)
		{
			return;
		}
		this.animator.Play(this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, this.time);
	}

	// Token: 0x0400023B RID: 571
	[SerializeField]
	[InspectorValidation]
	private Animator animator;

	// Token: 0x0400023C RID: 572
	[Space]
	[SerializeField]
	private bool onEnable;

	// Token: 0x0400023D RID: 573
	[SerializeField]
	[Range(-1f, 1f)]
	private float time;
}
