using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InControl
{
	// Token: 0x02000944 RID: 2372
	public static class Utility
	{
		// Token: 0x06005454 RID: 21588 RVA: 0x001802AC File Offset: 0x0017E4AC
		public static void DrawCircleGizmo(Vector2 center, float radius)
		{
			Vector2 v = Utility.circleVertexList[0] * radius + center;
			int num = Utility.circleVertexList.Length;
			for (int i = 1; i < num; i++)
			{
				Gizmos.DrawLine(v, v = Utility.circleVertexList[i] * radius + center);
			}
		}

		// Token: 0x06005455 RID: 21589 RVA: 0x0018030E File Offset: 0x0017E50E
		public static void DrawCircleGizmo(Vector2 center, float radius, Color color)
		{
			Gizmos.color = color;
			Utility.DrawCircleGizmo(center, radius);
		}

		// Token: 0x06005456 RID: 21590 RVA: 0x00180320 File Offset: 0x0017E520
		public static void DrawOvalGizmo(Vector2 center, Vector2 size)
		{
			Vector2 b = size / 2f;
			Vector2 v = Vector2.Scale(Utility.circleVertexList[0], b) + center;
			int num = Utility.circleVertexList.Length;
			for (int i = 1; i < num; i++)
			{
				Gizmos.DrawLine(v, v = Vector2.Scale(Utility.circleVertexList[i], b) + center);
			}
		}

		// Token: 0x06005457 RID: 21591 RVA: 0x0018038E File Offset: 0x0017E58E
		public static void DrawOvalGizmo(Vector2 center, Vector2 size, Color color)
		{
			Gizmos.color = color;
			Utility.DrawOvalGizmo(center, size);
		}

		// Token: 0x06005458 RID: 21592 RVA: 0x001803A0 File Offset: 0x0017E5A0
		public static void DrawRectGizmo(Rect rect)
		{
			Vector3 vector = new Vector3(rect.xMin, rect.yMin);
			Vector3 vector2 = new Vector3(rect.xMax, rect.yMin);
			Vector3 vector3 = new Vector3(rect.xMax, rect.yMax);
			Vector3 vector4 = new Vector3(rect.xMin, rect.yMax);
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector3, vector4);
			Gizmos.DrawLine(vector4, vector);
		}

		// Token: 0x06005459 RID: 21593 RVA: 0x0018041D File Offset: 0x0017E61D
		public static void DrawRectGizmo(Rect rect, Color color)
		{
			Gizmos.color = color;
			Utility.DrawRectGizmo(rect);
		}

		// Token: 0x0600545A RID: 21594 RVA: 0x0018042C File Offset: 0x0017E62C
		public static void DrawRectGizmo(Vector2 center, Vector2 size)
		{
			float num = size.x / 2f;
			float num2 = size.y / 2f;
			Vector3 vector = new Vector3(center.x - num, center.y - num2);
			Vector3 vector2 = new Vector3(center.x + num, center.y - num2);
			Vector3 vector3 = new Vector3(center.x + num, center.y + num2);
			Vector3 vector4 = new Vector3(center.x - num, center.y + num2);
			Gizmos.DrawLine(vector, vector2);
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawLine(vector3, vector4);
			Gizmos.DrawLine(vector4, vector);
		}

		// Token: 0x0600545B RID: 21595 RVA: 0x001804CF File Offset: 0x0017E6CF
		public static void DrawRectGizmo(Vector2 center, Vector2 size, Color color)
		{
			Gizmos.color = color;
			Utility.DrawRectGizmo(center, size);
		}

		// Token: 0x0600545C RID: 21596 RVA: 0x001804DE File Offset: 0x0017E6DE
		public static bool GameObjectIsCulledOnCurrentCamera(GameObject gameObject)
		{
			return (Camera.current.cullingMask & 1 << gameObject.layer) == 0;
		}

		// Token: 0x0600545D RID: 21597 RVA: 0x001804FC File Offset: 0x0017E6FC
		public static Color MoveColorTowards(Color color0, Color color1, float maxDelta)
		{
			float r = Mathf.MoveTowards(color0.r, color1.r, maxDelta);
			float g = Mathf.MoveTowards(color0.g, color1.g, maxDelta);
			float b = Mathf.MoveTowards(color0.b, color1.b, maxDelta);
			float a = Mathf.MoveTowards(color0.a, color1.a, maxDelta);
			return new Color(r, g, b, a);
		}

		// Token: 0x0600545E RID: 21598 RVA: 0x0018055C File Offset: 0x0017E75C
		public static float ApplyDeadZone(float value, float lowerDeadZone, float upperDeadZone)
		{
			float num = upperDeadZone - lowerDeadZone;
			if (value < 0f)
			{
				if (value > -lowerDeadZone)
				{
					return 0f;
				}
				if (value < -upperDeadZone)
				{
					return -1f;
				}
				return (value + lowerDeadZone) / num;
			}
			else
			{
				if (value < lowerDeadZone)
				{
					return 0f;
				}
				if (value > upperDeadZone)
				{
					return 1f;
				}
				return (value - lowerDeadZone) / num;
			}
		}

		// Token: 0x0600545F RID: 21599 RVA: 0x001805AC File Offset: 0x0017E7AC
		public static float ApplySmoothing(float thisValue, float lastValue, float deltaTime, float sensitivity)
		{
			if (Utility.Approximately(sensitivity, 1f))
			{
				return thisValue;
			}
			float maxDelta = deltaTime * sensitivity * 100f;
			if (Utility.IsNotZero(thisValue) && Utility.Sign(lastValue) != Utility.Sign(thisValue))
			{
				lastValue = 0f;
			}
			return Mathf.MoveTowards(lastValue, thisValue, maxDelta);
		}

		// Token: 0x06005460 RID: 21600 RVA: 0x001805F7 File Offset: 0x0017E7F7
		public static float ApplySnapping(float value, float threshold)
		{
			if (value < -threshold)
			{
				return -1f;
			}
			if (value > threshold)
			{
				return 1f;
			}
			return 0f;
		}

		// Token: 0x06005461 RID: 21601 RVA: 0x00180613 File Offset: 0x0017E813
		internal static bool TargetIsButton(InputControlType target)
		{
			return (target >= InputControlType.Action1 && target <= InputControlType.Action12) || (target >= InputControlType.Button0 && target <= InputControlType.Button19);
		}

		// Token: 0x06005462 RID: 21602 RVA: 0x00180636 File Offset: 0x0017E836
		internal static bool TargetIsStandard(InputControlType target)
		{
			return (target >= InputControlType.LeftStickUp && target <= InputControlType.Action12) || (target >= InputControlType.Command && target <= InputControlType.RightCommand);
		}

		// Token: 0x06005463 RID: 21603 RVA: 0x00180658 File Offset: 0x0017E858
		internal static bool TargetIsAlias(InputControlType target)
		{
			return target >= InputControlType.Command && target <= InputControlType.RightCommand;
		}

		// Token: 0x06005464 RID: 21604 RVA: 0x00180670 File Offset: 0x0017E870
		public static string ReadFromFile(string path)
		{
			StreamReader streamReader = new StreamReader(path);
			string result = streamReader.ReadToEnd();
			streamReader.Close();
			return result;
		}

		// Token: 0x06005465 RID: 21605 RVA: 0x00180690 File Offset: 0x0017E890
		public static void WriteToFile(string path, string data)
		{
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.Write(data);
			streamWriter.Flush();
			streamWriter.Close();
		}

		// Token: 0x06005466 RID: 21606 RVA: 0x001806AA File Offset: 0x0017E8AA
		public static float Abs(float value)
		{
			if (value >= 0f)
			{
				return value;
			}
			return -value;
		}

		// Token: 0x06005467 RID: 21607 RVA: 0x001806B8 File Offset: 0x0017E8B8
		public static bool Approximately(float v1, float v2)
		{
			float num = v1 - v2;
			return num >= -1E-07f && num <= 1E-07f;
		}

		// Token: 0x06005468 RID: 21608 RVA: 0x001806DE File Offset: 0x0017E8DE
		public static bool Approximately(Vector2 v1, Vector2 v2)
		{
			return Utility.Approximately(v1.x, v2.x) && Utility.Approximately(v1.y, v2.y);
		}

		// Token: 0x06005469 RID: 21609 RVA: 0x00180706 File Offset: 0x0017E906
		public static bool IsNotZero(float value)
		{
			return value < -1E-07f || value > 1E-07f;
		}

		// Token: 0x0600546A RID: 21610 RVA: 0x0018071A File Offset: 0x0017E91A
		public static bool IsZero(float value)
		{
			return value >= -1E-07f && value <= 1E-07f;
		}

		// Token: 0x0600546B RID: 21611 RVA: 0x00180731 File Offset: 0x0017E931
		public static int Sign(float f)
		{
			if ((double)f >= 0.0)
			{
				return 1;
			}
			return -1;
		}

		// Token: 0x0600546C RID: 21612 RVA: 0x00180743 File Offset: 0x0017E943
		public static bool AbsoluteIsOverThreshold(float value, float threshold)
		{
			return value < -threshold || value > threshold;
		}

		// Token: 0x0600546D RID: 21613 RVA: 0x00180750 File Offset: 0x0017E950
		public static float NormalizeAngle(float angle)
		{
			while (angle < 0f)
			{
				angle += 360f;
			}
			while (angle > 360f)
			{
				angle -= 360f;
			}
			return angle;
		}

		// Token: 0x0600546E RID: 21614 RVA: 0x00180779 File Offset: 0x0017E979
		public static float VectorToAngle(Vector2 vector)
		{
			if (Utility.IsZero(vector.x) && Utility.IsZero(vector.y))
			{
				return 0f;
			}
			return Utility.NormalizeAngle(Mathf.Atan2(vector.x, vector.y) * 57.29578f);
		}

		// Token: 0x0600546F RID: 21615 RVA: 0x001807B7 File Offset: 0x0017E9B7
		public static float Min(float v0, float v1)
		{
			if (v0 < v1)
			{
				return v0;
			}
			return v1;
		}

		// Token: 0x06005470 RID: 21616 RVA: 0x001807C0 File Offset: 0x0017E9C0
		public static float Max(float v0, float v1)
		{
			if (v0 > v1)
			{
				return v0;
			}
			return v1;
		}

		// Token: 0x06005471 RID: 21617 RVA: 0x001807CC File Offset: 0x0017E9CC
		public static float Min(float v0, float v1, float v2, float v3)
		{
			float num = (v0 >= v1) ? v1 : v0;
			float num2 = (v2 >= v3) ? v3 : v2;
			if (num < num2)
			{
				return num;
			}
			return num2;
		}

		// Token: 0x06005472 RID: 21618 RVA: 0x001807F4 File Offset: 0x0017E9F4
		public static float Max(float v0, float v1, float v2, float v3)
		{
			float num = (v0 <= v1) ? v1 : v0;
			float num2 = (v2 <= v3) ? v3 : v2;
			if (num > num2)
			{
				return num;
			}
			return num2;
		}

		// Token: 0x06005473 RID: 21619 RVA: 0x0018081C File Offset: 0x0017EA1C
		internal static float ValueFromSides(float negativeSide, float positiveSide)
		{
			float num = Utility.Abs(negativeSide);
			float num2 = Utility.Abs(positiveSide);
			if (Utility.Approximately(num, num2))
			{
				return 0f;
			}
			if (num <= num2)
			{
				return num2;
			}
			return -num;
		}

		// Token: 0x06005474 RID: 21620 RVA: 0x0018084E File Offset: 0x0017EA4E
		internal static float ValueFromSides(float negativeSide, float positiveSide, bool invertSides)
		{
			if (invertSides)
			{
				return Utility.ValueFromSides(positiveSide, negativeSide);
			}
			return Utility.ValueFromSides(negativeSide, positiveSide);
		}

		// Token: 0x06005475 RID: 21621 RVA: 0x00180862 File Offset: 0x0017EA62
		public static void ArrayResize<T>(ref T[] array, int capacity)
		{
			if (array == null || capacity > array.Length)
			{
				Array.Resize<T>(ref array, Utility.NextPowerOfTwo(capacity));
			}
		}

		// Token: 0x06005476 RID: 21622 RVA: 0x0018087B File Offset: 0x0017EA7B
		public static void ArrayExpand<T>(ref T[] array, int capacity)
		{
			if (array == null || capacity > array.Length)
			{
				array = new T[Utility.NextPowerOfTwo(capacity)];
			}
		}

		// Token: 0x06005477 RID: 21623 RVA: 0x00180895 File Offset: 0x0017EA95
		public static void ArrayAppend<T>(ref T[] array, T item)
		{
			if (array == null)
			{
				array = new T[1];
				array[0] = item;
				return;
			}
			Array.Resize<T>(ref array, array.Length + 1);
			array[array.Length - 1] = item;
		}

		// Token: 0x06005478 RID: 21624 RVA: 0x001808C7 File Offset: 0x0017EAC7
		public static void ArrayAppend<T>(ref T[] array, T[] items)
		{
			if (array == null)
			{
				array = new T[items.Length];
				Array.Copy(items, array, items.Length);
				return;
			}
			Array.Resize<T>(ref array, array.Length + items.Length);
			Array.ConstrainedCopy(items, 0, array, array.Length - items.Length, items.Length);
		}

		// Token: 0x06005479 RID: 21625 RVA: 0x00180905 File Offset: 0x0017EB05
		public static int NextPowerOfTwo(int value)
		{
			if (value > 0)
			{
				value--;
				value |= value >> 1;
				value |= value >> 2;
				value |= value >> 4;
				value |= value >> 8;
				value |= value >> 16;
				value++;
				return value;
			}
			return 0;
		}

		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x0600547A RID: 21626 RVA: 0x0018093C File Offset: 0x0017EB3C
		internal static bool Is32Bit
		{
			get
			{
				return IntPtr.Size == 4;
			}
		}

		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x0600547B RID: 21627 RVA: 0x00180946 File Offset: 0x0017EB46
		internal static bool Is64Bit
		{
			get
			{
				return IntPtr.Size == 8;
			}
		}

		// Token: 0x0600547C RID: 21628 RVA: 0x00180950 File Offset: 0x0017EB50
		public static string GetPlatformName(bool uppercase = true)
		{
			string windowsVersion = Utility.GetWindowsVersion();
			if (!uppercase)
			{
				return windowsVersion;
			}
			return windowsVersion.ToUpper();
		}

		// Token: 0x0600547D RID: 21629 RVA: 0x00180970 File Offset: 0x0017EB70
		private static string GetHumanUnderstandableWindowsVersion()
		{
			Version version = Environment.OSVersion.Version;
			if (version.Major == 6)
			{
				switch (version.Minor)
				{
				case 1:
					return "7";
				case 2:
					return "8";
				case 3:
					return "8.1";
				default:
					return "Vista";
				}
			}
			else
			{
				if (version.Major != 5)
				{
					return version.Major.ToString();
				}
				int minor = version.Minor;
				if (minor - 1 <= 1)
				{
					return "XP";
				}
				return "2000";
			}
		}

		// Token: 0x0600547E RID: 21630 RVA: 0x001809F8 File Offset: 0x0017EBF8
		public static string GetWindowsVersion()
		{
			string humanUnderstandableWindowsVersion = Utility.GetHumanUnderstandableWindowsVersion();
			string text = Utility.Is32Bit ? "32Bit" : "64Bit";
			return string.Concat(new string[]
			{
				"Windows ",
				humanUnderstandableWindowsVersion,
				" ",
				text,
				" Build ",
				Utility.GetSystemBuildNumber().ToString()
			});
		}

		// Token: 0x0600547F RID: 21631 RVA: 0x00180A5A File Offset: 0x0017EC5A
		public static int GetSystemBuildNumber()
		{
			return Environment.OSVersion.Version.Build;
		}

		// Token: 0x06005480 RID: 21632 RVA: 0x00180A6B File Offset: 0x0017EC6B
		public static void LoadScene(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		// Token: 0x06005481 RID: 21633 RVA: 0x00180A73 File Offset: 0x0017EC73
		internal static string PluginFileExtension()
		{
			return ".dll";
		}

		// Token: 0x040053A5 RID: 21413
		public const float Epsilon = 1E-07f;

		// Token: 0x040053A6 RID: 21414
		private static readonly Vector2[] circleVertexList = new Vector2[]
		{
			new Vector2(0f, 1f),
			new Vector2(0.2588f, 0.9659f),
			new Vector2(0.5f, 0.866f),
			new Vector2(0.7071f, 0.7071f),
			new Vector2(0.866f, 0.5f),
			new Vector2(0.9659f, 0.2588f),
			new Vector2(1f, 0f),
			new Vector2(0.9659f, -0.2588f),
			new Vector2(0.866f, -0.5f),
			new Vector2(0.7071f, -0.7071f),
			new Vector2(0.5f, -0.866f),
			new Vector2(0.2588f, -0.9659f),
			new Vector2(0f, -1f),
			new Vector2(-0.2588f, -0.9659f),
			new Vector2(-0.5f, -0.866f),
			new Vector2(-0.7071f, -0.7071f),
			new Vector2(-0.866f, -0.5f),
			new Vector2(-0.9659f, -0.2588f),
			new Vector2(-1f, --0f),
			new Vector2(-0.9659f, 0.2588f),
			new Vector2(-0.866f, 0.5f),
			new Vector2(-0.7071f, 0.7071f),
			new Vector2(-0.5f, 0.866f),
			new Vector2(-0.2588f, 0.9659f),
			new Vector2(0f, 1f)
		};
	}
}
