Shader "Custom/Screen/Retro Shader" {
	Properties {
		_DistanceMain ("Main Distance", Range(0, 100)) = 1
		_DistanceStart ("Fade Distance", Range(0, 100)) = 1
		_Size ("Pixel Size", Range(1, 50)) = 8
		_ColDepth ("Color Depth", Range(1, 50)) = 7
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}