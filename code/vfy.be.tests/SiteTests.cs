using System;
using NUnit.Framework;
using Nancy.Testing;
using Nancy;
using Nancy.Helpers;

namespace vfy.be.tests
{
	[TestFixture]
	public class SiteTests
	{
		private ConfigurableBootstrapper _bootstrapper;
		private Browser _browser;
		private FakeShortener _fakeShortener;
		
		[SetUp]
		public void SetUp()
		{
			_fakeShortener = new FakeShortener();

			_bootstrapper = new ConfigurableBootstrapper(with => with.Dependency(_fakeShortener));
			_browser = new Browser(_bootstrapper);
		}
		
		[Test]
		public void ApiShortenUrl_BlankStringSent_400Returned()
		{
			//Arrange
		    //Act
		    var response = _browser.Post("/api/shorten-url", (with) => {
		        with.HttpRequest();
		        with.FormValue("Url", "");
		    });
		
		    //Assert
		    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
		
		[Test]
		public void ApiShortenUrl_BlankBlankRequest_400Returned()
		{
			//Arrange
		    //Act
		    var response = _browser.Post("/api/shorten-url", (with) => {
		        with.HttpRequest();
		        with.Body("");
		    });
		
		    //Assert
			
		    Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
		
		[Test]
		public void ApiShortenUrl_ValidRequest_SiteAddressWithShortcodeReturned()
		{
			//Arrange
			const String shortCode = "a";
			_fakeShortener.ShortenReturnValue = shortCode;
		    //Act
		    var response = _browser.Post("/api/shorten-url", (with) => {
		        with.HttpRequest();
		        with.FormValue("Url", "www.google.com");
		    });
		
		    //Assert
			const String expectedValue = "http://vfy.be/" + shortCode;
		    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(expectedValue, response.Body.AsString());
		}
		
		[Test]
		public void ApiShortenUrl_ValidRequestUrlHasScriptTags_ValueIsHtmlEncodedBeforeSendingToShortener()
		{
			//Arrange
		    //Act
		    _browser.Post("/api/shorten-url", (with) => {
		        with.HttpRequest();
		        with.FormValue("Url","<script>You're a wizard Harry</script>");
		    });
		
		    //Assert
			const String expectedValue = "&lt;script&gt;You're a wizard Harry&lt;/script&gt;";
		    StringAssert.AreEqualIgnoringCase(expectedValue, _fakeShortener.ShortenCalledWith);
		}
		
		[Test]
		public void ApiShortenUrl_ValidRequestUrl_UrlIsStoredCorrectly()
		{
			//Arrange
			const String expectedUrl = "http://google.com";
		    //Act
		    _browser.Post("/api/shorten-url", (with) => {
		        with.HttpRequest();
		        with.FormValue("Url",expectedUrl);
		    });
		
		    //Assert
		    StringAssert.AreEqualIgnoringCase(expectedUrl, _fakeShortener.ShortenCalledWith);
		}
		
		[Test]
		public void ShortCodeLink_ValidShortcode_UserIsRedirected()
		{
			//Arrange
			const String fullLink = "http://google.com/";
			_fakeShortener.ExpandReturns = fullLink;
			
			//Act
			var response = _browser.Get("/a", with => with.HttpRequest());
			
			//Assert
			response.ShouldHaveRedirectedTo(fullLink);
		}
		
		
		[Test]
		public void ShortCodeLink_InvalidShortcode_UserIsRedirected()
		{
			//Arrange
			//Act
			var response = _browser.Get("/a", with => with.HttpRequest());
			
			//Assert
			Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
		}
		
		[Test]
		public void ApiExpandUrl_ValidShortCode_LinkInfoReturned()
		{
			//Arrange
			const String expectedUrl = "www.google.com";
			const Int32 expectedClickCount = 1;
			_fakeShortener.ExpandClickValue = expectedClickCount;
			_fakeShortener.ExpandReturns = expectedUrl;
			
			//Act
			var response = _browser.Get("/api/expand-url", with => 
			{
				with.HttpRequest();
				with.Query("Code", "a");
			});
			
			//Assert
			var res = response.Body.DeserializeJson<Site.DetailsResponse>();
			Assert.AreEqual(expectedUrl, res.Url);
			Assert.AreEqual(expectedClickCount, res.Clicks);
		}
		
		[Test]
		public void ApiExpandUrl_ValidShortCodeIncludingSiteName_JustCodePassedToShortener()
		{
			//Arrange
			const String expectedUrl = "www.google.com";
			const Int32 expectedClickCount = 1;
			_fakeShortener.ExpandClickValue = expectedClickCount;
			_fakeShortener.ExpandReturns = expectedUrl;
			
			//Act
			_browser.Get("/api/expand-url", with => 
			{
				with.HttpRequest();
				with.Query("Code", "vfy.be/a");
			});
			
			//Assert
			Assert.AreEqual("a", _fakeShortener.ExpandCalledWithHash);
		}
		
		[Test]
		public void ApiExpandUrl_ValidShortCodeIncludingSiteNameWithHttp_JustCodePassedToShortener()
		{
			//Arrange
			const String expectedUrl = "www.google.com";
			const Int32 expectedClickCount = 1;
			_fakeShortener.ExpandClickValue = expectedClickCount;
			_fakeShortener.ExpandReturns = expectedUrl;
			
			//Act
			_browser.Get("/api/expand-url", with => 
			{
				with.HttpRequest();
				with.Query("Code", "http://vfy.be/a");
			});
			
			//Assert
			Assert.AreEqual("a", _fakeShortener.ExpandCalledWithHash);
		}
		
		[Test]
		public void ApiExpandUrl_InValidShortCode_ErrorReturnedInJson()
		{
			//Arrange
			//Act
			var response = _browser.Get("/api/expand-url", with => 
			{
				with.HttpRequest();
				with.Query("Code", "a");
			});
			
			//Assert
			var res = response.Body.DeserializeJson<Site.DetailsResponse>();
			Assert.IsFalse(String.IsNullOrEmpty(res.Error));
		}
		
		[Test]
		public void ApiExpandUrl_NoShortCode_ErrorReturnedInJson()
		{
			//Arrange
			//Act
			var response = _browser.Get("/api/expand-url", with => 
			{
				with.HttpRequest();
			});
			
			//Assert
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
		
		[Test]
		public void ApiExpandUrl_ShortCodeIsBlank_ErrorReturnedInJson()
		{
			//Arrange
			//Act
			var response = _browser.Get("/api/expand-url", with => 
			{
				with.HttpRequest();
				with.FormValue("Code", "");
			});
			
			//Assert
			Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
}

