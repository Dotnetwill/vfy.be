using System;
using Nancy.Hosting.Self;

namespace vfy.be
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			    // initialize an instance of NancyHost (found in the Nancy.Hosting.Self package)
    		var host = new NancyHost(new Uri("http://127.0.0.1:8888"));    
    		host.Start(); // start hosting

    		Console.ReadKey();    
    		host.Stop();  // stop hosting
		}
	}
}

