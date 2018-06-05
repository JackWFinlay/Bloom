namespace Bloom
{
    public interface IBloomFilter
    {
        void Add(byte[] item);
        bool Contains(byte[] item);
    }
}