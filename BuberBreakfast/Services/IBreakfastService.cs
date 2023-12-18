using BuberBreakfast.Models;
using ErrorOr;

namespace BuberBreakfast.Services.Breakfasts
{
    public interface IBreakfastService
    {
        ErrorOr<Created> CreateBreakfast(Breakfast breakfast);

        ErrorOr<Deleted> DeleteBreakfast(Guid id);

        ErrorOr<Breakfast> GetBreakfast(Guid id);

        IEnumerable<Breakfast> GetBreakfasts();

        ErrorOr<UpsertedBreakfast> UpdateBreakfast(Breakfast breakfast);
        

    }
}