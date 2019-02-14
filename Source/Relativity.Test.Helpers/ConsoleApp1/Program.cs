using Relativity.Test.Helpers;
using Relativity.Test.Helpers.Configuration;
using Relativity.Test.Helpers.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = ConfigurationFactory.ReadConfigFromAppSettings();
			var helper = new TestHelper(config);

		}
	}
}
