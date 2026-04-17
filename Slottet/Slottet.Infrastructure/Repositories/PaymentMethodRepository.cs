using System.Data;
using Microsoft.Data.SqlClient;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;

namespace Slottet.Infrastructure.Repositories {
    public class PaymentMethodRepository : BaseRepository<PaymentMethod> {
        protected override string SqlSelectAll => "SELECT PaymentMethodID, PaymentMethod AS PaymentMethodName FROM dbo.PaymentMethod"; 
        protected override string SqlSelectById => "usp_SelectById_PaymentMethod"; 
        protected override string SqlInsert => "usp_Insert_PaymentMethod"; 
        protected override string SqlUpdate => "usp_Update_PaymentMethod"; 
        protected override string SqlDeleteById => "usp_Delete_PaymentMethod";

        protected override Guid GetKey(PaymentMethod e) => e.PaymentMethodID;

        protected override PaymentMethod Map(IDataRecord r) => new() {
            PaymentMethodID = r.GetGuid(r.GetOrdinal("PaymentMethodID")),
            PaymentMethodName = r.GetString(r.GetOrdinal("PaymentMethodName"))
        };

        protected override void BindId(SqlCommand cmd, Guid id)
            => cmd.Parameters.Add("@PaymentMethodID", SqlDbType.UniqueIdentifier).Value = id;
    }
}
