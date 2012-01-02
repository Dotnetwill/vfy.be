using System;
using NUnit.Framework;

namespace vfy.be.tests
{
	[TestFixture]
	public class ShortenerTests
	{
		private FakeDb _fakeDb;
		private Shortener _shortener;
		
		[SetUp]
		public void TestSetup()
		{
			_fakeDb = new FakeDb();
			_shortener = new Shortener(_fakeDb);
		}
		
		[Test]
		public void Shorten_UrlPassedIn_HashedDbIdReturned()
		{
			//arrange
			const Int32 dbReturnedId = 46;
			const String expectedReturnedHash = "1a";
			
			_fakeDb.InsertedUrlId = dbReturnedId;
			
			//Act
			var returnedHash = _shortener.Shorten("url");
			
			//Assert
			Assert.AreEqual(expectedReturnedHash, returnedHash);
		}
		
		[Test]
		public void Shorten_SameUrlPassedTwice_SameIdBothTime()
		{
			//arrange
			const Int32 dbReturnedId = 46;
			_fakeDb.InsertedUrlId = dbReturnedId;
			
			var returnedHash = _shortener.Shorten("url");
			_fakeDb.InsertedUrlId++;
			_fakeDb.GetIdForUrlReturns = dbReturnedId;
			
			//Act
			var secondReturnedHash = _shortener.Shorten("url");
			
			//Assert
			Assert.AreEqual(returnedHash, secondReturnedHash);
		}
		
		[Test]
		public void Shorten_UrlDoesNotStartWithHttp_HttpIsPreappendedBeforeInsert()
		{
			//Act
			_shortener.Shorten("url");
			
			//Assert
			const String expectedUrl = "http://url";
			Assert.AreEqual(expectedUrl, _fakeDb.InsertedUrl);
		}
		
		[Test]
		public void Shorten_UrlDoesStartsWithHttp_HttpIsNotPreappendedBeforeInsert()
		{
			//Arrange
			const String expectedUrl = "http://url";
			
			//Act
			_shortener.Shorten(expectedUrl);
			
			//Assert
			
			Assert.AreEqual(expectedUrl, _fakeDb.InsertedUrl);
		}
		
		[Test]
		public void Expand_UnknownHashPassed_NullReturned()
		{
			//Arrange
			const String hash = "aa";
			
			//Act
			var returnedUrl = _shortener.Expand(hash);
			
			//Assert
			Assert.IsNull(returnedUrl.Item1);
		}
		
		[Test]
		public void Expand_KnownHashPassed_UrlReturned()
		{
			//Arrange
			const String expectedUrl = "url";
			const String hash = "1";
			_fakeDb.GetUrlForIdReturns = expectedUrl;
				
			//Act
			var returnedUrl = _shortener.Expand(hash);
			
			//Assert
			Assert.AreEqual(expectedUrl, returnedUrl.Item1);			
		}
		
		[Test]
		public void Expand_KnownHashPassed_ClickCountIncremented()
		{
			//Arrange
			_fakeDb.GetUrlForIdReturns = "url";
			
			//Act
			_shortener.Expand("hash");
			
			//Assert
			Assert.IsTrue(_fakeDb.IncrementClickCountByIdCalled);
		}
		
		[Test]
		public void Expand_UnknownHashPassed_ClickCountNotIncremented()
		{
			//Act
			_shortener.Expand("hash");
			
			//Assert
			Assert.IsFalse(_fakeDb.IncrementClickCountByIdCalled);
		}
	}
}

