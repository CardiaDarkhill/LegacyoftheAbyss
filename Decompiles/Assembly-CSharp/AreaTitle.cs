using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000607 RID: 1543
public class AreaTitle : ManagerSingleton<AreaTitle>
{
	// Token: 0x17000655 RID: 1621
	// (get) Token: 0x06003715 RID: 14101 RVA: 0x000F3013 File Offset: 0x000F1213
	// (set) Token: 0x06003716 RID: 14102 RVA: 0x000F301B File Offset: 0x000F121B
	public bool Initialised { get; private set; }

	// Token: 0x06003717 RID: 14103 RVA: 0x000F3024 File Offset: 0x000F1224
	protected override void Awake()
	{
		base.Awake();
		if (ManagerSingleton<AreaTitle>.UnsafeInstance == this)
		{
			PlayMakerGlobals.Instance.Variables.FindFsmGameObject("AreaTitle").Value = base.gameObject;
		}
		this.ensureAwake.RemoveAll((GameObject o) => o == null);
		this.enabledStates = new bool[this.ensureAwake.Count];
		for (int i = 0; i < this.ensureAwake.Count; i++)
		{
			GameObject gameObject = this.ensureAwake[i];
			bool activeSelf = gameObject.activeSelf;
			this.enabledStates[i] = activeSelf;
			if (!activeSelf)
			{
				gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06003718 RID: 14104 RVA: 0x000F30E4 File Offset: 0x000F12E4
	private void Start()
	{
		for (int i = 0; i < this.ensureAwake.Count; i++)
		{
			GameObject gameObject = this.ensureAwake[i];
			if (!this.enabledStates[i])
			{
				gameObject.SetActive(false);
			}
		}
		this.Initialised = true;
		base.gameObject.SetActive(false);
	}

	// Token: 0x040039E7 RID: 14823
	[SerializeField]
	private List<GameObject> ensureAwake = new List<GameObject>();

	// Token: 0x040039E8 RID: 14824
	private bool[] enabledStates;
}
