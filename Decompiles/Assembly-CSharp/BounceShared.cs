using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200049E RID: 1182
public static class BounceShared
{
	// Token: 0x06002ADF RID: 10975 RVA: 0x000BA96E File Offset: 0x000B8B6E
	public static IEnumerator BouncePull(Transform transform, Vector2 heroBouncePos, HeroController hc, HitInstance hit)
	{
		Rigidbody2D body = hc.Body;
		Vector2 fromPos = body.position;
		Vector2 toPos = new Vector2(transform.position.x, heroBouncePos.y);
		bool flag = false;
		float bouncePullDuration = 0.03f;
		if (!hit.IsHarpoon && hit.IsNailTag)
		{
			string currentCrestID = hc.playerData.CurrentCrestID;
			if (!(currentCrestID == "Reaper"))
			{
				if (!(currentCrestID == "Wanderer"))
				{
					if (!(currentCrestID == "Toolmaster"))
					{
						if (!(currentCrestID == "Witch"))
						{
							if (currentCrestID == "Spell")
							{
								toPos.x = fromPos.x;
								toPos.y = Mathf.Clamp(toPos.y, fromPos.y - 1f, fromPos.y + 1f);
							}
						}
						else
						{
							bouncePullDuration = 0.08f;
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					toPos.x = fromPos.x;
				}
			}
			else
			{
				toPos.x = Mathf.Clamp(toPos.x, fromPos.x - 1f, fromPos.x + 1f);
				toPos.y = Mathf.Clamp(toPos.y, fromPos.y - 1.75f, fromPos.y + 1.75f);
			}
		}
		if (toPos.y < fromPos.y && !flag)
		{
			toPos.y = fromPos.y;
		}
		if (hc.cState.facingRight)
		{
			if (toPos.x < fromPos.x)
			{
				toPos.x = fromPos.x;
			}
		}
		else if (toPos.x > fromPos.x)
		{
			toPos.x = fromPos.x;
		}
		if (hc.Config.DownSlashType != HeroControllerConfig.DownSlashTypes.DownSpike)
		{
			hc.RelinquishControl();
			BounceShared.animationVersion = hc.StopAnimationControlVersioned();
			hc.AffectedByGravity(false);
			BounceShared.controlVersion = HeroController.ControlVersion;
			BounceShared.pullingCount++;
			tk2dSpriteAnimationClip clip = hc.GetComponent<HeroAnimationController>().GetClip("Pod Bounce");
			hc.GetComponent<tk2dSpriteAnimator>().PlayFromFrame(clip, 0);
			for (float elapsed = 0f; elapsed < bouncePullDuration; elapsed += Time.deltaTime)
			{
				Vector2 position = Vector2.Lerp(fromPos, toPos, elapsed / bouncePullDuration);
				body.MovePosition(position);
				yield return null;
			}
			if (BounceShared.controlVersion == HeroController.ControlVersion)
			{
				hc.RegainControl();
			}
			hc.StartAnimationControl(BounceShared.animationVersion);
			hc.AffectedByGravity(true);
			BounceShared.pullingCount--;
		}
		body.MovePosition(toPos);
		yield break;
	}

	// Token: 0x06002AE0 RID: 10976 RVA: 0x000BA994 File Offset: 0x000B8B94
	public static void OnBouncePullInterrupted()
	{
		if (BounceShared.pullingCount > 0)
		{
			BounceShared.pullingCount--;
			if (BounceShared.pullingCount == 0)
			{
				HeroController instance = HeroController.instance;
				if (instance != null)
				{
					if (BounceShared.controlVersion == HeroController.ControlVersion)
					{
						instance.RegainControl();
					}
					instance.StartAnimationControl(BounceShared.animationVersion);
					instance.AffectedByGravity(true);
				}
			}
		}
	}

	// Token: 0x04002BCF RID: 11215
	private const float BOUNCE_PULL_DURATION_DEFAULT = 0.03f;

	// Token: 0x04002BD0 RID: 11216
	private const float BOUNCE_PULL_DURATION_WITCH = 0.08f;

	// Token: 0x04002BD1 RID: 11217
	private const float REAPER_RANGE_X = 1f;

	// Token: 0x04002BD2 RID: 11218
	private const float REAPER_RANGE_Y = 1.75f;

	// Token: 0x04002BD3 RID: 11219
	private const float SHAMAN_RANGE_Y = 1f;

	// Token: 0x04002BD4 RID: 11220
	private static int pullingCount;

	// Token: 0x04002BD5 RID: 11221
	private static int controlVersion;

	// Token: 0x04002BD6 RID: 11222
	private static int animationVersion;
}
