using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Pliant.Tests.Integration
{
    /// <summary>
    /// Summary description for NewtonsoftComparisonTests
    /// </summary>
    [TestClass]
    public class NewtonsoftComparisonTests
    {
        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void NewtonsoftCanParseLargeJsonFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(reader))
            {
                while (jsonTextReader.Read())
                {
                }
            }
        }
    }
}