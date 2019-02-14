namespace Relativity.Test.Helpers
{
	internal interface IHelperTransform
	{
		TestAgentHelper AsTestAgentHelper();

		TestEHHelper AsTestEHHelper();
	}
}