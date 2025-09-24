using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HutongGames.PlayMaker;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200075F RID: 1887
public static class Extensions
{
	// Token: 0x060042BD RID: 17085 RVA: 0x00126238 File Offset: 0x00124438
	public static Selectable GetFirstInteractable(this Selectable start)
	{
		if (start == null)
		{
			return null;
		}
		if (start.interactable)
		{
			return start;
		}
		return start.navigation.selectOnDown.GetFirstInteractable();
	}

	// Token: 0x060042BE RID: 17086 RVA: 0x0012626D File Offset: 0x0012446D
	public static void PlayOnSource(this AudioClip self, AudioSource source, float pitchMin = 1f, float pitchMax = 1f)
	{
		if (self && source)
		{
			source.pitch = Random.Range(pitchMin, pitchMax);
			source.PlayOneShot(self);
		}
	}

	// Token: 0x060042BF RID: 17087 RVA: 0x00126294 File Offset: 0x00124494
	public static void SetActiveChildren(this GameObject self, bool value)
	{
		int childCount = self.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			self.transform.GetChild(i).gameObject.SetActive(value);
		}
	}

	// Token: 0x060042C0 RID: 17088 RVA: 0x001262D0 File Offset: 0x001244D0
	public static void SetActiveWithChildren(this MeshRenderer self, bool value)
	{
		if (self.transform.childCount > 0)
		{
			MeshRenderer[] componentsInChildren = self.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = value;
			}
			return;
		}
		self.enabled = value;
	}

	// Token: 0x060042C1 RID: 17089 RVA: 0x00126311 File Offset: 0x00124511
	private static AnimatorControllerParameter[] GetAnimatorParameters(Animator animator)
	{
		if (!false && Application.isPlaying)
		{
			return animator.parameters;
		}
		return null;
	}

	// Token: 0x060042C2 RID: 17090 RVA: 0x00126328 File Offset: 0x00124528
	public static bool HasParameter(this Animator self, string paramName, AnimatorControllerParameterType? type = null)
	{
		AnimatorControllerParameter[] animatorParameters = Extensions.GetAnimatorParameters(self);
		if (animatorParameters == null)
		{
			return false;
		}
		foreach (AnimatorControllerParameter animatorControllerParameter in animatorParameters)
		{
			if (animatorControllerParameter.name == paramName)
			{
				if (type != null)
				{
					AnimatorControllerParameterType type2 = animatorControllerParameter.type;
					AnimatorControllerParameterType? animatorControllerParameterType = type;
					if (!(type2 == animatorControllerParameterType.GetValueOrDefault() & animatorControllerParameterType != null))
					{
						goto IL_4B;
					}
				}
				return true;
			}
			IL_4B:;
		}
		return false;
	}

	// Token: 0x060042C3 RID: 17091 RVA: 0x0012638C File Offset: 0x0012458C
	public static bool HasParameter(this Animator self, int paramID, AnimatorControllerParameterType? type = null)
	{
		AnimatorControllerParameter[] animatorParameters = Extensions.GetAnimatorParameters(self);
		if (animatorParameters == null)
		{
			return false;
		}
		foreach (AnimatorControllerParameter animatorControllerParameter in animatorParameters)
		{
			if (animatorControllerParameter.nameHash == paramID)
			{
				if (type != null)
				{
					AnimatorControllerParameterType type2 = animatorControllerParameter.type;
					AnimatorControllerParameterType? animatorControllerParameterType = type;
					if (!(type2 == animatorControllerParameterType.GetValueOrDefault() & animatorControllerParameterType != null))
					{
						goto IL_46;
					}
				}
				return true;
			}
			IL_46:;
		}
		return false;
	}

	// Token: 0x060042C4 RID: 17092 RVA: 0x001263EA File Offset: 0x001245EA
	public static bool SetFloatIfExists(this Animator self, int id, float value)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Float)))
		{
			self.SetFloat(id, value);
			return true;
		}
		return false;
	}

	// Token: 0x060042C5 RID: 17093 RVA: 0x00126413 File Offset: 0x00124613
	public static bool SetIntIfExists(this Animator self, int id, int value)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Int)))
		{
			self.SetInteger(id, value);
			return true;
		}
		return false;
	}

	// Token: 0x060042C6 RID: 17094 RVA: 0x0012643C File Offset: 0x0012463C
	public static bool SetBoolIfExists(this Animator self, int id, bool value)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Bool)))
		{
			self.SetBool(id, value);
			return true;
		}
		return false;
	}

	// Token: 0x060042C7 RID: 17095 RVA: 0x00126465 File Offset: 0x00124665
	public static bool SetTriggerIfExists(this Animator self, int id)
	{
		if (!self || id == 0)
		{
			return false;
		}
		if (self.HasParameter(id, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Trigger)))
		{
			self.SetTrigger(id);
			return true;
		}
		return false;
	}

	// Token: 0x060042C8 RID: 17096 RVA: 0x0012648E File Offset: 0x0012468E
	public static IEnumerator PlayAnimWait(this tk2dSpriteAnimator self, string anim, Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int> triggerAction = null)
	{
		return self.PlayAnimWait(self.GetClipByName(anim), triggerAction);
	}

	// Token: 0x060042C9 RID: 17097 RVA: 0x0012649E File Offset: 0x0012469E
	public static IEnumerator PlayAnimWait(this tk2dSpriteAnimator self, tk2dSpriteAnimationClip clip, Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int> triggerAction = null)
	{
		if (clip == null)
		{
			yield break;
		}
		self.Play(clip);
		if (triggerAction != null)
		{
			self.AnimationEventTriggered = triggerAction;
		}
		WaitForTk2dAnimatorClipFinish waitForTk2dAnimatorClipFinish = new WaitForTk2dAnimatorClipFinish(self, null);
		while (self.CurrentClip == clip && self.IsPlaying(clip) && waitForTk2dAnimatorClipFinish.keepWaiting)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060042CA RID: 17098 RVA: 0x001264BB File Offset: 0x001246BB
	public static float PlayAnimGetTime(this tk2dSpriteAnimator self, string anim)
	{
		return self.PlayAnimGetTime(self.GetClipByName(anim));
	}

	// Token: 0x060042CB RID: 17099 RVA: 0x001264CA File Offset: 0x001246CA
	public static float PlayAnimGetTime(this tk2dSpriteAnimator self, tk2dSpriteAnimationClip clip)
	{
		if (clip == null)
		{
			return 0f;
		}
		self.Play(clip);
		return clip.Duration;
	}

	// Token: 0x060042CC RID: 17100 RVA: 0x001264E4 File Offset: 0x001246E4
	public static bool TryPlay(this tk2dSpriteAnimator self, string anim)
	{
		if (string.IsNullOrEmpty(anim))
		{
			return false;
		}
		tk2dSpriteAnimationClip clipByName = self.GetClipByName(anim);
		if (clipByName == null)
		{
			return false;
		}
		self.Play(clipByName);
		return true;
	}

	// Token: 0x060042CD RID: 17101 RVA: 0x00126510 File Offset: 0x00124710
	public static void SetPositionX(this Transform t, float newX)
	{
		Vector3 position = t.position;
		position = new Vector3(newX, position.y, position.z);
		t.position = position;
	}

	// Token: 0x060042CE RID: 17102 RVA: 0x00126540 File Offset: 0x00124740
	public static void SetPositionY(this Transform t, float newY)
	{
		Vector3 position = t.position;
		position = new Vector3(position.x, newY, position.z);
		t.position = position;
	}

	// Token: 0x060042CF RID: 17103 RVA: 0x00126570 File Offset: 0x00124770
	public static void SetPositionZ(this Transform t, float newZ)
	{
		Vector3 position = t.position;
		position = new Vector3(position.x, position.y, newZ);
		t.position = position;
	}

	// Token: 0x060042D0 RID: 17104 RVA: 0x001265A0 File Offset: 0x001247A0
	public static void SetLocalPositionX(this Transform t, float newX)
	{
		Vector3 localPosition = t.localPosition;
		localPosition = new Vector3(newX, localPosition.y, localPosition.z);
		t.localPosition = localPosition;
	}

	// Token: 0x060042D1 RID: 17105 RVA: 0x001265D0 File Offset: 0x001247D0
	public static void SetLocalPositionY(this Transform t, float newY)
	{
		Vector3 localPosition = t.localPosition;
		localPosition = new Vector3(localPosition.x, newY, localPosition.z);
		t.localPosition = localPosition;
	}

	// Token: 0x060042D2 RID: 17106 RVA: 0x00126600 File Offset: 0x00124800
	public static void SetLocalPositionZ(this Transform t, float newZ)
	{
		Vector3 localPosition = t.localPosition;
		localPosition = new Vector3(localPosition.x, localPosition.y, newZ);
		t.localPosition = localPosition;
	}

	// Token: 0x060042D3 RID: 17107 RVA: 0x0012662F File Offset: 0x0012482F
	public static void SetPosition2D(this Transform t, float x, float y)
	{
		t.position = new Vector3(x, y, t.position.z);
	}

	// Token: 0x060042D4 RID: 17108 RVA: 0x00126649 File Offset: 0x00124849
	public static void SetPosition2D(this Transform t, Vector2 position)
	{
		t.position = new Vector3(position.x, position.y, t.position.z);
	}

	// Token: 0x060042D5 RID: 17109 RVA: 0x0012666D File Offset: 0x0012486D
	public static void SetLocalPosition2D(this Transform t, float x, float y)
	{
		t.localPosition = new Vector3(x, y, t.localPosition.z);
	}

	// Token: 0x060042D6 RID: 17110 RVA: 0x00126687 File Offset: 0x00124887
	public static void SetLocalPosition2D(this Transform t, Vector2 position)
	{
		t.localPosition = new Vector3(position.x, position.y, t.localPosition.z);
	}

	// Token: 0x060042D7 RID: 17111 RVA: 0x001266AB File Offset: 0x001248AB
	public static void SetPosition3D(this Transform t, float x, float y, float z)
	{
		t.position = new Vector3(x, y, z);
	}

	// Token: 0x060042D8 RID: 17112 RVA: 0x001266BC File Offset: 0x001248BC
	public static void SetScaleX(this Transform t, float newXScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(newXScale, localScale.y, localScale.z);
		t.localScale = localScale;
	}

	// Token: 0x060042D9 RID: 17113 RVA: 0x001266EC File Offset: 0x001248EC
	public static void SetScaleY(this Transform t, float newYScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(localScale.x, newYScale, localScale.z);
		t.localScale = localScale;
	}

	// Token: 0x060042DA RID: 17114 RVA: 0x0012671C File Offset: 0x0012491C
	public static void SetScaleZ(this Transform t, float newZScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(localScale.x, localScale.y, newZScale);
		t.localScale = localScale;
	}

	// Token: 0x060042DB RID: 17115 RVA: 0x0012674C File Offset: 0x0012494C
	public static void SetScale2D(this Transform t, Vector2 newScale)
	{
		Vector3 localScale = t.localScale;
		localScale = new Vector3(newScale.x, newScale.y, localScale.z);
		t.localScale = localScale;
	}

	// Token: 0x060042DC RID: 17116 RVA: 0x00126780 File Offset: 0x00124980
	public static void SetRotationZ(this Transform t, float newZRotation)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, newZRotation);
		t.localEulerAngles = localEulerAngles;
	}

	// Token: 0x060042DD RID: 17117 RVA: 0x001267AF File Offset: 0x001249AF
	public static void SetScaleMatching(this Transform t, float newScale)
	{
		t.localScale = new Vector3(newScale, newScale, newScale);
	}

	// Token: 0x060042DE RID: 17118 RVA: 0x001267C0 File Offset: 0x001249C0
	public static void FlipLocalScale(this Transform t, bool x = false, bool y = false, bool z = false)
	{
		Vector3 localScale = t.localScale;
		if (x)
		{
			localScale.x = -localScale.x;
		}
		if (y)
		{
			localScale.y = -localScale.y;
		}
		if (z)
		{
			localScale.z = -localScale.z;
		}
		t.localScale = localScale;
	}

	// Token: 0x060042DF RID: 17119 RVA: 0x0012680E File Offset: 0x00124A0E
	public static void SetParentReset(this Transform t, Transform parent)
	{
		t.SetParent(parent);
		t.Reset();
	}

	// Token: 0x060042E0 RID: 17120 RVA: 0x0012681D File Offset: 0x00124A1D
	public static void Reset(this Transform t)
	{
		t.localScale = Vector3.one;
		t.localRotation = Quaternion.identity;
		t.localPosition = Vector3.zero;
	}

	// Token: 0x060042E1 RID: 17121 RVA: 0x00126840 File Offset: 0x00124A40
	public static bool IsOnHeroPlane(this Transform transform)
	{
		return Mathf.Abs(transform.position.z - 0.004f) <= 1.8f;
	}

	// Token: 0x060042E2 RID: 17122 RVA: 0x00126862 File Offset: 0x00124A62
	public static float GetPositionX(this Transform t)
	{
		return t.position.x;
	}

	// Token: 0x060042E3 RID: 17123 RVA: 0x0012686F File Offset: 0x00124A6F
	public static float GetPositionY(this Transform t)
	{
		return t.position.y;
	}

	// Token: 0x060042E4 RID: 17124 RVA: 0x0012687C File Offset: 0x00124A7C
	public static float GetPositionZ(this Transform t)
	{
		return t.position.z;
	}

	// Token: 0x060042E5 RID: 17125 RVA: 0x00126889 File Offset: 0x00124A89
	public static float GetScaleX(this Transform t)
	{
		return t.localScale.x;
	}

	// Token: 0x060042E6 RID: 17126 RVA: 0x00126896 File Offset: 0x00124A96
	public static float GetScaleY(this Transform t)
	{
		return t.localScale.y;
	}

	// Token: 0x060042E7 RID: 17127 RVA: 0x001268A3 File Offset: 0x00124AA3
	public static float GetScaleZ(this Transform t)
	{
		return t.localScale.z;
	}

	// Token: 0x060042E8 RID: 17128 RVA: 0x001268B0 File Offset: 0x00124AB0
	public static float GetLocalRotation2D(this Transform t)
	{
		return t.localEulerAngles.z;
	}

	// Token: 0x060042E9 RID: 17129 RVA: 0x001268BD File Offset: 0x00124ABD
	public static float GetRotation2D(this Transform t)
	{
		return t.eulerAngles.z;
	}

	// Token: 0x060042EA RID: 17130 RVA: 0x001268CC File Offset: 0x00124ACC
	public static void SetLocalRotation2D(this Transform t, float rotation)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		localEulerAngles.z = rotation;
		t.localEulerAngles = localEulerAngles;
	}

	// Token: 0x060042EB RID: 17131 RVA: 0x001268F0 File Offset: 0x00124AF0
	public static void SetRotation2D(this Transform t, float rotation)
	{
		Vector3 eulerAngles = t.eulerAngles;
		eulerAngles.z = rotation;
		t.eulerAngles = eulerAngles;
	}

	// Token: 0x060042EC RID: 17132 RVA: 0x00126914 File Offset: 0x00124B14
	public static float TransformRadius(this Transform t, float radius)
	{
		Vector3 lossyScale = t.lossyScale;
		float num = Mathf.Abs(Mathf.Max(lossyScale.x, lossyScale.y));
		return radius * num;
	}

	// Token: 0x060042ED RID: 17133 RVA: 0x00126944 File Offset: 0x00124B44
	public static bool IsWithinGameCameraBoundsDistance(this Transform transform, Vector2 margins)
	{
		Vector3 position = GameCameras.instance.mainCamera.transform.position;
		Vector3 position2 = transform.position;
		float num = position2.x - position.x;
		float num2 = position2.y - position.y;
		if (num < 0f)
		{
			num *= -1f;
		}
		if (num2 < 0f)
		{
			num2 *= -1f;
		}
		return num <= 15f + margins.x && num2 <= 9f + margins.y;
	}

	// Token: 0x060042EE RID: 17134 RVA: 0x001269CC File Offset: 0x00124BCC
	public static bool IsAny(this string value, params string[] others)
	{
		foreach (string value2 in others)
		{
			if (value.Equals(value2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060042EF RID: 17135 RVA: 0x001269FC File Offset: 0x00124BFC
	public static bool ContainsAny(this string value, params string[] others)
	{
		foreach (string value2 in others)
		{
			if (value.Contains(value2))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060042F0 RID: 17136 RVA: 0x00126A2C File Offset: 0x00124C2C
	public static bool TryFormat(this string text, out string outText, params object[] formatItems)
	{
		bool result;
		try
		{
			outText = string.Format(text, formatItems);
			result = true;
		}
		catch (FormatException)
		{
			outText = text;
			result = false;
		}
		return result;
	}

	// Token: 0x060042F1 RID: 17137 RVA: 0x00126A60 File Offset: 0x00124C60
	public static string ToSingleLine(this string multilineText)
	{
		LanguageCode languageCode = Language.CurrentLanguage();
		string text;
		if (languageCode == LanguageCode.JA || languageCode == LanguageCode.ZH)
		{
			text = "";
		}
		else
		{
			text = " ";
		}
		string newValue = text;
		return multilineText.Replace("\n", newValue);
	}

	// Token: 0x060042F2 RID: 17138 RVA: 0x00126A9C File Offset: 0x00124C9C
	public static T GetBy2DIndexes<T>(this List<T> list, int width, int x, int y, T def = default(T))
	{
		int num = y * width + x;
		if (num < 0 || num >= list.Count || x >= width || x < 0)
		{
			return def;
		}
		return list[num];
	}

	// Token: 0x060042F3 RID: 17139 RVA: 0x00126ACE File Offset: 0x00124CCE
	public static void SendEventSafe(this PlayMakerFSM fsm, string eventName)
	{
		if (fsm != null)
		{
			fsm.SendEvent(eventName);
		}
	}

	// Token: 0x060042F4 RID: 17140 RVA: 0x00126AE0 File Offset: 0x00124CE0
	public static Color Where(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
	{
		return new Color(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
	}

	// Token: 0x060042F5 RID: 17141 RVA: 0x00126B5C File Offset: 0x00124D5C
	public static Vector3 Where(this Vector3 original, float? x = null, float? y = null, float? z = null)
	{
		return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
	}

	// Token: 0x060042F6 RID: 17142 RVA: 0x00126BBC File Offset: 0x00124DBC
	public static Vector2 Where(this Vector2 original, float? x = null, float? y = null)
	{
		return new Vector2(x ?? original.x, y ?? original.y);
	}

	// Token: 0x060042F7 RID: 17143 RVA: 0x00126C02 File Offset: 0x00124E02
	public static Vector3 ToVector3(this Vector2 original, float z)
	{
		return new Vector3(original.x, original.y, z);
	}

	// Token: 0x060042F8 RID: 17144 RVA: 0x00126C16 File Offset: 0x00124E16
	public static Coroutine StartTimerRoutine(this MonoBehaviour self, float delay, float duration, Action<float> handler, Action onAfterDelay = null, Action onTimerEnd = null, bool isRealtime = false)
	{
		if (duration <= 0f && delay <= 0f)
		{
			Extensions.TimerRoutine(delay, duration, handler, onAfterDelay, onTimerEnd, isRealtime).MoveNext();
			return null;
		}
		return self.StartCoroutine(Extensions.TimerRoutine(delay, duration, handler, onAfterDelay, onTimerEnd, isRealtime));
	}

	// Token: 0x060042F9 RID: 17145 RVA: 0x00126C52 File Offset: 0x00124E52
	private static IEnumerator TimerRoutine(float delay, float duration, Action<float> handler, Action onAfterDelay, Action onTimerEnd, bool isRealtime)
	{
		if (handler != null)
		{
			handler(0f);
		}
		if (delay > 0f)
		{
			if (isRealtime)
			{
				yield return new WaitForSecondsRealtime(delay);
			}
			else
			{
				yield return new WaitForSeconds(delay);
			}
		}
		if (onAfterDelay != null)
		{
			onAfterDelay();
		}
		if (handler != null)
		{
			float elapsed = 0f;
			while (elapsed < duration)
			{
				handler(elapsed / duration);
				yield return null;
				if (isRealtime)
				{
					elapsed += Time.unscaledDeltaTime;
				}
				else
				{
					elapsed += Time.deltaTime;
				}
			}
			handler(1f);
		}
		else if (duration > 0f)
		{
			if (isRealtime)
			{
				yield return new WaitForSecondsRealtime(duration);
			}
			else
			{
				yield return new WaitForSeconds(duration);
			}
		}
		if (onTimerEnd != null)
		{
			onTimerEnd();
		}
		yield break;
	}

	// Token: 0x060042FA RID: 17146 RVA: 0x00126C86 File Offset: 0x00124E86
	public static Coroutine ExecuteDelayed(this MonoBehaviour runner, float delay, Action handler)
	{
		if (handler == null)
		{
			return null;
		}
		return runner.StartCoroutine(Extensions.DelayRoutine(delay, handler));
	}

	// Token: 0x060042FB RID: 17147 RVA: 0x00126C9A File Offset: 0x00124E9A
	private static IEnumerator DelayRoutine(float delay, Action handler)
	{
		yield return new WaitForSeconds(delay);
		handler();
		yield break;
	}

	// Token: 0x060042FC RID: 17148 RVA: 0x00126CB0 File Offset: 0x00124EB0
	public static void SetFsmBoolIfExists(this PlayMakerFSM fsm, string boolName, bool value)
	{
		FsmBool fsmBool = fsm.FsmVariables.FindFsmBool(boolName);
		if (fsmBool != null)
		{
			fsmBool.Value = value;
		}
	}

	// Token: 0x060042FD RID: 17149 RVA: 0x00126CD4 File Offset: 0x00124ED4
	public static bool GetFsmBoolIfExists(this PlayMakerFSM fsm, string boolName)
	{
		FsmBool fsmBool = fsm.FsmVariables.FindFsmBool(boolName);
		return fsmBool != null && fsmBool.Value;
	}

	// Token: 0x060042FE RID: 17150 RVA: 0x00126CFC File Offset: 0x00124EFC
	public static bool IsEventValid(this PlayMakerFSM fsm, string eventName)
	{
		bool? flag = fsm.IsEventValid(eventName, true);
		return flag != null && flag.Value;
	}

	// Token: 0x060042FF RID: 17151 RVA: 0x00126D24 File Offset: 0x00124F24
	public static bool? IsEventValid(this PlayMakerFSM fsmComponent, string eventName, bool isRequired)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			if (!isRequired)
			{
				return null;
			}
			return new bool?(false);
		}
		else
		{
			if (!fsmComponent)
			{
				return null;
			}
			return new bool?(Extensions.IsEventInFsmRecursive((fsmComponent.FsmTemplate == null) ? fsmComponent.Fsm : fsmComponent.FsmTemplate.fsm, eventName));
		}
	}

	// Token: 0x06004300 RID: 17152 RVA: 0x00126D8C File Offset: 0x00124F8C
	private static bool IsEventInFsmRecursive(Fsm fsm, string eventName)
	{
		FsmEvent[] events = fsm.Events;
		for (int i = 0; i < events.Length; i++)
		{
			if (events[i].Name == eventName)
			{
				return true;
			}
		}
		using (List<Fsm>.Enumerator enumerator = fsm.SubFsmList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Extensions.IsEventInFsmRecursive(enumerator.Current, eventName))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06004301 RID: 17153 RVA: 0x00126E10 File Offset: 0x00125010
	public static bool SendEventRecursive(this PlayMakerFSM fsm, string eventName)
	{
		return fsm && fsm.Fsm.SendEventRecursive(eventName);
	}

	// Token: 0x06004302 RID: 17154 RVA: 0x00126E28 File Offset: 0x00125028
	public static bool SendEventRecursive(this Fsm fsm, string eventName)
	{
		if (!fsm.Started)
		{
			return false;
		}
		foreach (Fsm fsm2 in fsm.SubFsmList)
		{
			if (fsm2.Active && fsm2.SendEventRecursive(eventName))
			{
				return true;
			}
		}
		string activeStateName = fsm.ActiveStateName;
		FsmState activeState = fsm.ActiveState;
		int num;
		int num2;
		if (activeState != null)
		{
			num = activeState.ActiveActionIndex;
			num2 = activeState.loopCount;
		}
		else
		{
			num = -1;
			num2 = -1;
		}
		fsm.Event(eventName);
		FsmState activeState2 = fsm.ActiveState;
		if (activeState2 != null)
		{
			if (activeState2.loopCount != num2)
			{
				return true;
			}
			if (activeState2.ActiveActionIndex != num)
			{
				return true;
			}
		}
		return !fsm.ActiveStateName.Equals(activeStateName);
	}

	// Token: 0x06004303 RID: 17155 RVA: 0x00126EFC File Offset: 0x001250FC
	public static bool? IsAnimValid(this tk2dSpriteAnimator animator, string animName, bool isRequired)
	{
		if (string.IsNullOrEmpty(animName))
		{
			if (!isRequired)
			{
				return null;
			}
			return new bool?(false);
		}
		else
		{
			if (!animator)
			{
				return new bool?(false);
			}
			return new bool?(animator.GetClipByName(animName) != null);
		}
	}

	// Token: 0x06004304 RID: 17156 RVA: 0x00126F44 File Offset: 0x00125144
	public static bool? IsVariableValid(this PlayMakerFSM fsm, string variableName, bool isRequired)
	{
		if (string.IsNullOrEmpty(variableName))
		{
			if (!isRequired)
			{
				return null;
			}
			return new bool?(false);
		}
		else
		{
			if (!fsm)
			{
				return null;
			}
			return new bool?(((fsm.FsmTemplate == null) ? fsm.FsmVariables : fsm.FsmTemplate.fsm.Variables).Contains(variableName));
		}
	}

	// Token: 0x06004305 RID: 17157 RVA: 0x00126FB0 File Offset: 0x001251B0
	public static bool IsWithinTolerance(this float original, float tolerance, float target)
	{
		return Mathf.Abs(target - original) <= tolerance;
	}

	// Token: 0x06004306 RID: 17158 RVA: 0x00126FC0 File Offset: 0x001251C0
	public static bool IsAngleWithinTolerance(this float original, float tolerance, float target)
	{
		original = (original + 360f) % 360f;
		target = (target + 360f) % 360f;
		float num = Mathf.Abs(target - original);
		num = Mathf.Min(num, 360f - num);
		return num <= tolerance;
	}

	// Token: 0x06004307 RID: 17159 RVA: 0x00127009 File Offset: 0x00125209
	public static float DirectionToAngle(this Vector2 direction)
	{
		return Mathf.Atan2(direction.x, -direction.y) * 180f / 3.1415927f - 90f;
	}

	// Token: 0x06004308 RID: 17160 RVA: 0x00127030 File Offset: 0x00125230
	public static Vector2 AngleToDirection(this float angle)
	{
		float f = angle * 0.017453292f;
		return new Vector2(Mathf.Cos(f), Mathf.Sin(f));
	}

	// Token: 0x06004309 RID: 17161 RVA: 0x00127058 File Offset: 0x00125258
	public static Vector3 MultiplyElements(this Vector3 self, Vector3 other)
	{
		Vector3 result = self;
		result.x *= other.x;
		result.y *= other.y;
		result.z *= other.z;
		return result;
	}

	// Token: 0x0600430A RID: 17162 RVA: 0x0012709C File Offset: 0x0012529C
	public static Vector3 MultiplyElements(this Vector3 self, float? x = null, float? y = null, float? z = null)
	{
		Vector3 result = self;
		result.x *= (x ?? 1f);
		result.y *= (y ?? 1f);
		result.z *= (z ?? 1f);
		return result;
	}

	// Token: 0x0600430B RID: 17163 RVA: 0x00127118 File Offset: 0x00125318
	public static Vector2 MultiplyElements(this Vector2 self, Vector2 other)
	{
		Vector2 result = self;
		result.x *= other.x;
		result.y *= other.y;
		return result;
	}

	// Token: 0x0600430C RID: 17164 RVA: 0x0012714C File Offset: 0x0012534C
	public static Vector2 MultiplyElements(this Vector2 self, float? x = null, float? y = null)
	{
		Vector2 result = self;
		result.x *= (x ?? 1f);
		result.y *= (y ?? 1f);
		return result;
	}

	// Token: 0x0600430D RID: 17165 RVA: 0x001271A4 File Offset: 0x001253A4
	public static Vector2 ClampVector2(this Vector2 self, Vector2 min, Vector2 max)
	{
		Vector2 result;
		result.x = Mathf.Clamp(self.x, min.x, max.x);
		result.y = Mathf.Clamp(self.y, min.y, max.y);
		return result;
	}

	// Token: 0x0600430E RID: 17166 RVA: 0x001271F0 File Offset: 0x001253F0
	public static Vector4 MultiplyElements(this Vector4 self, Vector4 other)
	{
		Vector4 result = self;
		result.x *= other.x;
		result.y *= other.y;
		result.z *= other.z;
		result.w += other.w;
		return result;
	}

	// Token: 0x0600430F RID: 17167 RVA: 0x00127244 File Offset: 0x00125444
	public static Vector4 MultiplyElements(this Vector4 self, float? x = null, float? y = null, float? z = null, float? w = null)
	{
		Vector4 result = self;
		result.x *= (x ?? 1f);
		result.y *= (y ?? 1f);
		result.z *= (z ?? 1f);
		result.w *= (w ?? 1f);
		return result;
	}

	// Token: 0x06004310 RID: 17168 RVA: 0x001272E8 File Offset: 0x001254E8
	public static Vector3 DivideElements(this Vector3 self, Vector3 other)
	{
		Vector3 result = self;
		result.x /= other.x;
		result.y /= other.y;
		result.z /= other.z;
		return result;
	}

	// Token: 0x06004311 RID: 17169 RVA: 0x0012732C File Offset: 0x0012552C
	public static Vector3 DivideElements(this Vector3 self, float? x = null, float? y = null, float? z = null)
	{
		Vector3 result = self;
		result.x /= (x ?? 1f);
		result.y /= (y ?? 1f);
		result.z /= (z ?? 1f);
		return result;
	}

	// Token: 0x06004312 RID: 17170 RVA: 0x001273A8 File Offset: 0x001255A8
	public static Vector2 DivideElements(this Vector2 self, Vector2 other)
	{
		Vector2 result = self;
		result.x /= other.x;
		result.y /= other.y;
		return result;
	}

	// Token: 0x06004313 RID: 17171 RVA: 0x001273DC File Offset: 0x001255DC
	public static Vector2 DivideElements(this Vector2 self, float? x = null, float? y = null)
	{
		Vector2 result = self;
		result.x /= (x ?? 1f);
		result.y /= (y ?? 1f);
		return result;
	}

	// Token: 0x06004314 RID: 17172 RVA: 0x00127434 File Offset: 0x00125634
	public static Vector4 DivideElements(this Vector4 self, Vector4 other)
	{
		Vector4 result = self;
		result.x /= other.x;
		result.y /= other.y;
		result.z /= other.z;
		result.w /= other.w;
		return result;
	}

	// Token: 0x06004315 RID: 17173 RVA: 0x00127488 File Offset: 0x00125688
	public static Vector4 DivideElements(this Vector4 self, float? x = null, float? y = null, float? z = null, float? w = null)
	{
		Vector4 result = self;
		result.x /= (x ?? 1f);
		result.y /= (y ?? 1f);
		result.z /= (z ?? 1f);
		result.w /= (w ?? 1f);
		return result;
	}

	// Token: 0x06004316 RID: 17174 RVA: 0x00127529 File Offset: 0x00125729
	public static Vector3 Abs(this Vector3 self)
	{
		return new Vector3(Mathf.Abs(self.x), Mathf.Abs(self.y), Mathf.Abs(self.z));
	}

	// Token: 0x06004317 RID: 17175 RVA: 0x00127551 File Offset: 0x00125751
	public static Vector2 Abs(this Vector2 self)
	{
		return new Vector2(Mathf.Abs(self.x), Mathf.Abs(self.y));
	}

	// Token: 0x06004318 RID: 17176 RVA: 0x00127570 File Offset: 0x00125770
	public static Color MultiplyElements(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
	{
		return original * (r ?? 1f) * (g ?? 1f) * (b ?? 1f) * (a ?? 1f);
	}

	// Token: 0x06004319 RID: 17177 RVA: 0x001275F8 File Offset: 0x001257F8
	public static Color MultiplyElements(this Color original, Color other)
	{
		Color result = original;
		result.r *= other.r;
		result.g *= other.g;
		result.b *= other.b;
		result.a *= other.a;
		return result;
	}

	// Token: 0x0600431A RID: 17178 RVA: 0x0012764C File Offset: 0x0012584C
	public static bool AddIfNotPresent<T>(this List<T> list, T item)
	{
		if (!list.Contains(item))
		{
			list.Add(item);
			return true;
		}
		return false;
	}

	// Token: 0x0600431B RID: 17179 RVA: 0x00127664 File Offset: 0x00125864
	public static void RemoveNulls<T>(this List<T> list)
	{
		for (int i = list.Count - 1; i >= 0; i--)
		{
			if (list[i] == null)
			{
				list.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600431C RID: 17180 RVA: 0x0012769C File Offset: 0x0012589C
	public static bool IsRayHittingLocal(this Transform transform, Vector2 originLocal, Vector2 directionLocal, float length, int layerMask)
	{
		Vector2 origin = transform.TransformPoint(originLocal);
		Vector2 direction = transform.TransformVector(directionLocal);
		return Helper.Raycast2D(origin, direction, length, layerMask).collider != null;
	}

	// Token: 0x0600431D RID: 17181 RVA: 0x001276E4 File Offset: 0x001258E4
	public static bool IsRayHittingLocalNoTriggers(this Transform transform, Vector2 originLocal, Vector2 directionLocal, float length, int layerMask)
	{
		Vector2 origin = transform.TransformPoint(originLocal);
		Vector2 direction = transform.TransformVector(directionLocal);
		return Helper.IsRayHittingNoTriggers(origin, direction, length, layerMask);
	}

	// Token: 0x0600431E RID: 17182 RVA: 0x00127720 File Offset: 0x00125920
	public static void SetVelocity(this Rigidbody2D body, float? x = null, float? y = null)
	{
		Vector2 linearVelocity = body.linearVelocity;
		linearVelocity.x = (x ?? linearVelocity.x);
		linearVelocity.y = (y ?? linearVelocity.y);
		body.linearVelocity = linearVelocity;
	}

	// Token: 0x0600431F RID: 17183 RVA: 0x00127780 File Offset: 0x00125980
	public static T GetRandomElement<T>(this T[] array)
	{
		if (array == null || array.Length == 0)
		{
			return default(T);
		}
		return array[Random.Range(0, array.Length)];
	}

	// Token: 0x06004320 RID: 17184 RVA: 0x001277B0 File Offset: 0x001259B0
	public static T GetRandomElement<T>(this List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x06004321 RID: 17185 RVA: 0x001277E4 File Offset: 0x001259E4
	public static T GetAndRemoveRandomElement<T>(this List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		int index = Random.Range(0, list.Count);
		T result = list[index];
		list.RemoveAt(index);
		return result;
	}

	// Token: 0x06004322 RID: 17186 RVA: 0x00127821 File Offset: 0x00125A21
	public static Vector2 RandomInRange(this Vector2 original)
	{
		return new Vector2(Random.Range(-original.x, original.x), Random.Range(-original.y, original.y));
	}

	// Token: 0x06004323 RID: 17187 RVA: 0x0012784C File Offset: 0x00125A4C
	public static Vector3 RandomInRange(this Vector3 original)
	{
		return new Vector3(Random.Range(-original.x, original.x), Random.Range(-original.y, original.y), Random.Range(-original.z, original.z));
	}

	// Token: 0x06004324 RID: 17188 RVA: 0x00127889 File Offset: 0x00125A89
	public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
	{
		return collection == null || collection.Count == 0;
	}

	// Token: 0x06004325 RID: 17189 RVA: 0x0012789C File Offset: 0x00125A9C
	public static void SetAllActive(this ICollection<GameObject> collection, bool value)
	{
		if (collection == null)
		{
			return;
		}
		foreach (GameObject gameObject in collection)
		{
			if (!(gameObject == null))
			{
				gameObject.SetActive(value);
			}
		}
	}

	// Token: 0x06004326 RID: 17190 RVA: 0x001278F4 File Offset: 0x00125AF4
	public static void PushIfNotNull<T>(this Stack<T> stack, T value)
	{
		if (value != null)
		{
			stack.Push(value);
		}
	}

	// Token: 0x06004327 RID: 17191 RVA: 0x00127905 File Offset: 0x00125B05
	public static T PushReturn<T>(this Stack<T> stack, T value)
	{
		stack.Push(value);
		return value;
	}

	// Token: 0x06004328 RID: 17192 RVA: 0x0012790F File Offset: 0x00125B0F
	public static T PushIfNotNullReturn<T>(this Stack<T> stack, T value)
	{
		if (value != null)
		{
			stack.Push(value);
		}
		return value;
	}

	// Token: 0x06004329 RID: 17193 RVA: 0x00127921 File Offset: 0x00125B21
	public static void ForceUpdateLayoutNoCanvas(this LayoutGroup layoutGroup)
	{
		layoutGroup.enabled = false;
		layoutGroup.CalculateLayoutInputVertical();
		layoutGroup.SetLayoutVertical();
		layoutGroup.CalculateLayoutInputHorizontal();
		layoutGroup.SetLayoutHorizontal();
		layoutGroup.enabled = true;
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);
	}

	// Token: 0x0600432A RID: 17194 RVA: 0x00127959 File Offset: 0x00125B59
	public static float GetArea(this Vector2 vector)
	{
		return Mathf.Abs(vector.x) * Mathf.Abs(vector.y);
	}

	// Token: 0x0600432B RID: 17195 RVA: 0x00127972 File Offset: 0x00125B72
	public static float GetArea(this Vector3 vector)
	{
		return Mathf.Abs(vector.x) * Mathf.Abs(vector.y) * Mathf.Abs(vector.z);
	}

	// Token: 0x0600432C RID: 17196 RVA: 0x00127997 File Offset: 0x00125B97
	public static Vector3 GetSign(this Vector3 vector3)
	{
		return new Vector3(Mathf.Sign(vector3.x), Mathf.Sign(vector3.y), Mathf.Sign(vector3.z));
	}

	// Token: 0x0600432D RID: 17197 RVA: 0x001279C0 File Offset: 0x00125BC0
	public static bool Test(this int lhs, Extensions.IntTest test, int rhs)
	{
		switch (test)
		{
		case Extensions.IntTest.Equal:
			return lhs == rhs;
		case Extensions.IntTest.LessThan:
			return lhs < rhs;
		case Extensions.IntTest.MoreThan:
			return lhs > rhs;
		case Extensions.IntTest.LessThanOrEqual:
			return lhs <= rhs;
		case Extensions.IntTest.MoreThanOrEqual:
			return lhs >= rhs;
		default:
			throw new NotImplementedException();
		}
	}

	// Token: 0x0600432E RID: 17198 RVA: 0x00127A10 File Offset: 0x00125C10
	public static void Shuffle<T>(this IList<T> ts)
	{
		int count = ts.Count;
		int num = count - 1;
		for (int i = 0; i < num; i++)
		{
			int num2 = Random.Range(i, count);
			int index = i;
			int index2 = num2;
			T value = ts[num2];
			T value2 = ts[i];
			ts[index] = value;
			ts[index2] = value2;
		}
	}

	// Token: 0x0600432F RID: 17199 RVA: 0x00127A72 File Offset: 0x00125C72
	public static InventoryItemManager.SelectionDirection Opposite(this InventoryItemManager.SelectionDirection direction)
	{
		switch (direction)
		{
		case InventoryItemManager.SelectionDirection.Up:
			return InventoryItemManager.SelectionDirection.Down;
		case InventoryItemManager.SelectionDirection.Down:
			return InventoryItemManager.SelectionDirection.Up;
		case InventoryItemManager.SelectionDirection.Left:
			return InventoryItemManager.SelectionDirection.Right;
		case InventoryItemManager.SelectionDirection.Right:
			return InventoryItemManager.SelectionDirection.Left;
		default:
			throw new NotImplementedException();
		}
	}

	// Token: 0x06004330 RID: 17200 RVA: 0x00127A9C File Offset: 0x00125C9C
	public static void CopyFrom(this Collider2D self, Collider2D other)
	{
		if (self is PolygonCollider2D)
		{
			if (other is PolygonCollider2D)
			{
				((PolygonCollider2D)self).CopyFrom((PolygonCollider2D)other);
				return;
			}
			throw new MismatchedTypeException();
		}
		else if (self is BoxCollider2D)
		{
			if (other is BoxCollider2D)
			{
				((BoxCollider2D)self).CopyFrom((BoxCollider2D)other);
				return;
			}
			throw new MismatchedTypeException();
		}
		else
		{
			if (!(self is CircleCollider2D))
			{
				throw new NotImplementedException();
			}
			if (other is CircleCollider2D)
			{
				((CircleCollider2D)self).CopyFrom((CircleCollider2D)other);
				return;
			}
			throw new MismatchedTypeException();
		}
	}

	// Token: 0x06004331 RID: 17201 RVA: 0x00127B28 File Offset: 0x00125D28
	private static void CopyShared(Collider2D to, Collider2D from)
	{
		to.offset = from.offset;
		to.isTrigger = from.isTrigger;
		to.usedByEffector = from.usedByEffector;
		to.usedByComposite = from.usedByComposite;
		to.sharedMaterial = from.sharedMaterial;
		to.includeLayers = from.includeLayers;
		to.excludeLayers = from.excludeLayers;
		to.forceSendLayers = from.forceSendLayers;
		to.forceReceiveLayers = from.forceReceiveLayers;
		to.contactCaptureLayers = from.contactCaptureLayers;
		to.callbackLayers = from.callbackLayers;
	}

	// Token: 0x06004332 RID: 17202 RVA: 0x00127BB9 File Offset: 0x00125DB9
	public static void CopyFrom(this BoxCollider2D self, BoxCollider2D other)
	{
		Extensions.CopyShared(self, other);
		self.offset = other.offset;
		self.size = other.size;
	}

	// Token: 0x06004333 RID: 17203 RVA: 0x00127BDA File Offset: 0x00125DDA
	public static void CopyFrom(this CircleCollider2D self, CircleCollider2D other)
	{
		Extensions.CopyShared(self, other);
		self.offset = other.offset;
		self.radius = other.radius;
	}

	// Token: 0x06004334 RID: 17204 RVA: 0x00127BFC File Offset: 0x00125DFC
	public static void CopyFrom(this PolygonCollider2D self, PolygonCollider2D other)
	{
		Extensions.CopyShared(self, other);
		self.offset = other.offset;
		self.pathCount = other.pathCount;
		for (int i = 0; i < self.pathCount; i++)
		{
			self.SetPath(i, other.GetPath(i));
		}
	}

	// Token: 0x06004335 RID: 17205 RVA: 0x00127C48 File Offset: 0x00125E48
	public static T AddComponentIfNotPresent<T>(this GameObject obj) where T : Component
	{
		T component = obj.GetComponent<T>();
		if (!component)
		{
			return obj.AddComponent<T>();
		}
		return component;
	}

	// Token: 0x06004336 RID: 17206 RVA: 0x00127C74 File Offset: 0x00125E74
	public static bool IsBitSet(this int bitmask, int index)
	{
		int num = 1 << index;
		return (bitmask & num) == num;
	}

	// Token: 0x06004337 RID: 17207 RVA: 0x00127C8E File Offset: 0x00125E8E
	public static int SetBitAtIndex(this int bitMask, int index)
	{
		bitMask |= 1 << index;
		return bitMask;
	}

	// Token: 0x06004338 RID: 17208 RVA: 0x00127C9B File Offset: 0x00125E9B
	public static int ResetBitAtIndex(this int bitMask, int index)
	{
		bitMask &= ~(1 << index);
		return bitMask;
	}

	// Token: 0x06004339 RID: 17209 RVA: 0x00127CAC File Offset: 0x00125EAC
	public static bool IsBitSet(this uint bitmask, int index)
	{
		int num = 1 << index;
		return ((ulong)bitmask & (ulong)((long)num)) == (ulong)((long)num);
	}

	// Token: 0x0600433A RID: 17210 RVA: 0x00127CC9 File Offset: 0x00125EC9
	public static uint SetBitAtIndex(this uint bitMask, int index)
	{
		bitMask |= 1U << index;
		return bitMask;
	}

	// Token: 0x0600433B RID: 17211 RVA: 0x00127CD6 File Offset: 0x00125ED6
	public static uint ResetBitAtIndex(this uint bitMask, int index)
	{
		bitMask &= ~(1U << index);
		return bitMask;
	}

	// Token: 0x0600433C RID: 17212 RVA: 0x00127CE4 File Offset: 0x00125EE4
	public static bool IsBitSet(this long bitmask, int index)
	{
		long num = 1L << index;
		return (bitmask & num) == num;
	}

	// Token: 0x0600433D RID: 17213 RVA: 0x00127CFF File Offset: 0x00125EFF
	public static long SetBitAtIndex(this long bitMask, int index)
	{
		bitMask |= 1L << index;
		return bitMask;
	}

	// Token: 0x0600433E RID: 17214 RVA: 0x00127D0D File Offset: 0x00125F0D
	public static long ResetBitAtIndex(this long bitMask, int index)
	{
		bitMask &= ~(1L << index);
		return bitMask;
	}

	// Token: 0x0600433F RID: 17215 RVA: 0x00127D1C File Offset: 0x00125F1C
	public static bool IsBitSet(this ulong bitmask, int index)
	{
		ulong num = 1UL << index;
		return (bitmask & num) == num;
	}

	// Token: 0x06004340 RID: 17216 RVA: 0x00127D37 File Offset: 0x00125F37
	public static ulong SetBitAtIndex(this ulong bitMask, int index)
	{
		bitMask |= 1UL << index;
		return bitMask;
	}

	// Token: 0x06004341 RID: 17217 RVA: 0x00127D45 File Offset: 0x00125F45
	public static ulong ResetBitAtIndex(this ulong bitMask, int index)
	{
		bitMask &= ~(1UL << index);
		return bitMask;
	}

	// Token: 0x06004342 RID: 17218 RVA: 0x00127D54 File Offset: 0x00125F54
	public static int CountSetBits(this ulong value)
	{
		int num = 0;
		while (value != 0UL)
		{
			num++;
			value &= value - 1UL;
		}
		return num;
	}

	// Token: 0x06004343 RID: 17219 RVA: 0x00127D78 File Offset: 0x00125F78
	public static void EmptyArray<T>(this T[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = default(T);
		}
	}

	// Token: 0x06004344 RID: 17220 RVA: 0x00127DA3 File Offset: 0x00125FA3
	private static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
	{
		if (type == typeof(object))
		{
			return new List<FieldInfo>();
		}
		List<FieldInfo> allFields = type.BaseType.GetAllFields(flags);
		allFields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
		return allFields;
	}

	// Token: 0x06004345 RID: 17221 RVA: 0x00127DD8 File Offset: 0x00125FD8
	private static T DeepCopy<T>(T obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("Object cannot be null");
		}
		return (T)((object)Extensions.DoCopy(obj));
	}

	// Token: 0x06004346 RID: 17222 RVA: 0x00127E00 File Offset: 0x00126000
	private static object DoCopy(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		Type type = obj.GetType();
		if (type.IsValueType || type == typeof(string))
		{
			return obj;
		}
		if (type.IsArray)
		{
			Type elementType = type.GetElementType();
			Array array = obj as Array;
			Array array2 = Array.CreateInstance(elementType, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				array2.SetValue(Extensions.DoCopy(array.GetValue(i)), i);
			}
			return Convert.ChangeType(array2, obj.GetType());
		}
		if (typeof(Object).IsAssignableFrom(type))
		{
			return obj;
		}
		if (type.IsClass)
		{
			object obj2 = Activator.CreateInstance(obj.GetType());
			foreach (FieldInfo fieldInfo in type.GetAllFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				object value = fieldInfo.GetValue(obj);
				if (value != null)
				{
					fieldInfo.SetValue(obj2, Extensions.DoCopy(value));
				}
			}
			return obj2;
		}
		throw new ArgumentException("Unknown type");
	}

	// Token: 0x06004347 RID: 17223 RVA: 0x00127F20 File Offset: 0x00126120
	public static T Clone<T>(this T ev) where T : UnityEventBase
	{
		return Extensions.DeepCopy<T>(ev);
	}

	// Token: 0x02001A2C RID: 6700
	public enum IntTest
	{
		// Token: 0x040098E4 RID: 39140
		Equal,
		// Token: 0x040098E5 RID: 39141
		LessThan,
		// Token: 0x040098E6 RID: 39142
		MoreThan,
		// Token: 0x040098E7 RID: 39143
		LessThanOrEqual,
		// Token: 0x040098E8 RID: 39144
		MoreThanOrEqual
	}
}
