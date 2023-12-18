using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.Models;
using BuberBreakfast.ServiceErrors;
using BuberBreakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace BuberBreakfast.Controllers
{
    public class BreakfastController : ApiController
    {
        private readonly IBreakfastService _breakfastService;

        public BreakfastController(IBreakfastService breakfastService)
        {
            _breakfastService = breakfastService;
        }

        [HttpPost]
        public IActionResult CreateBreakfast(CreateBreakfastRequest request)
        {
            ErrorOr<Breakfast> requestToBreakfastResult = Breakfast.From(request);

            if (requestToBreakfastResult.IsError)
            {
                return Problem(requestToBreakfastResult.Errors);
            }

            var breakfast = requestToBreakfastResult.Value;

            var createdBreakfast = _breakfastService.CreateBreakfast(breakfast);

            return createdBreakfast.Match(created => CreatedAsGetbreakfast(breakfast), Problem);
        }

        private IActionResult CreatedAsGetbreakfast(Breakfast breakfast)
        {
            return CreatedAtAction(
                nameof(GetBreakfast),
                new { id = breakfast.Id },
                MapBreakfastResponse(breakfast)
            );
        }

        [HttpGet]
        public IActionResult GetBreakfasts()
        {
            var breakfasts = _breakfastService.GetBreakfasts();
            var response = breakfasts.Select(
                breakfast =>
                    new BreakfastResponse(
                        breakfast.Id,
                        breakfast.Name,
                        breakfast.Description,
                        breakfast.StartDateTime,
                        breakfast.EndDateTime,
                        breakfast.LastModifiedDateTime,
                        breakfast.Savory,
                        breakfast.Sweet
                    )
            );
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetBreakfast(Guid id)
        {
            try
            {
                ErrorOr<Breakfast> getbreakfastsResult = _breakfastService.GetBreakfast(id);

                return getbreakfastsResult.Match(
                    breakfast => Ok(MapBreakfastResponse(breakfast)),
                    Problem
                );
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
        {
            try
            {
                ErrorOr<Breakfast> upsertedBreakfastRequest = Breakfast.From(id, request);

                if (upsertedBreakfastRequest.IsError)
                {
                    return Problem(upsertedBreakfastRequest.Errors);
                }

                var breakfast = upsertedBreakfastRequest.Value;

                ErrorOr<UpsertedBreakfast> updatedBreakfast = _breakfastService.UpdateBreakfast(
                    breakfast
                );

                if (updatedBreakfast.IsError)
                {
                    return Problem(updatedBreakfast.Errors);
                }

                return updatedBreakfast.Match(
                    upserted =>
                        upserted.IsNewlyCreated
                            ? CreatedAsGetbreakfast(breakfast)
                            : Ok(MapBreakfastResponse(breakfast)),
                    Problem
                );
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteBreakfast(Guid id)
        {
            try
            {
                var deletedBreakfast = _breakfastService.DeleteBreakfast(id);

                return deletedBreakfast.Match(deleted => NoContent(), Problem);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
        {
            return new BreakfastResponse(
                breakfast.Id,
                breakfast.Name,
                breakfast.Description,
                breakfast.StartDateTime,
                breakfast.EndDateTime,
                breakfast.LastModifiedDateTime,
                breakfast.Savory,
                breakfast.Sweet
            );
        }
    }
}
