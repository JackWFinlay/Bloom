using System;
using Xunit;
using Bloom;
using System.Text;
using Bloom.Configuration;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace BloomTest
{
    public class BloomTests
    {
        private readonly ITestOutputHelper _output;

        public BloomTests(ITestOutputHelper output)
        {
            _output = output;
        }

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

            string testString2 = $"TestString2";
            byte[] testByteArray2 = Encoding.ASCII.GetBytes(testString2);

            bloomFilter.Add(testByteArray1);
            bloomFilter.Add(testByteArray2);

            bool[] actual = bloomFilter.Filter;
            bool[] expected = {true,false,true,true,false,true,false,true};

            Assert.True(CompareBoolArrays(expected, actual));
            Assert.True(bloomFilter.Contains(testByteArray1));
            Assert.True(bloomFilter.Contains(testByteArray2));
        }

        [Fact]
        public void UseAdditionalHashFunctionsTest()
        {
            BloomConfiguration config = new BloomConfiguration{
                BloomFilterSize =  1 * (long)BloomFilterSizeMultipliers.Bytes,
                HashAlgorithms = new List<Type> {
                    typeof(MD5CryptoServiceProvider),
                    typeof(SHA1CryptoServiceProvider),
                    typeof(SHA256CryptoServiceProvider),
                    typeof(SHA384CryptoServiceProvider),
                    typeof(SHA512CryptoServiceProvider)
                }
            };

            BloomFilter bloomFilter = new BloomFilter(config);

            string testString = $"TestString";
            byte[] testByteArray1 = Encoding.ASCII.GetBytes(testString);

            string testString2 = $"TestString2";
            byte[] testByteArray2 = Encoding.ASCII.GetBytes(testString2);

            bloomFilter.Add(testByteArray1);
            bloomFilter.Add(testByteArray2);

            bool[] actual = bloomFilter.Filter;

            PrintFilterResults(actual);

            bool[] expected = { true,false,false,false,false,false,true,false,false,false,false,false,false,false,false,false,false,false,
                                false,false,false,false,false,false,false,false,false,false,false,false,false,true,false,false,false,false,
                                false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
                                false,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
                                false,false,false,false,false,true,false,false,false,false,false,false,false,true,true,false,false,false,
                                false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,false,
                                false,false,false,false,false,false,true,false,false,false,false,false,true,false,false,true,false,false,false,
                                false };

            Assert.True(CompareBoolArrays(expected, actual));
            Assert.True(bloomFilter.Contains(testByteArray1));
            Assert.True(bloomFilter.Contains(testByteArray2));
        }

        private void PrintFilterResults(bool[] actual)
        {
            foreach (bool flag in actual)
            {
                _output.WriteLine($"{flag},");
            }
        }

        private static bool CompareBoolArrays(bool[] expected, bool[] actual)
        {
            if (expected.Length != actual.Length){
                return false;
            }

            return !expected.Where((t, i) => t != actual[i]).Any();
        }
    }
}
