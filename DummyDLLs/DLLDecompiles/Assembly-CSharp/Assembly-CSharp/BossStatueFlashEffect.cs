using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200039D RID: 925
public class BossStatueFlashEffect : MonoBehaviour
{
	// Token: 0x1400005B RID: 91
	// (add) Token: 0x06001F36 RID: 7990 RVA: 0x0008EBF4 File Offset: 0x0008CDF4
	// (remove) Token: 0x06001F37 RID: 7991 RVA: 0x0008EC2C File Offset: 0x0008CE2C
	public event BossStatueFlashEffect.FlashCompleteDelegate OnFlashBegin;

	// Token: 0x06001F38 RID: 7992 RVA: 0x0008EC61 File Offset: 0x0008CE61
	private void Awake()
	{
		this.parentStatue = base.GetComponentInParent<BossStatue>();
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x0008EC7C File Offset: 0x0008CE7C
	private void Start()
	{
		if (this.templateSprite)
		{
			this.templateSprite.transform.localPosition += new Vector3(0f, -2000f, 0f);
			this.mat = new Material(this.templateSprite.sharedMaterial);
		}
		if (!this.parentStatue.StatueState.hasBeenSeen && !this.parentStatue.isAlwaysUnlocked)
		{
			if (this.statueSpritesParent)
			{
				this.statueSprites = this.statueSpritesParent.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer spriteRenderer in this.statueSprites)
				{
					spriteRenderer.color = Color.clear;
					spriteRenderer.sharedMaterial = this.mat;
				}
			}
			if (this.triggerEvent)
			{
				TriggerEnterEvent.CollisionEvent temp = null;
				temp = delegate(Collider2D collider, GameObject sender)
				{
					this.gameObject.SetActive(true);
					this.statueSpritesParent.SetActive(true);
					if (this.inspect)
					{
						this.inspect.SetActive(false);
					}
					this.StartCoroutine(this.FlashRoutine());
					this.triggerEvent.OnTriggerEntered -= temp;
				};
				this.triggerEvent.OnTriggerEntered += temp;
			}
		}
		this.propBlock = new MaterialPropertyBlock();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x0008EDA7 File Offset: 0x0008CFA7
	private void OnDestroy()
	{
		if (this.mat != null)
		{
			Object.Destroy(this.mat);
			this.mat = null;
		}
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x0008EDC9 File Offset: 0x0008CFC9
	private IEnumerator FlashRoutine()
	{
		if (this.OnFlashBegin != null)
		{
			this.OnFlashBegin();
		}
		this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		float duration = this.animator.GetCurrentAnimatorStateInfo(0).length;
		for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
		{
			SpriteRenderer[] array = this.statueSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = this.templateSprite.color;
			}
			this.templateSprite.GetPropertyBlock(this.propBlock);
			this.mat.SetFloat("_FlashAmount", this.propBlock.GetFloat("_FlashAmount"));
			yield return null;
		}
		yield return null;
		this.animator.enabled = false;
		SpriteFadeMaterial component = this.statueSpritesParent.GetComponent<SpriteFadeMaterial>();
		if (component)
		{
			component.FadeBack();
		}
		yield break;
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x0008EDD8 File Offset: 0x0008CFD8
	public void FlashApex()
	{
		if (this.inspect)
		{
			this.inspect.SetActive(true);
		}
		this.parentStatue.SetPlaquesVisible(true);
	}

	// Token: 0x04001E26 RID: 7718
	public SpriteRenderer templateSprite;

	// Token: 0x04001E27 RID: 7719
	public GameObject statueSpritesParent;

	// Token: 0x04001E28 RID: 7720
	private SpriteRenderer[] statueSprites;

	// Token: 0x04001E29 RID: 7721
	public GameObject inspect;

	// Token: 0x04001E2A RID: 7722
	public TriggerEnterEvent triggerEvent;

	// Token: 0x04001E2B RID: 7723
	private BossStatue parentStatue;

	// Token: 0x04001E2C RID: 7724
	private Animator animator;

	// Token: 0x04001E2D RID: 7725
	private Material mat;

	// Token: 0x04001E2E RID: 7726
	private MaterialPropertyBlock propBlock;

	// Token: 0x02001645 RID: 5701
	// (Invoke) Token: 0x06008980 RID: 35200
	public delegate void FlashCompleteDelegate();
}
