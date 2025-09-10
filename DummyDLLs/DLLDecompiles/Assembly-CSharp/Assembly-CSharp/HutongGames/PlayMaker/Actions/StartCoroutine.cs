using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001061 RID: 4193
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Starts a Coroutine in a Behaviour on a Game Object.\nSee Unity <a href=\"http://unity3d.com/support/documentation/ScriptReference/MonoBehaviour.StartCoroutine.html\">StartCoroutine</a> docs for more details.")]
	public class StartCoroutine : FsmStateAction
	{
		// Token: 0x060072A0 RID: 29344 RVA: 0x0023440F File Offset: 0x0023260F
		public override void Reset()
		{
			this.gameObject = null;
			this.behaviour = null;
			this.functionCall = null;
			this.stopOnExit = false;
		}

		// Token: 0x060072A1 RID: 29345 RVA: 0x0023442D File Offset: 0x0023262D
		public override void OnEnter()
		{
			this.DoStartCoroutine();
			base.Finish();
		}

		// Token: 0x060072A2 RID: 29346 RVA: 0x0023443C File Offset: 0x0023263C
		private void DoStartCoroutine()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.component = (ownerDefaultTarget.GetComponent(ReflectionUtils.GetGlobalType(this.behaviour.Value)) as MonoBehaviour);
			if (this.component == null)
			{
				base.LogWarning("StartCoroutine: " + ownerDefaultTarget.name + " missing behaviour: " + this.behaviour.Value);
				return;
			}
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
							if (num != 1796249895U)
							{
								return;
							}
							if (!(parameterType == "Rect"))
							{
								return;
							}
							this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.RectParamater.Value);
							return;
						}
						else
						{
							if (!(parameterType == "None"))
							{
								return;
							}
							this.component.StartCoroutine(this.functionCall.FunctionName);
							return;
						}
					}
					else
					{
						if (!(parameterType == "string"))
						{
							return;
						}
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.StringParameter.Value);
						return;
					}
				}
				else if (num <= 2214621635U)
				{
					if (num != 2197844016U)
					{
						if (num != 2214621635U)
						{
							return;
						}
						if (!(parameterType == "Vector3"))
						{
							return;
						}
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector3Parameter.Value);
						return;
					}
					else
					{
						if (!(parameterType == "Vector2"))
						{
							return;
						}
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.Vector2Parameter.Value);
						return;
					}
				}
				else if (num != 2515107422U)
				{
					if (num != 2571916692U)
					{
						return;
					}
					if (!(parameterType == "Texture"))
					{
						return;
					}
					this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.TextureParameter.Value);
					return;
				}
				else
				{
					if (!(parameterType == "int"))
					{
						return;
					}
					this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.IntParameter.Value);
					return;
				}
			}
			else if (num <= 3365180733U)
			{
				if (num != 2797886853U)
				{
					if (num != 3289806692U)
					{
						if (num != 3365180733U)
						{
							return;
						}
						if (!(parameterType == "bool"))
						{
							return;
						}
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.BoolParameter.Value);
						return;
					}
					else
					{
						if (!(parameterType == "GameObject"))
						{
							return;
						}
						this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.GameObjectParameter.Value);
						return;
					}
				}
				else
				{
					if (!(parameterType == "float"))
					{
						return;
					}
					this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.FloatParameter.Value);
					return;
				}
			}
			else if (num <= 3731074221U)
			{
				if (num != 3419754368U)
				{
					if (num != 3731074221U)
					{
						return;
					}
					if (!(parameterType == "Quaternion"))
					{
						return;
					}
					this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.QuaternionParameter.Value);
					return;
				}
				else
				{
					if (!(parameterType == "Material"))
					{
						return;
					}
					this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.MaterialParameter.Value);
					return;
				}
			}
			else if (num != 3851314394U)
			{
				if (num != 3897416224U)
				{
					return;
				}
				if (!(parameterType == "Enum"))
				{
					return;
				}
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.EnumParameter.Value);
				return;
			}
			else
			{
				if (!(parameterType == "Object"))
				{
					return;
				}
				this.component.StartCoroutine(this.functionCall.FunctionName, this.functionCall.ObjectParameter.Value);
				return;
			}
		}

		// Token: 0x060072A3 RID: 29347 RVA: 0x002348BD File Offset: 0x00232ABD
		public override void OnExit()
		{
			if (this.component == null)
			{
				return;
			}
			if (this.stopOnExit)
			{
				this.component.StopCoroutine(this.functionCall.FunctionName);
			}
		}

		// Token: 0x0400729F RID: 29343
		[RequiredField]
		[Tooltip("The Game Object that owns the Behaviour.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072A0 RID: 29344
		[RequiredField]
		[UIHint(UIHint.Behaviour)]
		[Tooltip("The Behaviour that contains the method to start as a coroutine.")]
		public FsmString behaviour;

		// Token: 0x040072A1 RID: 29345
		[RequiredField]
		[UIHint(UIHint.Coroutine)]
		[Tooltip("The name of the coroutine method.")]
		public FunctionCall functionCall;

		// Token: 0x040072A2 RID: 29346
		[Tooltip("Stop the coroutine when the state is exited.")]
		public bool stopOnExit;

		// Token: 0x040072A3 RID: 29347
		private MonoBehaviour component;
	}
}
