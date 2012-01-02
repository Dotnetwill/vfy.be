using System;
using Nancy;

namespace vfy.be
{
	public class Site : NancyModule
	{
		private const String SiteUrl = "http://vfy.be/";
		private readonly IShortener _shortener;
		
		public Site (IShortener shortener)
		{
			_shortener = shortener;
			StaticConfiguration.DisableCaches = true;
			StaticConfiguration.DisableErrorTraces = false;
			Get["/"] = (request) =>  View["index.html"];
			
			Post["/api/shorten-url"] = request => {
				return "Cheese nipples all over" + request.url;
			};
			Post["/api/expand-url"] = request => null;
			
			
		}
		
	}
}

