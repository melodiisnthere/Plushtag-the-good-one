Shader "Custom/Misc/Static Shader" {
	Properties {
		[Enum(UnityEngine.Rendering.CullMode)] _Cullmode ("Cull mode", Float) = 2
		[Header(Texture)] _Texture ("Texture", 2D) = "white" {}
		_Texturebrightness ("Texture brightness", Float) = 1
		_Texturesaturation ("Texture saturation", Range(0, 1)) = 1
		[Toggle] _Cutouttexture ("Cutout texture", Float) = 0
		_Texturealphacutoff ("Texture alpha cutoff", Range(0, 1)) = 0.5
		[Header(Static)] _Staticcolour1 ("Static colour 1", Vector) = (0,0,0,1)
		_Staticcolour2 ("Static colour 2", Vector) = (1,1,1,1)
		_StaticscaleX ("Static scale (X)", Float) = 1
		_StaticscaleY ("Static scale (Y)", Float) = 1
		_Staticrotationspeed ("Static rotation speed", Float) = 1
		[Toggle] _Screenspacestatic ("Screenspace static", Float) = 0
		[Header(Scanlines)] [Toggle] _Scanlines ("Scanlines", Float) = 0
		[Enum(X,0,Y,1,Z,2)] _Scanlineaxis ("Scanline axis", Float) = 1
		_Scanlinefrequency ("Scanline frequency", Float) = 1
		_Scanlineintensity ("Scanline intensity", Range(0, 1)) = 0.5
		_Scanlinespeed ("Scanline speed", Float) = 1
		[Header(Rim light)] [Toggle] _Rimlight ("Rim light", Float) = 0
		_Rimcolour ("Rim colour", Vector) = (1,1,1,1)
		_Rimbrightness ("Rim brightness", Float) = 1
		_Rimscale ("Rim scale", Float) = 1
		_Rimpower ("Rim power", Float) = 1
		[Toggle] _Rainbowrim ("Rainbow rim", Float) = 0
		_Rimcolourspeed ("Rim colour speed", Float) = 1
		[Header(Vertex offset)] [Enum(None,0,Offset,1,Deform,2)] _Vertexoffsetstyle ("Vertex offset style", Float) = 0
		_VertexoffsetX ("Vertex offset (X)", Float) = 0
		_VertexoffsetY ("Vertex offset (Y)", Float) = 0
		_VertexoffsetZ ("Vertex offset (Z)", Float) = 0
		[Toggle] _Vertexoffsetpulse ("Vertex offset pulse", Float) = 0
		_Vertexoffsetpulsespeed ("Vertex offset pulse speed", Float) = 1
		_Vertexdeformfrequency ("Vertex deform frequency", Float) = 1
		_Vertexdeformspeed ("Vertex deform speed", Float) = 1
		[Header(Opacity)] _Opacity ("Opacity", Range(0, 1)) = 1
		[Toggle] _Distancefade ("Distance fade", Float) = 0
		[Toggle] _Reversedistancefade ("Reverse distance fade", Float) = 0
		_Startfade ("Start fade", Float) = 0
		_Endfade ("End fade", Float) = 10
		[Toggle] _Usecustomdithertexture ("Use custom dither texture", Float) = 0
		[NoScaleOffset] _Dithertexture ("Dither texture", 2D) = "white" {}
		[Header(Mirror)] [Enum(None,0,Hide in mirror,1,Only show in mirror,2)] _Hidesettings ("Hide settings", Float) = 0
		[HideInInspector] _Color ("Fallback colour", Vector) = (0,0,0,1)
		[HideInInspector] _Maskclip ("Mask clip", Float) = 0.5
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}