using AutoMarket.Domain;

namespace AutoMarket.Application;

public interface ICatalogSearch
{
    IQueryable<Car> Match(IQueryable<Car> cars, string normalizedQuery);
    IQueryable<Car> Rank(IQueryable<Car> cars, string normalizedQuery);
}
public sealed class CatalogService(IStore store, ICatalogSearch search)
{
    public IQueryable<CarDto> Project(IQueryable<Car> cars) =>
        from c in cars join city in store.Query<City>() on c.CityId equals city.Id
        select new CarDto(c.Id, c.Brand, c.Model, c.Variant, c.ManufacturingYear, c.RegistrationYear,
            c.Kilometers, c.OwnershipCount, c.FuelType, c.Transmission, c.BodyType, c.Price,
            c.CityId, city.Name, city.State, c.Description, c.Status, c.IsFeatured,
            store.Query<CarImage>().Where(image => image.CarId == c.Id).OrderBy(image => image.SortOrder)
                .Select(image => image.PublicUrl ?? (image.StorageKey.StartsWith("demo/") ? "/demo-cars/" + image.ImageType.ToLower() + ".svg" : null)).FirstOrDefault());

    public async Task<PageResult<CarDto>> Search(SearchRequest r, CancellationToken ct)
    {
        Rules.Require(r.Page is >= 1 and <= 10000 && r.PageSize is >= 1 and <= 100, "Invalid pagination.");
        Rules.Require(r.Q is null || r.Q.Length <= 200, "Search is limited to 200 characters.");
        Rules.Require(r.MinPrice is null or >= 0 && r.MaxPrice is null or >= 0 &&
            (r.MinPrice is null || r.MaxPrice is null || r.MinPrice <= r.MaxPrice), "Invalid price range.");
        Rules.Require(r.MinYear is null || r.MaxYear is null || r.MinYear <= r.MaxYear, "Invalid year range.");
        Rules.Require(r.MinKilometers is null or >= 0 && r.MaxKilometers is null or >= 0 &&
            (r.MinKilometers is null || r.MaxKilometers is null || r.MinKilometers <= r.MaxKilometers), "Invalid mileage range.");
        var q = store.Query<Car>().Where(c => c.Status == "LISTED");
        var cities = store.Query<City>().Where(c => c.IsActive);
        if (r.CityId is not null) cities = cities.Where(c => c.Id == r.CityId);
        if (!string.IsNullOrWhiteSpace(r.City)) cities = cities.Where(c => c.Name.ToLower() == r.City.ToLower());
        if (!string.IsNullOrWhiteSpace(r.State)) cities = cities.Where(c => c.State.ToLower() == r.State.ToLower());
        q = q.Where(c => cities.Select(x => x.Id).Contains(c.CityId));
        if (!string.IsNullOrWhiteSpace(r.Brand)) q = q.Where(c => c.Brand.ToLower() == r.Brand.ToLower());
        if (!string.IsNullOrWhiteSpace(r.Model)) q = q.Where(c => c.Model.ToLower() == r.Model.ToLower());
        if (!string.IsNullOrWhiteSpace(r.Variant)) q = q.Where(c => c.Variant.ToLower() == r.Variant.ToLower());
        if (!string.IsNullOrWhiteSpace(r.FuelType)) q = q.Where(c => c.FuelType == r.FuelType.ToUpper());
        if (!string.IsNullOrWhiteSpace(r.Transmission)) q = q.Where(c => c.Transmission == r.Transmission.ToUpper());
        if (!string.IsNullOrWhiteSpace(r.BodyType)) q = q.Where(c => c.BodyType == r.BodyType.ToUpper());
        if (r.MinPrice.HasValue) q = q.Where(c => c.Price >= r.MinPrice);
        if (r.MaxPrice.HasValue) q = q.Where(c => c.Price <= r.MaxPrice);
        if (r.MinYear.HasValue) q = q.Where(c => c.ManufacturingYear >= r.MinYear);
        if (r.MaxYear.HasValue) q = q.Where(c => c.ManufacturingYear <= r.MaxYear);
        if (r.MinKilometers.HasValue) q = q.Where(c => c.Kilometers >= r.MinKilometers);
        if (r.MaxKilometers.HasValue) q = q.Where(c => c.Kilometers <= r.MaxKilometers);
        if (r.OwnershipCount.HasValue) q = q.Where(c => c.OwnershipCount == r.OwnershipCount);
        if (r.Featured == true) q = q.Where(c => c.IsFeatured);
        var text = string.Join(' ', (r.Q ?? "").Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        if (text.Length > 0) q = search.Match(q, text);
        var count = await store.Count(q, ct);
        q = r.Sort switch
        {
            "price-asc" => q.OrderBy(c => c.Price).ThenBy(c => c.Id),
            "price-desc" => q.OrderByDescending(c => c.Price).ThenBy(c => c.Id),
            "newest" => q.OrderByDescending(c => c.CreatedAt).ThenBy(c => c.Id),
            "mileage" => q.OrderBy(c => c.Kilometers).ThenBy(c => c.Id),
            "relevance" => search.Rank(q, text),
            _ => throw new BusinessException("VALIDATION_ERROR", "Unsupported sort order.")
        };
        return new(await store.List(Project(q.Skip((r.Page - 1) * r.PageSize).Take(r.PageSize)), ct), r.Page, r.PageSize, count);
    }
    public async Task<CarDetailDto> Detail(Guid id, CancellationToken ct)
    {
        var car = await store.First(Project(store.Query<Car>().Where(x => x.Id == id && (x.Status == "LISTED" || x.Status == "BOOKED"))), ct)
            ?? throw new BusinessException("NOT_FOUND", "Vehicle not found.", 404);
        var images = await store.List(store.Query<CarImage>().Where(x => x.CarId == id).OrderBy(x => x.SortOrder), ct);
        return new(car.Id, car.Brand, car.Model, car.Variant, car.ManufacturingYear, car.RegistrationYear,
            car.Kilometers, car.OwnershipCount, car.FuelType, car.Transmission, car.BodyType, car.Price,
            car.CityId, car.City, car.State, car.Description, car.Status, car.IsFeatured, car.PrimaryImageUrl,
            images.Where(image => image.PublicUrl is not null || image.StorageKey.StartsWith("demo/")).Select(image => new CarImageDto(image.ImageType,
                image.PublicUrl ?? "/demo-cars/" + image.ImageType.ToLowerInvariant() + ".svg", image.SortOrder, image.SourceUrl, image.Attribution)).ToList());
    }

    public async Task<Car> Create(Guid owner, CarRequest r, CancellationToken ct)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(r.Brand) && r.Brand.Length <= 80 && !string.IsNullOrWhiteSpace(r.Model) && r.Model.Length <= 80, "Make and model are required (maximum 80 characters).");
        Rules.Require(r.Variant.Length <= 100 && r.Description.Length <= 5000, "Vehicle description or variant is too long.");
        Rules.Require(r.Price is > 0 and <= 100000000 && r.Kilometers is >= 0 and <= 2000000 && r.OwnershipCount is >= 1 and <= 20, "Invalid price, mileage or ownership count.");
        Rules.Require(r.ManufacturingYear >= 1950 && r.RegistrationYear >= r.ManufacturingYear && r.RegistrationYear <= DateTime.UtcNow.Year, "Invalid vehicle year.");
        Rules.Require(new[] { "PETROL", "DIESEL", "CNG", "EV", "HYBRID", "OTHER" }.Contains(r.FuelType), "Invalid fuel type.");
        Rules.Require(new[] { "MANUAL", "AUTOMATIC", "AMT", "CVT", "DCT", "OTHER" }.Contains(r.Transmission), "Invalid transmission.");
        Rules.Require(new[] { "HATCHBACK", "SEDAN", "SUV", "MUV", "COUPE", "OTHER" }.Contains(r.BodyType), "Invalid body type.");
        Rules.Require(await store.First(store.Query<City>().Where(c => c.Id == r.CityId && c.IsActive), ct) is not null, "Choose an active service city.");
        var car = new Car { SellerId = owner, Brand = r.Brand.Trim(), Model = r.Model.Trim(), Variant = r.Variant.Trim(),
            ManufacturingYear = r.ManufacturingYear, RegistrationYear = r.RegistrationYear, Kilometers = r.Kilometers,
            OwnershipCount = r.OwnershipCount, FuelType = r.FuelType, Transmission = r.Transmission,
            BodyType = r.BodyType, Price = r.Price, CityId = r.CityId, Description = r.Description.Trim() };
        store.Add(car);
        await store.Save(ct);
        return car;
    }
}
