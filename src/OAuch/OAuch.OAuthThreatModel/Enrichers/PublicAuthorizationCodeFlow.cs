﻿using OAuch.OAuthThreatModel.Consequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.OAuthThreatModel.Enrichers {
    public class PublicAuthorizationCodeFlow : Enricher {
        public override string Id => "OAuch.Compliance.Tests.TokenEndpoint.IsClientAuthenticationRequiredTest";

        public override string Description => "The client is a public client because it does not use a client secret.";

        public override ConsequenceType[] DependsOn => [ConsequenceTypes.HasAuthorizationCode];

        public override ConsequenceType[] Consequences => [ConsequenceTypes.ClientAuthenticationSidestepped];

        protected override bool? RelevancyResult => false;
    }
}
