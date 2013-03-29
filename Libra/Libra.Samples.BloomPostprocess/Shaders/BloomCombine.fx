// ピクセル シェーダーは、微調整が可能な強度レベルと
// 彩度を使用して、ブルーム イメージを元のシーンと結合します。
// これは、ブルーム ポストプロセスの適用における最終ステップです。

sampler BloomSampler : register(s0);
sampler BaseSampler : register(s1);

float BloomIntensity;
float BaseIntensity;

float BloomSaturation;
float BaseSaturation;


// 彩度を変更するためのヘルパー。
float4 AdjustSaturation(float4 color, float saturation)
{
    // 人間の目は緑の光に対してより鋭敏であり、青に対してはそれほど
    // 鋭敏でないため、定数に 0.3、0.59、および 0.11 が選択されています。
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // ブルームと元のベース イメージの色を調べます。
    float4 bloom = tex2D(BloomSampler, texCoord);
    float4 base = tex2D(BaseSampler, texCoord);
    
    // 彩度と強度を調整します。
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // 物体が過度に燃え上がって見えないように、ブルームの強い領域では
    // ベース イメージを暗くします。
    base *= (1 - saturate(bloom));
    
    // 2 つのイメージを結合します。
    return base + bloom;
}


technique BloomCombine
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
