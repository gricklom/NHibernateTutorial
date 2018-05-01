using FirstSolution.Domain;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace FirstSolution.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public void Add(Product product)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(product);
                    transaction.Commit();
                }
            }
        }

        public ICollection<Product> GetByCategory(string category)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var products = session.CreateCriteria<Product>().Add(Restrictions.Eq("Category", category)).List<Product>();
                return products;
            }
        }

        public Product GetById(Guid productId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                return session.Get<Product>(productId);
            }
        }

        public Product GetByName(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var product = session.CreateCriteria<Product>().Add(Restrictions.Eq("Name", name)).UniqueResult<Product>();
                return product;
            }
        }

        public void Remove(Product product)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Delete(product);
                    transaction.Commit();
                }
            }
        }

        public void Update(Product product)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(product);
                    transaction.Commit();
                }
            }
        }
    }
}
