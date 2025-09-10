using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x0200080C RID: 2060
	[AddComponentMenu("UI/TMP Dropdown", 35)]
	[RequireComponent(typeof(RectTransform))]
	public class TMP_Dropdown : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICancelHandler
	{
		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06004819 RID: 18457 RVA: 0x0014FCF5 File Offset: 0x0014DEF5
		// (set) Token: 0x0600481A RID: 18458 RVA: 0x0014FCFD File Offset: 0x0014DEFD
		public RectTransform template
		{
			get
			{
				return this.m_Template;
			}
			set
			{
				this.m_Template = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x0600481B RID: 18459 RVA: 0x0014FD0C File Offset: 0x0014DF0C
		// (set) Token: 0x0600481C RID: 18460 RVA: 0x0014FD14 File Offset: 0x0014DF14
		public TMP_Text captionText
		{
			get
			{
				return this.m_CaptionText;
			}
			set
			{
				this.m_CaptionText = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x0600481D RID: 18461 RVA: 0x0014FD23 File Offset: 0x0014DF23
		// (set) Token: 0x0600481E RID: 18462 RVA: 0x0014FD2B File Offset: 0x0014DF2B
		public Image captionImage
		{
			get
			{
				return this.m_CaptionImage;
			}
			set
			{
				this.m_CaptionImage = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x0600481F RID: 18463 RVA: 0x0014FD3A File Offset: 0x0014DF3A
		// (set) Token: 0x06004820 RID: 18464 RVA: 0x0014FD42 File Offset: 0x0014DF42
		public TMP_Text itemText
		{
			get
			{
				return this.m_ItemText;
			}
			set
			{
				this.m_ItemText = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x06004821 RID: 18465 RVA: 0x0014FD51 File Offset: 0x0014DF51
		// (set) Token: 0x06004822 RID: 18466 RVA: 0x0014FD59 File Offset: 0x0014DF59
		public Image itemImage
		{
			get
			{
				return this.m_ItemImage;
			}
			set
			{
				this.m_ItemImage = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x06004823 RID: 18467 RVA: 0x0014FD68 File Offset: 0x0014DF68
		// (set) Token: 0x06004824 RID: 18468 RVA: 0x0014FD75 File Offset: 0x0014DF75
		public List<TMP_Dropdown.OptionData> options
		{
			get
			{
				return this.m_Options.options;
			}
			set
			{
				this.m_Options.options = value;
				this.RefreshShownValue();
			}
		}

		// Token: 0x17000837 RID: 2103
		// (get) Token: 0x06004825 RID: 18469 RVA: 0x0014FD89 File Offset: 0x0014DF89
		// (set) Token: 0x06004826 RID: 18470 RVA: 0x0014FD91 File Offset: 0x0014DF91
		public TMP_Dropdown.DropdownEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				this.m_OnValueChanged = value;
			}
		}

		// Token: 0x17000838 RID: 2104
		// (get) Token: 0x06004827 RID: 18471 RVA: 0x0014FD9A File Offset: 0x0014DF9A
		// (set) Token: 0x06004828 RID: 18472 RVA: 0x0014FDA4 File Offset: 0x0014DFA4
		public int value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				if (Application.isPlaying && (value == this.m_Value || this.options.Count == 0))
				{
					return;
				}
				this.m_Value = Mathf.Clamp(value, 0, this.options.Count - 1);
				this.RefreshShownValue();
				this.m_OnValueChanged.Invoke(this.m_Value);
			}
		}

		// Token: 0x06004829 RID: 18473 RVA: 0x0014FE00 File Offset: 0x0014E000
		protected TMP_Dropdown()
		{
		}

		// Token: 0x0600482A RID: 18474 RVA: 0x0014FE2C File Offset: 0x0014E02C
		protected override void Awake()
		{
			this.m_AlphaTweenRunner = new TweenRunner<FloatTween>();
			this.m_AlphaTweenRunner.Init(this);
			if (this.m_CaptionImage)
			{
				this.m_CaptionImage.enabled = (this.m_CaptionImage.sprite != null);
			}
			if (this.m_Template)
			{
				this.m_Template.gameObject.SetActive(false);
			}
		}

		// Token: 0x0600482B RID: 18475 RVA: 0x0014FE98 File Offset: 0x0014E098
		public void RefreshShownValue()
		{
			TMP_Dropdown.OptionData optionData = TMP_Dropdown.s_NoOptionData;
			if (this.options.Count > 0)
			{
				optionData = this.options[Mathf.Clamp(this.m_Value, 0, this.options.Count - 1)];
			}
			if (this.m_CaptionText)
			{
				if (optionData != null && optionData.text != null)
				{
					this.m_CaptionText.text = optionData.text;
				}
				else
				{
					this.m_CaptionText.text = "";
				}
			}
			if (this.m_CaptionImage)
			{
				if (optionData != null)
				{
					this.m_CaptionImage.sprite = optionData.image;
				}
				else
				{
					this.m_CaptionImage.sprite = null;
				}
				this.m_CaptionImage.enabled = (this.m_CaptionImage.sprite != null);
			}
		}

		// Token: 0x0600482C RID: 18476 RVA: 0x0014FF64 File Offset: 0x0014E164
		public void AddOptions(List<TMP_Dropdown.OptionData> options)
		{
			this.options.AddRange(options);
			this.RefreshShownValue();
		}

		// Token: 0x0600482D RID: 18477 RVA: 0x0014FF78 File Offset: 0x0014E178
		public void AddOptions(List<string> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new TMP_Dropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		// Token: 0x0600482E RID: 18478 RVA: 0x0014FFB4 File Offset: 0x0014E1B4
		public void AddOptions(List<Sprite> options)
		{
			for (int i = 0; i < options.Count; i++)
			{
				this.options.Add(new TMP_Dropdown.OptionData(options[i]));
			}
			this.RefreshShownValue();
		}

		// Token: 0x0600482F RID: 18479 RVA: 0x0014FFEF File Offset: 0x0014E1EF
		public void ClearOptions()
		{
			this.options.Clear();
			this.RefreshShownValue();
		}

		// Token: 0x06004830 RID: 18480 RVA: 0x00150004 File Offset: 0x0014E204
		private void SetupTemplate()
		{
			this.validTemplate = false;
			if (!this.m_Template)
			{
				Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
				return;
			}
			GameObject gameObject = this.m_Template.gameObject;
			gameObject.SetActive(true);
			Toggle componentInChildren = this.m_Template.GetComponentInChildren<Toggle>();
			this.validTemplate = true;
			if (!componentInChildren || componentInChildren.transform == this.template)
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", this.template);
			}
			else if (!(componentInChildren.transform.parent is RectTransform))
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", this.template);
			}
			else if (this.itemText != null && !this.itemText.transform.IsChildOf(componentInChildren.transform))
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", this.template);
			}
			else if (this.itemImage != null && !this.itemImage.transform.IsChildOf(componentInChildren.transform))
			{
				this.validTemplate = false;
				Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", this.template);
			}
			if (!this.validTemplate)
			{
				gameObject.SetActive(false);
				return;
			}
			TMP_Dropdown.DropdownItem dropdownItem = componentInChildren.gameObject.AddComponent<TMP_Dropdown.DropdownItem>();
			dropdownItem.text = this.m_ItemText;
			dropdownItem.image = this.m_ItemImage;
			dropdownItem.toggle = componentInChildren;
			dropdownItem.rectTransform = (RectTransform)componentInChildren.transform;
			Canvas orAddComponent = TMP_Dropdown.GetOrAddComponent<Canvas>(gameObject);
			orAddComponent.overrideSorting = true;
			orAddComponent.sortingOrder = 30000;
			TMP_Dropdown.GetOrAddComponent<GraphicRaycaster>(gameObject);
			TMP_Dropdown.GetOrAddComponent<CanvasGroup>(gameObject);
			gameObject.SetActive(false);
			this.validTemplate = true;
		}

		// Token: 0x06004831 RID: 18481 RVA: 0x001501B4 File Offset: 0x0014E3B4
		private static T GetOrAddComponent<T>(GameObject go) where T : Component
		{
			T t = go.GetComponent<T>();
			if (!t)
			{
				t = go.AddComponent<T>();
			}
			return t;
		}

		// Token: 0x06004832 RID: 18482 RVA: 0x001501DD File Offset: 0x0014E3DD
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			this.Show();
		}

		// Token: 0x06004833 RID: 18483 RVA: 0x001501E5 File Offset: 0x0014E3E5
		public virtual void OnSubmit(BaseEventData eventData)
		{
			this.Show();
		}

		// Token: 0x06004834 RID: 18484 RVA: 0x001501ED File Offset: 0x0014E3ED
		public virtual void OnCancel(BaseEventData eventData)
		{
			this.Hide();
		}

		// Token: 0x06004835 RID: 18485 RVA: 0x001501F8 File Offset: 0x0014E3F8
		public void Show()
		{
			if (!this.IsActive() || !this.IsInteractable() || this.m_Dropdown != null)
			{
				return;
			}
			if (!this.validTemplate)
			{
				this.SetupTemplate();
				if (!this.validTemplate)
				{
					return;
				}
			}
			List<Canvas> list = TMP_ListPool<Canvas>.Get();
			base.gameObject.GetComponentsInParent<Canvas>(false, list);
			if (list.Count == 0)
			{
				return;
			}
			Canvas canvas = list[0];
			TMP_ListPool<Canvas>.Release(list);
			this.m_Template.gameObject.SetActive(true);
			this.m_Dropdown = this.CreateDropdownList(this.m_Template.gameObject);
			this.m_Dropdown.name = "Dropdown List";
			this.m_Dropdown.SetActive(true);
			RectTransform rectTransform = this.m_Dropdown.transform as RectTransform;
			rectTransform.SetParent(this.m_Template.transform.parent, false);
			TMP_Dropdown.DropdownItem componentInChildren = this.m_Dropdown.GetComponentInChildren<TMP_Dropdown.DropdownItem>();
			RectTransform rectTransform2 = componentInChildren.rectTransform.parent.gameObject.transform as RectTransform;
			componentInChildren.rectTransform.gameObject.SetActive(true);
			Rect rect = rectTransform2.rect;
			Rect rect2 = componentInChildren.rectTransform.rect;
			Vector2 vector = rect2.min - rect.min + componentInChildren.rectTransform.localPosition;
			Vector2 vector2 = rect2.max - rect.max + componentInChildren.rectTransform.localPosition;
			Vector2 size = rect2.size;
			this.m_Items.Clear();
			Toggle toggle = null;
			for (int i = 0; i < this.options.Count; i++)
			{
				TMP_Dropdown.OptionData data = this.options[i];
				TMP_Dropdown.DropdownItem item = this.AddItem(data, this.value == i, componentInChildren, this.m_Items);
				if (!(item == null))
				{
					item.toggle.isOn = (this.value == i);
					item.toggle.onValueChanged.AddListener(delegate(bool x)
					{
						this.OnSelectItem(item.toggle);
					});
					if (item.toggle.isOn)
					{
						item.toggle.Select();
					}
					if (toggle != null)
					{
						Navigation navigation = toggle.navigation;
						Navigation navigation2 = item.toggle.navigation;
						navigation.mode = Navigation.Mode.Explicit;
						navigation2.mode = Navigation.Mode.Explicit;
						navigation.selectOnDown = item.toggle;
						navigation.selectOnRight = item.toggle;
						navigation2.selectOnLeft = toggle;
						navigation2.selectOnUp = toggle;
						toggle.navigation = navigation;
						item.toggle.navigation = navigation2;
					}
					toggle = item.toggle;
				}
			}
			Vector2 sizeDelta = rectTransform2.sizeDelta;
			sizeDelta.y = size.y * (float)this.m_Items.Count + vector.y - vector2.y;
			rectTransform2.sizeDelta = sizeDelta;
			float num = rectTransform.rect.height - rectTransform2.rect.height;
			if (num > 0f)
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - num);
			}
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			RectTransform rectTransform3 = canvas.transform as RectTransform;
			Rect rect3 = rectTransform3.rect;
			for (int j = 0; j < 2; j++)
			{
				bool flag = false;
				for (int k = 0; k < 4; k++)
				{
					Vector3 vector3 = rectTransform3.InverseTransformPoint(array[k]);
					if (vector3[j] < rect3.min[j] || vector3[j] > rect3.max[j])
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					RectTransformUtility.FlipLayoutOnAxis(rectTransform, j, false, false);
				}
			}
			for (int l = 0; l < this.m_Items.Count; l++)
			{
				RectTransform rectTransform4 = this.m_Items[l].rectTransform;
				rectTransform4.anchorMin = new Vector2(rectTransform4.anchorMin.x, 0f);
				rectTransform4.anchorMax = new Vector2(rectTransform4.anchorMax.x, 0f);
				rectTransform4.anchoredPosition = new Vector2(rectTransform4.anchoredPosition.x, vector.y + size.y * (float)(this.m_Items.Count - 1 - l) + size.y * rectTransform4.pivot.y);
				rectTransform4.sizeDelta = new Vector2(rectTransform4.sizeDelta.x, size.y);
			}
			this.AlphaFadeList(0.15f, 0f, 1f);
			this.m_Template.gameObject.SetActive(false);
			componentInChildren.gameObject.SetActive(false);
			this.m_Blocker = this.CreateBlocker(canvas);
		}

		// Token: 0x06004836 RID: 18486 RVA: 0x00150748 File Offset: 0x0014E948
		protected virtual GameObject CreateBlocker(Canvas rootCanvas)
		{
			GameObject gameObject = new GameObject("Blocker");
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.SetParent(rootCanvas.transform, false);
			rectTransform.anchorMin = Vector3.zero;
			rectTransform.anchorMax = Vector3.one;
			rectTransform.sizeDelta = Vector2.zero;
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.overrideSorting = true;
			Canvas component = this.m_Dropdown.GetComponent<Canvas>();
			canvas.sortingLayerID = component.sortingLayerID;
			canvas.sortingOrder = component.sortingOrder - 1;
			gameObject.AddComponent<GraphicRaycaster>();
			gameObject.AddComponent<Image>().color = Color.clear;
			gameObject.AddComponent<Button>().onClick.AddListener(new UnityAction(this.Hide));
			return gameObject;
		}

		// Token: 0x06004837 RID: 18487 RVA: 0x00150801 File Offset: 0x0014EA01
		protected virtual void DestroyBlocker(GameObject blocker)
		{
			Object.Destroy(blocker);
		}

		// Token: 0x06004838 RID: 18488 RVA: 0x00150809 File Offset: 0x0014EA09
		protected virtual GameObject CreateDropdownList(GameObject template)
		{
			return Object.Instantiate<GameObject>(template);
		}

		// Token: 0x06004839 RID: 18489 RVA: 0x00150811 File Offset: 0x0014EA11
		protected virtual void DestroyDropdownList(GameObject dropdownList)
		{
			Object.Destroy(dropdownList);
		}

		// Token: 0x0600483A RID: 18490 RVA: 0x00150819 File Offset: 0x0014EA19
		protected virtual TMP_Dropdown.DropdownItem CreateItem(TMP_Dropdown.DropdownItem itemTemplate)
		{
			return Object.Instantiate<TMP_Dropdown.DropdownItem>(itemTemplate);
		}

		// Token: 0x0600483B RID: 18491 RVA: 0x00150821 File Offset: 0x0014EA21
		protected virtual void DestroyItem(TMP_Dropdown.DropdownItem item)
		{
		}

		// Token: 0x0600483C RID: 18492 RVA: 0x00150824 File Offset: 0x0014EA24
		private TMP_Dropdown.DropdownItem AddItem(TMP_Dropdown.OptionData data, bool selected, TMP_Dropdown.DropdownItem itemTemplate, List<TMP_Dropdown.DropdownItem> items)
		{
			TMP_Dropdown.DropdownItem dropdownItem = this.CreateItem(itemTemplate);
			dropdownItem.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);
			dropdownItem.gameObject.SetActive(true);
			dropdownItem.gameObject.name = "Item " + items.Count.ToString() + ((data.text != null) ? (": " + data.text) : "");
			if (dropdownItem.toggle != null)
			{
				dropdownItem.toggle.isOn = false;
			}
			if (dropdownItem.text)
			{
				dropdownItem.text.text = data.text;
			}
			if (dropdownItem.image)
			{
				dropdownItem.image.sprite = data.image;
				dropdownItem.image.enabled = (dropdownItem.image.sprite != null);
			}
			items.Add(dropdownItem);
			return dropdownItem;
		}

		// Token: 0x0600483D RID: 18493 RVA: 0x0015091C File Offset: 0x0014EB1C
		private void AlphaFadeList(float duration, float alpha)
		{
			CanvasGroup component = this.m_Dropdown.GetComponent<CanvasGroup>();
			this.AlphaFadeList(duration, component.alpha, alpha);
		}

		// Token: 0x0600483E RID: 18494 RVA: 0x00150944 File Offset: 0x0014EB44
		private void AlphaFadeList(float duration, float start, float end)
		{
			if (end.Equals(start))
			{
				return;
			}
			FloatTween info = new FloatTween
			{
				duration = duration,
				startValue = start,
				targetValue = end
			};
			info.AddOnChangedCallback(new UnityAction<float>(this.SetAlpha));
			info.ignoreTimeScale = true;
			this.m_AlphaTweenRunner.StartTween(info);
		}

		// Token: 0x0600483F RID: 18495 RVA: 0x001509A5 File Offset: 0x0014EBA5
		private void SetAlpha(float alpha)
		{
			if (!this.m_Dropdown)
			{
				return;
			}
			this.m_Dropdown.GetComponent<CanvasGroup>().alpha = alpha;
		}

		// Token: 0x06004840 RID: 18496 RVA: 0x001509C8 File Offset: 0x0014EBC8
		public void Hide()
		{
			if (this.m_Dropdown != null)
			{
				this.AlphaFadeList(0.15f, 0f);
				if (this.IsActive())
				{
					base.StartCoroutine(this.DelayedDestroyDropdownList(0.15f));
				}
			}
			if (this.m_Blocker != null)
			{
				this.DestroyBlocker(this.m_Blocker);
			}
			this.m_Blocker = null;
			this.Select();
		}

		// Token: 0x06004841 RID: 18497 RVA: 0x00150A34 File Offset: 0x0014EC34
		private IEnumerator DelayedDestroyDropdownList(float delay)
		{
			yield return new WaitForSeconds(delay);
			for (int i = 0; i < this.m_Items.Count; i++)
			{
				if (this.m_Items[i] != null)
				{
					this.DestroyItem(this.m_Items[i]);
				}
				this.m_Items.Clear();
			}
			if (this.m_Dropdown != null)
			{
				this.DestroyDropdownList(this.m_Dropdown);
			}
			this.m_Dropdown = null;
			yield break;
		}

		// Token: 0x06004842 RID: 18498 RVA: 0x00150A4C File Offset: 0x0014EC4C
		private void OnSelectItem(Toggle toggle)
		{
			if (!toggle.isOn)
			{
				toggle.isOn = true;
			}
			int num = -1;
			Transform transform = toggle.transform;
			Transform parent = transform.parent;
			for (int i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i) == transform)
				{
					num = i - 1;
					break;
				}
			}
			if (num < 0)
			{
				return;
			}
			this.value = num;
			this.Hide();
		}

		// Token: 0x04004893 RID: 18579
		[SerializeField]
		private RectTransform m_Template;

		// Token: 0x04004894 RID: 18580
		[SerializeField]
		private TMP_Text m_CaptionText;

		// Token: 0x04004895 RID: 18581
		[SerializeField]
		private Image m_CaptionImage;

		// Token: 0x04004896 RID: 18582
		[Space]
		[SerializeField]
		private TMP_Text m_ItemText;

		// Token: 0x04004897 RID: 18583
		[SerializeField]
		private Image m_ItemImage;

		// Token: 0x04004898 RID: 18584
		[Space]
		[SerializeField]
		private int m_Value;

		// Token: 0x04004899 RID: 18585
		[Space]
		[SerializeField]
		private TMP_Dropdown.OptionDataList m_Options = new TMP_Dropdown.OptionDataList();

		// Token: 0x0400489A RID: 18586
		[Space]
		[SerializeField]
		private TMP_Dropdown.DropdownEvent m_OnValueChanged = new TMP_Dropdown.DropdownEvent();

		// Token: 0x0400489B RID: 18587
		private GameObject m_Dropdown;

		// Token: 0x0400489C RID: 18588
		private GameObject m_Blocker;

		// Token: 0x0400489D RID: 18589
		private List<TMP_Dropdown.DropdownItem> m_Items = new List<TMP_Dropdown.DropdownItem>();

		// Token: 0x0400489E RID: 18590
		private TweenRunner<FloatTween> m_AlphaTweenRunner;

		// Token: 0x0400489F RID: 18591
		private bool validTemplate;

		// Token: 0x040048A0 RID: 18592
		private static TMP_Dropdown.OptionData s_NoOptionData = new TMP_Dropdown.OptionData();

		// Token: 0x02001AB8 RID: 6840
		protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, ICancelHandler
		{
			// Token: 0x17001145 RID: 4421
			// (get) Token: 0x060097BD RID: 38845 RVA: 0x002AB2BA File Offset: 0x002A94BA
			// (set) Token: 0x060097BE RID: 38846 RVA: 0x002AB2C2 File Offset: 0x002A94C2
			public TMP_Text text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			// Token: 0x17001146 RID: 4422
			// (get) Token: 0x060097BF RID: 38847 RVA: 0x002AB2CB File Offset: 0x002A94CB
			// (set) Token: 0x060097C0 RID: 38848 RVA: 0x002AB2D3 File Offset: 0x002A94D3
			public Image image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			// Token: 0x17001147 RID: 4423
			// (get) Token: 0x060097C1 RID: 38849 RVA: 0x002AB2DC File Offset: 0x002A94DC
			// (set) Token: 0x060097C2 RID: 38850 RVA: 0x002AB2E4 File Offset: 0x002A94E4
			public RectTransform rectTransform
			{
				get
				{
					return this.m_RectTransform;
				}
				set
				{
					this.m_RectTransform = value;
				}
			}

			// Token: 0x17001148 RID: 4424
			// (get) Token: 0x060097C3 RID: 38851 RVA: 0x002AB2ED File Offset: 0x002A94ED
			// (set) Token: 0x060097C4 RID: 38852 RVA: 0x002AB2F5 File Offset: 0x002A94F5
			public Toggle toggle
			{
				get
				{
					return this.m_Toggle;
				}
				set
				{
					this.m_Toggle = value;
				}
			}

			// Token: 0x060097C5 RID: 38853 RVA: 0x002AB2FE File Offset: 0x002A94FE
			public virtual void OnPointerEnter(PointerEventData eventData)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}

			// Token: 0x060097C6 RID: 38854 RVA: 0x002AB310 File Offset: 0x002A9510
			public virtual void OnCancel(BaseEventData eventData)
			{
				TMP_Dropdown componentInParent = base.GetComponentInParent<TMP_Dropdown>();
				if (componentInParent)
				{
					componentInParent.Hide();
				}
			}

			// Token: 0x04009A4E RID: 39502
			[SerializeField]
			private TMP_Text m_Text;

			// Token: 0x04009A4F RID: 39503
			[SerializeField]
			private Image m_Image;

			// Token: 0x04009A50 RID: 39504
			[SerializeField]
			private RectTransform m_RectTransform;

			// Token: 0x04009A51 RID: 39505
			[SerializeField]
			private Toggle m_Toggle;
		}

		// Token: 0x02001AB9 RID: 6841
		[Serializable]
		public class OptionData
		{
			// Token: 0x17001149 RID: 4425
			// (get) Token: 0x060097C8 RID: 38856 RVA: 0x002AB33A File Offset: 0x002A953A
			// (set) Token: 0x060097C9 RID: 38857 RVA: 0x002AB342 File Offset: 0x002A9542
			public string text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					this.m_Text = value;
				}
			}

			// Token: 0x1700114A RID: 4426
			// (get) Token: 0x060097CA RID: 38858 RVA: 0x002AB34B File Offset: 0x002A954B
			// (set) Token: 0x060097CB RID: 38859 RVA: 0x002AB353 File Offset: 0x002A9553
			public Sprite image
			{
				get
				{
					return this.m_Image;
				}
				set
				{
					this.m_Image = value;
				}
			}

			// Token: 0x060097CC RID: 38860 RVA: 0x002AB35C File Offset: 0x002A955C
			public OptionData()
			{
			}

			// Token: 0x060097CD RID: 38861 RVA: 0x002AB364 File Offset: 0x002A9564
			public OptionData(string text)
			{
				this.text = text;
			}

			// Token: 0x060097CE RID: 38862 RVA: 0x002AB373 File Offset: 0x002A9573
			public OptionData(Sprite image)
			{
				this.image = image;
			}

			// Token: 0x060097CF RID: 38863 RVA: 0x002AB382 File Offset: 0x002A9582
			public OptionData(string text, Sprite image)
			{
				this.text = text;
				this.image = image;
			}

			// Token: 0x04009A52 RID: 39506
			[SerializeField]
			private string m_Text;

			// Token: 0x04009A53 RID: 39507
			[SerializeField]
			private Sprite m_Image;
		}

		// Token: 0x02001ABA RID: 6842
		[Serializable]
		public class OptionDataList
		{
			// Token: 0x1700114B RID: 4427
			// (get) Token: 0x060097D0 RID: 38864 RVA: 0x002AB398 File Offset: 0x002A9598
			// (set) Token: 0x060097D1 RID: 38865 RVA: 0x002AB3A0 File Offset: 0x002A95A0
			public List<TMP_Dropdown.OptionData> options
			{
				get
				{
					return this.m_Options;
				}
				set
				{
					this.m_Options = value;
				}
			}

			// Token: 0x060097D2 RID: 38866 RVA: 0x002AB3A9 File Offset: 0x002A95A9
			public OptionDataList()
			{
				this.options = new List<TMP_Dropdown.OptionData>();
			}

			// Token: 0x04009A54 RID: 39508
			[SerializeField]
			private List<TMP_Dropdown.OptionData> m_Options;
		}

		// Token: 0x02001ABB RID: 6843
		[Serializable]
		public class DropdownEvent : UnityEvent<int>
		{
		}
	}
}
