using ShoppingApp.Models.Entities;

namespace ShoppingApp.Models
{
    public static class SeedData
    {
        public static void SeedDatabase(ShoppingContext context)
        {
            seedUserIfNotExist(context);
            seedCategoryIfNotExist(context);
            seedProductIfNotExist(context);
        }

        private static void seedCategoryIfNotExist(ShoppingContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>()
                {
                    new Category(){Name="Book"},
                    new Category(){Name="Electronic"},
                    new Category(){Name="Clothing"},

                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
        }

        private static void seedUserIfNotExist(ShoppingContext context)
        {
            if (!context.Users.Any())
            {
                var users = new List<User>()
                {
                    new User(){FullName = "John Doe", Email = "john@mail.com", Role = "Client",
                               PasswordHash = "3zIc+HSDnvhRFCHtN6DDWZwiR3M9CSrllgAYY5JHqrFGg10cHEv8sj4GTiOhVQ7D5ednhwlolIAcm97GbeqUR8rv8YIAWg==", // password: 12345678
                               PasswordSalt = "kEGg/6iHWdP+QHrcSYG4dtzx+oiAu345SoXzc1hhvqYXxGO+EYqx2a8yYMsT7Q0gt2L2bRxsOkvzTG8FK/2cDCk7Gj5OSw=="},
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
        private static void seedProductIfNotExist(ShoppingContext context)
        {
            if (!context.Products.Any())
            {
                var products = new List<Product>()
                {
                    new Product(){Name="SAMSUNG Galaxy S21 FE 5G Cell Phone", CategoryId = 2, Description = "Second hand mobile phone.", OwnerId = 1,
                                  UnitPrice = 500, ImageUrl = "https://m.media-amazon.com/images/I/91OLd0dFODL._AC_UL480_FMwebp_QL65_.jpg"
                                  },
                    new Product(){Name="Small Crescent Shoulder Bag Under the Arm Purse", CategoryId = 3, Description = "High quality brand handbag, only used for two events.", OwnerId = 1,
                                  UnitPrice = 200, ImageUrl = "https://m.media-amazon.com/images/I/61bY1JDzI4L._AC_UL600_FMwebp_QL65_.jpg"
                                  },
                    new Product(){Name="Chess Story (New York Review Books Classics)", CategoryId = 1, Description = "The book is a bit old and some pages are torn. The price can be bargained.", OwnerId = 1,
                                  UnitPrice = 20, ImageUrl = "https://m.media-amazon.com/images/I/81uY6v33adL._AC_UL600_FMwebp_QL65_.jpg"
                                  },
                    new Product(){Name="Camp Zero: A Novel", CategoryId = 1, Description = "Only used once. The book is in good shape.", OwnerId = 1,
                                  UnitPrice = 4, ImageUrl = "https://m.media-amazon.com/images/I/51ayEcs5tCL.jpg"
                                  },
                    new Product(){Name="HP Laptop", CategoryId = 2, Description = "HP 2023 15'' HD IPS Laptop, Windows 11, Intel Pentium 4-Core Processor Up to 2.70GHz, 8GB RAM, 128GB SSD, HDMI, Super-Fast 6th Gen WiFi, Dale Red (Renewed)", OwnerId = 1,
                                  UnitPrice = 580, ImageUrl = "https://m.media-amazon.com/images/I/712VAsKBk-L._AC_SX679_.jpg"
                                  },
                    new Product(){Name="memzuoix Mouse", CategoryId = 2, Description = "memzuoix 2.4G Wireless Mouse, 1200 DPI Mobile Optical Cordless Mouse with USB Receiver, Portable Computer Mice for Laptop, PC, Desktop, MacBook, 5 Buttons, Red", OwnerId = 1,
                                  UnitPrice = 95, ImageUrl = "https://m.media-amazon.com/images/I/615EiRgcwnL._AC_SX466_.jpg"
                                  },
                    new Product(){Name="BOTHENIAL Shirt", CategoryId = 3, Description = "Women White Button Down Shirt Dressy Casual Work Tops Chiffon Blouse Summer Short Sleeve Shirts", OwnerId = 1,
                                  UnitPrice = 26, ImageUrl = "https://m.media-amazon.com/images/I/710nm18ZEPL._AC_UL1500_.jpg"
                                  },
                    new Product(){Name="Kedomiu Shoes", CategoryId = 3, Description = "Men's Lace Up Oxford Shoes. Brand new.", OwnerId = 1,
                                  UnitPrice = 48, ImageUrl = "https://m.media-amazon.com/images/I/71OkkDpHCJL._AC_UL1500_.jpg"
                                  },
                    new Product(){Name="CRRJU Men's watch", CategoryId = 3, Description = "Mens Watches Ultra-Thin Minimalist Waterproof-Fashion Wrist Watch for Men Unisex Dress with Leather Band", OwnerId = 1,
                                  UnitPrice = 250, ImageUrl = "https://m.media-amazon.com/images/I/61nMMtHOK4L._AC_UL1500_.jpg"
                                  },

                };
                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }

    }
}

