using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mathos.Parser;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F90 RID: 3984
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Math expression action. Enter the expression using variable names and common math syntax. Uses Mathos parser.")]
	public class MathExpression : FsmStateAction
	{
		// Token: 0x06006E20 RID: 28192 RVA: 0x002223E9 File Offset: 0x002205E9
		public override void Awake()
		{
			this.parser = new MathParser(true, true, true, null);
			this.parser.LocalVariables["Time.deltaTime"] = 0.0;
		}

		// Token: 0x06006E21 RID: 28193 RVA: 0x00222418 File Offset: 0x00220618
		public override void OnEnter()
		{
			this.DoMathExpression();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E22 RID: 28194 RVA: 0x0022242E File Offset: 0x0022062E
		public override void OnUpdate()
		{
			this.parser.LocalVariables["Time.deltaTime"] = (double)Time.deltaTime;
			this.DoMathExpression();
		}

		// Token: 0x06006E23 RID: 28195 RVA: 0x00222454 File Offset: 0x00220654
		private void DoMathExpression()
		{
			double num = this.ParseExpression();
			if (!this.storeResultAsFloat.IsNone)
			{
				this.storeResultAsFloat.Value = (float)num;
			}
			if (!this.storeResultAsInt.IsNone)
			{
				this.storeResultAsInt.Value = Mathf.FloorToInt((float)num);
			}
		}

		// Token: 0x06006E24 RID: 28196 RVA: 0x002224A4 File Offset: 0x002206A4
		public double ParseExpression()
		{
			if (this.expression.Value != this.cachedExpression)
			{
				this.BuildAndCacheExpression();
			}
			for (int i = 0; i < this.usedVariables.Count; i++)
			{
				NamedVariable namedVariable = this.usedVariables[i];
				switch (namedVariable.VariableType)
				{
				case VariableType.Float:
					this.parser.LocalVariables[namedVariable.Name] = (double)((FsmFloat)namedVariable).Value;
					break;
				case VariableType.Int:
					this.parser.LocalVariables[namedVariable.Name] = (double)((FsmInt)namedVariable).Value;
					break;
				case VariableType.Bool:
					this.parser.LocalVariables[namedVariable.Name] = (double)(((FsmBool)namedVariable).Value ? 1 : 0);
					break;
				}
			}
			return this.parser.Parse(this.tokens);
		}

		// Token: 0x06006E25 RID: 28197 RVA: 0x00222594 File Offset: 0x00220794
		private void BuildAndCacheExpression()
		{
			if (this.parser == null)
			{
				this.parser = new MathParser(true, true, true, null);
			}
			this.tokens = this.parser.GetTokens(this.expression.Value);
			foreach (NamedVariable namedVariable in this.usedVariables)
			{
				this.parser.LocalVariables.Remove(namedVariable.Name);
			}
			this.usedVariables.Clear();
			foreach (string name in this.tokens)
			{
				NamedVariable namedVariable2 = base.Fsm.Variables.FindVariable(name) ?? FsmVariables.GlobalVariables.FindVariable(name);
				if (namedVariable2 != null && !this.usedVariables.Contains(namedVariable2))
				{
					this.usedVariables.Add(namedVariable2);
				}
			}
			this.cachedExpression = this.expression.Value;
		}

		// Token: 0x04006DD1 RID: 28113
		[UIHint(UIHint.TextArea)]
		[Tooltip("Expression to evaluate. Accepts float, int, and bool variable names. Also: Time.deltaTime, ")]
		public FsmString expression;

		// Token: 0x04006DD2 RID: 28114
		[Tooltip("Store the result in a float variable")]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResultAsFloat;

		// Token: 0x04006DD3 RID: 28115
		[Tooltip("Store the result in an int variable")]
		[UIHint(UIHint.Variable)]
		public FsmInt storeResultAsInt;

		// Token: 0x04006DD4 RID: 28116
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04006DD5 RID: 28117
		private MathParser parser;

		// Token: 0x04006DD6 RID: 28118
		private string cachedExpression;

		// Token: 0x04006DD7 RID: 28119
		private ReadOnlyCollection<string> tokens;

		// Token: 0x04006DD8 RID: 28120
		private readonly List<NamedVariable> usedVariables = new List<NamedVariable>();

		// Token: 0x02001BB2 RID: 7090
		public class Property
		{
			// Token: 0x04009E49 RID: 40521
			public string path;
		}
	}
}
