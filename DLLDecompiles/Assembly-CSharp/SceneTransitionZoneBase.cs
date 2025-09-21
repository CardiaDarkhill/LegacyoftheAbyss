using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

// Token: 0x020007D1 RID: 2001
public abstract class SceneTransitionZoneBase : TriggerEnterEvent
{
	// Token: 0x170007F8 RID: 2040
	// (get) Token: 0x06004683 RID: 18051
	protected abstract string TargetScene { get; }

	// Token: 0x170007F9 RID: 2041
	// (get) Token: 0x06004684 RID: 18052
	protected abstract string TargetGate { get; }

	// Token: 0x06004685 RID: 18053 RVA: 0x00131B69 File Offset: 0x0012FD69
	protected virtual void OnPreTransition()
	{
	}

	// Token: 0x06004686 RID: 18054 RVA: 0x00131B6B File Offset: 0x0012FD6B
	protected override void Awake()
	{
		base.Awake();
		base.OnTriggerEntered += delegate(Collider2D col, GameObject _)
		{
			if (this.respawnRoutine != null)
			{
				return;
			}
			HeroController component = col.GetComponent<HeroController>();
			if (!component)
			{
				return;
			}
			HeroControllerStates cState = component.cState;
			if (cState.hazardDeath || cState.hazardRespawning)
			{
				return;
			}
			this.respawnRoutine = base.StartCoroutine(this.Respawn(component));
		};
	}

	// Token: 0x06004687 RID: 18055 RVA: 0x00131B85 File Offset: 0x0012FD85
	private IEnumerator Respawn(HeroController hc)
	{
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
		ScenePreloader.SpawnPreloader(this.TargetScene, LoadSceneMode.Additive);
		AudioSource audioSource = this.transitionSound.SpawnAndPlayOneShot(this.audioPrefab, base.transform.position, null);
		if (audioSource != null)
		{
			PlayAudioAndRecycle component = audioSource.GetComponent<PlayAudioAndRecycle>();
			if (component)
			{
				component.KeepAliveThroughNextScene = true;
			}
		}
		if (this.musicSnapshot)
		{
			this.musicSnapshot.TransitionTo(this.snapshotTransitionTime);
		}
		if (this.atmosSnapshot)
		{
			this.atmosSnapshot.TransitionTo(this.snapshotTransitionTime);
		}
		if (this.actorSnapshot)
		{
			this.actorSnapshot.TransitionTo(this.snapshotTransitionTime);
		}
		hc.RelinquishControlNotVelocity();
		hc.StopAnimationControl();
		if (hc.cState.onGround)
		{
			hc.AffectedByGravity(false);
		}
		hc.damageMode = DamageMode.NO_DAMAGE;
		if (this.freezeCamera)
		{
			GameCameras.instance.cameraController.FreezeInPlace(true);
		}
		Color original = this.fadeColor;
		float? a = new float?(0f);
		ScreenFaderUtils.Fade(original.Where(null, null, null, a), this.fadeColor, this.fadeDuration);
		float elapsedTime = 0f;
		Rigidbody2D body = hc.Body;
		float maxFallVelocity = -hc.GetMaxFallVelocity();
		while (elapsedTime <= this.fadeDuration)
		{
			Vector2 linearVelocity = body.linearVelocity;
			if (linearVelocity.y < maxFallVelocity)
			{
				body.linearVelocity = new Vector2(linearVelocity.x, maxFallVelocity);
			}
			yield return null;
			elapsedTime += Time.deltaTime;
		}
		hc.AffectedByGravity(false);
		hc.Body.linearVelocity = Vector2.zero;
		hc.StartAnimationControl();
		yield return new WaitForSeconds(this.holdDuration - this.fadeDuration);
		this.OnPreTransition();
		GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
		{
			PreventCameraFadeOut = true,
			EntryGateName = this.TargetGate,
			SceneName = this.TargetScene,
			Visualization = GameManager.SceneLoadVisualizations.Default
		});
		yield break;
	}

	// Token: 0x040046E9 RID: 18153
	[Space]
	[SerializeField]
	private Color fadeColor;

	// Token: 0x040046EA RID: 18154
	[SerializeField]
	private float fadeDuration;

	// Token: 0x040046EB RID: 18155
	[SerializeField]
	private float holdDuration;

	// Token: 0x040046EC RID: 18156
	[SerializeField]
	private bool freezeCamera = true;

	// Token: 0x040046ED RID: 18157
	[Space]
	[SerializeField]
	private AudioSource audioPrefab;

	// Token: 0x040046EE RID: 18158
	[SerializeField]
	private AudioEvent transitionSound;

	// Token: 0x040046EF RID: 18159
	[SerializeField]
	private AudioMixerSnapshot musicSnapshot;

	// Token: 0x040046F0 RID: 18160
	[SerializeField]
	private AudioMixerSnapshot atmosSnapshot;

	// Token: 0x040046F1 RID: 18161
	[SerializeField]
	private AudioMixerSnapshot actorSnapshot;

	// Token: 0x040046F2 RID: 18162
	[SerializeField]
	private float snapshotTransitionTime = 2f;

	// Token: 0x040046F3 RID: 18163
	private Coroutine respawnRoutine;
}
