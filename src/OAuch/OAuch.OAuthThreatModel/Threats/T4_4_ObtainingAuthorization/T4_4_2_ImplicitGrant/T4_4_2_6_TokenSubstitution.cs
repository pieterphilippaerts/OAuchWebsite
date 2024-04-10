﻿using OAuch.OAuthThreatModel.Consequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.OAuthThreatModel.Threats.T4_4_ObtainingAuthorization.T4_4_2_ImplicitGrant {
    public class T4_4_2_6_TokenSubstitution : Threat {
        public override string Id => "6819_4_4_2_6";

        public override string Description => "Token Substitution (OAuth Login)";

        public override IReadOnlyList<ConsequenceType> DependsOn => [ConsequenceTypes.HasTokenInFrontChannel];

        public override IReadOnlyList<ConsequenceType> Consequences => [ConsequenceTypes.PrivilegeEscalation];

        public override string[] Countermeasures => [
            "Clients should use an appropriate protocol, such as OpenID or SAML to implement user login"
            ];
    }
}
