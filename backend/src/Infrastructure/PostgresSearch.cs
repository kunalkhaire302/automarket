using AutoMarket.Application;
using AutoMarket.Domain;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Infrastructure;

public sealed class PostgresSearch : ICatalogSearch
{
    public IQueryable<Car> Match(IQueryable<Car> cars, string q) => cars.Where(c =>
        (c.Brand + " " + c.Model + " " + c.Variant + " " + c.BodyType).ToLower().Contains(q)
        || EF.Functions.TrigramsSimilarity(c.Brand + " " + c.Model, q) > 0.2);
    public IQueryable<Car> Rank(IQueryable<Car> cars, string q) => q.Length == 0
        ? cars.OrderByDescending(c => c.CreatedAt).ThenBy(c => c.Id)
        : cars.OrderByDescending(c => (c.Brand + " " + c.Model).ToLower() == q)
            .ThenByDescending(c => (c.Brand + " " + c.Model).ToLower().Contains(q))
            .ThenByDescending(c => EF.Functions.TrigramsSimilarity(c.Brand + " " + c.Model, q))
            .ThenByDescending(c => c.CreatedAt).ThenBy(c => c.Id);
}
