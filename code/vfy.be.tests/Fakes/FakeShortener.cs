using System;

namespace vfy.be.tests
{
	public class FakeShortener : IShortener
	{
		public String ShortenReturnValue { get; set; }
		public String ShortenCalledWith { get; set; }
		public String Shorten (String url)
		{
			ShortenCalledWith = url;
			return ShortenReturnValue ?? String.Empty;
		}
		
		public String ExpandReturns { get; set; }
		public Int32 ExpandClickValue { get; set; }
		public String ExpandCalledWithHash { get; set; }
		public Tuple<String, Int32> Expand (String hash)
		{
			ExpandCalledWithHash = hash;
			return new Tuple<String, Int32>(ExpandReturns ?? String.Empty, ExpandClickValue);
		}
	}
}

