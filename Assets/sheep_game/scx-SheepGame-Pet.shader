Shader "scx/SheepGame/Pet"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "DisableBatching"="True"
        }

        LOD 100

        Cull Off
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // 对应 Batch 通过属性块绑定的 float Buffer
            StructuredBuffer<float> _UnitData;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;

                UNITY_FOG_COORDS(1)

                // 将闪红开关传给片元 shader
                float flashEnabled : TEXCOORD2;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            v2f vert(appdata_t v, uint vertexID : SV_VertexID)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;

                // 每个 Unit 连续占 4 个顶点
                uint unitIndex = vertexID / 4u;

                // 每个 Unit 连续占 8 个 float，读取第一个
                float firstValue = _UnitData[unitIndex * 8u];

                o.flashEnabled = firstValue != 0.0 ? 1.0 : 0.0;

                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                clip(col.a - _Cutoff);
                col *= i.color;

                // _Time.y 是 Unity 提供的时间，单位秒
                // 每秒一轮，强度在 0～1 之间变化
                float pulse = 0.5 - 0.5 * cos(_Time.y * 6.2831853);

                float redAmount = pulse * i.flashEnabled;

                // 只修改 RGB，保留原来的透明度
                col.rgb = lerp(
                    col.rgb,
                    float3(1.0, 0.0, 0.0),
                    redAmount
                );

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}