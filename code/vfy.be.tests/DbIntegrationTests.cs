using System;
using NUnit.Framework;
using System.IO;

namespace vfy.be.tests
{
	[TestFixture]
	public class DBIntegrationTests
	{
		private static String TempFileDb = "./test.db";
		Db _db;
		
		[SetUp]
		public void SetUp()
		{
			_db = new Db(TempFileDb);
			
		}
		
		[TearDown]
		public void CleanUp()
		{
			if(File.Exists(TempFileDb))
			{
				File.Delete(TempFileDb);
			} 
		}
		
		[Test]
		public void InsertUrl_KnownUrl_CanBeRetrievedUsingGetUrlForId()
		{
			var url = "http://test.com";
			var id = _db.InsertUrl(url);
			
			var retrievedUrl = _db.GetUrlForId(id);
			
			Assert.AreEqual(url, retrievedUrl);
		}
		
		[Test]
		public void GetIdForUrl_NoIdForUrl_NullIsReturned()
		{
			var url = "http://test.com";
			
			var retrievedId = _db.GetIdForUrl(url);
			
			Assert.IsFalse(retrievedId.HasValue);
		}
		
		[Test]
		public void GetIdForUrl_IdForUrl_CorrectIdReturned()
		{
			var url = "http://test.com";
			var id = _db.InsertUrl(url);
			
			var retrievedId = _db.GetIdForUrl(url);
			
			Assert.AreEqual(id, retrievedId.Value);
		}
		
		[Test]
		public void IncrementClickCountById_CalledTwice_GettingAlDetailsThenReturnsTwoForCount()
		{
			var url = "http://test.com";
			var id = _db.InsertUrl(url);
			
			_db.IncrementClickCountById(id);
			_db.IncrementClickCountById(id);
			
			var details = _db.GetDetailsFromId(id);
			
			const Int32 expectedClicks = 2;
			Assert.AreEqual(expectedClicks, details.Clicks);
			
		}
	}
}

