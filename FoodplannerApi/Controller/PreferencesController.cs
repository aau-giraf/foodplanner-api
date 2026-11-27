using FoodplannerModels.Account;
using FoodplannerModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FoodplannerModels.Preferences;

namespace FoodplannerApi.Controller;

public class PreferencesController(IPreferencesService preferencesService) : BaseController
{
    // URL: api/Preferences/CreatePreferencesAsync
    // Creates a new preference from PreferenceDTO
    // When success returns 201, when failed returns 400
    [HttpPost]
    [ProducesResponseType(typeof(PreferenceDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePreferencesAsync([FromBody] PreferenceDTO createPreference)
    {
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

    // URL: api/Preferences/RecommendRetry/{childId}
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

        var preferences = await preferencesService.RecommendRetryAsync(childId);

        return Ok(preferences);
    }

    // URL: api/Preferences/UpdatePreferencesAsync
    // Updates an existing preference
    // Returns 200 OK when successful, or 400 Bad Request if the input is invalid
    [HttpPut]
    [Authorize(Roles = "Parent, Teacher")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePreferencesAsync([FromBody] PreferenceDTO updatePreferences)
    {
        if (!ModelState.IsValid || updatePreferences.Id <= 0)
        {
            return BadRequest(ModelState);
        }

        var id = await preferencesService.UpdatePreferencesAsync(updatePreferences);

        return Ok(id);
    }
}