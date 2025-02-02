using Smooth.Shop.Application.Contracts;
using Smooth.Shop.Domain.Models;
using Smooth.Shop.Infrastructure.Data;

namespace Smooth.Shop.Infrastructure.Repos;

public class NewMediumRepo : GenericRepo<NewMedium, SmoothWebDbContext>, INewMediumRepo
{
    public NewMediumRepo(SmoothWebDbContext context) : base(context) 
    {
    }

}