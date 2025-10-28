using System.Text;
using BankApiTests.Fixture;

namespace BankApiTests.IntegrationTests
{
    public class ImportFileHelper
    {
        private readonly IntegrationTestFixture _integrationTestFixture;
        private readonly IServiceProvider _serviceProvider;

        public ImportFileHelper(IntegrationTestFixture integrationTestFixture)
        {
            _integrationTestFixture = integrationTestFixture;
            _serviceProvider = _integrationTestFixture.GetServiceProvider();
        }

        private byte[] GetTestData(string filename)
        {
            var testFile = $"{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}/Journey/Messages/{filename}.json";
            return File.ReadAllBytes(testFile);
        }

        public string GetResultData(string filename)
        {
            var testFile = $"{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}/Controller/Expected/{filename}.json";
            return Encoding.UTF8.GetString(File.ReadAllBytes(testFile)).TrimStart('\uFEFF', '\u200B');
        }
    }
}