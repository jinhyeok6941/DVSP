using UnityEngine;

namespace InteractiveWater3D
{
	public class AutoMove : MonoBehaviour
	{
		[Header("Direction X")]
		public float m_AmplitudeX = 3f;
		[Range(0.02f, 0.2f)] public float m_VelocityX = 0.08f;
		private float m_AccumulatorX = 0f;
		[Header("Direction Y")]
		public float m_AmplitudeY = 3f;
		[Range(0.02f, 0.2f)] public float m_VelocityY = 0.08f;
		private float m_AccumulatorY = 0f;
		[Header("Direction Z")]
		public float m_AmplitudeZ = 3f;
		[Range(0.02f, 0.2f)] public float m_VelocityZ = 0.08f;
		private float m_AccumulatorZ = 0f;
		private Vector3 m_Orig;

		void Start()
		{
			m_Orig = transform.position;
		}
		void FixedUpdate()
		{
			m_AccumulatorX += m_VelocityX;
			m_AccumulatorY += m_VelocityY;
			m_AccumulatorZ += m_VelocityZ;
			float tX = Mathf.Sin(m_AccumulatorX);  // -1 ~ 1
			float tY = Mathf.Sin(m_AccumulatorY);  // -1 ~ 1
			float tZ = Mathf.Sin(m_AccumulatorZ);  // -1 ~ 1
			float dtX = m_Orig.x + tX * m_AmplitudeX;
			float dtY = m_Orig.y + tY * m_AmplitudeY;
			float dtZ = m_Orig.z + tZ * m_AmplitudeZ;
			transform.position = new Vector3(dtX, dtY, dtZ);
		}
	}
}