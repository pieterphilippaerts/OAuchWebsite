﻿using OAuch.OAuthThreatModel.Attackers;
using OAuch.OAuthThreatModel.Consequences;

namespace OAuch.OAuthThreatModel.Threats.ObtainingAuthorization.ClientCredentials {
    public class T4_4_4_4_SecretStolenFromDatabase : Threat {
        public override string Id => "6819_4_4_4_4";

        public override string Description => "Obtaining Client Secrets from Authorization Server Database";

        public override ConsequenceType[] DependsOn => [ConsequenceTypes.MachineToMachine];

        public override ConsequenceType[] Consequences => [ConsequenceTypes.ClientAuthenticationSidestepped];

        public override string[] Countermeasures => [
            "Enforce credential storage protection best practices"
            ];
        public override AttackerType[] Attackers => [AttackerTypes.SystemsAttacker];
        public override InvolvedParty[] Parties => [InvolvedParty.TokenEndpoint];
    }
}
