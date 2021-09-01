Shader "Interactive Water 3D/UnderWater" {
	Properties {
		_MainTex     ("Albedo", 2D) = "white" {}
		_BumpTex     ("Bump", 2D) = "bump" {}
		_Smoothness  ("Smoothness", Range(0, 1)) = 0
		_Metallic    ("Metallic", Range(0, 1)) = 0
		[Toggle(CAUSTIC)] _CAUSTIC ("Caustic", Float) = 1
		[Header(UnderWaterColor)]
		_WaterColor      ("Water Color", Color) = (1, 1, 1, 1)
		_WaterPlane      ("Water Plane Y", Float) = 0
		_WaterColorStart ("Water Color Start", Float) = 0.1
		_WaterColorEnd   ("Water Color Deepest", Float) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard addshadow fullforwardshadows
		#pragma shader_feature CAUSTIC
		#include "Utils.cginc"

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpTex;
			float3 worldPos;
		};

		sampler2D _MainTex, _BumpTex;
		float _Metallic, _Smoothness, _WaterPlane, _WaterColorStart, _WaterColorEnd;
		fixed4 _WaterColor;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float3 wldpos = IN.worldPos;
#if CAUSTIC
			// under water caustic
			float3 caustic = SampleCaustic(wldpos.xyz, float3(0, -1, 0));
#else
			float3 caustic = 0;
#endif
			// under water color
			float3 waterColor = 1.0;
			if (wldpos.y < _WaterPlane)
			{
				float f = clamp(_WaterPlane - wldpos.y, _WaterColorStart, _WaterColorEnd);
				waterColor = lerp(1.0, _WaterColor, f);
			}

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_BumpTex, IN.uv_BumpTex));
			o.Albedo = caustic + c.rgb * waterColor;
			o.Alpha = c.a;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
