// �s�N�Z�� �V�F�[�_�[�́A���������\�ȋ��x���x����
// �ʓx���g�p���āA�u���[�� �C���[�W�����̃V�[���ƌ������܂��B
// ����́A�u���[�� �|�X�g�v���Z�X�̓K�p�ɂ�����ŏI�X�e�b�v�ł��B

sampler BloomSampler : register(s0);
sampler BaseSampler : register(s1);

float BloomIntensity;
float BaseIntensity;

float BloomSaturation;
float BaseSaturation;


// �ʓx��ύX���邽�߂̃w���p�[�B
float4 AdjustSaturation(float4 color, float saturation)
{
    // �l�Ԃ̖ڂ͗΂̌��ɑ΂��Ă��s�q�ł���A�ɑ΂��Ă͂���ق�
    // �s�q�łȂ����߁A�萔�� 0.3�A0.59�A����� 0.11 ���I������Ă��܂��B
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // �u���[���ƌ��̃x�[�X �C���[�W�̐F�𒲂ׂ܂��B
    float4 bloom = tex2D(BloomSampler, texCoord);
    float4 base = tex2D(BaseSampler, texCoord);
    
    // �ʓx�Ƌ��x�𒲐����܂��B
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // ���̂��ߓx�ɔR���オ���Č����Ȃ��悤�ɁA�u���[���̋����̈�ł�
    // �x�[�X �C���[�W���Â����܂��B
    base *= (1 - saturate(bloom));
    
    // 2 �̃C���[�W���������܂��B
    return base + bloom;
}


technique BloomCombine
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
