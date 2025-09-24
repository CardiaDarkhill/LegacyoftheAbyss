using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x0200080F RID: 2063
	[AddComponentMenu("UI/TextMeshPro - Input Field", 11)]
	public class TMP_InputField : Selectable, IUpdateSelectedHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement
	{
		// Token: 0x06004856 RID: 18518 RVA: 0x00151464 File Offset: 0x0014F664
		protected TMP_InputField()
		{
		}

		// Token: 0x1700083E RID: 2110
		// (get) Token: 0x06004857 RID: 18519 RVA: 0x00151523 File Offset: 0x0014F723
		protected Mesh mesh
		{
			get
			{
				if (this.m_Mesh == null)
				{
					this.m_Mesh = new Mesh();
					this.m_Mesh.name = "TMP_InputField_Mesh";
				}
				return this.m_Mesh;
			}
		}

		// Token: 0x1700083F RID: 2111
		// (get) Token: 0x06004859 RID: 18521 RVA: 0x00151564 File Offset: 0x0014F764
		// (set) Token: 0x06004858 RID: 18520 RVA: 0x00151554 File Offset: 0x0014F754
		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android && platform != RuntimePlatform.tvOS) || this.m_HideMobileInput;
			}
			set
			{
				SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
			}
		}

		// Token: 0x17000840 RID: 2112
		// (get) Token: 0x0600485A RID: 18522 RVA: 0x0015158D File Offset: 0x0014F78D
		// (set) Token: 0x0600485B RID: 18523 RVA: 0x00151598 File Offset: 0x0014F798
		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (this.text == value)
				{
					return;
				}
				this.m_Text = value;
				if (this.m_Keyboard != null)
				{
					this.m_Keyboard.text = this.m_Text;
				}
				if (this.m_StringPosition > this.m_Text.Length)
				{
					this.m_StringPosition = (this.m_StringSelectPosition = this.m_Text.Length);
				}
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		// Token: 0x17000841 RID: 2113
		// (get) Token: 0x0600485C RID: 18524 RVA: 0x00151607 File Offset: 0x0014F807
		public bool isFocused
		{
			get
			{
				return this.m_AllowInput;
			}
		}

		// Token: 0x17000842 RID: 2114
		// (get) Token: 0x0600485D RID: 18525 RVA: 0x0015160F File Offset: 0x0014F80F
		// (set) Token: 0x0600485E RID: 18526 RVA: 0x00151617 File Offset: 0x0014F817
		public float caretBlinkRate
		{
			get
			{
				return this.m_CaretBlinkRate;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) && this.m_AllowInput)
				{
					this.SetCaretActive();
				}
			}
		}

		// Token: 0x17000843 RID: 2115
		// (get) Token: 0x0600485F RID: 18527 RVA: 0x00151635 File Offset: 0x0014F835
		// (set) Token: 0x06004860 RID: 18528 RVA: 0x0015163D File Offset: 0x0014F83D
		public int caretWidth
		{
			get
			{
				return this.m_CaretWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CaretWidth, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x17000844 RID: 2116
		// (get) Token: 0x06004861 RID: 18529 RVA: 0x00151653 File Offset: 0x0014F853
		// (set) Token: 0x06004862 RID: 18530 RVA: 0x0015165B File Offset: 0x0014F85B
		public RectTransform textViewport
		{
			get
			{
				return this.m_TextViewport;
			}
			set
			{
				SetPropertyUtility.SetClass<RectTransform>(ref this.m_TextViewport, value);
			}
		}

		// Token: 0x17000845 RID: 2117
		// (get) Token: 0x06004863 RID: 18531 RVA: 0x0015166A File Offset: 0x0014F86A
		// (set) Token: 0x06004864 RID: 18532 RVA: 0x00151672 File Offset: 0x0014F872
		public TMP_Text textComponent
		{
			get
			{
				return this.m_TextComponent;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_Text>(ref this.m_TextComponent, value);
			}
		}

		// Token: 0x17000846 RID: 2118
		// (get) Token: 0x06004865 RID: 18533 RVA: 0x00151681 File Offset: 0x0014F881
		// (set) Token: 0x06004866 RID: 18534 RVA: 0x00151689 File Offset: 0x0014F889
		public Graphic placeholder
		{
			get
			{
				return this.m_Placeholder;
			}
			set
			{
				SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
			}
		}

		// Token: 0x17000847 RID: 2119
		// (get) Token: 0x06004867 RID: 18535 RVA: 0x00151698 File Offset: 0x0014F898
		// (set) Token: 0x06004868 RID: 18536 RVA: 0x001516B4 File Offset: 0x0014F8B4
		public Color caretColor
		{
			get
			{
				if (!this.customCaretColor)
				{
					return this.textComponent.color;
				}
				return this.m_CaretColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_CaretColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x17000848 RID: 2120
		// (get) Token: 0x06004869 RID: 18537 RVA: 0x001516CA File Offset: 0x0014F8CA
		// (set) Token: 0x0600486A RID: 18538 RVA: 0x001516D2 File Offset: 0x0014F8D2
		public bool customCaretColor
		{
			get
			{
				return this.m_CustomCaretColor;
			}
			set
			{
				if (this.m_CustomCaretColor != value)
				{
					this.m_CustomCaretColor = value;
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x17000849 RID: 2121
		// (get) Token: 0x0600486B RID: 18539 RVA: 0x001516EA File Offset: 0x0014F8EA
		// (set) Token: 0x0600486C RID: 18540 RVA: 0x001516F2 File Offset: 0x0014F8F2
		public Color selectionColor
		{
			get
			{
				return this.m_SelectionColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_SelectionColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x0600486D RID: 18541 RVA: 0x00151708 File Offset: 0x0014F908
		// (set) Token: 0x0600486E RID: 18542 RVA: 0x00151710 File Offset: 0x0014F910
		public TMP_InputField.SubmitEvent onEndEdit
		{
			get
			{
				return this.m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnEndEdit, value);
			}
		}

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x0600486F RID: 18543 RVA: 0x0015171F File Offset: 0x0014F91F
		// (set) Token: 0x06004870 RID: 18544 RVA: 0x00151727 File Offset: 0x0014F927
		public TMP_InputField.SubmitEvent onSubmit
		{
			get
			{
				return this.m_OnSubmit;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnSubmit, value);
			}
		}

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x06004871 RID: 18545 RVA: 0x00151736 File Offset: 0x0014F936
		// (set) Token: 0x06004872 RID: 18546 RVA: 0x0015173E File Offset: 0x0014F93E
		public TMP_InputField.SubmitEvent onFocusLost
		{
			get
			{
				return this.m_OnFocusLost;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnFocusLost, value);
			}
		}

		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06004873 RID: 18547 RVA: 0x0015174D File Offset: 0x0014F94D
		// (set) Token: 0x06004874 RID: 18548 RVA: 0x00151755 File Offset: 0x0014F955
		public TMP_InputField.OnChangeEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.OnChangeEvent>(ref this.m_OnValueChanged, value);
			}
		}

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x06004875 RID: 18549 RVA: 0x00151764 File Offset: 0x0014F964
		// (set) Token: 0x06004876 RID: 18550 RVA: 0x0015176C File Offset: 0x0014F96C
		public TMP_InputField.OnValidateInput onValidateInput
		{
			get
			{
				return this.m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.OnValidateInput>(ref this.m_OnValidateInput, value);
			}
		}

		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x06004877 RID: 18551 RVA: 0x0015177B File Offset: 0x0014F97B
		// (set) Token: 0x06004878 RID: 18552 RVA: 0x00151783 File Offset: 0x0014F983
		public int characterLimit
		{
			get
			{
				return this.m_CharacterLimit;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, Math.Max(0, value)))
				{
					this.UpdateLabel();
				}
			}
		}

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x06004879 RID: 18553 RVA: 0x0015179F File Offset: 0x0014F99F
		// (set) Token: 0x0600487A RID: 18554 RVA: 0x001517A7 File Offset: 0x0014F9A7
		public TMP_InputField.ContentType contentType
		{
			get
			{
				return this.m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.ContentType>(ref this.m_ContentType, value))
				{
					this.EnforceContentType();
				}
			}
		}

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x0600487B RID: 18555 RVA: 0x001517BD File Offset: 0x0014F9BD
		// (set) Token: 0x0600487C RID: 18556 RVA: 0x001517C5 File Offset: 0x0014F9C5
		public TMP_InputField.LineType lineType
		{
			get
			{
				return this.m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.LineType>(ref this.m_LineType, value))
				{
					this.SetTextComponentWrapMode();
				}
				this.SetToCustomIfContentTypeIsNot(new TMP_InputField.ContentType[]
				{
					TMP_InputField.ContentType.Standard,
					TMP_InputField.ContentType.Autocorrected
				});
			}
		}

		// Token: 0x17000852 RID: 2130
		// (get) Token: 0x0600487D RID: 18557 RVA: 0x001517EB File Offset: 0x0014F9EB
		// (set) Token: 0x0600487E RID: 18558 RVA: 0x001517F3 File Offset: 0x0014F9F3
		public TMP_InputField.InputType inputType
		{
			get
			{
				return this.m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.InputType>(ref this.m_InputType, value))
				{
					this.SetToCustom();
				}
			}
		}

		// Token: 0x17000853 RID: 2131
		// (get) Token: 0x0600487F RID: 18559 RVA: 0x00151809 File Offset: 0x0014FA09
		// (set) Token: 0x06004880 RID: 18560 RVA: 0x00151811 File Offset: 0x0014FA11
		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.m_KeyboardType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
				{
					this.SetToCustom();
				}
			}
		}

		// Token: 0x17000854 RID: 2132
		// (get) Token: 0x06004881 RID: 18561 RVA: 0x00151827 File Offset: 0x0014FA27
		// (set) Token: 0x06004882 RID: 18562 RVA: 0x0015182F File Offset: 0x0014FA2F
		public TMP_InputField.CharacterValidation characterValidation
		{
			get
			{
				return this.m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.CharacterValidation>(ref this.m_CharacterValidation, value))
				{
					this.SetToCustom();
				}
			}
		}

		// Token: 0x17000855 RID: 2133
		// (get) Token: 0x06004883 RID: 18563 RVA: 0x00151845 File Offset: 0x0014FA45
		// (set) Token: 0x06004884 RID: 18564 RVA: 0x0015184D File Offset: 0x0014FA4D
		public bool readOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				this.m_ReadOnly = value;
			}
		}

		// Token: 0x17000856 RID: 2134
		// (get) Token: 0x06004885 RID: 18565 RVA: 0x00151856 File Offset: 0x0014FA56
		// (set) Token: 0x06004886 RID: 18566 RVA: 0x0015185E File Offset: 0x0014FA5E
		public bool richText
		{
			get
			{
				return this.m_RichText;
			}
			set
			{
				this.m_RichText = value;
				this.SetTextComponentRichTextMode();
			}
		}

		// Token: 0x17000857 RID: 2135
		// (get) Token: 0x06004887 RID: 18567 RVA: 0x0015186D File Offset: 0x0014FA6D
		public bool multiLine
		{
			get
			{
				return this.m_LineType == TMP_InputField.LineType.MultiLineNewline || this.lineType == TMP_InputField.LineType.MultiLineSubmit;
			}
		}

		// Token: 0x17000858 RID: 2136
		// (get) Token: 0x06004888 RID: 18568 RVA: 0x00151883 File Offset: 0x0014FA83
		// (set) Token: 0x06004889 RID: 18569 RVA: 0x0015188B File Offset: 0x0014FA8B
		public char asteriskChar
		{
			get
			{
				return this.m_AsteriskChar;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value))
				{
					this.UpdateLabel();
				}
			}
		}

		// Token: 0x17000859 RID: 2137
		// (get) Token: 0x0600488A RID: 18570 RVA: 0x001518A1 File Offset: 0x0014FAA1
		public bool wasCanceled
		{
			get
			{
				return this.m_WasCanceled;
			}
		}

		// Token: 0x0600488B RID: 18571 RVA: 0x001518A9 File Offset: 0x0014FAA9
		protected void ClampPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
				return;
			}
			if (pos > this.text.Length)
			{
				pos = this.text.Length;
			}
		}

		// Token: 0x1700085A RID: 2138
		// (get) Token: 0x0600488C RID: 18572 RVA: 0x001518D0 File Offset: 0x0014FAD0
		// (set) Token: 0x0600488D RID: 18573 RVA: 0x001518E3 File Offset: 0x0014FAE3
		protected int caretPositionInternal
		{
			get
			{
				return this.m_CaretPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		// Token: 0x1700085B RID: 2139
		// (get) Token: 0x0600488E RID: 18574 RVA: 0x001518F8 File Offset: 0x0014FAF8
		// (set) Token: 0x0600488F RID: 18575 RVA: 0x0015190B File Offset: 0x0014FB0B
		protected int stringPositionInternal
		{
			get
			{
				return this.m_StringPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_StringPosition = value;
				this.ClampPos(ref this.m_StringPosition);
			}
		}

		// Token: 0x1700085C RID: 2140
		// (get) Token: 0x06004890 RID: 18576 RVA: 0x00151920 File Offset: 0x0014FB20
		// (set) Token: 0x06004891 RID: 18577 RVA: 0x00151933 File Offset: 0x0014FB33
		protected int caretSelectPositionInternal
		{
			get
			{
				return this.m_CaretSelectPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		// Token: 0x1700085D RID: 2141
		// (get) Token: 0x06004892 RID: 18578 RVA: 0x00151948 File Offset: 0x0014FB48
		// (set) Token: 0x06004893 RID: 18579 RVA: 0x0015195B File Offset: 0x0014FB5B
		protected int stringSelectPositionInternal
		{
			get
			{
				return this.m_StringSelectPosition + Input.compositionString.Length;
			}
			set
			{
				this.m_StringSelectPosition = value;
				this.ClampPos(ref this.m_StringSelectPosition);
			}
		}

		// Token: 0x1700085E RID: 2142
		// (get) Token: 0x06004894 RID: 18580 RVA: 0x00151970 File Offset: 0x0014FB70
		private bool hasSelection
		{
			get
			{
				return this.stringPositionInternal != this.stringSelectPositionInternal;
			}
		}

		// Token: 0x1700085F RID: 2143
		// (get) Token: 0x06004895 RID: 18581 RVA: 0x00151983 File Offset: 0x0014FB83
		// (set) Token: 0x06004896 RID: 18582 RVA: 0x00151996 File Offset: 0x0014FB96
		public int caretPosition
		{
			get
			{
				return this.m_StringSelectPosition + Input.compositionString.Length;
			}
			set
			{
				this.selectionAnchorPosition = value;
				this.selectionFocusPosition = value;
			}
		}

		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x06004897 RID: 18583 RVA: 0x001519A6 File Offset: 0x0014FBA6
		// (set) Token: 0x06004898 RID: 18584 RVA: 0x001519CB File Offset: 0x0014FBCB
		public int selectionAnchorPosition
		{
			get
			{
				this.m_StringPosition = this.GetStringIndexFromCaretPosition(this.m_CaretPosition);
				return this.m_StringPosition + Input.compositionString.Length;
			}
			set
			{
				if (Input.compositionString.Length != 0)
				{
					return;
				}
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x06004899 RID: 18585 RVA: 0x001519ED File Offset: 0x0014FBED
		// (set) Token: 0x0600489A RID: 18586 RVA: 0x00151A12 File Offset: 0x0014FC12
		public int selectionFocusPosition
		{
			get
			{
				this.m_StringSelectPosition = this.GetStringIndexFromCaretPosition(this.m_CaretSelectPosition);
				return this.m_StringSelectPosition + Input.compositionString.Length;
			}
			set
			{
				if (Input.compositionString.Length != 0)
				{
					return;
				}
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		// Token: 0x0600489B RID: 18587 RVA: 0x00151A34 File Offset: 0x0014FC34
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_Text == null)
			{
				this.m_Text = string.Empty;
			}
			this.m_DrawStart = 0;
			this.m_DrawEnd = this.m_Text.Length;
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.UpdateLabel();
			}
		}

		// Token: 0x0600489C RID: 18588 RVA: 0x00151AD8 File Offset: 0x0014FCD8
		protected override void OnDisable()
		{
			this.m_BlinkCoroutine = null;
			this.DeactivateInputField();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.Clear();
			}
			if (this.m_Mesh != null)
			{
				Object.DestroyImmediate(this.m_Mesh);
			}
			this.m_Mesh = null;
			base.OnDisable();
		}

		// Token: 0x0600489D RID: 18589 RVA: 0x00151B73 File Offset: 0x0014FD73
		private IEnumerator CaretBlink()
		{
			this.m_CaretVisible = true;
			yield return null;
			while (this.isFocused && this.m_CaretBlinkRate > 0f)
			{
				float num = 1f / this.m_CaretBlinkRate;
				bool flag = (Time.unscaledTime - this.m_BlinkStartTime) % num < num / 2f;
				if (this.m_CaretVisible != flag)
				{
					this.m_CaretVisible = flag;
					if (!this.hasSelection)
					{
						this.MarkGeometryAsDirty();
					}
				}
				yield return null;
			}
			this.m_BlinkCoroutine = null;
			yield break;
		}

		// Token: 0x0600489E RID: 18590 RVA: 0x00151B82 File Offset: 0x0014FD82
		private void SetCaretVisible()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_CaretVisible = true;
			this.m_BlinkStartTime = Time.unscaledTime;
			this.SetCaretActive();
		}

		// Token: 0x0600489F RID: 18591 RVA: 0x00151BA5 File Offset: 0x0014FDA5
		private void SetCaretActive()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			if (this.m_CaretBlinkRate > 0f)
			{
				if (this.m_BlinkCoroutine == null)
				{
					this.m_BlinkCoroutine = base.StartCoroutine(this.CaretBlink());
					return;
				}
			}
			else
			{
				this.m_CaretVisible = true;
			}
		}

		// Token: 0x060048A0 RID: 18592 RVA: 0x00151BDF File Offset: 0x0014FDDF
		protected void OnFocus()
		{
			this.SelectAll();
		}

		// Token: 0x060048A1 RID: 18593 RVA: 0x00151BE7 File Offset: 0x0014FDE7
		protected void SelectAll()
		{
			this.stringPositionInternal = this.text.Length;
			this.stringSelectPositionInternal = 0;
		}

		// Token: 0x060048A2 RID: 18594 RVA: 0x00151C04 File Offset: 0x0014FE04
		public void MoveTextEnd(bool shift)
		{
			int length = this.text.Length;
			if (shift)
			{
				this.stringSelectPositionInternal = length;
			}
			else
			{
				this.stringPositionInternal = length;
				this.stringSelectPositionInternal = this.stringPositionInternal;
			}
			this.UpdateLabel();
		}

		// Token: 0x060048A3 RID: 18595 RVA: 0x00151C44 File Offset: 0x0014FE44
		public void MoveTextStart(bool shift)
		{
			int num = 0;
			if (shift)
			{
				this.stringSelectPositionInternal = num;
			}
			else
			{
				this.stringPositionInternal = num;
				this.stringSelectPositionInternal = this.stringPositionInternal;
			}
			this.UpdateLabel();
		}

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x060048A4 RID: 18596 RVA: 0x00151C78 File Offset: 0x0014FE78
		// (set) Token: 0x060048A5 RID: 18597 RVA: 0x00151C7F File Offset: 0x0014FE7F
		private static string clipboard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}
			set
			{
				GUIUtility.systemCopyBuffer = value;
			}
		}

		// Token: 0x060048A6 RID: 18598 RVA: 0x00151C87 File Offset: 0x0014FE87
		private bool InPlaceEditing()
		{
			return !TouchScreenKeyboard.isSupported;
		}

		// Token: 0x060048A7 RID: 18599 RVA: 0x00151C94 File Offset: 0x0014FE94
		protected virtual void LateUpdate()
		{
			if (this.m_ShouldActivateNextUpdate)
			{
				if (!this.isFocused)
				{
					this.ActivateInputFieldInternal();
					this.m_ShouldActivateNextUpdate = false;
					return;
				}
				this.m_ShouldActivateNextUpdate = false;
			}
			if (this.InPlaceEditing() || !this.isFocused)
			{
				return;
			}
			this.AssignPositioningIfNeeded();
			if (this.m_Keyboard == null || !this.m_Keyboard.active)
			{
				if (this.m_Keyboard != null)
				{
					if (!this.m_ReadOnly)
					{
						this.text = this.m_Keyboard.text;
					}
					if (this.m_Keyboard.status == TouchScreenKeyboard.Status.Canceled)
					{
						this.m_WasCanceled = true;
					}
				}
				this.OnDeselect(null);
				return;
			}
			string text = this.m_Keyboard.text;
			if (this.m_Text != text)
			{
				if (this.m_ReadOnly)
				{
					this.m_Keyboard.text = this.m_Text;
				}
				else
				{
					this.m_Text = "";
					foreach (char c in text)
					{
						if (c == '\r' || c == '\u0003')
						{
							c = '\n';
						}
						if (this.onValidateInput != null)
						{
							c = this.onValidateInput(this.m_Text, this.m_Text.Length, c);
						}
						else if (this.characterValidation != TMP_InputField.CharacterValidation.None)
						{
							c = this.Validate(this.m_Text, this.m_Text.Length, c);
						}
						if (this.lineType == TMP_InputField.LineType.MultiLineSubmit && c == '\n')
						{
							this.m_Keyboard.text = this.m_Text;
							this.OnDeselect(null);
							return;
						}
						if (c != '\0')
						{
							this.m_Text += c.ToString();
						}
					}
					if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
					{
						this.m_Text = this.m_Text.Substring(0, this.characterLimit);
					}
					this.stringPositionInternal = (this.stringSelectPositionInternal = this.m_Text.Length);
					if (this.m_Text != text)
					{
						this.m_Keyboard.text = this.m_Text;
					}
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
			if (this.m_Keyboard.status == TouchScreenKeyboard.Status.Done)
			{
				if (this.m_Keyboard.status == TouchScreenKeyboard.Status.Canceled)
				{
					this.m_WasCanceled = true;
				}
				this.OnDeselect(null);
			}
		}

		// Token: 0x060048A8 RID: 18600 RVA: 0x00151EC9 File Offset: 0x001500C9
		protected int GetCharacterIndexFromPosition(Vector2 pos)
		{
			return 0;
		}

		// Token: 0x060048A9 RID: 18601 RVA: 0x00151ECC File Offset: 0x001500CC
		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left && this.m_TextComponent != null && this.m_Keyboard == null;
		}

		// Token: 0x060048AA RID: 18602 RVA: 0x00151EFF File Offset: 0x001500FF
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = true;
		}

		// Token: 0x060048AB RID: 18603 RVA: 0x00151F14 File Offset: 0x00150114
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			CaretPosition caretPosition;
			int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(this.m_TextComponent, eventData.position, eventData.pressEventCamera, out caretPosition);
			if (caretPosition == CaretPosition.Left)
			{
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition);
			}
			else if (caretPosition == CaretPosition.Right)
			{
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1;
			}
			this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			this.MarkGeometryAsDirty();
			this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textViewport, eventData.position, eventData.pressEventCamera);
			if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
			{
				this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
			}
			eventData.Use();
		}

		// Token: 0x060048AC RID: 18604 RVA: 0x00151FD1 File Offset: 0x001501D1
		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			while (this.m_UpdateDrag && this.m_DragPositionOutOfBounds)
			{
				Vector2 vector;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textViewport, eventData.position, eventData.pressEventCamera, out vector);
				Rect rect = this.textViewport.rect;
				if (this.multiLine)
				{
					if (vector.y > rect.yMax)
					{
						this.MoveUp(true, true);
					}
					else if (vector.y < rect.yMin)
					{
						this.MoveDown(true, true);
					}
				}
				else if (vector.x < rect.xMin)
				{
					this.MoveLeft(true, false);
				}
				else if (vector.x > rect.xMax)
				{
					this.MoveRight(true, false);
				}
				this.UpdateLabel();
				float seconds = this.multiLine ? 0.1f : 0.05f;
				yield return new WaitForSeconds(seconds);
			}
			this.m_DragCoroutine = null;
			yield break;
		}

		// Token: 0x060048AD RID: 18605 RVA: 0x00151FE7 File Offset: 0x001501E7
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = false;
		}

		// Token: 0x060048AE RID: 18606 RVA: 0x00151FFC File Offset: 0x001501FC
		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
			bool allowInput = this.m_AllowInput;
			base.OnPointerDown(eventData);
			if (!this.InPlaceEditing() && (this.m_Keyboard == null || !this.m_Keyboard.active))
			{
				this.OnSelect(eventData);
				return;
			}
			if (allowInput)
			{
				CaretPosition caretPosition;
				int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(this.m_TextComponent, eventData.position, eventData.pressEventCamera, out caretPosition);
				if (caretPosition == CaretPosition.Left)
				{
					this.stringPositionInternal = (this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition));
				}
				else if (caretPosition == CaretPosition.Right)
				{
					this.stringPositionInternal = (this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(cursorIndexFromPosition) + 1);
				}
				this.caretPositionInternal = (this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal));
			}
			this.UpdateLabel();
			eventData.Use();
		}

		// Token: 0x060048AF RID: 18607 RVA: 0x001520D8 File Offset: 0x001502D8
		protected TMP_InputField.EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			RuntimePlatform platform = Application.platform;
			bool flag = (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer) ? ((modifiers & EventModifiers.Command) > EventModifiers.None) : ((modifiers & EventModifiers.Control) > EventModifiers.None);
			bool flag2 = (modifiers & EventModifiers.Shift) > EventModifiers.None;
			bool flag3 = (modifiers & EventModifiers.Alt) > EventModifiers.None;
			bool flag4 = flag && !flag3 && !flag2;
			KeyCode keyCode = evt.keyCode;
			if (keyCode <= KeyCode.A)
			{
				if (keyCode <= KeyCode.Return)
				{
					if (keyCode == KeyCode.Backspace)
					{
						this.Backspace();
						return TMP_InputField.EditState.Continue;
					}
					if (keyCode != KeyCode.Return)
					{
						goto IL_1C1;
					}
				}
				else
				{
					if (keyCode == KeyCode.Escape)
					{
						this.m_WasCanceled = true;
						return TMP_InputField.EditState.Finish;
					}
					if (keyCode != KeyCode.A)
					{
						goto IL_1C1;
					}
					if (flag4)
					{
						this.SelectAll();
						return TMP_InputField.EditState.Continue;
					}
					goto IL_1C1;
				}
			}
			else if (keyCode <= KeyCode.V)
			{
				if (keyCode != KeyCode.C)
				{
					if (keyCode != KeyCode.V)
					{
						goto IL_1C1;
					}
					if (flag4)
					{
						this.Append(TMP_InputField.clipboard);
						return TMP_InputField.EditState.Continue;
					}
					goto IL_1C1;
				}
				else
				{
					if (flag4)
					{
						if (this.inputType != TMP_InputField.InputType.Password)
						{
							TMP_InputField.clipboard = this.GetSelectedString();
						}
						else
						{
							TMP_InputField.clipboard = "";
						}
						return TMP_InputField.EditState.Continue;
					}
					goto IL_1C1;
				}
			}
			else if (keyCode != KeyCode.X)
			{
				if (keyCode == KeyCode.Delete)
				{
					this.ForwardSpace();
					return TMP_InputField.EditState.Continue;
				}
				switch (keyCode)
				{
				case KeyCode.KeypadEnter:
					break;
				case KeyCode.KeypadEquals:
				case KeyCode.Insert:
					goto IL_1C1;
				case KeyCode.UpArrow:
					this.MoveUp(flag2);
					return TMP_InputField.EditState.Continue;
				case KeyCode.DownArrow:
					this.MoveDown(flag2);
					return TMP_InputField.EditState.Continue;
				case KeyCode.RightArrow:
					this.MoveRight(flag2, flag);
					return TMP_InputField.EditState.Continue;
				case KeyCode.LeftArrow:
					this.MoveLeft(flag2, flag);
					return TMP_InputField.EditState.Continue;
				case KeyCode.Home:
					this.MoveTextStart(flag2);
					return TMP_InputField.EditState.Continue;
				case KeyCode.End:
					this.MoveTextEnd(flag2);
					return TMP_InputField.EditState.Continue;
				default:
					goto IL_1C1;
				}
			}
			else
			{
				if (flag4)
				{
					if (this.inputType != TMP_InputField.InputType.Password)
					{
						TMP_InputField.clipboard = this.GetSelectedString();
					}
					else
					{
						TMP_InputField.clipboard = "";
					}
					this.Delete();
					this.SendOnValueChangedAndUpdateLabel();
					return TMP_InputField.EditState.Continue;
				}
				goto IL_1C1;
			}
			if (this.lineType != TMP_InputField.LineType.MultiLineNewline)
			{
				return TMP_InputField.EditState.Finish;
			}
			IL_1C1:
			char c = evt.character;
			if (!this.multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				return TMP_InputField.EditState.Continue;
			}
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (this.IsValidChar(c))
			{
				this.Append(c);
			}
			if (c == '\0' && Input.compositionString.Length > 0)
			{
				this.UpdateLabel();
			}
			return TMP_InputField.EditState.Continue;
		}

		// Token: 0x060048B0 RID: 18608 RVA: 0x00152303 File Offset: 0x00150503
		private bool IsValidChar(char c)
		{
			return c != '\u007f' && (c == '\t' || c == '\n' || this.m_TextComponent.font.HasCharacter(c, true));
		}

		// Token: 0x060048B1 RID: 18609 RVA: 0x0015232A File Offset: 0x0015052A
		public void ProcessEvent(Event e)
		{
			this.KeyPressed(e);
		}

		// Token: 0x060048B2 RID: 18610 RVA: 0x00152334 File Offset: 0x00150534
		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (!this.isFocused)
			{
				return;
			}
			bool flag = false;
			while (Event.PopEvent(this.m_ProcessingEvent))
			{
				if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
				{
					flag = true;
					if (this.KeyPressed(this.m_ProcessingEvent) == TMP_InputField.EditState.Finish)
					{
						this.DeactivateInputField();
						break;
					}
				}
				EventType type = this.m_ProcessingEvent.type;
				if (type - EventType.ValidateCommand <= 1 && this.m_ProcessingEvent.commandName == "SelectAll")
				{
					this.SelectAll();
					flag = true;
				}
			}
			if (flag)
			{
				this.UpdateLabel();
			}
			eventData.Use();
		}

		// Token: 0x060048B3 RID: 18611 RVA: 0x001523C4 File Offset: 0x001505C4
		private string GetSelectedString()
		{
			if (!this.hasSelection)
			{
				return "";
			}
			int num = this.stringPositionInternal;
			int num2 = this.stringSelectPositionInternal;
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return this.text.Substring(num, num2 - num);
		}

		// Token: 0x060048B4 RID: 18612 RVA: 0x00152404 File Offset: 0x00150604
		private int FindtNextWordBegin()
		{
			if (this.stringSelectPositionInternal + 1 >= this.text.Length)
			{
				return this.text.Length;
			}
			int num = this.text.IndexOfAny(TMP_InputField.kSeparators, this.stringSelectPositionInternal + 1);
			if (num == -1)
			{
				num = this.text.Length;
			}
			else
			{
				num++;
			}
			return num;
		}

		// Token: 0x060048B5 RID: 18613 RVA: 0x00152464 File Offset: 0x00150664
		private void MoveRight(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				this.stringPositionInternal = (this.stringSelectPositionInternal = Mathf.Max(this.stringPositionInternal, this.stringSelectPositionInternal));
				this.caretPositionInternal = (this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
				return;
			}
			int caretSelectPositionInternal = this.caretSelectPositionInternal;
			int num;
			if (ctrl)
			{
				num = this.FindtNextWordBegin();
			}
			else
			{
				num = this.stringSelectPositionInternal + 1;
			}
			if (shift)
			{
				this.stringSelectPositionInternal = num;
				this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			}
			else
			{
				this.stringSelectPositionInternal = (this.stringPositionInternal = num);
				this.caretSelectPositionInternal = (this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
			}
			this.isCaretInsideTag = (caretSelectPositionInternal == this.caretSelectPositionInternal);
		}

		// Token: 0x060048B6 RID: 18614 RVA: 0x0015252C File Offset: 0x0015072C
		private int FindtPrevWordBegin()
		{
			if (this.stringSelectPositionInternal - 2 < 0)
			{
				return 0;
			}
			int num = this.text.LastIndexOfAny(TMP_InputField.kSeparators, this.stringSelectPositionInternal - 2);
			if (num == -1)
			{
				num = 0;
			}
			else
			{
				num++;
			}
			return num;
		}

		// Token: 0x060048B7 RID: 18615 RVA: 0x0015256C File Offset: 0x0015076C
		private void MoveLeft(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				this.stringPositionInternal = (this.stringSelectPositionInternal = Mathf.Min(this.stringPositionInternal, this.stringSelectPositionInternal));
				this.caretPositionInternal = (this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
				return;
			}
			int caretSelectPositionInternal = this.caretSelectPositionInternal;
			int num;
			if (ctrl)
			{
				num = this.FindtPrevWordBegin();
			}
			else
			{
				num = this.stringSelectPositionInternal - 1;
			}
			if (shift)
			{
				this.stringSelectPositionInternal = num;
				this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			}
			else
			{
				this.stringSelectPositionInternal = (this.stringPositionInternal = num);
				this.caretSelectPositionInternal = (this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
			}
			this.isCaretInsideTag = (caretSelectPositionInternal == this.caretSelectPositionInternal);
		}

		// Token: 0x060048B8 RID: 18616 RVA: 0x00152634 File Offset: 0x00150834
		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				originalPos--;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = (int)tmp_CharacterInfo.lineNumber;
			if (lineNumber - 1 >= 0)
			{
				int num = this.m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex - 1;
				int i = this.m_TextComponent.textInfo.lineInfo[lineNumber - 1].firstCharacterIndex;
				while (i < num)
				{
					TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
					float num2 = (tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin) / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
					if (num2 >= 0f && num2 <= 1f)
					{
						if (num2 < 0.5f)
						{
							return i;
						}
						return i + 1;
					}
					else
					{
						i++;
					}
				}
				return num;
			}
			if (!goToFirstChar)
			{
				return originalPos;
			}
			return 0;
		}

		// Token: 0x060048B9 RID: 18617 RVA: 0x00152724 File Offset: 0x00150924
		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				return this.text.Length;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = (int)tmp_CharacterInfo.lineNumber;
			if (lineNumber + 1 < this.m_TextComponent.textInfo.lineCount)
			{
				int lastCharacterIndex = this.m_TextComponent.textInfo.lineInfo[lineNumber + 1].lastCharacterIndex;
				int i = this.m_TextComponent.textInfo.lineInfo[lineNumber + 1].firstCharacterIndex;
				while (i < lastCharacterIndex)
				{
					TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
					float num = (tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin) / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
					if (num >= 0f && num <= 1f)
					{
						if (num < 0.5f)
						{
							return i;
						}
						return i + 1;
					}
					else
					{
						i++;
					}
				}
				return lastCharacterIndex;
			}
			if (!goToLastChar)
			{
				return originalPos;
			}
			return this.m_TextComponent.textInfo.characterCount - 1;
		}

		// Token: 0x060048BA RID: 18618 RVA: 0x0015283B File Offset: 0x00150A3B
		private void MoveDown(bool shift)
		{
			this.MoveDown(shift, true);
		}

		// Token: 0x060048BB RID: 18619 RVA: 0x00152848 File Offset: 0x00150A48
		private void MoveDown(bool shift, bool goToLastChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int num = this.multiLine ? this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar) : this.text.Length;
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
			this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal));
		}

		// Token: 0x060048BC RID: 18620 RVA: 0x001528EB File Offset: 0x00150AEB
		private void MoveUp(bool shift)
		{
			this.MoveUp(shift, true);
		}

		// Token: 0x060048BD RID: 18621 RVA: 0x001528F8 File Offset: 0x00150AF8
		private void MoveUp(bool shift, bool goToFirstChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int num = this.multiLine ? this.LineUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar) : 0;
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
			this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal));
		}

		// Token: 0x060048BE RID: 18622 RVA: 0x00152994 File Offset: 0x00150B94
		private void Delete()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.stringPositionInternal == this.stringSelectPositionInternal)
			{
				return;
			}
			if (this.stringPositionInternal < this.stringSelectPositionInternal)
			{
				this.m_Text = this.text.Substring(0, this.stringPositionInternal) + this.text.Substring(this.stringSelectPositionInternal, this.text.Length - this.stringSelectPositionInternal);
				this.stringSelectPositionInternal = this.stringPositionInternal;
				return;
			}
			this.m_Text = this.text.Substring(0, this.stringSelectPositionInternal) + this.text.Substring(this.stringPositionInternal, this.text.Length - this.stringPositionInternal);
			this.stringPositionInternal = this.stringSelectPositionInternal;
		}

		// Token: 0x060048BF RID: 18623 RVA: 0x00152A60 File Offset: 0x00150C60
		private void ForwardSpace()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.hasSelection)
			{
				this.Delete();
				this.SendOnValueChangedAndUpdateLabel();
				return;
			}
			if (this.stringPositionInternal < this.text.Length)
			{
				this.m_Text = this.text.Remove(this.stringPositionInternal, 1);
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		// Token: 0x060048C0 RID: 18624 RVA: 0x00152ABC File Offset: 0x00150CBC
		private void Backspace()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.hasSelection)
			{
				this.Delete();
				this.SendOnValueChangedAndUpdateLabel();
				return;
			}
			if (this.stringPositionInternal > 0)
			{
				this.m_Text = this.text.Remove(this.stringPositionInternal - 1, 1);
				this.stringSelectPositionInternal = --this.stringPositionInternal;
				this.m_isLastKeyBackspace = true;
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		// Token: 0x060048C1 RID: 18625 RVA: 0x00152B30 File Offset: 0x00150D30
		private void Insert(char c)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			string text = c.ToString();
			this.Delete();
			if (this.characterLimit > 0 && this.text.Length >= this.characterLimit)
			{
				return;
			}
			this.m_Text = this.text.Insert(this.m_StringPosition, text);
			this.stringSelectPositionInternal = (this.stringPositionInternal += text.Length);
			this.SendOnValueChanged();
		}

		// Token: 0x060048C2 RID: 18626 RVA: 0x00152BAB File Offset: 0x00150DAB
		private void SendOnValueChangedAndUpdateLabel()
		{
			this.SendOnValueChanged();
			this.UpdateLabel();
		}

		// Token: 0x060048C3 RID: 18627 RVA: 0x00152BB9 File Offset: 0x00150DB9
		private void SendOnValueChanged()
		{
			if (this.onValueChanged != null)
			{
				this.onValueChanged.Invoke(this.text);
			}
		}

		// Token: 0x060048C4 RID: 18628 RVA: 0x00152BD4 File Offset: 0x00150DD4
		protected void SendOnSubmit()
		{
			if (this.onEndEdit != null)
			{
				this.onEndEdit.Invoke(this.m_Text);
			}
		}

		// Token: 0x060048C5 RID: 18629 RVA: 0x00152BEF File Offset: 0x00150DEF
		protected void SendOnFocusLost()
		{
			if (this.onFocusLost != null)
			{
				this.onFocusLost.Invoke(this.m_Text);
			}
		}

		// Token: 0x060048C6 RID: 18630 RVA: 0x00152C0C File Offset: 0x00150E0C
		protected virtual void Append(string input)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (!this.InPlaceEditing())
			{
				return;
			}
			int i = 0;
			int length = input.Length;
			while (i < length)
			{
				char c = input[i];
				if (c >= ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\n')
				{
					this.Append(c);
				}
				i++;
			}
		}

		// Token: 0x060048C7 RID: 18631 RVA: 0x00152C68 File Offset: 0x00150E68
		protected virtual void Append(char input)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (!this.InPlaceEditing())
			{
				return;
			}
			if (this.onValidateInput != null)
			{
				input = this.onValidateInput(this.text, this.stringPositionInternal, input);
			}
			else if (this.characterValidation != TMP_InputField.CharacterValidation.None)
			{
				input = this.Validate(this.text, this.stringPositionInternal, input);
			}
			if (input == '\0')
			{
				return;
			}
			this.Insert(input);
		}

		// Token: 0x060048C8 RID: 18632 RVA: 0x00152CD4 File Offset: 0x00150ED4
		protected void UpdateLabel()
		{
			if (this.m_TextComponent != null && this.m_TextComponent.font != null)
			{
				string text;
				if (Input.compositionString.Length > 0)
				{
					text = this.text.Substring(0, this.m_StringPosition) + Input.compositionString + this.text.Substring(this.m_StringPosition);
				}
				else
				{
					text = this.text;
				}
				string str;
				if (this.inputType == TMP_InputField.InputType.Password)
				{
					str = new string(this.asteriskChar, text.Length);
				}
				else
				{
					str = text;
				}
				bool flag = string.IsNullOrEmpty(text);
				if (this.m_Placeholder != null)
				{
					this.m_Placeholder.enabled = flag;
				}
				if (!this.m_AllowInput)
				{
					this.m_DrawStart = 0;
					this.m_DrawEnd = this.m_Text.Length;
				}
				if (!flag)
				{
					this.SetCaretVisible();
				}
				this.m_TextComponent.text = str + "​";
				this.MarkGeometryAsDirty();
			}
		}

		// Token: 0x060048C9 RID: 18633 RVA: 0x00152DD4 File Offset: 0x00150FD4
		private int GetCaretPositionFromStringIndex(int stringIndex)
		{
			int characterCount = this.m_TextComponent.textInfo.characterCount;
			for (int i = 0; i < characterCount; i++)
			{
				if ((int)this.m_TextComponent.textInfo.characterInfo[i].index >= stringIndex)
				{
					return i;
				}
			}
			return characterCount;
		}

		// Token: 0x060048CA RID: 18634 RVA: 0x00152E1F File Offset: 0x0015101F
		private int GetStringIndexFromCaretPosition(int caretPosition)
		{
			return (int)this.m_TextComponent.textInfo.characterInfo[caretPosition].index;
		}

		// Token: 0x060048CB RID: 18635 RVA: 0x00152E3C File Offset: 0x0015103C
		public void ForceLabelUpdate()
		{
			this.UpdateLabel();
		}

		// Token: 0x060048CC RID: 18636 RVA: 0x00152E44 File Offset: 0x00151044
		private void MarkGeometryAsDirty()
		{
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		// Token: 0x060048CD RID: 18637 RVA: 0x00152E4C File Offset: 0x0015104C
		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.LatePreRender)
			{
				this.UpdateGeometry();
			}
		}

		// Token: 0x060048CE RID: 18638 RVA: 0x00152E58 File Offset: 0x00151058
		public virtual void LayoutComplete()
		{
		}

		// Token: 0x060048CF RID: 18639 RVA: 0x00152E5A File Offset: 0x0015105A
		public virtual void GraphicUpdateComplete()
		{
		}

		// Token: 0x060048D0 RID: 18640 RVA: 0x00152E5C File Offset: 0x0015105C
		private void UpdateGeometry()
		{
			if (!this.shouldHideMobileInput)
			{
				return;
			}
			if (this.m_CachedInputRenderer == null && this.m_TextComponent != null)
			{
				GameObject gameObject = new GameObject(base.transform.name + " Input Caret");
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(this.m_TextComponent.transform.parent);
				gameObject.transform.SetAsFirstSibling();
				gameObject.layer = base.gameObject.layer;
				this.caretRectTrans = gameObject.AddComponent<RectTransform>();
				this.m_CachedInputRenderer = gameObject.AddComponent<CanvasRenderer>();
				this.m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
				gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				this.AssignPositioningIfNeeded();
			}
			if (this.m_CachedInputRenderer == null)
			{
				return;
			}
			this.OnFillVBO(this.mesh);
			this.m_CachedInputRenderer.SetMesh(this.mesh);
		}

		// Token: 0x060048D1 RID: 18641 RVA: 0x00152F5C File Offset: 0x0015115C
		private void AssignPositioningIfNeeded()
		{
			if (this.m_TextComponent != null && this.caretRectTrans != null && (this.caretRectTrans.localPosition != this.m_TextComponent.rectTransform.localPosition || this.caretRectTrans.localRotation != this.m_TextComponent.rectTransform.localRotation || this.caretRectTrans.localScale != this.m_TextComponent.rectTransform.localScale || this.caretRectTrans.anchorMin != this.m_TextComponent.rectTransform.anchorMin || this.caretRectTrans.anchorMax != this.m_TextComponent.rectTransform.anchorMax || this.caretRectTrans.anchoredPosition != this.m_TextComponent.rectTransform.anchoredPosition || this.caretRectTrans.sizeDelta != this.m_TextComponent.rectTransform.sizeDelta || this.caretRectTrans.pivot != this.m_TextComponent.rectTransform.pivot))
			{
				this.caretRectTrans.localPosition = this.m_TextComponent.rectTransform.localPosition;
				this.caretRectTrans.localRotation = this.m_TextComponent.rectTransform.localRotation;
				this.caretRectTrans.localScale = this.m_TextComponent.rectTransform.localScale;
				this.caretRectTrans.anchorMin = this.m_TextComponent.rectTransform.anchorMin;
				this.caretRectTrans.anchorMax = this.m_TextComponent.rectTransform.anchorMax;
				this.caretRectTrans.anchoredPosition = this.m_TextComponent.rectTransform.anchoredPosition;
				this.caretRectTrans.sizeDelta = this.m_TextComponent.rectTransform.sizeDelta;
				this.caretRectTrans.pivot = this.m_TextComponent.rectTransform.pivot;
			}
		}

		// Token: 0x060048D2 RID: 18642 RVA: 0x00153184 File Offset: 0x00151384
		private void OnFillVBO(Mesh vbo)
		{
			using (VertexHelper vertexHelper = new VertexHelper())
			{
				if (!this.isFocused)
				{
					vertexHelper.FillMesh(vbo);
				}
				else
				{
					if (!this.hasSelection)
					{
						this.GenerateCaret(vertexHelper, Vector2.zero);
					}
					else
					{
						this.GenerateHightlight(vertexHelper, Vector2.zero);
					}
					vertexHelper.FillMesh(vbo);
				}
			}
		}

		// Token: 0x060048D3 RID: 18643 RVA: 0x001531F0 File Offset: 0x001513F0
		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (!this.m_CaretVisible)
			{
				return;
			}
			if (this.m_CursorVerts == null)
			{
				this.CreateCursorVerts();
			}
			float num = (float)this.m_CaretWidth;
			int characterCount = this.m_TextComponent.textInfo.characterCount;
			Vector2 zero = Vector2.zero;
			this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
			TMP_CharacterInfo tmp_CharacterInfo;
			float num2;
			if (this.caretPositionInternal == 0)
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[0];
				zero = new Vector2(tmp_CharacterInfo.origin, tmp_CharacterInfo.descender);
				num2 = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
			}
			else if (this.caretPositionInternal < characterCount)
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal];
				zero = new Vector2(tmp_CharacterInfo.origin, tmp_CharacterInfo.descender);
				num2 = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
			}
			else
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[characterCount - 1];
				zero = new Vector2(tmp_CharacterInfo.xAdvance, tmp_CharacterInfo.descender);
				num2 = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
			}
			this.AdjustRectTransformRelativeToViewport(zero, num2, tmp_CharacterInfo.isVisible);
			float num3 = zero.y + num2;
			float y = num3 - Mathf.Min(num2, this.m_TextComponent.rectTransform.rect.height);
			this.m_CursorVerts[0].position = new Vector3(zero.x, y, 0f);
			this.m_CursorVerts[1].position = new Vector3(zero.x, num3, 0f);
			this.m_CursorVerts[2].position = new Vector3(zero.x + num, num3, 0f);
			this.m_CursorVerts[3].position = new Vector3(zero.x + num, y, 0f);
			this.m_CursorVerts[0].color = this.caretColor;
			this.m_CursorVerts[1].color = this.caretColor;
			this.m_CursorVerts[2].color = this.caretColor;
			this.m_CursorVerts[3].color = this.caretColor;
			vbo.AddUIVertexQuad(this.m_CursorVerts);
			int height = Screen.height;
			zero.y = (float)height - zero.y;
			Input.compositionCursorPos = zero;
		}

		// Token: 0x060048D4 RID: 18644 RVA: 0x00153488 File Offset: 0x00151688
		private void CreateCursorVerts()
		{
			this.m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i] = UIVertex.simpleVert;
				this.m_CursorVerts[i].uv0 = Vector2.zero;
			}
		}

		// Token: 0x060048D5 RID: 18645 RVA: 0x001534E0 File Offset: 0x001516E0
		private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
		{
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
			this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			Vector2 startPosition;
			float height;
			if (this.caretSelectPositionInternal < textInfo.characterCount)
			{
				startPosition = new Vector2(textInfo.characterInfo[this.caretSelectPositionInternal].origin, textInfo.characterInfo[this.caretSelectPositionInternal].descender);
				height = textInfo.characterInfo[this.caretSelectPositionInternal].ascender - textInfo.characterInfo[this.caretSelectPositionInternal].descender;
			}
			else
			{
				startPosition = new Vector2(textInfo.characterInfo[this.caretSelectPositionInternal - 1].xAdvance, textInfo.characterInfo[this.caretSelectPositionInternal - 1].descender);
				height = textInfo.characterInfo[this.caretSelectPositionInternal - 1].ascender - textInfo.characterInfo[this.caretSelectPositionInternal - 1].descender;
			}
			this.AdjustRectTransformRelativeToViewport(startPosition, height, true);
			int num = Mathf.Max(0, this.caretPositionInternal);
			int num2 = Mathf.Max(0, this.caretSelectPositionInternal);
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			num2--;
			int num4 = (int)textInfo.characterInfo[num].lineNumber;
			int lastCharacterIndex = textInfo.lineInfo[num4].lastCharacterIndex;
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.uv0 = Vector2.zero;
			simpleVert.color = this.selectionColor;
			int num5 = num;
			while (num5 <= num2 && num5 < textInfo.characterCount)
			{
				if (num5 == lastCharacterIndex || num5 == num2)
				{
					TMP_CharacterInfo tmp_CharacterInfo = textInfo.characterInfo[num];
					TMP_CharacterInfo tmp_CharacterInfo2 = textInfo.characterInfo[num5];
					Vector2 vector = new Vector2(tmp_CharacterInfo.origin, tmp_CharacterInfo.ascender);
					Vector2 vector2 = new Vector2(tmp_CharacterInfo2.xAdvance, tmp_CharacterInfo2.descender);
					Vector2 min = this.m_TextViewport.rect.min;
					Vector2 max = this.m_TextViewport.rect.max;
					float num6 = this.m_TextComponent.rectTransform.anchoredPosition.x + vector.x - min.x;
					if (num6 < 0f)
					{
						vector.x -= num6;
					}
					float num7 = this.m_TextComponent.rectTransform.anchoredPosition.y + vector2.y - min.y;
					if (num7 < 0f)
					{
						vector2.y -= num7;
					}
					float num8 = max.x - (this.m_TextComponent.rectTransform.anchoredPosition.x + vector2.x);
					if (num8 < 0f)
					{
						vector2.x += num8;
					}
					float num9 = max.y - (this.m_TextComponent.rectTransform.anchoredPosition.y + vector.y);
					if (num9 < 0f)
					{
						vector.y += num9;
					}
					if (this.m_TextComponent.rectTransform.anchoredPosition.y + vector.y >= min.y && this.m_TextComponent.rectTransform.anchoredPosition.y + vector2.y <= max.y)
					{
						int currentVertCount = vbo.currentVertCount;
						simpleVert.position = new Vector3(vector.x, vector2.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector2.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector2.x, vector.y, 0f);
						vbo.AddVert(simpleVert);
						simpleVert.position = new Vector3(vector.x, vector.y, 0f);
						vbo.AddVert(simpleVert);
						vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
						vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
					}
					num = num5 + 1;
					num4++;
					if (num4 < textInfo.lineCount)
					{
						lastCharacterIndex = textInfo.lineInfo[num4].lastCharacterIndex;
					}
				}
				num5++;
			}
		}

		// Token: 0x060048D6 RID: 18646 RVA: 0x0015395C File Offset: 0x00151B5C
		private void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
		{
			float xMin = this.m_TextViewport.rect.xMin;
			float xMax = this.m_TextViewport.rect.xMax;
			float num = xMax - (this.m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x + this.m_TextComponent.margin.z);
			if (num < 0f && (!this.multiLine || (this.multiLine && isCharVisible)))
			{
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(num, 0f);
				this.AssignPositioningIfNeeded();
			}
			float num2 = this.m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x - this.m_TextComponent.margin.x - xMin;
			if (num2 < 0f)
			{
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(-num2, 0f);
				this.AssignPositioningIfNeeded();
			}
			if (this.m_LineType != TMP_InputField.LineType.SingleLine)
			{
				float num3 = this.m_TextViewport.rect.yMax - (this.m_TextComponent.rectTransform.anchoredPosition.y + startPosition.y + height);
				if (num3 < -0.0001f)
				{
					this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, num3);
					this.AssignPositioningIfNeeded();
				}
				float num4 = this.m_TextComponent.rectTransform.anchoredPosition.y + startPosition.y - this.m_TextViewport.rect.yMin;
				if (num4 < 0f)
				{
					this.m_TextComponent.rectTransform.anchoredPosition -= new Vector2(0f, num4);
					this.AssignPositioningIfNeeded();
				}
			}
			if (this.m_isLastKeyBackspace)
			{
				float num5 = this.m_TextComponent.rectTransform.anchoredPosition.x + this.m_TextComponent.textInfo.characterInfo[0].origin - this.m_TextComponent.margin.x;
				float num6 = this.m_TextComponent.rectTransform.anchoredPosition.x + this.m_TextComponent.textInfo.characterInfo[this.m_TextComponent.textInfo.characterCount - 1].origin + this.m_TextComponent.margin.z;
				if (this.m_TextComponent.rectTransform.anchoredPosition.x + startPosition.x <= xMin + 0.0001f)
				{
					if (num5 < xMin)
					{
						float x = Mathf.Min((xMax - xMin) / 2f, xMin - num5);
						this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(x, 0f);
						this.AssignPositioningIfNeeded();
					}
				}
				else if (num6 < xMax && num5 < xMin)
				{
					float x2 = Mathf.Min(xMax - num6, xMin - num5);
					this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(x2, 0f);
					this.AssignPositioningIfNeeded();
				}
				this.m_isLastKeyBackspace = false;
			}
		}

		// Token: 0x060048D7 RID: 18647 RVA: 0x00153CB0 File Offset: 0x00151EB0
		protected char Validate(string text, int pos, char ch)
		{
			if (this.characterValidation == TMP_InputField.CharacterValidation.None || !base.enabled)
			{
				return ch;
			}
			if (this.characterValidation == TMP_InputField.CharacterValidation.Integer || this.characterValidation == TMP_InputField.CharacterValidation.Decimal)
			{
				bool flag = pos == 0 && text.Length > 0 && text[0] == '-';
				bool flag2 = this.stringPositionInternal == 0 || this.stringSelectPositionInternal == 0;
				if (!flag)
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '-' && (pos == 0 || flag2))
					{
						return ch;
					}
					if (ch == '.' && this.characterValidation == TMP_InputField.CharacterValidation.Decimal && !text.Contains("."))
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Alphanumeric)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Name)
			{
				char c = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
				char c2 = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
				if (char.IsLetter(ch))
				{
					if (char.IsLower(ch) && c == ' ')
					{
						return char.ToUpper(ch);
					}
					if (char.IsUpper(ch) && c != ' ' && c != '\'')
					{
						return char.ToLower(ch);
					}
					return ch;
				}
				else if (ch == '\'')
				{
					if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
					{
						return ch;
					}
				}
				else if (ch == ' ' && c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
				{
					return ch;
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.EmailAddress)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '@' && text.IndexOf('@') == -1)
				{
					return ch;
				}
				if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
				{
					return ch;
				}
				if (ch == '.')
				{
					int num = (int)((text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ');
					char c3 = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
					if (num != 46 && c3 != '.')
					{
						return ch;
					}
				}
			}
			return '\0';
		}

		// Token: 0x060048D8 RID: 18648 RVA: 0x00153F18 File Offset: 0x00152118
		public void ActivateInputField()
		{
			if (this.m_TextComponent == null || this.m_TextComponent.font == null || !this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (this.isFocused && this.m_Keyboard != null && !this.m_Keyboard.active)
			{
				this.m_Keyboard.active = true;
				this.m_Keyboard.text = this.m_Text;
			}
			this.m_HasLostFocus = false;
			this.m_ShouldActivateNextUpdate = true;
		}

		// Token: 0x060048D9 RID: 18649 RVA: 0x00153FA0 File Offset: 0x001521A0
		private void ActivateInputFieldInternal()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			if (EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
			if (TouchScreenKeyboard.isSupported)
			{
				if (Input.touchSupported)
				{
					TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
				}
				this.m_Keyboard = ((this.inputType == TMP_InputField.InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == TMP_InputField.InputType.AutoCorrect, this.multiLine));
				this.MoveTextEnd(false);
			}
			else
			{
				Input.imeCompositionMode = IMECompositionMode.On;
				this.OnFocus();
			}
			this.m_AllowInput = true;
			this.m_OriginalText = this.text;
			this.m_WasCanceled = false;
			this.SetCaretVisible();
			this.UpdateLabel();
		}

		// Token: 0x060048DA RID: 18650 RVA: 0x00154080 File Offset: 0x00152280
		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.ActivateInputField();
		}

		// Token: 0x060048DB RID: 18651 RVA: 0x0015408F File Offset: 0x0015228F
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			this.ActivateInputField();
		}

		// Token: 0x060048DC RID: 18652 RVA: 0x001540A0 File Offset: 0x001522A0
		public void DeactivateInputField()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_HasDoneFocusTransition = false;
			this.m_AllowInput = false;
			if (this.m_Placeholder != null)
			{
				this.m_Placeholder.enabled = string.IsNullOrEmpty(this.m_Text);
			}
			if (this.m_TextComponent != null && this.IsInteractable())
			{
				if (this.m_WasCanceled)
				{
					this.text = this.m_OriginalText;
				}
				if (this.m_Keyboard != null)
				{
					this.m_Keyboard.active = false;
					this.m_Keyboard = null;
				}
				this.m_StringPosition = (this.m_StringSelectPosition = 0);
				this.m_TextComponent.rectTransform.localPosition = Vector3.zero;
				if (this.caretRectTrans != null)
				{
					this.caretRectTrans.localPosition = Vector3.zero;
				}
				this.SendOnSubmit();
				if (this.m_HasLostFocus)
				{
					this.SendOnFocusLost();
				}
				Input.imeCompositionMode = IMECompositionMode.Auto;
			}
			this.MarkGeometryAsDirty();
		}

		// Token: 0x060048DD RID: 18653 RVA: 0x00154196 File Offset: 0x00152396
		public override void OnDeselect(BaseEventData eventData)
		{
			this.m_HasLostFocus = true;
			this.DeactivateInputField();
			base.OnDeselect(eventData);
		}

		// Token: 0x060048DE RID: 18654 RVA: 0x001541AC File Offset: 0x001523AC
		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (!this.isFocused)
			{
				this.m_ShouldActivateNextUpdate = true;
			}
		}

		// Token: 0x060048DF RID: 18655 RVA: 0x001541D0 File Offset: 0x001523D0
		private void EnforceContentType()
		{
			switch (this.contentType)
			{
			case TMP_InputField.ContentType.Standard:
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				return;
			case TMP_InputField.ContentType.Autocorrected:
				this.m_InputType = TMP_InputField.InputType.AutoCorrect;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				return;
			case TMP_InputField.ContentType.IntegerNumber:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Integer;
				return;
			case TMP_InputField.ContentType.DecimalNumber:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Decimal;
				return;
			case TMP_InputField.ContentType.Alphanumeric:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
				return;
			case TMP_InputField.ContentType.Name:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Name;
				return;
			case TMP_InputField.ContentType.EmailAddress:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.EmailAddress;
				return;
			case TMP_InputField.ContentType.Password:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				return;
			case TMP_InputField.ContentType.Pin:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_TextComponent.enableWordWrapping = false;
				this.m_InputType = TMP_InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Integer;
				return;
			default:
				return;
			}
		}

		// Token: 0x060048E0 RID: 18656 RVA: 0x00154359 File Offset: 0x00152559
		private void SetTextComponentWrapMode()
		{
			if (this.m_TextComponent == null)
			{
				return;
			}
			if (this.m_LineType == TMP_InputField.LineType.SingleLine)
			{
				this.m_TextComponent.enableWordWrapping = false;
				return;
			}
			this.m_TextComponent.enableWordWrapping = true;
		}

		// Token: 0x060048E1 RID: 18657 RVA: 0x0015438B File Offset: 0x0015258B
		private void SetTextComponentRichTextMode()
		{
			if (this.m_TextComponent == null)
			{
				return;
			}
			this.m_TextComponent.richText = this.m_RichText;
		}

		// Token: 0x060048E2 RID: 18658 RVA: 0x001543B0 File Offset: 0x001525B0
		private void SetToCustomIfContentTypeIsNot(params TMP_InputField.ContentType[] allowedContentTypes)
		{
			if (this.contentType == TMP_InputField.ContentType.Custom)
			{
				return;
			}
			for (int i = 0; i < allowedContentTypes.Length; i++)
			{
				if (this.contentType == allowedContentTypes[i])
				{
					return;
				}
			}
			this.contentType = TMP_InputField.ContentType.Custom;
		}

		// Token: 0x060048E3 RID: 18659 RVA: 0x001543EA File Offset: 0x001525EA
		private void SetToCustom()
		{
			if (this.contentType == TMP_InputField.ContentType.Custom)
			{
				return;
			}
			this.contentType = TMP_InputField.ContentType.Custom;
		}

		// Token: 0x060048E4 RID: 18660 RVA: 0x001543FF File Offset: 0x001525FF
		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.m_HasDoneFocusTransition)
			{
				state = Selectable.SelectionState.Highlighted;
			}
			else if (state == Selectable.SelectionState.Pressed)
			{
				this.m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
		}

		// Token: 0x060048E6 RID: 18662 RVA: 0x00154439 File Offset: 0x00152639
		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		// Token: 0x040048B7 RID: 18615
		protected TouchScreenKeyboard m_Keyboard;

		// Token: 0x040048B8 RID: 18616
		private static readonly char[] kSeparators = new char[]
		{
			' ',
			'.',
			',',
			'\t',
			'\r',
			'\n'
		};

		// Token: 0x040048B9 RID: 18617
		[SerializeField]
		protected RectTransform m_TextViewport;

		// Token: 0x040048BA RID: 18618
		[SerializeField]
		protected TMP_Text m_TextComponent;

		// Token: 0x040048BB RID: 18619
		protected RectTransform m_TextComponentRectTransform;

		// Token: 0x040048BC RID: 18620
		[SerializeField]
		protected Graphic m_Placeholder;

		// Token: 0x040048BD RID: 18621
		[SerializeField]
		private TMP_InputField.ContentType m_ContentType;

		// Token: 0x040048BE RID: 18622
		[SerializeField]
		private TMP_InputField.InputType m_InputType;

		// Token: 0x040048BF RID: 18623
		[SerializeField]
		private char m_AsteriskChar = '*';

		// Token: 0x040048C0 RID: 18624
		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		// Token: 0x040048C1 RID: 18625
		[SerializeField]
		private TMP_InputField.LineType m_LineType;

		// Token: 0x040048C2 RID: 18626
		[SerializeField]
		private bool m_HideMobileInput;

		// Token: 0x040048C3 RID: 18627
		[SerializeField]
		private TMP_InputField.CharacterValidation m_CharacterValidation;

		// Token: 0x040048C4 RID: 18628
		[SerializeField]
		private int m_CharacterLimit;

		// Token: 0x040048C5 RID: 18629
		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnEndEdit = new TMP_InputField.SubmitEvent();

		// Token: 0x040048C6 RID: 18630
		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnSubmit = new TMP_InputField.SubmitEvent();

		// Token: 0x040048C7 RID: 18631
		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnFocusLost = new TMP_InputField.SubmitEvent();

		// Token: 0x040048C8 RID: 18632
		[SerializeField]
		private TMP_InputField.OnChangeEvent m_OnValueChanged = new TMP_InputField.OnChangeEvent();

		// Token: 0x040048C9 RID: 18633
		[SerializeField]
		private TMP_InputField.OnValidateInput m_OnValidateInput;

		// Token: 0x040048CA RID: 18634
		[SerializeField]
		private Color m_CaretColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

		// Token: 0x040048CB RID: 18635
		[SerializeField]
		private bool m_CustomCaretColor;

		// Token: 0x040048CC RID: 18636
		[SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.80784315f, 1f, 0.7529412f);

		// Token: 0x040048CD RID: 18637
		[SerializeField]
		protected string m_Text = string.Empty;

		// Token: 0x040048CE RID: 18638
		[SerializeField]
		[Range(0f, 4f)]
		private float m_CaretBlinkRate = 0.85f;

		// Token: 0x040048CF RID: 18639
		[SerializeField]
		[Range(1f, 5f)]
		private int m_CaretWidth = 1;

		// Token: 0x040048D0 RID: 18640
		[SerializeField]
		private bool m_ReadOnly;

		// Token: 0x040048D1 RID: 18641
		[SerializeField]
		private bool m_RichText = true;

		// Token: 0x040048D2 RID: 18642
		protected int m_StringPosition;

		// Token: 0x040048D3 RID: 18643
		protected int m_StringSelectPosition;

		// Token: 0x040048D4 RID: 18644
		protected int m_CaretPosition;

		// Token: 0x040048D5 RID: 18645
		protected int m_CaretSelectPosition;

		// Token: 0x040048D6 RID: 18646
		private RectTransform caretRectTrans;

		// Token: 0x040048D7 RID: 18647
		protected UIVertex[] m_CursorVerts;

		// Token: 0x040048D8 RID: 18648
		private CanvasRenderer m_CachedInputRenderer;

		// Token: 0x040048D9 RID: 18649
		[NonSerialized]
		protected Mesh m_Mesh;

		// Token: 0x040048DA RID: 18650
		private bool m_AllowInput;

		// Token: 0x040048DB RID: 18651
		private bool m_HasLostFocus;

		// Token: 0x040048DC RID: 18652
		private bool m_ShouldActivateNextUpdate;

		// Token: 0x040048DD RID: 18653
		private bool m_UpdateDrag;

		// Token: 0x040048DE RID: 18654
		private bool m_DragPositionOutOfBounds;

		// Token: 0x040048DF RID: 18655
		private const float kHScrollSpeed = 0.05f;

		// Token: 0x040048E0 RID: 18656
		private const float kVScrollSpeed = 0.1f;

		// Token: 0x040048E1 RID: 18657
		protected bool m_CaretVisible;

		// Token: 0x040048E2 RID: 18658
		private Coroutine m_BlinkCoroutine;

		// Token: 0x040048E3 RID: 18659
		private float m_BlinkStartTime;

		// Token: 0x040048E4 RID: 18660
		protected int m_DrawStart;

		// Token: 0x040048E5 RID: 18661
		protected int m_DrawEnd;

		// Token: 0x040048E6 RID: 18662
		private Coroutine m_DragCoroutine;

		// Token: 0x040048E7 RID: 18663
		private string m_OriginalText = "";

		// Token: 0x040048E8 RID: 18664
		private bool m_WasCanceled;

		// Token: 0x040048E9 RID: 18665
		private bool m_HasDoneFocusTransition;

		// Token: 0x040048EA RID: 18666
		private bool m_isLastKeyBackspace;

		// Token: 0x040048EB RID: 18667
		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		// Token: 0x040048EC RID: 18668
		private bool isCaretInsideTag;

		// Token: 0x040048ED RID: 18669
		private Event m_ProcessingEvent = new Event();

		// Token: 0x02001AC0 RID: 6848
		public enum ContentType
		{
			// Token: 0x04009A62 RID: 39522
			Standard,
			// Token: 0x04009A63 RID: 39523
			Autocorrected,
			// Token: 0x04009A64 RID: 39524
			IntegerNumber,
			// Token: 0x04009A65 RID: 39525
			DecimalNumber,
			// Token: 0x04009A66 RID: 39526
			Alphanumeric,
			// Token: 0x04009A67 RID: 39527
			Name,
			// Token: 0x04009A68 RID: 39528
			EmailAddress,
			// Token: 0x04009A69 RID: 39529
			Password,
			// Token: 0x04009A6A RID: 39530
			Pin,
			// Token: 0x04009A6B RID: 39531
			Custom
		}

		// Token: 0x02001AC1 RID: 6849
		public enum InputType
		{
			// Token: 0x04009A6D RID: 39533
			Standard,
			// Token: 0x04009A6E RID: 39534
			AutoCorrect,
			// Token: 0x04009A6F RID: 39535
			Password
		}

		// Token: 0x02001AC2 RID: 6850
		public enum CharacterValidation
		{
			// Token: 0x04009A71 RID: 39537
			None,
			// Token: 0x04009A72 RID: 39538
			Integer,
			// Token: 0x04009A73 RID: 39539
			Decimal,
			// Token: 0x04009A74 RID: 39540
			Alphanumeric,
			// Token: 0x04009A75 RID: 39541
			Name,
			// Token: 0x04009A76 RID: 39542
			EmailAddress
		}

		// Token: 0x02001AC3 RID: 6851
		public enum LineType
		{
			// Token: 0x04009A78 RID: 39544
			SingleLine,
			// Token: 0x04009A79 RID: 39545
			MultiLineSubmit,
			// Token: 0x04009A7A RID: 39546
			MultiLineNewline
		}

		// Token: 0x02001AC4 RID: 6852
		// (Invoke) Token: 0x060097E0 RID: 38880
		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		// Token: 0x02001AC5 RID: 6853
		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		// Token: 0x02001AC6 RID: 6854
		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		// Token: 0x02001AC7 RID: 6855
		protected enum EditState
		{
			// Token: 0x04009A7C RID: 39548
			Continue,
			// Token: 0x04009A7D RID: 39549
			Finish
		}
	}
}
