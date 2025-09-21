using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001060 RID: 4192
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
	public class SendMessageOnExit : FsmStateAction
	{
		// Token: 0x0600729C RID: 29340 RVA: 0x00233F7B File Offset: 0x0023217B
		public override void Reset()
		{
			this.gameObject = null;
			this.delivery = SendMessageOnExit.MessageType.SendMessage;
			this.options = SendMessageOptions.DontRequireReceiver;
			this.functionCall = null;
		}

		// Token: 0x0600729D RID: 29341 RVA: 0x00233F99 File Offset: 0x00232199
		public override void OnExit()
		{
			this.DoSendMessage();
			base.Finish();
		}

		// Token: 0x0600729E RID: 29342 RVA: 0x00233FA8 File Offset: 0x002321A8
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
					if (num <= 398550328U)
					{
						if (num != 382270662U)
						{
							if (num == 398550328U)
							{
								if (parameterType == "string")
								{
									obj = this.functionCall.StringParameter.Value;
								}
							}
						}
						else if (parameterType == "Array")
						{
							obj = this.functionCall.ArrayParameter.Values;
						}
					}
					else if (num != 810547195U)
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
			else if (num <= 3419754368U)
			{
				if (num <= 3289806692U)
				{
					if (num != 2797886853U)
					{
						if (num == 3289806692U)
						{
							if (parameterType == "GameObject")
							{
								obj = this.functionCall.GameObjectParameter.Value;
							}
						}
					}
					else if (parameterType == "float")
					{
						obj = this.functionCall.FloatParameter.Value;
					}
				}
				else if (num != 3365180733U)
				{
					if (num == 3419754368U)
					{
						if (parameterType == "Material")
						{
							obj = this.functionCall.MaterialParameter.Value;
						}
					}
				}
				else if (parameterType == "bool")
				{
					obj = this.functionCall.BoolParameter.Value;
				}
			}
			else if (num <= 3851314394U)
			{
				if (num != 3731074221U)
				{
					if (num == 3851314394U)
					{
						if (parameterType == "Object")
						{
							obj = this.functionCall.ObjectParameter.Value;
						}
					}
				}
				else if (parameterType == "Quaternion")
				{
					obj = this.functionCall.QuaternionParameter.Value;
				}
			}
			else if (num != 3853794552U)
			{
				if (num == 3897416224U)
				{
					if (parameterType == "Enum")
					{
						obj = this.functionCall.EnumParameter.Value;
					}
				}
			}
			else if (parameterType == "Color")
			{
				obj = this.functionCall.ColorParameter.Value;
			}
			switch (this.delivery)
			{
			case SendMessageOnExit.MessageType.SendMessage:
				ownerDefaultTarget.SendMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessageOnExit.MessageType.SendMessageUpwards:
				ownerDefaultTarget.SendMessageUpwards(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessageOnExit.MessageType.BroadcastMessage:
				ownerDefaultTarget.BroadcastMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			default:
				return;
			}
		}

		// Token: 0x0400729B RID: 29339
		[RequiredField]
		[Tooltip("The Game Object to send a message to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400729C RID: 29340
		[Tooltip("Pick between <a href=\"http://unity3d.com/support/documentation/ScriptReference/GameObject.SendMessage.html\" rel=\"nofollow\">SendMessage</a>, <a href=\"http://unity3d.com/support/documentation/ScriptReference/GameObject.SendMessageUpwards.html\" rel=\"nofollow\">SendMessageUpwards</a>, or <a href=\"http://unity3d.com/support/documentation/ScriptReference/GameObject.BroadcastMessage.html\" rel=\"nofollow\">BroadcastMessage</a>.")]
		public SendMessageOnExit.MessageType delivery;

		// Token: 0x0400729D RID: 29341
		[Tooltip("Message delivery options. See <a href=\"http://unity3d.com/support/documentation/ScriptReference/SendMessageOptions.html\" rel=\"nofollow\">SendMessageOptions</a> in Unity Docs.")]
		public SendMessageOptions options;

		// Token: 0x0400729E RID: 29342
		[RequiredField]
		[Tooltip("Select a Method Name first then Parameters.")]
		public FunctionCall functionCall;

		// Token: 0x02001BC5 RID: 7109
		public enum MessageType
		{
			// Token: 0x04009EA2 RID: 40610
			SendMessage,
			// Token: 0x04009EA3 RID: 40611
			SendMessageUpwards,
			// Token: 0x04009EA4 RID: 40612
			BroadcastMessage
		}
	}
}
