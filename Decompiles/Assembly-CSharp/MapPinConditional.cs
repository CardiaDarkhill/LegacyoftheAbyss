using System;
using UnityEngine;

// Token: 0x020006D6 RID: 1750
public class MapPinConditional : MonoBehaviour
{
	// Token: 0x06003F50 RID: 16208 RVA: 0x001178D7 File Offset: 0x00115AD7
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.initialMaterial = this.spriteRenderer.sharedMaterial;
	}

	// Token: 0x06003F51 RID: 16209 RVA: 0x001178F8 File Offset: 0x00115AF8
	private void OnEnable()
	{
		if (!this.visibleCondition.IsFulfilled)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.spriteRenderer.sharedMaterial = (this.materialCondition.IsFulfilled ? this.material : this.initialMaterial);
	}

	// Token: 0x04004118 RID: 16664
	[SerializeField]
	private MapPinConditional.Condition visibleCondition;

	// Token: 0x04004119 RID: 16665
	[Space]
	[SerializeField]
	private MapPinConditional.Condition materialCondition;

	// Token: 0x0400411A RID: 16666
	[SerializeField]
	private Material material;

	// Token: 0x0400411B RID: 16667
	private SpriteRenderer spriteRenderer;

	// Token: 0x0400411C RID: 16668
	private Material initialMaterial;

	// Token: 0x020019D4 RID: 6612
	[Serializable]
	public struct PersistentBoolMatch
	{
		// Token: 0x0400974C RID: 38732
		public string Id;

		// Token: 0x0400974D RID: 38733
		public string SceneName;

		// Token: 0x0400974E RID: 38734
		public bool ExpectedValue;
	}

	// Token: 0x020019D5 RID: 6613
	[Serializable]
	public class Condition
	{
		// Token: 0x170010CB RID: 4299
		// (get) Token: 0x06009547 RID: 38215 RVA: 0x002A4FB8 File Offset: 0x002A31B8
		public bool IsFulfilled
		{
			get
			{
				PersistentItemData<bool> persistentItemData;
				return this.PlayerDataTest.IsFulfilled && (string.IsNullOrEmpty(this.PersistentBool.Id) || string.IsNullOrEmpty(this.PersistentBool.SceneName) || (SceneData.instance.PersistentBools.TryGetValue(this.PersistentBool.SceneName, this.PersistentBool.Id, out persistentItemData) && persistentItemData.Value) == this.PersistentBool.ExpectedValue);
			}
		}

		// Token: 0x0400974F RID: 38735
		public PlayerDataTest PlayerDataTest;

		// Token: 0x04009750 RID: 38736
		public MapPinConditional.PersistentBoolMatch PersistentBool = new MapPinConditional.PersistentBoolMatch
		{
			ExpectedValue = true
		};
	}
}
