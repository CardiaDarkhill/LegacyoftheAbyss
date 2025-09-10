using System;
using UnityEngine;

// Token: 0x0200073C RID: 1852
public class UIAnimationEvents : MonoBehaviour
{
	// Token: 0x06004213 RID: 16915 RVA: 0x00122930 File Offset: 0x00120B30
	public void OnEnable()
	{
		if (!(this.ui == null))
		{
			return;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("UIManager");
		if (gameObject != null)
		{
			this.ui = gameObject.GetComponent<UIManager>();
			return;
		}
		Debug.LogError(base.name + " could not find a UI Manager in this scene");
	}

	// Token: 0x06004214 RID: 16916 RVA: 0x00122982 File Offset: 0x00120B82
	public void OnDisable()
	{
	}

	// Token: 0x06004215 RID: 16917 RVA: 0x00122984 File Offset: 0x00120B84
	private void AnimateIn()
	{
		Debug.Log(base.name + " animate in called.");
		this.animator.ResetTrigger("hide");
		this.animator.SetTrigger("show");
	}

	// Token: 0x06004216 RID: 16918 RVA: 0x001229BB File Offset: 0x00120BBB
	private void AnimateOut()
	{
		Debug.Log(base.name + " animate out called.");
		this.animator.ResetTrigger("show");
		this.animator.SetTrigger("hide");
	}

	// Token: 0x04004390 RID: 17296
	public Animator animator;

	// Token: 0x04004391 RID: 17297
	private UIManager ui;
}
