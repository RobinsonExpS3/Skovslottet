using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Repositories
{
    class SpecialResponsibilityRepository : BaseRepository <SpecialResponsibility>
    {
        protected override string SqlSelectAll => "SELECT * FROM vm_SpecialResponsibility";

        protected override string SqlSelectById => "usp_SelectById_SpecialResponsibility";

        protected override string SqlInsert => "usp_Insert_SpecialResponsibility";

        protected override string SqlUpdate => "usp_Update_SpecialResponsibility";

        protected override string SqlDeleteById => "usp_Delete";

        protected override Guid GetKey(SpecialResponsibility e) => e.SpecialResponsibilityID;

        protected override SpecialResponsibility Map(IDataRecord r) => new SpecialResponsibility
        {
            SpecialResponsibilityID = r.GetGuid(r.GetOrdinal("SpecialResponsibilityID")),
            TaskName = r.GetString(r.GetOrdinal("TaskName"))

        };

        protected override void BindInsert(SqlCommand cmd, SpecialResponsibility e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@SpecialResponsibilityID", SqlDbType.UniqueIdentifier).Value = e.SpecialResponsibilityID;
            cmd.Parameters.Add("@TaskName", SqlDbType.NVarChar, 255).Value = e.TaskName;

        }
        protected override void BindUpdate(SqlCommand cmd, SpecialResponsibility e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@SpecialResponsibilityID", SqlDbType.UniqueIdentifier).Value = e.SpecialResponsibilityID;
            cmd.Parameters.Add("@TaskName", SqlDbType.NVarChar, 255).Value = e.TaskName;
        }

        public async Task DeleteAsync (Guid id)
        {

        }
    }
}
