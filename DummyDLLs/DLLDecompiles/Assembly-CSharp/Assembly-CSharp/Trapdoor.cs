using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200057E RID: 1406
public class Trapdoor : MonoBehaviour
{
	// Token: 0x17000565 RID: 1381
	// (get) Token: 0x06003255 RID: 12885 RVA: 0x000E0404 File Offset: 0x000DE604
	public bool IsOpen
	{
		get
		{
			return this.isOpen;
		}
	}

	// Token: 0x06003256 RID: 12886 RVA: 0x000E040C File Offset: 0x000DE60C
	private void Awake()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isOpen;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isOpen = value;
				if (value)
				{
					this.SetInitialStateOpen();
				}
			};
		}
		else if (this.enterSceneTrigger)
		{
			HeroController hc = HeroController.instance;
			HeroController.HeroInPosition enterSceneSetState = null;
			enterSceneSetState = delegate(bool forceDirect)
			{
				if (this.enterSceneTrigger.IsInside)
				{
					this.SetInitialStateOpen();
				}
				hc.heroInPositionDelayed -= enterSceneSetState;
			};
			if (hc.isHeroInPosition)
			{
				enterSceneSetState(false);
			}
			else
			{
				hc.heroInPositionDelayed += enterSceneSetState;
			}
		}
		Trapdoor.DirectionalLeverParts[] array = this.retractLevers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LeverControl.OnHit.AddListener(new UnityAction(this.ResetLeverState));
		}
		Action value2 = delegate()
		{
			Trapdoor.DirectionalLeverParts[] array2 = this.retractLevers;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].LeverControl.HitBlocked = false;
			}
		};
		foreach (Trapdoor.DirectionalLeverParts directionalLeverParts in this.retractLevers)
		{
			if (directionalLeverParts.LeverRetract)
			{
				CaptureAnimationEvent component = directionalLeverParts.LeverRetract.GetComponent<CaptureAnimationEvent>();
				if (component)
				{
					component.EventFired += value2;
				}
			}
		}
		this.positiveAnims.UpdateAnimHashes();
		this.negativeAnims.UpdateAnimHashes();
	}

	// Token: 0x06003257 RID: 12887 RVA: 0x000E055D File Offset: 0x000DE75D
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.openDoorRoutine = null;
	}

	// Token: 0x06003258 RID: 12888 RVA: 0x000E056C File Offset: 0x000DE76C
	private void OnEnable()
	{
		this.ResetLeverState();
	}

	// Token: 0x06003259 RID: 12889 RVA: 0x000E0574 File Offset: 0x000DE774
	private void SetInitialStateOpen()
	{
		if (Math.Abs(this.startOpenSign) <= Mathf.Epsilon)
		{
			return;
		}
		if (this.openDoorRoutine != null)
		{
			base.StopCoroutine(this.openDoorRoutine);
		}
		if (!this.doorAnimator)
		{
			return;
		}
		this.openDoorRoutine = base.StartCoroutine(this.DoOpenDoor(this.startOpenSign, true));
	}

	// Token: 0x0600325A RID: 12890 RVA: 0x000E05CF File Offset: 0x000DE7CF
	public void SetOpened()
	{
		if (this.openDoorRoutine != null)
		{
			base.StopCoroutine(this.openDoorRoutine);
		}
		if (!this.doorAnimator)
		{
			return;
		}
		this.openDoorRoutine = base.StartCoroutine(this.DoOpenDoor(this.startOpenSign, true));
	}

	// Token: 0x0600325B RID: 12891 RVA: 0x000E060C File Offset: 0x000DE80C
	private void AttemptResetDoor()
	{
		if (!this.isOpen && this.openDoorRoutine != null)
		{
			base.StopCoroutine(this.openDoorRoutine);
			this.openDoorRoutine = null;
		}
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x000E0631 File Offset: 0x000DE831
	[ContextMenu("Open Positive", true)]
	[ContextMenu("Open Negative", true)]
	private bool CanTestOpen()
	{
		return Application.isPlaying;
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x000E0638 File Offset: 0x000DE838
	[ContextMenu("Open Positive")]
	public void OpenDoorPositive()
	{
		this.OpenDoor(1);
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x000E0641 File Offset: 0x000DE841
	[ContextMenu("Open Negative")]
	public void OpenDoorNegative()
	{
		this.OpenDoor(-1);
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x000E064A File Offset: 0x000DE84A
	public void OpenDoor(int direction)
	{
		this.InternalOpenDoor(direction, false);
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x000E0654 File Offset: 0x000DE854
	private void InternalOpenDoor(int direction, bool skip)
	{
		this.AttemptResetDoor();
		if (this.openDoorRoutine != null || !this.doorAnimator)
		{
			this.resetCloseCounter = true;
			return;
		}
		float doorSign;
		if (Math.Abs(this.openForceDirection) > Mathf.Epsilon)
		{
			doorSign = this.openForceDirection;
		}
		else
		{
			doorSign = (float)direction;
		}
		this.openDoorRoutine = base.StartCoroutine(this.DoOpenDoor(doorSign, skip));
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x000E06B6 File Offset: 0x000DE8B6
	public void OpenDoorCustom(int direction)
	{
		this.isCustomOpened = true;
		this.OpenDoor(direction);
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x000E06C6 File Offset: 0x000DE8C6
	public void OpenDoorCustomSilent(int direction)
	{
		this.isCustomOpened = true;
		this.InternalOpenDoor(direction, true);
	}

	// Token: 0x06003263 RID: 12899 RVA: 0x000E06D7 File Offset: 0x000DE8D7
	public void CloseDoorCustom()
	{
		this.isCustomOpened = false;
	}

	// Token: 0x06003264 RID: 12900 RVA: 0x000E06E0 File Offset: 0x000DE8E0
	private IEnumerator DoOpenDoor(float doorSign, bool skipOpen = false)
	{
		this.isOpen = true;
		bool doAutoClose = !this.isCustomOpened;
		foreach (Trapdoor.DirectionalLeverParts directionalLeverParts in this.retractLevers)
		{
			directionalLeverParts.LeverControl.HitBlocked = true;
			if (!directionalLeverParts.LeverControl || directionalLeverParts.LeverControl.gameObject != directionalLeverParts.Lever)
			{
				directionalLeverParts.Lever.SetActive(false);
			}
		}
		if (!skipOpen && this.openWaitTime > 0f)
		{
			yield return new WaitForSeconds(this.openWaitTime);
		}
		if (this.mechanismAnimator)
		{
			this.mechanismAnimator.Play("Open");
		}
		if (!skipOpen)
		{
			if (this.retractStartDelay > 0f)
			{
				yield return new WaitForSeconds(this.retractStartDelay);
			}
			foreach (Trapdoor.DirectionalLeverParts directionalLeverParts2 in this.retractLevers)
			{
				if (directionalLeverParts2.LeverRetract)
				{
					if (directionalLeverParts2.RetractPreventStates != null)
					{
						int currentState = directionalLeverParts2.LeverRetract.GetCurrentAnimatorStateInfo(0).shortNameHash;
						if (directionalLeverParts2.RetractPreventStates.Any((string stateName) => currentState == Animator.StringToHash(stateName)))
						{
							goto IL_1D5;
						}
					}
					if (directionalLeverParts2.LeverRetract.HasState(0, Trapdoor._retractAnim))
					{
						directionalLeverParts2.LeverRetract.Play(Trapdoor._retractAnim);
					}
				}
				IL_1D5:;
			}
			if (this.retractEndDelay > 0f)
			{
				yield return new WaitForSeconds(this.retractEndDelay);
			}
		}
		else
		{
			foreach (Trapdoor.DirectionalLeverParts directionalLeverParts3 in this.retractLevers)
			{
				if (directionalLeverParts3.LeverRetract && directionalLeverParts3.LeverRetract.HasState(0, Trapdoor._retractAnim))
				{
					directionalLeverParts3.LeverRetract.Play(Trapdoor._retractAnim, 0, 1f);
				}
			}
			this.OnStartOpen.Invoke();
		}
		Trapdoor.DirectionalAnims anims;
		if (doorSign < 0f)
		{
			doorSign = -1f;
			anims = this.negativeAnims;
		}
		else
		{
			doorSign = 1f;
			anims = this.positiveAnims;
		}
		if (this.isDirectional)
		{
			Vector3 localScale = this.doorRoot.localScale;
			localScale.x = Mathf.Abs(localScale.x) * doorSign * (float)(this.defaultDoorScalePositive ? 1 : -1);
			this.doorRoot.localScale = localScale;
		}
		if (this.OnOpen != null)
		{
			this.OnOpen.Invoke();
		}
		if (!skipOpen)
		{
			this.openSound.SpawnAndPlayOneShot(base.transform.position, null);
			if (anims.OpeningAnimHash != null)
			{
				this.doorAnimator.Play(anims.OpeningAnimHash.Value, 0, 0f);
				yield return null;
				yield return new WaitForSeconds(this.doorAnimator.GetCurrentAnimatorStateInfo(0).length);
			}
		}
		if (anims.OpenedAnimHash != null)
		{
			this.doorAnimator.Play(anims.OpenedAnimHash.Value);
		}
		if (this.stayOpen || this.persistent)
		{
			yield break;
		}
		if (doAutoClose)
		{
			float openTimeLeft = this.closeWaitTime;
			while (openTimeLeft > 0f)
			{
				yield return null;
				if (this.resetCloseCounter || (this.cannotCloseTrigger && this.cannotCloseTrigger.IsInside))
				{
					openTimeLeft = this.closeWaitTime;
					this.resetCloseCounter = false;
				}
				else
				{
					openTimeLeft -= Time.deltaTime;
				}
			}
		}
		else
		{
			while (this.isCustomOpened)
			{
				yield return null;
			}
		}
		if (this.OnClose != null)
		{
			this.OnClose.Invoke();
		}
		this.closeSound.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.mechanismAnimator)
		{
			this.mechanismAnimator.Play("Close");
		}
		if (anims.ClosingAnimHash != null)
		{
			this.doorAnimator.Play(anims.ClosingAnimHash.Value, 0, 0f);
			yield return null;
			yield return new WaitForSeconds(this.doorAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		if (anims.ClosedAnimHash != null)
		{
			this.doorAnimator.Play(anims.ClosedAnimHash.Value);
		}
		if (this.OnClosed != null)
		{
			this.OnClosed.Invoke();
		}
		this.isOpen = false;
		yield return base.StartCoroutine(this.ReturnRetractedLevers());
		this.openDoorRoutine = null;
		yield break;
	}

	// Token: 0x06003265 RID: 12901 RVA: 0x000E06FD File Offset: 0x000DE8FD
	private IEnumerator ReturnRetractedLevers()
	{
		float returnTime = 0f;
		foreach (Trapdoor.DirectionalLeverParts directionalLeverParts in this.retractLevers)
		{
			if (directionalLeverParts.LeverRetract && directionalLeverParts.LeverRetract.HasState(0, Trapdoor._returnAnim))
			{
				directionalLeverParts.LeverRetract.Play(Trapdoor._returnAnim);
			}
		}
		yield return null;
		foreach (Trapdoor.DirectionalLeverParts directionalLeverParts2 in this.retractLevers)
		{
			if (directionalLeverParts2.LeverRetract && directionalLeverParts2.LeverRetract.isActiveAndEnabled)
			{
				AnimatorStateInfo currentAnimatorStateInfo = directionalLeverParts2.LeverRetract.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.shortNameHash == Trapdoor._returnAnim)
				{
					float length = currentAnimatorStateInfo.length;
					returnTime = Mathf.Max(returnTime, length);
				}
			}
		}
		if (returnTime > 0f)
		{
			yield return new WaitForSeconds(returnTime);
		}
		this.ResetLeverState();
		yield break;
	}

	// Token: 0x06003266 RID: 12902 RVA: 0x000E070C File Offset: 0x000DE90C
	private void ResetLeverState()
	{
		this.SetLeversRetracted(false, true);
	}

	// Token: 0x06003267 RID: 12903 RVA: 0x000E0718 File Offset: 0x000DE918
	public void SetLeversRetracted(bool isRetracted, bool isInstant)
	{
		if (!isRetracted && !isInstant)
		{
			base.StartCoroutine(this.ReturnRetractedLevers());
			return;
		}
		foreach (Trapdoor.DirectionalLeverParts directionalLeverParts in this.retractLevers)
		{
			directionalLeverParts.LeverControl.HitBlocked = isRetracted;
			if (!directionalLeverParts.LeverControl || directionalLeverParts.LeverControl.gameObject != directionalLeverParts.Lever)
			{
				directionalLeverParts.Lever.gameObject.SetActive(!isRetracted);
			}
			int num = isRetracted ? Trapdoor._retractAnim : Trapdoor._hiddenAnim;
			if (directionalLeverParts.LeverRetract.HasState(0, num))
			{
				directionalLeverParts.LeverRetract.Play(num, 0, isInstant ? 1f : 0f);
			}
		}
	}

	// Token: 0x04003604 RID: 13828
	private static readonly int _retractAnim = Animator.StringToHash("Retract");

	// Token: 0x04003605 RID: 13829
	private static readonly int _returnAnim = Animator.StringToHash("Return");

	// Token: 0x04003606 RID: 13830
	private static readonly int _hiddenAnim = Animator.StringToHash("Hidden");

	// Token: 0x04003607 RID: 13831
	[SerializeField]
	private Animator doorAnimator;

	// Token: 0x04003608 RID: 13832
	[SerializeField]
	private Animator mechanismAnimator;

	// Token: 0x04003609 RID: 13833
	[SerializeField]
	private bool isDirectional;

	// Token: 0x0400360A RID: 13834
	[SerializeField]
	[ModifiableProperty]
	[Conditional("isDirectional", true, false, false)]
	private Transform doorRoot;

	// Token: 0x0400360B RID: 13835
	[SerializeField]
	[ModifiableProperty]
	[Conditional("isDirectional", true, false, false)]
	private bool defaultDoorScalePositive;

	// Token: 0x0400360C RID: 13836
	[SerializeField]
	private TrackTriggerObjects cannotCloseTrigger;

	// Token: 0x0400360D RID: 13837
	[SerializeField]
	private float openWaitTime;

	// Token: 0x0400360E RID: 13838
	[SerializeField]
	private float closeWaitTime;

	// Token: 0x0400360F RID: 13839
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003610 RID: 13840
	[SerializeField]
	[ModifiableProperty]
	[Conditional("persistent", false, false, false)]
	private bool stayOpen;

	// Token: 0x04003611 RID: 13841
	[SerializeField]
	[ModifiableProperty]
	[Conditional("persistent", false, false, false)]
	private TrackTriggerObjects enterSceneTrigger;

	// Token: 0x04003612 RID: 13842
	[SerializeField]
	private float startOpenSign;

	// Token: 0x04003613 RID: 13843
	[SerializeField]
	private float openForceDirection;

	// Token: 0x04003614 RID: 13844
	[SerializeField]
	private AudioEvent openSound;

	// Token: 0x04003615 RID: 13845
	[SerializeField]
	private AudioEvent closeSound;

	// Token: 0x04003616 RID: 13846
	[Space]
	[SerializeField]
	private Trapdoor.DirectionalAnims positiveAnims;

	// Token: 0x04003617 RID: 13847
	[SerializeField]
	private Trapdoor.DirectionalAnims negativeAnims;

	// Token: 0x04003618 RID: 13848
	[Space]
	[SerializeField]
	private Trapdoor.DirectionalLeverParts[] retractLevers;

	// Token: 0x04003619 RID: 13849
	[SerializeField]
	private float retractStartDelay;

	// Token: 0x0400361A RID: 13850
	[SerializeField]
	private float retractEndDelay;

	// Token: 0x0400361B RID: 13851
	[Space]
	public UnityEvent OnOpen;

	// Token: 0x0400361C RID: 13852
	public UnityEvent OnClose;

	// Token: 0x0400361D RID: 13853
	public UnityEvent OnClosed;

	// Token: 0x0400361E RID: 13854
	public UnityEvent OnStartOpen;

	// Token: 0x0400361F RID: 13855
	private bool isOpen;

	// Token: 0x04003620 RID: 13856
	private Coroutine openDoorRoutine;

	// Token: 0x04003621 RID: 13857
	private bool resetCloseCounter;

	// Token: 0x04003622 RID: 13858
	private bool isCustomOpened;

	// Token: 0x0200188F RID: 6287
	[Serializable]
	private class DirectionalAnims
	{
		// Token: 0x17001017 RID: 4119
		// (get) Token: 0x06009199 RID: 37273 RVA: 0x0029945E File Offset: 0x0029765E
		// (set) Token: 0x0600919A RID: 37274 RVA: 0x00299466 File Offset: 0x00297666
		public int? OpeningAnimHash { get; private set; }

		// Token: 0x17001018 RID: 4120
		// (get) Token: 0x0600919B RID: 37275 RVA: 0x0029946F File Offset: 0x0029766F
		// (set) Token: 0x0600919C RID: 37276 RVA: 0x00299477 File Offset: 0x00297677
		public int? OpenedAnimHash { get; private set; }

		// Token: 0x17001019 RID: 4121
		// (get) Token: 0x0600919D RID: 37277 RVA: 0x00299480 File Offset: 0x00297680
		// (set) Token: 0x0600919E RID: 37278 RVA: 0x00299488 File Offset: 0x00297688
		public int? ClosingAnimHash { get; private set; }

		// Token: 0x1700101A RID: 4122
		// (get) Token: 0x0600919F RID: 37279 RVA: 0x00299491 File Offset: 0x00297691
		// (set) Token: 0x060091A0 RID: 37280 RVA: 0x00299499 File Offset: 0x00297699
		public int? ClosedAnimHash { get; private set; }

		// Token: 0x060091A1 RID: 37281 RVA: 0x002994A4 File Offset: 0x002976A4
		public void UpdateAnimHashes()
		{
			this.OpeningAnimHash = (string.IsNullOrEmpty(this.OpeningAnim) ? null : new int?(Animator.StringToHash(this.OpeningAnim)));
			this.OpenedAnimHash = (string.IsNullOrEmpty(this.OpenedAnim) ? null : new int?(Animator.StringToHash(this.OpenedAnim)));
			this.ClosingAnimHash = (string.IsNullOrEmpty(this.ClosingAnim) ? null : new int?(Animator.StringToHash(this.ClosingAnim)));
			this.ClosedAnimHash = (string.IsNullOrEmpty(this.ClosedAnim) ? null : new int?(Animator.StringToHash(this.ClosedAnim)));
		}

		// Token: 0x0400927B RID: 37499
		public string OpeningAnim;

		// Token: 0x0400927C RID: 37500
		public string OpenedAnim;

		// Token: 0x0400927D RID: 37501
		public string ClosingAnim;

		// Token: 0x0400927E RID: 37502
		public string ClosedAnim;
	}

	// Token: 0x02001890 RID: 6288
	[Serializable]
	private class DirectionalLeverParts
	{
		// Token: 0x04009283 RID: 37507
		public Lever LeverControl;

		// Token: 0x04009284 RID: 37508
		public GameObject Lever;

		// Token: 0x04009285 RID: 37509
		public Animator LeverRetract;

		// Token: 0x04009286 RID: 37510
		public List<string> RetractPreventStates;
	}
}
