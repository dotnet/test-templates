namespace Microsoft.TestTemplates.AcceptanceTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.IO;

    [TestClass]
    public class DotnetCoreTemplateTests : IntegrationTestBase
    {
        private static string[] NetCoreVersions = { "1.x", "2.0", "2.1", "2.2", "3.0" };
        private static string[] templateTypes = { "MSTest-CSharp", "MSTest-FSharp", "MSTest-VisualBasic", "XUnit-CSharp", "XUnit-FSharp", "XUnit-VisualBasic" };
 
        [DataTestMethod]
        [DynamicData(nameof(GetTestTemplatesPath), DynamicDataSourceType.Method)]
        public void TemplateTest(string path)
        {
            InvokeDotnetTest(path);
            ValidateSummaryStatus(1, 0, 0);
        }

        private static IEnumerable<object[]> GetTestTemplatesPath()
        {
            var list = new List<string[]>();

            foreach (var netcoreVersion in NetCoreVersions)
            {
                foreach (var templateType in templateTypes)
                {
                    list.Add(new string[] { Path.Combine("template_feed", "Microsoft.DotNet.Test.ProjectTemplates." + netcoreVersion, "content", templateType) });
                }
            }

            return list;
        }
    }
}
