using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InControl.Internal
{
	// Token: 0x0200094A RID: 2378
	public class CodeWriter
	{
		// Token: 0x060054CE RID: 21710 RVA: 0x00182331 File Offset: 0x00180531
		public CodeWriter()
		{
			this.indent = 0;
			this.stringBuilder = new StringBuilder(4096);
		}

		// Token: 0x060054CF RID: 21711 RVA: 0x00182350 File Offset: 0x00180550
		public void IncreaseIndent()
		{
			this.indent++;
		}

		// Token: 0x060054D0 RID: 21712 RVA: 0x00182360 File Offset: 0x00180560
		public void DecreaseIndent()
		{
			this.indent--;
		}

		// Token: 0x060054D1 RID: 21713 RVA: 0x00182370 File Offset: 0x00180570
		public void Append(string code)
		{
			this.Append(false, code);
		}

		// Token: 0x060054D2 RID: 21714 RVA: 0x0018237C File Offset: 0x0018057C
		public void Append(bool trim, string code)
		{
			if (trim)
			{
				code = code.Trim();
			}
			string[] array = Regex.Split(code, "\\r?\\n|\\n");
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				string text = array[i];
				if (!text.All(new Func<char, bool>(char.IsWhiteSpace)))
				{
					this.stringBuilder.Append('\t', this.indent);
					this.stringBuilder.Append(text);
				}
				if (i < num - 1)
				{
					this.stringBuilder.Append('\n');
				}
			}
		}

		// Token: 0x060054D3 RID: 21715 RVA: 0x001823FD File Offset: 0x001805FD
		public void AppendLine(string code)
		{
			this.Append(code);
			this.stringBuilder.Append('\n');
		}

		// Token: 0x060054D4 RID: 21716 RVA: 0x00182414 File Offset: 0x00180614
		public void AppendLine(int count)
		{
			this.stringBuilder.Append('\n', count);
		}

		// Token: 0x060054D5 RID: 21717 RVA: 0x00182425 File Offset: 0x00180625
		public void AppendFormat(string format, params object[] args)
		{
			this.Append(string.Format(format, args));
		}

		// Token: 0x060054D6 RID: 21718 RVA: 0x00182434 File Offset: 0x00180634
		public void AppendLineFormat(string format, params object[] args)
		{
			this.AppendLine(string.Format(format, args));
		}

		// Token: 0x060054D7 RID: 21719 RVA: 0x00182443 File Offset: 0x00180643
		public override string ToString()
		{
			return this.stringBuilder.ToString();
		}

		// Token: 0x040053CD RID: 21453
		private const char newLine = '\n';

		// Token: 0x040053CE RID: 21454
		private int indent;

		// Token: 0x040053CF RID: 21455
		private readonly StringBuilder stringBuilder;
	}
}
