using System;
using System.Collections.Generic;
using System.Linq;
using TMProOld;
using UnityEngine;

// Token: 0x02000652 RID: 1618
public class FadeGroup : MonoBehaviour
{
	// Token: 0x17000692 RID: 1682
	// (get) Token: 0x060039F4 RID: 14836 RVA: 0x000FE076 File Offset: 0x000FC276
	private float DeltaTime
	{
		get
		{
			if (!this.isRealtime)
			{
				return Time.deltaTime;
			}
			return Time.unscaledDeltaTime;
		}
	}

	// Token: 0x060039F5 RID: 14837 RVA: 0x000FE08B File Offset: 0x000FC28B
	private void OnEnable()
	{
		if (this.disableRenderersOnEnable)
		{
			this.DisableRenderers();
		}
		if (this.getChildrenAutomatically)
		{
			this.ScanChildren();
		}
	}

	// Token: 0x060039F6 RID: 14838 RVA: 0x000FE0A9 File Offset: 0x000FC2A9
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.useCollider)
		{
			this.FadeUp();
		}
	}

	// Token: 0x060039F7 RID: 14839 RVA: 0x000FE0B9 File Offset: 0x000FC2B9
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.useCollider)
		{
			this.FadeDown();
		}
	}

	// Token: 0x060039F8 RID: 14840 RVA: 0x000FE0CC File Offset: 0x000FC2CC
	private void Update()
	{
		if (this.state != 0)
		{
			float t = 0f;
			if (this.state == 1)
			{
				this.timer += this.DeltaTime;
				if (this.timer > this.fadeInTime)
				{
					this.timer = this.fadeInTime;
					this.state = 0;
					for (int i = 0; i < this.spriteRenderers.Count; i++)
					{
						if (this.spriteRenderers[i] != null)
						{
							Color color = this.spriteRenderers[i].color;
							color.a = this.fullAlpha;
							this.spriteRenderers[i].color = color;
						}
					}
					for (int j = 0; j < this.texts.Count; j++)
					{
						if (this.texts[j] != null)
						{
							Color color2 = this.texts[j].color;
							color2.a = this.fullAlpha;
							this.texts[j].color = color2;
						}
					}
				}
				t = this.timer / this.fadeInTime;
			}
			else if (this.state == 2)
			{
				this.timer -= this.DeltaTime;
				if (this.timer < 0f)
				{
					this.timer = 0f;
					this.state = 0;
					if (this.downAlpha > 0f)
					{
						for (int k = 0; k < this.spriteRenderers.Count; k++)
						{
							if (this.spriteRenderers[k] != null)
							{
								Color color3 = this.spriteRenderers[k].color;
								color3.a = this.downAlpha;
								this.spriteRenderers[k].color = color3;
							}
						}
						for (int l = 0; l < this.texts.Count; l++)
						{
							if (this.texts[l] != null)
							{
								Color color4 = this.texts[l].color;
								color4.a = this.downAlpha;
								this.texts[l].color = color4;
							}
						}
					}
					else
					{
						this.DisableRenderers();
					}
				}
				t = this.timer / this.fadeOutTime;
			}
			if (this.state != 0)
			{
				this.currentAlpha = Mathf.Lerp(this.downAlpha, this.fullAlpha, t);
				for (int m = 0; m < this.spriteRenderers.Count; m++)
				{
					if (this.spriteRenderers[m] != null)
					{
						Color color5 = this.spriteRenderers[m].color;
						color5.a = this.currentAlpha;
						this.spriteRenderers[m].color = color5;
					}
				}
				for (int n = 0; n < this.texts.Count; n++)
				{
					if (this.texts[n] != null)
					{
						Color color6 = this.texts[n].color;
						color6.a = this.currentAlpha;
						this.texts[n].color = color6;
					}
				}
			}
		}
	}

	// Token: 0x060039F9 RID: 14841 RVA: 0x000FE414 File Offset: 0x000FC614
	public void FadeUp()
	{
		this.timer = 0f;
		this.state = 1;
		for (int i = 0; i < this.spriteRenderers.Count; i++)
		{
			if (this.spriteRenderers[i] != null)
			{
				Color color = this.spriteRenderers[i].color;
				color.a = 0f;
				this.spriteRenderers[i].color = color;
				this.spriteRenderers[i].enabled = true;
			}
		}
		for (int j = 0; j < this.texts.Count; j++)
		{
			if (this.texts[j] != null)
			{
				Color color2 = this.texts[j].color;
				color2.a = 0f;
				this.texts[j].color = color2;
				this.texts[j].gameObject.GetComponent<MeshRenderer>().SetActiveWithChildren(true);
			}
		}
		for (int k = 0; k < this.animators.Count; k++)
		{
			if (this.animators[k] != null)
			{
				this.animators[k].AnimateUp();
			}
		}
	}

	// Token: 0x060039FA RID: 14842 RVA: 0x000FE558 File Offset: 0x000FC758
	public void FadeDown()
	{
		this.timer = this.fadeOutTime;
		this.state = 2;
		for (int i = 0; i < this.animators.Count; i++)
		{
			if (this.animators[i] != null)
			{
				this.animators[i].AnimateDown();
			}
		}
	}

	// Token: 0x060039FB RID: 14843 RVA: 0x000FE5B4 File Offset: 0x000FC7B4
	public void FadeDownFast()
	{
		this.timer = this.fadeOutTimeFast;
		this.state = 2;
		for (int i = 0; i < this.animators.Count; i++)
		{
			if (this.animators[i] != null)
			{
				this.animators[i].AnimateDown();
			}
		}
	}

	// Token: 0x060039FC RID: 14844 RVA: 0x000FE610 File Offset: 0x000FC810
	private void DisableRenderers()
	{
		for (int i = 0; i < this.spriteRenderers.Count; i++)
		{
			if (this.spriteRenderers[i] != null)
			{
				this.spriteRenderers[i].enabled = false;
			}
		}
		for (int j = 0; j < this.texts.Count; j++)
		{
			if (this.texts[j] != null)
			{
				Color color = this.texts[j].color;
				color.a = 0f;
				this.texts[j].color = color;
				this.texts[j].gameObject.GetComponent<MeshRenderer>().SetActiveWithChildren(false);
			}
		}
	}

	// Token: 0x060039FD RID: 14845 RVA: 0x000FE6D0 File Offset: 0x000FC8D0
	[ContextMenu("Clean Up Lists")]
	public void CleanUpLists()
	{
		this.spriteRenderers = (from s in this.spriteRenderers
		where s != null
		select s).ToList<SpriteRenderer>();
		this.texts = (from s in this.texts
		where s != null
		select s).ToList<TextMeshPro>();
		this.animators = (from s in this.animators
		where s != null
		select s).ToList<InvAnimateUpAndDown>();
	}

	// Token: 0x060039FE RID: 14846 RVA: 0x000FE77C File Offset: 0x000FC97C
	[ContextMenu("Add Missing Children")]
	public void ScanChildren()
	{
		this.AddMissingChildrenToList<SpriteRenderer>(ref this.spriteRenderers);
		this.AddMissingChildrenToList<TextMeshPro>(ref this.texts);
		this.AddMissingChildrenToList<InvAnimateUpAndDown>(ref this.animators);
	}

	// Token: 0x060039FF RID: 14847 RVA: 0x000FE7A4 File Offset: 0x000FC9A4
	private void AddMissingChildrenToList<T>(ref List<T> target) where T : Component
	{
		if (target == null)
		{
			target = new List<T>();
		}
		foreach (T t in base.GetComponentsInChildren<T>(true))
		{
			if (!t.GetComponent<ExcludeFromFadeGroup>() && !target.Contains(t))
			{
				target.Add(t);
			}
		}
	}

	// Token: 0x04003C94 RID: 15508
	public List<SpriteRenderer> spriteRenderers;

	// Token: 0x04003C95 RID: 15509
	public List<TextMeshPro> texts;

	// Token: 0x04003C96 RID: 15510
	public List<InvAnimateUpAndDown> animators;

	// Token: 0x04003C97 RID: 15511
	public float fadeInTime = 0.2f;

	// Token: 0x04003C98 RID: 15512
	public float fadeOutTime = 0.2f;

	// Token: 0x04003C99 RID: 15513
	public float fadeOutTimeFast = 0.2f;

	// Token: 0x04003C9A RID: 15514
	public float fullAlpha = 1f;

	// Token: 0x04003C9B RID: 15515
	public float downAlpha;

	// Token: 0x04003C9C RID: 15516
	public bool activateTexts;

	// Token: 0x04003C9D RID: 15517
	private int state;

	// Token: 0x04003C9E RID: 15518
	private float timer;

	// Token: 0x04003C9F RID: 15519
	private Color currentColour;

	// Token: 0x04003CA0 RID: 15520
	private Color fadeOutColour = new Color(1f, 1f, 1f, 0f);

	// Token: 0x04003CA1 RID: 15521
	private Color fadeInColour = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04003CA2 RID: 15522
	private float currentAlpha;

	// Token: 0x04003CA3 RID: 15523
	public bool disableRenderersOnEnable;

	// Token: 0x04003CA4 RID: 15524
	public bool useCollider;

	// Token: 0x04003CA5 RID: 15525
	public bool getChildrenAutomatically;

	// Token: 0x04003CA6 RID: 15526
	public bool isRealtime;
}
