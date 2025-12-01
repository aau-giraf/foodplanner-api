using AutoMapper;
using FoodplannerModels.Lunchbox;

namespace FoodplannerServices.Lunchbox;

/// <summary>
/// Service implementation for SubIngredientRelation business logic
/// </summary>
public class SubIngredientRelationService : ISubIngredientRelationService
{
    private readonly ISubIngredientRelationRepository _subIngredientRelationRepository;
    private readonly ISubIngredientRepository _subIngredientRepository;
    private readonly IMapper _mapper;

    public SubIngredientRelationService(
        ISubIngredientRelationRepository subIngredientRelationRepository,
        ISubIngredientRepository subIngredientRepository,
        IMapper mapper)
    {
        _subIngredientRelationRepository = subIngredientRelationRepository;
        _subIngredientRepository = subIngredientRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SubIngredientRelationProperDTO>> GetAllSubIngredientRelationsAsync()
    {
        var relations = await _subIngredientRelationRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<SubIngredientRelationProperDTO>>(relations);
    }

    public async Task<IEnumerable<SubIngredientRelationDTO>> GetAllSubIngredientRelationsByIngredientIdAsync(int ingredientId)
    {
        var relations = await _subIngredientRelationRepository.GetAllByIngredientIdAsync(ingredientId);
        
        var relationsWithDetails = new List<SubIngredientRelationDTO>();
        
        foreach (var relation in relations)
        {
            var subingredient = await _subIngredientRepository.GetByIdAsync(relation.Subingredient_id);
            
            relationsWithDetails.Add(new SubIngredientRelationDTO
            {
                Id = relation.Id,
                Ingredient_id = relation.Ingredient_id,
                Subingredient_id = subingredient,
                Order_number = relation.Order_number
            });
        }
        
        return relationsWithDetails;
    }

    public async Task<SubIngredientRelation> GetSubIngredientRelationByIdAsync(int id)
    {
        return await _subIngredientRelationRepository.GetByIdAsync(id);
    }

    public async Task<int> CreateSubIngredientRelationAsync(int ingredientId, int subingredientId)
    {
        return await _subIngredientRelationRepository.InsertAsync(ingredientId, subingredientId);
    }

    public async Task<int> UpdateSubIngredientRelationAsync(SubIngredientRelation relation, int id)
    {
        return await _subIngredientRelationRepository.UpdateAsync(relation, id);
    }

    public async Task<int> DeleteSubIngredientRelationAsync(int id)
    {
        return await _subIngredientRelationRepository.DeleteAsync(id);
    }

    public async Task<bool> UpdateSubIngredientRelationOrderAsync(List<SubIngredientRelation> relations)
    {
        foreach (var relation in relations)
        {
            var result = await _subIngredientRelationRepository.UpdateOrderAsync(relation.Id, relation.Order_number);
            if (!result)
            {
                return false;
            }
        }
        return true;
    }
}