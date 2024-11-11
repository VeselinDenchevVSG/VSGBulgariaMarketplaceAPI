using Dapper;
using FluentAssertions.Execution;
using FluentAssertions;

using Microsoft.Data.SqlClient;

using Test.IntegrationTests.Extensions;
using Test.IntegrationTests.Persistence.Factories;

using VSGBulgariaMarketplace.Domain.Entities;
using VSGBulgariaMarketplace.Application.Models.ItemLoan.Interfaces;
using VSGBulgariaMarketplace.Persistence.Repositories;
using VSGBulgariaMarketplace.Application.Models.Item.Interfaces;

using static VSGBulgariaMarketplace.Persistence.Constants.DatabaseConstant;

namespace Test.IntegrationTests.Persistence.Repositories;

internal class ItemLoanRepositoryTests : RepositoryTests
{
    private readonly IItemLoanRepository itemLoanRepository;
    private readonly IItemRepository itemRepository;

    public ItemLoanRepositoryTests()
        : base()
    {
        this.itemLoanRepository = new ItemLoanRepository(base.unitOfWork);
        this.itemRepository = new ItemRepository(base.unitOfWork);
    }

    [Test]
    public async Task CreateAsync_WithNullProperties_CreatesItemLoan()
    {
        // Arrange
        Item item = ItemFactory.GetItemWithNullProperties();

        ItemLoan itemLoan = new()
        {
            ItemId = await InsertItemAndSelectIdAsync(item),
            Quantity = 1,
            Email = "vdenchev@vsgbg.com",
            EndDatetimeUtc = null
        };

        await this.CreateAsyncCreatesItemLoanBaseAsync(itemLoan);
    }

    [Test]
    public async Task CreateAsyncWithNotNullProperties_CreatesItemLoan()
    {
        // Arrange
        Item item = ItemFactory.GetItemWithNullProperties();

        ItemLoan itemLoan = new()
        {
            ItemId = await InsertItemAndSelectIdAsync(item),
            Quantity = 1,
            Email = "vdenchev@vsgbg.com",
            EndDatetimeUtc = DateTime.UtcNow
        };

        await this.CreateAsyncCreatesItemLoanBaseAsync(itemLoan);
    }

    private async Task<string> InsertItemAndSelectIdAsync(Item item)
    {
        await using SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString);
        
        await this.itemRepository.CreateAsync(item, default);
        string sql = "SELECT TOP (1) Id FROM Items ORDER BY CreatedAtUtc DESC";

        return await connection.QueryFirstAsync<string>(sql);
    }

    private async Task CreateAsyncCreatesItemLoanBaseAsync(ItemLoan itemLoanToBeCreated)
    {
        // Act
        string sql = "SELECT * FROM ItemLoans ORDER BY CreatedAtUtc";
        IEnumerable<ItemLoan> itemLoansBeforeCreation;
        IEnumerable<ItemLoan> itemLoansAfterCreation;
        await using (SqlConnection connection = new(this.databaseHelper.integrationTestsConnectionString))
        {
            itemLoansBeforeCreation = await connection.QueryAsync<ItemLoan>(sql);
            this.itemLoanRepository.Create(itemLoanToBeCreated);
            itemLoansAfterCreation = await connection.QueryAsync<ItemLoan>(sql);
        }

        // Wait for 1ms so we are sure itemLoansAfterCreation.CreatedAtUtc and utcNow are different
        Thread.Sleep(1);

        var utcNow = DateTime.UtcNow;

        ItemLoan? createdItemLoan = itemLoansAfterCreation.MaxBy(i => i.CreatedAtUtc);

        // Assert
        createdItemLoan.Should().NotBeNull();
        using (new AssertionScope())
        {
            itemLoansAfterCreation.Count().Should().Be(itemLoansBeforeCreation.Count() + 1);
            itemLoanToBeCreated.Should().BeEquivalentTo(createdItemLoan, options => options.Excluding(il => il!.CreatedAtUtc)
                                                                                           .Excluding(il => il!.ModifiedAtUtc)
                                                                                           .Excluding(il => il!.EndDatetimeUtc));
            itemLoanToBeCreated.CreatedAtUtc.Should().Be(itemLoanToBeCreated.ModifiedAtUtc);
            itemLoanToBeCreated.CreatedAtUtc.RoundToNearestSecond().Should().Be(createdItemLoan!.CreatedAtUtc.RoundToNearestSecond());
            itemLoanToBeCreated.ModifiedAtUtc.RoundToNearestSecond().Should().Be(createdItemLoan.ModifiedAtUtc.RoundToNearestSecond());
            createdItemLoan.CreatedAtUtc.Should().BeBefore(utcNow);
            createdItemLoan.CreatedAtUtc.Should().Be(createdItemLoan.ModifiedAtUtc);
            itemLoanToBeCreated.EndDatetimeUtc.RoundToNearestSecond().Should().Be(createdItemLoan.EndDatetimeUtc.RoundToNearestSecond());
        }
    }

    [TearDown]
    public async Task TruncateItemLoansTableAsync() => await base.DeleteFromTableAsync(ITEM_LOANS_TABLE_NAME);
}