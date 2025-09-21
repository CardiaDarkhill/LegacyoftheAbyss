using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x0200062E RID: 1582
public class DialogueBox : MonoBehaviour
{
	// Token: 0x06003853 RID: 14419 RVA: 0x000F8A25 File Offset: 0x000F6C25
	private void Awake()
	{
		if (!DialogueBox._instance)
		{
			DialogueBox._instance = this;
		}
	}

	// Token: 0x06003854 RID: 14420 RVA: 0x000F8A3C File Offset: 0x000F6C3C
	private void Start()
	{
		this.initialPos = base.transform.localPosition;
		this.initialStopPos = this.stopAnimator.transform.localPosition;
		this.currentRevealSpeed = this.regularRevealSpeed;
		this.group.AlphaSelf = 0f;
		this.animator.Play("Close", 0, 1f);
		EventRegister.GetRegisterGuaranteed(base.gameObject, "LEAVING SCENE").ReceivedEvent += this.OnLeavingScene;
	}

	// Token: 0x06003855 RID: 14421 RVA: 0x000F8AC3 File Offset: 0x000F6CC3
	private void OnDestroy()
	{
		if (DialogueBox._instance == this)
		{
			DialogueBox._instance = null;
		}
	}

	// Token: 0x06003856 RID: 14422 RVA: 0x000F8AD8 File Offset: 0x000F6CD8
	private void OnLeavingScene()
	{
		DialogueBox.RunCancelledCallback();
		DialogueBox.EndConversation(DialogueBox._instance.isDialogueRunning, null);
	}

	// Token: 0x06003857 RID: 14423 RVA: 0x000F8AEF File Offset: 0x000F6CEF
	public static void StartConversation(LocalisedString text, NPCControlBase instigator, bool overrideContinue = false)
	{
		DialogueBox.StartConversation(text, instigator, overrideContinue, DialogueBox.DisplayOptions.Default, null);
	}

	// Token: 0x06003858 RID: 14424 RVA: 0x000F8AFF File Offset: 0x000F6CFF
	public static void StartConversation(LocalisedString text, NPCControlBase instigator, bool overrideContinue, DialogueBox.DisplayOptions displayOptions, Action onDialogueEnd = null)
	{
		DialogueBox.StartConversation(CheatManager.IsDialogueDebugEnabled ? (text.Sheet + " / " + text.Key) : text.ToString(false), instigator, overrideContinue, displayOptions, onDialogueEnd, null);
	}

	// Token: 0x06003859 RID: 14425 RVA: 0x000F8B34 File Offset: 0x000F6D34
	public static void StartConversation(string text, NPCControlBase instigator, bool overrideContinue, DialogueBox.DisplayOptions displayOptions, Action onDialogueEnd = null, Action onDialogueCancelled = null)
	{
		if (!DialogueBox._instance)
		{
			return;
		}
		List<DialogueBox.DialogueLine> lines;
		DialogueBox._instance.ShowShared(text, displayOptions, out lines);
		DialogueBox._instance.instigator = instigator;
		DialogueBox._onCancelledCallback = onDialogueCancelled;
		DialogueBox._instance.StartCoroutine(DialogueBox._instance.RunDialogue(lines, overrideContinue, onDialogueEnd, ++DialogueBox._conversationID));
		DialogueBox._instance.isDialogueRunning = true;
	}

	// Token: 0x0600385A RID: 14426 RVA: 0x000F8BA0 File Offset: 0x000F6DA0
	public static void ShowInstant(string text, DialogueBox.DisplayOptions displayOptions, int lineIndex, int pageIndex, out int lineCount, out int pageCount)
	{
		if (!DialogueBox._instance)
		{
			lineCount = 0;
			pageCount = 0;
			return;
		}
		List<DialogueBox.DialogueLine> list;
		DialogueBox._instance.ShowShared(text, displayOptions, out list);
		DialogueBox._instance.instigator = null;
		lineCount = list.Count;
		DialogueBox.DialogueLine dialogueLine = list[lineIndex];
		DialogueBox._instance.UpdateAppearance(list[lineIndex]);
		DialogueBox._instance.hudFSM.SendEventSafe("OUT INSTANT");
		DialogueBox._instance.animator.Play("Open", 0, 1f);
		DialogueBox._instance.animator.Update(0f);
		DialogueBox._instance.isBoxOpen = true;
		DialogueBox._instance.textMesh.text = dialogueLine.Text;
		DialogueBox._instance.textMesh.maxVisibleCharacters = int.MaxValue;
		TMP_TextInfo textInfo = DialogueBox._instance.textMesh.GetTextInfo(dialogueLine.Text);
		pageCount = textInfo.pageCount;
		DialogueBox._instance.textMesh.pageToDisplay = pageIndex + 1;
	}

	// Token: 0x0600385B RID: 14427 RVA: 0x000F8CA4 File Offset: 0x000F6EA4
	public static void HideInstant()
	{
		DialogueBox._instance.isBoxOpen = false;
		DialogueBox._instance.animator.Play("Close", 0, 1f);
		DialogueBox._instance.animator.Update(0f);
	}

	// Token: 0x0600385C RID: 14428 RVA: 0x000F8CE0 File Offset: 0x000F6EE0
	private void ShowShared(string text, DialogueBox.DisplayOptions displayOptions, out List<DialogueBox.DialogueLine> lines)
	{
		base.transform.localPosition = this.initialPos + new Vector3(0f, displayOptions.OffsetY);
		if (this.textMesh)
		{
			this.textMesh.alignment = displayOptions.Alignment;
			this.textMesh.color = displayOptions.TextColor;
		}
		if (this.stopSprite)
		{
			this.stopSprite.BaseColor = displayOptions.TextColor;
		}
		GameObject[] array = this.decorators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(displayOptions.ShowDecorators);
		}
		lines = DialogueBox.ParseTextForDialogueLines(text);
		if (lines.Count == 0)
		{
			lines.Add(new DialogueBox.DialogueLine
			{
				IsPlayer = false,
				Text = "ERROR: Empty dialogue line!"
			});
		}
		DialogueBox.RunCancelledCallback();
		base.StopAllCoroutines();
		this.conversationEnded = false;
	}

	// Token: 0x0600385D RID: 14429 RVA: 0x000F8DCA File Offset: 0x000F6FCA
	private static void RunCancelledCallback()
	{
		if (DialogueBox._onCancelledCallback == null)
		{
			return;
		}
		Action onCancelledCallback = DialogueBox._onCancelledCallback;
		DialogueBox._onCancelledCallback = null;
		onCancelledCallback();
	}

	// Token: 0x0600385E RID: 14430 RVA: 0x000F8DE4 File Offset: 0x000F6FE4
	private void Update()
	{
		if (!this.isDialogueRunning)
		{
			return;
		}
		if (!this.isPrintingText && !this.waitingToAdvance)
		{
			return;
		}
		if (ManagerSingleton<InputHandler>.Instance.WasSkipButtonPressed)
		{
			this.AdvanceConversation();
		}
	}

	// Token: 0x0600385F RID: 14431 RVA: 0x000F8E12 File Offset: 0x000F7012
	private void AdvanceConversation()
	{
		this.currentRevealSpeed = (this.waitingToAdvance ? this.regularRevealSpeed : this.fastRevealSpeed);
		this.waitingToAdvance = false;
	}

	// Token: 0x06003860 RID: 14432 RVA: 0x000F8E38 File Offset: 0x000F7038
	public static void EndConversation(bool returnHud = true, Action onBoxHidden = null)
	{
		if (!DialogueBox._instance)
		{
			return;
		}
		if (!DialogueBox._instance.isDialogueRunning)
		{
			if (returnHud)
			{
				DialogueBox._instance.hudFSM.SendEventSafe("IN");
			}
			if (onBoxHidden != null)
			{
				onBoxHidden();
			}
			return;
		}
		DialogueBox._instance.conversationEnded = true;
		DialogueBox._instance.StartCoroutine(DialogueBox._instance.CloseAndEnd(returnHud, onBoxHidden));
	}

	// Token: 0x06003861 RID: 14433 RVA: 0x000F8EA1 File Offset: 0x000F70A1
	private IEnumerator CloseAndEnd(bool returnHud, Action onBoxHidden)
	{
		yield return base.StartCoroutine(this.Close());
		NPCControlBase npccontrolBase = this.instigator;
		this.instigator = null;
		this.isDialogueRunning = false;
		if (returnHud)
		{
			this.hudFSM.SendEventSafe("IN");
		}
		if (npccontrolBase)
		{
			npccontrolBase.EndDialogue();
		}
		if (onBoxHidden != null)
		{
			onBoxHidden();
		}
		yield break;
	}

	// Token: 0x06003862 RID: 14434 RVA: 0x000F8EBE File Offset: 0x000F70BE
	public static bool IsSpeakerDifferent(DialogueBox.DialogueLine currentLine, DialogueBox.DialogueLine nextLine)
	{
		return nextLine.IsPlayer != currentLine.IsPlayer || nextLine.Event != currentLine.Event;
	}

	// Token: 0x06003863 RID: 14435 RVA: 0x000F8EE1 File Offset: 0x000F70E1
	public static bool WillShowFullStop(IReadOnlyList<DialogueBox.DialogueLine> lines, int lineIndex, bool overrideContinue)
	{
		if (lineIndex >= lines.Count - 1)
		{
			return !overrideContinue;
		}
		return DialogueBox.IsSpeakerDifferent(lines[lineIndex], lines[lineIndex + 1]);
	}

	// Token: 0x06003864 RID: 14436 RVA: 0x000F8F08 File Offset: 0x000F7108
	private void UpdateAppearance(DialogueBox.DialogueLine line)
	{
		this.currentLine = line;
		if (this.defaultAppearance)
		{
			this.defaultAppearance.SetActive(!line.IsPlayer);
		}
		if (this.playerAppearance)
		{
			this.playerAppearance.SetActive(line.IsPlayer);
		}
	}

	// Token: 0x06003865 RID: 14437 RVA: 0x000F8F5B File Offset: 0x000F715B
	private IEnumerator RunDialogue(List<DialogueBox.DialogueLine> lines, bool overrideContinue, Action onEnd, int iD)
	{
		if (!this.textMesh)
		{
			yield break;
		}
		NPCControlBase dialogueInstigator = this.instigator;
		if (!dialogueInstigator)
		{
			yield break;
		}
		EventRegister.SendEvent(EventRegisterEvents.DialogueBoxAppearing, null);
		if (dialogueInstigator)
		{
			dialogueInstigator.OnOpeningDialogueBox(lines[0]);
		}
		if (!this.isBoxOpen)
		{
			this.UpdateAppearance(lines[0]);
			if (this.stopAnimator)
			{
				this.stopAnimator.gameObject.SetActive(false);
			}
			this.textMesh.text = string.Empty;
			this.hudFSM.SendEventSafe("OUT");
			yield return new WaitForSeconds(this.firstOpenDelay);
			if (!this.conversationEnded)
			{
				this.animator.Play("Open");
				this.isBoxOpen = true;
			}
		}
		int lineIndex = 0;
		while (lineIndex < lines.Count && !this.conversationEnded)
		{
			DialogueBox.DialogueLine line = lines[lineIndex];
			this.textMesh.text = string.Empty;
			if (DialogueBox.IsSpeakerDifferent(this.currentLine, line))
			{
				yield return base.StartCoroutine(this.Close());
				if (this.conversationEnded)
				{
					break;
				}
				this.UpdateAppearance(line);
				this.animator.Play("Open");
				this.isBoxOpen = true;
			}
			if (dialogueInstigator)
			{
				dialogueInstigator.NewLineStarted(line);
			}
			this.textMesh.text = line.Text;
			TMP_TextInfo textInfo = this.textMesh.GetTextInfo(line.Text);
			int pageCount = textInfo.pageCount;
			int num;
			for (int pageIndex = 0; pageIndex < pageCount; pageIndex = num + 1)
			{
				this.textMesh.pageToDisplay = pageIndex + 1;
				yield return base.StartCoroutine(this.PrintText(textInfo, pageIndex));
				yield return new WaitForSeconds(this.lineEndPause);
				if (pageIndex < pageCount - 1)
				{
					yield return this.LineEndedWait(false);
				}
				num = pageIndex;
			}
			if (dialogueInstigator)
			{
				dialogueInstigator.NewLineEnded(line);
			}
			yield return base.StartCoroutine(this.LineEndedWait(DialogueBox.WillShowFullStop(lines, lineIndex, overrideContinue)));
			line = default(DialogueBox.DialogueLine);
			textInfo = null;
			num = lineIndex;
			lineIndex = num + 1;
		}
		if (dialogueInstigator)
		{
			dialogueInstigator.OnDialogueBoxEnded();
		}
		if (onEnd != null)
		{
			onEnd();
		}
		if (DialogueBox._conversationID == iD)
		{
			DialogueBox._onCancelledCallback = null;
		}
		if (dialogueInstigator && dialogueInstigator.AutoEnd)
		{
			DialogueBox.EndConversation(true, null);
		}
		yield break;
	}

	// Token: 0x06003866 RID: 14438 RVA: 0x000F8F87 File Offset: 0x000F7187
	private IEnumerator LineEndedWait(bool isFullStop)
	{
		string animPrefix = isFullStop ? "Stop" : "Arrow";
		if (this.stopAnimator)
		{
			this.stopAnimator.gameObject.SetActive(true);
			this.stopAnimator.Play(animPrefix + " " + (this.currentLine.IsPlayer ? "Hornet" : "NPC") + " Up");
		}
		this.waitingToAdvance = true;
		while (this.waitingToAdvance && !this.conversationEnded)
		{
			yield return null;
		}
		Audio.StopConfirmSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		if (this.stopAnimator)
		{
			this.stopAnimator.Play(animPrefix + " " + (this.currentLine.IsPlayer ? "Hornet" : "NPC") + " Down");
		}
		yield break;
	}

	// Token: 0x06003867 RID: 14439 RVA: 0x000F8F9D File Offset: 0x000F719D
	private IEnumerator Close()
	{
		this.isBoxOpen = false;
		this.animator.Play("Close");
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		yield break;
	}

	// Token: 0x06003868 RID: 14440 RVA: 0x000F8FAC File Offset: 0x000F71AC
	private IEnumerator PrintText(TMP_TextInfo textInfo, int pageIndex)
	{
		TMP_PageInfo pageInfo = textInfo.pageInfo[pageIndex];
		this.isPrintingText = true;
		this.textMesh.maxVisibleCharacters = pageInfo.firstCharacterIndex;
		if (!CheatManager.IsTextPrintSkipEnabled)
		{
			float visibleCharacters = (float)pageInfo.firstCharacterIndex;
			while (this.textMesh.maxVisibleCharacters < pageInfo.lastCharacterIndex)
			{
				yield return null;
				visibleCharacters += this.currentRevealSpeed * Time.deltaTime;
				this.textMesh.maxVisibleCharacters = Mathf.RoundToInt(visibleCharacters);
			}
		}
		this.textMesh.maxVisibleCharacters = int.MaxValue;
		this.isPrintingText = false;
		yield break;
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x000F8FCC File Offset: 0x000F71CC
	public static List<DialogueBox.DialogueLine> ParseTextForDialogueLines(string text)
	{
		DialogueBox.<>c__DisplayClass50_0 CS$<>8__locals1;
		CS$<>8__locals1.lines = new List<DialogueBox.DialogueLine>();
		if (string.IsNullOrEmpty(text))
		{
			CS$<>8__locals1.lines.Add(new DialogueBox.DialogueLine
			{
				IsPlayer = false,
				Text = string.Empty
			});
			return CS$<>8__locals1.lines;
		}
		CS$<>8__locals1.builder = Helper.GetTempStringBuilder();
		CS$<>8__locals1.wasPlayer = false;
		CS$<>8__locals1.eventName = null;
		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == '<')
			{
				int num = i + 1;
				int num2 = -1;
				for (;;)
				{
					char c = text[i];
					if (c == '>')
					{
						break;
					}
					if (c == '=')
					{
						num2 = i;
					}
					i++;
					if (i >= text.Length)
					{
						goto Block_5;
					}
				}
				string a;
				string newEventName;
				if (num2 < 0)
				{
					a = text.Substring(num, i - num);
					newEventName = null;
				}
				else
				{
					a = text.Substring(num, num2 - num);
					newEventName = text.Substring(num2 + 1, i - num2 - 1);
				}
				if (a == "page")
				{
					DialogueBox.<ParseTextForDialogueLines>g__BeginNewLine|50_1(false, newEventName, ref CS$<>8__locals1);
					goto IL_140;
				}
				if (a == "hpage")
				{
					DialogueBox.<ParseTextForDialogueLines>g__BeginNewLine|50_1(true, newEventName, ref CS$<>8__locals1);
					goto IL_140;
				}
				goto IL_140;
				Block_5:
				Debug.LogError(string.Format("DialogueText tag opened with '{0}' but no closing '{1}' was found!", '<', '>'));
				return CS$<>8__locals1.lines;
			}
			CS$<>8__locals1.builder.Append(text[i]);
			IL_140:;
		}
		DialogueBox.<ParseTextForDialogueLines>g__EndCurrentLine|50_0(ref CS$<>8__locals1);
		return CS$<>8__locals1.lines;
	}

	// Token: 0x0600386B RID: 14443 RVA: 0x000F916C File Offset: 0x000F736C
	[CompilerGenerated]
	internal static void <ParseTextForDialogueLines>g__EndCurrentLine|50_0(ref DialogueBox.<>c__DisplayClass50_0 A_0)
	{
		string text = A_0.builder.ToString().Trim();
		if (!string.IsNullOrEmpty(text))
		{
			A_0.lines.Add(new DialogueBox.DialogueLine
			{
				IsPlayer = A_0.wasPlayer,
				Text = text,
				Event = A_0.eventName
			});
		}
	}

	// Token: 0x0600386C RID: 14444 RVA: 0x000F91C8 File Offset: 0x000F73C8
	[CompilerGenerated]
	internal static void <ParseTextForDialogueLines>g__BeginNewLine|50_1(bool isPlayer, string newEventName, ref DialogueBox.<>c__DisplayClass50_0 A_2)
	{
		DialogueBox.<ParseTextForDialogueLines>g__EndCurrentLine|50_0(ref A_2);
		A_2.builder.Clear();
		A_2.wasPlayer = isPlayer;
		A_2.eventName = newEventName;
	}

	// Token: 0x04003B42 RID: 15170
	private static DialogueBox _instance;

	// Token: 0x04003B43 RID: 15171
	private static Action _onCancelledCallback;

	// Token: 0x04003B44 RID: 15172
	private static int _conversationID;

	// Token: 0x04003B45 RID: 15173
	[SerializeField]
	private TextMeshPro textMesh;

	// Token: 0x04003B46 RID: 15174
	[SerializeField]
	private float regularRevealSpeed = 65f;

	// Token: 0x04003B47 RID: 15175
	[SerializeField]
	private float fastRevealSpeed = 200f;

	// Token: 0x04003B48 RID: 15176
	private float currentRevealSpeed;

	// Token: 0x04003B49 RID: 15177
	[SerializeField]
	private GameObject defaultAppearance;

	// Token: 0x04003B4A RID: 15178
	[SerializeField]
	private GameObject playerAppearance;

	// Token: 0x04003B4B RID: 15179
	[SerializeField]
	private Animator stopAnimator;

	// Token: 0x04003B4C RID: 15180
	[SerializeField]
	private NestedFadeGroupSpriteRenderer stopSprite;

	// Token: 0x04003B4D RID: 15181
	[SerializeField]
	private float firstOpenDelay = 0.1f;

	// Token: 0x04003B4E RID: 15182
	[SerializeField]
	private float lineEndPause = 0.1f;

	// Token: 0x04003B4F RID: 15183
	[SerializeField]
	private NestedFadeGroupBase group;

	// Token: 0x04003B50 RID: 15184
	[SerializeField]
	private Animator animator;

	// Token: 0x04003B51 RID: 15185
	[SerializeField]
	private PlayMakerFSM hudFSM;

	// Token: 0x04003B52 RID: 15186
	[SerializeField]
	private GameObject[] decorators;

	// Token: 0x04003B53 RID: 15187
	private bool isDialogueRunning;

	// Token: 0x04003B54 RID: 15188
	private bool waitingToAdvance;

	// Token: 0x04003B55 RID: 15189
	private bool isPrintingText;

	// Token: 0x04003B56 RID: 15190
	private bool isBoxOpen;

	// Token: 0x04003B57 RID: 15191
	private DialogueBox.DialogueLine currentLine;

	// Token: 0x04003B58 RID: 15192
	private Vector3 initialPos;

	// Token: 0x04003B59 RID: 15193
	private Vector3 initialStopPos;

	// Token: 0x04003B5A RID: 15194
	private NPCControlBase instigator;

	// Token: 0x04003B5B RID: 15195
	private bool conversationEnded;

	// Token: 0x0200193E RID: 6462
	public struct DialogueLine
	{
		// Token: 0x060093A3 RID: 37795 RVA: 0x0029EA60 File Offset: 0x0029CC60
		public bool IsNpcEvent(string eventName)
		{
			return !this.IsPlayer && (string.IsNullOrEmpty(eventName) || eventName == this.Event);
		}

		// Token: 0x040094EF RID: 38127
		public bool IsPlayer;

		// Token: 0x040094F0 RID: 38128
		public string Text;

		// Token: 0x040094F1 RID: 38129
		public string Event;
	}

	// Token: 0x0200193F RID: 6463
	[Serializable]
	public struct DisplayOptions
	{
		// Token: 0x1700107A RID: 4218
		// (get) Token: 0x060093A4 RID: 37796 RVA: 0x0029EA84 File Offset: 0x0029CC84
		public static DialogueBox.DisplayOptions Default
		{
			get
			{
				return new DialogueBox.DisplayOptions
				{
					ShowDecorators = true,
					Alignment = TextAlignmentOptions.TopLeft,
					OffsetY = 0f,
					StopOffsetY = 0f,
					TextColor = Color.white
				};
			}
		}

		// Token: 0x040094F2 RID: 38130
		public bool ShowDecorators;

		// Token: 0x040094F3 RID: 38131
		public TextAlignmentOptions Alignment;

		// Token: 0x040094F4 RID: 38132
		public float OffsetY;

		// Token: 0x040094F5 RID: 38133
		public float StopOffsetY;

		// Token: 0x040094F6 RID: 38134
		public Color TextColor;
	}
}
