using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Preferences;

namespace FoodplannerApi.Controller;

public class PreferencesController(IPreferencesService preferencesService) : BaseController
{

    // URL: "api/Preference/CreatePreference"
    // Creates a new preference from PreferenceDTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [ProducesResponseType(typeof(PreferencesDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePreferences ([FromBody] PreferencesDTO createPreference)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var id = await preferencesService.CreatePreferences(createPreference);
        if (id > 0)
        {
            return Created(string.Empty, id);
        }
        return BadRequest();
    }


    // URL: api/Preference/RecommendRetry/{childId}
    // Retrieves previously disliked foods that can be recommended again
    [HttpGet("{childId}")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<Preference>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecommendRetry(int childId)
    {
        if (childId <= 0)
        {
            return BadRequest();
        }

        var preferences = await preferenceService.RecommendRetryAsync(childId);

        return Ok(preferences);
    }
}
