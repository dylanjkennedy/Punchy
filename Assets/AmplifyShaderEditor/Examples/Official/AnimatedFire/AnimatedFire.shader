// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/AnimatedFire"
{
	Properties
	{
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_Albedo("Albedo", 2D) = "white" {}
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_Normals("Normals", 2D) = "bump" {}
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 4
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		_Mask("Mask", 2D) = "white" {}
		_Specular("Specular", 2D) = "white" {}
		_TileableFire("TileableFire", 2D) = "white" {}
		_FireIntensity("FireIntensity", Range( 0 , 2)) = 0
		_Smoothness("Smoothness", Float) = 1
		_Vector1("Vector 1", Vector) = (0,0,0,0)
		_TileSpeed("TileSpeed", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Tessellation.cginc"
		#pragma target 5.0
		#pragma surface surf StandardSpecularCustom keepalpha exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardSpecularCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half3 Specular;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform sampler2D _Mask;
		uniform sampler2D _TileableFire;
		uniform float2 _TileSpeed;
		uniform float _FireIntensity;
		uniform sampler2D _Normals;
		uniform float2 _Vector1;
		uniform sampler2D _Albedo;
		uniform sampler2D _Specular;
		uniform float _Smoothness;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float4 tex2DNode2 = tex2Dlod( _Mask, float4( v.texcoord.xy, 0, 1.0) );
			float2 panner16 = ( _Time.x * _TileSpeed + v.texcoord.xy);
			float4 temp_output_8_0 = ( ( tex2DNode2 * tex2Dlod( _TileableFire, float4( panner16, 0, 1.0) ) ) * _FireIntensity );
			v.vertex.xyz += temp_output_8_0.rgb;
		}

		inline half4 LightingStandardSpecularCustom(SurfaceOutputStandardSpecularCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandardSpecular r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Specular = s.Specular;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandardSpecular (r, viewDir, gi) + c;
		}

		inline void LightingStandardSpecularCustom_GI(SurfaceOutputStandardSpecularCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardSpecularCustom o )
		{
			float2 panner21 = ( _Time.x * _Vector1 + i.uv_texcoord);
			o.Normal = UnpackNormal( tex2D( _Normals, panner21 ) );
			o.Albedo = tex2D( _Albedo, i.uv_texcoord ).rgb;
			float4 tex2DNode2 = tex2D( _Mask, i.uv_texcoord );
			float2 panner16 = ( _Time.x * _TileSpeed + i.uv_texcoord);
			float4 temp_output_8_0 = ( ( tex2DNode2 * tex2D( _TileableFire, panner16 ) ) * _FireIntensity );
			o.Emission = temp_output_8_0.rgb;
			o.Specular = tex2D( _Specular, i.uv_texcoord ).rgb;
			o.Smoothness = _Smoothness;
			float3 temp_cast_3 = (1.0).xxx;
			o.Translucency = temp_cast_3;
			o.Alpha = tex2DNode2.r;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
1958;192;1586;857;1560.913;567.5746;1.582783;True;True
Node;AmplifyShaderEditor.Vector2Node;17;-1061.203,309.7833;Float;False;Property;_TileSpeed;TileSpeed;19;0;Create;True;0;0;False;0;0,0;-35.2,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TimeNode;5;-1168,448.5;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1236,81.5;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;16;-842.203,309.7833;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-602,262.5;Float;True;Property;_TileableFire;TileableFire;15;0;Create;True;0;0;False;0;None;f7e96904e8667e1439548f0f86389447;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-615,-237.5;Float;True;Property;_Mask;Mask;13;0;Create;True;0;0;False;0;None;36be8d528a4fa024faa4680d7658642c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;20;-1157.731,161.7438;Float;False;Property;_Vector1;Vector 1;18;0;Create;True;0;0;False;0;0,0;-35.2,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TimeNode;19;-1264.528,300.4604;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-484,485.5;Float;False;Property;_FireIntensity;FireIntensity;16;0;Create;True;0;0;False;0;0;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-296,-169.5;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;21;-938.7306,161.7438;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-96,123.5;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-138,509.5;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;50.841,404.82;Float;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-60.95492,233.5525;Float;False;Property;_Smoothness;Smoothness;17;0;Create;True;0;0;False;0;1;0.16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-556,-625.5;Float;True;Property;_Albedo;Albedo;6;0;Create;True;0;0;False;0;None;7130c16fd8005b546b111d341310a9a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;11;-306,616.5;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-520,-427.5;Float;True;Property;_Specular;Specular;14;0;Create;True;0;0;False;0;None;bea7fa376f932ba419f3d1fc95bd1a2b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinTimeNode;9;-508,600.5;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;14;-691,31.5;Float;True;Property;_Normals;Normals;7;0;Create;True;0;0;False;0;None;11f03d9db1a617e40b7ece71f0a84f6f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;262.1252,-212.9322;Float;False;True;7;Float;ASEMaterialInspector;0;0;StandardSpecular;ASESampleShaders/AnimatedFire;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;False;0;False;Opaque;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;0;-1;8;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;6;0
WireConnection;16;2;17;0
WireConnection;16;1;5;1
WireConnection;1;1;16;0
WireConnection;2;1;6;0
WireConnection;3;0;2;0
WireConnection;3;1;1;0
WireConnection;21;0;6;0
WireConnection;21;2;20;0
WireConnection;21;1;19;1
WireConnection;8;0;3;0
WireConnection;8;1;7;0
WireConnection;10;0;7;0
WireConnection;10;1;11;0
WireConnection;12;1;6;0
WireConnection;11;0;9;4
WireConnection;13;1;6;0
WireConnection;14;1;21;0
WireConnection;0;0;12;0
WireConnection;0;1;14;0
WireConnection;0;2;8;0
WireConnection;0;3;13;0
WireConnection;0;4;15;0
WireConnection;0;7;18;0
WireConnection;0;9;2;0
WireConnection;0;11;8;0
ASEEND*/
//CHKSM=E30E1F9CF36BDFFE4823F3A3AADE4ED786DA932D