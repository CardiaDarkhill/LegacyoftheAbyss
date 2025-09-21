using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000928 RID: 2344
	public abstract class TouchControl : MonoBehaviour
	{
		// Token: 0x0600530E RID: 21262
		public abstract void CreateControl();

		// Token: 0x0600530F RID: 21263
		public abstract void DestroyControl();

		// Token: 0x06005310 RID: 21264
		public abstract void ConfigureControl();

		// Token: 0x06005311 RID: 21265
		public abstract void SubmitControlState(ulong updateTick, float deltaTime);

		// Token: 0x06005312 RID: 21266
		public abstract void CommitControlState(ulong updateTick, float deltaTime);

		// Token: 0x06005313 RID: 21267
		public abstract void TouchBegan(Touch touch);

		// Token: 0x06005314 RID: 21268
		public abstract void TouchMoved(Touch touch);

		// Token: 0x06005315 RID: 21269
		public abstract void TouchEnded(Touch touch);

		// Token: 0x06005316 RID: 21270
		public abstract void DrawGizmos();

		// Token: 0x06005317 RID: 21271 RVA: 0x0017C4F9 File Offset: 0x0017A6F9
		private void OnEnable()
		{
			TouchManager.OnSetup += this.Setup;
		}

		// Token: 0x06005318 RID: 21272 RVA: 0x0017C50C File Offset: 0x0017A70C
		private void OnDisable()
		{
			this.DestroyControl();
			Resources.UnloadUnusedAssets();
		}

		// Token: 0x06005319 RID: 21273 RVA: 0x0017C51A File Offset: 0x0017A71A
		private void Setup()
		{
			if (!base.enabled)
			{
				return;
			}
			this.CreateControl();
			this.ConfigureControl();
		}

		// Token: 0x0600531A RID: 21274 RVA: 0x0017C534 File Offset: 0x0017A734
		protected Vector3 OffsetToWorldPosition(TouchControlAnchor anchor, Vector2 offset, TouchUnitType offsetUnitType, bool lockAspectRatio)
		{
			Vector3 b;
			if (offsetUnitType == TouchUnitType.Pixels)
			{
				b = TouchUtility.RoundVector(offset) * TouchManager.PixelToWorld;
			}
			else if (lockAspectRatio)
			{
				b = offset * TouchManager.PercentToWorld;
			}
			else
			{
				b = Vector3.Scale(offset, TouchManager.ViewSize);
			}
			return TouchManager.ViewToWorldPoint(TouchUtility.AnchorToViewPoint(anchor)) + b;
		}

		// Token: 0x0600531B RID: 21275 RVA: 0x0017C598 File Offset: 0x0017A798
		protected void SubmitButtonState(TouchControl.ButtonTarget target, bool state, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device == null || target == TouchControl.ButtonTarget.None)
			{
				return;
			}
			InputControl control = TouchManager.Device.GetControl((InputControlType)target);
			if (control != null && control != InputControl.Null)
			{
				control.UpdateWithState(state, updateTick, deltaTime);
			}
		}

		// Token: 0x0600531C RID: 21276 RVA: 0x0017C5D4 File Offset: 0x0017A7D4
		protected void SubmitButtonValue(TouchControl.ButtonTarget target, float value, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device == null || target == TouchControl.ButtonTarget.None)
			{
				return;
			}
			InputControl control = TouchManager.Device.GetControl((InputControlType)target);
			if (control != null && control != InputControl.Null)
			{
				control.UpdateWithValue(value, updateTick, deltaTime);
			}
		}

		// Token: 0x0600531D RID: 21277 RVA: 0x0017C610 File Offset: 0x0017A810
		protected void CommitButton(TouchControl.ButtonTarget target)
		{
			if (TouchManager.Device == null || target == TouchControl.ButtonTarget.None)
			{
				return;
			}
			InputControl control = TouchManager.Device.GetControl((InputControlType)target);
			if (control != null && control != InputControl.Null)
			{
				control.Commit();
			}
		}

		// Token: 0x0600531E RID: 21278 RVA: 0x0017C648 File Offset: 0x0017A848
		protected void SubmitAnalogValue(TouchControl.AnalogTarget target, Vector2 value, float lowerDeadZone, float upperDeadZone, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device == null || target == TouchControl.AnalogTarget.None)
			{
				return;
			}
			Vector2 value2 = DeadZone.Circular(value.x, value.y, lowerDeadZone, upperDeadZone);
			if (target == TouchControl.AnalogTarget.LeftStick || target == TouchControl.AnalogTarget.Both)
			{
				TouchManager.Device.UpdateLeftStickWithValue(value2, updateTick, deltaTime);
			}
			if (target == TouchControl.AnalogTarget.RightStick || target == TouchControl.AnalogTarget.Both)
			{
				TouchManager.Device.UpdateRightStickWithValue(value2, updateTick, deltaTime);
			}
		}

		// Token: 0x0600531F RID: 21279 RVA: 0x0017C6A3 File Offset: 0x0017A8A3
		protected void CommitAnalog(TouchControl.AnalogTarget target)
		{
			if (TouchManager.Device == null || target == TouchControl.AnalogTarget.None)
			{
				return;
			}
			if (target == TouchControl.AnalogTarget.LeftStick || target == TouchControl.AnalogTarget.Both)
			{
				TouchManager.Device.CommitLeftStick();
			}
			if (target == TouchControl.AnalogTarget.RightStick || target == TouchControl.AnalogTarget.Both)
			{
				TouchManager.Device.CommitRightStick();
			}
		}

		// Token: 0x06005320 RID: 21280 RVA: 0x0017C6D4 File Offset: 0x0017A8D4
		protected void SubmitRawAnalogValue(TouchControl.AnalogTarget target, Vector2 rawValue, ulong updateTick, float deltaTime)
		{
			if (TouchManager.Device == null || target == TouchControl.AnalogTarget.None)
			{
				return;
			}
			if (target == TouchControl.AnalogTarget.LeftStick || target == TouchControl.AnalogTarget.Both)
			{
				TouchManager.Device.UpdateLeftStickWithRawValue(rawValue, updateTick, deltaTime);
			}
			if (target == TouchControl.AnalogTarget.RightStick || target == TouchControl.AnalogTarget.Both)
			{
				TouchManager.Device.UpdateRightStickWithRawValue(rawValue, updateTick, deltaTime);
			}
		}

		// Token: 0x06005321 RID: 21281 RVA: 0x0017C710 File Offset: 0x0017A910
		protected static Vector3 SnapTo(Vector2 vector, TouchControl.SnapAngles snapAngles)
		{
			if (snapAngles == TouchControl.SnapAngles.None)
			{
				return vector;
			}
			float snapAngle = 360f / (float)snapAngles;
			return TouchControl.SnapTo(vector, snapAngle);
		}

		// Token: 0x06005322 RID: 21282 RVA: 0x0017C738 File Offset: 0x0017A938
		protected static Vector3 SnapTo(Vector2 vector, float snapAngle)
		{
			float num = Vector2.Angle(vector, Vector2.up);
			if (num < snapAngle / 2f)
			{
				return Vector2.up * vector.magnitude;
			}
			if (num > 180f - snapAngle / 2f)
			{
				return -Vector2.up * vector.magnitude;
			}
			float angle = Mathf.Round(num / snapAngle) * snapAngle - num;
			Vector3 axis = Vector3.Cross(Vector2.up, vector);
			return Quaternion.AngleAxis(angle, axis) * vector;
		}

		// Token: 0x06005323 RID: 21283 RVA: 0x0017C7D1 File Offset: 0x0017A9D1
		private void OnDrawGizmosSelected()
		{
			if (!base.enabled)
			{
				return;
			}
			if (TouchManager.ControlsShowGizmos != TouchManager.GizmoShowOption.WhenSelected)
			{
				return;
			}
			if (Utility.GameObjectIsCulledOnCurrentCamera(base.gameObject))
			{
				return;
			}
			if (!Application.isPlaying)
			{
				this.ConfigureControl();
			}
			this.DrawGizmos();
		}

		// Token: 0x06005324 RID: 21284 RVA: 0x0017C808 File Offset: 0x0017AA08
		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			if (TouchManager.ControlsShowGizmos == TouchManager.GizmoShowOption.UnlessPlaying)
			{
				if (Application.isPlaying)
				{
					return;
				}
			}
			else if (TouchManager.ControlsShowGizmos != TouchManager.GizmoShowOption.Always)
			{
				return;
			}
			if (Utility.GameObjectIsCulledOnCurrentCamera(base.gameObject))
			{
				return;
			}
			if (!Application.isPlaying)
			{
				this.ConfigureControl();
			}
			this.DrawGizmos();
		}

		// Token: 0x02001B64 RID: 7012
		public enum ButtonTarget
		{
			// Token: 0x04009C92 RID: 40082
			None,
			// Token: 0x04009C93 RID: 40083
			DPadDown = 12,
			// Token: 0x04009C94 RID: 40084
			DPadLeft,
			// Token: 0x04009C95 RID: 40085
			DPadRight,
			// Token: 0x04009C96 RID: 40086
			DPadUp = 11,
			// Token: 0x04009C97 RID: 40087
			LeftTrigger = 15,
			// Token: 0x04009C98 RID: 40088
			RightTrigger,
			// Token: 0x04009C99 RID: 40089
			LeftBumper,
			// Token: 0x04009C9A RID: 40090
			RightBumper,
			// Token: 0x04009C9B RID: 40091
			Action1,
			// Token: 0x04009C9C RID: 40092
			Action2,
			// Token: 0x04009C9D RID: 40093
			Action3,
			// Token: 0x04009C9E RID: 40094
			Action4,
			// Token: 0x04009C9F RID: 40095
			Action5,
			// Token: 0x04009CA0 RID: 40096
			Action6,
			// Token: 0x04009CA1 RID: 40097
			Action7,
			// Token: 0x04009CA2 RID: 40098
			Action8,
			// Token: 0x04009CA3 RID: 40099
			Action9,
			// Token: 0x04009CA4 RID: 40100
			Action10,
			// Token: 0x04009CA5 RID: 40101
			Action11,
			// Token: 0x04009CA6 RID: 40102
			Action12,
			// Token: 0x04009CA7 RID: 40103
			Menu = 106,
			// Token: 0x04009CA8 RID: 40104
			Button0 = 500,
			// Token: 0x04009CA9 RID: 40105
			Button1,
			// Token: 0x04009CAA RID: 40106
			Button2,
			// Token: 0x04009CAB RID: 40107
			Button3,
			// Token: 0x04009CAC RID: 40108
			Button4,
			// Token: 0x04009CAD RID: 40109
			Button5,
			// Token: 0x04009CAE RID: 40110
			Button6,
			// Token: 0x04009CAF RID: 40111
			Button7,
			// Token: 0x04009CB0 RID: 40112
			Button8,
			// Token: 0x04009CB1 RID: 40113
			Button9,
			// Token: 0x04009CB2 RID: 40114
			Button10,
			// Token: 0x04009CB3 RID: 40115
			Button11,
			// Token: 0x04009CB4 RID: 40116
			Button12,
			// Token: 0x04009CB5 RID: 40117
			Button13,
			// Token: 0x04009CB6 RID: 40118
			Button14,
			// Token: 0x04009CB7 RID: 40119
			Button15,
			// Token: 0x04009CB8 RID: 40120
			Button16,
			// Token: 0x04009CB9 RID: 40121
			Button17,
			// Token: 0x04009CBA RID: 40122
			Button18,
			// Token: 0x04009CBB RID: 40123
			Button19
		}

		// Token: 0x02001B65 RID: 7013
		public enum AnalogTarget
		{
			// Token: 0x04009CBD RID: 40125
			None,
			// Token: 0x04009CBE RID: 40126
			LeftStick,
			// Token: 0x04009CBF RID: 40127
			RightStick,
			// Token: 0x04009CC0 RID: 40128
			Both
		}

		// Token: 0x02001B66 RID: 7014
		public enum SnapAngles
		{
			// Token: 0x04009CC2 RID: 40130
			None,
			// Token: 0x04009CC3 RID: 40131
			Four = 4,
			// Token: 0x04009CC4 RID: 40132
			Eight = 8,
			// Token: 0x04009CC5 RID: 40133
			Sixteen = 16
		}
	}
}
