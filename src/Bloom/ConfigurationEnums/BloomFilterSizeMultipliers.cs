namespace Bloom.Configuration
{
    public enum BloomFilterSizeMultipliers : long
    {
        Bits = 1,
        Bytes = 128,
        KiloBytes = (1024 * Bytes),
        MegaBytes = (1024 * KiloBytes)
    }
}