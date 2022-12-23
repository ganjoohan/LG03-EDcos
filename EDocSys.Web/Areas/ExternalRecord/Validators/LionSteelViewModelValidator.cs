using EDocSys.Infrastructure.DbContexts;
using EDocSys.Web.Areas.Documentation.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using EDocSys.Web.Areas.ExternalRecord.Models;

namespace EDocSys.Web.Areas.ExternalRecord.Validators
{
    public class LionSteelViewModelValidator : AbstractValidator<LionSteelViewModel>
    {
        private readonly ApplicationExternalDbContext _context;

        public LionSteelViewModelValidator(ApplicationExternalDbContext context) 
        {
            _context = context;

            

            RuleFor(p => p.FormNo)
                //.MustAsync(async (name, ct) =>
                //(await repository.GetDataAsync()).All(x => x.Name != name))
                //(await _lionSteelCache.GetByDOCNoAsync(query.docno)
                // var response = await _mediator.Send(new GetLionSteelByIdQuery() { Id = id });

                // var lionSteel = await _lionSteelCache.GetByDOCNoAsync(query.docno);
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                
                //.MustAsync(BeUniqueDOCNo).WithMessage("The specified WSCP No already exists.")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }


        public async Task<bool> BeUniqueDOCNo(string docno, CancellationToken cancellation)
        {
            return await _context.LionSteels.AllAsync(x => x.FormNo != docno);
        }
    }
}