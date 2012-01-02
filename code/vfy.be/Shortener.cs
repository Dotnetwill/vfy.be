using System;
using vfy.be.Interfaces;

namespace vfy.be
{
	public class Shortener : IShortener
	{
		private readonly IShortenerDb _db;
		
		public Shortener (IShortenerDb db)
		{
			if(db == null) throw new ArgumentNullException("db");
			
			_db = db;
		}
		
		public String Shorten(String url)
		{
			url = url.StartsWith("http", StringComparison.CurrentCultureIgnoreCase) ? url : String.Format("http://{0}", url);
			url = url.ToLower();
			
			var existingId = _db.GetIdForUrl(url);
			var id = 0;
			
			if(existingId.HasValue)
			{
				id = existingId.Value;
			}
			else
			{
				id = _db.InsertUrl(url);
			}
			
			return ShortenerMathBits.Encode(id);
		}
		
		public Tuple<String, Int32> Expand(String hash)
		{
			var id = ShortenerMathBits.Decode(hash);
			
			var info = _db.GetDetailsFromId(id);

			if(!String.IsNullOrEmpty(info.Url))
			{
				_db.IncrementClickCountById(id);
			}
			
			return new Tuple<String, Int32>(info.Url, (info.Clicks++));
		}
	}
}

