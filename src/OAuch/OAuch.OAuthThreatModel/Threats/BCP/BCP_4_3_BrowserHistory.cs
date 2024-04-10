﻿using OAuch.OAuthThreatModel.Consequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.OAuthThreatModel.Threats.BCP {
    public class BCP_4_3_1_AuthCodeInBrowserHistory : Threat {
        public override string Id => "BCP_4_3_1";

        public override string Description => "Authorization Code in Browser History";

        public override IReadOnlyList<ConsequenceType> DependsOn => [ConsequenceTypes.HasAuthorizationCode];

        public override IReadOnlyList<ConsequenceType> Consequences => [ConsequenceTypes.AuthorizationCodeLeaked];

        public override string[] Countermeasures => [
            "Authorization code replay prevention",
            "Use form post response mode instead of redirect for the authorization response"
            ];
    }
    public class BCP_4_3_2_TokenInApiUri : Threat {
        public override string Id => "BCP_4_3_2_A";

        public override string Description => "Access Token in Browser History";

        public override IReadOnlyList<ConsequenceType> DependsOn => [];

        public override IReadOnlyList<ConsequenceType> Consequences => [ConsequenceTypes.AccessTokenLeaked];

        public override string[] Countermeasures => [
            "Use the authorization code grant or alternative OAuth response modes like the form post response mode"
            ];
    }
    public class BCP_4_3_2_TokenInBrowserHistory : Threat {
        public override string Id => "BCP_4_3_2_B";

        public override string Description => "Access Token in Browser History";

        public override IReadOnlyList<ConsequenceType> DependsOn => [ConsequenceTypes.HasTokenInFrontChannel];

        public override IReadOnlyList<ConsequenceType> Consequences => [ConsequenceTypes.AccessTokenLeaked];

        public override string[] Countermeasures => [
            "Use the authorization code grant or alternative OAuth response modes like the form post response mode"
            ];
    }
}
