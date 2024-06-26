﻿using OAuch.OAuthThreatModel.Consequences;

namespace OAuch.OAuthThreatModel.Enrichers {
    public class NonPKCEFlow : Enricher {
        public override string Id => "OAuch.Compliance.Tests.Pkce.IsPkceRequiredTest";

        public override string Description => "The authorization code flow does not require PKCE. PKCE is the only server-side countermeasure against CSRF and code injection attacks.";

        public override ConsequenceType[] DependsOn => [ConsequenceTypes.HasAuthorizationCode];

        public override ConsequenceType[] Consequences => [ConsequenceTypes.SessionAuthenticationSidestepped];

        protected override bool? RelevancyResult => false;
    }
}
