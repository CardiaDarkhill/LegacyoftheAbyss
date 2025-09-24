using System;
using UnityEngine;

// Token: 0x02000754 RID: 1876
public static class Collision2DUtils
{
	// Token: 0x06004296 RID: 17046 RVA: 0x00125B2C File Offset: 0x00123D2C
	public static Collision2DUtils.Collision2DSafeContact GetSafeContact(this Collision2D collision)
	{
		if (collision.GetContacts(Collision2DUtils.contactsBuffer) >= 1)
		{
			ContactPoint2D contactPoint2D = Collision2DUtils.contactsBuffer[0];
			return new Collision2DUtils.Collision2DSafeContact
			{
				Point = contactPoint2D.point,
				Normal = contactPoint2D.normal,
				IsLegitimate = true
			};
		}
		Vector2 b = collision.collider.transform.TransformPoint(collision.collider.offset);
		Vector2 a = collision.otherCollider.transform.TransformPoint(collision.otherCollider.offset);
		return new Collision2DUtils.Collision2DSafeContact
		{
			Point = (a + b) * 0.5f,
			Normal = (a - b).normalized,
			IsLegitimate = false
		};
	}

	// Token: 0x04004418 RID: 17432
	private static ContactPoint2D[] contactsBuffer = new ContactPoint2D[1];

	// Token: 0x02001A2A RID: 6698
	public struct Collision2DSafeContact
	{
		// Token: 0x040098D5 RID: 39125
		public Vector2 Point;

		// Token: 0x040098D6 RID: 39126
		public Vector2 Normal;

		// Token: 0x040098D7 RID: 39127
		public bool IsLegitimate;
	}
}
