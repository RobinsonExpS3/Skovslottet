using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Slottet.Domain.Entities;

namespace Slottet.Infrastructure.Repositories
{
    public sealed class ResidentRepository : BaseRepository<Resident>
    {
        protected override string SqlSelectAll => "SELECT * FROM vm_Resident";

        protected override string SqlSelectById => "usp_Insert_Resident";

        protected override string SqlInsert => "usp_Insert_Resident";

        protected override string SqlUpdate => "usp_Update_Resident";

        protected override string SqlDeleteById => "usp_Delete";

        protected override Guid GetKey(Resident e)
        {

        }

        protected override Resident Map(IDataRecord r) => new Resident
        {

        };

        protected override void BindInsert(SqlCommand cmd, Resident e)
        {

        }
        protected override void BindUpdate(SqlCommand cmd, Resident e)
        {

        }
        public async Task DeleteAsync(Guid id)
        {

        }
    }
}
