using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class Turret : MonoBehaviour
{
	// Token: 0x060000B5 RID: 181 RVA: 0x000052DA File Offset: 0x000034DA
	private void Awake()
	{
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x000052DC File Offset: 0x000034DC
	private void Update()
	{
		Plane plane = new Plane(Vector3.up, base.transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		if (plane.Raycast(ray, out distance))
		{
			Quaternion to = Quaternion.LookRotation(Vector3.Normalize(ray.GetPoint(distance) - base.transform.position));
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 360f * Time.deltaTime);
			if (Input.GetMouseButtonDown(0))
			{
				this.bulletPrefab.Spawn(this.gun.position, this.gun.rotation);
			}
			if (Input.GetMouseButtonDown(1))
			{
				this.testPrefab.Spawn(this.gun.position, this.gun.rotation);
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.bulletPrefab.DestroyPooled<Bullet>();
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			this.bulletPrefab.DestroyAll<Bullet>();
		}
	}

	// Token: 0x040000A2 RID: 162
	public Bullet bulletPrefab;

	// Token: 0x040000A3 RID: 163
	public Transform gun;

	// Token: 0x040000A4 RID: 164
	public GameObject testPrefab;
}
