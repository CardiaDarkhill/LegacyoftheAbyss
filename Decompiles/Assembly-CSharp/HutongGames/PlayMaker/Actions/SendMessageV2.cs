using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D14 RID: 3348
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
	public class SendMessageV2 : FsmStateAction
	{
		// Token: 0x060062DF RID: 25311 RVA: 0x001F3D55 File Offset: 0x001F1F55
		public override void Reset()
		{
			this.gameObject = null;
			this.delivery = SendMessageV2.MessageType.SendMessage;
			this.options = SendMessageOptions.DontRequireReceiver;
			this.functionCall = null;
		}

		// Token: 0x060062E0 RID: 25312 RVA: 0x001F3D73 File Offset: 0x001F1F73
		public override void OnEnter()
		{
			this.DoSendMessage();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060062E1 RID: 25313 RVA: 0x001F3D89 File Offset: 0x001F1F89
		public override void OnUpdate()
		{
			this.DoSendMessage();
		}

		// Token: 0x060062E2 RID: 25314 RVA: 0x001F3D94 File Offset: 0x001F1F94
		private void DoSendMessage()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			object obj = null;
			string parameterType = this.functionCall.ParameterType;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(parameterType);
			if (num <= 2571916692U)
			{
				if (num <= 1796249895U)
				{
					if (num != 398550328U)
					{
						if (num != 810547195U)
						{
							if (num == 1796249895U)
							{
								if (parameterType == "Rect")
								{
									obj = this.functionCall.RectParamater.Value;
								}
							}
						}
						else if (!(parameterType == "None"))
						{
						}
					}
					else if (parameterType == "string")
					{
						obj = this.functionCall.StringParameter.Value;
					}
				}
				else if (num <= 2214621635U)
				{
					if (num != 2197844016U)
					{
						if (num == 2214621635U)
						{
							if (parameterType == "Vector3")
							{
								obj = this.functionCall.Vector3Parameter.Value;
							}
						}
					}
					else if (parameterType == "Vector2")
					{
						obj = this.functionCall.Vector2Parameter.Value;
					}
				}
				else if (num != 2515107422U)
				{
					if (num == 2571916692U)
					{
						if (parameterType == "Texture")
						{
							obj = this.functionCall.TextureParameter.Value;
						}
					}
				}
				else if (parameterType == "int")
				{
					obj = this.functionCall.IntParameter.Value;
				}
			}
			else if (num <= 3365180733U)
			{
				if (num != 2797886853U)
				{
					if (num != 3289806692U)
					{
						if (num == 3365180733U)
						{
							if (parameterType == "bool")
							{
								obj = this.functionCall.BoolParameter.Value;
							}
						}
					}
					else if (parameterType == "GameObject")
					{
						obj = this.functionCall.GameObjectParameter.Value;
					}
				}
				else if (parameterType == "float")
				{
					obj = this.functionCall.FloatParameter.Value;
				}
			}
			else if (num <= 3731074221U)
			{
				if (num != 3419754368U)
				{
					if (num == 3731074221U)
					{
						if (parameterType == "Quaternion")
						{
							obj = this.functionCall.QuaternionParameter.Value;
						}
					}
				}
				else if (parameterType == "Material")
				{
					obj = this.functionCall.MaterialParameter.Value;
				}
			}
			else if (num != 3851314394U)
			{
				if (num == 3853794552U)
				{
					if (parameterType == "Color")
					{
						obj = this.functionCall.ColorParameter.Value;
					}
				}
			}
			else if (parameterType == "Object")
			{
				obj = this.functionCall.ObjectParameter.Value;
			}
			switch (this.delivery)
			{
			case SendMessageV2.MessageType.SendMessage:
				ownerDefaultTarget.SendMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessageV2.MessageType.SendMessageUpwards:
				ownerDefaultTarget.SendMessageUpwards(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessageV2.MessageType.BroadcastMessage:
				ownerDefaultTarget.BroadcastMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			default:
				return;
			}
		}

		// Token: 0x04006145 RID: 24901
		[RequiredField]
		[Tooltip("GameObject that sends the message.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006146 RID: 24902
		[Tooltip("Where to send the message.\nSee Unity docs.")]
		public SendMessageV2.MessageType delivery;

		// Token: 0x04006147 RID: 24903
		[Tooltip("Send options.\nSee Unity docs.")]
		public SendMessageOptions options;

		// Token: 0x04006148 RID: 24904
		[RequiredField]
		public FunctionCall functionCall;

		// Token: 0x04006149 RID: 24905
		public bool everyFrame;

		// Token: 0x02001B8B RID: 7051
		public enum MessageType
		{
			// Token: 0x04009D88 RID: 40328
			SendMessage,
			// Token: 0x04009D89 RID: 40329
			SendMessageUpwards,
			// Token: 0x04009D8A RID: 40330
			BroadcastMessage
		}
	}
}
