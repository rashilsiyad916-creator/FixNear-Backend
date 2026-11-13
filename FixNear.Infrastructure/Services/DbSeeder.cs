using FixNear.Infrastructure.Data;
using FixNear.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProviderServiceEntity = FixNear.Domain.Entities.ProviderService;

namespace FixNear.Infrastructure.Services
{
    public static class DbSeeder
    {
        private record DemoProvider(
            string Email,
            string Name,
            string Business,
            string Category,
            string Description,
            string ServiceName,
            string Area,
            string Address,
            int Experience,
            decimal Price,
            string Image,
            bool Available,
            bool Verified,
            int Rating,
            string Review,
            int DaysAgo,
            string? ServiceImage = null);

        private static readonly DemoProvider[] Catalog = new[]
        {
            new DemoProvider("provider@demo.com", "Demo Provider", "Rahul Electricals", "Electrician",
                "Certified electrician offering residential and commercial wiring, fan installation, and fault finding.",
                "Wiring & Installation", "Tirur", "Near Bus Stand, Tirur", 8, 450,
                "/assets/services/electrician-wiring-pro-v2.jpg", true, true, 5,
                "Very professional and fixed the issue quickly. Highly recommended!", 4),

            new DemoProvider("plumbing@demo.com", "AquaFix Team", "AquaFix Plumbing", "Plumber",
                "Quick plumbing repairs, pipe leakage fixing and bathroom fittings at affordable rates.",
                "Pipe Leakage Repair", "Kottakkal", "Changuvetty Road, Kottakkal", 6, 350,
                "/assets/services/plumber-leak-pro.jpg", true, true, 5,
                "Leak fixed in under an hour. Fair price, no upselling.", 6),

            new DemoProvider("coolair@demo.com", "CoolAir Team", "CoolAir AC Services", "AC Repair",
                "AC service, gas refill, deep cleaning and installation for all brands.",
                "AC Service & Gas Refill", "Malappuram", "Down Hill, Malappuram", 10, 799,
                "/assets/services/ac-repair-pro.jpg", true, true, 5,
                "Cooling is back to normal after the gas refill. Superb service.", 5),

            new DemoProvider("mechanic@demo.com", "Speedy Team", "Speedy Mechanic Works", "Mechanic",
                "Two wheeler and car repair, general service and breakdown assistance.",
                "Bike General Service", "Tirur", "Puthoor Road, Tirur", 5, 550,
                "/assets/services/mechanic-car-pro.jpg", true, false, 4,
                "Honest estimate and solid service. Will use again.", 8),

            new DemoProvider("arun.kumar@demo.com", "Arun Kumar", "Arun Electrical Works", "Electrician",
                "Electrical repairs, wiring and installations for homes and shops.",
                "Electrical Repair", "Tirur", "Market Road, Tirur", 5, 500,
                "/assets/services/electrician-repair-pro-v2.jpg", true, true, 5,
                "Fixed our wiring issue in under an hour. Very professional and clean work.", 7),

            new DemoProvider("rahul.das@demo.com", "Rahul Das", "Rahul Plumbing Care", "Plumber",
                "Leak repairs, pipe fitting and tap installation.",
                "Tap & Pipe Fitting", "Kottakkal", "ST Road, Kottakkal", 7, 400,
                "/assets/services/plumber-fitting-pro.jpg", true, true, 5,
                "Quick leak fix under the sink. Very tidy worker.", 9),

            new DemoProvider("sameer.ali@demo.com", "Sameer Ali", "Sameer Auto Care", "Mechanic",
                "Vehicle servicing and common mechanical repairs.",
                "Car General Service", "Tirur", "Bypass Road, Tirur", 6, 600,
                "/assets/services/mechanic-car-pro.jpg", true, true, 5,
                "Diagnosed the engine noise instantly. Saved me a dealership visit.", 5),

            new DemoProvider("nikhil.raj@demo.com", "Nikhil Raj", "Nikhil Appliance Fix", "Appliance Repair",
                "Basic home appliance inspection and repair.",
                "Appliance Inspection", "Malappuram", "Kadalundi Nagar, Malappuram", 4, 450,
                "/assets/services/appliance-repair-pro-v2.jpg", false, false, 5,
                "Washing machine fixed same day. Great communication throughout.", 3),

            new DemoProvider("ajmal.hassan@demo.com", "Ajmal Hassan", "Ajmal Car Care", "Car Wash",
                "Foam wash, wax polish and interior vacuum detailing.",
                "Foam Wash & Detailing", "Tirur", "Puliyakode Road, Tirur", 3, 250,
                "/assets/services/carwash2.jpg", true, true, 5,
                "Car looked brand new after the foam wash. Very thorough interior cleaning.", 4,
                "assets/services/carwash2.jpg"),

            new DemoProvider("suresh.kumar@demo.com", "Suresh Kumar", "Suresh Wood Works", "Carpenter",
                "Furniture repair, door fitting and custom woodwork.",
                "Furniture Repair", "Kottakkal", "Angadipuram Road, Kottakkal", 9, 600,
                "/assets/services/carpenter-pro.jpg", true, true, 5,
                "Repaired my wardrobe door perfectly. Skilled and neat work.", 10),

            new DemoProvider("riya.thomas@demo.com", "Riya Thomas", "Riya Painters", "Painter",
                "Interior and exterior wall painting with premium paints.",
                "Wall Painting", "Malappuram", "Kuttippuram Road, Malappuram", 5, 400,
                "/assets/services/painter-pro.jpg", true, true, 5,
                "Living room repainted in one day. Clean edges, no mess left behind.", 6),

            new DemoProvider("shalini.menon@demo.com", "Shalini Menon", "Sparkle Cleaning Co", "Cleaning",
                "Deep cleaning for homes, kitchens and bathrooms.",
                "Home Deep Cleaning", "Tirur", "Chokli, Tirur", 4, 350,
                "/assets/services/cleaning-pro.jpg", true, true, 5,
                "Kitchen deep clean was outstanding. Everything sparkled.", 3),

            new DemoProvider("basheer.ahmed@demo.com", "Basheer Ahmed", "Basheer Pest Control", "Pest Control",
                "Safe pest inspection, termite and cockroach treatment.",
                "Termite Treatment", "Kottakkal", "Vengara Road, Kottakkal", 7, 700,
                "/assets/services/pest-control-pro.jpg", true, true, 4,
                "Termite treatment done properly. Explained the process clearly.", 12),

            new DemoProvider("vishnu.prasad@demo.com", "Vishnu Prasad", "Vishnu AC Care", "AC Repair",
                "AC service, gas refill and cooling problem fixes.",
                "Split AC Service", "Malappuram", "Kottakkal Road, Malappuram", 6, 550,
                "/assets/services/ac-service-pro.jpg", true, true, 5,
                "AC gas refill fixed the cooling instantly. Fair charges.", 5),

            new DemoProvider("anjali.krishnan@demo.com", "Anjali Krishnan", "Green Leaf Gardening", "Gardening",
                "Garden cleanup, plant care and landscape design.",
                "Garden Maintenance", "Tirur", "Thanal Road, Tirur", 3, 300,
                "/assets/services/gardening-pro.jpg", true, false, 4,
                "Garden trimmed and cleaned up nicely in two hours.", 9),

            new DemoProvider("manoj.pillai@demo.com", "Manoj Pillai", "Manoj Lock & Key", "Locksmith",
                "Lock repair, key cutting and emergency lockout help.",
                "Emergency Lockout", "Kottakkal", "Town Hall Road, Kottakkal", 8, 450,
                "/assets/services/locksmith-pro-v2.jpg", true, true, 5,
                "Locked out at 10 PM - arrived in 30 minutes and opened the door safely.", 4),

            new DemoProvider("abdul.rahman@demo.com", "Abdul Rahman", "Sun Power Solar Fittings", "Solar Fitting",
                "Rooftop solar panel installation, wiring and annual maintenance.",
                "Solar Panel Installation", "Tirur", "Ottupura Road, Tirur", 7, 800,
                "/assets/services/solar-fitting-pro.jpg", true, true, 5,
                "6kW rooftop solar installed in two days. Subsidy paperwork handled for me.", 8),

            new DemoProvider("rasheed.k@demo.com", "Rasheed K.", "Rasheed Tank Care", "Tank Cleaning",
                "Overhead and underground tank cleaning with high-pressure jet wash.",
                "Overhead Tank Cleaning", "Kottakkal", "Puthiyakadavu, Kottakkal", 5, 500,
                "/assets/services/tank-cleaning-pro.jpg", true, true, 5,
                "Overhead tank cleaned thoroughly. Water is clear now, no more smell.", 6),

            new DemoProvider("manu.varghese@demo.com", "Manu Varghese", "Manu Steel Fabricators", "Welding",
                "Gate grills, steel fabrication and on-site welding repairs.",
                "Gate Grill Fabrication", "Malappuram", "Kureepuzha Road, Malappuram", 10, 600,
                "/assets/services/welding-pro-v2.jpg", true, true, 5,
                "Welded a new steel gate grill. Strong joints, finished same day.", 7),

            new DemoProvider("sreejith.nair@demo.com", "Sreejith Nair", "Sreejith Tiling Works", "Tiling",
                "Floor and wall tiling, granite fixing and grout finishing.",
                "Floor Tiling", "Tirur", "Kadalundi Road, Tirur", 6, 550,
                "/assets/services/tiling-pro.jpg", true, true, 5,
                "Bathroom retiled beautifully. Level finishing, no uneven joints.", 5),

            new DemoProvider("arjun.raj@demo.com", "Arjun Raj", "Arjun Secure Vision", "CCTV Installation",
                "CCTV camera supply, mounting and mobile app setup.",
                "CCTV Installation Package", "Kottakkal", "Nettur Road, Kottakkal", 5, 750,
                "/assets/services/cctv-installation-pro.jpg", true, true, 5,
                "Four CCTV cameras installed with mobile viewing. Clean cable routing.", 4),

            new DemoProvider("deepa.menon@demo.com", "Deepa Menon", "PureFlow RO Care", "Water Purifier",
                "RO service, filter replacement and purifier installation.",
                "RO Filter Replacement", "Malappuram", "Anakkapetta, Malappuram", 4, 400,
                "/assets/services/water-purifier-pro.jpg", true, false, 4,
                "RO filter replaced quickly. Water tastes fine now.", 10),

            new DemoProvider("gopakumar.n@demo.com", "Gopakumar N.", "Gopakumar Movers", "House Shifting",
                "Packing, loading and safe home relocation within Kerala.",
                "Home Shifting (2BHK)", "Tirur", "Mongam Road, Tirur", 9, 800,
                "/assets/services/house-shifting-pro.jpg", true, true, 5,
                "Shifted our 2BHK from Tirur to Kozhikode. Everything arrived safe.", 6),

            new DemoProvider("shafi.muhammad@demo.com", "Shafi Muhammad", "Shafi Ceiling Designs", "False Ceiling",
                "Gypsum and POP ceiling design, fitting and lighting cutouts.",
                "POP Ceiling Work", "Kottakkal", "Kadungathukundu, Kottakkal", 8, 800,
                "/assets/services/false-ceiling-pro.jpg", true, true, 5,
                "POP ceiling for the living room turned out great. Very tidy workers.", 8),

            new DemoProvider("faisal.rahman@demo.com", "Faisal Rahman", "Faisal Mobile Clinic", "Mobile Repair",
                "Screen replacement, battery change and software fixes for all brands.",
                "Screen Replacement", "Tirur", "Jamaat Road, Tirur", 6, 350,
                "/assets/services/mobile-repair-pro.jpg", true, true, 5,
                "Cracked iPhone screen replaced in 30 minutes. Perfect fit, no issues.", 3),

            new DemoProvider("nithin.das@demo.com", "Nithin Das", "Nithin Laptop Service", "Laptop Repair",
                "Laptop hardware repair, OS install, SSD/RAM upgrades and service.",
                "Laptop Hardware Repair", "Kottakkal", "Perinthalmanna Road, Kottakkal", 7, 600,
                "/assets/services/laptop-repair-pro.jpg", true, true, 5,
                "Laptop was dead - fixed the motherboard issue and added an SSD too.", 5),

            new DemoProvider("neha.petcare@demo.com", "Dr. Neha Menon", "Paws & Whiskers Grooming", "Pet Grooming",
                "Gentle doorstep grooming for dogs and cats: warm bath, blow dry, coat styling, nail clipping and ear sanitization.",
                "Dog & Cat Grooming Package", "Kottakkal", "Near Civil Station, Kottakkal", 6, 450,
                "/assets/services/pet-grooming-pro.jpg", true, true, 5,
                "Handled our golden retriever with such care and love. Great coat styling!", 3),

            new DemoProvider("pooja.salon@demo.com", "Pooja Nair", "Glow & Grace Home Salon", "Salon at Home",
                "Certified beautician offering luxury herbal facials, haircut & styling, waxing, manicure and bridal skincare at home.",
                "Facial & Hair Styling at Home", "Malappuram", "Up Hill, Malappuram", 7, 500,
                "/assets/services/salon-at-home-pro.jpg", true, true, 5,
                "Top-notch salon experience without stepping out. Very clean and hygienic kit.", 4),

            new DemoProvider("faheem.power@demo.com", "Faheem Rahman", "VoltShield Inverter Care", "Inverter Service",
                "Home and office inverter diagnostics, tubular battery testing, distilled water refill and solar hybrid conversion.",
                "Inverter Health Check & Service", "Tirur", "Railway Station Road, Tirur", 8, 450,
                "/assets/services/inverter-service-pro.jpg", true, true, 5,
                "Diagnosed our inverter tripping issue within 30 minutes. Solid professional work.", 5),

            new DemoProvider("nihal.reel@demo.com", "Nihal Rahman", "ReelCraft Video Studio", "Photography",
                "Reels, YouTube edits, colour grading and sound cleanup for creators and small businesses.",
                "Reels & YouTube Video Editing", "Tirur", "Thazhepalam, Tirur", 6, 650,
                "/assets/services/photography-pro.jpg", true, true, 5,
                "Turned a messy phone clip into a crisp reel with perfect sound. Delivered a day early.", 4,
                "/assets/services/video-editing-pro-v2.jpg"),

            new DemoProvider("anjali.lens@demo.com", "Anjali Menon", "Lens & Light Photography", "Photography",
                "Event, portrait and product photography with professional retouching and quick delivery.",
                "Event & Portrait Photography", "Kottakkal", "Kottakkal Town, Kottakkal", 7, 750,
                "/assets/services/photography-pro.jpg", true, true, 5,
                "Beautiful candid shots from our family function. Edited gallery arrived in three days.", 5),

            new DemoProvider("sreelakshmi.nurse@demo.com", "Sreelakshmi Nair", "CareAtHome Nursing Services", "Home Nursing",
                "Certified home nurses providing elderly care, post-surgery support, vitals monitoring, medication assistance and compassionate in-home patient care.",
                "Home Nurse Visit & Patient Care", "Tirur", "Chembra Road, Tirur", 9, 800,
                "/assets/services/home-nursing-pro.jpg", true, true, 5,
                "Very caring and professional nurse. Helped my father recover comfortably at home.", 2,
                "assets/services/home-nursing-pro.jpg")
        };

        // Demo services that were offered briefly and should no longer appear.
        // These are removed automatically on startup so every environment stays in sync.
        private static readonly string[] RetiredDemoBusinesses =
        {
            "PixelForge Web Studio",
            "Inkwell Design Co.",
            "WordSmith Content Studio"
        };

        private static readonly string[] RetiredCategoryNames =
        {
            "Video Editing",
            "Web Design",
            "Graphic Design",
            "Content Writing"
        };

        private static readonly (string Name, string Description)[] CategorySeed =
        {
            ("Electrician", "Electrical repairs and installation"),
            ("Plumber", "Plumbing and pipe work"),
            ("Mechanic", "Vehicle repair and maintenance"),
            ("Appliance Repair", "Home appliance fixes"),
            ("Car Wash", "Vehicle washing, waxing and interior detailing"),
            ("Carpenter", "Woodwork, furniture repair and fittings"),
            ("Painter", "Interior and exterior painting services"),
            ("Cleaning", "Home and office deep cleaning"),
            ("Pest Control", "Pest inspection and treatment"),
            ("AC Repair", "Air conditioner service and gas refill"),
            ("Gardening", "Garden care, trimming and landscaping"),
            ("Locksmith", "Lock repair, fitting and key services"),
            ("Solar Fitting", "Solar panel installation and maintenance"),
            ("Tank Cleaning", "Water tank cleaning and sanitisation"),
            ("Welding", "Metal welding and fabrication work"),
            ("Tiling", "Floor and wall tiling services"),
            ("CCTV Installation", "CCTV camera supply and installation"),
            ("Water Purifier", "RO service, filter change and repair"),
            ("House Shifting", "Packing, moving and home relocation"),
            ("False Ceiling", "Gypsum and POP ceiling work"),
            ("Mobile Repair", "Smartphone screen, battery and software fixes"),
            ("Laptop Repair", "Laptop hardware, OS and upgrade service"),
            ("Pet Grooming", "Dog and cat grooming, pet bath and hygiene care"),
            ("Salon at Home", "Professional beauty, haircare and spa services at home"),
            ("Inverter Service", "Inverter installation, battery repair and power backup solutions"),
            ("Photography", "Event, portrait and product photography, plus video editing and post-production"),
            ("Home Nursing", "Professional home nursing, elderly care and post-surgery patient support at home")
        };

        public static void Seed(IServiceProvider services)
        {
            var db = services.GetRequiredService<FixNearDbContext>();
            var hasher = services.GetRequiredService<IPasswordHasher<User>>();
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

            try
            {
                SeedCategories(db);
                EnsureAdminUser(db, hasher);
                var customer = db.Users.FirstOrDefault(u => u.Role == "Customer")
                               ?? EnsureUser(db, hasher, "naheel@gmail.com", "Naheel", "Customer", "12345678");
                var categories = db.Categories.ToDictionary(c => c.Name, c => c.CategoryId);
                var seeded = 0;

                foreach (var def in Catalog)
                {
                    var provider = db.Providers.FirstOrDefault(p => p.BusinessName == def.Business);
                    if (provider == null)
                    {
                        var user = EnsureUser(db, hasher, def.Email, def.Name);
                        provider = new Provider
                        {
                            UserId = user.UserId,
                            BusinessName = def.Business,
                            Description = def.Description,
                            ServiceArea = def.Area,
                            Address = def.Address,
                            IsVerified = def.Verified,
                            Experience = def.Experience,
                            Available = def.Available,
                            Image = def.Image
                        };
                        db.Providers.Add(provider);
                        db.SaveChanges();

                        db.ProviderServices.Add(new ProviderServiceEntity
                        {
                            ProviderId = provider.ProviderId,
                            CategoryId = categories[def.Category],
                            ServiceName = def.ServiceName,
                            Description = def.Description,
                            BasePrice = def.Price,
                            ImageUrl = def.ServiceImage,
                            IsActive = true
                        });
                        db.SaveChanges();
                        seeded++;
                    }
                    else
                    {
                        var changed = false;

                        // Keep the demo photos in sync with the service catalog so
                        // every card shows a picture that matches its service.
                        // Custom uploads (e.g. Cloudinary URLs) are left untouched.
                        if (!string.IsNullOrWhiteSpace(def.Image) &&
                            IsLegacySeedImage(provider.Image) &&
                            !string.Equals(provider.Image, def.Image, StringComparison.Ordinal))
                        {
                            provider.Image = def.Image;
                            changed = true;
                        }

                        var existingService = db.ProviderServices.FirstOrDefault(s => s.ProviderId == provider.ProviderId);
                        if (existingService != null && existingService.BasePrice != def.Price)
                        {
                            existingService.BasePrice = def.Price;
                            changed = true;
                        }

                        // Keep the demo service in the category it now belongs to
                        // (for example, video editing moved under Photography).
                        if (existingService != null && existingService.CategoryId != categories[def.Category])
                        {
                            existingService.CategoryId = categories[def.Category];
                            changed = true;
                        }

                        if (existingService != null &&
                            !string.IsNullOrWhiteSpace(def.ServiceImage) &&
                            string.IsNullOrWhiteSpace(existingService.ImageUrl))
                        {
                            existingService.ImageUrl = def.ServiceImage;
                            changed = true;
                        }

                        // Keep the category description in sync with the catalog.
                        var category = db.Categories.FirstOrDefault(c => c.CategoryId == categories[def.Category]);
                        if (category != null && category.Description != CategorySeed.First(c => c.Name == def.Category).Description)
                        {
                            category.Description = CategorySeed.First(c => c.Name == def.Category).Description;
                            changed = true;
                        }

                        if (changed)
                            db.SaveChanges();
                    }

                    if (customer != null && !db.Reviews.Any(r => r.ProviderId == provider.ProviderId))
                    {
                        var service = db.ProviderServices
                            .Where(s => s.ProviderId == provider.ProviderId && s.IsActive)
                            .OrderBy(s => s.ProviderServiceId)
                            .First();

                        var booking = new Booking
                        {
                            CustomerId = customer.UserId,
                            ProviderServiceId = service.ProviderServiceId,
                            ScheduledAt = DateTime.UtcNow.AddDays(-def.DaysAgo),
                            Address = def.Area + ", Kerala",
                            ProblemDescription = def.ServiceName,
                            Status = "Completed",
                            CreatedAt = DateTime.UtcNow.AddDays(-(def.DaysAgo + 1))
                        };
                        db.Bookings.Add(booking);
                        db.SaveChanges();

                        db.Reviews.Add(new Review
                        {
                            BookingId = booking.BookingId,
                            CustomerId = customer.UserId,
                            ProviderId = provider.ProviderId,
                            Rating = def.Rating,
                            Comment = def.Review,
                            CreatedAt = DateTime.UtcNow.AddDays(-(def.DaysAgo - 1))
                        });
                        db.SaveChanges();
                    }
                }

                RetireLegacyDemoCatalog(db, logger);

                var overpricedServices = db.ProviderServices.Where(s => s.BasePrice > 800).ToList();
                if (overpricedServices.Any())
                {
                    foreach (var s in overpricedServices)
                    {
                        s.BasePrice = 800;
                    }
                    db.SaveChanges();
                    logger.LogInformation("Capped {Count} provider services to 800", overpricedServices.Count);
                }

                if (seeded > 0)
                {
                    logger.LogInformation("Seeded {Count} new demo providers", seeded);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Database seeding skipped/failed: {Message}", ex.Message);
            }
        }

        private static bool IsLegacySeedImage(string? image) =>
            string.IsNullOrWhiteSpace(image)
            || image.StartsWith("https://images.unsplash.com/", StringComparison.OrdinalIgnoreCase)
            || image.StartsWith("https://images.pexels.com/", StringComparison.OrdinalIgnoreCase);

        private static User EnsureUser(FixNearDbContext db, IPasswordHasher<User> hasher, string email, string fullName,
            string role = "Provider", string password = "Password123!")
        {
            var existing = db.Users.FirstOrDefault(u => u.Email == email);
            if (existing != null) return existing;

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = hasher.HashPassword(user, password);
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        /// <summary>
        /// Creates the seeded administrator account used to approve provider
        /// category requests. The role is corrected if the account already exists.
        /// </summary>
        private static void EnsureAdminUser(FixNearDbContext db, IPasswordHasher<User> hasher)
        {
            const string email = "admin@fixnear.app";
            const string password = "Admin123!";

            var admin = db.Users.FirstOrDefault(u => u.Email == email);
            if (admin == null)
            {
                admin = new User
                {
                    FullName = "FixNear Admin",
                    Email = email,
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                admin.PasswordHash = hasher.HashPassword(admin, password);
                db.Users.Add(admin);
                db.SaveChanges();
                return;
            }

            if (admin.Role != "Admin" || !admin.IsActive)
            {
                admin.Role = "Admin";
                admin.IsActive = true;
                db.SaveChanges();
            }
        }

        private static void SeedCategories(FixNearDbContext db)
        {
            var existing = db.Categories.Select(c => c.Name).ToHashSet();
            foreach (var (name, description) in CategorySeed)
            {
                if (!existing.Contains(name))
                {
                    db.Categories.Add(new Category { Name = name, Description = description, IsActive = true });
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Removes demo providers and categories that were retired from the catalog.
        /// Only demo data is touched, and a category is kept whenever any remaining
        /// service still references it, so real provider data is never deleted.
        /// </summary>
        private static void RetireLegacyDemoCatalog(FixNearDbContext db, ILogger logger)
        {
            foreach (var business in RetiredDemoBusinesses)
            {
                var provider = db.Providers.FirstOrDefault(p => p.BusinessName == business);
                if (provider == null) continue;

                var userId = provider.UserId;
                var serviceIds = db.ProviderServices
                    .Where(s => s.ProviderId == provider.ProviderId)
                    .Select(s => s.ProviderServiceId)
                    .ToList();
                var bookingIds = db.Bookings
                    .Where(b => serviceIds.Contains(b.ProviderServiceId))
                    .Select(b => b.BookingId)
                    .ToList();

                db.Reviews.RemoveRange(db.Reviews.Where(r =>
                    r.ProviderId == provider.ProviderId || bookingIds.Contains(r.BookingId)));
                db.Bookings.RemoveRange(db.Bookings.Where(b => serviceIds.Contains(b.ProviderServiceId)));
                db.Favorites.RemoveRange(db.Favorites.Where(f => f.ProviderId == provider.ProviderId));
                db.ProviderServices.RemoveRange(db.ProviderServices.Where(s => s.ProviderId == provider.ProviderId));
                db.Providers.Remove(provider);
                db.SaveChanges();

                var userStillUsed = db.Providers.Any(p => p.UserId == userId)
                                    || db.Bookings.Any(b => b.CustomerId == userId)
                                    || db.Reviews.Any(r => r.CustomerId == userId);
                if (!userStillUsed)
                {
                    var user = db.Users.FirstOrDefault(u => u.UserId == userId);
                    if (user != null)
                    {
                        db.Users.Remove(user);
                        db.SaveChanges();
                    }
                }

                logger.LogInformation("Removed retired demo provider {Business}", business);
            }

            foreach (var name in RetiredCategoryNames)
            {
                var category = db.Categories.FirstOrDefault(c => c.Name == name);
                if (category == null) continue;
                if (db.ProviderServices.Any(s => s.CategoryId == category.CategoryId)) continue;

                db.Categories.Remove(category);
                db.SaveChanges();
                logger.LogInformation("Removed retired category {Category}", name);
            }
        }
    }
}


