using System;
using UnityEngine;

// Token: 0x0200065A RID: 1626
public class FastTravelMapCorePiece : MonoBehaviour, IFastTravelMapPiece
{
	// Token: 0x17000694 RID: 1684
	// (get) Token: 0x06003A29 RID: 14889 RVA: 0x000FF070 File Offset: 0x000FD270
	public bool IsVisible
	{
		get
		{
			bool flag = false;
			foreach (IFastTravelMapPiece fastTravelMapPiece in this.anyPiece)
			{
				if (fastTravelMapPiece != null)
				{
					flag = true;
					if (fastTravelMapPiece.IsVisible)
					{
						return true;
					}
				}
			}
			return !flag;
		}
	}

	// Token: 0x06003A2A RID: 14890 RVA: 0x000FF0B0 File Offset: 0x000FD2B0
	private void OnValidate()
	{
		if (this.anyPiece == null)
		{
			this.anyPiece = new Object[0];
		}
		for (int i = 0; i < this.anyPiece.Length; i++)
		{
			Object @object = this.anyPiece[i];
			if (!(@object is IFastTravelMapPiece))
			{
				GameObject gameObject = @object as GameObject;
				if (gameObject != null)
				{
					IFastTravelMapPiece component = gameObject.GetComponent<IFastTravelMapPiece>();
					if (component != null)
					{
						this.anyPiece[i] = (component as Component);
						goto IL_5C;
					}
				}
				this.anyPiece[i] = null;
			}
			IL_5C:;
		}
	}

	// Token: 0x06003A2B RID: 14891 RVA: 0x000FF128 File Offset: 0x000FD328
	private void Awake()
	{
		this.OnValidate();
		IFastTravelMap componentInParent = base.GetComponentInParent<IFastTravelMap>();
		if (componentInParent != null)
		{
			componentInParent.Opening += this.OnOpening;
		}
	}

	// Token: 0x06003A2C RID: 14892 RVA: 0x000FF157 File Offset: 0x000FD357
	private void OnOpening()
	{
		base.gameObject.SetActive(this.IsVisible);
	}

	// Token: 0x04003CC2 RID: 15554
	[SerializeField]
	private Object[] anyPiece;
}
