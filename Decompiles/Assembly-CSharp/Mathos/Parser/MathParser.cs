using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mathos.Parser
{
	// Token: 0x020008CF RID: 2255
	public class MathParser
	{
		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x06004ED4 RID: 20180 RVA: 0x0016E04B File Offset: 0x0016C24B
		// (set) Token: 0x06004ED5 RID: 20181 RVA: 0x0016E053 File Offset: 0x0016C253
		public List<string> OperatorList { get; set; }

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06004ED6 RID: 20182 RVA: 0x0016E05C File Offset: 0x0016C25C
		// (set) Token: 0x06004ED7 RID: 20183 RVA: 0x0016E064 File Offset: 0x0016C264
		public Dictionary<string, Func<double, double, double>> OperatorAction { get; set; }

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06004ED8 RID: 20184 RVA: 0x0016E06D File Offset: 0x0016C26D
		// (set) Token: 0x06004ED9 RID: 20185 RVA: 0x0016E075 File Offset: 0x0016C275
		public Dictionary<string, Func<double[], double>> LocalFunctions { get; set; }

		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06004EDA RID: 20186 RVA: 0x0016E07E File Offset: 0x0016C27E
		// (set) Token: 0x06004EDB RID: 20187 RVA: 0x0016E086 File Offset: 0x0016C286
		public Dictionary<string, double> LocalVariables { get; set; }

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06004EDC RID: 20188 RVA: 0x0016E08F File Offset: 0x0016C28F
		// (set) Token: 0x06004EDD RID: 20189 RVA: 0x0016E097 File Offset: 0x0016C297
		public CultureInfo CultureInfo { get; set; }

		// Token: 0x06004EDE RID: 20190 RVA: 0x0016E0A0 File Offset: 0x0016C2A0
		public MathParser(bool loadPreDefinedFunctions = true, bool loadPreDefinedOperators = true, bool loadPreDefinedVariables = true, CultureInfo cultureInfo = null)
		{
			if (loadPreDefinedOperators)
			{
				this.OperatorList = new List<string>(10)
				{
					"^",
					"%",
					":",
					"/",
					"*",
					"-",
					"+",
					">",
					"<",
					"="
				};
				this.OperatorAction = new Dictionary<string, Func<double, double, double>>(10);
				this.OperatorAction["^"] = new Func<double, double, double>(Math.Pow);
				this.OperatorAction["%"] = ((double a, double b) => a % b);
				this.OperatorAction[":"] = ((double a, double b) => a / b);
				this.OperatorAction["/"] = ((double a, double b) => a / b);
				this.OperatorAction["*"] = ((double a, double b) => a * b);
				this.OperatorAction["-"] = ((double a, double b) => a - b);
				this.OperatorAction["+"] = ((double a, double b) => a + b);
				this.OperatorAction[">"] = ((double a, double b) => (double)((a > b) ? 1 : 0));
				this.OperatorAction["<"] = ((double a, double b) => (double)((a < b) ? 1 : 0));
				this.OperatorAction["="] = ((double a, double b) => (double)((Math.Abs(a - b) < 1E-08) ? 1 : 0));
			}
			else
			{
				this.OperatorList = new List<string>();
				this.OperatorAction = new Dictionary<string, Func<double, double, double>>();
			}
			if (loadPreDefinedFunctions)
			{
				this.LocalFunctions = new Dictionary<string, Func<double[], double>>(26);
				this.LocalFunctions["abs"] = ((double[] inputs) => Math.Abs(inputs[0]));
				this.LocalFunctions["cos"] = ((double[] inputs) => Math.Cos(inputs[0]));
				this.LocalFunctions["cosh"] = ((double[] inputs) => Math.Cosh(inputs[0]));
				this.LocalFunctions["acos"] = ((double[] inputs) => Math.Acos(inputs[0]));
				this.LocalFunctions["arccos"] = ((double[] inputs) => Math.Acos(inputs[0]));
				this.LocalFunctions["sin"] = ((double[] inputs) => Math.Sin(inputs[0]));
				this.LocalFunctions["sinh"] = ((double[] inputs) => Math.Sinh(inputs[0]));
				this.LocalFunctions["asin"] = ((double[] inputs) => Math.Asin(inputs[0]));
				this.LocalFunctions["arcsin"] = ((double[] inputs) => Math.Asin(inputs[0]));
				this.LocalFunctions["tan"] = ((double[] inputs) => Math.Tan(inputs[0]));
				this.LocalFunctions["tanh"] = ((double[] inputs) => Math.Tanh(inputs[0]));
				this.LocalFunctions["atan"] = ((double[] inputs) => Math.Atan(inputs[0]));
				this.LocalFunctions["arctan"] = ((double[] inputs) => Math.Atan(inputs[0]));
				this.LocalFunctions["sqrt"] = ((double[] inputs) => Math.Sqrt(inputs[0]));
				this.LocalFunctions["pow"] = ((double[] inputs) => Math.Pow(inputs[0], inputs[1]));
				this.LocalFunctions["root"] = ((double[] inputs) => Math.Pow(inputs[0], 1.0 / inputs[1]));
				this.LocalFunctions["rem"] = ((double[] inputs) => Math.IEEERemainder(inputs[0], inputs[1]));
				this.LocalFunctions["sign"] = ((double[] inputs) => (double)Math.Sign(inputs[0]));
				this.LocalFunctions["exp"] = ((double[] inputs) => Math.Exp(inputs[0]));
				this.LocalFunctions["floor"] = ((double[] inputs) => Math.Floor(inputs[0]));
				this.LocalFunctions["ceil"] = ((double[] inputs) => Math.Ceiling(inputs[0]));
				this.LocalFunctions["ceiling"] = ((double[] inputs) => Math.Ceiling(inputs[0]));
				this.LocalFunctions["round"] = ((double[] inputs) => Math.Round(inputs[0]));
				this.LocalFunctions["truncate"] = delegate(double[] inputs)
				{
					if (inputs[0] >= 0.0)
					{
						return Math.Floor(inputs[0]);
					}
					return -Math.Floor(-inputs[0]);
				};
				this.LocalFunctions["log"] = delegate(double[] inputs)
				{
					int num = inputs.Length;
					if (num == 1)
					{
						return Math.Log10(inputs[0]);
					}
					if (num != 2)
					{
						return 0.0;
					}
					return Math.Log(inputs[0], inputs[1]);
				};
				this.LocalFunctions["ln"] = ((double[] inputs) => Math.Log(inputs[0]));
			}
			else
			{
				this.LocalFunctions = new Dictionary<string, Func<double[], double>>();
			}
			if (loadPreDefinedVariables)
			{
				this.LocalVariables = new Dictionary<string, double>(8);
				this.LocalVariables["pi"] = 3.14159265358979;
				this.LocalVariables["tao"] = 6.28318530717959;
				this.LocalVariables["e"] = 2.71828182845905;
				this.LocalVariables["phi"] = 1.61803398874989;
				this.LocalVariables["major"] = 0.61803398874989;
				this.LocalVariables["minor"] = 0.38196601125011;
				this.LocalVariables["pitograd"] = 57.2957795130823;
				this.LocalVariables["piofgrad"] = 0.01745329251994;
			}
			else
			{
				this.LocalVariables = new Dictionary<string, double>();
			}
			this.CultureInfo = (cultureInfo ?? CultureInfo.InvariantCulture);
		}

		// Token: 0x06004EDF RID: 20191 RVA: 0x0016E910 File Offset: 0x0016CB10
		public double Parse(string mathExpression)
		{
			return this.MathParserLogic(this.Lexer(mathExpression));
		}

		// Token: 0x06004EE0 RID: 20192 RVA: 0x0016E91F File Offset: 0x0016CB1F
		public double Parse(ReadOnlyCollection<string> mathExpression)
		{
			return this.MathParserLogic(new List<string>(mathExpression));
		}

		// Token: 0x06004EE1 RID: 20193 RVA: 0x0016E930 File Offset: 0x0016CB30
		public double ProgrammaticallyParse(string mathExpression, bool correctExpression = true, bool identifyComments = true)
		{
			if (identifyComments)
			{
				mathExpression = Regex.Replace(mathExpression, "#\\{.*?\\}#", "");
				mathExpression = Regex.Replace(mathExpression, "#.*$", "");
			}
			if (correctExpression)
			{
				mathExpression = this.Correction(mathExpression);
			}
			string text;
			double num;
			if (mathExpression.Contains("let"))
			{
				if (mathExpression.Contains("be"))
				{
					text = mathExpression.Substring(mathExpression.IndexOf("let", StringComparison.Ordinal) + 3, mathExpression.IndexOf("be", StringComparison.Ordinal) - mathExpression.IndexOf("let", StringComparison.Ordinal) - 3);
					mathExpression = mathExpression.Replace(text + "be", "");
				}
				else
				{
					text = mathExpression.Substring(mathExpression.IndexOf("let", StringComparison.Ordinal) + 3, mathExpression.IndexOf("=", StringComparison.Ordinal) - mathExpression.IndexOf("let", StringComparison.Ordinal) - 3);
					mathExpression = mathExpression.Replace(text + "=", "");
				}
				text = text.Replace(" ", "");
				mathExpression = mathExpression.Replace("let", "");
				num = this.Parse(mathExpression);
				if (this.LocalVariables.ContainsKey(text))
				{
					this.LocalVariables[text] = num;
				}
				else
				{
					this.LocalVariables.Add(text, num);
				}
				return num;
			}
			if (!mathExpression.Contains(":="))
			{
				return this.Parse(mathExpression);
			}
			text = mathExpression.Substring(0, mathExpression.IndexOf(":=", StringComparison.Ordinal));
			mathExpression = mathExpression.Replace(text + ":=", "");
			num = this.Parse(mathExpression);
			text = text.Replace(" ", "");
			if (this.LocalVariables.ContainsKey(text))
			{
				this.LocalVariables[text] = num;
			}
			else
			{
				this.LocalVariables.Add(text, num);
			}
			return num;
		}

		// Token: 0x06004EE2 RID: 20194 RVA: 0x0016EAFB File Offset: 0x0016CCFB
		public ReadOnlyCollection<string> GetTokens(string mathExpression)
		{
			return this.Lexer(mathExpression).AsReadOnly();
		}

		// Token: 0x06004EE3 RID: 20195 RVA: 0x0016EB09 File Offset: 0x0016CD09
		private string Correction(string input)
		{
			input = Regex.Replace(input, "\\b(sqr|sqrt)\\b", "sqrt", RegexOptions.IgnoreCase);
			input = Regex.Replace(input, "\\b(atan2|arctan2)\\b", "arctan2", RegexOptions.IgnoreCase);
			return input;
		}

		// Token: 0x06004EE4 RID: 20196 RVA: 0x0016EB34 File Offset: 0x0016CD34
		private List<string> Lexer(string expr)
		{
			string text = "";
			List<string> list = new List<string>();
			expr = expr.Replace("+-", "-");
			expr = expr.Replace("-+", "-");
			expr = expr.Replace("--", "+");
			for (int i = 0; i < expr.Length; i++)
			{
				char c = expr[i];
				if (!char.IsWhiteSpace(c))
				{
					if (char.IsLetter(c))
					{
						if (i != 0 && (char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
						{
							list.Add("*");
						}
						text += c.ToString();
						while (i + 1 < expr.Length && (char.IsLetterOrDigit(expr[i + 1]) || expr[i + 1] == '.'))
						{
							text += expr[++i].ToString();
						}
						list.Add(text);
						text = "";
					}
					else if (char.IsDigit(c))
					{
						text += c.ToString();
						while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
						{
							text += expr[++i].ToString();
						}
						list.Add(text);
						text = "";
					}
					else if (i + 1 < expr.Length && (c == '-' || c == '+') && char.IsDigit(expr[i + 1]) && (i == 0 || this.OperatorList.IndexOf(expr[i - 1].ToString(this.CultureInfo)) != -1 || (i - 1 > 0 && expr[i - 1] == '(')))
					{
						text += c.ToString();
						while (i + 1 < expr.Length && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
						{
							text += expr[++i].ToString();
						}
						list.Add(text);
						text = "";
					}
					else if (c == '(')
					{
						if (i != 0 && (char.IsDigit(expr[i - 1]) || char.IsDigit(expr[i - 1]) || expr[i - 1] == ')'))
						{
							list.Add("*");
							list.Add("(");
						}
						else
						{
							list.Add("(");
						}
					}
					else
					{
						list.Add(c.ToString());
					}
				}
			}
			return list;
		}

		// Token: 0x06004EE5 RID: 20197 RVA: 0x0016EDEC File Offset: 0x0016CFEC
		private double MathParserLogic(List<string> tokens)
		{
			for (int i = 0; i < tokens.Count; i++)
			{
				if (this.LocalVariables.Keys.Contains(tokens[i]))
				{
					tokens[i] = this.LocalVariables[tokens[i]].ToString(this.CultureInfo);
				}
			}
			while (tokens.IndexOf("(") != -1)
			{
				int num = tokens.LastIndexOf("(");
				int num2 = tokens.IndexOf(")", num);
				if (num >= num2)
				{
					throw new ArithmeticException("No closing bracket/parenthesis. Token: " + num.ToString(this.CultureInfo));
				}
				this.roughExpr.Clear();
				for (int j = num + 1; j < num2; j++)
				{
					this.roughExpr.Add(tokens[j]);
				}
				this.args.Clear();
				string text = tokens[(num == 0) ? 0 : (num - 1)];
				double num4;
				if (this.LocalFunctions.Keys.Contains(text))
				{
					if (this.roughExpr.Contains(","))
					{
						for (int k = 0; k < this.roughExpr.Count; k++)
						{
							List<string> list = new List<string>();
							int num3 = (this.roughExpr.IndexOf(",", k) != -1) ? this.roughExpr.IndexOf(",", k) : this.roughExpr.Count;
							while (k < num3)
							{
								list.Add(this.roughExpr[k++]);
							}
							this.args.Add((list.Count == 0) ? 0.0 : this.BasicArithmeticalExpression(list));
						}
						num4 = double.Parse(this.LocalFunctions[text](this.args.ToArray()).ToString(this.CultureInfo), this.CultureInfo);
					}
					else
					{
						num4 = double.Parse(this.LocalFunctions[text](new double[]
						{
							this.BasicArithmeticalExpression(this.roughExpr)
						}).ToString(this.CultureInfo), this.CultureInfo);
					}
				}
				else
				{
					num4 = this.BasicArithmeticalExpression(this.roughExpr);
				}
				tokens[num] = num4.ToString(this.CultureInfo);
				tokens.RemoveRange(num + 1, num2 - num);
				if (this.LocalFunctions.Keys.Contains(text))
				{
					tokens.RemoveAt(num - 1);
				}
			}
			return this.BasicArithmeticalExpression(tokens);
		}

		// Token: 0x06004EE6 RID: 20198 RVA: 0x0016F088 File Offset: 0x0016D288
		private double BasicArithmeticalExpression(List<string> tokens)
		{
			switch (tokens.Count)
			{
			case 0:
				return 0.0;
			case 1:
				return double.Parse(tokens[0], this.CultureInfo);
			case 2:
			{
				string text = tokens[0];
				if (text == "-" || text == "+")
				{
					return double.Parse(((text == "+") ? "" : ((tokens[1].Substring(0, 1) == "-") ? "" : "-")) + tokens[1], this.CultureInfo);
				}
				return this.OperatorAction[text](0.0, double.Parse(tokens[1], this.CultureInfo));
			}
			default:
				foreach (string text2 in this.OperatorList)
				{
					while (tokens.IndexOf(text2) != -1)
					{
						int num = tokens.IndexOf(text2);
						double arg = double.Parse(tokens[num - 1], this.CultureInfo);
						double arg2 = double.Parse(tokens[num + 1], this.CultureInfo);
						double num2 = this.OperatorAction[text2](arg, arg2);
						tokens[num - 1] = num2.ToString(this.CultureInfo);
						tokens.RemoveRange(num, 2);
					}
				}
				return double.Parse(tokens[0], this.CultureInfo);
			}
		}

		// Token: 0x04004F79 RID: 20345
		private readonly List<string> roughExpr = new List<string>();

		// Token: 0x04004F7A RID: 20346
		private readonly List<double> args = new List<double>();
	}
}
