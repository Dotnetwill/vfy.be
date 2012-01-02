using System;

namespace vfy.be.Interfaces
{
	public interface IShortenerDb
	{
		Int32 InsertUrl(String url);
		Int32? GetIdForUrl(String url);
		dynamic GetDetailsFromId(int id);
		void IncrementClickCountById(Int32 id);
	}
}

