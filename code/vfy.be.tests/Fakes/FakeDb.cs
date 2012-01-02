using System;
using vfy.be.Interfaces;

namespace vfy.be.tests
{
	public class FakeDb : IShortenerDb
	{
		public Int32? InsertedUrlId { get; set; } 
		public String InsertedUrl { get; set; }
		public Int32 InsertUrl (string url)
		{
			InsertedUrl = url;
			return InsertedUrlId.HasValue ?  InsertedUrlId.Value : new Random().Next(0, 500);
		}
		
		public Int32? GetIdForUrlReturns { get; set; }
		public int? GetIdForUrl (string url)
		{
			return GetIdForUrlReturns;
		}
		
		public String GetUrlForIdReturns { get; set; }
		public String GetUrlForId(Int32 id)
		{
			return GetUrlForIdReturns;
		}
		
		public Boolean IncrementClickCountByIdCalled { get; set; }
		public void IncrementClickCountById(Int32 id)
		{
			IncrementClickCountByIdCalled = true;
		}
	}
}

