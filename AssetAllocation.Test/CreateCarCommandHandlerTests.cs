using AssetAllocation.Api;
using AssetAllocation.Api.Infrastructure.Persistence.Contexts;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;

namespace AssetAllocation.Test;

public class CreateCarCommandHandlerTests
{
    private readonly Mock<AssetAllocationDbContext> _mockDbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateCarCommand.CreteCarCommandHandler _handler;

    public CreateCarCommandHandlerTests()
    {
        //Dependencies
        _mockDbContext = new Mock<AssetAllocationDbContext>(
            new DbContextOptions<AssetAllocationDbContext>(), null);
        _mockMapper = new Mock<IMapper>();

        //SUT
        _handler = new CreateCarCommand.CreteCarCommandHandler(_mockDbContext.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequestResult_WhenCarModelNotFound()
    {
        // Arrange
        var command = new CreateCarCommand("34ABC123", 2024, "Red", 0, Guid.NewGuid());

        var mockCarModels = CreateDbSetMock<CarModel>();

        _mockDbContext.Setup(context => context.CarModels)
            .Returns(mockCarModels.Object);

        mockCarModels.Setup(set => set.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as CarModel);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().BeOfType<Result<CarCreatedResponse>>().Which.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.ResultFailure.Should().NotBeNull();
        result.ResultFailure!.Errors[0].Should().Be("Car model not found");
        result.Payload.Should().BeNull();

        mockCarModels.Verify(m => m.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequestResult_WhenCarWithPlateExists()
    {
        //Arrange
        var command = new CreateCarCommand("34ABC123", 2024, "Red", 0, Guid.NewGuid());

        Mock<DbSet<CarModel>> mockCarModels = CreateDbSetMock<CarModel>();

        _mockDbContext.Setup(m => m.CarModels)
            .Returns(mockCarModels.Object);

        mockCarModels.Setup(set => set.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CarModel());

        List<Car> existingCars = [new Car(2022, "Blue", "34ABC123", 0, Guid.NewGuid())];
        Mock<DbSet<Car>> mockCars = CreateDbSetMock(existingCars);

        _mockDbContext.Setup(context => context.Cars)
            .Returns(mockCars.Object);

        //Act
        var result = await _handler.Handle(command, default);

        //Assert
        result.Should().BeOfType<Result<CarCreatedResponse>>().Which.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.ResultFailure.Should().NotBeNull();
        result.ResultFailure!.Errors[0].Should().Be("A car with the plate already exists");
        result.Payload.Should().BeNull();

        mockCarModels.Verify(m => m.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCarCreatedSuccessfully()
    {
        //Arrange
        var command = new CreateCarCommand("34ABC123", 2024, "Red", 0, Guid.NewGuid());

        var mockCarModels = CreateDbSetMock<CarModel>();

        _mockDbContext.Setup(m => m.CarModels)
            .Returns(mockCarModels.Object);

        mockCarModels.Setup(set => set
            .FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CarModel());

        var mockCars = CreateDbSetMock<Car>();
        
        _mockDbContext.Setup(m => m.Cars)
            .Returns(mockCars.Object);

        mockCars.Setup(set => set.AddAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Car car, CancellationToken token) => null);

        _mockDbContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockMapper.Setup(mapper => mapper.Map<CarCreatedResponse>(It.IsAny<Car>()))
                   .Returns(new CarCreatedResponse());

        //Act
        var result = await _handler.Handle(command, default);

        //Assert
        result.Should().BeOfType<Result<CarCreatedResponse>>().Which.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.ResultFailure.Should().BeNull();
        result.Payload.Should().BeOfType<CarCreatedResponse>();

        mockCarModels.Verify(m => m.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        mockCars.Verify(m => m.AddAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<CarCreatedResponse>(It.IsAny<Car>()), Times.Once);
    }

    private Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T>? elements = null) where T : class
    {
        elements ??= [];
        var queryable = elements.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        return dbSetMock;
    }
}