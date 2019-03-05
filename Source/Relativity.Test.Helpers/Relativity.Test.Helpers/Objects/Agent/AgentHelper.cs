using Relativity.API;
using Relativity.Services.Agent;
using Relativity.Services.Interfaces.Agent;
using Relativity.Services.Interfaces.Agent.Models;
using Relativity.Services.ResourceServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Relativity.Test.Helpers.Objects.Agent
{
	public class AgentHelper
	{
		private TestHelper _helper;

		public AgentHelper(TestHelper helper)
		{
			_helper = helper;
		}

		/// <summary>
		/// Returns a list of all available agent types.
		/// </summary>
		/// <returns></returns>
		public List<AgentTypeResponse> ReadAgentTypes()
		{
			List<AgentTypeResponse> agentTypes = null;
			using (var agentManager = _helper.GetServicesManager().CreateProxy<Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
			{
				agentTypes = agentManager.GetAgentTypesAsync(-1).Result;
			}
			return agentTypes;
		}

		/// <summary>
		/// Returns the first resource server where type is equal to 'Agent'
		/// </summary>
		/// <returns></returns>
		public AgentServerResponse ReadAgentServer()
		{
			List<AgentServerResponse> resourceServers = null;
			using (var agentManager = _helper.GetServicesManager().CreateProxy<Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
			{
				resourceServers = agentManager.GetAgentServersAsync(-1).Result;
			}

			var agentServers = resourceServers.Where(s => s.Type == "Agent");
			return agentServers.First();
		}

		/// <summary>
		/// Deprecated: Needs update to newest Agent Manager API
		/// </summary>
		/// <param name="agentType"></param>
		/// <param name="agentServerRef"></param>
		/// <returns></returns>
		public int Create(AgentTypeRef agentType, ResourceServerRef agentServerRef)
		{
			int agentID = -1;
			using (var agentManager = _helper.GetServicesManager().CreateProxy<Services.Interfaces.Agent.IAgentManager>(ExecutionIdentity.System))
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

		/// <summary>
		/// Deprecated: Needs update to newest Agent Manager API
		/// </summary>
		/// <param name="agentID"></param>
		public void Delete(int agentID)
		{
			throw new NotImplementedException();
		}
	}
}