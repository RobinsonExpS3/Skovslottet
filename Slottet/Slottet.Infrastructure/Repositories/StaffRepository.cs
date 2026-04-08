using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Repositories
{
    public sealed class StaffRepository : BaseRepository<Staff>
    {
        protected override string SqlSelectAll => "SELECT * FROM vm_Staff";

        protected override string SqlSelectById => "usp_SelectById_Staff";

        protected override string SqlInsert => "usp_Insert_Staff";

        protected override string SqlUpdate => "usp_Update_Staff";

        protected override string SqlDeleteById => "usp_Delete";

        protected override Guid GetKey(Staff e) => e.StaffID;

        protected override Staff Map(IDataRecord r) => new Staff
        {
            StaffID = r.GetGuid(r.GetOrdinal("StaffID")),
            StaffName = r.GetString(r.GetOrdinal("StaffName")),
            Initials = r.GetString(r.GetOrdinal("Initials")),
            Role = r.GetString(r.GetOrdinal("Role")),

        };

        protected override void BindInsert(SqlCommand cmd, Staff e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@StaffId", SqlDbType.UniqueIdentifier).Value = e.StaffID;
            cmd.Parameters.Add("@StaffName", SqlDbType.NVarChar, 255).Value = e.StaffName;
            cmd.Parameters.Add("@Initials", SqlDbType.NVarChar, 255).Value = e.Initials;
            cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 255).Value = e.Role;

        }
        protected override void BindUpdate(SqlCommand cmd, Staff e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@StaffId", SqlDbType.UniqueIdentifier).Value = e.StaffID;
            cmd.Parameters.Add("@StaffName", SqlDbType.NVarChar, 255).Value = e.StaffName;
            cmd.Parameters.Add("@Initials", SqlDbType.NVarChar, 255).Value = e.Initials;
            cmd.Parameters.Add("@Role", SqlDbType.NVarChar, 255).Value = e.Role;
        }

        public async Task DeleteAsync(Guid id)
        {

        }
    }
}
