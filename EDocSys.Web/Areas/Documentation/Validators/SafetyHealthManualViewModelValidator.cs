using EDocSys.Infrastructure.DbContexts;
using EDocSys.Web.Areas.Documentation.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace EDocSys.Web.Areas.Documentation.Validators
{
    public class SafetyHealthManualViewModelValidator : AbstractValidator<SafetyHealthManualViewModel>
    {
        private readonly ApplicationDbContext _context;

        public SafetyHealthManualViewModelValidator(ApplicationDbContext context) 
        {
            _context = context;

            

            RuleFor(p => p.DOCNo)
                //.MustAsync(async (name, ct) =>
                //(await repository.GetDataAsync()).All(x => x.Name != name))
                //(await _safetyHealthManualCache.GetByDOCNoAsync(query.docno)
                // var response = await _mediator.Send(new GetSafetyHealthManualByIdQuery() { Id = id });

                // var safetyHealthManual = await _safetyHealthManualCache.GetByDOCNoAsync(query.docno);
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                
                //.MustAsync(BeUniqueDOCNo).WithMessage("The specified WSCP No already exists.")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }


        public async Task<bool> BeUniqueDOCNo(string docno, CancellationToken cancellation)
        {
            return await _context.SafetyHealthManuals.AllAsync(x => x.DOCNo != docno);
        }
    }
}