using System;

namespace vfy.be
{
	public interface IShortener
	{
		String Shorten(String url);
		Tuple<String, Int32> Expand(String hash);
	}
}

