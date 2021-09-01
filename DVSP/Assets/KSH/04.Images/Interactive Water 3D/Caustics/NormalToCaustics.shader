Shader "Interactive Water 3D/Normal To Caustics" {
	Properties {
		_WaterNormalMap ("Water Normal", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _WaterNormalMap;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 oldpos : TEXCOORD0;
				float2 newpos : TEXCOORD1;
			};
			v2f vert (appdata_base v)
			{
				v2f o;
				float3 nrm = UnpackNormal(tex2Dlod(_WaterNormalMap, float4(v.texcoord.xy, 0, 0)));

				o.oldpos = v.vertex.xz;
				v.vertex.xz += nrm.xy * 0.15;
				o.newpos = v.vertex.xz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				float o = length(ddx(i.oldpos)) * length(ddy(i.oldpos));
				float n = length(ddx(i.newpos)) * length(ddy(i.newpos));
				float c = (o / n) * 0.5;
				return float4(c.xxx, 1);
			}
			ENDCG
		}
	}
	Fallback Off
}
