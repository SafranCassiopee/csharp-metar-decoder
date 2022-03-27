using MetarDecoder.Exceptions;
using NUnit.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MetarDecoder.Tests
{
    [TestFixture, Category("MetarChunkDecoderException")]
    public class MetarChunkDecoderExceptionTest
    {
        [Test]
        public void SerializationWithNoMessageTest()
        {
            var dto = new MetarChunkDecoderException();
            var mem = new MemoryStream();
            var b = new BinaryFormatter();
            Assert.DoesNotThrow(() =>
            {
                b.Serialize(mem, dto);
            });
        }

        [Test]
        public void SerializationWithMessageTest()
        {
            var dto = new MetarChunkDecoderException("Test");
            var mem = new MemoryStream();
            var b = new BinaryFormatter();
            Assert.DoesNotThrow(() =>
            {
                b.Serialize(mem, dto);
            });
        }


        [Test]
        public void SerializationWithSerializationInfoTest()
        {
            var dto = new MetarChunkDecoderException("Test");
            var mem = new MemoryStream();
            var b = new BinaryFormatter();
            Assert.DoesNotThrow(() =>
            {
                b.Serialize(mem, dto);
            });
        }
    }
}
