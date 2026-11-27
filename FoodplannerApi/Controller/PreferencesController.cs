using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodplannerApi.Controller;

public class PreferencesController(IPreferencesService preferencesService) : BaseController
{

    // URL: "api/Preference/CreatePreference"
    // Creates a new preference from PreferenceDTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [ProducesResponseType(typeof(PreferenceDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePreferencesAsync ([FromBody] PreferenceDTO createPreference)
    {
        // Validate the model state to ensure the incoming data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var id = await preferencesService.CreatePreferencesAsync(createPreference);
        if (id > 0)
        {
            return Created(string.Empty, id);
        }
        return BadRequest();
    }

    // URL: api/Preference/GetPreferencesByChildId/{id}
    // Retrieves a list of preference by a child ID.
    // Returns 200 OK with preference
    [HttpGet("{id}")]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(PreferenceDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreferencesByChildId(int id)
    {
        var preferences = await preferencesService.GetPreferencesByChildIdAsync(id);
        return Ok(preferences);
    }

    // URL: api/Preference/GetPreferencesFromRating
    // Retrieves a list of preference from rating 
    // Returns 200 OK with preference, 
    [HttpGet]
    [Authorize(Roles = "Child, Parent")]
    [ProducesResponseType(typeof(PreferenceDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPreferencesFromRating([FromQuery] string rating)
    {
        var preferences = await preferencesService.GetPreferencesFromRatingAsync(rating);
        return Ok(preferences);
    }

}