using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010FA RID: 4346
	public static class TweenHelpers
	{
		// Token: 0x0600757F RID: 30079 RVA: 0x0023E48C File Offset: 0x0023C68C
		public static Quaternion GetTargetRotation(RotationOptions option, Transform owner, Transform target, Vector3 rotation)
		{
			if (owner == null)
			{
				return Quaternion.identity;
			}
			switch (option)
			{
			case RotationOptions.CurrentRotation:
				return owner.rotation;
			case RotationOptions.WorldRotation:
				return Quaternion.Euler(rotation);
			case RotationOptions.LocalRotation:
				if (owner.parent == null)
				{
					return Quaternion.Euler(rotation);
				}
				return owner.parent.rotation * Quaternion.Euler(rotation);
			case RotationOptions.WorldOffsetRotation:
				return Quaternion.Euler(rotation) * owner.rotation;
			case RotationOptions.LocalOffsetRotation:
				return owner.rotation * Quaternion.Euler(rotation);
			case RotationOptions.MatchGameObjectRotation:
				if (target == null)
				{
					return owner.rotation;
				}
				return target.rotation * Quaternion.Euler(rotation);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06007580 RID: 30080 RVA: 0x0023E550 File Offset: 0x0023C750
		public static bool GetTargetRotation(RotationOptions option, Transform owner, FsmVector3 rotation, FsmGameObject target, out Quaternion targetRotation)
		{
			targetRotation = Quaternion.identity;
			if (owner == null || !TweenHelpers.CanEditTargetRotation(option, rotation, target))
			{
				return false;
			}
			targetRotation = TweenHelpers.GetTargetRotation(option, owner, (target.Value != null) ? target.Value.transform : null, rotation.Value);
			return true;
		}

		// Token: 0x06007581 RID: 30081 RVA: 0x0023E5B0 File Offset: 0x0023C7B0
		private static bool CanEditTargetRotation(RotationOptions option, NamedVariable rotation, FsmGameObject target)
		{
			if (target == null)
			{
				return false;
			}
			switch (option)
			{
			case RotationOptions.CurrentRotation:
				return false;
			case RotationOptions.WorldRotation:
			case RotationOptions.LocalRotation:
			case RotationOptions.WorldOffsetRotation:
			case RotationOptions.LocalOffsetRotation:
				return !rotation.IsNone;
			case RotationOptions.MatchGameObjectRotation:
				return target.Value != null;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06007582 RID: 30082 RVA: 0x0023E600 File Offset: 0x0023C800
		public static Vector3 GetTargetScale(ScaleOptions option, Transform owner, Transform target, Vector3 scale)
		{
			if (owner == null)
			{
				return Vector3.one;
			}
			switch (option)
			{
			case ScaleOptions.CurrentScale:
				return owner.localScale;
			case ScaleOptions.LocalScale:
				return scale;
			case ScaleOptions.MultiplyScale:
			{
				Vector3 localScale = owner.localScale;
				return new Vector3(localScale.x * scale.x, localScale.y * scale.y, localScale.z * scale.z);
			}
			case ScaleOptions.AddToScale:
			{
				Vector3 localScale2 = owner.localScale;
				return new Vector3(localScale2.x + scale.x, localScale2.y + scale.y, localScale2.z + scale.z);
			}
			case ScaleOptions.MatchGameObject:
				if (target == null)
				{
					return owner.localScale;
				}
				return target.localScale;
			default:
				return owner.localScale;
			}
		}

		// Token: 0x06007583 RID: 30083 RVA: 0x0023E6CC File Offset: 0x0023C8CC
		public static bool GetTargetPosition(PositionOptions option, Transform owner, FsmVector3 position, FsmGameObject target, out Vector3 targetPosition)
		{
			targetPosition = Vector3.zero;
			if (owner == null || !TweenHelpers.IsValidTargetPosition(option, position, target))
			{
				return false;
			}
			targetPosition = TweenHelpers.GetTargetPosition(option, owner, (target != null && target.Value != null) ? target.Value.transform : null, (position != null) ? position.Value : Vector3.zero);
			return true;
		}

		// Token: 0x06007584 RID: 30084 RVA: 0x0023E738 File Offset: 0x0023C938
		private static bool IsValidTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
		{
			switch (option)
			{
			case PositionOptions.CurrentPosition:
				return true;
			case PositionOptions.WorldPosition:
			case PositionOptions.LocalPosition:
			case PositionOptions.WorldOffset:
			case PositionOptions.LocalOffset:
				return !position.IsNone;
			case PositionOptions.TargetGameObject:
				return target.Value != null;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06007585 RID: 30085 RVA: 0x0023E778 File Offset: 0x0023C978
		public static bool CanEditTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
		{
			switch (option)
			{
			case PositionOptions.CurrentPosition:
				return false;
			case PositionOptions.WorldPosition:
			case PositionOptions.LocalPosition:
			case PositionOptions.WorldOffset:
			case PositionOptions.LocalOffset:
				return !position.IsNone;
			case PositionOptions.TargetGameObject:
				return target.Value != null;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06007586 RID: 30086 RVA: 0x0023E7B8 File Offset: 0x0023C9B8
		public static Vector3 GetTargetPosition(PositionOptions option, Transform owner, Transform target, Vector3 position)
		{
			if (owner == null)
			{
				return Vector3.zero;
			}
			switch (option)
			{
			case PositionOptions.CurrentPosition:
				return owner.position;
			case PositionOptions.WorldPosition:
				return position;
			case PositionOptions.LocalPosition:
				if (owner.parent == null)
				{
					return position;
				}
				return owner.parent.TransformPoint(position);
			case PositionOptions.WorldOffset:
				return owner.position + position;
			case PositionOptions.LocalOffset:
				return owner.TransformPoint(position);
			case PositionOptions.TargetGameObject:
				if (target == null)
				{
					return owner.position;
				}
				if (position != Vector3.zero)
				{
					return target.TransformPoint(position);
				}
				return target.position;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06007587 RID: 30087 RVA: 0x0023E860 File Offset: 0x0023CA60
		public static Vector3 GetUiTargetPosition(UiPositionOptions option, RectTransform owner, Transform target, Vector3 position)
		{
			if (owner == null)
			{
				return Vector3.zero;
			}
			switch (option)
			{
			case UiPositionOptions.CurrentPosition:
				return owner.anchoredPosition3D;
			case UiPositionOptions.Position:
				return position;
			case UiPositionOptions.Offset:
				return owner.anchoredPosition3D + position;
			case UiPositionOptions.OffscreenTop:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = TweenHelpers.GetWorldRect(owner);
				anchoredPosition3D.y += (float)Screen.height - worldRect.yMin;
				return anchoredPosition3D;
			}
			case UiPositionOptions.OffscreenBottom:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = TweenHelpers.GetWorldRect(owner);
				anchoredPosition3D.y -= worldRect.yMax;
				return anchoredPosition3D;
			}
			case UiPositionOptions.OffscreenLeft:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = TweenHelpers.GetWorldRect(owner);
				anchoredPosition3D.x -= worldRect.xMax;
				return anchoredPosition3D;
			}
			case UiPositionOptions.OffscreenRight:
			{
				Vector3 anchoredPosition3D = owner.anchoredPosition3D;
				Rect worldRect = TweenHelpers.GetWorldRect(owner);
				anchoredPosition3D.x += (float)Screen.width - worldRect.xMin;
				return anchoredPosition3D;
			}
			case UiPositionOptions.TargetGameObject:
				if (target == null)
				{
					return owner.anchoredPosition3D;
				}
				if (position != Vector3.zero)
				{
					return target.TransformPoint(position);
				}
				return target.position;
			default:
				throw new ArgumentOutOfRangeException("option", option, null);
			}
		}

		// Token: 0x06007588 RID: 30088 RVA: 0x0023E990 File Offset: 0x0023CB90
		public static Rect GetWorldRect(RectTransform rectTransform)
		{
			Vector3[] array = new Vector3[4];
			rectTransform.GetWorldCorners(array);
			float num = Mathf.Max(new float[]
			{
				array[0].y,
				array[1].y,
				array[2].y,
				array[3].y
			});
			float num2 = Mathf.Min(new float[]
			{
				array[0].y,
				array[1].y,
				array[2].y,
				array[3].y
			});
			float num3 = Mathf.Max(new float[]
			{
				array[0].x,
				array[1].x,
				array[2].x,
				array[3].x
			});
			float num4 = Mathf.Min(new float[]
			{
				array[0].x,
				array[1].x,
				array[2].x,
				array[3].x
			});
			return new Rect(num4, num2, num3 - num4, num - num2);
		}
	}
}
