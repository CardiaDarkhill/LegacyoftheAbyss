using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B60 RID: 2912
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Split a text asset or string into an arrayList")]
	public class SplitTextToArrayList : ArrayListActions
	{
		// Token: 0x06005A87 RID: 23175 RVA: 0x001C9C78 File Offset: 0x001C7E78
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = null;
			this.parseRange = null;
			this.textAsset = null;
			this.split = SplitTextToArrayList.SplitSpecialChars.NewLine;
			this.parseAsType = SplitTextToArrayList.ArrayMakerParseStringAs.String;
		}

		// Token: 0x06005A88 RID: 23176 RVA: 0x001C9CAB File Offset: 0x001C7EAB
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.splitText();
			}
			base.Finish();
		}

		// Token: 0x06005A89 RID: 23177 RVA: 0x001C9CE0 File Offset: 0x001C7EE0
		public void splitText()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			string text;
			if (this.OrThisString.Value.Length == 0)
			{
				if (this.textAsset == null)
				{
					return;
				}
				text = this.textAsset.text;
			}
			else
			{
				text = this.OrThisString.Value;
			}
			this.proxy.arrayList.Clear();
			string[] array;
			if (this.OrThisChar.Value.Length == 0)
			{
				char separator = '\n';
				SplitTextToArrayList.SplitSpecialChars splitSpecialChars = this.split;
				if (splitSpecialChars != SplitTextToArrayList.SplitSpecialChars.Tab)
				{
					if (splitSpecialChars == SplitTextToArrayList.SplitSpecialChars.Space)
					{
						separator = ' ';
					}
				}
				else
				{
					separator = '\t';
				}
				array = text.Split(separator, StringSplitOptions.None);
			}
			else
			{
				array = text.Split(this.OrThisChar.Value.ToCharArray());
			}
			int value = this.startIndex.Value;
			int num = array.Length;
			if (this.parseRange.Value > 0)
			{
				num = Mathf.Min(num - value, this.parseRange.Value);
			}
			string[] array2 = new string[num];
			int num2 = 0;
			for (int i = value; i < value + num; i++)
			{
				array2[num2] = array[i];
				num2++;
			}
			if (this.parseAsType == SplitTextToArrayList.ArrayMakerParseStringAs.String)
			{
				this.proxy.arrayList.InsertRange(0, array2);
				return;
			}
			if (this.parseAsType == SplitTextToArrayList.ArrayMakerParseStringAs.Int)
			{
				int[] array3 = new int[array2.Length];
				int num3 = 0;
				string[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					int.TryParse(array4[j], out array3[num3]);
					num3++;
				}
				this.proxy.arrayList.InsertRange(0, array3);
				return;
			}
			if (this.parseAsType == SplitTextToArrayList.ArrayMakerParseStringAs.Float)
			{
				float[] array5 = new float[array2.Length];
				int num4 = 0;
				string[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					float.TryParse(array4[j], out array5[num4]);
					num4++;
				}
				this.proxy.arrayList.InsertRange(0, array5);
			}
		}

		// Token: 0x0400562A RID: 22058
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400562B RID: 22059
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400562C RID: 22060
		[Tooltip("From where to start parsing, leave to 0 to start from the beginning")]
		public FsmInt startIndex;

		// Token: 0x0400562D RID: 22061
		[Tooltip("the range of parsing")]
		public FsmInt parseRange;

		// Token: 0x0400562E RID: 22062
		[ActionSection("Source")]
		[Tooltip("Text asset source")]
		public TextAsset textAsset;

		// Token: 0x0400562F RID: 22063
		[Tooltip("Text Asset is ignored if this is set.")]
		public FsmString OrThisString;

		// Token: 0x04005630 RID: 22064
		[ActionSection("Split")]
		[Tooltip("Split")]
		public SplitTextToArrayList.SplitSpecialChars split;

		// Token: 0x04005631 RID: 22065
		[Tooltip("Split is ignored if this value is not empty. Each chars taken in account for split")]
		public FsmString OrThisChar;

		// Token: 0x04005632 RID: 22066
		[ActionSection("Value")]
		[Tooltip("Parse the line as a specific type")]
		public SplitTextToArrayList.ArrayMakerParseStringAs parseAsType;

		// Token: 0x02001B75 RID: 7029
		public enum ArrayMakerParseStringAs
		{
			// Token: 0x04009D28 RID: 40232
			String,
			// Token: 0x04009D29 RID: 40233
			Int,
			// Token: 0x04009D2A RID: 40234
			Float
		}

		// Token: 0x02001B76 RID: 7030
		public enum SplitSpecialChars
		{
			// Token: 0x04009D2C RID: 40236
			NewLine,
			// Token: 0x04009D2D RID: 40237
			Tab,
			// Token: 0x04009D2E RID: 40238
			Space
		}
	}
}
