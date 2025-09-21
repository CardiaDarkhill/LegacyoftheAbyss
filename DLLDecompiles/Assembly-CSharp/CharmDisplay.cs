using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000613 RID: 1555
public class CharmDisplay : MonoBehaviour
{
	// Token: 0x06003777 RID: 14199 RVA: 0x000F49A4 File Offset: 0x000F2BA4
	private void Reset()
	{
		if (!this.spriteRenderer)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
		if (!this.flashSpriteRenderer)
		{
			Transform transform = base.transform.Find("Flash Sprite");
			if (transform)
			{
				this.flashSpriteRenderer = transform.GetComponent<SpriteRenderer>();
			}
		}
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x000F49FC File Offset: 0x000F2BFC
	private void Start()
	{
		Sprite sprite = CharmIconList.Instance.GetSprite(this.id);
		if (this.spriteRenderer)
		{
			this.spriteRenderer.sprite = sprite;
		}
		if (this.flashSpriteRenderer)
		{
			this.flashSpriteRenderer.sprite = sprite;
		}
		this.Check();
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x000F4A54 File Offset: 0x000F2C54
	public void Check()
	{
		if (CharmDisplay.charmsMenuFsm == null)
		{
			GameObject gameObject = GameObject.FindWithTag("Charms Pane");
			if (gameObject)
			{
				CharmDisplay.charmsMenuFsm = PlayMakerFSM.FindFsmOnGameObject(gameObject, "UI Charms");
			}
		}
		if (CharmDisplay.charmsMenuFsm)
		{
			FsmString fsmString = CharmDisplay.charmsMenuFsm.FsmVariables.FindFsmString("Newly Equipped Name");
			if (fsmString != null && fsmString.Value == base.gameObject.name)
			{
				fsmString.Value = "none";
				if (this.charmPlaceEffect)
				{
					this.charmPlaceEffect.Spawn(base.transform.position + new Vector3(0f, 0f, -1f));
					if (this.flashSpriteRenderer)
					{
						this.flashSpriteRenderer.gameObject.SetActive(true);
					}
				}
			}
		}
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x000F4B38 File Offset: 0x000F2D38
	private void FixedUpdate()
	{
		if (this.doJitter)
		{
			base.transform.localPosition = new Vector3(this.startPos.x + Random.Range(-0.075f, 0.075f), this.startPos.y + Random.Range(-0.075f, 0.075f), this.startPos.z);
		}
	}

	// Token: 0x04003A60 RID: 14944
	public int id;

	// Token: 0x04003A61 RID: 14945
	public SpriteRenderer spriteRenderer;

	// Token: 0x04003A62 RID: 14946
	public SpriteRenderer flashSpriteRenderer;

	// Token: 0x04003A63 RID: 14947
	[Space]
	public Sprite brokenGlassHP;

	// Token: 0x04003A64 RID: 14948
	public Sprite brokenGlassGeo;

	// Token: 0x04003A65 RID: 14949
	public Sprite brokenGlassAttack;

	// Token: 0x04003A66 RID: 14950
	public Sprite whiteCharm;

	// Token: 0x04003A67 RID: 14951
	public Sprite blackCharm;

	// Token: 0x04003A68 RID: 14952
	public GameObject charmPlaceEffect;

	// Token: 0x04003A69 RID: 14953
	private static PlayMakerFSM charmsMenuFsm;

	// Token: 0x04003A6A RID: 14954
	private bool doJitter;

	// Token: 0x04003A6B RID: 14955
	private Vector3 startPos;

	// Token: 0x04003A6C RID: 14956
	private const float jitterX = 0.075f;

	// Token: 0x04003A6D RID: 14957
	private const float jitterY = 0.075f;
}
