using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C09 RID: 3081
	[ActionCategory("Hollow Knight")]
	[ActionTarget(typeof(GameObject), "gameObject", true)]
	[Tooltip("Creates a Game Object, usually using a Prefab.")]
	public class CreateUIMsgGetItem : FsmStateAction
	{
		// Token: 0x06005E08 RID: 24072 RVA: 0x001DA2D9 File Offset: 0x001D84D9
		public override void Reset()
		{
			this.gameObject = null;
			this.storeObject = null;
			this.sprite = new FsmObject
			{
				UseVariable = true
			};
		}

		// Token: 0x06005E09 RID: 24073 RVA: 0x001DA2FC File Offset: 0x001D84FC
		public override void Awake()
		{
			GameObject value = this.gameObject.Value;
			if (value != null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(value, Vector3.zero, Quaternion.identity);
				this.storeObject.Value = gameObject;
				Sprite exists = this.sprite.Value as Sprite;
				if (exists)
				{
					Transform transform = gameObject.transform.Find("Icon");
					if (transform)
					{
						SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
						if (component)
						{
							component.sprite = exists;
						}
						this.hasSetSprite = true;
					}
				}
				gameObject.SetActive(false);
			}
		}

		// Token: 0x06005E0A RID: 24074 RVA: 0x001DA394 File Offset: 0x001D8594
		public override void OnEnter()
		{
			if (this.storeObject.Value != null)
			{
				GameObject value = this.storeObject.Value;
				value.SetActive(true);
				if (!this.hasSetSprite)
				{
					Sprite exists = this.sprite.Value as Sprite;
					if (exists)
					{
						Transform transform = value.transform.Find("Icon");
						if (transform)
						{
							SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
							if (component)
							{
								component.sprite = exists;
							}
						}
					}
				}
				EventRegister.GetRegisterGuaranteed(base.Owner, "GET ITEM MSG END");
			}
			else
			{
				GameObject value2 = this.gameObject.Value;
				if (value2 != null)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(value2, Vector3.zero, Quaternion.identity);
					this.storeObject.Value = gameObject;
					Sprite exists2 = this.sprite.Value as Sprite;
					if (exists2)
					{
						Transform transform2 = gameObject.transform.Find("Icon");
						if (transform2)
						{
							SpriteRenderer component2 = transform2.GetComponent<SpriteRenderer>();
							if (component2)
							{
								component2.sprite = exists2;
							}
						}
					}
					EventRegister.GetRegisterGuaranteed(base.Owner, "GET ITEM MSG END");
				}
			}
			base.Finish();
		}

		// Token: 0x04005A6B RID: 23147
		[RequiredField]
		[Tooltip("GameObject to create. Usually a Prefab.")]
		public FsmGameObject gameObject;

		// Token: 0x04005A6C RID: 23148
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally store the created object.")]
		public FsmGameObject storeObject;

		// Token: 0x04005A6D RID: 23149
		[ObjectType(typeof(Sprite))]
		public FsmObject sprite;

		// Token: 0x04005A6E RID: 23150
		private bool hasSetSprite;
	}
}
