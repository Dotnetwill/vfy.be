using System;
using NUnit.Framework;

namespace vfy.be.tests
{
	[TestFixture()]
	public class ShortenerMathBitsTests
	{
		
		[Test]
		public void Encode_9_ShouldReturn9 ()
		{
			const String expectedValue = "9";
			const Int32 inputValue = 9;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Encode(inputValue));
		}

		[Test]
		public void Encode_35_ShouldReturnZ ()
		{
			const String expectedValue = "z";
			const Int32 inputValue = 35;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Encode(inputValue));
		
		}
		
		[Test]
		public void Encode_44_ShouldReturn18 ()
		{
			const String expectedValue = "18";
			const Int32 inputValue = 44;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Encode(inputValue));
		
		}
		
	    [Test]
		public void Encode_72_ShouldReturn20 ()
		{
			const String expectedValue = "20";
			const Int32 inputValue = 72;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Encode(inputValue));
		
		}

		
		[Test]
		public void Encode_83_ShouldReturn2B ()
		{
			const String expectedValue = "2b";
			const Int32 inputValue = 83;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Encode(inputValue));
		
		}
		
		[Test]
		public void Encode_361_ShouldReturnA1 ()
		{
			const String expectedValue = "a1";
			const Int32 inputValue = 361;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Encode(inputValue));
		
		}
		
		[Test]
		public void Decode_7_ShouldReturn7()
		{
			const String inputValue = "7";
			const Int32 expectedValue = 7;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Decode(inputValue));
		}
		
		[Test]
		public void Decode_AA_ShouldReturn366()
		{
			const String inputValue = "AA";
			const Int32 expectedValue = 370;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Decode(inputValue));
		}

		[Test]
		public void Decode_aa_ShouldReturn366()
		{
			const String inputValue = "aa";
			const Int32 expectedValue = 370;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Decode(inputValue));
		}
		
		[Test]
		public void Decode_12_ShouldReturn38()
		{
			const String inputValue = "12";
			const Int32 expectedValue = 38;
			Assert.AreEqual(expectedValue, ShortenerMathBits.Decode(inputValue));
		}
		
		[Ignore("SLOW SLOW SLOW SLOW SLOW")]
		[Test]
		//Caution slowish (~25secs)
		public void EncodeDecode_EveryNumberBetween1And1mEncoded_DecodedBackToOrginalValue()
		{
			const Int32 start = 0;
			const Int32 end = 1000000;
			
			for(int i = 0; i <= end; i++)
			{
				var enc = ShortenerMathBits.Encode(i);
				var dec = ShortenerMathBits.Decode(enc);
				
				Assert.AreEqual(i, dec);
			}
		}
	}
}

