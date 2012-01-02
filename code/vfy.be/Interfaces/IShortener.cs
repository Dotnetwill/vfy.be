using System;

namespace vfy.be
{
	public interface IShortener
	{
		String Shorten(String url);
		String Expand(String hash);
	}
}

