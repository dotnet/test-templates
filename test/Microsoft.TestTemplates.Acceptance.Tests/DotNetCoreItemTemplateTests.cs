// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Globalization;

namespace Microsoft.TestTemplates.Acceptance.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.IO;

    [TestClass]
    public class DotNetCoreItemTemplateTests : AcceptanceTestBase
    {
        /// <summary>
        /// The net core versions for which templates are present
        /// </summary>
        private static string[] netCoreVersions = {
            // refer to https://dotnet.microsoft.com/download/dotnet-core
            // for a list of supported dotnet versions and only include the ones
            // that are not end-of-life
            "2.1",
            "3.0",
            "3.1",
        };

        /// <summary>
        /// The type of the test template, combination of the test framework and language
        /// </summary>
        private static readonly (string ProjectTemplateName, string ItemTemplateName, string Language)[] templateTypes = new [] {
            ("nunit", "nunit-test", "c#"),
            ("nunit", "nunit-test", "f#"),
            ("nunit", "nunit-test", "vb"),
        };

        [ClassInitialize]
        public static void InstallTemplates(TestContext testContext)
        {
            foreach (var netcoreVersion in netCoreVersions)
            {
                var template = Path.Combine("template_feed", "Microsoft.DotNet.Test.ProjectTemplates." + netcoreVersion, "content");
                InvokeDotnetNewInstall(template);
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetTemplateItemsToTest), DynamicDataSourceType.Method)]
        public void TemplateItemTest(string targetFramework, string projectTemplate, string itemTemplate, string language)
        {
            var testProjectName = Guid.NewGuid().ToString();
            // Create new test project: dotnet new <projectTemplate> -n <testProjectName> -f <targetFramework> -lang <language>
            InvokeDotnetNew(projectTemplate, testProjectName, targetFramework, language);

            // Add test item to test project: dotnet new <itemTemplate> -n <test> -lang <language> -o <testProjectName>
            InvokeDotnetNew(itemTemplate, "test", language: language, outputDirectory: testProjectName);

            // Run tests: dotnet test <path>
            InvokeDotnetTest(testProjectName);

            // Verfiy the tests run as expected.
            ValidateSummaryStatus(2, 0, 0);
        }

        private static IEnumerable<object[]> GetTemplateItemsToTest()
        {
            foreach (var netcoreVersion in netCoreVersions)
            {
                foreach (var (projectTemplate, itemTemplate, language) in templateTypes)
                {
                    var targetFramework = double.Parse(netcoreVersion, CultureInfo.InvariantCulture) < 5.0
                        ? "netcoreapp" + netcoreVersion
                        : "net" + netcoreVersion;

                    yield return new object[] { targetFramework, projectTemplate, itemTemplate, language};
                }
            }
        }
    }
}
