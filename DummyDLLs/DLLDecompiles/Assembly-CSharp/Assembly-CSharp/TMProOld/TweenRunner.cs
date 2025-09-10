using System;
using System.Collections;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x0200080A RID: 2058
	internal class TweenRunner<T> where T : struct, ITweenValue
	{
		// Token: 0x0600480A RID: 18442 RVA: 0x0014F1F9 File Offset: 0x0014D3F9
		private static IEnumerator Start(T tweenInfo)
		{
			if (!tweenInfo.ValidTarget())
			{
				yield break;
			}
			float elapsedTime = 0f;
			while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += (tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
				float floatPercentage = Mathf.Clamp01(elapsedTime / tweenInfo.duration);
				tweenInfo.TweenValue(floatPercentage);
				yield return null;
			}
			tweenInfo.TweenValue(1f);
			yield break;
		}

		// Token: 0x0600480B RID: 18443 RVA: 0x0014F208 File Offset: 0x0014D408
		public void Init(MonoBehaviour coroutineContainer)
		{
			this.m_CoroutineContainer = coroutineContainer;
		}

		// Token: 0x0600480C RID: 18444 RVA: 0x0014F214 File Offset: 0x0014D414
		public void StartTween(T info)
		{
			if (this.m_CoroutineContainer == null)
			{
				Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
				return;
			}
			this.StopTween();
			if (!this.m_CoroutineContainer.gameObject.activeInHierarchy)
			{
				info.TweenValue(1f);
				return;
			}
			this.m_Tween = TweenRunner<T>.Start(info);
			this.m_CoroutineContainer.StartCoroutine(this.m_Tween);
		}

		// Token: 0x0600480D RID: 18445 RVA: 0x0014F283 File Offset: 0x0014D483
		public void StopTween()
		{
			if (this.m_Tween != null)
			{
				this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
				this.m_Tween = null;
			}
		}

		// Token: 0x0400488A RID: 18570
		protected MonoBehaviour m_CoroutineContainer;

		// Token: 0x0400488B RID: 18571
		protected IEnumerator m_Tween;
	}
}
