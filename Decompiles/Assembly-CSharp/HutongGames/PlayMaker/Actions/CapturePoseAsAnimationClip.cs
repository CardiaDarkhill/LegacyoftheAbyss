using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCF RID: 3535
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Captures the current pose of a hierarchy as an animation clip.\n\nUseful to blend from an arbitrary pose (e.g. a rag-doll death) back to a known animation (e.g. idle).")]
	public class CapturePoseAsAnimationClip : FsmStateAction
	{
		// Token: 0x06006661 RID: 26209 RVA: 0x00207116 File Offset: 0x00205316
		public override void Reset()
		{
			this.gameObject = null;
			this.position = false;
			this.rotation = true;
			this.scale = false;
			this.storeAnimationClip = null;
		}

		// Token: 0x06006662 RID: 26210 RVA: 0x0020714A File Offset: 0x0020534A
		public override void OnEnter()
		{
			this.DoCaptureAnimationClip();
			base.Finish();
		}

		// Token: 0x06006663 RID: 26211 RVA: 0x00207158 File Offset: 0x00205358
		private void DoCaptureAnimationClip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AnimationClip animationClip = new AnimationClip();
			foreach (object obj in ownerDefaultTarget.transform)
			{
				Transform transform = (Transform)obj;
				this.CaptureTransform(transform, "", animationClip);
			}
			this.storeAnimationClip.Value = animationClip;
		}

		// Token: 0x06006664 RID: 26212 RVA: 0x002071E8 File Offset: 0x002053E8
		private void CaptureTransform(Transform transform, string path, AnimationClip clip)
		{
			path += transform.name;
			if (this.position.Value)
			{
				this.CapturePosition(transform, path, clip);
			}
			if (this.rotation.Value)
			{
				this.CaptureRotation(transform, path, clip);
			}
			if (this.scale.Value)
			{
				this.CaptureScale(transform, path, clip);
			}
			foreach (object obj in transform)
			{
				Transform transform2 = (Transform)obj;
				this.CaptureTransform(transform2, path + "/", clip);
			}
		}

		// Token: 0x06006665 RID: 26213 RVA: 0x00207298 File Offset: 0x00205498
		private void CapturePosition(Transform transform, string path, AnimationClip clip)
		{
			this.SetConstantCurve(clip, path, "localPosition.x", transform.localPosition.x);
			this.SetConstantCurve(clip, path, "localPosition.y", transform.localPosition.y);
			this.SetConstantCurve(clip, path, "localPosition.z", transform.localPosition.z);
		}

		// Token: 0x06006666 RID: 26214 RVA: 0x002072F0 File Offset: 0x002054F0
		private void CaptureRotation(Transform transform, string path, AnimationClip clip)
		{
			this.SetConstantCurve(clip, path, "localRotation.x", transform.localRotation.x);
			this.SetConstantCurve(clip, path, "localRotation.y", transform.localRotation.y);
			this.SetConstantCurve(clip, path, "localRotation.z", transform.localRotation.z);
			this.SetConstantCurve(clip, path, "localRotation.w", transform.localRotation.w);
		}

		// Token: 0x06006667 RID: 26215 RVA: 0x00207360 File Offset: 0x00205560
		private void CaptureScale(Transform transform, string path, AnimationClip clip)
		{
			this.SetConstantCurve(clip, path, "localScale.x", transform.localScale.x);
			this.SetConstantCurve(clip, path, "localScale.y", transform.localScale.y);
			this.SetConstantCurve(clip, path, "localScale.z", transform.localScale.z);
		}

		// Token: 0x06006668 RID: 26216 RVA: 0x002073B8 File Offset: 0x002055B8
		private void SetConstantCurve(AnimationClip clip, string childPath, string propertyPath, float value)
		{
			AnimationCurve animationCurve = AnimationCurve.Linear(0f, value, 100f, value);
			animationCurve.postWrapMode = WrapMode.Loop;
			clip.SetCurve(childPath, typeof(Transform), propertyPath, animationCurve);
		}

		// Token: 0x040065BE RID: 26046
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject root of the hierarchy to capture.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065BF RID: 26047
		[Tooltip("Capture position keys.")]
		public FsmBool position;

		// Token: 0x040065C0 RID: 26048
		[Tooltip("Capture rotation keys.")]
		public FsmBool rotation;

		// Token: 0x040065C1 RID: 26049
		[Tooltip("Capture scale keys.")]
		public FsmBool scale;

		// Token: 0x040065C2 RID: 26050
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(AnimationClip))]
		[Tooltip("Store the result in an Object variable of type AnimationClip.")]
		public FsmObject storeAnimationClip;
	}
}
