using System.Data;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Spenses.Resources.Relational.ComponentModel;

namespace Spenses.Resources.Relational.Infrastructure;

public static class ViewBuilder
{
    public static async Task UpdateViews(this DbContext db)
    {
        var connection = db.Database.GetDbConnection();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        var keylessTypes = db.Model.GetEntityTypes()
            .Where(et => !et.GetDeclaredKeys().Any())
            .Select(et => et.ClrType);

        foreach (var queryType in keylessTypes)
        {
            if (queryType.GetCustomAttribute<BaseTableAttribute>() is null)
                continue;

            var viewName = queryType.Name;

            var definition = BuildViewDefinition(queryType);

            var existing = connection.GetViewDefinition(viewName);

            if (string.IsNullOrEmpty(existing) ||
                !string.Equals(existing, definition, StringComparison.OrdinalIgnoreCase))
            {
                await db.SetViewDefinition(viewName, definition);
            }
        }
    }

    private static string? GetViewDefinition(this IDbConnection connection, string viewName)
    {
        using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;

        command.CommandText =
            "select [definition] from sys.sql_modules where [object_id] = object_id(@viewName, 'V')";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@viewName";
        parameter.Value = viewName;
        command.Parameters.Add(parameter);

        var result = command.ExecuteScalar();

        if (result is not string definition)
            return null;

        var index = definition.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase);

        return index < 0 ? null : definition[(index + 4)..];
    }

    private static async Task SetViewDefinition(this DbContext db, string viewName, string definition)
    {
        var sql = $"CREATE OR ALTER VIEW {EscapeIdentifier(viewName)} AS {definition}";

        await db.Database.ExecuteSqlRawAsync(sql);
    }

    public static string BuildViewDefinition(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var tableAliases = new HashSet<string>();

        var baseTable = type.GetCustomAttribute<BaseTableAttribute>()
            ?? throw new InvalidOperationException(
                $"The type {type.FullName} must have a {nameof(BaseTableAttribute)} to be used as a view.");

        if (string.IsNullOrEmpty(baseTable.TableName))
        {
            throw new InvalidOperationException(
                $"The {nameof(BaseTableAttribute)} on {type.FullName} must specify a " +
                $"{nameof(BaseTableAttribute.TableName)} to be used as a view.");
        }

        if (string.IsNullOrEmpty(baseTable.Alias))
        {
            throw new InvalidOperationException(
                $"The {nameof(BaseTableAttribute)} on {type.FullName} must specify an " +
                $"{nameof(BaseTableAttribute.Alias)} to be used as a view.");
        }

        tableAliases.Add(baseTable.Alias);

        foreach (var joinedTable in type.GetCustomAttributes<JoinedTableAttribute>())
        {
            if (string.IsNullOrEmpty(joinedTable.Alias))
            {
                throw new InvalidOperationException(
                    $"All {nameof(JoinedTableAttribute)}s on {type.FullName} must specify an {nameof(JoinedTableAttribute.Alias)} to be used as a view.");
            }

            if (!tableAliases.Add(joinedTable.Alias))
            {
                throw new InvalidOperationException(
                    $"The alias '{joinedTable.Alias}' is defined more than once on the {type.FullName}. All " +
                    "table attributes must specify a unique alias.");
            }

            if (string.IsNullOrEmpty(joinedTable.TableName))
            {
                throw new InvalidOperationException(
                    $"The {nameof(JoinedTableAttribute)} on {type.FullName} for the alias {joinedTable.Alias} " +
                    $"must specify a {nameof(JoinedTableAttribute.TableName)} to be used as a view.");
            }

            if (string.IsNullOrEmpty(joinedTable.Condition))
            {
                throw new InvalidOperationException(
                    $"The {nameof(JoinedTableAttribute)} on {type.FullName} for the alias {joinedTable.Alias} " +
                    "must specify a join condition.");
            }
        }

        var sql = new StringBuilder();

        sql.Append("SELECT ");

        var properties = type.GetProperties();

        if (properties.Length == 0)
        {
            throw new InvalidOperationException(
                $"There were no public properties found on the type {type.FullName}.");
        }

        foreach (var property in properties)
        {
            var sourceColumn = property.GetCustomAttribute<SourceColumnAttribute>();

            var calculatedColumn = property.GetCustomAttribute<CalculatedColumnAttribute>();

            if (sourceColumn != null && calculatedColumn != null)
            {
                throw new InvalidOperationException(
                    $"The property {property.Name} can not define both a {nameof(SourceColumnAttribute)} and a " +
                    $"{nameof(CalculatedColumnAttribute)}.");
            }

            if (sourceColumn != null)
            {
                var tableAlias = sourceColumn.TableAlias;

                var columnName = sourceColumn.ColumnName ?? property.Name;

                sql.Append(
                    $"{EscapeIdentifier(tableAlias)}.{EscapeIdentifier(columnName)} {EscapeIdentifier(property.Name)}, ");
            }
            else if (calculatedColumn != null)
            {
                if (string.IsNullOrWhiteSpace(calculatedColumn.Sql))
                {
                    throw new InvalidOperationException(
                        $"The {nameof(CalculatedColumnAttribute)} on {type.FullName}::{property.Name} must " +
                        "specify the SQL for calculating the column.");
                }

                sql.Append($"({calculatedColumn.Sql}) {EscapeIdentifier(property.Name)}, ");
            }
            else
            {
                sql.Append($"{EscapeIdentifier(baseTable.Alias)}.{EscapeIdentifier(property.Name)}, ");
            }
        }

        sql.Length -= 2; // Remove the last ", " at the end of the string

        sql.Append($" FROM {EscapeIdentifier(baseTable.TableName)} {EscapeIdentifier(baseTable.Alias)}");

        foreach (var appliedTable in type.GetCustomAttributes<ApplyFunction>())
        {
            var applyType = appliedTable.ApplyFunctionType.ToString().ToUpperInvariant();

            sql.Append($" {applyType} APPLY ({appliedTable.Function}) {appliedTable.Alias}");
        }

        foreach (var joinedTable in type.GetCustomAttributes<JoinedTableAttribute>())
        {
            var joinType = joinedTable.JoinType.ToString().ToUpperInvariant();

            sql.Append(
                $" {joinType} JOIN {EscapeIdentifier(joinedTable.TableName)} {EscapeIdentifier(joinedTable.Alias)} ON {joinedTable.Condition}");
        }

        return sql.ToString();
    }

    private static string EscapeIdentifier(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        if (identifier.Length == 0)
            throw new ArgumentException("The identifier can not be an empty string.", nameof(identifier));

        if (identifier.Length > 128)
            throw new ArgumentException("The identifier can not exceed 128 characters in length.", nameof(identifier));

        if (identifier.Contains('[') || identifier.Contains(']'))
            throw new ArgumentException($"The identifier '{identifier}' contains invalid characters.",
                nameof(identifier));

        return $"[{identifier}]";
    }
}
