// ピクセル シェーダーはイメージの明るい領域を抽出します。
// これは、ブルーム ポストプロセスの適用における最初のステップです。

sampler TextureSampler : register(s0);

float BloomThreshold;


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // 元イメージの色を調べます。
    float4 c = tex2D(TextureSampler, texCoord);

    // 指定されたしきい値よりも明るい値だけを維持するように調整します。
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
