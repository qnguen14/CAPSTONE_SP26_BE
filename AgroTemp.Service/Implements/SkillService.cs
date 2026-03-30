using System.Linq.Expressions;
using AgroTemp.Domain.Context;
using AgroTemp.Domain.DTO.Skill;
using AgroTemp.Domain.Entities;
using AgroTemp.Domain.Mapper;
using AgroTemp.Repository.Interfaces;
using AgroTemp.Service.Base;
using AgroTemp.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using AgroTemp.Domain.Metadata;

namespace AgroTemp.Service.Implements
{
    public class SkillService : BaseService<Skill>, ISkillService
    {
        private readonly IMapperlyMapper _mapper;

        public SkillService(
            IUnitOfWork<AgroTempDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapperlyMapper mapper) : base(unitOfWork, httpContextAccessor, mapper)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<SkillResponse>> GetAllSkills()
        {
            try
            {
                var skills = await _unitOfWork.GetRepository<Skill>()
                    .GetListAsync(
                        predicate: null,
                        include: null,
                        orderBy: s => s.OrderBy(x => x.Name));

                if (skills == null || !skills.Any())
                {
                    return null;
                }

                return _mapper.SkillsToSkillResponses(skills);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResponse<SkillResponse>> GetSkillsByCategoryPagedAsync(Guid categoryId, int page, int limit)
        {
            try
            {
                page = page < 1 ? 1 : page;
                limit = limit <= 0 ? 20 : limit;
                limit = Math.Min(limit, 100);
                var skip = (page - 1) * limit;

                Expression<Func<Skill, bool>> predicate = s => s.JobCategoryId == categoryId;
    
                var total = await _unitOfWork.GetRepository<Skill>().CountAsync(predicate);

                var query = _unitOfWork.GetRepository<Skill>().CreateBaseQuery(
                    predicate: predicate,
                    orderBy: q => q.OrderBy(s => s.Name),
                    include: null,
                    asNoTracking: true);

                var items = await query.Skip(skip).Take(limit).ToListAsync();

                return new PaginatedResponse<SkillResponse>
                {
                    Data = _mapper.SkillsToSkillResponses(items),
                    Pagination = new PaginationMetadata
                    {
                        Page = page,
                        Limit = limit,
                        Total = total,
                        TotalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)limit)
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SkillResponse> GetSkillById(Guid id)
        {
            try
            {
                var skill = await _unitOfWork.GetRepository<Skill>()
                    .FirstOrDefaultAsync(
                        predicate: s => s.Id == id,
                        include: null);

                if (skill == null)
                {
                    return null;
                }

                return _mapper.SkillToSkillResponse(skill);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SkillResponse> CreateSkill(CreateSkillRequest request)
        {
            try
            {
                var skill = _mapper.CreateSkillRequestToSkill(request);
                skill.Id = Guid.NewGuid();

                await _unitOfWork.GetRepository<Skill>().InsertAsync(skill);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.SkillToSkillResponse(skill);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SkillResponse> UpdateSkill(Guid id, UpdateSkillRequest request)
        {
            try
            {
                var existingSkill = await _unitOfWork.GetRepository<Skill>()
                    .FirstOrDefaultAsync(
                        predicate: s => s.Id == id,
                        include: null);

                if (existingSkill == null)
                {
                    return null;
                }

                _mapper.UpdateSkillRequestToSkill(request, existingSkill);
                _unitOfWork.GetRepository<Skill>().UpdateAsync(existingSkill);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.SkillToSkillResponse(existingSkill);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteSkill(Guid id)
        {
            try
            {
                var existingSkill = await _unitOfWork.GetRepository<Skill>()
                    .FirstOrDefaultAsync(
                        predicate: s => s.Id == id,
                        include: null);

                if (existingSkill == null)
                {
                    return false;
                }

                _unitOfWork.GetRepository<Skill>().DeleteAsync(existingSkill);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}