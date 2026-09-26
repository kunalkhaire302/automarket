using AutoMarket.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AutoMarket.Infrastructure;

public sealed record DemoSeedReport(int Cars, int Cities, int Centers, int Images, int Featured);

public static class DemoInventorySeeder
{
    private static readonly string[] ImageTypes = ["FRONT", "REAR", "SIDE", "INTERIOR", "DASHBOARD", "SEATS"];
    private static readonly DemoCity[] Cities =
    [
        new("Mumbai", "Maharashtra", 19.0760, 72.8777), new("Pune", "Maharashtra", 18.5204, 73.8567),
        new("Nashik", "Maharashtra", 19.9975, 73.7898), new("Thane", "Maharashtra", 19.2183, 72.9781),
        new("Navi Mumbai", "Maharashtra", 19.0330, 73.0297), new("Nagpur", "Maharashtra", 21.1458, 79.0882),
        new("Bengaluru", "Karnataka", 12.9716, 77.5946), new("Hyderabad", "Telangana", 17.3850, 78.4867),
        new("Delhi", "Delhi", 28.6139, 77.2090), new("Gurugram", "Haryana", 28.4595, 77.0266),
        new("Noida", "Uttar Pradesh", 28.5355, 77.3910), new("Ahmedabad", "Gujarat", 23.0225, 72.5714),
        new("Surat", "Gujarat", 21.1702, 72.8311), new("Jaipur", "Rajasthan", 26.9124, 75.7873),
        new("Chennai", "Tamil Nadu", 13.0827, 80.2707), new("Kolkata", "West Bengal", 22.5726, 88.3639),
        new("Indore", "Madhya Pradesh", 22.7196, 75.8577), new("Bhopal", "Madhya Pradesh", 23.2599, 77.4126)
    ];
    private static readonly string[] Sellers = ["Prime Auto Hub", "Metro Motors", "Urban Wheels", "CityDrive Motors", "DriveSmart Cars"];
    private static readonly IReadOnlyDictionary<string, CommonsPhoto> CommonsPhotos = new Dictionary<string, CommonsPhoto>(StringComparer.OrdinalIgnoreCase)
    {
        ["Maruti Suzuki"] = P("Maruti Suzuki Swift LXi.jpg", "Julian Lim", "CC BY 2.0"),
        ["Hyundai"] = P("Hyundai Creta SU2id PE 1.5T N Line Creamy White Pearl with Midnight Black roof 02.jpg", "Ethan Llamas", "CC BY-SA 4.0"),
        ["Tata"] = P("Tata tiago.jpg", "Bharadwaj 7645", "CC BY-SA 4.0"),
        ["Honda"] = P("Honda City 1.5 i-VTEC V (VIII, Facelift) – h 22032025.jpg", "M 93", "CC BY-SA 3.0 DE"),
        ["Kia"] = P("KIA SELTOS.jpg", "Reji Jacob", "CC BY-SA 4.0"),
        ["Toyota"] = P("Toyota Fortuner.jpg", "TTTNIS", "CC0 1.0"),
        ["Mahindra"] = P("A black Mahindra XUV700 SUV in Ashiana Brahmananda, Jamshedpur, India (Ank Kumar, Infosys Limited) 03.jpg", "Ank Kumar", "CC BY-SA 4.0")
    };
    private static readonly DemoCar[] Cars =
    [
        D("Maruti Suzuki","Alto K10","VXI",2018,68000,"PETROL","MANUAL",325000,"Mumbai","HATCHBACK"), D("Maruti Suzuki","Wagon R","VXI CNG",2019,55000,"CNG","MANUAL",420000,"Pune","HATCHBACK"),
        D("Maruti Suzuki","Swift","VXI",2020,48000,"PETROL","MANUAL",580000,"Thane","HATCHBACK"), D("Maruti Suzuki","Baleno","Zeta",2021,32000,"PETROL","CVT",720000,"Navi Mumbai","HATCHBACK"),
        D("Maruti Suzuki","Celerio","VXI",2022,25000,"PETROL","AMT",550000,"Nashik","HATCHBACK"), D("Maruti Suzuki","Ignis","Alpha",2020,41000,"PETROL","AMT",590000,"Nagpur","HATCHBACK"),
        D("Hyundai","Grand i10 Nios","Sportz",2021,35000,"PETROL","MANUAL",620000,"Bengaluru","HATCHBACK"), D("Hyundai","i20","Asta",2022,24000,"PETROL","CVT",910000,"Hyderabad","HATCHBACK"),
        D("Tata","Tiago","XT",2019,51000,"PETROL","MANUAL",410000,"Delhi","HATCHBACK"), D("Tata","Altroz","XZ",2021,31000,"PETROL","MANUAL",730000,"Gurugram","HATCHBACK"),
        D("Renault","Kwid","Climber",2018,64000,"PETROL","MANUAL",310000,"Noida","HATCHBACK"), D("Honda","Jazz","VX",2019,45000,"PETROL","CVT",660000,"Ahmedabad","HATCHBACK"),
        D("Maruti Suzuki","S-Presso","VXI",2022,21000,"PETROL","MANUAL",480000,"Surat","HATCHBACK"), D("Hyundai","Santro","Sportz",2020,43000,"PETROL","AMT",460000,"Jaipur","HATCHBACK"),
        D("Tata","Tiago","XZ iCNG",2023,16000,"CNG","MANUAL",700000,"Chennai","HATCHBACK"), D("Maruti Suzuki","Baleno","Delta",2024,9000,"PETROL","AMT",890000,"Kolkata","HATCHBACK"),
        D("Hyundai","Grand i10","Sportz",2017,72000,"PETROL","MANUAL",390000,"Indore","HATCHBACK"), D("Tata","Altroz","XZA",2023,18000,"PETROL","DCT",920000,"Bhopal","HATCHBACK"),
        D("Honda","City","VX",2018,66000,"PETROL","CVT",830000,"Pune","SEDAN"), D("Honda","Amaze","VX",2020,51000,"DIESEL","MANUAL",650000,"Bengaluru","SEDAN"),
        D("Hyundai","Verna","SX",2021,34000,"PETROL","DCT",1250000,"Hyderabad","SEDAN"), D("Maruti Suzuki","Dzire","VXI",2019,59000,"PETROL","AMT",590000,"Delhi","SEDAN"),
        D("Hyundai","Aura","SX CNG",2022,26000,"CNG","MANUAL",730000,"Gurugram","SEDAN"), D("Volkswagen","Virtus","Highline",2023,20000,"PETROL","AUTOMATIC",1450000,"Noida","SEDAN"),
        D("Skoda","Slavia","Style",2023,22000,"PETROL","AUTOMATIC",1570000,"Ahmedabad","SEDAN"), D("Honda","City","ZX",2022,28000,"PETROL","CVT",1400000,"Surat","SEDAN"),
        D("Toyota","Yaris","VX",2018,55000,"PETROL","CVT",860000,"Jaipur","SEDAN"), D("Tata","Tigor","XZ",2021,38000,"PETROL","AMT",610000,"Chennai","SEDAN"),
        D("Maruti Suzuki","Ciaz","Alpha",2019,51000,"PETROL","AUTOMATIC",810000,"Kolkata","SEDAN"), D("Hyundai","Verna","SX(O)",2024,12000,"PETROL","DCT",1780000,"Indore","SEDAN"),
        D("Skoda","Rapid","Rider",2020,48000,"PETROL","AUTOMATIC",820000,"Bhopal","SEDAN"), D("Honda","Amaze","S",2017,83000,"PETROL","MANUAL",430000,"Mumbai","SEDAN"),
        D("Hyundai","Creta","SX",2020,52000,"DIESEL","AUTOMATIC",1450000,"Mumbai","SUV"), D("Hyundai","Creta","SX(O)",2023,21000,"PETROL","DCT",2080000,"Pune","SUV"),
        D("Kia","Seltos","HTX",2021,35000,"PETROL","CVT",1600000,"Bengaluru","SUV"), D("Kia","Seltos","GTX+",2023,23000,"DIESEL","AUTOMATIC",2250000,"Hyderabad","SUV"),
        D("Tata","Nexon","XZ+",2020,47000,"DIESEL","MANUAL",900000,"Delhi","SUV"), D("Tata","Nexon","Fearless",2024,11000,"PETROL","DCT",1490000,"Gurugram","SUV"),
        D("Tata","Punch","Accomplished",2022,28000,"PETROL","AMT",820000,"Noida","SUV"), D("Tata","Punch","Creative",2024,9000,"PETROL","AMT",1010000,"Ahmedabad","SUV"),
        D("Tata","Harrier","XZ",2020,51000,"DIESEL","MANUAL",1680000,"Surat","SUV"), D("Tata","Harrier","Fearless",2024,13000,"DIESEL","AUTOMATIC",2750000,"Jaipur","SUV"),
        D("Mahindra","XUV300","W8",2021,39000,"DIESEL","MANUAL",1100000,"Chennai","SUV"), D("Mahindra","XUV 3XO","AX5",2024,10000,"PETROL","AUTOMATIC",1580000,"Kolkata","SUV"),
        D("Mahindra","XUV700","AX7",2022,26000,"PETROL","AUTOMATIC",2560000,"Indore","SUV"), D("Mahindra","XUV700","AX7L",2024,12000,"DIESEL","AUTOMATIC",3190000,"Bhopal","SUV"),
        D("Mahindra","Thar","LX",2021,34000,"DIESEL","MANUAL",1480000,"Mumbai","SUV"), D("Mahindra","Thar","LX",2023,19000,"PETROL","AUTOMATIC",1890000,"Pune","SUV"),
        D("Toyota","Urban Cruiser","Premium",2021,36000,"PETROL","AUTOMATIC",1020000,"Nashik","SUV"), D("Toyota","Fortuner","4x2",2019,61000,"DIESEL","AUTOMATIC",3250000,"Bengaluru","SUV"),
        D("Toyota","Fortuner Legender","4x2",2022,27000,"DIESEL","AUTOMATIC",4800000,"Hyderabad","SUV"), D("MG","Hector","Sharp",2020,45000,"PETROL","DCT",1580000,"Delhi","SUV"),
        D("MG","Hector Plus","Sharp",2022,31000,"DIESEL","AUTOMATIC",2100000,"Gurugram","SUV"), D("Jeep","Compass","Limited",2019,56000,"DIESEL","AUTOMATIC",1830000,"Noida","SUV"),
        D("Jeep","Compass","Model S",2023,22000,"DIESEL","AUTOMATIC",3150000,"Ahmedabad","SUV"), D("Ford","EcoSport","Titanium",2018,68000,"DIESEL","MANUAL",730000,"Surat","SUV"),
        D("Ford","EcoSport","S",2021,39000,"PETROL","AUTOMATIC",1150000,"Jaipur","SUV"), D("Hyundai","Venue","SX",2020,44000,"PETROL","DCT",940000,"Chennai","SUV"),
        D("Kia","Sonet","HTX",2022,26000,"DIESEL","AUTOMATIC",1340000,"Kolkata","SUV"), D("Tata","Safari","XZ+",2022,30000,"DIESEL","AUTOMATIC",2340000,"Indore","SUV"),
        D("Mahindra","Scorpio N","Z8",2023,18000,"DIESEL","AUTOMATIC",2530000,"Bhopal","SUV"), D("Volkswagen","Taigun","GT",2023,22000,"PETROL","AUTOMATIC",1720000,"Mumbai","SUV"),
        D("Skoda","Kushaq","Style",2022,29000,"PETROL","AUTOMATIC",1670000,"Pune","SUV"), D("Nissan","Magnite","XV Premium",2022,30000,"PETROL","CVT",880000,"Nagpur","SUV"),
        D("Renault","Kiger","RXZ",2021,38000,"PETROL","CVT",840000,"Bengaluru","SUV"), D("Tata","Nexon EV Max","XZ+",2022,27000,"EV","AUTOMATIC",1590000,"Delhi","SUV"),
        D("Tata","Nexon EV","Empowered",2024,10000,"EV","AUTOMATIC",2040000,"Gurugram","SUV"), D("MG","ZS EV","Exclusive",2022,31000,"EV","AUTOMATIC",2150000,"Noida","SUV"),
        D("Kia","EV6","GT Line",2023,18000,"EV","AUTOMATIC",5500000,"Ahmedabad","SUV"), D("Tata","Curvv EV","Empowered+",2025,6000,"EV","AUTOMATIC",2290000,"Surat","SUV"),
        D("Maruti Suzuki","Ertiga","ZXI",2021,42000,"PETROL","AUTOMATIC",1100000,"Jaipur","MUV"), D("Maruti Suzuki","Ertiga","VXI CNG",2022,35000,"CNG","MANUAL",1120000,"Chennai","MUV"),
        D("Kia","Carens","Prestige",2023,22000,"PETROL","MANUAL",1530000,"Kolkata","MUV"), D("Kia","Carens","Luxury+",2024,12000,"DIESEL","AUTOMATIC",2140000,"Indore","MUV"),
        D("Toyota","Innova Crysta","GX",2019,68000,"DIESEL","MANUAL",2050000,"Bhopal","MUV"), D("Toyota","Innova Crysta","ZX",2022,33000,"DIESEL","AUTOMATIC",3150000,"Mumbai","MUV"),
        D("Toyota","Innova Hycross","VX",2024,14000,"HYBRID","AUTOMATIC",3290000,"Pune","MUV"), D("Renault","Triber","RXZ",2021,41000,"PETROL","AMT",700000,"Nagpur","MUV"),
        D("Maruti Suzuki","XL6","Alpha",2022,29000,"PETROL","AUTOMATIC",1420000,"Bengaluru","MUV"), D("Toyota","Rumion","V",2024,11000,"PETROL","AUTOMATIC",1510000,"Hyderabad","MUV")
    ];

    public static async Task<DemoSeedReport> SeedAsync(MarketplaceDb db, CancellationToken ct = default)
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Demo inventory seeding is restricted to Development.");
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var cityByName = new Dictionary<string, City>(StringComparer.OrdinalIgnoreCase);
        foreach (var seed in Cities)
        {
            var city = await db.Set<City>().FirstOrDefaultAsync(x => x.Name == seed.Name && x.State == seed.State, ct);
            if (city is null) { city = new City { Name = seed.Name, State = seed.State, Latitude = seed.Latitude, Longitude = seed.Longitude, ServiceRadiusKm = 35 }; db.Add(city); }
            cityByName[seed.Name] = city;
        }
        await db.SaveChangesAsync(ct);
        foreach (var pair in cityByName)
            if (!await db.Set<ServiceCenter>().AnyAsync(x => x.CityId == pair.Value.Id && x.Name == $"AutoMarket {pair.Key} Viewing Hub", ct))
                db.Add(new ServiceCenter { CityId = pair.Value.Id, Name = $"AutoMarket {pair.Key} Viewing Hub", Address = $"Demo service hub, {pair.Key}", Kind = "INSPECTION", Latitude = pair.Value.Latitude, Longitude = pair.Value.Longitude });
        var sellers = new List<User>();
        foreach (var name in Sellers)
        {
            var email = name.ToLowerInvariant().Replace(" ", ".") + "@inventory.automarket.invalid";
            var seller = await db.Set<User>().FirstOrDefaultAsync(x => x.Email == email, ct);
            if (seller is null) { seller = new User { Name = name, Email = email, Role = "SELLER", EmailVerified = true, Phone = "", IsActive = true }; seller.PasswordHash = new PasswordHasher<User>().HashPassword(seller, "Demo inventory account: sign-in disabled"); db.Add(seller); }
            sellers.Add(seller);
        }
        await db.SaveChangesAsync(ct);
        var added = 0;
        for (var index = 0; index < Cars.Length; index++)
        {
            var source = Cars[index]; var key = $"AUTO-DEMO-{index + 1:000}";
            var existing = await db.Set<Car>().FirstOrDefaultAsync(x => x.InventoryKey == key, ct);
            if (existing is not null)
            {
                await AddOrRefreshImages(db, existing, source.Brand, key, ct);
                continue;
            }
            var status = index switch { 11 or 48 => "BOOKED", 26 or 65 => "SOLD", 43 => "INACTIVE", _ => "LISTED" };
            var featured = new[] { 3, 18, 20, 31, 33, 38, 41, 48, 53, 62, 68, 75 }.Contains(index + 1);
            var car = new Car { InventoryKey = key, SellerId = sellers[index % sellers.Count].Id, CityId = cityByName[source.City].Id,
                Brand = source.Brand, Model = source.Model, Variant = source.Variant, ManufacturingYear = source.Year, RegistrationYear = source.Year,
                Kilometers = source.Kilometers, OwnershipCount = source.Year >= 2022 ? 1 : index % 7 == 0 ? 3 : 2, FuelType = source.Fuel,
                Transmission = source.Transmission, BodyType = source.BodyType, Price = source.Price, Status = status, IsFeatured = featured,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-index), UpdatedAt = DateTimeOffset.UtcNow.AddDays(-index), Description = Description(source, index) };
            db.Add(car); await db.SaveChangesAsync(ct);
            await AddOrRefreshImages(db, car, source.Brand, key, ct);
            added++;
        }
        await db.SaveChangesAsync(ct); await transaction.CommitAsync(ct);
        return new(await db.Set<Car>().CountAsync(x => x.InventoryKey != null, ct), cityByName.Count, await db.Set<ServiceCenter>().CountAsync(x => x.Name.StartsWith("AutoMarket "), ct), await db.Set<CarImage>().CountAsync(x => x.StorageKey.StartsWith("demo/"), ct), await db.Set<Car>().CountAsync(x => x.InventoryKey != null && x.IsFeatured, ct));
    }

    private static DemoCar D(string brand, string model, string variant, int year, int kilometers, string fuel, string transmission, decimal price, string city, string bodyType) => new(brand, model, variant, year, kilometers, fuel, transmission, price, city, bodyType);
    private static CommonsPhoto P(string fileName, string author, string license) => new(fileName, author, license);
    private static async Task AddOrRefreshImages(MarketplaceDb db, Car car, string brand, string key, CancellationToken ct)
    {
        var existing = await db.Set<CarImage>().Where(x => x.CarId == car.Id).ToListAsync(ct);
        CommonsPhotos.TryGetValue(brand, out var photo);
        foreach (var imageType in ImageTypes)
        {
            var image = existing.FirstOrDefault(x => x.ImageType == imageType);
            if (image is null)
            {
                image = new CarImage { CarId = car.Id, StorageKey = $"demo/{key}/{imageType.ToLowerInvariant()}", ImageType = imageType, SortOrder = Array.IndexOf(ImageTypes, imageType) };
                db.Add(image);
            }
            if (photo is not null && imageType == "FRONT")
            {
                image.PublicUrl = photo.ThumbnailUrl;
                image.SourceUrl = photo.SourceUrl;
                image.Attribution = $"{photo.Author} — {photo.License}, via Wikimedia Commons";
            }
        }
        await db.SaveChangesAsync(ct);
    }
    private static string Description(DemoCar car, int index)
    {
        var templates = new[]
        {
            "Well-maintained {0} {1} {2} with {3:N0} km, complete service history and a clean cabin. {4}-owner example in {5}, suited to daily city use and weekend drives.",
            "Carefully inspected {0} {1} {2}; regular maintenance records, tidy interiors and confident road manners. This {4}-owner vehicle has covered {3:N0} km and is available in {5}.",
            "A thoughtfully kept {0} {1} {2} with documented servicing and good tyres. Finished for a practical ownership experience after {3:N0} km; located at our {5} demo hub.",
            "Clean, road-ready {0} {1} {2} with a verified odometer of {3:N0} km. {4}-owner history, useful everyday equipment and a comfortable cabin for city and highway travel.",
            "This {0} {1} {2} has been maintained with care and presents well inside and out. {3:N0} km, {4}-owner history, and a scheduled viewing location in {5}.",
            "Well looked after {0} {1} {2} with service documentation, responsive drivability and a clean interior. {3:N0} km recorded; a {4}-owner vehicle ready for inspection in {5}."
        };
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, templates[index % templates.Length], car.Brand, car.Model, car.Variant, car.Kilometers, car.Year >= 2022 ? "first" : index % 7 == 0 ? "third" : "second", car.City);
    }
    private sealed record DemoCity(string Name, string State, double Latitude, double Longitude);
    private sealed record DemoCar(string Brand, string Model, string Variant, int Year, int Kilometers, string Fuel, string Transmission, decimal Price, string City, string BodyType);
    private sealed record CommonsPhoto(string FileName, string Author, string License)
    {
        public string ThumbnailUrl => "https://commons.wikimedia.org/wiki/Special:FilePath/" + Uri.EscapeDataString(FileName) + "?width=1280";
        public string SourceUrl => "https://commons.wikimedia.org/wiki/File:" + Uri.EscapeDataString(FileName).Replace("%20", "_");
    }
}
