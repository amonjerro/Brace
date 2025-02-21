Shader "Unlit/ActionShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Arc("Arc", float) = 0
        _AngleOffset("AngleOffset", float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }


        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Arc;
            float _AngleOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture, though we want this for alpha mostly
                fixed4 col = tex2D(_MainTex, i.uv);

                // Position of Fragment relative to center

                float angleToCenter = atan2(i.uv.y*2-1, i.uv.x*2-1) + UNITY_PI;
                float mask = angleToCenter < _Arc;
                return fixed4(col.xyz*0.1,col.a * mask*0.5);
            }
            ENDCG
        }
    }
}
