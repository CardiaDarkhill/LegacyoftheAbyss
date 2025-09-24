using System;
using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200025F RID: 607
public abstract class PoisonTintBase : MonoBehaviour
{
	// Token: 0x17000251 RID: 593
	// (get) Token: 0x060015CA RID: 5578
	// (set) Token: 0x060015CB RID: 5579
	protected abstract Color Colour { get; set; }

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x060015CC RID: 5580 RVA: 0x0006253F File Offset: 0x0006073F
	private bool WillReadFromTool
	{
		get
		{
			return !this.doRecolour && !this.doHueShift;
		}
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x060015CD RID: 5581 RVA: 0x00062554 File Offset: 0x00060754
	// (set) Token: 0x060015CE RID: 5582 RVA: 0x0006255C File Offset: 0x0006075C
	public ToolItem ReadFromTool
	{
		get
		{
			return this.readFromTool;
		}
		set
		{
			this.readFromTool = value;
			this.UpdatePoison();
		}
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x0006256B File Offset: 0x0006076B
	protected virtual void Awake()
	{
		this.defaultColor = this.Colour;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x00062579 File Offset: 0x00060779
	private void OnEnable()
	{
		this.UpdatePoison();
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x00062584 File Offset: 0x00060784
	private void UpdatePoison()
	{
		if (this.isStateOverridden)
		{
			this.UpdatePoison(this.isPoisonOverride);
			return;
		}
		bool isEquipped = Gameplay.PoisonPouchTool.IsEquipped;
		this.UpdatePoison(isEquipped);
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x000625B8 File Offset: 0x000607B8
	private void UpdatePoison(bool isPoison)
	{
		if (isPoison)
		{
			if (this.doRecolour)
			{
				this.EnableKeyword("RECOLOUR");
				this.DisableKeyword("CAN_HUESHIFT");
				this.Colour = Gameplay.PoisonPouchTintColour;
			}
			else if (this.doHueShift)
			{
				this.DisableKeyword("RECOLOUR");
				this.EnableKeyword("CAN_HUESHIFT");
				this.SetFloat(PoisonTintBase.HueShiftPropId, this.hueShift);
				this.Colour = this.defaultColor;
			}
			else if (this.readFromTool)
			{
				if (this.readFromTool.UsePoisonTintRecolour)
				{
					this.EnableKeyword("RECOLOUR");
					this.DisableKeyword("CAN_HUESHIFT");
					this.Colour = Gameplay.PoisonPouchTintColour;
				}
				else
				{
					this.DisableKeyword("RECOLOUR");
					this.EnableKeyword("CAN_HUESHIFT");
					this.SetFloat(PoisonTintBase.HueShiftPropId, this.readFromTool.PoisonHueShift);
					this.Colour = this.defaultColor;
				}
			}
			this.OnIsPoison.Invoke();
			return;
		}
		this.DisableKeyword("RECOLOUR");
		this.DisableKeyword("CAN_HUESHIFT");
		this.Colour = this.defaultColor;
		this.OnIsNotPoison.Invoke();
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x000626E4 File Offset: 0x000608E4
	public void Clear()
	{
		this.isStateOverridden = false;
		this.isPoisonOverride = false;
		this.UpdatePoison(false);
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x000626FB File Offset: 0x000608FB
	public void SetPoisoned(bool isPoison)
	{
		this.isStateOverridden = true;
		this.isPoisonOverride = isPoison;
		this.UpdatePoison(isPoison);
	}

	// Token: 0x060015D5 RID: 5589
	protected abstract void EnableKeyword(string keyword);

	// Token: 0x060015D6 RID: 5590
	protected abstract void DisableKeyword(string keyword);

	// Token: 0x060015D7 RID: 5591
	protected abstract void SetFloat(int propId, float value);

	// Token: 0x04001464 RID: 5220
	[SerializeField]
	private bool doRecolour;

	// Token: 0x04001465 RID: 5221
	[SerializeField]
	[ModifiableProperty]
	[Conditional("doRecolour", false, false, false)]
	private bool doHueShift;

	// Token: 0x04001466 RID: 5222
	[SerializeField]
	[Range(-1f, 1f)]
	[ModifiableProperty]
	[Conditional("doRecolour", false, false, false)]
	private float hueShift;

	// Token: 0x04001467 RID: 5223
	[SerializeField]
	[ModifiableProperty]
	[Conditional("WillReadFromTool", true, false, false)]
	private ToolItem readFromTool;

	// Token: 0x04001468 RID: 5224
	public UnityEvent OnIsPoison;

	// Token: 0x04001469 RID: 5225
	public UnityEvent OnIsNotPoison;

	// Token: 0x0400146A RID: 5226
	private Color defaultColor;

	// Token: 0x0400146B RID: 5227
	private bool isStateOverridden;

	// Token: 0x0400146C RID: 5228
	private bool isPoisonOverride;

	// Token: 0x0400146D RID: 5229
	public static readonly int HueShiftPropId = Shader.PropertyToID("_HueShift");
}
