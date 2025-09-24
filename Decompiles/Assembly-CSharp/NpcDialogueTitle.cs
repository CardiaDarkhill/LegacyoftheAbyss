using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000637 RID: 1591
public class NpcDialogueTitle : MonoBehaviour
{
	// Token: 0x06003905 RID: 14597 RVA: 0x000FB3EA File Offset: 0x000F95EA
	private void Reset()
	{
		this.npcControls = base.GetComponents<NPCControlBase>();
	}

	// Token: 0x06003906 RID: 14598 RVA: 0x000FB3F8 File Offset: 0x000F95F8
	private void OnValidate()
	{
		if (this.npcControl)
		{
			this.npcControls = new NPCControlBase[]
			{
				this.npcControl
			};
			this.npcControl = null;
		}
		if (!string.IsNullOrEmpty(this.npcTitle))
		{
			this.titles = new NpcDialogueTitle.SpeakerTitle[]
			{
				new NpcDialogueTitle.SpeakerTitle
				{
					Title = this.npcTitle
				}
			};
			this.npcTitle = null;
		}
	}

	// Token: 0x06003907 RID: 14599 RVA: 0x000FB462 File Offset: 0x000F9662
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06003908 RID: 14600 RVA: 0x000FB46C File Offset: 0x000F966C
	private void OnEnable()
	{
		foreach (NPCControlBase npccontrolBase in this.npcControls)
		{
			if (npccontrolBase)
			{
				npccontrolBase.OpeningDialogueBox += this.Show;
				npccontrolBase.EndingDialogue += this.Hide;
			}
		}
	}

	// Token: 0x06003909 RID: 14601 RVA: 0x000FB4C0 File Offset: 0x000F96C0
	private void OnDisable()
	{
		foreach (NPCControlBase npccontrolBase in this.npcControls)
		{
			if (npccontrolBase)
			{
				npccontrolBase.OpeningDialogueBox -= this.Show;
				npccontrolBase.EndingDialogue -= this.Hide;
			}
		}
		this.skipNextHide = false;
		if (this.isShowing)
		{
			this.Hide();
		}
	}

	// Token: 0x0600390A RID: 14602 RVA: 0x000FB527 File Offset: 0x000F9727
	public void EnableAndShow()
	{
		if (base.enabled)
		{
			return;
		}
		base.enabled = true;
		this.Show(this.previousFirstLine);
	}

	// Token: 0x0600390B RID: 14603 RVA: 0x000FB548 File Offset: 0x000F9748
	private void Show(DialogueBox.DialogueLine firstLine)
	{
		this.previousFirstLine = firstLine;
		if (this.isShowing)
		{
			return;
		}
		this.isShowing = true;
		NpcDialogueTitle.SpeakerTitle speakerTitle = null;
		foreach (NpcDialogueTitle.SpeakerTitle speakerTitle2 in this.titles)
		{
			if (speakerTitle == null || speakerTitle2.SpeakerEvent == firstLine.Event)
			{
				speakerTitle = speakerTitle2;
			}
		}
		if (speakerTitle == null)
		{
			Debug.LogError("No NPC title found for speaker event: " + firstLine.Event, this);
			return;
		}
		AreaTitle instance = ManagerSingleton<AreaTitle>.Instance;
		if (!instance)
		{
			return;
		}
		GameObject gameObject = instance.gameObject;
		PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(gameObject, "Area Title Control");
		bool value = gameObjectFsm.FsmVariables.FindFsmBool("NPC Title Waiting").Value;
		FsmString fsmString = gameObjectFsm.FsmVariables.FindFsmString("Area Event");
		if (value && fsmString.Value == speakerTitle.Title)
		{
			gameObjectFsm.SendEventSafe("NPC TITLE DOWN CANCEL");
			return;
		}
		gameObject.SetActive(false);
		gameObjectFsm.FsmVariables.FindFsmBool("Visited").Value = true;
		gameObjectFsm.FsmVariables.FindFsmBool("NPC Title").Value = true;
		gameObjectFsm.FsmVariables.FindFsmBool("Display Right").Value = this.displayRight;
		fsmString.Value = speakerTitle.Title;
		gameObject.SetActive(true);
	}

	// Token: 0x0600390C RID: 14604 RVA: 0x000FB698 File Offset: 0x000F9898
	public void Hide()
	{
		if (this.skipNextHide)
		{
			this.skipNextHide = false;
			return;
		}
		this.isShowing = false;
		AreaTitle instance = ManagerSingleton<AreaTitle>.Instance;
		if (!instance)
		{
			return;
		}
		ActionHelpers.GetGameObjectFsm(instance.gameObject, "Area Title Control").SendEventSafe("NPC TITLE DOWN");
	}

	// Token: 0x0600390D RID: 14605 RVA: 0x000FB6E5 File Offset: 0x000F98E5
	public void SkipNextHide()
	{
		this.skipNextHide = true;
	}

	// Token: 0x04003BDE RID: 15326
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private NPCControlBase npcControl;

	// Token: 0x04003BDF RID: 15327
	[SerializeField]
	private NPCControlBase[] npcControls;

	// Token: 0x04003BE0 RID: 15328
	[Space]
	[SerializeField]
	private bool displayRight;

	// Token: 0x04003BE1 RID: 15329
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private string npcTitle;

	// Token: 0x04003BE2 RID: 15330
	[SerializeField]
	private NpcDialogueTitle.SpeakerTitle[] titles;

	// Token: 0x04003BE3 RID: 15331
	private bool isShowing;

	// Token: 0x04003BE4 RID: 15332
	private DialogueBox.DialogueLine previousFirstLine;

	// Token: 0x04003BE5 RID: 15333
	private bool skipNextHide;

	// Token: 0x02001958 RID: 6488
	[Serializable]
	private class SpeakerTitle
	{
		// Token: 0x04009571 RID: 38257
		public string Title;

		// Token: 0x04009572 RID: 38258
		public string SpeakerEvent;
	}
}
