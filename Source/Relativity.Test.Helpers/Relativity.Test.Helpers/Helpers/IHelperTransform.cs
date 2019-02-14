namespace Relativity.Test.Helpers
{
	interface IHelperTransform
	{
		TestAgentHelper AsTestAgentHelper();

		TestEHHelper AsTestEHHelper();
	}
}