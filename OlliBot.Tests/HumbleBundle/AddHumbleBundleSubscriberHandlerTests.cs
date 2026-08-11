using Microsoft.Extensions.Logging;
using NSubstitute;
using OlliBot.Application.HumbleBundle;
using OlliBot.Application.Interfaces;
using OlliBot.Domain.Enums;

namespace OlliBot.Tests.HumbleBundle;
public class AddHumbleBundleSubscriberHandlerTests
{
    [Fact]
    public async Task AddHumbleBundleSubscriberHandler_Handle_WithAlreadyRegisteredUserSubscriber_ReturnsFalseResult()
    {
        ILogger<AddHumbleBundleSubscriberHandler> loggerMock = Substitute.For<ILogger<AddHumbleBundleSubscriberHandler>>();
        IHumbleBundleRepository repositoryMock = Substitute.For<IHumbleBundleRepository>();

        repositoryMock.SubscriberExistsAsync(
            Arg.Any<ulong>(),
            Arg.Any<HumbleBundleType>(),
            Arg.Any<CancellationToken>())
            .Returns(true);

        //repositoryMock
        //    .GetSubscribersAsync(Arg.Any<HumbleBundleType>(), Arg.Any<CancellationToken>())
        //    .Returns(
        //    [
        //        new() {
        //            DiscordId = 1111111,
        //            SubscriberType = HumbleBundleSubscriberType.User,
        //            SubscriptionType = HumbleBundleType.Games,
        //            Id = 1,
        //        },
        //    ]);

        var sut = new AddHumbleBundleSubscriberHandler(loggerMock, repositoryMock);

        var command = new AddHumbleBundleSubscriberCommand(
            HumbleBundleType.Games,
            1111111,
            HumbleBundleSubscriberType.User);

        AddHumbleBundleSubscriberResult result = await sut.Handle(command, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
    }
}
