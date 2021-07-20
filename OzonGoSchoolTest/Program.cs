using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OzonGoSchoolTest
{
	class Program
	{
		static Stopwatch stopwatch = new Stopwatch();

		static void Main(string[] args)
		{
			if(args.Length > 0)
			{
				var input = args[0];
				if (!File.Exists(input))
					Console.WriteLine("Input file doesn't exist.");

				var output = "output.txt";
				if (args.Length > 1)
					output = args[1];

				var text = File.ReadAllText(input);
				var data = text.Select(c => byte.Parse(c.ToString())).ToArray();
				var result = MaxOnesAfterRemoveItem(data);
				File.WriteAllText(output, result.ToString());
				return;
			}


			Console.WriteLine("Arguments are empty, running regular tests:");
			stopwatch.Start();
			//example cases from the task
			CheckResult(new byte[] { 0, 0 }, 0);
			CheckResult(new byte[] { 0, 1 }, 1);
			CheckResult(new byte[] { 1, 0 }, 1);
			CheckResult(new byte[] { 1, 1 }, 1);
			CheckResult(new byte[] { 1, 1, 0, 1, 1 }, 4);
			CheckResult(new byte[] { 1, 1, 0, 1, 1, 0, 1, 1, 1 }, 5);
			CheckResult(new byte[] { 1, 1, 0, 1, 1, 0, 1, 1, 1, 0 }, 5);

			//empty array
			CheckResult(null, 0);
			CheckResult(new byte[] { }, 0);
			//decreasing sequence
			CheckResult(new byte[] { 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, }, 7);
			//starting\ending zeros
			CheckResult(new byte[] { 0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 0}, 7);
			//double zeros
			CheckResult(new byte[] { 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0 }, 4);
			//biggest end
			CheckResult(new byte[] { 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1 }, 5);
			//biggest start with zero
			CheckResult(new byte[] { 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0,  }, 5);
			//biggest end with zero
			CheckResult(new byte[] { 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1 }, 5);

			//check O(n) time complexity
			CheckResult(GetRandomArray(int.MaxValue / 32), -1);
			CheckResult(GetRandomArray(int.MaxValue / 16), -1);
			CheckResult(GetRandomArray(int.MaxValue / 8), -1);
			CheckResult(GetRandomArray(int.MaxValue / 4), -1);
			CheckResult(GetRandomArray(int.MaxValue / 2), -1);
		}


		public static uint MaxOnesAfterRemoveItem(byte[] data)
		{
			if (data == null || data.Length == 0)
				return 0;

			var atLeastOneZero = false;
			uint index = 0, firstBreak = 0, currentLength = 0, longestLength = 0;

			while(index < data.Length) 
			{
				if (data[index] == 1) //increment result if positive
				{
					currentLength++;
					if (longestLength < currentLength)
						longestLength = currentLength;
				}
				else
				{
					if (firstBreak == 0) //skip first zero as 'removed item' 
					{
						firstBreak = index;
						atLeastOneZero = true;
					}
					else 
					{
						if (firstBreak + longestLength + 1 >= data.Length) // no sense to check more if the longest length bigger than length of unchecked items
							break;
						currentLength = index - firstBreak - 1; //treat second zero as first zero of the next checking sequence
						firstBreak = index;
					}
				}

				index++;
			}
			if (!atLeastOneZero && longestLength > 0) //remove '1' if there was no zero
				longestLength--;
			return longestLength;
		}


		#region Helpers

		static void CheckResult(byte[] data, int expectedResult)
		{
			var begin = stopwatch.Elapsed;
			var result = MaxOnesAfterRemoveItem(data);
			if (expectedResult >= 0 && result != expectedResult)
				throw new NotImplementedException($"FAIL. Result: {result}; Expected: {expectedResult}");

			var time = stopwatch.Elapsed - begin;
			Console.WriteLine($"[{data?.Length}]; {time.TotalMilliseconds}ms");
		}

		static byte[] GetRandomArray(uint length)
		{
			var result = new byte[length - 1];
			new Random().NextBytes(result);
			return result;
		}

		#endregion

	}
}
