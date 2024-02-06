﻿using Coop.Core.Client.Services.Kingdoms.Messages;
using Coop.IntegrationTests.Environment;
using Coop.IntegrationTests.Environment.Instance;
using GameInterface.Services.Kingdoms.Data;
using GameInterface.Services.Kingdoms.Messages;

namespace Coop.IntegrationTests.Kingdoms
{
    public class KingdomAddDecisionRequestTest
    {
        // Creates a test environment with 1 server and 2 clients by default
        internal TestEnvironment TestEnvironment { get; } = new TestEnvironment();

        /// <summary>
        /// Used to Test that client recieves AddDecision messages after requesting with AddDecisionRequest message.
        /// </summary>
        [Fact]
        public void ClientKingdom_AddDecisionRequest_Publishes_ToServer()
        {
            // Arrange
            var triggerMessage = new LocalDecisionAdded("Kingdom1", new DeclareWarDecisionData("ProposerClan", "Kingdom", 10, true, true, true, "ClanFaction"), true);

            var server = TestEnvironment.Server;
            var client1 = TestEnvironment.Clients.First();

            // Act
            client1.SimulateMessage(this, triggerMessage);

            // Assert
            // Verify the server sends a single message to it's game interface
            Assert.Equal(1, client1.NetworkSentMessages.GetMessageCount<AddDecisionRequest>());
            Assert.Equal(1, server.NetworkSentMessages.GetMessageCount<AddDecision>());

            // Verify the all clients send a single message to their game interfaces
            foreach (EnvironmentInstance client in TestEnvironment.Clients)
            {
                Assert.Equal(1, client.InternalMessages.GetMessageCount<AddDecision>());
            }
        }
    }
}
