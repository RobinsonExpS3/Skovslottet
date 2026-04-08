using System.Data;
using Microsoft.Data.SqlClient;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Repositories
{
    public sealed class ResidentRepository : BaseRepository<Resident>
    {
        protected override string SqlSelectAll => "SELECT * FROM vm_Resident";

        protected override string SqlSelectById => "usp_SelectById_Resident";

        protected override string SqlInsert => "usp_Insert_Resident";

        protected override string SqlUpdate => "usp_Update_Resident";

        protected override string SqlDeleteById => "usp_Delete";

        protected override Guid GetKey(Resident e) => e.ResidentID;

        protected override Resident Map(IDataRecord r) => new Resident
        {
            ResidentID = r.GetGuid(r.GetOrdinal("ResidentID")),
            ResidentName = r.GetString(r.GetOrdinal("ResidentName")),
            IsActive = r.GetBoolean(r.GetOrdinal("IsActive")),
            //GroceryDayID = r.GetGuid(r.GetOrdinal("GroceryDayID"))
            
        };

        
        protected override void BindInsert(SqlCommand cmd, Resident e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = e.ResidentID;
            cmd.Parameters.Add("@ResidentName", SqlDbType.NVarChar).Value = e.ResidentName;
            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = e.IsActive;

        }
        protected override void BindUpdate(SqlCommand cmd, Resident e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = e.ResidentID;
            cmd.Parameters.Add("@ResidentName", SqlDbType.NVarChar).Value = e.ResidentName;
            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = e.IsActive;
        }
        public async Task DeleteAsync(Guid id)
        {

        }
    }
}
