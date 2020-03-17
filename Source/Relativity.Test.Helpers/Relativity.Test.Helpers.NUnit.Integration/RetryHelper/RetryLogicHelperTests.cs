using NUnit.Framework;
using Relativity.Test.Helpers.Exceptions;
using Relativity.Test.Helpers.RetryHelper;
using System;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.NUnit.Integration.RetryHelper
{
	[TestFixture]
	public class RetryLogicHelperTests
	{
		private RetryLogicHelper SuT;

		[SetUp]
		public void SetUp()
		{
			SuT = new RetryLogicHelper();
		}

		[TearDown]
		public void TearDown()
		{

		}

		[TestCase(0, 0)]
		[TestCase(2, 2)]
		public void RetryFunction_Return_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			// Act
			int result = SuT.RetryFunction<int>(numberOfRetries, delayInSeconds, TestFunction);

			// Assert
			Assert.AreEqual(10, result);
		}

		[TestCase(-1, 2)]
		[TestCase(2, -1)]
		public void RetryFunction_Return_Invalid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			// Act
			// Assert
			Assert.Throws<TestHelpersException>(() => { SuT.RetryFunction<int>(numberOfRetries, delayInSeconds, TestFunction); });
		}

		[TestCase(0, 0)]
		[TestCase(2, 2)]
		public async Task RetryFunctionAsync_Return_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			// Act
			int result = await SuT.RetryFunctionAsync<int>(numberOfRetries, delayInSeconds, TestFunctionAsync);

			// Assert
			Assert.AreEqual(10, result);
		}

		[TestCase(-1, 2)]
		[TestCase(2, -1)]
		public async Task RetryFunctionAsync_Return_Invalid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			await Task.Yield();

			// Act
			// Assert
			Assert.ThrowsAsync<TestHelpersException>(async () => { await SuT.RetryFunctionAsync<int>(numberOfRetries, delayInSeconds, TestFunctionAsync); });
		}

		[TestCase(0, 0)]
		[TestCase(2, 2)]
		public void RetryFunction_Void_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			// Act
			// Assert
			Assert.DoesNotThrow(() => { SuT.RetryFunction(numberOfRetries, delayInSeconds, BasicVoidReturnFunction); });
		}

		[TestCase(-1, 2)]
		[TestCase(2, -1)]
		public void RetryFunction_Void_Invalid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			// Act
			// Assert
			Assert.Throws<TestHelpersException>(() => { SuT.RetryFunction(numberOfRetries, delayInSeconds, BasicVoidReturnFunction); });
		}

		[TestCase(0, 0)]
		[TestCase(2, 2)]
		public async Task RetryFunctionAsync_Void_Valid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			await Task.Yield();

			// Act
			// Assert
			Assert.DoesNotThrowAsync(async () => { await SuT.RetryFunctionAsync(numberOfRetries, delayInSeconds, BasicVoidReturnFunctionAsync); });
		}

		[TestCase(0, 0)]
		[TestCase(2, 2)]
		public async Task RetryFunctionAsync_Void_Invalid(int numberOfRetries, int delayInSeconds)
		{
			// Arrange
			await Task.Yield();

			// Act
			// Assert
			Assert.DoesNotThrowAsync(async () => { await SuT.RetryFunctionAsync(numberOfRetries, delayInSeconds, BasicVoidReturnFunctionAsync); });
		}

		public int TestFunction()
		{
			return 10;
		}

		public async Task<int> TestFunctionAsync()
		{
			return await Task.FromResult(10);
		}

		public void BasicVoidReturnFunction()
		{
			Console.WriteLine("Void Return Function called");
		}

		public async Task BasicVoidReturnFunctionAsync()
		{
			await Task.Yield();
			Console.WriteLine("Void Return Function called");
		}
	}
}
