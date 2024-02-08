Shader "Unlit/DangerFlash" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
		_FlashColour("Flash Colour", Color) = (1, 0, 0, 0.5)
    }

    SubShader {
        Tags { "RenderType"="Transparent" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;

            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
				

                return (0,0,0,0);
            }
            ENDCG
        }
    }
}
