using AgroTemp.Domain.DTO.Skill;

namespace AgroTemp.Service.Interfaces
{
    public interface ISkillService
    {
        Task<List<SkillResponse>> GetAllSkills();
        Task<SkillResponse> GetSkillById(Guid id);
        Task<SkillResponse> CreateSkill(CreateSkillRequest request);
        Task<SkillResponse> UpdateSkill(Guid id, UpdateSkillRequest request);
        Task<bool> DeleteSkill(Guid id);
    }
}