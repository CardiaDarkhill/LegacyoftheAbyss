using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001084 RID: 4228
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets info on the last event that caused a state change. See also: {{Set Event Data}} action.")]
	[SeeAlso("{{SetEventData}}")]
	public class GetEventInfo : FsmStateAction
	{
		// Token: 0x06007329 RID: 29481 RVA: 0x00236274 File Offset: 0x00234474
		public override void Reset()
		{
			this.sentByGameObject = null;
			this.fsmName = null;
			this.getBoolData = null;
			this.getIntData = null;
			this.getFloatData = null;
			this.getVector2Data = null;
			this.getVector3Data = null;
			this.getStringData = null;
			this.getGameObjectData = null;
			this.getRectData = null;
			this.getQuaternionData = null;
			this.getMaterialData = null;
			this.getTextureData = null;
			this.getColorData = null;
			this.getObjectData = null;
		}

		// Token: 0x0600732A RID: 29482 RVA: 0x002362EC File Offset: 0x002344EC
		public override void OnEnter()
		{
			if (Fsm.EventData.SentByGameObject != null)
			{
				this.sentByGameObject.Value = Fsm.EventData.SentByGameObject;
			}
			else if (Fsm.EventData.SentByFsm != null)
			{
				this.sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
				this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
			}
			else
			{
				this.sentByGameObject.Value = null;
				this.fsmName.Value = "";
			}
			this.getBoolData.Value = Fsm.EventData.BoolData;
			this.getIntData.Value = Fsm.EventData.IntData;
			this.getFloatData.Value = Fsm.EventData.FloatData;
			this.getVector2Data.Value = Fsm.EventData.Vector2Data;
			this.getVector3Data.Value = Fsm.EventData.Vector3Data;
			this.getStringData.Value = Fsm.EventData.StringData;
			this.getGameObjectData.Value = Fsm.EventData.GameObjectData;
			this.getRectData.Value = Fsm.EventData.RectData;
			this.getQuaternionData.Value = Fsm.EventData.QuaternionData;
			this.getMaterialData.Value = Fsm.EventData.MaterialData;
			this.getTextureData.Value = Fsm.EventData.TextureData;
			this.getColorData.Value = Fsm.EventData.ColorData;
			this.getObjectData.Value = Fsm.EventData.ObjectData;
			base.Finish();
		}

		// Token: 0x0400732A RID: 29482
		[UIHint(UIHint.Variable)]
		[Tooltip("The Game Object that sent the Event.")]
		public FsmGameObject sentByGameObject;

		// Token: 0x0400732B RID: 29483
		[UIHint(UIHint.Variable)]
		[Tooltip("The name of the FSM that sent the Event.")]
		public FsmString fsmName;

		// Token: 0x0400732C RID: 29484
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Bool data.")]
		public FsmBool getBoolData;

		// Token: 0x0400732D RID: 29485
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Int data.")]
		public FsmInt getIntData;

		// Token: 0x0400732E RID: 29486
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Float data.")]
		public FsmFloat getFloatData;

		// Token: 0x0400732F RID: 29487
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Vector2 data.")]
		public FsmVector2 getVector2Data;

		// Token: 0x04007330 RID: 29488
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Vector3 data.")]
		public FsmVector3 getVector3Data;

		// Token: 0x04007331 RID: 29489
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom String data.")]
		public FsmString getStringData;

		// Token: 0x04007332 RID: 29490
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom GameObject data.")]
		public FsmGameObject getGameObjectData;

		// Token: 0x04007333 RID: 29491
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Rect data.")]
		public FsmRect getRectData;

		// Token: 0x04007334 RID: 29492
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Quaternion data.")]
		public FsmQuaternion getQuaternionData;

		// Token: 0x04007335 RID: 29493
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Material data.")]
		public FsmMaterial getMaterialData;

		// Token: 0x04007336 RID: 29494
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Texture data.")]
		public FsmTexture getTextureData;

		// Token: 0x04007337 RID: 29495
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Color data.")]
		public FsmColor getColorData;

		// Token: 0x04007338 RID: 29496
		[UIHint(UIHint.Variable)]
		[Tooltip("Custom Object data.")]
		public FsmObject getObjectData;
	}
}
