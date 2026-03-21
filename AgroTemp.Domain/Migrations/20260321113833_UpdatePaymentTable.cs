using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroTemp.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM information_schema.tables
        WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments'
    ) THEN
        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'Id'
        ) AND NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'id'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" RENAME COLUMN ""Id"" TO id';
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'Amount'
        ) AND NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'amount'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" RENAME COLUMN ""Amount"" TO amount';
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'CreatedAt'
        ) AND NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'create_at'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" RENAME COLUMN ""CreatedAt"" TO create_at';
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'PaidAt'
        ) AND NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'paid_at'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" RENAME COLUMN ""PaidAt"" TO paid_at';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'wallet_id'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN wallet_id uuid';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'order_code'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN order_code bigint';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'paymnetLink_id'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN ""paymnetLink_id"" text';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'checkout_id'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN checkout_id text';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'status'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN status text';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'description'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN description text';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'create_at'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN create_at timestamp with time zone';
        END IF;

        IF NOT EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'paid_at'
        ) THEN
            EXECUTE 'ALTER TABLE ""AgroTempV1"".""Payments"" ADD COLUMN paid_at timestamp with time zone';
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'UserId'
        ) THEN
            IF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'user_id'
            ) AND EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'id'
            ) THEN
                EXECUTE '
                    UPDATE ""AgroTempV1"".""Payments"" p
                    SET wallet_id = w.id
                    FROM ""AgroTempV1"".""Wallets"" w
                    WHERE p.wallet_id IS NULL AND w.user_id = p.""UserId""';
            ELSIF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'user_id'
            ) AND EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'Id'
            ) THEN
                EXECUTE '
                    UPDATE ""AgroTempV1"".""Payments"" p
                    SET wallet_id = w.""Id""
                    FROM ""AgroTempV1"".""Wallets"" w
                    WHERE p.wallet_id IS NULL AND w.user_id = p.""UserId""';
            ELSIF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'UserId'
            ) AND EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'id'
            ) THEN
                EXECUTE '
                    UPDATE ""AgroTempV1"".""Payments"" p
                    SET wallet_id = w.id
                    FROM ""AgroTempV1"".""Wallets"" w
                    WHERE p.wallet_id IS NULL AND w.""UserId"" = p.""UserId""';
            ELSIF EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'UserId'
            ) AND EXISTS (
                SELECT 1 FROM information_schema.columns
                WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'Id'
            ) THEN
                EXECUTE '
                    UPDATE ""AgroTempV1"".""Payments"" p
                    SET wallet_id = w.""Id""
                    FROM ""AgroTempV1"".""Wallets"" w
                    WHERE p.wallet_id IS NULL AND w.""UserId"" = p.""UserId""';
            END IF;
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'VnPayTxnRef'
        ) THEN
            EXECUTE '
                UPDATE ""AgroTempV1"".""Payments""
                SET ""paymnetLink_id"" = COALESCE(""paymnetLink_id"", ""VnPayTxnRef"")
                WHERE ""paymnetLink_id"" IS NULL';
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'VnPayResponseCode'
        ) THEN
            EXECUTE '
                UPDATE ""AgroTempV1"".""Payments""
                SET description = COALESCE(description, ""VnPayResponseCode"")
                WHERE description IS NULL';
        END IF;

        IF EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Payments' AND column_name = 'Status'
        ) THEN
            EXECUTE '
                UPDATE ""AgroTempV1"".""Payments""
                SET status = COALESCE(
                    status,
                    CASE ""Status""
                        WHEN 0 THEN ''PENDING''
                        WHEN 1 THEN ''PAID''
                        WHEN 2 THEN ''CANCELLED''
                        ELSE ''UNKNOWN''
                    END)
                WHERE status IS NULL';
        END IF;

        EXECUTE '
            UPDATE ""AgroTempV1"".""Payments""
            SET create_at = COALESCE(create_at, NOW()),
                description = COALESCE(description, ''''),
                status = COALESCE(status, ''PENDING''),
                ""paymnetLink_id"" = COALESCE(""paymnetLink_id"", ''''),
                checkout_id = COALESCE(checkout_id, '''')';

        EXECUTE '
            ALTER TABLE ""AgroTempV1"".""Payments""
            ALTER COLUMN amount TYPE numeric(18,2)';

        EXECUTE '
            WITH numbered AS (
                SELECT id,
                       (EXTRACT(EPOCH FROM COALESCE(create_at, NOW()))::bigint * 1000)
                       + ROW_NUMBER() OVER (ORDER BY COALESCE(create_at, NOW()), id) AS generated_order_code
                FROM ""AgroTempV1"".""Payments""
                WHERE order_code IS NULL
            )
            UPDATE ""AgroTempV1"".""Payments"" p
            SET order_code = n.generated_order_code
            FROM numbered n
            WHERE p.id = n.id';

        IF NOT EXISTS (
            SELECT 1
            FROM pg_indexes
            WHERE schemaname = 'AgroTempV1' AND tablename = 'Payments' AND indexname = 'IX_Payments_WalletId'
        ) THEN
            EXECUTE 'CREATE INDEX ""IX_Payments_WalletId"" ON ""AgroTempV1"".""Payments"" (wallet_id)';
        END IF;

        IF NOT EXISTS (
            SELECT 1
            FROM pg_constraint
            WHERE conname = 'FK_Payments_Wallets_wallet_id'
        ) AND NOT EXISTS (
            SELECT 1 FROM ""AgroTempV1"".""Payments"" WHERE wallet_id IS NULL
        ) AND EXISTS (
            SELECT 1 FROM information_schema.columns
            WHERE table_schema = 'AgroTempV1' AND table_name = 'Wallets' AND column_name = 'id'
        ) THEN
            EXECUTE '
                ALTER TABLE ""AgroTempV1"".""Payments""
                ADD CONSTRAINT ""FK_Payments_Wallets_wallet_id""
                FOREIGN KEY (wallet_id)
                REFERENCES ""AgroTempV1"".""Wallets"" (id)
                ON DELETE CASCADE
                NOT VALID';
        END IF;
    END IF;
END $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Irreversible reconciliation migration for an already drifted Payments table.
        }
    }
}
