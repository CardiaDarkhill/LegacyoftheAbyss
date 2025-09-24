using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000793 RID: 1939
public sealed class tk2dSpriteInitialiser : MonoBehaviour, IInitialisable
{
	// Token: 0x06004499 RID: 17561 RVA: 0x0012C5E4 File Offset: 0x0012A7E4
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		if (this.autoFind)
		{
			this.GatherSprites();
		}
		else
		{
			this.sprites.RemoveAll((tk2dSprite o) => o == null);
		}
		foreach (tk2dSprite tk2dSprite in this.sprites)
		{
			if (!tk2dSprite.gameObject.activeInHierarchy)
			{
				tk2dSprite.ForceBuild();
			}
		}
		return true;
	}

	// Token: 0x0600449A RID: 17562 RVA: 0x0012C690 File Offset: 0x0012A890
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x0600449B RID: 17563 RVA: 0x0012C6AB File Offset: 0x0012A8AB
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x0600449C RID: 17564 RVA: 0x0012C6B4 File Offset: 0x0012A8B4
	[ContextMenu("GatherSprites")]
	private void GatherSprites()
	{
		this.sprites.RemoveAll((tk2dSprite o) => o == null);
		this.sprites = this.sprites.Union(base.gameObject.GetComponentsInChildren<tk2dSprite>(true)).ToList<tk2dSprite>();
	}

	// Token: 0x0600449E RID: 17566 RVA: 0x0012C728 File Offset: 0x0012A928
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04004594 RID: 17812
	[SerializeField]
	private bool autoFind = true;

	// Token: 0x04004595 RID: 17813
	[SerializeField]
	private List<tk2dSprite> sprites = new List<tk2dSprite>();

	// Token: 0x04004596 RID: 17814
	private bool hasAwaken;

	// Token: 0x04004597 RID: 17815
	private bool hasStarted;
}
