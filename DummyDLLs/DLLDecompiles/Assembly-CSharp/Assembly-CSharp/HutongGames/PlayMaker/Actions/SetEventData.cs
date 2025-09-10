using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010AC RID: 4268
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets Event Data before sending an event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventData : FsmStateAction
	{
		// Token: 0x060073E9 RID: 29673 RVA: 0x00238890 File Offset: 0x00236A90
		public override void Reset()
		{
			this.setGameObjectData = new FsmGameObject
			{
				UseVariable = true
			};
			this.setIntData = new FsmInt
			{
				UseVariable = true
			};
			this.setFloatData = new FsmFloat
			{
				UseVariable = true
			};
			this.setStringData = new FsmString
			{
				UseVariable = true
			};
			this.setBoolData = new FsmBool
			{
				UseVariable = true
			};
			this.setVector2Data = new FsmVector2
			{
				UseVariable = true
			};
			this.setVector3Data = new FsmVector3
			{
				UseVariable = true
			};
			this.setRectData = new FsmRect
			{
				UseVariable = true
			};
			this.setQuaternionData = new FsmQuaternion
			{
				UseVariable = true
			};
			this.setColorData = new FsmColor
			{
				UseVariable = true
			};
			this.setMaterialData = new FsmMaterial
			{
				UseVariable = true
			};
			this.setTextureData = new FsmTexture
			{
				UseVariable = true
			};
			this.setObjectData = new FsmObject
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x060073EA RID: 29674 RVA: 0x0023898E File Offset: 0x00236B8E
		public override void OnEnter()
		{
			this.DoSetData();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060073EB RID: 29675 RVA: 0x002389A4 File Offset: 0x00236BA4
		public override void OnUpdate()
		{
			this.DoSetData();
		}

		// Token: 0x060073EC RID: 29676 RVA: 0x002389AC File Offset: 0x00236BAC
		private void DoSetData()
		{
			Fsm.EventData.BoolData = this.setBoolData.Value;
			Fsm.EventData.IntData = this.setIntData.Value;
			Fsm.EventData.FloatData = this.setFloatData.Value;
			Fsm.EventData.Vector2Data = this.setVector2Data.Value;
			Fsm.EventData.Vector3Data = this.setVector3Data.Value;
			Fsm.EventData.StringData = this.setStringData.Value;
			Fsm.EventData.GameObjectData = this.setGameObjectData.Value;
			Fsm.EventData.RectData = this.setRectData.Value;
			Fsm.EventData.QuaternionData = this.setQuaternionData.Value;
			Fsm.EventData.ColorData = this.setColorData.Value;
			Fsm.EventData.MaterialData = this.setMaterialData.Value;
			Fsm.EventData.TextureData = this.setTextureData.Value;
			Fsm.EventData.ObjectData = this.setObjectData.Value;
		}

		// Token: 0x040073FC RID: 29692
		[Tooltip("Custom Game Object data.")]
		public FsmGameObject setGameObjectData;

		// Token: 0x040073FD RID: 29693
		[Tooltip("Custom Int data.")]
		public FsmInt setIntData;

		// Token: 0x040073FE RID: 29694
		[Tooltip("Custom Float data.")]
		public FsmFloat setFloatData;

		// Token: 0x040073FF RID: 29695
		[Tooltip("Custom String data.")]
		public FsmString setStringData;

		// Token: 0x04007400 RID: 29696
		[Tooltip("Custom Bool data.")]
		public FsmBool setBoolData;

		// Token: 0x04007401 RID: 29697
		[Tooltip("Custom Vector2 data.")]
		public FsmVector2 setVector2Data;

		// Token: 0x04007402 RID: 29698
		[Tooltip("Custom Vector3 data.")]
		public FsmVector3 setVector3Data;

		// Token: 0x04007403 RID: 29699
		[Tooltip("Custom Rect data.")]
		public FsmRect setRectData;

		// Token: 0x04007404 RID: 29700
		[Tooltip("Custom Quaternion data.")]
		public FsmQuaternion setQuaternionData;

		// Token: 0x04007405 RID: 29701
		[Tooltip("Custom Color data.")]
		public FsmColor setColorData;

		// Token: 0x04007406 RID: 29702
		[Tooltip("Custom Material data.")]
		public FsmMaterial setMaterialData;

		// Token: 0x04007407 RID: 29703
		[Tooltip("Custom Texture data.")]
		public FsmTexture setTextureData;

		// Token: 0x04007408 RID: 29704
		[Tooltip("Custom Object data.")]
		public FsmObject setObjectData;

		// Token: 0x04007409 RID: 29705
		public bool everyFrame;
	}
}
