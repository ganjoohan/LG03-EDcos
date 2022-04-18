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
    public class WIViewModelValidator : AbstractValidator<WIViewModel>
    {
        private readonly ApplicationDbContext _context;

        public WIViewModelValidator(ApplicationDbContext context) 
        {
            _context = context;

            

            RuleFor(p => p.WSCPNo)
                //.MustAsync(async (name, ct) =>
                //(await repository.GetDataAsync()).All(x => x.Name != name))
                //(await _wiCache.GetByWSCPNoAsync(query.wscpno)
                // var response = await _mediator.Send(new GetWIByIdQuery() { Id = id });

                // var wi = await _wiCache.GetByWSCPNoAsync(query.wscpno);
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                
                //.MustAsync(BeUniqueWSCPNo).WithMessage("The specified WSCP No already exists.")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters.");
        }


        //public async Task<bool> BeUniqueWSCPNo(string wscpno, CancellationToken cancellation)
        //{
            //   return await _context.WIs.AllAsync(x => x.WSCPNo != wscpno);
                //   }
    }
}