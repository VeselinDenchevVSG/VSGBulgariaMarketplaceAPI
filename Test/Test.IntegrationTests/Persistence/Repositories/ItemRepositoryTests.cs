namespace Test.IntegrationTests.Persistence.Repositories
{
    using Dapper;

    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;

    using Test.IntegrationTests.Extensions;

    using VSGBulgariaMarketplace.Application.Models.Image.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
    using VSGBulgariaMarketplace.Application.Models.Order.Interfaces;
    using VSGBulgariaMarketplace.Domain.Entities;
    using VSGBulgariaMarketplace.Domain.Enums;
    using VSGBulgariaMarketplace.Persistence.Repositories;

    using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

    internal class ItemRepositoryTests : RepositoryTests
    {
        private static bool isImageCreated = false;
        private static bool isItemLoanCreated = false;
        private readonly IItemRepository itemRepository;
        private readonly IImageRepository imageRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IItemLoanRepository itemLoanRepository;

        public ItemRepositoryTests() 
            : base()
        {
            this.itemRepository = new ItemRepository(base.unitOfWork);
            this.imageRepository = new ImageRepository(base.unitOfWork);
            this.orderRepository = new OrderRepository(base.unitOfWork);
            this.itemLoanRepository = new ItemLoanRepository(base.unitOfWork);
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            if (isItemLoanCreated)
            {
                await base.TruncateTableAsync(ITEM_LOANS_TABLE_NAME);
                isItemLoanCreated = false;
            }

            await base.TruncateTableAsync(ITEMS_TABLE_NAME);
            if (isImageCreated)
            {
                await base.TruncateTableAsync(CLOUDINARY_IMAGES_TABLE_NAME);
                isImageCreated = false;
            }

        }

        [Test]
        public async Task CreateAsync_WithNullProperties_CreatesItem()
        {
            // Arrange
            Item itemToBeCreated = new()
            {
                Code = "test",
                Name = "test",
                ImagePublicId = null,
                Price = null,
                Category = Category.Misc,
                QuantityCombined = 1,
                QuantityForSale = null,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };

            await this.CreateAsyncCreatesItemBaseAsync(itemToBeCreated);
        }

        [Test]
        public async Task CreateAsyncWithNotNullProperties_CreatesItem()
        {
            // Arrange
            CloudinaryImage image = new()
            {
                Id = Guid.NewGuid().ToString().Substring(0, 20),
                FileExtension = ".png",
                Version = 0
            };
            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                await this.imageRepository.CreateAsync(image, default);
            }
            isImageCreated = true;

            Item itemToBeCreated = new()
            {
                Code = "test",
                Name = "test",
                ImagePublicId = image.Id,
                Price = 123.45m,
                Category = Category.Misc,
                QuantityCombined = 3,
                QuantityForSale = 1,
                AvailableQuantity = 1,
                Description = "test",
                Location = Location.Home
            };

            await this.CreateAsyncCreatesItemBaseAsync(itemToBeCreated);
        }

        [Test]
        public async Task UpdateAsync_WithNotNullProperties_UpdatesItem()
        {
            // Arrange
            Item itemToBeCreated = new()
            {
                Code = "test",
                Name = "test",
                Price = null,
                Category = Category.Misc,
                QuantityCombined = 2,
                QuantityForSale = null,
                AvailableQuantity = null,
                Description = "test",
                Location = Location.Home
            };
            Item itemToBeUpdated = new()
            {
                Code = "updated",
                Name = "updated",
                Price = 123.45m,
                Category = Category.OfficeTools,
                QuantityCombined = 3,
                QuantityForSale = 1,
                AvailableQuantity = 1,
                Description = "updated",
                Location = Location.VelikoTarnovo
            };

            await this.UpdateAsyncUpdatesItemTestAsync(itemToBeCreated, itemToBeUpdated, false);
        }

        [Test]
        public async Task UpdateAsync_WithNullProperties_UpdatesItem()
        {
            // Arrange
            Item itemToBeCreated = new()
            {
                Code = "test",
                Name = "test",
                Price = 123.45m,
                Category = Category.Misc,
                QuantityCombined = 3,
                QuantityForSale = 1,
                AvailableQuantity = 1,
                Description = "test",
                Location = Location.Home
            };
            Item itemToBeUpdated = new()
            {
                Code = "updated",
                Name = "updated",
                Price = null,
                Category = Category.OfficeTools,
                QuantityCombined = 2,
                QuantityForSale = null,
                AvailableQuantity = null,
                Description = "updated",
                Location = Location.VelikoTarnovo
            };

            await this.UpdateAsyncUpdatesItemTestAsync(itemToBeCreated, itemToBeUpdated, true);
        }

        [Test]
        public async Task Delete_ItemNotLinkedToAnything_DeletesItem()
        {
            // Arrange
            Item createdItem = new()
            {
                Code = "test",
                Name = "test",
                ImagePublicId = null,
                Price = null,
                Category = Category.Misc,
                QuantityCombined = 1,
                QuantityForSale = null,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };

            bool isDeleted = false;
            string sql = "SELECT 1 FROM Items WHERE Id = @Id";
            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                await this.itemRepository.CreateAsync(createdItem, default);
            // Act
                this.itemRepository.Delete(createdItem.Id);
                isDeleted = !(await connection.ExecuteScalarAsync<bool>(sql, new { Id = createdItem.Id }));
            }

            // Assert
            isDeleted.Should().BeTrue();
        }

        [Test]
        public async Task Delete_ItemLinkedToOrder_DeletesItem()
        {
            // Arrange
            Item createdItem = new()
            {
                Code = "test",
                Name = "test",
                ImagePublicId = null,
                Price = 123.45m,
                Category = Category.Misc,
                QuantityCombined = 1,
                QuantityForSale = 1,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };
            Order order = new()
            {
                ItemCode = createdItem.Code,
                ItemName = createdItem.Name,
                ItemPrice = 123.45m,
                Quantity = 1,
                Email = "vdenchev@vsgbg.com",
                Status = OrderStatus.Pending
            };

            bool isDeleted = false;
            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                await this.itemRepository.CreateAsync(createdItem, default);
                order.ItemId = createdItem.Id;
                this.orderRepository.Create(order);

                // Act
                try
                {
                    this.itemRepository.Delete(createdItem.Id);
                }
                catch (SqlException)
                {
                    // In case it fails
                }

                string sql = "SELECT 1 FROM Items WHERE Id = @Id";
                isDeleted = !(await connection.ExecuteScalarAsync<bool>(sql, new { Id = createdItem.Id }));
            }

            // Assert
            isDeleted.Should().BeTrue();
        }

        [Test]
        public async Task Delete_ItemLinkedToItemLoan_DoesNotDeleteItem()
        {
            // Arrange
            Item createdItem = new()
            {
                Code = "test",
                Name = "test",
                ImagePublicId = null,
                Price = 123.45m,
                Category = Category.Misc,
                QuantityCombined = 3,
                QuantityForSale = 1,
                AvailableQuantity = 1,
                Description = null,
                Location = Location.Home
            };
            ItemLoan itemLoan = new()
            {
                Email = "vdenchev@vsgbg.com",
                Quantity = 1,
                EndDatetimeUtc = null
            };

            bool isDeleted = false;
            string sql = "SELECT 1 FROM Items WHERE Id = @Id";
            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                await this.itemRepository.CreateAsync(createdItem, default);
                itemLoan.ItemId = createdItem.Id;
                this.itemLoanRepository.Create(itemLoan);
                isItemLoanCreated = true;

                // Act
                try
                {
                    this.itemRepository.Delete(createdItem.Id);
                }
                catch (SqlException)
                {
                    // Expected result is should fail because the item is linked to item loan
                }
                isDeleted = !(await connection.ExecuteScalarAsync<bool>(sql, new { Id = createdItem.Id }));
            }

            // Assert
            isDeleted.Should().BeFalse();
        }

        [Test]
        public async Task GetMarketplace_ReturnsItemsWithQuantityForSaleGreaterThanZero()
        {
            // Arrange
            Item itemWithNullQuantityForSale = new()
            {
                Code = "test1",
                Name = "test1",
                Price = null,
                Category = Category.Misc,
                QuantityCombined = 1,
                QuantityForSale = null,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };
            Item itemWithZeroQuantityForSale = new()
            {
                Code = "test2",
                Name = "test2",
                Price = 123.45m,
                Category = Category.Misc,
                QuantityCombined = 3,
                QuantityForSale = 0,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };
            Item[] itemsWithNullOrZeroQuantityForSale = [ itemWithNullQuantityForSale, itemWithZeroQuantityForSale ];

            Item item1WithQuantityForSaleGreaterThanZero = new()
            {
                Code = "test3",
                Name = "test3",
                Price = null,
                Category = Category.Misc,
                QuantityCombined = 3,
                QuantityForSale = 1,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };
            Item item2WithQuantityForSaleGreaterThanZero = new()
            {
                Code = "test4",
                Name = "test4",
                Price = 123.45m,
                Category = Category.Misc,
                QuantityCombined = 3,
                QuantityForSale = 2,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };
            Item[] itemsWithQuantityForSaleGreaterThanZero = new[] { item1WithQuantityForSaleGreaterThanZero, item2WithQuantityForSaleGreaterThanZero };

            Item[] marketplace;
            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                await this.itemRepository.CreateAsync(itemWithNullQuantityForSale, default);
                await this.itemRepository.CreateAsync(itemWithZeroQuantityForSale, default);
                await this.itemRepository.CreateAsync(item1WithQuantityForSaleGreaterThanZero, default);
                await this.itemRepository.CreateAsync(item2WithQuantityForSaleGreaterThanZero, default);

            // Act
                marketplace = this.itemRepository.GetMarketplace();
            }

            // Assert
            using (new AssertionScope())
            {
                marketplace.Length.Should().Be(2);

                using (new AssertionScope())
                {
                    foreach (Item itemWithNullOrZero in itemsWithNullOrZeroQuantityForSale)
                    {
                        marketplace.Should().NotContain(i =>
                            i.Id == itemWithNullOrZero.Id &&
                            i.Code == itemWithNullOrZero.Code &&
                            i.Price == itemWithNullOrZero.Price &&
                            i.Category == itemWithNullOrZero.Category &&
                            i.QuantityForSale == itemWithNullOrZero.QuantityForSale
                        );
                    }
                }

                using (new AssertionScope())
                {
                    foreach (Item itemWithQuantityForSaleGreaterThanZero in itemsWithQuantityForSaleGreaterThanZero)
                    {
                        marketplace.Should().Contain(i =>
                            i.Id == itemWithQuantityForSaleGreaterThanZero.Id &&
                            i.Code == itemWithQuantityForSaleGreaterThanZero.Code &&
                            i.Price == itemWithQuantityForSaleGreaterThanZero.Price &&
                            i.Category == itemWithQuantityForSaleGreaterThanZero.Category
                        );
                    }
                }
            }

        }

        [Test]
        [TestCase(null)]
        [TestCase(1)]
        public async Task GetById_ItemQuantityForSale_ReturnsItemIfItsQuantityForSaleIsNotNull(short? quantityForSale)
        {
            // Arrange
            bool beEquivalent = quantityForSale is not null;
            Item itemBeforeCreation = new()
            {
                Code = "test",
                Name = "test",
                ImagePublicId = null,
                Price = null,
                Category = Category.Misc,
                QuantityCombined = 1,
                QuantityForSale = quantityForSale,
                AvailableQuantity = null,
                Description = null,
                Location = Location.Home
            };
            Item? itemAfterCreation;

            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                await this.itemRepository.CreateAsync(itemBeforeCreation, default);
            
            // Act
                itemAfterCreation = this.itemRepository.GetById(itemBeforeCreation.Id);
            }

            // Assert
            this.AssertGetById(itemBeforeCreation, itemAfterCreation, beEquivalent);
        }

        private async Task CreateAsyncCreatesItemBaseAsync(Item itemToBeCreated)
        {
            // Act
            string sql = "SELECT * FROM Items ORDER BY CreatedAtUtc";
            IEnumerable<Item> itemsBeforeCreation;
            IEnumerable<Item> itemsAfterCreation;
            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                itemsBeforeCreation = await connection.QueryAsync<Item>(sql);
                await this.itemRepository.CreateAsync(itemToBeCreated, default);
                itemsAfterCreation = await connection.QueryAsync<Item>(sql);
            }

            DateTime utcNow = DateTime.UtcNow;

            Item? createdItem = itemsAfterCreation.MaxBy(i => i.CreatedAtUtc);

            // Assert
            createdItem.Should().NotBeNull();
            using (new AssertionScope())
            {
                itemsAfterCreation.Count().Should().Be(itemsBeforeCreation.Count() + 1);
                itemToBeCreated.Should().BeEquivalentTo(createdItem, options => options.Excluding(i => i.CreatedAtUtc)
                                                                                       .Excluding(i => i.ModifiedAtUtc));
                itemToBeCreated.CreatedAtUtc.Should().Be(itemToBeCreated.ModifiedAtUtc);
                itemToBeCreated.CreatedAtUtc.RoundToNearestSecond().Should().Be(createdItem!.CreatedAtUtc.RoundToNearestSecond());
                itemToBeCreated.ModifiedAtUtc.RoundToNearestSecond().Should().Be(createdItem.ModifiedAtUtc.RoundToNearestSecond());
                createdItem.CreatedAtUtc.Should().BeBefore(utcNow);
                createdItem.CreatedAtUtc.Should().Be(createdItem.ModifiedAtUtc);
            }
        }

        private async Task UpdateAsyncUpdatesItemTestAsync(Item itemToBeCreated, Item itemToBeUpdated, bool hasImageOnCreate)
        {
            bool hasImageOnUpdate = !hasImageOnCreate;

            CloudinaryImage image = new()
            {
                Id = Guid.NewGuid().ToString().Substring(0, 20),
                FileExtension = ".png",
                Version = 0
            };

            string sql = "SELECT * FROM Items";
            IEnumerable<Item> itemsAfterCreation;
            await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                if (hasImageOnCreate)
                {
                    await this.imageRepository.CreateAsync(image, default);
                    itemToBeCreated.ImagePublicId = image.Id;
                    isImageCreated = true;
                }
                await this.itemRepository.CreateAsync(itemToBeCreated, default);
                itemsAfterCreation = await connection.QueryAsync<Item>(sql);
            }

            Item? createdItem = itemsAfterCreation.MaxBy(i => i.CreatedAtUtc);
            ArgumentNullException.ThrowIfNull(createdItem);
            itemToBeUpdated.Id = createdItem.Id;

            // Wait for 1ms so we are sure createdItem.ModifiedAtUtc and updatedItem.ModifiedAtUtc are different
            Thread.Sleep(1);

            // Act
            IEnumerable<Item> itemsAfterUpdate;
            using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
            {
                if (hasImageOnUpdate)
                {
                    await this.imageRepository.CreateAsync(image, default);
                    itemToBeUpdated.ImagePublicId = image.Id;
                    isImageCreated = true;
                }

                this.itemRepository.Update(createdItem.Id, itemToBeUpdated);
                itemsAfterUpdate = await connection.QueryAsync<Item>(sql);
            }
            Item? updatedItem = itemsAfterUpdate.MaxBy(i => i.ModifiedAtUtc);

            // Assert
            updatedItem.Should().NotBeNull();
            using (new AssertionScope())
            {
                itemsAfterUpdate.Count().Should().Be(itemsAfterCreation.Count());
                updatedItem.Should().BeEquivalentTo(itemToBeUpdated, options => options.Excluding(i => i.CreatedAtUtc)
                                                                                       .Excluding(i => i.ModifiedAtUtc));

                using (new AssertionScope())
                {
                    updatedItem!.Id.Should().Be(createdItem.Id);
                    updatedItem.CreatedAtUtc.Should().Be(createdItem.CreatedAtUtc);
                    updatedItem.ModifiedAtUtc.Should().BeAfter(createdItem.ModifiedAtUtc);
                    updatedItem.Code.Should().NotBe(createdItem.Code);
                    updatedItem.Name.Should().NotBe(createdItem.Name);
                    updatedItem.ImagePublicId.Should().NotBe(createdItem.ImagePublicId);
                    updatedItem.Price?.Should().NotBe(createdItem.Price);
                    updatedItem.Category.Should().NotBe(createdItem.Category);
                    updatedItem.QuantityCombined.Should().NotBe(createdItem.QuantityCombined);
                    updatedItem.QuantityForSale?.Should().NotBe(createdItem.QuantityForSale);
                    updatedItem.AvailableQuantity?.Should().NotBe(createdItem.AvailableQuantity);
                    updatedItem.Description.Should().NotBe(createdItem.Description);
                    updatedItem.Location.Should().NotBe(createdItem.Location);
                }
            }
        }

        private void AssertGetById(Item itemBeforeCreation, Item? itemAfterCreation, bool beEquivalent)
        {
            using AssertionScope assertionScope = new();
            var itemAfterCreationShould = itemAfterCreation.Should();
            if (!beEquivalent)
            {
                itemAfterCreationShould.BeNull();

                return;
            }

            itemAfterCreationShould.NotBeNull();
            itemBeforeCreation.Should().BeEquivalentTo(itemAfterCreation, options => options.Including(o => o.Name)
                                                                                            .Including(o => o.Price)
                                                                                            .Including(o => o.Category)
                                                                                            .Including(o => o.QuantityForSale)
                                                                                            .Including(o => o.Description));
        }
    }
}
