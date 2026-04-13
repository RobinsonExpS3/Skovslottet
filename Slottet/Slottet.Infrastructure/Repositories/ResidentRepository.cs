using System.Data;
using Microsoft.Data.SqlClient;
using Slottet.Domain.Entities;
using Slottet.Infrastructure.Data;

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
            GroceryDayID = r.GetGuid(r.GetOrdinal("GroceryDayID"))
            
        };

        protected override void BindInsert(SqlCommand cmd, Resident e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = e.ResidentID;
            cmd.Parameters.Add("@ResidentName", SqlDbType.NVarChar).Value = e.ResidentName;
            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = e.IsActive;
            cmd.Parameters.Add("@GroceryDayID", SqlDbType.UniqueIdentifier).Value = e.GroceryDayID;

        }

        protected override void BindUpdate(SqlCommand cmd, Resident e)
        {
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = e.ResidentID;
            cmd.Parameters.Add("@ResidentName", SqlDbType.NVarChar).Value = e.ResidentName;
            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = e.IsActive;
            cmd.Parameters.Add("@GroceryDayID", SqlDbType.UniqueIdentifier).Value = e.GroceryDayID;
        }

        public async Task<List<Guid>> GetPaymentMethodIdsAsync(Guid residentId) {
            var result = new List<Guid>();
            using var con = await DBContext.OpenConnection();
            using var cmd = new SqlCommand(
                "SELECT PaymentMethodID FROM dbo.ResidentPaymentMethod WHERE ResidentID = @ResidentID", con);
            cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
                result.Add(rd.GetGuid(0));

            return result;
        }

        public async Task<List<DateTime>> GetMedicineTimesAsync(Guid residentId) {
            var result = new List<DateTime>();
            using var con = await DBContext.OpenConnection();
            using var cmd = new SqlCommand(@"
                SELECT m.MedicineTime
                FROM dbo.ResidentMedicine rm
                INNER JOIN dbo.Medicine m ON m.MedicineID = rm.MedicineID
                WHERE rm.ResidentID = @ResidentID", con);
            cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
                result.Add(rd.GetDateTime(0));

            return result;
        }

        public async Task ReplacePaymentMethodsAsync(Guid residentId, IEnumerable<Guid> paymentMethodIds) {
            using var con = await DBContext.OpenConnection();
            using var tx = (SqlTransaction)await con.BeginTransactionAsync();
            try {
                using (var del = new SqlCommand(
                    "DELETE FROM dbo.ResidentPaymentMethod WHERE ResidentID = @ResidentID", con, tx)) {
                    del.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                    await del.ExecuteNonQueryAsync();
                }

                foreach (var pmId in paymentMethodIds.Distinct()) {
                    using var ins = new SqlCommand(@"
                    INSERT INTO dbo.ResidentPaymentMethod (ResidentID, PaymentMethodID)
                    VALUES (@ResidentID, @PaymentMethodID)", con, tx);
                    ins.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                    ins.Parameters.Add("@PaymentMethodID", SqlDbType.UniqueIdentifier).Value = pmId;
                    await ins.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            } catch {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task ReplaceMedicineTimesAsync(Guid residentId, IEnumerable<DateTime> medicineTimes) {
            using var con = await DBContext.OpenConnection();
            using var tx = (SqlTransaction)await con.BeginTransactionAsync();
            try {
                using (var delMedicine = new SqlCommand(@"
                DELETE m
                FROM dbo.Medicine m
                INNER JOIN dbo.ResidentMedicine rm ON rm.MedicineID = m.MedicineID
                WHERE rm.ResidentID = @ResidentID", con, tx)) {
                    delMedicine.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                    await delMedicine.ExecuteNonQueryAsync();
                }

                using (var delJoin = new SqlCommand(
                    "DELETE FROM dbo.ResidentMedicine WHERE ResidentID = @ResidentID", con, tx)) {
                    delJoin.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                    await delJoin.ExecuteNonQueryAsync();
                }

                foreach (var t in medicineTimes.Distinct()) {
                    var medicineId = Guid.NewGuid();

                    using (var insM = new SqlCommand(@"
                    INSERT INTO dbo.Medicine (MedicineID, MedicineTime, MedicineGivenTime, MedicineRegisteredTime)
                    VALUES (@MedicineID, @MedicineTime, @MedicineGivenTime, @MedicineRegisteredTime)", con, tx)) {
                        insM.Parameters.Add("@MedicineID", SqlDbType.UniqueIdentifier).Value = medicineId;
                        insM.Parameters.Add("@MedicineTime", SqlDbType.DateTime).Value = t;
                        insM.Parameters.Add("@MedicineGivenTime", SqlDbType.DateTime).Value = t;
                        insM.Parameters.Add("@MedicineRegisteredTime", SqlDbType.DateTime).Value = t;
                        await insM.ExecuteNonQueryAsync();
                    }

                    using var insRM = new SqlCommand(@"
                    INSERT INTO dbo.ResidentMedicine (ResidentID, MedicineID)
                    VALUES (@ResidentID, @MedicineID)", con, tx);
                    insRM.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                    insRM.Parameters.Add("@MedicineID", SqlDbType.UniqueIdentifier).Value = medicineId;
                    await insRM.ExecuteNonQueryAsync();
                }

                await tx.CommitAsync();
            } catch {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteRelationsAsync(Guid residentId) {
            using var con = await DBContext.OpenConnection();

            using (var cmd = new SqlCommand("DELETE FROM dbo.ResidentPaymentMethod WHERE ResidentID=@ResidentID", con)) {
                cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                await cmd.ExecuteNonQueryAsync();
            }

            using (var cmd = new SqlCommand(@"
            DELETE m
            FROM dbo.Medicine m
            INNER JOIN dbo.ResidentMedicine rm ON rm.MedicineID = m.MedicineID
            WHERE rm.ResidentID = @ResidentID", con)) {
                cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                await cmd.ExecuteNonQueryAsync();
            }

            using (var cmd = new SqlCommand("DELETE FROM dbo.ResidentMedicine WHERE ResidentID=@ResidentID", con)) {
                cmd.Parameters.Add("@ResidentID", SqlDbType.UniqueIdentifier).Value = residentId;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private static string? TryGetString(IDataRecord r, string columnName) {
            try {
                var idx = r.GetOrdinal(columnName);
                return r.IsDBNull(idx) ? null : r.GetString(idx);
            } catch {
                return null;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using var con = await DBContext.OpenConnection();
            using var cmd = new SqlCommand(SqlDeleteById, con);
            cmd.CommandType = CommandType.StoredProcedure;

            BindId(cmd, id);

            await cmd.ExecuteNonQueryAsync();

            var existing = _items.FirstOrDefault(x => Equals(GetKey(x), id));
            if (existing != null)
                _items.Remove(existing);
        }
    }
}