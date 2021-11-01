using EmployeeHR.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeHR.Dal.Tests
{
    [TestClass()]
    public static class TestSetup
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            Console.WriteLine("AssemblyInit " + context.TestName);
            string outputPath = context.TestDeploymentDir;


            foreach (DictionaryEntry item in context.Properties)
            {
                if (item.Key.ToString().StartsWith("ENV__"))
                {
                    var k = item.Key.ToString()["ENV__".Length..];
                    var v = item.Value.ToString();
                    Environment.SetEnvironmentVariable(k, v);
                }
            }

            context.Properties.Add("configuration", TestHelper.GetIConfigurationRoot(outputPath));
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");
        }
    }
}
