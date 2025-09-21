using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C39 RID: 3129
	public class FindFastTravelTeleportPoint : FsmStateAction
	{
		// Token: 0x06005F1B RID: 24347 RVA: 0x001E2549 File Offset: 0x001E0749
		public override void Reset()
		{
			this.RelativeTo = null;
			this.Position = null;
			this.StoreGroundPos = null;
			this.StoreRoofHeight = null;
			this.StoreRightClear = null;
			this.StoreLeftClear = null;
			this.IsClearEvent = null;
			this.NotClearEvent = null;
		}

		// Token: 0x06005F1C RID: 24348 RVA: 0x001E2584 File Offset: 0x001E0784
		public override void OnEnter()
		{
			GameObject safe = this.RelativeTo.GetSafe(this);
			Vector2 value;
			float value2;
			bool value3;
			bool value4;
			bool flag = FindFastTravelTeleportPoint.IsClear(safe ? safe.transform.TransformPoint(this.Position.Value) : this.Position.Value, out value, out value2, out value3, out value4, false);
			this.StoreRoofHeight.Value = value2;
			this.StoreRightClear.Value = value3;
			this.StoreLeftClear.Value = value4;
			this.StoreGroundPos.Value = value;
			base.Fsm.Event(flag ? this.IsClearEvent : this.NotClearEvent);
			base.Finish();
		}

		// Token: 0x06005F1D RID: 24349 RVA: 0x001E2638 File Offset: 0x001E0838
		public static bool IsClear(Vector2 pos, out Vector2 groundPos, out float roofHeight, out bool isRightClear, out bool isLeftClear, bool drawDebugLines)
		{
			isRightClear = false;
			isLeftClear = false;
			roofHeight = 0f;
			if (!FindFastTravelTeleportPoint.TryGetGroundPos(pos, out groundPos, drawDebugLines))
			{
				return false;
			}
			Vector2 origin = groundPos;
			origin.y += 0.3f;
			if (!FindFastTravelTeleportPoint.CheckRoofHeight(groundPos + new Vector2(-0.3f, 0f), out roofHeight, drawDebugLines))
			{
				return false;
			}
			if (!FindFastTravelTeleportPoint.CheckRoofHeight(groundPos + new Vector2(0.3f, 0f), out roofHeight, drawDebugLines))
			{
				return false;
			}
			isLeftClear = FindFastTravelTeleportPoint.IsClearDirectional(origin, -1f, 2.75f, 256, drawDebugLines);
			isRightClear = FindFastTravelTeleportPoint.IsClearDirectional(origin, 1f, 2.75f, 256, drawDebugLines);
			return isLeftClear | isRightClear;
		}

		// Token: 0x06005F1E RID: 24350 RVA: 0x001E26FC File Offset: 0x001E08FC
		private static bool TryGetGroundPos(Vector2 pos, out Vector2 groundPos, bool drawDebugLines)
		{
			RaycastHit2D raycastHit2D;
			bool flag = Helper.IsRayHittingNoTriggers(pos, Vector2.down, 5f, 256, out raycastHit2D);
			if (NoTeleportRegion.GetTeleportBlockedState(pos) == NoTeleportRegion.TeleportAllowState.Blocked)
			{
				flag = false;
			}
			if (!flag)
			{
				if (drawDebugLines)
				{
					FindFastTravelTeleportPoint.DrawDebugLine(pos, pos + Vector2.down * 5f, Color.red);
				}
				groundPos = Vector2.zero;
				return false;
			}
			groundPos = raycastHit2D.point;
			if (drawDebugLines)
			{
				FindFastTravelTeleportPoint.DrawDebugLine(pos, groundPos, Color.yellow);
			}
			return true;
		}

		// Token: 0x06005F1F RID: 24351 RVA: 0x001E2784 File Offset: 0x001E0984
		private static bool CheckRoofHeight(Vector2 groundPos, out float roofHeight, bool drawDebugLines)
		{
			groundPos.y += Physics2D.defaultContactOffset * 2f;
			RaycastHit2D raycastHit2D;
			bool flag = Helper.IsRayHittingNoTriggers(groundPos, Vector2.up, 4f, 256, out raycastHit2D);
			if (drawDebugLines)
			{
				FindFastTravelTeleportPoint.DrawDebugLine(groundPos, flag ? raycastHit2D.point : (groundPos + Vector2.up * 4f), flag ? Color.red : Color.blue);
			}
			NoTeleportRegion.TeleportAllowState teleportBlockedState = NoTeleportRegion.GetTeleportBlockedState(groundPos);
			bool flag2 = teleportBlockedState == NoTeleportRegion.TeleportAllowState.Blocked || (teleportBlockedState != NoTeleportRegion.TeleportAllowState.Allowed && flag);
			flag = flag2;
			roofHeight = (flag ? raycastHit2D.distance : 10f);
			return !flag;
		}

		// Token: 0x06005F20 RID: 24352 RVA: 0x001E282C File Offset: 0x001E0A2C
		private static bool IsClearDirectional(Vector2 origin, float direction, float length, int layerMask, bool drawDebugLines)
		{
			Vector2 vector = new Vector2(direction, 0f);
			bool flag = Helper.IsRayHittingNoTriggers(origin, vector, length, layerMask);
			if (drawDebugLines)
			{
				FindFastTravelTeleportPoint.DrawDebugLine(origin, origin + vector * length, Color.yellow);
			}
			if (flag)
			{
				return false;
			}
			try
			{
				while (length > 0f)
				{
					Vector2 vector2 = origin + vector * length;
					int num = Physics2D.RaycastNonAlloc(vector2, Vector2.down, FindFastTravelTeleportPoint._rayHitStoreA, 1.8f, 256);
					bool flag2 = false;
					RaycastHit2D raycastHit2D = default(RaycastHit2D);
					for (int i = 0; i < Mathf.Min(num, FindFastTravelTeleportPoint._rayHitStoreA.Length); i++)
					{
						RaycastHit2D raycastHit2D2 = FindFastTravelTeleportPoint._rayHitStoreA[i];
						if (!raycastHit2D2.collider.isTrigger && raycastHit2D2.distance <= 0.5f)
						{
							flag2 = true;
							raycastHit2D = raycastHit2D2;
							break;
						}
					}
					length -= 0.5f;
					if (drawDebugLines)
					{
						FindFastTravelTeleportPoint.DrawDebugLine(vector2, vector2 + Vector2.down * 1.8f, Color.yellow);
					}
					if (!flag2)
					{
						return false;
					}
					NoTeleportRegion.TeleportAllowState teleportBlockedState = NoTeleportRegion.GetTeleportBlockedState(vector2);
					if (teleportBlockedState == NoTeleportRegion.TeleportAllowState.Blocked)
					{
						return false;
					}
					if (teleportBlockedState != NoTeleportRegion.TeleportAllowState.Allowed)
					{
						Vector2 vector3 = raycastHit2D.point - new Vector2(0f, 1.5f);
						int num2 = num;
						int a = Physics2D.RaycastNonAlloc(vector3, Vector2.up, FindFastTravelTeleportPoint._rayHitStoreB, 1.4f, 256);
						bool flag3 = false;
						RaycastHit2D raycastHit2D3 = default(RaycastHit2D);
						for (int j = 0; j < Mathf.Min(a, FindFastTravelTeleportPoint._rayHitStoreB.Length); j++)
						{
							RaycastHit2D raycastHit2D4 = FindFastTravelTeleportPoint._rayHitStoreB[j];
							if (!raycastHit2D4.collider.isTrigger)
							{
								flag3 = true;
								raycastHit2D3 = raycastHit2D4;
								bool flag4 = false;
								for (int k = 0; k < Mathf.Min(num, FindFastTravelTeleportPoint._rayHitStoreA.Length); k++)
								{
									RaycastHit2D raycastHit2D5 = FindFastTravelTeleportPoint._rayHitStoreA[k];
									if (!(raycastHit2D5.collider != raycastHit2D4.collider) && Math.Abs(raycastHit2D5.point.y - raycastHit2D4.point.y) >= 0.01f)
									{
										flag4 = true;
										break;
									}
								}
								if (flag4)
								{
									num2--;
								}
							}
						}
						if (num2 > 0)
						{
							flag3 = false;
						}
						if (drawDebugLines)
						{
							FindFastTravelTeleportPoint.DrawDebugLine(vector3, vector3 + Vector2.up * 1.4f, new Color(1f, 1f, 0f, 0.5f));
						}
						if (flag3)
						{
							if (drawDebugLines)
							{
								FindFastTravelTeleportPoint.DrawDebugLine(vector3, raycastHit2D3.point, Color.red);
							}
							return false;
						}
					}
				}
			}
			finally
			{
				for (int l = 0; l < FindFastTravelTeleportPoint._rayHitStoreA.Length; l++)
				{
					FindFastTravelTeleportPoint._rayHitStoreA[l] = default(RaycastHit2D);
				}
				for (int m = 0; m < FindFastTravelTeleportPoint._rayHitStoreB.Length; m++)
				{
					FindFastTravelTeleportPoint._rayHitStoreB[m] = default(RaycastHit2D);
				}
			}
			return true;
		}

		// Token: 0x06005F21 RID: 24353 RVA: 0x001E2B34 File Offset: 0x001E0D34
		private static void DrawDebugLine(Vector2 startPoint, Vector2 endPoint, Color color)
		{
			if (FindFastTravelTeleportPoint._debugLineMaterial == null)
			{
				FindFastTravelTeleportPoint._debugLineMaterial = new Material(Shader.Find("Sprites/Default"));
			}
			GL.PushMatrix();
			FindFastTravelTeleportPoint._debugLineMaterial.SetPass(0);
			GL.Begin(1);
			GL.Color(color);
			GL.Vertex(startPoint);
			GL.Vertex(endPoint);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x04005C05 RID: 23557
		private const int LAYER_MASK = 256;

		// Token: 0x04005C06 RID: 23558
		private const float GROUND_PADDING = 0.3f;

		// Token: 0x04005C07 RID: 23559
		private const float GROUND_RAY_SPACING = 0.5f;

		// Token: 0x04005C08 RID: 23560
		private const float GROUND_WIDTH = 5.5f;

		// Token: 0x04005C09 RID: 23561
		private const float ROOF_HEIGHT = 4f;

		// Token: 0x04005C0A RID: 23562
		private const float GROUND_DISTANCE = 0.5f;

		// Token: 0x04005C0B RID: 23563
		private const float GROUND_THICKNESS = 1.5f;

		// Token: 0x04005C0C RID: 23564
		private const float GROUND_THICKNESS_PADDING = 0.1f;

		// Token: 0x04005C0D RID: 23565
		public FsmOwnerDefault RelativeTo;

		// Token: 0x04005C0E RID: 23566
		public FsmVector2 Position;

		// Token: 0x04005C0F RID: 23567
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreGroundPos;

		// Token: 0x04005C10 RID: 23568
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreRoofHeight;

		// Token: 0x04005C11 RID: 23569
		[UIHint(UIHint.Variable)]
		public FsmBool StoreRightClear;

		// Token: 0x04005C12 RID: 23570
		[UIHint(UIHint.Variable)]
		public FsmBool StoreLeftClear;

		// Token: 0x04005C13 RID: 23571
		public FsmEvent IsClearEvent;

		// Token: 0x04005C14 RID: 23572
		public FsmEvent NotClearEvent;

		// Token: 0x04005C15 RID: 23573
		private static Material _debugLineMaterial;

		// Token: 0x04005C16 RID: 23574
		private static RaycastHit2D[] _rayHitStoreA = new RaycastHit2D[10];

		// Token: 0x04005C17 RID: 23575
		private static RaycastHit2D[] _rayHitStoreB = new RaycastHit2D[10];
	}
}
