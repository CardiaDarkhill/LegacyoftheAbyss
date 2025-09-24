using System;
using UnityEngine;

// Token: 0x020004F4 RID: 1268
public class HarpoonRingChain : MonoBehaviour
{
	// Token: 0x06002D65 RID: 11621 RVA: 0x000C6176 File Offset: 0x000C4376
	private void OnValidate()
	{
		this.UpdateRingDisplay();
		this.UpdateChainDisplay();
	}

	// Token: 0x06002D66 RID: 11622 RVA: 0x000C6184 File Offset: 0x000C4384
	private void Awake()
	{
		ReplaceWithTemplate[] componentsInChildren = base.GetComponentsInChildren<ReplaceWithTemplate>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Awake();
		}
		if (this.ringBody)
		{
			this.DisableBacking(this.ringBody.transform);
		}
	}

	// Token: 0x06002D67 RID: 11623 RVA: 0x000C61CC File Offset: 0x000C43CC
	private void Start()
	{
		this.UpdateRingDisplay();
		this.UpdateChainDisplay();
		if (!this.isSnowy)
		{
			return;
		}
		IdleForceAnimator component = base.GetComponent<IdleForceAnimator>();
		if (component)
		{
			component.SpeedMultiplier *= 5f;
			Vector2 a = base.transform.position;
			float num = float.MaxValue;
			UmbrellaWindRegion umbrellaWindRegion = null;
			foreach (UmbrellaWindRegion umbrellaWindRegion2 in UmbrellaWindRegion.EnumerateActiveRegions())
			{
				Vector2 b = umbrellaWindRegion2.transform.position;
				float num2 = Vector2.Distance(a, b);
				if (num2 <= num)
				{
					num = num2;
					umbrellaWindRegion = umbrellaWindRegion2;
				}
			}
			if (umbrellaWindRegion != null)
			{
				component.ExtraHorizontalSpeed += umbrellaWindRegion.SpeedX * umbrellaWindRegion.SpeedMultiplier;
			}
		}
	}

	// Token: 0x06002D68 RID: 11624 RVA: 0x000C62B4 File Offset: 0x000C44B4
	private void UpdateRingDisplay()
	{
		if (!this.ringObject)
		{
			return;
		}
		string text = this.isSnowy ? "Idle Snowy" : "Idle";
		tk2dSpriteAnimator componentInChildren = this.ringObject.GetComponentInChildren<tk2dSpriteAnimator>();
		if (!componentInChildren || componentInChildren.DefaultClip.name == text)
		{
			return;
		}
		int clipIdByName = componentInChildren.GetClipIdByName(text);
		if (clipIdByName < 0)
		{
			return;
		}
		componentInChildren.DefaultClipId = clipIdByName;
		tk2dSpriteAnimationClip[] clips = componentInChildren.Library.clips;
		componentInChildren.Sprite.SetSprite(clips[clipIdByName].frames[0].spriteCollection, clips[clipIdByName].frames[0].spriteId);
	}

	// Token: 0x06002D69 RID: 11625 RVA: 0x000C6358 File Offset: 0x000C4558
	private void UpdateChainDisplay()
	{
		if (!this.chainRenderer)
		{
			return;
		}
		Material material = this.isSnowy ? this.chainMatSnowy : this.chainMatDefault;
		if (this.chainRenderer.sharedMaterial == material)
		{
			return;
		}
		this.chainRenderer.sharedMaterial = material;
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x000C63AC File Offset: 0x000C45AC
	private void DisableBacking(Transform parent)
	{
		foreach (object obj in parent)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.name == "Backing")
			{
				transform.gameObject.SetActive(false);
			}
			else
			{
				this.DisableBacking(transform);
			}
		}
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x000C6428 File Offset: 0x000C4628
	public void HeroNeedleConnect()
	{
		if (this.chainInteraction)
		{
			this.chainInteraction.DisableLinks(HeroController.instance.transform);
		}
		this.ApplyImpulse(this.needleConnectImpulse);
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x000C6458 File Offset: 0x000C4658
	public void HeroOnRing()
	{
		this.ApplyImpulse(this.heroAttachImpulse);
	}

	// Token: 0x06002D6D RID: 11629 RVA: 0x000C6466 File Offset: 0x000C4666
	public void HeroOffRing()
	{
		this.ApplyImpulse(this.heroLeaveImpulse);
	}

	// Token: 0x06002D6E RID: 11630 RVA: 0x000C6474 File Offset: 0x000C4674
	public void DamageHit(float hitDirection)
	{
		this.ApplyImpulse(this.needleHitImpulse);
	}

	// Token: 0x06002D6F RID: 11631 RVA: 0x000C6484 File Offset: 0x000C4684
	private void ApplyImpulse(Vector2 impulse)
	{
		if (!this.ringBody)
		{
			return;
		}
		float x = HeroController.instance.transform.position.x;
		float x2 = this.ringBody.position.x;
		if (x > x2)
		{
			impulse.x *= -1f;
		}
		this.ringBody.AddForce(impulse, ForceMode2D.Impulse);
	}

	// Token: 0x04002F16 RID: 12054
	[SerializeField]
	private ChainPushReaction chainInteraction;

	// Token: 0x04002F17 RID: 12055
	[SerializeField]
	private Rigidbody2D ringBody;

	// Token: 0x04002F18 RID: 12056
	[SerializeField]
	private Vector2 needleConnectImpulse;

	// Token: 0x04002F19 RID: 12057
	[SerializeField]
	private Vector2 heroAttachImpulse;

	// Token: 0x04002F1A RID: 12058
	[SerializeField]
	private Vector2 heroLeaveImpulse;

	// Token: 0x04002F1B RID: 12059
	[SerializeField]
	private Vector2 needleHitImpulse;

	// Token: 0x04002F1C RID: 12060
	[Space]
	[SerializeField]
	private bool isSnowy;

	// Token: 0x04002F1D RID: 12061
	[SerializeField]
	private GameObject ringObject;

	// Token: 0x04002F1E RID: 12062
	[SerializeField]
	private MeshRenderer chainRenderer;

	// Token: 0x04002F1F RID: 12063
	[SerializeField]
	private Material chainMatDefault;

	// Token: 0x04002F20 RID: 12064
	[SerializeField]
	private Material chainMatSnowy;
}
