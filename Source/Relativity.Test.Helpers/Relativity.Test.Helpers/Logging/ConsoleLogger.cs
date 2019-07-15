using Relativity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Logging
{
	public class ConsoleLogger : IAPILog
	{
		private string _Context;
		public ConsoleLogger()
		{

		}
		public IAPILog ForContext<T>()
		{
			this._Context = typeof(T).ToString();
			return this;
		}

		public IAPILog ForContext(Type source)
		{
			this._Context = source.ToString();
			return this;
		}

		public IAPILog ForContext(string propertyName, object value, bool destructureObjects)
		{
			this._Context = string.Format("Prop: {0}, Val: {1}", propertyName, value.ToString());
			return this;
		}

		public IDisposable LogContextPushProperty(string propertyName, object obj)
		{
			throw new NotImplementedException();
		}

		public void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			System.Console.WriteLine(string.Format(messageTemplate, propertyValues));
		}

		public void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			System.Console.WriteLine(string.Format("Exception: {0}, {1}", exception.ToString(), string.Format(messageTemplate, propertyValues)));
		}

		public void LogError(string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(messageTemplate, propertyValues);
		}

		public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(exception, messageTemplate, propertyValues);
		}

		public void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(messageTemplate, propertyValues);
		}

		public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(exception, messageTemplate, propertyValues);
		}

		public void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(messageTemplate, propertyValues);
		}

		public void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(exception, messageTemplate, propertyValues);
		}

		public void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(messageTemplate, propertyValues);
		}

		public void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(exception, messageTemplate, propertyValues);
		}

		public void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(messageTemplate, propertyValues);
		}

		public void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.LogDebug(exception, messageTemplate, propertyValues);
		}
	}
}
