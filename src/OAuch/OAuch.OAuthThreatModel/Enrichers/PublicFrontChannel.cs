﻿using OAuch.OAuthThreatModel.Consequences;

namespace OAuch.OAuthThreatModel.Enrichers {
    public class PublicFrontChannel : Enricher {
        public override string Id => "PublicFrontChannel";

        public override string Description => "Flows with the access token in the front channel are public";

        public override ConsequenceType[] DependsOn => [ConsequenceTypes.HasTokenInFrontChannel];

        public override ConsequenceType[] Consequences => [ConsequenceTypes.IsPublicClient];
    }
}
