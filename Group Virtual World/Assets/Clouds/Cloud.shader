Shader "Unlit/Cloud" {

    SubShader {
        Tags { "Queue"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			uniform int _EntityCount = 0;
			uniform float4 _Entities[25];
			uniform float4 _Test;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float3 wPos : TEXCOORD0; // World position
                float4 pos : SV_POSITION;
            };

			#define STEPS 32
			#define COLLIDE_THRESHOLD 0.01

			float length(float3 v) {
				return sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
			}

			float magnitude(float3 v) {
				return v.x + v.y + v.z;
			}

			float signedDstToSphere(float3 p, float3 centre, float radius) {
				return length(centre - p) - radius;
			}

			int closestEntity(float3 position) {
				int index;
				float distance = 999999;

				for (int i = 0; i < _EntityCount; i++) {
					float dstToEntity = magnitude((_Entities[i].xyz - position));

					if (dstToEntity < distance) {
						distance = dstToEntity;
						index = i;
					}
				}

				return index;
				
			}

			float Raymarch(float3 position, float3 direction) {
				float smallest = 999999;

				for (int i = 0; i < STEPS; i++) {
					int closestIndex = closestEntity(position);

					float distance = signedDstToSphere(position, _Entities[closestIndex].xyz, 0.5);

					/*if (distance < smallest)
						distance = smallest;*/

					if (distance < COLLIDE_THRESHOLD) {
						return distance;

					} else {
						position += direction * distance;
					}

				}

				return 0;

				//return smallest;
			}

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target {
				float depth = Raymarch(i.wPos, normalize(i.wPos - _WorldSpaceCameraPos));
				float dst = signedDstToSphere(_WorldSpaceCameraPos, _Entities[0].xyz, 0.5);

				float testDst = magnitude(_Entities[0].xyz - _WorldSpaceCameraPos);

				return fixed4(_Test.xyz, 1);

            }

            ENDCG
        }
    }
}
