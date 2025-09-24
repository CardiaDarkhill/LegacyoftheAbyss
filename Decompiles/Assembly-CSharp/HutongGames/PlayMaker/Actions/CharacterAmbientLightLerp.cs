using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDC RID: 3036
	public class CharacterAmbientLightLerp : FsmStateAction
	{
		// Token: 0x06005D09 RID: 23817 RVA: 0x001D3E72 File Offset: 0x001D2072
		public override void Reset()
		{
			this.Target = null;
			this.EnableAmbientColor = null;
		}

		// Token: 0x06005D0A RID: 23818 RVA: 0x001D3E84 File Offset: 0x001D2084
		public override void Awake()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			tk2dSprite component = safe.GetComponent<tk2dSprite>();
			if (!component)
			{
				return;
			}
			component.EnableKeyword("CAN_LERP_AMBIENT");
		}

		// Token: 0x06005D0B RID: 23819 RVA: 0x001D3EC4 File Offset: 0x001D20C4
		public override void OnEnter()
		{
			tk2dSprite component = this.Target.GetSafe(this).GetComponent<tk2dSprite>();
			component.EnableKeyword("CAN_LERP_AMBIENT");
			if (this.fadeRoutine != null)
			{
				base.StopCoroutine(this.fadeRoutine);
			}
			this.fadeRoutine = base.StartCoroutine(this.FadeRoutine(component, this.EnableAmbientColor.Value));
			base.Finish();
		}

		// Token: 0x06005D0C RID: 23820 RVA: 0x001D3F26 File Offset: 0x001D2126
		private IEnumerator FadeRoutine(tk2dSprite sprite, bool enableAmbient)
		{
			MeshRenderer renderer = sprite.GetComponent<MeshRenderer>();
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			float fromVal = block.GetFloat(CharacterAmbientLightLerp._ambientLerpProp);
			float toVal = enableAmbient ? 1f : 0f;
			if (Math.Abs(fromVal - toVal) > 0.01f)
			{
				for (float elapsed = 0f; elapsed <= 0.1f; elapsed += Time.deltaTime)
				{
					float value = Mathf.Lerp(fromVal, toVal, elapsed / 0.1f);
					renderer.GetPropertyBlock(block);
					block.SetFloat(CharacterAmbientLightLerp._ambientLerpProp, value);
					renderer.SetPropertyBlock(block);
					yield return null;
				}
			}
			renderer.GetPropertyBlock(block);
			block.SetFloat(CharacterAmbientLightLerp._ambientLerpProp, toVal);
			renderer.SetPropertyBlock(block);
			this.fadeRoutine = null;
			yield break;
		}

		// Token: 0x040058B6 RID: 22710
		[RequiredField]
		[CheckForComponent(typeof(tk2dSprite))]
		public FsmOwnerDefault Target;

		// Token: 0x040058B7 RID: 22711
		[RequiredField]
		public FsmBool EnableAmbientColor;

		// Token: 0x040058B8 RID: 22712
		private Coroutine fadeRoutine;

		// Token: 0x040058B9 RID: 22713
		private static readonly int _ambientLerpProp = Shader.PropertyToID("_AmbientLerp");

		// Token: 0x040058BA RID: 22714
		private const string REQUIRED_KEYWORD = "CAN_LERP_AMBIENT";
	}
}
