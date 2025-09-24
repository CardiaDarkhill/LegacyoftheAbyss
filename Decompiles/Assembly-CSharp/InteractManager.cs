using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020003CF RID: 975
public class InteractManager : ManagerSingleton<InteractManager>
{
	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06002150 RID: 8528 RVA: 0x00099F7C File Offset: 0x0009817C
	public static bool CanInteract
	{
		get
		{
			InteractManager instance = ManagerSingleton<InteractManager>.Instance;
			if (instance && Time.timeAsDouble < instance.canInteractTime)
			{
				return false;
			}
			HeroController silentInstance = HeroController.SilentInstance;
			return silentInstance && !silentInstance.IsPaused() && !silentInstance.IsInputBlocked() && InteractManager.BlockingInteractable == null && !InteractManager.IsDisabled;
		}
	}

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x06002151 RID: 8529 RVA: 0x00099FDD File Offset: 0x000981DD
	// (set) Token: 0x06002152 RID: 8530 RVA: 0x00099FE4 File Offset: 0x000981E4
	[UsedImplicitly]
	public static bool AlwaysShowPrompts { get; set; }

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x06002153 RID: 8531 RVA: 0x00099FEC File Offset: 0x000981EC
	// (set) Token: 0x06002154 RID: 8532 RVA: 0x0009A006 File Offset: 0x00098206
	public static bool IsDisabled
	{
		get
		{
			return !ManagerSingleton<InteractManager>.Instance || ManagerSingleton<InteractManager>.Instance.isDisabled;
		}
		set
		{
			if (ManagerSingleton<InteractManager>.Instance)
			{
				ManagerSingleton<InteractManager>.Instance.isDisabled = value;
			}
		}
	}

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x06002155 RID: 8533 RVA: 0x0009A01F File Offset: 0x0009821F
	// (set) Token: 0x06002156 RID: 8534 RVA: 0x0009A039 File Offset: 0x00098239
	public static InteractableBase BlockingInteractable
	{
		get
		{
			if (!ManagerSingleton<InteractManager>.Instance)
			{
				return null;
			}
			return ManagerSingleton<InteractManager>.Instance.blockingInteractable;
		}
		set
		{
			if (ManagerSingleton<InteractManager>.Instance)
			{
				ManagerSingleton<InteractManager>.Instance.blockingInteractable = value;
			}
		}
	}

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x06002157 RID: 8535 RVA: 0x0009A054 File Offset: 0x00098254
	public static bool IsPromptVisible
	{
		get
		{
			InteractManager instance = ManagerSingleton<InteractManager>.Instance;
			return instance && instance.currentInteractable != null && instance.currentInteractable.Prompt && instance.currentInteractable.Prompt.IsVisible;
		}
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x0009A0A0 File Offset: 0x000982A0
	protected override void Awake()
	{
		base.Awake();
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO DAMAGED").ReceivedEvent += this.OnHeroInterrupted;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "FSM CANCEL").ReceivedEvent += this.OnHeroInterrupted;
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x0009A0F5 File Offset: 0x000982F5
	private void Start()
	{
		this.inputHandler = ManagerSingleton<InputHandler>.Instance;
		GameManager.instance.NextSceneWillActivate += this.ResetState;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x0009A118 File Offset: 0x00098318
	private void OnDisable()
	{
		if (GameManager.instance)
		{
			GameManager.instance.NextSceneWillActivate -= this.ResetState;
		}
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x0009A13C File Offset: 0x0009833C
	private void ResetState()
	{
		this.ClearCurrentInteractible();
		this.allInteractables.Clear();
		this.blockingInteractable = null;
		this.isDisabled = false;
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x0009A160 File Offset: 0x00098360
	private void Update()
	{
		if (!this.player && GameManager.instance && GameManager.instance.hero_ctrl)
		{
			this.player = GameManager.instance.hero_ctrl.transform;
		}
		bool flag = this.player && InteractManager.CanInteract && HeroController.instance.CanInteract();
		if (this.player)
		{
			InteractManager.InteractableReference interactableReference = null;
			float num = float.MaxValue;
			InteractManager.InteractableReference interactableReference2 = null;
			float num2 = float.MaxValue;
			InteractManager.InteractableReference interactableReference3 = null;
			float num3 = float.MaxValue;
			if (flag || InteractManager.AlwaysShowPrompts)
			{
				foreach (InteractManager.InteractableReference interactableReference4 in this.allInteractables)
				{
					if (!interactableReference4.Interactable.IsBlocked)
					{
						float num4 = Vector3.Distance(interactableReference4.Transform.position, this.player.position);
						if (num4 < num)
						{
							interactableReference = interactableReference4;
							num = num4;
						}
						switch (interactableReference4.Interactable.Priority)
						{
						case InteractPriority.Regular:
							if (num4 < num2)
							{
								interactableReference2 = interactableReference4;
								num2 = num4;
							}
							break;
						case InteractPriority.LowPriority:
							break;
						case InteractPriority.HighPriority:
							if (num4 < num3)
							{
								interactableReference3 = interactableReference4;
								num3 = num4;
							}
							break;
						default:
							Debug.LogError(string.Format("InteractPriority \"{0}\" not implemented", interactableReference4.Interactable));
							break;
						}
					}
				}
			}
			if (this.canInteractFrames > 0)
			{
				this.canInteractFrames--;
			}
			InteractManager.InteractableReference interactableReference5;
			if ((interactableReference5 = interactableReference3) == null)
			{
				interactableReference5 = (interactableReference2 ?? interactableReference);
			}
			InteractManager.InteractableReference interactableReference6 = interactableReference5;
			if (interactableReference6 != this.currentInteractable)
			{
				this.ClearCurrentInteractible();
				if (this.canInteractFrames <= 0 && InteractManager.CanInteract)
				{
					if (interactableReference6 != null)
					{
						this.currentInteractable = interactableReference6;
						this.currentInteractable.Interactable.CanInteract();
						if (this.interactPromptPrefab)
						{
							PromptMarker promptMarker = this.interactPromptPrefab.Spawn<PromptMarker>();
							promptMarker.SetLabel(this.currentInteractable.Interactable.InteractLabelDisplay);
							promptMarker.SetOwner(this.currentInteractable.Transform.gameObject);
							promptMarker.SetFollowing(this.currentInteractable.Transform, this.currentInteractable.PromptOffset);
							promptMarker.Show();
							this.currentInteractable.Prompt = promptMarker;
						}
					}
				}
				else if (!InteractManager.CanInteract)
				{
					this.canInteractFrames = 5;
				}
			}
			else if (!InteractManager.CanInteract)
			{
				this.canInteractFrames = 5;
			}
		}
		HeroActions inputActions = this.inputHandler.inputActions;
		if (flag && this.currentInteractable != null && (inputActions.Up.WasPressed || inputActions.Down.WasPressed) && !inputActions.Left.IsPressed && !inputActions.Right.IsPressed)
		{
			this.currentInteractable.Interactable.QueueInteraction();
		}
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x0009A448 File Offset: 0x00098648
	private void ClearCurrentInteractible()
	{
		if (this.currentInteractable != null)
		{
			if (this.currentInteractable.Prompt != null)
			{
				this.currentInteractable.Prompt.Hide();
				this.currentInteractable.Prompt = null;
			}
			this.currentInteractable.Interactable.CanNotInteract();
			this.currentInteractable = null;
		}
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x0009A4A4 File Offset: 0x000986A4
	public static void AddInteractible(InteractableBase interactible, Transform transform, Vector3 promptOffset)
	{
		if (ManagerSingleton<InteractManager>.Instance == null)
		{
			return;
		}
		using (List<InteractManager.InteractableReference>.Enumerator enumerator = ManagerSingleton<InteractManager>.UnsafeInstance.allInteractables.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Interactable == interactible)
				{
					return;
				}
			}
		}
		ManagerSingleton<InteractManager>.UnsafeInstance.allInteractables.Add(new InteractManager.InteractableReference(interactible, transform, promptOffset));
	}

	// Token: 0x0600215F RID: 8543 RVA: 0x0009A528 File Offset: 0x00098728
	public static bool RemoveInteractible(InteractableBase interactible)
	{
		if (ManagerSingleton<InteractManager>.UnsafeInstance == null)
		{
			return false;
		}
		InteractManager.InteractableReference interactableReference = null;
		foreach (InteractManager.InteractableReference interactableReference2 in ManagerSingleton<InteractManager>.UnsafeInstance.allInteractables)
		{
			if (interactableReference2.Interactable == interactible)
			{
				interactableReference = interactableReference2;
			}
		}
		if (interactableReference != null)
		{
			ManagerSingleton<InteractManager>.UnsafeInstance.allInteractables.Remove(interactableReference);
			if (ManagerSingleton<InteractManager>.UnsafeInstance.currentInteractable == interactableReference)
			{
				ManagerSingleton<InteractManager>.UnsafeInstance.ClearCurrentInteractible();
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002160 RID: 8544 RVA: 0x0009A5C8 File Offset: 0x000987C8
	private void OnHeroInterrupted()
	{
		if (!this.blockingInteractable)
		{
			return;
		}
		PlayMakerNPC playMakerNPC = this.blockingInteractable as PlayMakerNPC;
		if (playMakerNPC != null)
		{
			playMakerNPC.ForceEndDialogue();
			return;
		}
		DialogueBox.EndConversation(true, null);
	}

	// Token: 0x06002161 RID: 8545 RVA: 0x0009A608 File Offset: 0x00098808
	public static void SetEnabledDelay(float delay)
	{
		InteractManager instance = ManagerSingleton<InteractManager>.Instance;
		if (instance)
		{
			instance.canInteractTime = Time.timeAsDouble + (double)delay;
		}
	}

	// Token: 0x04002038 RID: 8248
	[SerializeField]
	private PromptMarker interactPromptPrefab;

	// Token: 0x04002039 RID: 8249
	private InteractableBase blockingInteractable;

	// Token: 0x0400203A RID: 8250
	private bool isDisabled;

	// Token: 0x0400203B RID: 8251
	private double canInteractTime;

	// Token: 0x0400203C RID: 8252
	private int canInteractFrames;

	// Token: 0x0400203D RID: 8253
	private const int INTERACT_COOLDOWN_FRAMES = 5;

	// Token: 0x0400203E RID: 8254
	private readonly List<InteractManager.InteractableReference> allInteractables = new List<InteractManager.InteractableReference>();

	// Token: 0x0400203F RID: 8255
	private InteractManager.InteractableReference currentInteractable;

	// Token: 0x04002040 RID: 8256
	private Transform player;

	// Token: 0x04002041 RID: 8257
	private InputHandler inputHandler;

	// Token: 0x02001685 RID: 5765
	private class InteractableReference
	{
		// Token: 0x06008A4C RID: 35404 RVA: 0x0027F789 File Offset: 0x0027D989
		public InteractableReference(InteractableBase interactable, Transform transform, Vector3 promptOffset)
		{
			this.Interactable = interactable;
			this.Transform = transform;
			this.PromptOffset = promptOffset;
		}

		// Token: 0x04008B2F RID: 35631
		public readonly InteractableBase Interactable;

		// Token: 0x04008B30 RID: 35632
		public readonly Transform Transform;

		// Token: 0x04008B31 RID: 35633
		public readonly Vector3 PromptOffset;

		// Token: 0x04008B32 RID: 35634
		public PromptMarker Prompt;
	}
}
