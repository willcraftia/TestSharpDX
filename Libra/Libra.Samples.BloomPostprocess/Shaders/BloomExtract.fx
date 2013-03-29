// �s�N�Z�� �V�F�[�_�[�̓C���[�W�̖��邢�̈�𒊏o���܂��B
// ����́A�u���[�� �|�X�g�v���Z�X�̓K�p�ɂ�����ŏ��̃X�e�b�v�ł��B

sampler TextureSampler : register(s0);

float BloomThreshold;


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // ���C���[�W�̐F�𒲂ׂ܂��B
    float4 c = tex2D(TextureSampler, texCoord);

    // �w�肳�ꂽ�������l�������邢�l�������ێ�����悤�ɒ������܂��B
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
