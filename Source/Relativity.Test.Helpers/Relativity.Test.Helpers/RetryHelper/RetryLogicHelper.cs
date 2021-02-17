using Polly;
using Polly.Retry;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.RetryHelper
{
	public class RetryLogicHelper : IRetryLogicHelper
	{
		/// <summary>
		/// Retry a function that has an expected return
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		public T RetryFunction<T>(int numberOfRetries, int delayInSeconds, Func<T> operation)
		{
			try
			{
				ValidateRetryParameters(numberOfRetries, delayInSeconds, operation);

				TimeSpan[] sleepDurations = Enumerable.Range(1, numberOfRetries).Select(i => TimeSpan.FromSeconds(delayInSeconds)).ToArray();

				RetryPolicy retryPolicy = Polly.Policy
					.Handle<Exception>()
					.WaitAndRetry(sleepDurations);

				return retryPolicy.Execute(operation);
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Retrying operation with return failed to execute without running into more exceptions on every retry", ex);
			}
		}

		/// <summary>
		/// Retry an async function that has an expected return
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		public async Task<T> RetryFunctionAsync<T>(int numberOfRetries, int delayInSeconds, Func<Task<T>> operation)
		{
			try
			{
				ValidateRetryParameters(numberOfRetries, delayInSeconds, operation);

				TimeSpan[] sleepDurations = Enumerable.Range(1, numberOfRetries).Select(i => TimeSpan.FromSeconds(delayInSeconds)).ToArray();

				RetryPolicy retryPolicy = Polly.Policy
					.Handle<Exception>()
					.WaitAndRetryAsync(sleepDurations);

				return await retryPolicy.ExecuteAsync(operation);
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Retrying async operation with return failed to execute without running into more exceptions on every retry", ex);
			}
		}

		/// <summary>
		/// Retry a function that has no return
		/// </summary>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		public void RetryFunction(int numberOfRetries, int delayInSeconds, Action operation)
		{
			try
			{
				ValidateRetryParameters(numberOfRetries, delayInSeconds, operation);

				TimeSpan[] sleepDurations = Enumerable.Range(1, numberOfRetries).Select(i => TimeSpan.FromSeconds(delayInSeconds)).ToArray();

				RetryPolicy retryPolicy = Polly.Policy
					.Handle<Exception>()
					.WaitAndRetry(sleepDurations);

				retryPolicy.Execute(operation);
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Retrying operation with no return failed to execute without running into more exceptions on every retry", ex);
			}
		}

		/// <summary>
		/// Retry an async function that has no return
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		public async Task RetryFunctionAsync(int numberOfRetries, int delayInSeconds, Func<Task> operation)
		{
			try
			{
				ValidateRetryParameters(numberOfRetries, delayInSeconds, operation);

				TimeSpan[] sleepDurations = Enumerable.Range(1, numberOfRetries).Select(i => TimeSpan.FromSeconds(delayInSeconds)).ToArray();

				RetryPolicy retryPolicy = Polly.Policy
					.Handle<Exception>()
					.WaitAndRetryAsync(sleepDurations);

				await retryPolicy.ExecuteAsync(operation);
			}
			catch (Exception ex)
			{
				throw new TestHelpersException("Retrying async operation with no return failed to execute without running into more exceptions on every retry", ex);
			}
		}

		/// <summary>
		/// Basic validation that every function here uses
		/// </summary>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		private void ValidateRetryParameters(int numberOfRetries, int delayInSeconds, object operation)
		{
			if (numberOfRetries < 0) throw new ArgumentException($"Parameter {nameof(numberOfRetries)} cannot be null or less than 0.");
			if (delayInSeconds < 0) throw new ArgumentException($"Parameter {nameof(delayInSeconds)} cannot be null or less than 0.");
			if (operation == null) throw new ArgumentException($"Parameter {nameof(operation)} cannot be null.");
		}


	}
}
