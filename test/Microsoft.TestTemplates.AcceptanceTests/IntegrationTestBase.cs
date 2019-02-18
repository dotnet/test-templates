// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestTemplates.AcceptanceTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    public class IntegrationTestBase
    {
        private const string TestSummaryStatusMessageFormat = "Total tests: {0}. Passed: {1}. Failed: {2}. Skipped: {3}";
        private string standardTestOutput = string.Empty;
        private string standardTestError = string.Empty;
        private int runnerExitCode = -1;

        private string arguments = string.Empty;
        
        public IntegrationTestBase()
        {
        }

        /// <summary>
        /// Invokes <c>vstest.console</c> with specified arguments.
        /// </summary>
        /// <param name="arguments">Arguments provided to <c>vstest.console</c>.exe</param>
        public void InvokeDotnetTest(string arguments)
        {
            this.Execute(arguments, out this.standardTestOutput, out this.standardTestError, out this.runnerExitCode);
            this.FormatStandardOutCome();
        }

        /// <summary>
        /// Validate if the overall test count and results are matching.
        /// </summary>
        /// <param name="passedTestsCount">Passed test count</param>
        /// <param name="failedTestsCount">Failed test count</param>
        /// <param name="skippedTestsCount">Skipped test count</param>
        public void ValidateSummaryStatus(int passedTestsCount, int failedTestsCount, int skippedTestsCount)
        {
            var totalTestCount = passedTestsCount + failedTestsCount + skippedTestsCount;
            if (totalTestCount == 0)
            {
                // No test should be found/run
                var summaryStatus = string.Format(
                    TestSummaryStatusMessageFormat,
                    @"\d+",
                    @"\d+",
                    @"\d+",
                    @"\d+");
                StringAssert.DoesNotMatch(
                    this.standardTestOutput,
                    new Regex(summaryStatus),
                    "Excepted: There should not be test summary{2}Actual: {0}{2}Standard Error: {1}{2}Arguments: {3}{2}",
                    this.standardTestOutput,
                    this.standardTestError,
                    Environment.NewLine,
                    this.arguments);
            }
            else
            {
                var summaryStatus = string.Format(
                    TestSummaryStatusMessageFormat,
                    totalTestCount,
                    passedTestsCount,
                    failedTestsCount,
                    skippedTestsCount);

                Assert.IsTrue(
                    this.standardTestOutput.Contains(summaryStatus),
                    "The Test summary does not match.{3}Expected summary: {1}{3}Test Output: {0}{3}Standard Error: {2}{3}Arguments: {4}{3}",
                    this.standardTestOutput,
                    summaryStatus,
                    this.standardTestError,
                    Environment.NewLine,
                    this.arguments);
            }
        }

        private void Execute(string args, out string stdOut, out string stdError, out int exitCode)
        {
            this.arguments = args;

            using (Process dotnet = new Process())
            {
                Console.WriteLine("IntegrationTestBase.Execute: Starting dotnet.exe");
                dotnet.StartInfo.FileName = "dotnet.exe";
                dotnet.StartInfo.Arguments = "test " + args;
                dotnet.StartInfo.UseShellExecute = false;
                dotnet.StartInfo.RedirectStandardError = true;
                dotnet.StartInfo.RedirectStandardOutput = true;
                dotnet.StartInfo.CreateNoWindow = true;

                var stdoutBuffer = new StringBuilder();
                var stderrBuffer = new StringBuilder();
                dotnet.OutputDataReceived += (sender, eventArgs) => stdoutBuffer.Append(eventArgs.Data).Append(Environment.NewLine);
                dotnet.ErrorDataReceived += (sender, eventArgs) => stderrBuffer.Append(eventArgs.Data).Append(Environment.NewLine);

                Console.WriteLine("IntegrationTestBase.Execute: Path = {0}", dotnet.StartInfo.FileName);
                Console.WriteLine("IntegrationTestBase.Execute: Arguments = {0}", dotnet.StartInfo.Arguments);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                dotnet.Start();
                dotnet.BeginOutputReadLine();
                dotnet.BeginErrorReadLine();
                if (!dotnet.WaitForExit(80 * 1000))
                {
                    Console.WriteLine("IntegrationTestBase.Execute: Timed out waiting for vstest.console.exe. Terminating the process.");
                    dotnet.Kill();
                }
                else
                {
                    // Ensure async buffers are flushed
                    dotnet.WaitForExit();
                }

                stopwatch.Stop();

                Console.WriteLine($"IntegrationTestBase.Execute: Total execution time: {stopwatch.Elapsed.Duration()}");

                stdError = stderrBuffer.ToString();
                stdOut = stdoutBuffer.ToString();
                exitCode = dotnet.ExitCode;

                Console.WriteLine("IntegrationTestBase.Execute: stdError = {0}", stdError);
                Console.WriteLine("IntegrationTestBase.Execute: stdOut = {0}", stdOut);
                Console.WriteLine("IntegrationTestBase.Execute: Stopped vstest.console.exe. Exit code = {0}", exitCode);
            }
        }

        private void FormatStandardOutCome()
        {
            this.standardTestError = Regex.Replace(this.standardTestError, @"\s+", " ");
            this.standardTestOutput = Regex.Replace(this.standardTestOutput, @"\s+", " ");
        }
    }
}
