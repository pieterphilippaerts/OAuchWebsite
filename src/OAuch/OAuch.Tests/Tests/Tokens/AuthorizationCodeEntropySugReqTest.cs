﻿using OAuch.Compliance.Tests.Features;
using OAuch.Compliance.Tests.Shared;
using OAuch.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.Compliance.Tests.Tokens {
    public class AuthorizationCodeEntropySugReqTest : Test {
        public override string Title => "Are the authorization codes secure (160 bits)";
        public override string Description => "This test calculates the entropy of the authorization codes and verifies that it conforms to the suggested requirements of 160 bits";
        public override TestResultFormatter ResultFormatter => TestResultFormatter.YesGoodNoBad;
        public override Type ResultType => typeof(AuthorizationCodeEntropySugReqTestResult);
    }
    public class AuthorizationCodeEntropySugReqTestResult : TestResult<EntropyInfo> {
        public AuthorizationCodeEntropySugReqTestResult(string testId) : base(testId) { }
        public override Type ImplementationType => typeof(AuthorizationCodeEntropySugReqTestImplementation);
    }
    public class AuthorizationCodeEntropySugReqTestImplementation : EntropyTestImplementationBase {
        public AuthorizationCodeEntropySugReqTestImplementation(TestRunContext context, AuthorizationCodeEntropySugReqTestResult result, AuthorizationCodeEntropyMinReqTestResult min, HasSupportedFlowsTestResult supportedFlows)
            : base(context, result, min, "authorization codes", 160, t => t.AuthorizationCode, supportedFlows) { }
    }
}
