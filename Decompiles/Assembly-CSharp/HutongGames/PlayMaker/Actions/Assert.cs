using System;
using PlayMaker.ConditionalExpression;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E74 RID: 3700
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Checks if the conditional expression Is True or Is False. Stops execution of the game if the assertion fails.\nThis is a useful way to check your assumptions. If you expect a certain value use an Assert to make sure!\nOnly runs in Editor.")]
	[SeeAlso("{{Debugging}}")]
	public class Assert : FsmStateAction, IEvaluatorContext
	{
		// Token: 0x17000BF7 RID: 3063
		// (get) Token: 0x06006975 RID: 26997 RVA: 0x00210A97 File Offset: 0x0020EC97
		// (set) Token: 0x06006976 RID: 26998 RVA: 0x00210A9F File Offset: 0x0020EC9F
		public CompiledAst Ast { get; set; }

		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x06006977 RID: 26999 RVA: 0x00210AA8 File Offset: 0x0020ECA8
		// (set) Token: 0x06006978 RID: 27000 RVA: 0x00210AB0 File Offset: 0x0020ECB0
		public string LastErrorMessage { get; set; }

		// Token: 0x06006979 RID: 27001 RVA: 0x00210ABC File Offset: 0x0020ECBC
		FsmVar IEvaluatorContext.GetVariable(string name)
		{
			NamedVariable variable = base.Fsm.Variables.GetVariable(name);
			if (variable != null)
			{
				return new FsmVar(variable);
			}
			throw new VariableNotFoundException(name);
		}

		// Token: 0x040068B3 RID: 26803
		[UIHint(UIHint.TextArea)]
		[Tooltip("Enter an expression to evaluate.\n\nExamples:\nhealth <= maxHealth\nlives < 100\n\nHint:Use $(for variable names with spaces)")]
		public FsmString expression;

		// Token: 0x040068B4 RID: 26804
		[Tooltip("Expected result of the expression.")]
		public Assert.AssertType assert;

		// Token: 0x040068B5 RID: 26805
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x040068B6 RID: 26806
		private string cachedExpression;

		// Token: 0x02001BA4 RID: 7076
		public enum AssertType
		{
			// Token: 0x04009E06 RID: 40454
			IsTrue,
			// Token: 0x04009E07 RID: 40455
			IsFalse
		}
	}
}
