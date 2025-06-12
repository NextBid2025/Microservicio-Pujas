using BluidingBlocks.CQRS;
using FluentValidation;
using Puja.Application.DTOS;


namespace Puja.Application.Commands;

public record CreatePujaCommand(PujaDto Puja)
    : ICommand<CreatePujaResult>;

public record CreatePujaResult(Guid Id);

public class CreatePujaCommandValidator : AbstractValidator<CreatePujaCommand>
{
    public CreatePujaCommandValidator()
    {
        RuleFor(x => x.Puja.SubastaId).NotEmpty().WithMessage("SubastaId es requerido");
        RuleFor(x => x.Puja.UserId).NotEmpty().WithMessage("UserId es requerido");
        RuleFor(x => x.Puja.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a 0");
        
    }
}