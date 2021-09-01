Shader "Interactive Water 3D/Water" {
	Properties {
		[NoScaleOffset] _HeightMap     ("Height", 2D) = "black" {}
		[NoScaleOffset] _NormalMap     ("Normal", 2D) = "black" {}
		[NoScaleOffset] _ReflectionMap ("Reflection", 2D) = "black" {}
		_ReflectionStrength            ("Reflection Strength", Float) = 1
		[NoScaleOffset] _RefractionMap ("Refraction", 2D) = "black" {}
		_RefractionStrength            ("Refraction Strength", Float) = 1
//		[NoScaleOffset] _FresnelMap    ("Fresnel", 2D) = "black" {}
		_HeightScale                   ("Height Scale", Float) = 0.02
		_Diffuse                       ("Diffuse", Color) = (0, 0.5, 1, 1)
		_Specular                      ("Specular", Color) = (1, 1, 1, 1)
		_SpecularPower                 ("Specular Power", Float) = 256
		_Transparence                  ("Transparence", Range(0, 1)) = 1
		_Blend                         ("Blend", Range(0, 1)) = 0.5
		[Header(Distortion)]
		_DistortMap       ("Distort", 2D) = "black" {}
		_DistortStrength  ("Distort Strength", Float) = 1
		_DistortSpeed     ("Distort Speed", Vector) = (1, 1, 0, 0)
	}
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Pass {
			Zwrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ IW3D_USE_VTF
			#include "UnityCG.cginc"
			#include "Utils.cginc"

			sampler2D _HeightMap, _NormalMap, _ReflectionMap, _RefractionMap, _FresnelMap, _DistortMap;
			float4x4 _OrthCamViewProj;
			float4 _Diffuse, _Specular, _DistortSpeed, _DistortMap_ST;
			float _HeightScale, _Transparence, _SpecularPower, _ReflectionStrength, _RefractionStrength, _DistortStrength, _Blend;

			struct v2f
			{
				float4 pos     : SV_POSITION;
				float2 uv      : TEXCOORD0;
				float3 wldpos  : TEXCOORD1;
				float3 wldview : TEXCOORD2;
				float3 wldlit  : TEXCOORD3;
				float4 ref     : TEXCOORD4;
				float2 dsttUv  : TEXCOORD5;
			};
			v2f vert (appdata_base v)
			{
				float4x4 orthMVP = mul(_OrthCamViewProj, unity_ObjectToWorld);
				float4 proj = mul(orthMVP, v.vertex);
				float2 uv = 0.5 * proj.xy / proj.w + 0.5;
#if UNITY_UV_STARTS_AT_TOP
				uv.y = 1 - uv.y;
#endif
#ifdef IW3D_USE_VTF
				float height = 0;
				height = HEIGHT_MAP_READ_LOD(_HeightMap, uv) * _HeightScale;
				float3 newPos = v.vertex.xyz + float3(0, height, 0);
#else
				float3 newPos = v.vertex.xyz;
#endif
				v2f o;
				o.pos = UnityObjectToClipPos(float4(newPos,1.0));
				o.uv = uv;
				o.wldpos = mul(unity_ObjectToWorld, newPos);   // the modified vertex world position
				o.wldview = WorldSpaceViewDir(v.vertex);
				o.wldlit = WorldSpaceLightDir(v.vertex);
				o.ref = ComputeScreenPos(o.pos);

				// water distortion uv
				o.dsttUv = TRANSFORM_TEX(v.texcoord, _DistortMap);
				o.dsttUv.x += _Time.x * _DistortSpeed.x;
				o.dsttUv.y += _Time.x * _DistortSpeed.y;
				return o;
			}
			float4 frag (v2f i) : COLOR
			{
				float2 dstt = tex2D(_DistortMap, i.dsttUv).rg - 0.5;
				dstt *= _DistortStrength;

				float4 uv1 = i.ref;
				uv1.xy += dstt;
				float4 refl = tex2Dproj(_ReflectionMap, UNITY_PROJ_COORD(uv1)) * _ReflectionStrength;

				float4 uv2 = i.ref;
				uv2.xy += dstt;
				float4 refr = tex2Dproj(_RefractionMap, UNITY_PROJ_COORD(uv2)) * _RefractionStrength;

				float4 nrmMap = tex2D(_NormalMap, i.uv);
				float3 N = (nrmMap.rgb - 0.5) * 2.0;
				float3 L = normalize(i.wldlit);

				float3 incident = normalize(i.wldpos.xyz - _WorldSpaceCameraPos.xyz);
				float3 R = normalize(reflect(incident, N));

//				float fresnelFac = dot(incident, N);
//				float fresnel = UNITY_SAMPLE_1CHANNEL(_FresnelMap, fresnelFac.xx);
//				float4 c2 = lerp(refr, refl, fresnel);
				float4 c2 = lerp(refr, refl, _Blend);

				float diff = 0.2 + 0.2 * dot(L, N);
				float spec = pow(max(0.0, dot(L, R)), _SpecularPower);
				float3 c = diff * _Diffuse.rgb * _Diffuse.a;
				c += spec * _Specular.rgb * _Specular.a;
				c += c2;
				return float4(c, _Transparence);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}