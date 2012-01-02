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
		
		public String Expand(String hash)
		{
			var id = ShortenerMathBits.Decode(hash);
			
			var url = _db.GetUrlForId(id);
			if(!String.IsNullOrEmpty(url))
			{
				_db.IncrementClickCountById(id);
			}
			
			return url;
		}
	}
}

