Shader "Custom/WallShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ClipNormal ("Clip Normal", Vector) = (0, 1, 0, 0) // 切り取る法線ベクトルを指定
        _Color ("Color", Color) = (1, 1, 1, 1) // インスペクターから変更可能な色
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Lighting On // ライティングを有効にする
        CGPROGRAM
        #pragma surface surf Standard // Standardライティングモデルを使用する
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        float4 _ClipNormal;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // 法線ベクトルのチェック
            if (dot(normalize(IN.worldNormal), normalize(_ClipNormal.xyz)) < _ClipNormal.w)
            {
                discard;
            }

            // テクスチャを取得して出力
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse" // フォールバックとしてDiffuseを使用する
}
