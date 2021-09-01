using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace InteractiveWater3D
{
	public class CausticsRenderer : MonoBehaviour
	{
		public IW3D m_Iw3D;
		public float m_Width = 10f;
		public float m_Intensity = 1f;
		public Material m_Mat;
		Mesh m_Mesh;
		Camera m_Camera;
		RenderTexture m_Rt;
		CommandBuffer m_CmdBuf;

		void Start()
		{
			m_Camera = gameObject.AddComponent<Camera>();
			m_Camera.aspect = 1f;
			m_Camera.backgroundColor = Color.black;
			m_Camera.depth = 0;
			m_Camera.farClipPlane = 5;
			m_Camera.nearClipPlane = -5;
			m_Camera.orthographic = true;
			m_Camera.orthographicSize = m_Width * 0.5f;
			m_Camera.clearFlags = CameraClearFlags.SolidColor;
			m_Camera.allowHDR = m_Camera.useOcclusionCulling = false;
			m_Camera.backgroundColor = Color.black;
			m_Camera.cullingMask = 0;

			m_Rt = new RenderTexture(512, 512, 16);
			m_Rt.name = "[Caustic]";
			m_Camera.targetTexture = m_Rt;

			m_CmdBuf = new CommandBuffer();
			m_CmdBuf.name = "[Caustic]";
			m_Camera.AddCommandBuffer(CameraEvent.AfterImageEffectsOpaque, m_CmdBuf);

			// need a good tessellation plane
			m_Mesh = IW3DUtils.CreateGridPlaneMesh(256, m_Width, m_Width, new Vector3(-5f, 0, -5f));
			m_Mesh.name = "[Caustic]";
		}
		void OnPostRender()
		{
			m_Mat.SetTexture("_WaterNormalMap", m_Iw3D.m_RTNormal);

			Matrix4x4 trs = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
			m_CmdBuf.Clear();
			m_CmdBuf.ClearRenderTarget(true, true, Color.black);
			m_CmdBuf.SetRenderTarget(m_Rt);
			m_CmdBuf.DrawMesh(m_Mesh, trs, m_Mat);

			Vector4 plane = new Vector4(0, 1, 0, Vector3.Dot(new Vector3(0, 1, 0), transform.position));
			Vector4 range = new Vector4(transform.position.x, transform.position.z, m_Width * 0.5f, m_Width * 0.5f);
			Shader.SetGlobalTexture("_Global_CausticTex", m_Rt);
			Shader.SetGlobalVector("_Global_CausticPlane", plane);
			Shader.SetGlobalVector("_Global_CausticRange", range);
			Shader.SetGlobalFloat("_Global_CausticIntensity", m_Intensity);
		}
		void OnDestroy()
		{
			if (m_Rt)
			{
				Destroy(m_Rt);
				m_Rt = null;
			}
			if (m_Mesh)
			{
				Destroy(m_Mesh);
				m_Mesh = null;
			}
			if (m_CmdBuf != null)
			{
				m_CmdBuf.Release();
				m_CmdBuf = null;
			}
		}
//		void OnGUI()
//		{
//			GUI.DrawTextureWithTexCoords(new Rect(10, 30, 64, 64), m_Rt, new Rect(0, 0, 1, 1));
//		}
	}
}