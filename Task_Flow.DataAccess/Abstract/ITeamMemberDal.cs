﻿using Task_Flow.Core.DataAccess;
using Task_Flow.Entities.Models;


namespace Task_Flow.DataAccess.Abstract
{
    public interface ITeamMemberDal:IEntityRepository<TeamMember>
    {
        Task<List<TeamMember>> GetTeamMembers();
        Task DeleteAllMembers(List<TeamMember> members);
    }
}
