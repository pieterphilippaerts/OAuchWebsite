﻿using OAuch.OAuthThreatModel.Consequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.OAuthThreatModel.Enrichers {
    public class VulnerableAuthCode : Enricher {
        public override string Id => "VulnerableAuthCode";

        public override string Description => "A leaked authorization code can be exchanged for an access token if session and client authentication are not required (or circumvented)";

        public override IReadOnlyList<ConsequenceType> DependsOn => [ConsequenceTypes.AuthorizationCodeLeaked, ConsequenceTypes.SessionAuthenticationSidestepped, ConsequenceTypes.ClientAuthenticationSidestepped];

        public override IReadOnlyList<ConsequenceType> Consequences => [ConsequenceTypes.AccessTokenLeaked];
    }
}
