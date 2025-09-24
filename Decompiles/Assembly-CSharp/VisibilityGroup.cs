using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000372 RID: 882
public sealed class VisibilityGroup : VisibilityEvent
{
	// Token: 0x06001E32 RID: 7730 RVA: 0x0008B58A File Offset: 0x0008978A
	private void Awake()
	{
		this.Init(false);
	}

	// Token: 0x06001E33 RID: 7731 RVA: 0x0008B593 File Offset: 0x00089793
	private void OnValidate()
	{
		this.childObjects.Remove(this);
	}

	// Token: 0x06001E34 RID: 7732 RVA: 0x0008B5A4 File Offset: 0x000897A4
	public void Init(bool forced)
	{
		if (this.init && !forced)
		{
			return;
		}
		base.FindParent();
		this.init = true;
		VisibilityGroup.SetupMode setupMode = this.setupMode;
		if (setupMode == VisibilityGroup.SetupMode.Full)
		{
			this.FullSetup();
			return;
		}
		if (setupMode != VisibilityGroup.SetupMode.ChildObjectList)
		{
			return;
		}
		this.ChildSetup();
	}

	// Token: 0x06001E35 RID: 7733 RVA: 0x0008B5E8 File Offset: 0x000897E8
	[ContextMenu("Gather Renderers")]
	private void Gather()
	{
		this.renderers.RemoveAll((Renderer o) => o == null);
		this.renderers = this.renderers.Union(base.GetComponentsInChildren<Renderer>(true)).ToList<Renderer>();
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x0008B640 File Offset: 0x00089840
	[ContextMenu("Add Visibility Objects")]
	private void AddVisibilityObjects()
	{
		this.childObjects.RemoveAll((VisibilityEvent o) => o == null);
		foreach (Renderer renderer in this.renderers)
		{
			VisibilityObject item = renderer.gameObject.AddComponentIfNotPresent<VisibilityObject>();
			if (!this.childObjects.Contains(item))
			{
				this.childObjects.Add(item);
			}
		}
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x0008B6DC File Offset: 0x000898DC
	private void FullSetup()
	{
		this.renderers = base.GetComponentsInChildren<Renderer>(true).ToList<Renderer>();
		foreach (Renderer renderer in this.renderers)
		{
			VisibilityObject visibilityObject = renderer.gameObject.AddComponentIfNotPresent<VisibilityObject>();
			visibilityObject.SetRenderer(renderer);
			this.UnsafeRegisterObject(visibilityObject);
		}
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x0008B754 File Offset: 0x00089954
	private void ChildSetup()
	{
		this.childObjects.RemoveAll((VisibilityEvent o) => o == null || o == this);
		foreach (VisibilityEvent visibilityObject in this.childObjects)
		{
			this.UnsafeRegisterObject(visibilityObject);
		}
	}

	// Token: 0x06001E39 RID: 7737 RVA: 0x0008B7C0 File Offset: 0x000899C0
	private void UpdateVisibility(bool visible)
	{
		if (visible)
		{
			base.IsVisible = visible;
			return;
		}
		base.IsVisible = this.IsAnyVisible();
	}

	// Token: 0x06001E3A RID: 7738 RVA: 0x0008B7DC File Offset: 0x000899DC
	private bool IsAnyVisible()
	{
		using (HashSet<VisibilityEvent>.Enumerator enumerator = this.runtimeVisibilityEvents.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsVisible)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x0008B838 File Offset: 0x00089A38
	public void RegisterObject(VisibilityEvent visibilityObject)
	{
		if (visibilityObject == null)
		{
			return;
		}
		this.UnsafeRegisterObject(visibilityObject);
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x0008B84B File Offset: 0x00089A4B
	public void UnsafeRegisterObject(VisibilityEvent visibilityObject)
	{
		if (visibilityObject == this)
		{
			return;
		}
		if (this.runtimeVisibilityEvents.Add(visibilityObject))
		{
			visibilityObject.OnVisibilityChanged += this.UpdateVisibility;
		}
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x0008B877 File Offset: 0x00089A77
	public void UnregisterObject(VisibilityEvent visibilityObject)
	{
		if (this.runtimeVisibilityEvents.Remove(visibilityObject))
		{
			visibilityObject.OnVisibilityChanged -= this.UpdateVisibility;
		}
	}

	// Token: 0x04001D49 RID: 7497
	[SerializeField]
	private VisibilityGroup.SetupMode setupMode;

	// Token: 0x04001D4A RID: 7498
	[SerializeField]
	private List<Renderer> renderers = new List<Renderer>();

	// Token: 0x04001D4B RID: 7499
	[SerializeField]
	private List<VisibilityEvent> childObjects = new List<VisibilityEvent>();

	// Token: 0x04001D4C RID: 7500
	private readonly HashSet<VisibilityEvent> runtimeVisibilityEvents = new HashSet<VisibilityEvent>();

	// Token: 0x04001D4D RID: 7501
	private bool init;

	// Token: 0x0200161F RID: 5663
	[Serializable]
	private enum SetupMode
	{
		// Token: 0x040089C4 RID: 35268
		Full,
		// Token: 0x040089C5 RID: 35269
		ChildObjectList
	}
}
