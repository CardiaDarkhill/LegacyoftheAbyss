using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020007B8 RID: 1976
public class AdditiveLoreSceneController : MonoBehaviour
{
	// Token: 0x060045B6 RID: 17846 RVA: 0x0012F671 File Offset: 0x0012D871
	private void Awake()
	{
		this.screenFader.AlphaSelf = 0f;
		if (this.loreSceneOnly)
		{
			this.loreSceneOnly.SetActive(false);
		}
	}

	// Token: 0x060045B7 RID: 17847 RVA: 0x0012F69C File Offset: 0x0012D89C
	private void OnDestroy()
	{
		if (this.isOtherSceneLoaded)
		{
			SceneManager.UnloadSceneAsync(this.sceneName);
		}
	}

	// Token: 0x060045B8 RID: 17848 RVA: 0x0012F6B4 File Offset: 0x0012D8B4
	private void Update()
	{
		bool isPerforming = HeroPerformanceRegion.IsPerforming;
		if (this.isInOtherScene && !this.preventCancel)
		{
			if (isPerforming)
			{
				this.stopPlayGraceLeft = this.stopPlayGrace;
				if (!this.needolinGraceToggle)
				{
					this.needolinGraceToggle = true;
					if (this.transitionToMainParticles)
					{
						this.transitionToMainParticles.StopParticleSystems();
					}
				}
			}
			else
			{
				this.stopPlayGraceLeft -= Time.deltaTime;
				if (this.needolinGraceToggle)
				{
					this.needolinGraceToggle = false;
					if (this.transitionToMainParticles)
					{
						this.transitionToMainParticles.PlayParticleSystems();
					}
				}
			}
			if (this.stopPlayGraceLeft > 0f)
			{
				return;
			}
		}
		this.needolinGraceToggle = false;
		if (isPerforming == this.wasTransitioning)
		{
			return;
		}
		if (this.currentTransitionRoutine != null)
		{
			if (this.preventCancel)
			{
				return;
			}
			base.StopCoroutine(this.currentTransitionRoutine);
		}
		if (isPerforming)
		{
			if (!this.activePDTest.IsFulfilled)
			{
				return;
			}
			if (this.requireInside && !this.requireInside.IsInside)
			{
				return;
			}
			this.currentTransitionRoutine = base.StartCoroutine(this.TransitionToOtherScene());
		}
		else
		{
			this.currentTransitionRoutine = base.StartCoroutine(this.TransitionToMainScene());
		}
		this.wasTransitioning = isPerforming;
	}

	// Token: 0x060045B9 RID: 17849 RVA: 0x0012F7E0 File Offset: 0x0012D9E0
	private IEnumerator TransitionToOtherScene()
	{
		yield return new WaitForSeconds(this.startWaitTime);
		EventRegister.SendEvent(this.needolinStartedEvent, null);
		yield break;
	}

	// Token: 0x060045BA RID: 17850 RVA: 0x0012F7EF File Offset: 0x0012D9EF
	private IEnumerator TransitionToMainScene()
	{
		EventRegister.SendEvent(this.needolinEndedEvent, null);
		this.currentTransitionRoutine = null;
		yield break;
	}

	// Token: 0x060045BB RID: 17851 RVA: 0x0012F7FE File Offset: 0x0012D9FE
	private IEnumerator SetupOtherScene()
	{
		bool flag = false;
		Scene scene = default(Scene);
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (!(sceneAt.name != this.sceneName))
			{
				flag = true;
				this.isOtherSceneLoaded = true;
				scene = sceneAt;
				break;
			}
		}
		if (!flag)
		{
			yield return SceneManager.LoadSceneAsync(this.sceneName, LoadSceneMode.Additive);
			this.isOtherSceneLoaded = true;
			scene = SceneManager.GetSceneByName(this.sceneName);
		}
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		if (rootGameObjects.Length == 0)
		{
			Debug.LogError("Other scene did not have any root GameObjects", this);
			yield break;
		}
		this.otherSceneParent = rootGameObjects[0].transform;
		yield break;
	}

	// Token: 0x04004652 RID: 18002
	[SerializeField]
	private PlayerDataTest activePDTest;

	// Token: 0x04004653 RID: 18003
	[SerializeField]
	private TrackTriggerObjects requireInside;

	// Token: 0x04004654 RID: 18004
	[Space]
	[SerializeField]
	private float startWaitTime;

	// Token: 0x04004655 RID: 18005
	[SerializeField]
	private string sceneName;

	// Token: 0x04004656 RID: 18006
	[Space]
	[SerializeField]
	private Transform wholeSceneParent;

	// Token: 0x04004657 RID: 18007
	[SerializeField]
	private GameObject loreSceneOnly;

	// Token: 0x04004658 RID: 18008
	[SerializeField]
	private Vector2 scenePosOffset;

	// Token: 0x04004659 RID: 18009
	[Space]
	[SerializeField]
	private NestedFadeGroupBase screenFader;

	// Token: 0x0400465A RID: 18010
	[SerializeField]
	private AnimationCurve screenFaderToOtherCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400465B RID: 18011
	[SerializeField]
	private AnimationCurve screenFaderToMainCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400465C RID: 18012
	[SerializeField]
	private float lightFadeUpDuration;

	// Token: 0x0400465D RID: 18013
	[SerializeField]
	private PlayParticleEffects transitionToOtherParticles;

	// Token: 0x0400465E RID: 18014
	[SerializeField]
	private float blankerHoldDuration;

	// Token: 0x0400465F RID: 18015
	[SerializeField]
	private float blankerFadeDownDuration;

	// Token: 0x04004660 RID: 18016
	[SerializeField]
	private float minPlayDuration;

	// Token: 0x04004661 RID: 18017
	[SerializeField]
	private float stopPlayGrace;

	// Token: 0x04004662 RID: 18018
	[SerializeField]
	private float blankerFadeUpDuration;

	// Token: 0x04004663 RID: 18019
	[SerializeField]
	private PlayParticleEffects transitionToMainParticles;

	// Token: 0x04004664 RID: 18020
	[SerializeField]
	private float blankerFadeBackDuration;

	// Token: 0x04004665 RID: 18021
	[Space]
	[SerializeField]
	private LocalisedTextCollection needolinText;

	// Token: 0x04004666 RID: 18022
	[SerializeField]
	private string needolinStartedEvent;

	// Token: 0x04004667 RID: 18023
	[SerializeField]
	private string needolinEndedEvent;

	// Token: 0x04004668 RID: 18024
	private Transform otherSceneParent;

	// Token: 0x04004669 RID: 18025
	private bool isOtherSceneLoaded;

	// Token: 0x0400466A RID: 18026
	private Vector3 mainScenePosition;

	// Token: 0x0400466B RID: 18027
	private Vector3 altScenePosition;

	// Token: 0x0400466C RID: 18028
	private bool wasTransitioning;

	// Token: 0x0400466D RID: 18029
	private float stopPlayGraceLeft;

	// Token: 0x0400466E RID: 18030
	private bool needolinGraceToggle;

	// Token: 0x0400466F RID: 18031
	private Color? initialAmbientLightColor;

	// Token: 0x04004670 RID: 18032
	private Coroutine currentTransitionRoutine;

	// Token: 0x04004671 RID: 18033
	private bool preventCancel;

	// Token: 0x04004672 RID: 18034
	private bool isInOtherScene;
}
