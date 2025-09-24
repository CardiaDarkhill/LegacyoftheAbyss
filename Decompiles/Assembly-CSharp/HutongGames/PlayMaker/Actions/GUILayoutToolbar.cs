using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EFD RID: 3837
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("GUILayout Toolbar. NOTE: Arrays must be the same length as NumButtons or empty.")]
	public class GUILayoutToolbar : GUILayoutAction
	{
		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x06006B6F RID: 27503 RVA: 0x00217108 File Offset: 0x00215308
		public GUIContent[] Contents
		{
			get
			{
				if (this.contents == null)
				{
					this.SetButtonsContent();
				}
				return this.contents;
			}
		}

		// Token: 0x06006B70 RID: 27504 RVA: 0x00217120 File Offset: 0x00215320
		private void SetButtonsContent()
		{
			if (this.contents == null)
			{
				this.contents = new GUIContent[this.numButtons.Value];
			}
			for (int i = 0; i < this.numButtons.Value; i++)
			{
				this.contents[i] = new GUIContent();
			}
			for (int j = 0; j < this.imagesArray.Length; j++)
			{
				this.contents[j].image = this.imagesArray[j].Value;
			}
			for (int k = 0; k < this.textsArray.Length; k++)
			{
				this.contents[k].text = this.textsArray[k].Value;
			}
			for (int l = 0; l < this.tooltipsArray.Length; l++)
			{
				this.contents[l].tooltip = this.tooltipsArray[l].Value;
			}
		}

		// Token: 0x06006B71 RID: 27505 RVA: 0x002171F8 File Offset: 0x002153F8
		public override void Reset()
		{
			base.Reset();
			this.numButtons = 0;
			this.selectedButton = null;
			this.buttonEventsArray = new FsmEvent[0];
			this.imagesArray = new FsmTexture[0];
			this.tooltipsArray = new FsmString[0];
			this.style = "Button";
			this.everyFrame = false;
		}

		// Token: 0x06006B72 RID: 27506 RVA: 0x0021725C File Offset: 0x0021545C
		public override void OnEnter()
		{
			string text = this.ErrorCheck();
			if (!string.IsNullOrEmpty(text))
			{
				base.LogError(text);
				base.Finish();
			}
		}

		// Token: 0x06006B73 RID: 27507 RVA: 0x00217288 File Offset: 0x00215488
		public override void OnGUI()
		{
			if (this.everyFrame)
			{
				this.SetButtonsContent();
			}
			bool changed = GUI.changed;
			GUI.changed = false;
			this.selectedButton.Value = GUILayout.Toolbar(this.selectedButton.Value, this.Contents, this.style.Value, base.LayoutOptions);
			if (GUI.changed)
			{
				if (this.selectedButton.Value < this.buttonEventsArray.Length)
				{
					base.Fsm.Event(this.buttonEventsArray[this.selectedButton.Value]);
					GUIUtility.ExitGUI();
					return;
				}
			}
			else
			{
				GUI.changed = changed;
			}
		}

		// Token: 0x06006B74 RID: 27508 RVA: 0x0021732C File Offset: 0x0021552C
		public override string ErrorCheck()
		{
			string text = "";
			if (this.imagesArray.Length != 0 && this.imagesArray.Length != this.numButtons.Value)
			{
				text += "Images array doesn't match NumButtons.\n";
			}
			if (this.textsArray.Length != 0 && this.textsArray.Length != this.numButtons.Value)
			{
				text += "Texts array doesn't match NumButtons.\n";
			}
			if (this.tooltipsArray.Length != 0 && this.tooltipsArray.Length != this.numButtons.Value)
			{
				text += "Tooltips array doesn't match NumButtons.\n";
			}
			return text;
		}

		// Token: 0x04006AC1 RID: 27329
		[Tooltip("The number of buttons in the toolbar")]
		public FsmInt numButtons;

		// Token: 0x04006AC2 RID: 27330
		[Tooltip("Store the index of the selected button in an Integer Variable")]
		[UIHint(UIHint.Variable)]
		public FsmInt selectedButton;

		// Token: 0x04006AC3 RID: 27331
		[Tooltip("Event to send when each button is pressed.")]
		public FsmEvent[] buttonEventsArray;

		// Token: 0x04006AC4 RID: 27332
		[Tooltip("Image to use on each button.")]
		public FsmTexture[] imagesArray;

		// Token: 0x04006AC5 RID: 27333
		[Tooltip("Text to use on each button.")]
		public FsmString[] textsArray;

		// Token: 0x04006AC6 RID: 27334
		[Tooltip("Tooltip to use for each button.")]
		public FsmString[] tooltipsArray;

		// Token: 0x04006AC7 RID: 27335
		[Tooltip("A named GUIStyle to use for the toolbar buttons. Default is Button.")]
		public FsmString style;

		// Token: 0x04006AC8 RID: 27336
		[Tooltip("Update the content of the buttons every frame. Useful if the buttons are using variables that change.")]
		public bool everyFrame;

		// Token: 0x04006AC9 RID: 27337
		private GUIContent[] contents;
	}
}
