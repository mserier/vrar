// Toony Colors Pro+Mobile 2
// (c) 2014-2018 Jean Moreno

Shader "Toony Colors Pro 2/User/My TCP2 Shader"
{
	Properties
	{
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_Color ("Color", Color) = (1,1,1,1)
		_HColor ("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor ("Shadow Color", Color) = (0.195,0.195,0.195,1.0)

		//DIFFUSE
		_MainTex ("Main Texture", 2D) = "white" {}
	[TCP2Separator]

		//TOONY COLORS RAMP
		[TCP2Header(RAMP SETTINGS)]

		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
		_ThresholdTex ("Threshold Texture (Alpha)", 2D) = "gray" {}
	[TCP2Separator]

	[TCP2HeaderHelp(DISSOLVE)]
		[NoScaleOffset]
		_DissolveMap ("Dissolve Map", 2D) = "white" {}
		_DissolveValue ("Dissolve Value", Range(0,1)) = 0.5
		[TCP2Gradient] _DissolveRamp ("Dissolve Ramp", 2D) = "white" {}
		_DissolveGradientWidth ("Ramp Width", Range(0,1)) = 0.2
	[TCP2Separator]

	[TCP2HeaderHelp(SILHOUETTE)]
		_SilhouetteColor ("Color (RGB) Opacity (A)", Color) = (0,0,0,0.5)
	[TCP2Separator]

	[TCP2HeaderHelp(OUTLINE, Outline)]
		//OUTLINE
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1.0)
		_Outline ("Outline Width", Float) = 1

		//Outline Textured
		[Toggle(TCP2_OUTLINE_TEXTURED)] _EnableTexturedOutline ("Color from Texture", Float) = 0
		[TCP2KeywordFilter(TCP2_OUTLINE_TEXTURED)] _TexLod ("Texture LOD", Range(0,10)) = 5

		//Constant-size outline
		[Toggle(TCP2_OUTLINE_CONST_SIZE)] _EnableConstSizeOutline ("Constant Size Outline", Float) = 0

		//ZSmooth
		[Toggle(TCP2_ZSMOOTH_ON)] _EnableZSmooth ("Correct Z Artefacts", Float) = 0
		//Z Correction & Offset
		[TCP2KeywordFilter(TCP2_ZSMOOTH_ON)] _ZSmooth ("Z Correction", Range(-3.0,3.0)) = -0.5
		[TCP2KeywordFilter(TCP2_ZSMOOTH_ON)] _Offset1 ("Z Offset 1", Float) = 0
		[TCP2KeywordFilter(TCP2_ZSMOOTH_ON)] _Offset2 ("Z Offset 2", Float) = 0

		//This property will be ignored and will draw the custom normals GUI instead
		[TCP2OutlineNormalsGUI] __outline_gui_dummy__ ("_unused_", Float) = 0
			_OutlineOffset ("Outline Rim Offset", Vector) = (0,0,0,0)
	[TCP2Separator]

	[TCP2HeaderHelp(TRANSPARENCY)]
		//Alpha Testing
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	[TCP2Separator]


		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		//Make sure that the objects are rendered later to avoid sorting issues with the transparent silhouette
		Tags { "Queue"="Geometry+10" }
		
		//Silhouette Pass
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Greater
			ZWrite Off			
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0

				#include "UnityCG.cginc"

				fixed4 _SilhouetteColor;

				struct appdata_sil
				{
					float4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f_sil
				{
					float4 vertex : SV_POSITION;
					UNITY_VERTEX_OUTPUT_STEREO
				};

				v2f_sil vert (appdata_sil v)
				{
					v2f_sil o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.vertex = UnityObjectToClipPos(v.vertex);
					return o;
				}

				fixed4 frag (v2f_sil i) : COLOR
				{
					return _SilhouetteColor;
				}
			ENDCG
		}

		//================================================================
		// OUTLINE INCLUDE

		CGINCLUDE

		#include "UnityCG.cginc"

		struct a2v
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
	#if TCP2_OUTLINE_TEXTURED
			float3 texcoord : TEXCOORD0;
	#endif
			float4 color : COLOR;
	#if TCP2_UV2_AS_NORMALS
			float2 uv2 : TEXCOORD1;
	#endif
	#if TCP2_TANGENT_AS_NORMALS
			float4 tangent : TANGENT;
	#endif
	#if UNITY_VERSION >= 550
			UNITY_VERTEX_INPUT_INSTANCE_ID
	#endif
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
	#if TCP2_OUTLINE_TEXTURED
			float3 texlod : TEXCOORD1;
	#endif
		};

		float _Outline;
		float _ZSmooth;
		fixed4 _OutlineColor;
		half4 _OutlineOffset;

	#if TCP2_OUTLINE_TEXTURED
		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _TexLod;
	#endif

		#define OUTLINE_WIDTH (_Outline * v.color.r)

		v2f TCP2_Outline_Vert(a2v v)
		{
			v2f o;

	#if UNITY_VERSION >= 550
			//GPU instancing support
			UNITY_SETUP_INSTANCE_ID(v);
	#endif

			v.vertex += _OutlineOffset;

	#if TCP2_ZSMOOTH_ON
			float4 pos = float4(UnityObjectToViewPos(v.vertex), 1.0);
	#endif

	#ifdef TCP2_COLORS_AS_NORMALS
			//Vertex Color for Normals
			float3 normal = (v.color.xyz*2) - 1;
	#elif TCP2_TANGENT_AS_NORMALS
			//Tangent for Normals
			float3 normal = v.tangent.xyz;
	#elif TCP2_UV2_AS_NORMALS
			//UV2 for Normals
			float3 n;
			//unpack uv2
			v.uv2.x = v.uv2.x * 255.0/16.0;
			n.x = floor(v.uv2.x) / 15.0;
			n.y = frac(v.uv2.x) * 16.0 / 15.0;
			//get z
			n.z = v.uv2.y;
			//transform
			n = n*2 - 1;
			float3 normal = n;
	#else
			float3 normal = v.normal;
	#endif

	#if TCP2_ZSMOOTH_ON
			//Correct Z artefacts
			normal = UnityObjectToViewPos(normal);
			normal.z = -_ZSmooth;
	#endif

	#ifdef TCP2_OUTLINE_CONST_SIZE
			//Camera-independent outline size
			float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
			#define SIZE	dist
	#else
			#define SIZE	1.0
	#endif

	#if TCP2_ZSMOOTH_ON
			o.pos = mul(UNITY_MATRIX_P, pos + float4(normalize(normal),0) * OUTLINE_WIDTH * 0.01 * SIZE);
	#else
			o.pos = UnityObjectToClipPos(v.vertex + float4(normal,0) * OUTLINE_WIDTH * 0.01 * SIZE);
	#endif

	#if TCP2_OUTLINE_TEXTURED
			half2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.texlod = tex2Dlod(_MainTex, float4(uv, 0, _TexLod)).rgb;
	#endif

			return o;
		}

		#define OUTLINE_COLOR _OutlineColor

		float4 TCP2_Outline_Frag (v2f IN) : SV_Target
		{
	#if TCP2_OUTLINE_TEXTURED
			return float4(IN.texlod, 1) * OUTLINE_COLOR;
	#else
			return OUTLINE_COLOR;
	#endif
		}

		ENDCG

		// OUTLINE INCLUDE END
		//================================================================
		//Outline
		Pass
		{
			Cull Off
			ZWrite Off
			Offset [_Offset1],[_Offset2]

			Tags { "LightMode"="ForwardBase" "Queue"="Transparent" "IgnoreProjectors"="True" }

			CGPROGRAM

			#pragma vertex TCP2_Outline_Vert
			#pragma fragment TCP2_Outline_Frag

			#pragma multi_compile TCP2_NONE TCP2_ZSMOOTH_ON
			#pragma multi_compile TCP2_NONE TCP2_OUTLINE_CONST_SIZE
			#pragma multi_compile TCP2_NONE TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS
			#pragma multi_compile TCP2_NONE TCP2_OUTLINE_TEXTURED
			#pragma multi_compile_instancing

			#pragma target 2.5

			ENDCG
		}

		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}

		CGPROGRAM

		#pragma surface surf ToonyColorsCustom noforwardadd exclude_path:deferred exclude_path:prepass
		#pragma target 2.5

		//================================================================
		// VARIABLES

		fixed4 _Color;
		sampler2D _MainTex;
		sampler2D _ThresholdTex;
		sampler2D _DissolveMap;
		half _DissolveValue;
		sampler2D _DissolveRamp;
		half _DissolveGradientWidth;
		fixed _SketchSpeed;
		fixed _Cutoff;

		#define UV_MAINTEX uv_MainTex

		struct Input
		{
			half2 uv_MainTex;
			half2 uv_ThresholdTex;
		};

		//================================================================
		// CUSTOM LIGHTING

		//Lighting-related variables
		fixed4 _HColor;
		fixed4 _SColor;
		half _RampThreshold;
		half _RampSmooth;

		// Instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			half atten;
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			fixed TexThreshold;
		};

		inline half4 LightingToonyColorsCustom (inout SurfaceOutputCustom s, half3 viewDir, UnityGI gi)
		{
		#define IN_NORMAL s.Normal
	
			half3 lightDir = gi.light.dir;
		#if defined(UNITY_PASS_FORWARDBASE)
			half3 lightColor = _LightColor0.rgb;
			half atten = s.atten;
		#else
			half3 lightColor = gi.light.color.rgb;
			half atten = 1;
		#endif

			IN_NORMAL = normalize(IN_NORMAL);
			fixed ndl = max(0, dot(IN_NORMAL, lightDir));
			#define NDL ndl
			NDL += s.TexThreshold;

			#define		RAMP_THRESHOLD	_RampThreshold
			#define		RAMP_SMOOTH		_RampSmooth

			fixed3 ramp = smoothstep(RAMP_THRESHOLD - RAMP_SMOOTH*0.5, RAMP_THRESHOLD + RAMP_SMOOTH*0.5, NDL);
		#if !(POINT) && !(SPOT)
			ramp *= atten;
		#endif
			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);
			fixed4 c;
			c.rgb = s.Albedo * lightColor.rgb * ramp;
			c.a = s.Alpha;

		#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
			c.rgb += s.Albedo * gi.indirect.diffuse;
		#endif

			return c;
		}

		void LightingToonyColorsCustom_GI(inout SurfaceOutputCustom s, UnityGIInput data, inout UnityGI gi)
		{
			gi = UnityGlobalIllumination(data, 1.0, IN_NORMAL);

			s.atten = data.atten;	//transfer attenuation to lighting function
			gi.light.color = _LightColor0.rgb;	//remove attenuation
		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.UV_MAINTEX);
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = _Color.a;

			//Dissolve
			fixed4 dslv = tex2D(_DissolveMap, IN.UV_MAINTEX.xy);
			#define DSLV dslv.r
			float dissValue = lerp(-_DissolveGradientWidth, 1, _DissolveValue);
			float dissolveUV = smoothstep(DSLV - _DissolveGradientWidth, DSLV + _DissolveGradientWidth, dissValue);
			half4 dissolveColor = tex2D(_DissolveRamp, dissolveUV.xx);
			dissolveColor *= lerp(0, 3.0, dissolveUV);
			o.Emission += dissolveColor.rgb;

			o.Alpha *= DSLV + _Cutoff - dissValue;
	
			//Cutout (Alpha Testing)
			clip (o.Alpha - _Cutoff);

			//Textured Threshold
			o.TexThreshold = tex2D(_ThresholdTex, IN.uv_ThresholdTex).a - 0.5;
		}

		ENDCG

		//Outline - Depth Pass Only
		Pass
		{
			Name "OUTLINE_DEPTH"

			Cull Off
			Offset [_Offset1],[_Offset2]
			Tags { "LightMode"="ForwardBase" }

			//Write to Depth Buffer only
			ColorMask 0
			ZWrite On

			CGPROGRAM

			#pragma vertex TCP2_Outline_Vert
			#pragma fragment TCP2_Outline_Frag

			#pragma multi_compile TCP2_NONE TCP2_ZSMOOTH_ON
			#pragma multi_compile TCP2_NONE TCP2_OUTLINE_CONST_SIZE
			#pragma multi_compile TCP2_NONE TCP2_COLORS_AS_NORMALS TCP2_TANGENT_AS_NORMALS TCP2_UV2_AS_NORMALS
			//#pragma multi_compile TCP2_NONE TCP2_OUTLINE_TEXTURED		//Not needed for depth
			#pragma multi_compile_instancing

			#pragma target 2.5

			ENDCG
		}
	}

	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
