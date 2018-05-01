using FirstSolution.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace FirstSolution.Tests
{
    [TestClass]
    public class GenerateSchemaFixture
    {
        [TestMethod]
        public void CanGenerateSchema()
        {
            var cfg = new Configuration();
            cfg.Configure();
            cfg.AddAssembly(typeof(Product).Assembly);
            new SchemaExport(cfg).Execute(false, true, false);
        }
    }
}
