Shader "Unlit/ForestHighlight" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
	}

		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			LOD 100

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull off

			Pass {
				CGPROGRAM

				// Declare vertex and fragment shader programs
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
				};

				// Vertex to fragment structure
				struct v2f {
					float4 vertex : SV_POSITION;
					float4 worldPos : TEXCOORD0;
				};

				fixed4 _Color;

				v2f vert(appdata v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, input.vertex);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					
					return _Color;

				}

				ENDCG
			}
	}
}
