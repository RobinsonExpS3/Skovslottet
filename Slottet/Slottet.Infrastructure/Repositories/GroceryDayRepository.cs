using System.Data;
using Microsoft.Data.SqlClient;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;

namespace Slottet.Infrastructure.Repositories {
    public class GroceryDayRepository : BaseRepository<GroceryDay> {
        protected override string SqlSelectAll => "SELECT GroceryDayID, GroceryDay AS GroceryDayName FROM dbo.GroceryDay"; 
        protected override string SqlSelectById => "usp_SelectById_GroceryDay"; 
        protected override string SqlInsert => "usp_Insert_GroceryDay"; 
        protected override string SqlUpdate => "usp_Update_GroceryDay"; 
        protected override string SqlDeleteById => "usp_Delete_GroceryDay";

        protected override Guid GetKey(GroceryDay e) => e.GroceryDayID;

        protected override GroceryDay Map(IDataRecord r) => new() {
            GroceryDayID = r.GetGuid(r.GetOrdinal("GroceryDayID")),
            GroceryDayName = r.GetString(r.GetOrdinal("GroceryDayName"))
        };

        protected override void BindId(SqlCommand cmd, Guid id)
            => cmd.Parameters.Add("@GroceryDayID", SqlDbType.UniqueIdentifier).Value = id;
    }
}
