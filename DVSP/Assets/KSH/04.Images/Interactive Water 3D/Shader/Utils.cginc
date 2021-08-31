#ifndef IW3D_UTILS
#define IW3D_UTILS

// encode float to RGBA
float4 EncodeHeightmap (float f)
{
	float positive = f > 0 ? f : 0;
	float negative = f < 0 ? -f : 0;
	
	float4 c = 0;
	c.r = positive;
	c.g = negative;
	c.ba = frac(c.rg * 256);
	c.rg -= c.ba / 256.0;
	return c;
}
// decode RGBA to float
float DecodeHeightmap (float4 rgba)
{
	float4 v = float4(1.0, -1.0, 1.0 / 256.0, -1.0 / 256.0);
	return dot(rgba, v);
}
//#ifdef IW3D_USE_FloatRenderTarget
	// in 2019 we always use float render target, say bye-bye to non float render target
	#define HEIGHT_MAP_READ_LOD(t, uv)   (tex2Dlod(t, float4(uv, 0, 1)))
	#define HEIGHT_MAP_READ(t, uv)       (tex2D(t, uv))
	#define HEIGHT_MAP_WRITE(h)          (h)
//#else
//	#define HEIGHT_MAP_READ_LOD(t, uv)   (DecodeHeightmap(tex2Dlod(t, float4(uv, 0, 1))))
//	#define HEIGHT_MAP_READ(t, uv)       (DecodeHeightmap(tex2D(t, uv)))
//	#define HEIGHT_MAP_WRITE(h)          (EncodeHeightmap(h))
//#endif

// caustic //////////////////////////////////////////////////////////////////////////
sampler2D _Global_CausticTex;
float4 _Global_CausticPlane;
float4 _Global_CausticRange;
float _Global_CausticIntensity;

float3 SampleCaustic (float3 wldpos, float3 wldlit)
{
	float4 plane = _Global_CausticPlane;
	float4 range = _Global_CausticRange;

	float3 hit = wldpos + wldlit * (plane.w - dot(wldpos, plane.xyz) / dot(wldlit, plane.xyz));
	float2 uv = (hit.xz - range.xy) / range.zw * 0.5 + 0.5;
	uv.x = 1 - uv.x;
	float3 caustic = tex2D(_Global_CausticTex, uv).rgb - 0.5;
	if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
		return 0;
	return caustic * _Global_CausticIntensity;
}

#endif

