using System;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.RetryHelper
{
	public interface IRetryLogicHelper
	{
		/// <summary>
		/// Retry a function that has an expected return
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		T RetryFunction<T>(int numberOfRetries, int delayInSeconds, Func<T> operation);

		/// <summary>
		/// Retry an async function that has an expected return
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		Task<T> RetryFunctionAsync<T>(int numberOfRetries, int delayInSeconds, Func<Task<T>> operation);

		/// <summary>
		/// Retry a function that has no return
		/// </summary>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		void RetryFunction(int numberOfRetries, int delayInSeconds, Action operation);

		/// <summary>
		/// Retry an async function that has no return
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="numberOfRetries"></param>
		/// <param name="delayInSeconds"></param>
		/// <param name="operation"></param>
		Task RetryFunctionAsync(int numberOfRetries, int delayInSeconds, Func<Task> operation);
	}
}
