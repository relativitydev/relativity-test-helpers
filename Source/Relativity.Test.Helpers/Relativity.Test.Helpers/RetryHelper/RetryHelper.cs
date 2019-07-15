using System;

using System.Threading.Tasks;

public static class RetryHelper
{
	public static void RetryOnException(int times, TimeSpan delay, Action operation)
	{
		var attempts = 0;
		do
		{
			try
			{
				attempts++;
				operation();
				break; // Sucess! Lets exit the loop!
			}
			catch (Exception ex)
			{
				if (attempts == times)
					throw;

				Console.WriteLine($"Exception caught on attempt {attempts} - will retry after delay {delay}", ex);

				Task.Delay(delay).Wait();
			}
		} while (true);
	}
}