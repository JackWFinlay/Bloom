using System;
using Xunit;
using Bloom;
using System.Text;
using Bloom.Configuration;

namespace BloomTest
{
    public class BloomTests
    {
        [Fact]
        public void AddItemToBloomTest()
        {
            BloomConfiguration config = new BloomConfiguration{
                BloomFilterSize =  8 * (long)BloomFilterSizeMultipliers.Bits,
            };

            BloomFilter bloomFilter = new BloomFilter(config);

            string testString = $"TestString";
            byte[] testByteArray = Encoding.ASCII.GetBytes(testString);

            bloomFilter.Add(testByteArray);

            bool[] actual = bloomFilter.Filter;
            bool[] expected = {true,false,true,false,false,false,false,true};

            Assert.True(CompareBoolArrays(expected, actual));
            Assert.True(bloomFilter.Contains(testByteArray));
        }

        [Fact]
        public void AddMultipleOfSameItemToBloomTest()
        {
            BloomConfiguration config = new BloomConfiguration{
                BloomFilterSize =  8 * (long)BloomFilterSizeMultipliers.Bits,
            };

            BloomFilter bloomFilter = new BloomFilter(config);

            string testString = $"TestString";
            byte[] testByteArray = Encoding.ASCII.GetBytes(testString);

            bloomFilter.Add(testByteArray);
            bloomFilter.Add(testByteArray);

            bool[] actual = bloomFilter.Filter;
            bool[] expected = {true,false,true,false,false,false,false,true};

            Assert.True(CompareBoolArrays(expected, actual));
            Assert.True(bloomFilter.Contains(testByteArray));
        }

        [Fact]
        public void AddMultipleDifferentItemsToBloomTest()
        {
            BloomConfiguration config = new BloomConfiguration{
                BloomFilterSize =  8 * (long)BloomFilterSizeMultipliers.Bits,
            };

            BloomFilter bloomFilter = new BloomFilter(config);

            string testString = $"TestString";
            byte[] testByteArray1 = Encoding.ASCII.GetBytes(testString);

            string TestString2 = $"TestString2";
            byte[] testByteArray2 = Encoding.ASCII.GetBytes(TestString2);

            bloomFilter.Add(testByteArray1);
            bloomFilter.Add(testByteArray2);

            bool[] actual = bloomFilter.Filter;
            bool[] expected = {true,false,true,true,false,true,false,true};

            Assert.True(CompareBoolArrays(expected, actual));
            Assert.True(bloomFilter.Contains(testByteArray1));
            Assert.True(bloomFilter.Contains(testByteArray2));
        }

        private bool CompareBoolArrays(bool[] expected, bool[] actual)
        {
            bool match = true;

            if (expected.Length != actual.Length){
                return false;
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != actual[i])
                {
                    match = false;
                    break;
                }
            }

            return match;
        }
    }
}
