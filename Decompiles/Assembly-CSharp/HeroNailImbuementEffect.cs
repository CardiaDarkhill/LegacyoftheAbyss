using System;
using UnityEngine;

// Token: 0x020003B9 RID: 953
public class HeroNailImbuementEffect : MonoBehaviour
{
	// Token: 0x0600200B RID: 8203 RVA: 0x00091CB8 File Offset: 0x0008FEB8
	private void Awake()
	{
		HeroController instance = HeroController.instance;
		this.imbuementControl = instance.GetComponent<HeroNailImbuement>();
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x00091CD8 File Offset: 0x0008FED8
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		bool active = this.imbuementControl.CurrentElement == this.nailElement;
		foreach (GameObject gameObject in this.activateObjects)
		{
			if (gameObject)
			{
				gameObject.SetActive(active);
			}
		}
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x00091D2A File Offset: 0x0008FF2A
	private void Start()
	{
		this.hasStarted = true;
		this.OnEnable();
	}

	// Token: 0x04001F0D RID: 7949
	[SerializeField]
	private NailElements nailElement;

	// Token: 0x04001F0E RID: 7950
	[SerializeField]
	private GameObject[] activateObjects;

	// Token: 0x04001F0F RID: 7951
	private bool hasStarted;

	// Token: 0x04001F10 RID: 7952
	private HeroNailImbuement imbuementControl;
}
