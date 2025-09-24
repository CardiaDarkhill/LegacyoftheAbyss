using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000049 RID: 73
[ExecuteInEditMode]
public class PlanarRealtimeReflection : MonoBehaviour
{
	// Token: 0x06000205 RID: 517 RVA: 0x0000C9A0 File Offset: 0x0000ABA0
	public void OnWillRenderObject()
	{
		if (!base.enabled || !base.GetComponent<Renderer>() || !base.GetComponent<Renderer>().sharedMaterial || !base.GetComponent<Renderer>().enabled)
		{
			return;
		}
		Camera current = Camera.current;
		if (!current)
		{
			return;
		}
		if (this.m_NormalsFromMesh && base.GetComponent<MeshFilter>() != null)
		{
			this.m_calculatedNormal = base.transform.TransformDirection(base.GetComponent<MeshFilter>().sharedMesh.normals[0]);
		}
		if (this.m_BaseClipOffsetFromMesh && base.GetComponent<MeshFilter>() != null)
		{
			this.m_finalClipPlaneOffset = (base.transform.position - base.transform.TransformPoint(base.GetComponent<MeshFilter>().sharedMesh.vertices[0])).magnitude + this.m_clipPlaneOffset;
		}
		else if (this.m_BaseClipOffsetFromMeshInverted && base.GetComponent<MeshFilter>() != null)
		{
			this.m_finalClipPlaneOffset = -(base.transform.position - base.transform.TransformPoint(base.GetComponent<MeshFilter>().sharedMesh.vertices[0])).magnitude + this.m_clipPlaneOffset;
		}
		else
		{
			this.m_finalClipPlaneOffset = this.m_clipPlaneOffset;
		}
		if (PlanarRealtimeReflection.s_InsideRendering)
		{
			return;
		}
		PlanarRealtimeReflection.s_InsideRendering = true;
		Camera camera;
		this.CreateSurfaceObjects(current, out camera);
		Vector3 position = base.transform.position;
		Vector3 vector = (this.m_NormalsFromMesh && base.GetComponent<MeshFilter>() != null) ? this.m_calculatedNormal : base.transform.up;
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		this.UpdateCameraModes(current, camera);
		float w = -Vector3.Dot(vector, position) - this.m_finalClipPlaneOffset;
		Vector4 plane = new Vector4(vector.x, vector.y, vector.z, w);
		Matrix4x4 zero = Matrix4x4.zero;
		PlanarRealtimeReflection.CalculateReflectionMatrix(ref zero, plane);
		Vector3 position2 = current.transform.position;
		Vector3 position3 = zero.MultiplyPoint(position2);
		camera.worldToCameraMatrix = current.worldToCameraMatrix * zero;
		Vector4 clipPlane = this.CameraSpacePlane(camera, position, vector, 1f);
		Matrix4x4 projectionMatrix = current.projectionMatrix;
		PlanarRealtimeReflection.CalculateObliqueMatrix(ref projectionMatrix, clipPlane);
		camera.projectionMatrix = projectionMatrix;
		camera.cullingMask = (-17 & this.m_ReflectLayers.value);
		camera.targetTexture = this.m_ReflectionTexture;
		GL.invertCulling = true;
		camera.transform.position = position3;
		Vector3 eulerAngles = current.transform.eulerAngles;
		camera.transform.eulerAngles = new Vector3(0f, eulerAngles.y, eulerAngles.z);
		camera.Render();
		camera.transform.position = position2;
		GL.invertCulling = false;
		Material[] sharedMaterials = base.GetComponent<Renderer>().sharedMaterials;
		foreach (Material material in sharedMaterials)
		{
			if (material.HasProperty("_ReflectionTex"))
			{
				material.SetTexture("_ReflectionTex", this.m_ReflectionTexture);
			}
		}
		Matrix4x4 lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
		Vector3 lossyScale = base.transform.lossyScale;
		Matrix4x4 matrix4x = base.transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z));
		matrix4x = lhs * current.projectionMatrix * current.worldToCameraMatrix * matrix4x;
		Material[] array = sharedMaterials;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetMatrix("_ProjMatrix", matrix4x);
		}
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}
		PlanarRealtimeReflection.s_InsideRendering = false;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000CD98 File Offset: 0x0000AF98
	private void OnDisable()
	{
		if (this.m_ReflectionTexture)
		{
			Object.DestroyImmediate(this.m_ReflectionTexture);
			this.m_ReflectionTexture = null;
		}
		foreach (object obj in this.m_ReflectionCameras)
		{
			Object.DestroyImmediate(((Camera)((DictionaryEntry)obj).Value).gameObject);
		}
		this.m_ReflectionCameras.Clear();
	}

	// Token: 0x06000207 RID: 519 RVA: 0x0000CE2C File Offset: 0x0000B02C
	private void UpdateCameraModes(Camera src, Camera dest)
	{
		if (dest == null)
		{
			return;
		}
		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = src.backgroundColor;
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox skybox = src.GetComponent(typeof(Skybox)) as Skybox;
			Skybox skybox2 = dest.GetComponent(typeof(Skybox)) as Skybox;
			if (!skybox || !skybox.material)
			{
				skybox2.enabled = false;
			}
			else
			{
				skybox2.enabled = true;
				skybox2.material = skybox.material;
			}
		}
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000CF0C File Offset: 0x0000B10C
	private void CreateSurfaceObjects(Camera currentCamera, out Camera reflectionCamera)
	{
		reflectionCamera = null;
		if (!this.m_ReflectionTexture || this.m_OldReflectionTextureSize != this.m_TextureResolution)
		{
			if (this.m_ReflectionTexture)
			{
				Object.DestroyImmediate(this.m_ReflectionTexture);
			}
			this.m_ReflectionTexture = new RenderTexture(this.m_TextureResolution, this.m_TextureResolution, 16);
			this.m_ReflectionTexture.name = "__SurfaceReflection" + base.GetInstanceID().ToString();
			this.m_ReflectionTexture.isPowerOfTwo = true;
			this.m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			this.m_OldReflectionTextureSize = this.m_TextureResolution;
		}
		reflectionCamera = (this.m_ReflectionCameras[currentCamera] as Camera);
		if (!reflectionCamera)
		{
			GameObject gameObject = new GameObject("Surface Refl Camera id" + base.GetInstanceID().ToString() + " for " + currentCamera.GetInstanceID().ToString(), new Type[]
			{
				typeof(Camera),
				typeof(Skybox)
			});
			reflectionCamera = gameObject.GetComponent<Camera>();
			reflectionCamera.enabled = false;
			reflectionCamera.transform.position = base.transform.position;
			reflectionCamera.transform.rotation = base.transform.rotation;
			reflectionCamera.gameObject.AddComponent<FlareLayer>();
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			this.m_ReflectionCameras[currentCamera] = reflectionCamera;
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000D081 File Offset: 0x0000B281
	private static float sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000D0A4 File Offset: 0x0000B2A4
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * this.m_finalClipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 vector = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector.x, vector.y, vector.z, -Vector3.Dot(lhs, vector));
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000D10C File Offset: 0x0000B30C
	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(PlanarRealtimeReflection.sgn(clipPlane.x), PlanarRealtimeReflection.sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000D1B8 File Offset: 0x0000B3B8
	private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
	}

	// Token: 0x040001AC RID: 428
	public bool m_DisablePixelLights = true;

	// Token: 0x040001AD RID: 429
	public int m_TextureResolution = 1024;

	// Token: 0x040001AE RID: 430
	public float m_clipPlaneOffset = 0.07f;

	// Token: 0x040001AF RID: 431
	private float m_finalClipPlaneOffset;

	// Token: 0x040001B0 RID: 432
	public bool m_NormalsFromMesh;

	// Token: 0x040001B1 RID: 433
	public bool m_BaseClipOffsetFromMesh;

	// Token: 0x040001B2 RID: 434
	public bool m_BaseClipOffsetFromMeshInverted;

	// Token: 0x040001B3 RID: 435
	private Vector3 m_calculatedNormal = Vector3.zero;

	// Token: 0x040001B4 RID: 436
	public LayerMask m_ReflectLayers = -1;

	// Token: 0x040001B5 RID: 437
	private Hashtable m_ReflectionCameras = new Hashtable();

	// Token: 0x040001B6 RID: 438
	private RenderTexture m_ReflectionTexture;

	// Token: 0x040001B7 RID: 439
	private int m_OldReflectionTextureSize;

	// Token: 0x040001B8 RID: 440
	private static bool s_InsideRendering;
}
