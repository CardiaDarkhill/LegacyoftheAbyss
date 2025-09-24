using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001139 RID: 4409
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Set up multiple button events in a single action.")]
	public class UiButtonArray : FsmStateAction
	{
		// Token: 0x060076C3 RID: 30403 RVA: 0x002435F8 File Offset: 0x002417F8
		public override void Reset()
		{
			this.gameObjects = new FsmGameObject[3];
			this.clickEvents = new FsmEvent[3];
			this.clickIndex = null;
		}

		// Token: 0x060076C4 RID: 30404 RVA: 0x0024361C File Offset: 0x0024181C
		public override void OnPreprocess()
		{
			this.buttons = new Button[this.gameObjects.Length];
			this.cachedGameObjects = new GameObject[this.gameObjects.Length];
			this.actions = new UnityAction[this.gameObjects.Length];
			this.InitButtons();
		}

		// Token: 0x060076C5 RID: 30405 RVA: 0x00243668 File Offset: 0x00241868
		private void InitButtons()
		{
			if (this.cachedGameObjects == null || this.cachedGameObjects.Length != this.gameObjects.Length)
			{
				this.OnPreprocess();
			}
			for (int i = 0; i < this.gameObjects.Length; i++)
			{
				GameObject value = this.gameObjects[i].Value;
				if (value != null && this.cachedGameObjects[i] != value)
				{
					this.buttons[i] = value.GetComponent<Button>();
					this.cachedGameObjects[i] = value;
				}
			}
		}

		// Token: 0x060076C6 RID: 30406 RVA: 0x002436E8 File Offset: 0x002418E8
		public override void OnEnter()
		{
			this.InitButtons();
			for (int i = 0; i < this.buttons.Length; i++)
			{
				Button button = this.buttons[i];
				if (!(button == null))
				{
					int index = i;
					this.actions[i] = delegate()
					{
						this.OnClick(index);
					};
					button.onClick.AddListener(this.actions[i]);
				}
			}
		}

		// Token: 0x060076C7 RID: 30407 RVA: 0x0024375C File Offset: 0x0024195C
		public override void OnExit()
		{
			for (int i = 0; i < this.gameObjects.Length; i++)
			{
				FsmGameObject fsmGameObject = this.gameObjects[i];
				if (!(fsmGameObject.Value == null))
				{
					fsmGameObject.Value.GetComponent<Button>().onClick.RemoveListener(this.actions[i]);
				}
			}
		}

		// Token: 0x060076C8 RID: 30408 RVA: 0x002437B0 File Offset: 0x002419B0
		public void OnClick(int index)
		{
			this.clickIndex.Value = index;
			base.Fsm.Event(this.gameObjects[index].Value, this.eventTarget, this.clickEvents[index]);
		}

		// Token: 0x04007736 RID: 30518
		[Tooltip("Where to send the events.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04007737 RID: 30519
		[CompoundArray("Buttons", "Button", "Click Event")]
		[CheckForComponent(typeof(Button))]
		[Tooltip("The GameObject with the UI Button component.")]
		public FsmGameObject[] gameObjects;

		// Token: 0x04007738 RID: 30520
		[Tooltip("Send this event when the button is Clicked.")]
		public FsmEvent[] clickEvents;

		// Token: 0x04007739 RID: 30521
		[UIHint(UIHint.Variable)]
		[Tooltip("The index of the last button clicked. (0 = first button, 1 = second, etc.)")]
		public FsmInt clickIndex;

		// Token: 0x0400773A RID: 30522
		[SerializeField]
		private Button[] buttons;

		// Token: 0x0400773B RID: 30523
		[SerializeField]
		private GameObject[] cachedGameObjects;

		// Token: 0x0400773C RID: 30524
		private UnityAction[] actions;
	}
}
