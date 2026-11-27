using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class PreferenceController(IPreferenceService preferenceService) : BaseController
{

    // URL: "api/Account/CreatePreference"
    // Creates a new preference from PreferenceDTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [ProducesResponseType(typeof(PreferenceDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePreference ([FromBody] PreferenceDTO createPreference)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var id = await preferenceService.CreatePreference(createPreference);
        if (id > 0)
        {
            return Created(string.Empty, id);
        }
        return BadRequest();
    }
}