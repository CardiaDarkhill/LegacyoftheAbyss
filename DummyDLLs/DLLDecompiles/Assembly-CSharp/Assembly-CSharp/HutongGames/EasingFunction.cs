using System;
using UnityEngine;

namespace HutongGames
{
	// Token: 0x02000AE2 RID: 2786
	public class EasingFunction
	{
		// Token: 0x06005847 RID: 22599 RVA: 0x001C1006 File Offset: 0x001BF206
		public static float Linear(float start, float end, float value)
		{
			return Mathf.Lerp(start, end, value);
		}

		// Token: 0x06005848 RID: 22600 RVA: 0x001C1010 File Offset: 0x001BF210
		public static float Spring(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			value = (Mathf.Sin(value * 3.1415927f * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
			return start + (end - start) * value;
		}

		// Token: 0x06005849 RID: 22601 RVA: 0x001C1074 File Offset: 0x001BF274
		public static float EaseInQuad(float start, float end, float value)
		{
			end -= start;
			return end * value * value + start;
		}

		// Token: 0x0600584A RID: 22602 RVA: 0x001C1082 File Offset: 0x001BF282
		public static float EaseOutQuad(float start, float end, float value)
		{
			end -= start;
			return -end * value * (value - 2f) + start;
		}

		// Token: 0x0600584B RID: 22603 RVA: 0x001C1098 File Offset: 0x001BF298
		public static float EaseInOutQuad(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value + start;
			}
			value -= 1f;
			return -end * 0.5f * (value * (value - 2f) - 1f) + start;
		}

		// Token: 0x0600584C RID: 22604 RVA: 0x001C10EC File Offset: 0x001BF2EC
		public static float EaseInCubic(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value + start;
		}

		// Token: 0x0600584D RID: 22605 RVA: 0x001C10FC File Offset: 0x001BF2FC
		public static float EaseOutCubic(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value + 1f) + start;
		}

		// Token: 0x0600584E RID: 22606 RVA: 0x001C111C File Offset: 0x001BF31C
		public static float EaseInOutCubic(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value + start;
			}
			value -= 2f;
			return end * 0.5f * (value * value * value + 2f) + start;
		}

		// Token: 0x0600584F RID: 22607 RVA: 0x001C116D File Offset: 0x001BF36D
		public static float EaseInQuart(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value + start;
		}

		// Token: 0x06005850 RID: 22608 RVA: 0x001C117F File Offset: 0x001BF37F
		public static float EaseOutQuart(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -end * (value * value * value * value - 1f) + start;
		}

		// Token: 0x06005851 RID: 22609 RVA: 0x001C11A4 File Offset: 0x001BF3A4
		public static float EaseInOutQuart(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value * value + start;
			}
			value -= 2f;
			return -end * 0.5f * (value * value * value * value - 2f) + start;
		}

		// Token: 0x06005852 RID: 22610 RVA: 0x001C11FA File Offset: 0x001BF3FA
		public static float EaseInQuint(float start, float end, float value)
		{
			end -= start;
			return end * value * value * value * value * value + start;
		}

		// Token: 0x06005853 RID: 22611 RVA: 0x001C120E File Offset: 0x001BF40E
		public static float EaseOutQuint(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * (value * value * value * value * value + 1f) + start;
		}

		// Token: 0x06005854 RID: 22612 RVA: 0x001C1234 File Offset: 0x001BF434
		public static float EaseInOutQuint(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * value * value * value * value * value + start;
			}
			value -= 2f;
			return end * 0.5f * (value * value * value * value * value + 2f) + start;
		}

		// Token: 0x06005855 RID: 22613 RVA: 0x001C128D File Offset: 0x001BF48D
		public static float EaseInSine(float start, float end, float value)
		{
			end -= start;
			return -end * Mathf.Cos(value * 1.5707964f) + end + start;
		}

		// Token: 0x06005856 RID: 22614 RVA: 0x001C12A7 File Offset: 0x001BF4A7
		public static float EaseOutSine(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Sin(value * 1.5707964f) + start;
		}

		// Token: 0x06005857 RID: 22615 RVA: 0x001C12BE File Offset: 0x001BF4BE
		public static float EaseInOutSine(float start, float end, float value)
		{
			end -= start;
			return -end * 0.5f * (Mathf.Cos(3.1415927f * value) - 1f) + start;
		}

		// Token: 0x06005858 RID: 22616 RVA: 0x001C12E2 File Offset: 0x001BF4E2
		public static float EaseInExpo(float start, float end, float value)
		{
			end -= start;
			return end * Mathf.Pow(2f, 10f * (value - 1f)) + start;
		}

		// Token: 0x06005859 RID: 22617 RVA: 0x001C1304 File Offset: 0x001BF504
		public static float EaseOutExpo(float start, float end, float value)
		{
			end -= start;
			return end * (-Mathf.Pow(2f, -10f * value) + 1f) + start;
		}

		// Token: 0x0600585A RID: 22618 RVA: 0x001C1328 File Offset: 0x001BF528
		public static float EaseInOutExpo(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * 0.5f * Mathf.Pow(2f, 10f * (value - 1f)) + start;
			}
			value -= 1f;
			return end * 0.5f * (-Mathf.Pow(2f, -10f * value) + 2f) + start;
		}

		// Token: 0x0600585B RID: 22619 RVA: 0x001C1398 File Offset: 0x001BF598
		public static float EaseInCirc(float start, float end, float value)
		{
			end -= start;
			return -end * (Mathf.Sqrt(1f - value * value) - 1f) + start;
		}

		// Token: 0x0600585C RID: 22620 RVA: 0x001C13B8 File Offset: 0x001BF5B8
		public static float EaseOutCirc(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return end * Mathf.Sqrt(1f - value * value) + start;
		}

		// Token: 0x0600585D RID: 22621 RVA: 0x001C13DC File Offset: 0x001BF5DC
		public static float EaseInOutCirc(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return -end * 0.5f * (Mathf.Sqrt(1f - value * value) - 1f) + start;
			}
			value -= 2f;
			return end * 0.5f * (Mathf.Sqrt(1f - value * value) + 1f) + start;
		}

		// Token: 0x0600585E RID: 22622 RVA: 0x001C1448 File Offset: 0x001BF648
		public static float EaseInBounce(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			return end - EasingFunction.EaseOutBounce(0f, end, num - value) + start;
		}

		// Token: 0x0600585F RID: 22623 RVA: 0x001C1474 File Offset: 0x001BF674
		public static float EaseOutBounce(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return end * (7.5625f * value * value) + start;
			}
			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return end * (7.5625f * value * value + 0.75f) + start;
			}
			if ((double)value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return end * (7.5625f * value * value + 0.9375f) + start;
			}
			value -= 0.95454544f;
			return end * (7.5625f * value * value + 0.984375f) + start;
		}

		// Token: 0x06005860 RID: 22624 RVA: 0x001C1510 File Offset: 0x001BF710
		public static float EaseInOutBounce(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			if (value < num * 0.5f)
			{
				return EasingFunction.EaseInBounce(0f, end, value * 2f) * 0.5f + start;
			}
			return EasingFunction.EaseOutBounce(0f, end, value * 2f - num) * 0.5f + end * 0.5f + start;
		}

		// Token: 0x06005861 RID: 22625 RVA: 0x001C1574 File Offset: 0x001BF774
		public static float EaseInBack(float start, float end, float value)
		{
			end -= start;
			value /= 1f;
			float num = 1.70158f;
			return end * value * value * ((num + 1f) * value - num) + start;
		}

		// Token: 0x06005862 RID: 22626 RVA: 0x001C15A8 File Offset: 0x001BF7A8
		public static float EaseOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value -= 1f;
			return end * (value * value * ((num + 1f) * value + num) + 1f) + start;
		}

		// Token: 0x06005863 RID: 22627 RVA: 0x001C15E4 File Offset: 0x001BF7E4
		public static float EaseInOutBack(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return end * 0.5f * (value * value * ((num + 1f) * value - num)) + start;
			}
			value -= 2f;
			num *= 1.525f;
			return end * 0.5f * (value * value * ((num + 1f) * value + num) + 2f) + start;
		}

		// Token: 0x06005864 RID: 22628 RVA: 0x001C1660 File Offset: 0x001BF860
		public static float EaseInElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			return -(num3 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2)) + start;
		}

		// Token: 0x06005865 RID: 22629 RVA: 0x001C1704 File Offset: 0x001BF904
		public static float EaseOutElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num) == 1f)
			{
				return start + end;
			}
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 * 0.25f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			return num3 * Mathf.Pow(2f, -10f * value) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) + end + start;
		}

		// Token: 0x06005866 RID: 22630 RVA: 0x001C17A0 File Offset: 0x001BF9A0
		public static float EaseInOutElastic(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			if (value == 0f)
			{
				return start;
			}
			if ((value /= num * 0.5f) == 2f)
			{
				return start + end;
			}
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			if (value < 1f)
			{
				return -0.5f * (num3 * Mathf.Pow(2f, 10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2)) + start;
			}
			return num3 * Mathf.Pow(2f, -10f * (value -= 1f)) * Mathf.Sin((value * num - num4) * 6.2831855f / num2) * 0.5f + end + start;
		}

		// Token: 0x06005867 RID: 22631 RVA: 0x001C188E File Offset: 0x001BFA8E
		public static float LinearD(float start, float end, float value)
		{
			return end - start;
		}

		// Token: 0x06005868 RID: 22632 RVA: 0x001C1893 File Offset: 0x001BFA93
		public static float EaseInQuadD(float start, float end, float value)
		{
			return 2f * (end - start) * value;
		}

		// Token: 0x06005869 RID: 22633 RVA: 0x001C18A0 File Offset: 0x001BFAA0
		public static float EaseOutQuadD(float start, float end, float value)
		{
			end -= start;
			return -end * value - end * (value - 2f);
		}

		// Token: 0x0600586A RID: 22634 RVA: 0x001C18B5 File Offset: 0x001BFAB5
		public static float EaseInOutQuadD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * value;
			}
			value -= 1f;
			return end * (1f - value);
		}

		// Token: 0x0600586B RID: 22635 RVA: 0x001C18E3 File Offset: 0x001BFAE3
		public static float EaseInCubicD(float start, float end, float value)
		{
			return 3f * (end - start) * value * value;
		}

		// Token: 0x0600586C RID: 22636 RVA: 0x001C18F2 File Offset: 0x001BFAF2
		public static float EaseOutCubicD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return 3f * end * value * value;
		}

		// Token: 0x0600586D RID: 22637 RVA: 0x001C190D File Offset: 0x001BFB0D
		public static float EaseInOutCubicD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 1.5f * end * value * value;
			}
			value -= 2f;
			return 1.5f * end * value * value;
		}

		// Token: 0x0600586E RID: 22638 RVA: 0x001C1945 File Offset: 0x001BFB45
		public static float EaseInQuartD(float start, float end, float value)
		{
			return 4f * (end - start) * value * value * value;
		}

		// Token: 0x0600586F RID: 22639 RVA: 0x001C1956 File Offset: 0x001BFB56
		public static float EaseOutQuartD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -4f * end * value * value * value;
		}

		// Token: 0x06005870 RID: 22640 RVA: 0x001C1973 File Offset: 0x001BFB73
		public static float EaseInOutQuartD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 2f * end * value * value * value;
			}
			value -= 2f;
			return -2f * end * value * value * value;
		}

		// Token: 0x06005871 RID: 22641 RVA: 0x001C19AF File Offset: 0x001BFBAF
		public static float EaseInQuintD(float start, float end, float value)
		{
			return 5f * (end - start) * value * value * value * value;
		}

		// Token: 0x06005872 RID: 22642 RVA: 0x001C19C2 File Offset: 0x001BFBC2
		public static float EaseOutQuintD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return 5f * end * value * value * value * value;
		}

		// Token: 0x06005873 RID: 22643 RVA: 0x001C19E1 File Offset: 0x001BFBE1
		public static float EaseInOutQuintD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 2.5f * end * value * value * value * value;
			}
			value -= 2f;
			return 2.5f * end * value * value * value * value;
		}

		// Token: 0x06005874 RID: 22644 RVA: 0x001C1A21 File Offset: 0x001BFC21
		public static float EaseInSineD(float start, float end, float value)
		{
			return (end - start) * 0.5f * 3.1415927f * Mathf.Sin(1.5707964f * value);
		}

		// Token: 0x06005875 RID: 22645 RVA: 0x001C1A3F File Offset: 0x001BFC3F
		public static float EaseOutSineD(float start, float end, float value)
		{
			end -= start;
			return 1.5707964f * end * Mathf.Cos(value * 1.5707964f);
		}

		// Token: 0x06005876 RID: 22646 RVA: 0x001C1A5A File Offset: 0x001BFC5A
		public static float EaseInOutSineD(float start, float end, float value)
		{
			end -= start;
			return end * 0.5f * 3.1415927f * Mathf.Cos(3.1415927f * value);
		}

		// Token: 0x06005877 RID: 22647 RVA: 0x001C1A7B File Offset: 0x001BFC7B
		public static float EaseInExpoD(float start, float end, float value)
		{
			return 6.931472f * (end - start) * Mathf.Pow(2f, 10f * (value - 1f));
		}

		// Token: 0x06005878 RID: 22648 RVA: 0x001C1A9E File Offset: 0x001BFC9E
		public static float EaseOutExpoD(float start, float end, float value)
		{
			end -= start;
			return 3.465736f * end * Mathf.Pow(2f, 1f - 10f * value);
		}

		// Token: 0x06005879 RID: 22649 RVA: 0x001C1AC4 File Offset: 0x001BFCC4
		public static float EaseInOutExpoD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return 3.465736f * end * Mathf.Pow(2f, 10f * (value - 1f));
			}
			value -= 1f;
			return 3.465736f * end / Mathf.Pow(2f, 10f * value);
		}

		// Token: 0x0600587A RID: 22650 RVA: 0x001C1B29 File Offset: 0x001BFD29
		public static float EaseInCircD(float start, float end, float value)
		{
			return (end - start) * value / Mathf.Sqrt(1f - value * value);
		}

		// Token: 0x0600587B RID: 22651 RVA: 0x001C1B3F File Offset: 0x001BFD3F
		public static float EaseOutCircD(float start, float end, float value)
		{
			value -= 1f;
			end -= start;
			return -end * value / Mathf.Sqrt(1f - value * value);
		}

		// Token: 0x0600587C RID: 22652 RVA: 0x001C1B64 File Offset: 0x001BFD64
		public static float EaseInOutCircD(float start, float end, float value)
		{
			value /= 0.5f;
			end -= start;
			if (value < 1f)
			{
				return end * value / (2f * Mathf.Sqrt(1f - value * value));
			}
			value -= 2f;
			return -end * value / (2f * Mathf.Sqrt(1f - value * value));
		}

		// Token: 0x0600587D RID: 22653 RVA: 0x001C1BC4 File Offset: 0x001BFDC4
		public static float EaseInBounceD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			return EasingFunction.EaseOutBounceD(0f, end, num - value);
		}

		// Token: 0x0600587E RID: 22654 RVA: 0x001C1BEC File Offset: 0x001BFDEC
		public static float EaseOutBounceD(float start, float end, float value)
		{
			value /= 1f;
			end -= start;
			if (value < 0.36363637f)
			{
				return 2f * end * 7.5625f * value;
			}
			if (value < 0.72727275f)
			{
				value -= 0.54545456f;
				return 2f * end * 7.5625f * value;
			}
			if ((double)value < 0.9090909090909091)
			{
				value -= 0.8181818f;
				return 2f * end * 7.5625f * value;
			}
			value -= 0.95454544f;
			return 2f * end * 7.5625f * value;
		}

		// Token: 0x0600587F RID: 22655 RVA: 0x001C1C80 File Offset: 0x001BFE80
		public static float EaseInOutBounceD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			if (value < num * 0.5f)
			{
				return EasingFunction.EaseInBounceD(0f, end, value * 2f) * 0.5f;
			}
			return EasingFunction.EaseOutBounceD(0f, end, value * 2f - num) * 0.5f;
		}

		// Token: 0x06005880 RID: 22656 RVA: 0x001C1CD8 File Offset: 0x001BFED8
		public static float EaseInBackD(float start, float end, float value)
		{
			float num = 1.70158f;
			return 3f * (num + 1f) * (end - start) * value * value - 2f * num * (end - start) * value;
		}

		// Token: 0x06005881 RID: 22657 RVA: 0x001C1D10 File Offset: 0x001BFF10
		public static float EaseOutBackD(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value -= 1f;
			return end * ((num + 1f) * value * value + 2f * value * ((num + 1f) * value + num));
		}

		// Token: 0x06005882 RID: 22658 RVA: 0x001C1D54 File Offset: 0x001BFF54
		public static float EaseInOutBackD(float start, float end, float value)
		{
			float num = 1.70158f;
			end -= start;
			value /= 0.5f;
			if (value < 1f)
			{
				num *= 1.525f;
				return 0.5f * end * (num + 1f) * value * value + end * value * ((num + 1f) * value - num);
			}
			value -= 2f;
			num *= 1.525f;
			return 0.5f * end * ((num + 1f) * value * value + 2f * value * ((num + 1f) * value + num));
		}

		// Token: 0x06005883 RID: 22659 RVA: 0x001C1DE4 File Offset: 0x001BFFE4
		public static float EaseInElasticD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			float num5 = 6.2831855f;
			return -num3 * num * num5 * Mathf.Cos(num5 * (num * (value - 1f) - num4) / num2) / num2 - 3.465736f * num3 * Mathf.Sin(num5 * (num * (value - 1f) - num4) / num2) * Mathf.Pow(2f, 10f * (value - 1f) + 1f);
		}

		// Token: 0x06005884 RID: 22660 RVA: 0x001C1E9C File Offset: 0x001C009C
		public static float EaseOutElasticD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 * 0.25f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			return num3 * 3.1415927f * num * Mathf.Pow(2f, 1f - 10f * value) * Mathf.Cos(6.2831855f * (num * value - num4) / num2) / num2 - 3.465736f * num3 * Mathf.Pow(2f, 1f - 10f * value) * Mathf.Sin(6.2831855f * (num * value - num4) / num2);
		}

		// Token: 0x06005885 RID: 22661 RVA: 0x001C1F5C File Offset: 0x001C015C
		public static float EaseInOutElasticD(float start, float end, float value)
		{
			end -= start;
			float num = 1f;
			float num2 = num * 0.3f;
			float num3 = 0f;
			float num4;
			if (num3 == 0f || num3 < Mathf.Abs(end))
			{
				num3 = end;
				num4 = num2 / 4f;
			}
			else
			{
				num4 = num2 / 6.2831855f * Mathf.Asin(end / num3);
			}
			if (value < 1f)
			{
				value -= 1f;
				return -3.465736f * num3 * Mathf.Pow(2f, 10f * value) * Mathf.Sin(6.2831855f * (num * value - 2f) / num2) - num3 * 3.1415927f * num * Mathf.Pow(2f, 10f * value) * Mathf.Cos(6.2831855f * (num * value - num4) / num2) / num2;
			}
			value -= 1f;
			return num3 * 3.1415927f * num * Mathf.Cos(6.2831855f * (num * value - num4) / num2) / (num2 * Mathf.Pow(2f, 10f * value)) - 3.465736f * num3 * Mathf.Sin(6.2831855f * (num * value - num4) / num2) / Mathf.Pow(2f, 10f * value);
		}

		// Token: 0x06005886 RID: 22662 RVA: 0x001C208C File Offset: 0x001C028C
		public static float SpringD(float start, float end, float value)
		{
			value = Mathf.Clamp01(value);
			end -= start;
			return end * (6f * (1f - value) / 5f + 1f) * (-2.2f * Mathf.Pow(1f - value, 1.2f) * Mathf.Sin(3.1415927f * value * (2.5f * value * value * value + 0.2f)) + Mathf.Pow(1f - value, 2.2f) * (3.1415927f * (2.5f * value * value * value + 0.2f) + 23.561945f * value * value * value) * Mathf.Cos(3.1415927f * value * (2.5f * value * value * value + 0.2f)) + 1f) - 6f * end * (Mathf.Pow(1f - value, 2.2f) * Mathf.Sin(3.1415927f * value * (2.5f * value * value * value + 0.2f)) + value / 5f);
		}

		// Token: 0x06005887 RID: 22663 RVA: 0x001C2194 File Offset: 0x001C0394
		public static float CustomCurve(float start, float end, float value)
		{
			if (EasingFunction.AnimationCurve == null)
			{
				return Mathf.Lerp(start, end, value);
			}
			return Mathf.Lerp(start, end, EasingFunction.AnimationCurve.Evaluate(value));
		}

		// Token: 0x06005888 RID: 22664 RVA: 0x001C21B8 File Offset: 0x001C03B8
		public static float Punch(float start, float end, float value)
		{
			if (value <= 0f)
			{
				return start;
			}
			if (value >= 1f)
			{
				return start;
			}
			return Mathf.Pow(2f, -10f * value) * Mathf.Sin(value * 6.2831855f / 0.15f);
		}

		// Token: 0x06005889 RID: 22665 RVA: 0x001C21F4 File Offset: 0x001C03F4
		public static float PunchD(float start, float end, float value)
		{
			return -(10f * Mathf.Log(2f) * 0.15f * Mathf.Sin(6.2831855f * value / 0.15f) - 6.2831855f * Mathf.Cos(6.2831855f * value / 0.15f)) / Mathf.Pow(0.3f, 10f * value);
		}

		// Token: 0x0600588A RID: 22666 RVA: 0x001C2258 File Offset: 0x001C0458
		public static EasingFunction.Function GetEasingFunction(EasingFunction.Ease easingFunction)
		{
			switch (easingFunction)
			{
			case EasingFunction.Ease.EaseInQuad:
				return new EasingFunction.Function(EasingFunction.EaseInQuad);
			case EasingFunction.Ease.EaseOutQuad:
				return new EasingFunction.Function(EasingFunction.EaseOutQuad);
			case EasingFunction.Ease.EaseInOutQuad:
				return new EasingFunction.Function(EasingFunction.EaseInOutQuad);
			case EasingFunction.Ease.EaseInCubic:
				return new EasingFunction.Function(EasingFunction.EaseInCubic);
			case EasingFunction.Ease.EaseOutCubic:
				return new EasingFunction.Function(EasingFunction.EaseOutCubic);
			case EasingFunction.Ease.EaseInOutCubic:
				return new EasingFunction.Function(EasingFunction.EaseInOutCubic);
			case EasingFunction.Ease.EaseInQuart:
				return new EasingFunction.Function(EasingFunction.EaseInQuart);
			case EasingFunction.Ease.EaseOutQuart:
				return new EasingFunction.Function(EasingFunction.EaseOutQuart);
			case EasingFunction.Ease.EaseInOutQuart:
				return new EasingFunction.Function(EasingFunction.EaseInOutQuart);
			case EasingFunction.Ease.EaseInQuint:
				return new EasingFunction.Function(EasingFunction.EaseInQuint);
			case EasingFunction.Ease.EaseOutQuint:
				return new EasingFunction.Function(EasingFunction.EaseOutQuint);
			case EasingFunction.Ease.EaseInOutQuint:
				return new EasingFunction.Function(EasingFunction.EaseInOutQuint);
			case EasingFunction.Ease.EaseInSine:
				return new EasingFunction.Function(EasingFunction.EaseInSine);
			case EasingFunction.Ease.EaseOutSine:
				return new EasingFunction.Function(EasingFunction.EaseOutSine);
			case EasingFunction.Ease.EaseInOutSine:
				return new EasingFunction.Function(EasingFunction.EaseInOutSine);
			case EasingFunction.Ease.EaseInExpo:
				return new EasingFunction.Function(EasingFunction.EaseInExpo);
			case EasingFunction.Ease.EaseOutExpo:
				return new EasingFunction.Function(EasingFunction.EaseOutExpo);
			case EasingFunction.Ease.EaseInOutExpo:
				return new EasingFunction.Function(EasingFunction.EaseInOutExpo);
			case EasingFunction.Ease.EaseInCirc:
				return new EasingFunction.Function(EasingFunction.EaseInCirc);
			case EasingFunction.Ease.EaseOutCirc:
				return new EasingFunction.Function(EasingFunction.EaseOutCirc);
			case EasingFunction.Ease.EaseInOutCirc:
				return new EasingFunction.Function(EasingFunction.EaseInOutCirc);
			case EasingFunction.Ease.Linear:
				return new EasingFunction.Function(EasingFunction.Linear);
			case EasingFunction.Ease.Spring:
				return new EasingFunction.Function(EasingFunction.Spring);
			case EasingFunction.Ease.EaseInBounce:
				return new EasingFunction.Function(EasingFunction.EaseInBounce);
			case EasingFunction.Ease.EaseOutBounce:
				return new EasingFunction.Function(EasingFunction.EaseOutBounce);
			case EasingFunction.Ease.EaseInOutBounce:
				return new EasingFunction.Function(EasingFunction.EaseInOutBounce);
			case EasingFunction.Ease.EaseInBack:
				return new EasingFunction.Function(EasingFunction.EaseInBack);
			case EasingFunction.Ease.EaseOutBack:
				return new EasingFunction.Function(EasingFunction.EaseOutBack);
			case EasingFunction.Ease.EaseInOutBack:
				return new EasingFunction.Function(EasingFunction.EaseInOutBack);
			case EasingFunction.Ease.EaseInElastic:
				return new EasingFunction.Function(EasingFunction.EaseInElastic);
			case EasingFunction.Ease.EaseOutElastic:
				return new EasingFunction.Function(EasingFunction.EaseOutElastic);
			case EasingFunction.Ease.EaseInOutElastic:
				return new EasingFunction.Function(EasingFunction.EaseInOutElastic);
			case EasingFunction.Ease.CustomCurve:
				return new EasingFunction.Function(EasingFunction.CustomCurve);
			case EasingFunction.Ease.Punch:
				return new EasingFunction.Function(EasingFunction.Punch);
			default:
				return null;
			}
		}

		// Token: 0x0600588B RID: 22667 RVA: 0x001C24B4 File Offset: 0x001C06B4
		public static EasingFunction.Function GetEasingFunctionDerivative(EasingFunction.Ease easingFunction)
		{
			switch (easingFunction)
			{
			case EasingFunction.Ease.EaseInQuad:
				return new EasingFunction.Function(EasingFunction.EaseInQuadD);
			case EasingFunction.Ease.EaseOutQuad:
				return new EasingFunction.Function(EasingFunction.EaseOutQuadD);
			case EasingFunction.Ease.EaseInOutQuad:
				return new EasingFunction.Function(EasingFunction.EaseInOutQuadD);
			case EasingFunction.Ease.EaseInCubic:
				return new EasingFunction.Function(EasingFunction.EaseInCubicD);
			case EasingFunction.Ease.EaseOutCubic:
				return new EasingFunction.Function(EasingFunction.EaseOutCubicD);
			case EasingFunction.Ease.EaseInOutCubic:
				return new EasingFunction.Function(EasingFunction.EaseInOutCubicD);
			case EasingFunction.Ease.EaseInQuart:
				return new EasingFunction.Function(EasingFunction.EaseInQuartD);
			case EasingFunction.Ease.EaseOutQuart:
				return new EasingFunction.Function(EasingFunction.EaseOutQuartD);
			case EasingFunction.Ease.EaseInOutQuart:
				return new EasingFunction.Function(EasingFunction.EaseInOutQuartD);
			case EasingFunction.Ease.EaseInQuint:
				return new EasingFunction.Function(EasingFunction.EaseInQuintD);
			case EasingFunction.Ease.EaseOutQuint:
				return new EasingFunction.Function(EasingFunction.EaseOutQuintD);
			case EasingFunction.Ease.EaseInOutQuint:
				return new EasingFunction.Function(EasingFunction.EaseInOutQuintD);
			case EasingFunction.Ease.EaseInSine:
				return new EasingFunction.Function(EasingFunction.EaseInSineD);
			case EasingFunction.Ease.EaseOutSine:
				return new EasingFunction.Function(EasingFunction.EaseOutSineD);
			case EasingFunction.Ease.EaseInOutSine:
				return new EasingFunction.Function(EasingFunction.EaseInOutSineD);
			case EasingFunction.Ease.EaseInExpo:
				return new EasingFunction.Function(EasingFunction.EaseInExpoD);
			case EasingFunction.Ease.EaseOutExpo:
				return new EasingFunction.Function(EasingFunction.EaseOutExpoD);
			case EasingFunction.Ease.EaseInOutExpo:
				return new EasingFunction.Function(EasingFunction.EaseInOutExpoD);
			case EasingFunction.Ease.EaseInCirc:
				return new EasingFunction.Function(EasingFunction.EaseInCircD);
			case EasingFunction.Ease.EaseOutCirc:
				return new EasingFunction.Function(EasingFunction.EaseOutCircD);
			case EasingFunction.Ease.EaseInOutCirc:
				return new EasingFunction.Function(EasingFunction.EaseInOutCircD);
			case EasingFunction.Ease.Linear:
				return new EasingFunction.Function(EasingFunction.LinearD);
			case EasingFunction.Ease.Spring:
				return new EasingFunction.Function(EasingFunction.SpringD);
			case EasingFunction.Ease.EaseInBounce:
				return new EasingFunction.Function(EasingFunction.EaseInBounceD);
			case EasingFunction.Ease.EaseOutBounce:
				return new EasingFunction.Function(EasingFunction.EaseOutBounceD);
			case EasingFunction.Ease.EaseInOutBounce:
				return new EasingFunction.Function(EasingFunction.EaseInOutBounceD);
			case EasingFunction.Ease.EaseInBack:
				return new EasingFunction.Function(EasingFunction.EaseInBackD);
			case EasingFunction.Ease.EaseOutBack:
				return new EasingFunction.Function(EasingFunction.EaseOutBackD);
			case EasingFunction.Ease.EaseInOutBack:
				return new EasingFunction.Function(EasingFunction.EaseInOutBackD);
			case EasingFunction.Ease.EaseInElastic:
				return new EasingFunction.Function(EasingFunction.EaseInElasticD);
			case EasingFunction.Ease.EaseOutElastic:
				return new EasingFunction.Function(EasingFunction.EaseOutElasticD);
			case EasingFunction.Ease.EaseInOutElastic:
				return new EasingFunction.Function(EasingFunction.EaseInOutElasticD);
			case EasingFunction.Ease.Punch:
				return new EasingFunction.Function(EasingFunction.PunchD);
			}
			return null;
		}

		// Token: 0x040053FD RID: 21501
		private const float NATURAL_LOG_OF_2 = 0.6931472f;

		// Token: 0x040053FE RID: 21502
		public static AnimationCurve AnimationCurve;

		// Token: 0x02001B72 RID: 7026
		public enum Ease
		{
			// Token: 0x04009D03 RID: 40195
			EaseInQuad,
			// Token: 0x04009D04 RID: 40196
			EaseOutQuad,
			// Token: 0x04009D05 RID: 40197
			EaseInOutQuad,
			// Token: 0x04009D06 RID: 40198
			EaseInCubic,
			// Token: 0x04009D07 RID: 40199
			EaseOutCubic,
			// Token: 0x04009D08 RID: 40200
			EaseInOutCubic,
			// Token: 0x04009D09 RID: 40201
			EaseInQuart,
			// Token: 0x04009D0A RID: 40202
			EaseOutQuart,
			// Token: 0x04009D0B RID: 40203
			EaseInOutQuart,
			// Token: 0x04009D0C RID: 40204
			EaseInQuint,
			// Token: 0x04009D0D RID: 40205
			EaseOutQuint,
			// Token: 0x04009D0E RID: 40206
			EaseInOutQuint,
			// Token: 0x04009D0F RID: 40207
			EaseInSine,
			// Token: 0x04009D10 RID: 40208
			EaseOutSine,
			// Token: 0x04009D11 RID: 40209
			EaseInOutSine,
			// Token: 0x04009D12 RID: 40210
			EaseInExpo,
			// Token: 0x04009D13 RID: 40211
			EaseOutExpo,
			// Token: 0x04009D14 RID: 40212
			EaseInOutExpo,
			// Token: 0x04009D15 RID: 40213
			EaseInCirc,
			// Token: 0x04009D16 RID: 40214
			EaseOutCirc,
			// Token: 0x04009D17 RID: 40215
			EaseInOutCirc,
			// Token: 0x04009D18 RID: 40216
			Linear,
			// Token: 0x04009D19 RID: 40217
			Spring,
			// Token: 0x04009D1A RID: 40218
			EaseInBounce,
			// Token: 0x04009D1B RID: 40219
			EaseOutBounce,
			// Token: 0x04009D1C RID: 40220
			EaseInOutBounce,
			// Token: 0x04009D1D RID: 40221
			EaseInBack,
			// Token: 0x04009D1E RID: 40222
			EaseOutBack,
			// Token: 0x04009D1F RID: 40223
			EaseInOutBack,
			// Token: 0x04009D20 RID: 40224
			EaseInElastic,
			// Token: 0x04009D21 RID: 40225
			EaseOutElastic,
			// Token: 0x04009D22 RID: 40226
			EaseInOutElastic,
			// Token: 0x04009D23 RID: 40227
			CustomCurve,
			// Token: 0x04009D24 RID: 40228
			Punch
		}

		// Token: 0x02001B73 RID: 7027
		// (Invoke) Token: 0x06009A1E RID: 39454
		public delegate float Function(float s, float e, float v);
	}
}
