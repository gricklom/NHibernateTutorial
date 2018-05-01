using FirstSolution.Domain;
using FirstSolution.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System.Linq;

namespace FirstSolution.Tests
{
    [TestClass]
    public class ProductRepositoryFixture
    {
        private static ISessionFactory _sessionFactory;
        private static Configuration _configuration;

        private readonly Product[] _products = new[]
        {
            new Product { Name = "Melon", Category = "Fruits"},
            new Product { Name = "Pear", Category = "Fruits"},
            new Product { Name = "Milk", Category = "Beverages"},
            new Product { Name = "Coca cola", Category = "Beverages"},
            new Product { Name = "Pepsi cola", Category = "Beverages"}
        };

        private void CreateInitialData()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    foreach (var product in _products)
                    {
                        session.Save(product);
                    }
                    transaction.Commit();
                }
            }
        }

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext context)
        {
            _configuration = new Configuration();
            _configuration.Configure();
            _configuration.AddAssembly(typeof(Product).Assembly);
            _sessionFactory = _configuration.BuildSessionFactory();
        }

        [TestInitialize]
        public void SetupContext()
        {
            new SchemaExport(_configuration).Execute(false, true, false);
            CreateInitialData();
        }

        [TestMethod]
        public void CanAddNewProduct()
        {
            var product = new Product { Name = "Apple", Category = "Fruites" };
            var repo = new ProductRepository();
            repo.Add(product);

            // use session to try to load the product
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.IsNotNull(fromDb);
                Assert.AreNotSame(product, fromDb);
                Assert.AreEqual(product.Name, fromDb.Name);
                Assert.AreEqual(product.Category, fromDb.Category);
            }
        }

        [TestMethod]
        public void CanUpdateExistingProduct()
        {
            var product = _products[0];
            product.Name = "Yellow pear";

            var repo = new ProductRepository();
            repo.Update(product);

            // use session to try to load the product
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.AreEqual(product.Name, fromDb.Name);
            }
        }

        [TestMethod]
        public void CanRemoveExistingProduct()
        {
            var product = _products[0];

            var repo = new ProductRepository();
            repo.Remove(product);

            // use session to try to load the product
            using (var session = _sessionFactory.OpenSession())
            {
                var fromDb = session.Get<Product>(product.Id);
                Assert.IsNull(fromDb);
            }
        }

        [TestMethod]
        public void CanGetExistingProductById()
        {
            var repo = new ProductRepository();
            var fromDb = repo.GetById(_products[1].Id);
            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(_products[1], fromDb);
            Assert.AreEqual(_products[1].Name, fromDb.Name);
        }

        [TestMethod]
        public void CanGetExistingProductByName()
        {
            var repo = new ProductRepository();
            var fromDb = repo.GetByName(_products[1].Name);
            Assert.IsNotNull(fromDb);
            Assert.AreNotSame(_products[1], fromDb);
            Assert.AreEqual(_products[1].Id, fromDb.Id);
        }

        [TestMethod]
        public void CanGetExistingProductsByCategory()
        {
            string category = "Fruits";
            var products = _products.Where(i => i.Category == category);

            var repo = new ProductRepository();
            var fromDb = repo.GetByCategory(category);

            Assert.AreEqual(products.Count(), fromDb.Count);
            foreach (var product in products)
            {
                Assert.IsNotNull(fromDb.FirstOrDefault(i => i.Id == product.Id));
            }            
        }

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
    }
}
