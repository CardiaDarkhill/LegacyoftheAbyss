using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002A4 RID: 676
public class ClockworkHatchlingDummy : MonoBehaviour
{
	// Token: 0x06001803 RID: 6147 RVA: 0x0006DD40 File Offset: 0x0006BF40
	private void OnValidate()
	{
		if (this.sprite == null)
		{
			this.sprite = base.GetComponent<tk2dSprite>();
		}
	}

	// Token: 0x06001804 RID: 6148 RVA: 0x0006DD5C File Offset: 0x0006BF5C
	private void OnEnable()
	{
		this.CheckPoison();
	}

	// Token: 0x06001805 RID: 6149 RVA: 0x0006DD64 File Offset: 0x0006BF64
	private void CheckPoison()
	{
		if (Gameplay.PoisonPouchTool.IsEquipped && !ToolItemManager.IsCustomToolOverride)
		{
			Color poisonPouchTintColour = Gameplay.PoisonPouchTintColour;
			if (this.representingTool)
			{
				this.sprite.EnableKeyword("CAN_HUESHIFT");
				this.sprite.SetFloat(PoisonTintBase.HueShiftPropId, this.representingTool.PoisonHueShift);
			}
			else
			{
				this.sprite.EnableKeyword("RECOLOUR");
				this.sprite.color = poisonPouchTintColour;
			}
			this.ptPoisonTrail.Play();
			return;
		}
		this.sprite.DisableKeyword("CAN_HUESHIFT");
		this.sprite.DisableKeyword("RECOLOUR");
		this.sprite.color = Color.white;
	}

	// Token: 0x040016DA RID: 5850
	[SerializeField]
	private tk2dSprite sprite;

	// Token: 0x040016DB RID: 5851
	[SerializeField]
	private ToolItem representingTool;

	// Token: 0x040016DC RID: 5852
	[SerializeField]
	private ParticleSystem ptPoisonTrail;
}
