using System;
using Nancy;
using Nancy.ModelBinding;
using System.Collections.Generic;
using Nancy.Helpers;
using System.Linq;

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
			
			Get["/"] = _ =>  View["index.html"];
			
			Get["/api/expand-url"] = _ => 
			{
				if(!Request.Form.Code.HasValue || String.IsNullOrEmpty(Request.Form.Code))
					return HttpStatusCode.BadRequest;
				
				String code = Request.Form.Code;
				
				if(code.Contains("/"))
				{
					code = code.Split('/').Last();
				}
				
				var info = _shortener.Expand(code);
				var res = new DetailsResponse();
				if(String.IsNullOrEmpty(info.Item1))
				{
					res.Error = "No url found";
				}
				else
				{
					res.Url = info.Item1;
					res.Clicks = info.Item2;
				}
				
				return Response.AsJson(res);
			};
			
			Get["/{shortCode}"] = (arg) =>  
			{
				String realUrl = _shortener.Expand(arg.shortCode).Item1;
				if(String.IsNullOrEmpty(realUrl))
					return HttpStatusCode.NotFound;
				
				return Response.AsRedirect(realUrl);
			};
			
			Post["/api/shorten-url"] = _ => 
			{ 
				if(!Request.Form.Url.HasValue || String.IsNullOrEmpty(Request.Form.Url))
					return HttpStatusCode.BadRequest;
				
				var url = HttpUtility.UrlEncodeUnicode(Request.Form.Url);
				return String.Concat(SiteUrl, _shortener.Shorten(url));
			};
			
		}
		
		public class DetailsResponse
		{
			public String Url { get; set; }
			public Int32? Clicks { get; set; }
			public String Error { get; set; }
		}
		
	}
	
}

