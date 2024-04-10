﻿using OAuch.OAuthThreatModel.Consequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.OAuthThreatModel.Threats.ObtainingAuthorization.ResourceOwnerPasswordCredentials {
    public class T4_4_3_3_AutomaticAuthorization : Threat {
        public override string Id => "6819_4_4_3_3";

        public override string Description => "Client Obtains Refresh Token through Automatic Authorization";

        public override IReadOnlyList<ConsequenceType> DependsOn => [ConsequenceTypes.ClientHoldsUserPassword];

        public override IReadOnlyList<ConsequenceType> Consequences => [ConsequenceTypes.PrivilegeEscalation];

        public override string[] Countermeasures => [
            "Use other flows that do not rely on the client's cooperation for resource owner interaction",
            "The authorization server may generally refuse to issue refresh tokens in this flow",
            "The authorization server could notify the resource owner by an appropriate medium, e.g., email, of the refresh token issued"
            ];
    }
}
