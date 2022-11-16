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
    public class LabAccreditationManualViewModelValidator : AbstractValidator<LabAccreditationManualViewModel>
    {
        private readonly ApplicationDbContext _context;

        public LabAccreditationManualViewModelValidator(ApplicationDbContext context) 
        {
            _context = context;

            

            RuleFor(p => p.DOCNo)
                //.MustAsync(async (name, ct) =>
                //(await repository.GetDataAsync()).All(x => x.Name != name))
                //(await _labAccreditationManualCache.GetByDOCNoAsync(query.docno)
                // var response = await _mediator.Send(new GetLabAccreditationManualByIdQuery() { Id = id });

                // var labAccreditationManual = await _labAccreditationManualCache.GetByDOCNoAsync(query.docno);
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                
                //.MustAsync(BeUniqueDOCNo).WithMessage("The specified WSCP No already exists.")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }


        public async Task<bool> BeUniqueDOCNo(string docno, CancellationToken cancellation)
        {
            return await _context.LabAccreditationManuals.AllAsync(x => x.DOCNo != docno);
        }
    }
}