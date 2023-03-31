﻿using OAuch.Compliance.Tests.Features;
using OAuch.Compliance.Tests.Shared;
using OAuch.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuch.Compliance.Tests.Tokens {
    public class AccessTokenEntropyMinReqTest : Test {
        public override string Title => "Are the access tokens secure (128 bits)";
        public override string Description => "This test calculates the entropy of the access tokens and verifies that it conforms to the minimum requirements of 128 bits";
        public override string? TestingStrategy => null;
        public override TestResultFormatter ResultFormatter => TestResultFormatter.YesGoodNoBad;
        public override Type ResultType => typeof(AccessTokenEntropyMinReqTestResult);
    }
    public class AccessTokenEntropyMinReqTestResult : TestResult<EntropyInfo> {
        public AccessTokenEntropyMinReqTestResult(string testId) : base(testId) { }
        public override Type ImplementationType => typeof(AccessTokenEntropyMinReqTestImplementation);
    }
    public class AccessTokenEntropyMinReqTestImplementation : EntropyTestImplementationBase {
        public AccessTokenEntropyMinReqTestImplementation(TestRunContext context, AccessTokenEntropyMinReqTestResult result, HasSupportedFlowsTestResult supportedFlows)
            : base(context, result, null, "access tokens", 128, t => t.AccessToken, supportedFlows) { }
    }
}
