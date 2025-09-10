using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200105F RID: 4191
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
	public class SendMessageDelay : FsmStateAction
	{
		// Token: 0x06007297 RID: 29335 RVA: 0x00233A83 File Offset: 0x00231C83
		public override void Reset()
		{
			this.gameObject = null;
			this.delivery = SendMessageDelay.MessageType.SendMessage;
			this.options = SendMessageOptions.DontRequireReceiver;
			this.functionCall = null;
			this.delay = null;
			this.timer = 0f;
		}

		// Token: 0x06007298 RID: 29336 RVA: 0x00233AB3 File Offset: 0x00231CB3
		public override void OnEnter()
		{
			if (this.delay.Value <= 0f)
			{
				this.DoSendMessage();
				base.Finish();
				return;
			}
			this.timer = this.delay.Value;
		}

		// Token: 0x06007299 RID: 29337 RVA: 0x00233AE5 File Offset: 0x00231CE5
		public override void OnUpdate()
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.DoSendMessage();
			base.Finish();
		}

		// Token: 0x0600729A RID: 29338 RVA: 0x00233B14 File Offset: 0x00231D14
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
			case SendMessageDelay.MessageType.SendMessage:
				ownerDefaultTarget.SendMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessageDelay.MessageType.SendMessageUpwards:
				ownerDefaultTarget.SendMessageUpwards(this.functionCall.FunctionName, obj, this.options);
				return;
			case SendMessageDelay.MessageType.BroadcastMessage:
				ownerDefaultTarget.BroadcastMessage(this.functionCall.FunctionName, obj, this.options);
				return;
			default:
				return;
			}
		}

		// Token: 0x04007295 RID: 29333
		[RequiredField]
		[Tooltip("GameObject that sends the message.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007296 RID: 29334
		[Tooltip("Where to send the message.\nSee Unity docs.")]
		public SendMessageDelay.MessageType delivery;

		// Token: 0x04007297 RID: 29335
		[Tooltip("Send options.\nSee Unity docs.")]
		public SendMessageOptions options;

		// Token: 0x04007298 RID: 29336
		[RequiredField]
		public FunctionCall functionCall;

		// Token: 0x04007299 RID: 29337
		public FsmFloat delay;

		// Token: 0x0400729A RID: 29338
		private float timer;

		// Token: 0x02001BC4 RID: 7108
		public enum MessageType
		{
			// Token: 0x04009E9E RID: 40606
			SendMessage,
			// Token: 0x04009E9F RID: 40607
			SendMessageUpwards,
			// Token: 0x04009EA0 RID: 40608
			BroadcastMessage
		}
	}
}
