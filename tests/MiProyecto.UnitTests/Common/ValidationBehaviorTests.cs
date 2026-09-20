using FluentValidation;
using MediatR;
using MiProyecto.Application.Common.Behaviors;
using MiProyecto.Application.Common.Errors;
using ValidationException = MiProyecto.Application.Common.Exceptions.ValidationException;

namespace MiProyecto.UnitTests.Common;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_CuandoNoHayValidators_Continua()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>([]);
        var nextCalled = false;

        var result = await behavior.Handle(
            new SampleRequest { Name = "ok" },
            _ =>
            {
                nextCalled = true;
                return Task.FromResult("ok");
            },
            CancellationToken.None);

        Assert.True(nextCalled);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_CuandoLaValidacionFalla_LanzaValidationException()
    {
        var behavior = new ValidationBehavior<SampleRequest, string>([new SampleRequestValidator()]);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(
                new SampleRequest { Name = "" },
                _ => Task.FromResult("ok"),
                CancellationToken.None));

        Assert.Equal(ErrorCodes.ValidationError, exception.Code);
        Assert.Contains(exception.Errors, error => error.Field == "name");
        Assert.Contains(exception.Errors, error => error.Message == "El nombre es obligatorio.");
    }

    public sealed class SampleRequest
    {
        public string Name { get; init; } = string.Empty;
    }

    private sealed class SampleRequestValidator : AbstractValidator<SampleRequest>
    {
        public SampleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("El nombre es obligatorio.");
        }
    }
}
