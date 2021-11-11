using System;
using NUnit.Framework.Interfaces;

namespace NURFG
{
    public class LambdaListener : ITestListener
    {
        public Action<TestMessage> SendMessageCallback;
        public void SendMessage(TestMessage message)
            => SendMessageCallback?.Invoke(message);

        public Action<ITestResult> TestFinishedCallback;
        public void TestFinished(ITestResult result)
            => TestFinishedCallback?.Invoke(result);

        public Action<TestOutput> TestOutputCallback;
        public void TestOutput(TestOutput output)
            => TestOutputCallback?.Invoke(output);

        public Action<ITest> TestStartedCallback;
        public void TestStarted(ITest test)
            => TestStartedCallback?.Invoke(test);
    }
}