using Relativity.API;
using Relativity.Services.Agent;
using Relativity.Services.Interfaces.Agent;
using Relativity.Services.Interfaces.Agent.Models;
using Relativity.Services.ResourceServer;
using Relativity.Test.Helpers.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Test.Helpers.Objects.Agent
{
	public class AgentHelper
	{
		private TestHelper _helper;

		public AgentHelper(TestHelper helper)
		{
			_helper = helper;
		}

		public List<AgentTypeResponse> ReadAgentTypes()
		{
			List<AgentTypeResponse> agentTypes = null;
			using (var agentManager = _helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.System))
			{
			  agentTypes = agentManager.GetAgentTypesAsync(-1).Result;
			}
			return agentTypes;
		}

		public AgentServerResponse WithAnAgentServer()
		{
			List<AgentServerResponse> resourceServers = null;
			using (var agentManager = _helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.System))
			{
				resourceServers = agentManager.GetAgentServersAsync(-1).Result;
			}

			var agentServers = resourceServers.Where(s => s.Type == "Agent");
			return agentServers.First();
		}

		public int Create(AgentTypeRef agentType, ResourceServerRef agentServerRef)
		{
			int agentID = -1;
			using (var agentManager = _helper.GetServicesManager().CreateProxy<IAgentManager>(ExecutionIdentity.System))
			{
				/*
				var agentDto = new AgentRequest()
				{
					AgentType = agentType,
					Enabled = true,
					Interval = 5,
					Server = agentServerRef,
					Keywords = "Integration",
					LoggingLevel = Agent.LoggingLevelEnum.Critical
				};
				agentID = agentManager.CreateAsync(-1, agentDto).Result;
				*/
			}
			return agentID;
		}

	}
}
