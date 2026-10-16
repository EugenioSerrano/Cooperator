using System;
using System.Data;
using System.Data.Common;
using Cooperator.Framework.Data.Casting;
using Cooperator.Framework.Data.Exceptions;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Tests;

/// <summary>
/// AC-2 y AC-3 del SPEC-261016-0020 §7 (NFR <i>No hidden casts</i>, ADR-005 §5): el <b>punto único de conversión</b>
/// valida lo que recibe y, si el objeto no es del proveedor, falla con el error <b>tipado</b> que nombra el tipo
/// recibido y el esperado — nunca un <c>InvalidCastException</c> a mitad de una operación.
/// <para>
/// Los casos positivos (transacciones y conexiones <b>reales</b> del proveedor) se ejercitan en la <b>lane de
/// integración</b>, que es donde hay una conexión de verdad; acá se ejercitan los <b>negativos por tipo</b> con
/// dobles que derivan de <see cref="DbTransaction"/>/<see cref="DbConnection"/> (§8.1).
/// </para>
/// </summary>
public sealed class TransactionAdapterTests
{
    private const string Origin = "prueba:TransactionAdapterTests";

    [Fact]
    public void A_null_transaction_is_allowed_and_returned_as_null()
    {
        // No toda operación tiene transacción: el adaptador lo refleja sin inventar nada.
        Assert.Null(TransactionAdapter.AsSqlTransaction(null, Origin));
    }

    [Fact]
    public void A_foreign_transaction_is_a_typed_error_with_the_received_and_expected_types()
    {
        ForeignTransaction foreign = new ForeignTransaction();

        InvalidTransactionTypeException exception = Assert.Throws<InvalidTransactionTypeException>(
            () => TransactionAdapter.AsSqlTransaction(foreign, Origin));

        Assert.Equal(typeof(ForeignTransaction).FullName, exception.ReceivedType);
        Assert.Equal(typeof(SqlTransaction).FullName, exception.ExpectedType);
        Assert.Equal(Origin, exception.Origin);
        Assert.Contains(typeof(ForeignTransaction).FullName!, exception.Message, StringComparison.Ordinal);
        Assert.Contains("Microsoft.Data.SqlClient", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_foreign_or_missing_connection_is_a_typed_error()
    {
        ForeignConnection foreign = new ForeignConnection();

        InvalidTransactionTypeException exception = Assert.Throws<InvalidTransactionTypeException>(
            () => TransactionAdapter.AsSqlConnection(foreign, Origin));
        Assert.Equal(typeof(ForeignConnection).FullName, exception.ReceivedType);
        Assert.Equal(typeof(SqlConnection).FullName, exception.ExpectedType);

        // Sin conexión también es un error tipado (el punto es el borde público, no un validador de null).
        Assert.Throws<InvalidTransactionTypeException>(() => TransactionAdapter.AsSqlConnection(null, Origin));
    }

    /// <summary>Doble de transacción: deriva del contrato BCL pero NO es del proveedor (el caso del SPEC §8.1).</summary>
    private sealed class ForeignTransaction : DbTransaction
    {
        public override IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }

        protected override DbConnection? DbConnection => null;
    }

    /// <summary>Doble de conexión: deriva del contrato BCL pero NO es del proveedor.</summary>
    private sealed class ForeignConnection : DbConnection
    {
        // El contrato BCL acepta null en el setter: se declara igual (el doble no lo usa).
        [System.Diagnostics.CodeAnalysis.AllowNull]
        public override string ConnectionString
        {
            get => string.Empty;
            set { }
        }

        public override string Database => string.Empty;

        public override string DataSource => string.Empty;

        public override string ServerVersion => string.Empty;

        public override System.Data.ConnectionState State => System.Data.ConnectionState.Closed;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => new ForeignTransaction();

        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();
    }
}
