using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class SpriteExtruder : MonoBehaviour
{
	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000589 RID: 1417 RVA: 0x0001D00F File Offset: 0x0001B20F
	protected float ExtrusionDepth
	{
		get
		{
			return this.extrusionDepth;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x0600058A RID: 1418 RVA: 0x0001D017 File Offset: 0x0001B217
	protected SpriteRenderer OriginalDisplay
	{
		get
		{
			return this.originalDisplay;
		}
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0001D01F File Offset: 0x0001B21F
	private void OnValidate()
	{
		if (this.extrusionLayers < 1)
		{
			this.extrusionLayers = 1;
		}
		if (this.rotationThreshold < 0f)
		{
			this.rotationThreshold = 0f;
		}
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x0001D049 File Offset: 0x0001B249
	protected virtual void Awake()
	{
		this.FullRefresh();
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x0001D051 File Offset: 0x0001B251
	private void Start()
	{
		this.hasStarted = true;
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0001D05A File Offset: 0x0001B25A
	protected virtual void OnEnable()
	{
		this.OnValidate();
		if (!this.hasStarted)
		{
			return;
		}
		if (this.originalDisplay)
		{
			this.originalDisplay.gameObject.SetActive(true);
			this.originalDisplay.transform.Reset();
		}
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x0001D09C File Offset: 0x0001B29C
	private void LateUpdate()
	{
		if (!this.originalDisplay)
		{
			return;
		}
		bool flag;
		if (this.rotationThreshold > 0f)
		{
			Vector3 eulerAngles = this.originalDisplay.transform.eulerAngles;
			flag = (!eulerAngles.x.IsWithinTolerance(this.rotationThreshold, 0f) || !eulerAngles.y.IsWithinTolerance(this.rotationThreshold, 0f));
		}
		else
		{
			flag = true;
		}
		if (flag != this.wasExtrusionVisible)
		{
			foreach (SpriteRenderer spriteRenderer in this.extrusions)
			{
				spriteRenderer.enabled = flag;
			}
			this.wasExtrusionVisible = flag;
		}
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x0001D164 File Offset: 0x0001B364
	private void CreateExtrusions()
	{
		if (!this.originalDisplay)
		{
			return;
		}
		if (this.extrusionDepth == 0f)
		{
			return;
		}
		Sprite sprite = this.originalDisplay.sprite;
		Transform transform = this.originalDisplay.transform;
		int num = this.extrusionLayers - 1;
		for (int i = 0; i < this.extrusionLayers; i++)
		{
			SpriteRenderer component = new GameObject("extrusion", new Type[]
			{
				typeof(SpriteRenderer)
			})
			{
				hideFlags = HideFlags.HideAndDontSave
			}.GetComponent<SpriteRenderer>();
			component.sprite = this.originalDisplay.sprite;
			component.color = ((this.colourBack && i == num) ? this.originalDisplay.color : Color.black);
			component.transform.SetParentReset(transform);
			this.extrusions.Add(component);
		}
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0001D23C File Offset: 0x0001B43C
	private void DestroyExtrusions()
	{
		foreach (SpriteRenderer spriteRenderer in this.extrusions)
		{
			if (spriteRenderer)
			{
				Object.DestroyImmediate(spriteRenderer.gameObject);
			}
		}
		this.extrusions.Clear();
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0001D2A8 File Offset: 0x0001B4A8
	private void LayoutExtrusions()
	{
		float num = (this.extrusionDepth != 0f) ? (this.extrusionDepth / (float)this.extrusionLayers) : 0f;
		for (int i = 0; i < this.extrusions.Count; i++)
		{
			this.extrusions[i].transform.SetLocalPositionZ(num * (float)(i + 1));
		}
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0001D30A File Offset: 0x0001B50A
	private void OnDestroy()
	{
		this.DestroyExtrusions();
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0001D312 File Offset: 0x0001B512
	[ContextMenu("Full Refresh")]
	public void FullRefresh()
	{
		this.DestroyExtrusions();
		this.CreateExtrusions();
		this.LayoutExtrusions();
		this.wasExtrusionVisible = true;
	}

	// Token: 0x040005A3 RID: 1443
	[SerializeField]
	private SpriteRenderer originalDisplay;

	// Token: 0x040005A4 RID: 1444
	[SerializeField]
	private float extrusionDepth;

	// Token: 0x040005A5 RID: 1445
	[SerializeField]
	private int extrusionLayers;

	// Token: 0x040005A6 RID: 1446
	[SerializeField]
	private bool colourBack;

	// Token: 0x040005A7 RID: 1447
	[SerializeField]
	private float rotationThreshold = 1f;

	// Token: 0x040005A8 RID: 1448
	private readonly List<SpriteRenderer> extrusions = new List<SpriteRenderer>();

	// Token: 0x040005A9 RID: 1449
	private bool wasExtrusionVisible;

	// Token: 0x040005AA RID: 1450
	private bool hasStarted;
}
