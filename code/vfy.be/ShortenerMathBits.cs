using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace vfy.be
{
	/// <summary>
	/// Computes and decodes hashes from base 36 using the alogrithm talked about here http://www.cs.umd.edu/class/sum2003/cmsc311/Notes/Data/toBaseK.html
	/// </summary>
	public static class ShortenerMathBits
	{
		private static readonly char[] baseChars = new[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'};
		private const Int32 NumberBase = 36;
		
		public static String Encode(Int32 decimalValue)
		{
			var sb = new StringBuilder();	
			sb.Append(ComputeHashChar(decimalValue).Reverse().ToArray());
			return sb.ToString();
		}
		
		private static IEnumerable<Char> ComputeHashChar(Int32 decimalValue)
		{
			if(decimalValue == 0)  return new Char[0];
			
			var	newBaseNum = decimalValue % NumberBase;
			return new[] { baseChars[newBaseNum] }.Concat(ComputeHashChar(decimalValue / NumberBase));
		}
		
		public static Int32 Decode(String hash)
		{
			var chars = hash.ToLower().ToCharArray().Reverse();
			return chars.Select((c, i) => IntPow(NumberBase, (uint)i) * Array.IndexOf(baseChars, c)).Sum();
		}
		
		private static Int32 IntPow(int x, uint pow)
		{
			//Nicked from SO, http://stackoverflow.com/questions/383587/how-do-you-do-integer-exponentiation-in-c
			//Surpised this isn't part of the std lib, yes I know about math.pow but that's double.  This is ints.
    		int ret = 1;
    		while ( pow != 0 )
    		{
		        if ( (pow & 1) == 1 )
		            ret *= x;
		        x *= x;
		        pow >>= 1;
		    }
		    return ret;
		}

	}
}

