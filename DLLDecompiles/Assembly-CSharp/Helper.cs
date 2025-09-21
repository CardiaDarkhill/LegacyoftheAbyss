﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x0200076C RID: 1900
public static class Helper
{
	// Token: 0x060043AF RID: 17327 RVA: 0x00129740 File Offset: 0x00127940
	public static int GetCollidingLayerMaskForLayer(int layer)
	{
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			if (!Physics2D.GetIgnoreLayerCollision(layer, i))
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	// Token: 0x060043B0 RID: 17328 RVA: 0x00129770 File Offset: 0x00127970
	public static float GetReflectedAngle(float angle, bool reflectHorizontal, bool reflectVertical, bool disallowNegative = false)
	{
		if (reflectHorizontal)
		{
			angle = 180f - angle;
		}
		if (reflectVertical)
		{
			angle = -angle;
		}
		while (angle > 360f)
		{
			angle -= 360f;
		}
		if (disallowNegative)
		{
			while (angle < 0f)
			{
				angle += 360f;
			}
		}
		else
		{
			while (angle < -360f)
			{
				angle += 360f;
			}
		}
		return angle;
	}

	// Token: 0x060043B1 RID: 17329 RVA: 0x001297D0 File Offset: 0x001279D0
	public static Vector3 GetRandomVector3InRange(Vector3 min, Vector3 max)
	{
		float x = (min.x != max.x) ? Random.Range(min.x, max.x) : min.x;
		float y = (min.y != max.y) ? Random.Range(min.y, max.y) : min.y;
		float z = (min.z != max.z) ? Random.Range(min.z, max.z) : min.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x060043B2 RID: 17330 RVA: 0x0012985C File Offset: 0x00127A5C
	public static Vector2 GetRandomVector2InRange(Vector2 min, Vector2 max)
	{
		float x = (min.x != max.x) ? Random.Range(min.x, max.x) : min.x;
		float y = (min.y != max.y) ? Random.Range(min.y, max.y) : min.y;
		return new Vector2(x, y);
	}

	// Token: 0x060043B3 RID: 17331 RVA: 0x001298C0 File Offset: 0x00127AC0
	public static bool IsRayHittingNoTriggers(Vector2 origin, Vector2 direction, float length, int layerMask, Func<Collider2D, bool> predicate, out RaycastHit2D closestHit)
	{
		Helper.IsHittingNoTriggersPre();
		int hitCount = Physics2D.RaycastNonAlloc(origin, direction, Helper._rayHitStore, length, layerMask);
		return Helper.IsHittingNoTriggersPost(predicate, out closestHit, hitCount);
	}

	// Token: 0x060043B4 RID: 17332 RVA: 0x001298EC File Offset: 0x00127AEC
	public static bool IsLineHittingNoTriggers(Vector2 from, Vector2 to, int layerMask, Func<Collider2D, bool> predicate, out RaycastHit2D closestHit)
	{
		Helper.IsHittingNoTriggersPre();
		int hitCount = Physics2D.LinecastNonAlloc(from, to, Helper._rayHitStore, layerMask);
		return Helper.IsHittingNoTriggersPost(predicate, out closestHit, hitCount);
	}

	// Token: 0x060043B5 RID: 17333 RVA: 0x00129915 File Offset: 0x00127B15
	private static void IsHittingNoTriggersPre()
	{
		if (Helper._rayHitStore == null)
		{
			Helper._rayHitStore = new RaycastHit2D[10];
		}
	}

	// Token: 0x060043B6 RID: 17334 RVA: 0x0012992C File Offset: 0x00127B2C
	private static bool IsHittingNoTriggersPost(Func<Collider2D, bool> predicate, out RaycastHit2D closestHit, int hitCount)
	{
		bool flag = predicate == null;
		bool flag2 = false;
		closestHit = default(RaycastHit2D);
		for (int i = 0; i < hitCount; i++)
		{
			RaycastHit2D raycastHit2D = Helper._rayHitStore[i];
			Collider2D collider = raycastHit2D.collider;
			if (!collider.isTrigger && (flag || predicate(collider)))
			{
				if (!flag2 || raycastHit2D.distance < closestHit.distance)
				{
					closestHit = raycastHit2D;
				}
				flag2 = true;
			}
			Helper._rayHitStore[i] = default(RaycastHit2D);
		}
		return flag2;
	}

	// Token: 0x060043B7 RID: 17335 RVA: 0x001299AB File Offset: 0x00127BAB
	public static bool IsRayHittingNoTriggers(Vector2 origin, Vector2 direction, float length, int layerMask, out RaycastHit2D closestHit)
	{
		return Helper.IsRayHittingNoTriggers(origin, direction, length, layerMask, null, out closestHit);
	}

	// Token: 0x060043B8 RID: 17336 RVA: 0x001299BC File Offset: 0x00127BBC
	public static bool IsRayHittingNoTriggers(Vector2 origin, Vector2 direction, float length, int layerMask)
	{
		RaycastHit2D raycastHit2D;
		return Helper.IsRayHittingNoTriggers(origin, direction, length, layerMask, out raycastHit2D);
	}

	// Token: 0x060043B9 RID: 17337 RVA: 0x001299D4 File Offset: 0x00127BD4
	public static RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, float distance)
	{
		Helper.IsHittingNoTriggersPre();
		if (Physics2D.RaycastNonAlloc(origin, direction, Helper._rayHitStore, distance) == 0)
		{
			return Helper.BLANK_HIT;
		}
		return Helper._rayHitStore[0];
	}

	// Token: 0x060043BA RID: 17338 RVA: 0x001299FB File Offset: 0x00127BFB
	public static RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, float distance, int layerMask)
	{
		Helper.IsHittingNoTriggersPre();
		if (Physics2D.RaycastNonAlloc(origin, direction, Helper._rayHitStore, distance, layerMask) == 0)
		{
			return Helper.BLANK_HIT;
		}
		return Helper._rayHitStore[0];
	}

	// Token: 0x060043BB RID: 17339 RVA: 0x00129A23 File Offset: 0x00127C23
	public static bool Raycast2DHit(Vector2 origin, Vector2 direction, float distance, int layerMask, out RaycastHit2D hit)
	{
		Helper.IsHittingNoTriggersPre();
		if (Physics2D.RaycastNonAlloc(origin, direction, Helper._rayHitStore, distance, layerMask) == 0)
		{
			hit = Helper.BLANK_HIT;
			return false;
		}
		hit = Helper._rayHitStore[0];
		return true;
	}

	// Token: 0x060043BC RID: 17340 RVA: 0x00129A5B File Offset: 0x00127C5B
	public static RaycastHit2D Raycast2D(Vector2 origin, Vector2 direction, float distance, int layerMask, float minDepth, float maxDepth)
	{
		Helper.IsHittingNoTriggersPre();
		if (Physics2D.RaycastNonAlloc(origin, direction, Helper._rayHitStore, distance, layerMask, minDepth, maxDepth) == 0)
		{
			return Helper.BLANK_HIT;
		}
		return Helper._rayHitStore[0];
	}

	// Token: 0x060043BD RID: 17341 RVA: 0x00129A88 File Offset: 0x00127C88
	public static ContactFilter2D CreateLegacyFilter(int layerMask, float minDepth, float maxDepth)
	{
		ContactFilter2D result = default(ContactFilter2D);
		result.useTriggers = Physics2D.queriesHitTriggers;
		result.SetLayerMask(layerMask);
		result.SetDepth(minDepth, maxDepth);
		return result;
	}

	// Token: 0x060043BE RID: 17342 RVA: 0x00129AC0 File Offset: 0x00127CC0
	public static ContactFilter2D CreateLegacyFilter(int layerMask)
	{
		ContactFilter2D result = default(ContactFilter2D);
		result.useTriggers = Physics2D.queriesHitTriggers;
		result.SetLayerMask(layerMask);
		return result;
	}

	// Token: 0x060043BF RID: 17343 RVA: 0x00129AF0 File Offset: 0x00127CF0
	public static RaycastHit2D LineCast2D(Vector2 start, Vector2 end, int layerMask)
	{
		Helper.IsHittingNoTriggersPre();
		ContactFilter2D legacy_FILTER = Helper.LEGACY_FILTER;
		legacy_FILTER.SetLayerMask(layerMask);
		if (Physics2D.Linecast(start, end, legacy_FILTER, Helper._rayHitStore) > 0)
		{
			return Helper._rayHitStore[0];
		}
		return Helper.BLANK_HIT;
	}

	// Token: 0x060043C0 RID: 17344 RVA: 0x00129B38 File Offset: 0x00127D38
	public static bool LineCast2DHit(Vector2 start, Vector2 end, int layerMask, out RaycastHit2D hit)
	{
		Helper.IsHittingNoTriggersPre();
		ContactFilter2D legacy_FILTER = Helper.LEGACY_FILTER;
		legacy_FILTER.SetLayerMask(layerMask);
		if (Physics2D.Linecast(start, end, legacy_FILTER, Helper._rayHitStore) > 0)
		{
			hit = Helper._rayHitStore[0];
			return true;
		}
		hit = Helper.BLANK_HIT;
		return false;
	}

	// Token: 0x060043C1 RID: 17345 RVA: 0x00129B8C File Offset: 0x00127D8C
	public static string CombinePaths(string path1, params string[] paths)
	{
		if (path1 == null)
		{
			throw new ArgumentNullException("path1");
		}
		if (paths == null)
		{
			throw new ArgumentNullException("paths");
		}
		return paths.Aggregate(path1, (string acc, string p) => Path.Combine(acc, p));
	}

	// Token: 0x060043C2 RID: 17346 RVA: 0x00129BDB File Offset: 0x00127DDB
	public static bool FileOrFolderExists(string path)
	{
		return File.Exists(path) || Directory.Exists(path);
	}

	// Token: 0x060043C3 RID: 17347 RVA: 0x00129BED File Offset: 0x00127DED
	public static void DeleteFileOrFolder(string path)
	{
		if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
		{
			Directory.Delete(path, true);
			return;
		}
		File.Delete(path);
	}

	// Token: 0x060043C4 RID: 17348 RVA: 0x00129C0C File Offset: 0x00127E0C
	public static void CopyFileOrFolder(string src, string dest)
	{
		if ((File.GetAttributes(src) & FileAttributes.Directory) == FileAttributes.Directory)
		{
			DirectoryInfo source = new DirectoryInfo(src);
			DirectoryInfo target = Directory.Exists(dest) ? new DirectoryInfo(dest) : Directory.CreateDirectory(dest);
			Helper.DeepCopy(source, target);
			return;
		}
		File.Copy(src, dest);
	}

	// Token: 0x060043C5 RID: 17349 RVA: 0x00129C54 File Offset: 0x00127E54
	public static void DeepCopy(DirectoryInfo source, DirectoryInfo target)
	{
		foreach (DirectoryInfo directoryInfo in source.GetDirectories())
		{
			Helper.DeepCopy(directoryInfo, target.CreateSubdirectory(directoryInfo.Name));
		}
		foreach (FileInfo fileInfo in source.GetFiles())
		{
			fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name));
		}
	}

	// Token: 0x060043C6 RID: 17350 RVA: 0x00129CC0 File Offset: 0x00127EC0
	public static void MoveFileOrFolder(string src, string dest)
	{
		if ((File.GetAttributes(src) & FileAttributes.Directory) == FileAttributes.Directory)
		{
			Directory.Move(src, dest);
			return;
		}
		File.Copy(src, dest);
	}

	// Token: 0x060043C7 RID: 17351 RVA: 0x00129CE0 File Offset: 0x00127EE0
	public static bool CheckMatchingSearchFilter(string text, string filter)
	{
		text = text.ToLower();
		filter = filter.ToLower().Replace('_', ' ');
		return filter.Split(' ', StringSplitOptions.None).All((string f) => text.Contains(f));
	}

	// Token: 0x060043C8 RID: 17352 RVA: 0x00129D36 File Offset: 0x00127F36
	public static string ParseSearchString(string original)
	{
		if (string.IsNullOrEmpty(original))
		{
			return null;
		}
		return original.Trim().ToLower().Replace(" ", "");
	}

	// Token: 0x060043C9 RID: 17353 RVA: 0x00129D5C File Offset: 0x00127F5C
	public static float LinearToDecibel(float sliderValue)
	{
		return Helper.LinearToDecibelExponential(sliderValue);
	}

	// Token: 0x060043CA RID: 17354 RVA: 0x00129D64 File Offset: 0x00127F64
	public static float DecibelToLinear(float dB)
	{
		return Helper.DecibelToLinearExponential(dB);
	}

	// Token: 0x060043CB RID: 17355 RVA: 0x00129D6C File Offset: 0x00127F6C
	private static float LinearToDecibelLog(float sliderValue)
	{
		sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1.2f);
		if (sliderValue > 1f)
		{
			return Mathf.Clamp((sliderValue - 1f) * 100f, 0f, 20f);
		}
		return Mathf.Clamp(Mathf.Log10(sliderValue) * 20f, -80f, 0f);
	}

	// Token: 0x060043CC RID: 17356 RVA: 0x00129DCB File Offset: 0x00127FCB
	private static float DecibelToLinearLog(float dB)
	{
		if (dB > 0f)
		{
			return Mathf.Clamp(1f + dB / 100f, 1f, 1.2f);
		}
		return Mathf.Clamp01(Mathf.Pow(10f, dB / 20f));
	}

	// Token: 0x060043CD RID: 17357 RVA: 0x00129E08 File Offset: 0x00128008
	public static float LinearToDecibelExponential(float sliderValue)
	{
		sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1.2f);
		if (sliderValue > 1f)
		{
			sliderValue -= 1f;
			return sliderValue * 100f;
		}
		return Mathf.Lerp(-80f, 0f, Mathf.Sqrt(sliderValue));
	}

	// Token: 0x060043CE RID: 17358 RVA: 0x00129E58 File Offset: 0x00128058
	public static float DecibelToLinearExponential(float dB)
	{
		if (dB > 0f)
		{
			return Mathf.Clamp(1f + dB / 100f, 1f, 1.2f);
		}
		return Mathf.Clamp01(Mathf.Pow((dB + 80f) / 80f, 2f));
	}

	// Token: 0x060043CF RID: 17359 RVA: 0x00129EA6 File Offset: 0x001280A6
	[Conditional("UNITY_EDITOR")]
	public static void RecordUndoChanges(this Object obj, string name = "Undo")
	{
	}

	// Token: 0x060043D0 RID: 17360 RVA: 0x00129EA8 File Offset: 0x001280A8
	[Conditional("UNITY_EDITOR")]
	public static void RegisterCreatedObjectUndo(this Object obj, string name = "Undo")
	{
	}

	// Token: 0x060043D1 RID: 17361 RVA: 0x00129EAA File Offset: 0x001280AA
	[Conditional("UNITY_EDITOR")]
	public static void ApplyPrefabInstanceModifications(this Object obj)
	{
	}

	// Token: 0x060043D2 RID: 17362 RVA: 0x00129EAC File Offset: 0x001280AC
	[Conditional("UNITY_EDITOR")]
	public static void SetAssetDirty(this Object obj)
	{
	}

	// Token: 0x060043D3 RID: 17363 RVA: 0x00129EAE File Offset: 0x001280AE
	public static Color SetAlpha(this Color color, float alpha)
	{
		color.a = alpha;
		return color;
	}

	// Token: 0x060043D4 RID: 17364 RVA: 0x00129EBC File Offset: 0x001280BC
	public static int GetClosestOffsetToIndex(int targetIndex, int currentIndex, int arrayLength)
	{
		if (arrayLength == 0)
		{
			Debug.LogError("Array length cannot be zero.");
			return 0;
		}
		int num = (targetIndex - currentIndex + arrayLength) % arrayLength;
		int num2 = (currentIndex - targetIndex + arrayLength) % arrayLength;
		if (num >= num2)
		{
			return -num2;
		}
		return num;
	}

	// Token: 0x060043D5 RID: 17365 RVA: 0x00129EF0 File Offset: 0x001280F0
	public static int GetContentHash<T>(this ICollection<T> collection)
	{
		if (collection == null)
		{
			return 0;
		}
		int num = 17;
		foreach (T t in collection)
		{
			num = num * 31 + ((t == null) ? 0 : t.GetHashCode());
		}
		return num;
	}

	// Token: 0x060043D6 RID: 17366 RVA: 0x00129F58 File Offset: 0x00128158
	public static int GetContentHash<T>(this List<T> collection)
	{
		if (collection == null)
		{
			return 0;
		}
		int num = 17;
		for (int i = 0; i < collection.Count; i++)
		{
			T t = collection[i];
			num = num * 31 + ((t == null) ? 0 : t.GetHashCode());
		}
		return num;
	}

	// Token: 0x060043D7 RID: 17367 RVA: 0x00129FA8 File Offset: 0x001281A8
	public static int GetContentHash<T>(this T[] collection)
	{
		if (collection == null)
		{
			return 0;
		}
		int num = 17;
		foreach (T t in collection)
		{
			num = num * 31 + ((t == null) ? 0 : t.GetHashCode());
		}
		return num;
	}

	// Token: 0x060043D8 RID: 17368 RVA: 0x00129FF4 File Offset: 0x001281F4
	public static T[] SafeCastToArray<T>(this object[] source) where T : class
	{
		if (source == null)
		{
			return null;
		}
		List<T> list = new List<T>(source.Length);
		for (int i = 0; i < source.Length; i++)
		{
			T t = source[i] as T;
			if (t != null)
			{
				list.Add(t);
			}
		}
		return list.ToArray();
	}

	// Token: 0x060043D9 RID: 17369 RVA: 0x0012A042 File Offset: 0x00128242
	public static StringBuilder GetTempStringBuilder()
	{
		return Helper.GetTempStringBuilder(string.Empty);
	}

	// Token: 0x060043DA RID: 17370 RVA: 0x0012A04E File Offset: 0x0012824E
	public static StringBuilder GetTempStringBuilder(string initialString)
	{
		if (Helper._tempStringBuilder == null)
		{
			Helper._tempStringBuilder = new StringBuilder(initialString);
		}
		else
		{
			Helper._tempStringBuilder.Clear();
			Helper._tempStringBuilder.Append(initialString);
		}
		return Helper._tempStringBuilder;
	}

	// Token: 0x060043DB RID: 17371 RVA: 0x0012A080 File Offset: 0x00128280
	public static double Max(double a, double b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	// Token: 0x0400452A RID: 17706
	private static RaycastHit2D[] _rayHitStore;

	// Token: 0x0400452B RID: 17707
	private static readonly RaycastHit2D BLANK_HIT = default(RaycastHit2D);

	// Token: 0x0400452C RID: 17708
	private static ContactFilter2D LEGACY_FILTER = Helper.CreateLegacyFilter(-1);

	// Token: 0x0400452D RID: 17709
	private static StringBuilder _tempStringBuilder;
}
