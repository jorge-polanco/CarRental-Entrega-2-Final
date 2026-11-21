using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly IMySqlDataService _dataService;

    public CustomerRepository(IMySqlDataService dataService) => _dataService = dataService;

    public IEnumerable<Customer> GetCustomers()
    {
        var result = _dataService.ExecSP("GetCustomers");
        return GetFirstTable(result)?.AsEnumerable().Select(MapCustomer).ToList() ?? [];
    }

    public Customer? GetCustomerById(int id)
    {
        var parameters = new[] { new MySqlParameter("pCustomerId", MySqlDbType.Int32) { Value = id } };
        var result = _dataService.ExecSP("GetCustomerById", parameters);
        var table = GetFirstTable(result);
        return table is { Rows.Count: > 0 } ? MapCustomer(table.Rows[0]) : null;
    }

    public bool InsertCustomer(Customer customer)
    {
        var result = _dataService.ExecSP("InsertCustomer", CreateParameters(customer));
        var table = GetFirstTable(result);
        if (table is not { Rows.Count: > 0 } || !table.Columns.Contains("newCustomerId")) return false;
        customer.IdCustomer = Convert.ToInt32(table.Rows[0]["newCustomerId"]);
        return customer.IdCustomer > 0;
    }

    public bool UpdateCustomer(Customer customer)
    {
        var parameters = CreateParameters(customer).ToList();
        parameters.Insert(0, new MySqlParameter("pCustomerId", MySqlDbType.Int32) { Value = customer.IdCustomer });
        return WasUpdated(_dataService.ExecSP("UpdateCustomer", parameters));
    }

    public bool DeleteCustomer(int id)
    {
        var parameters = new[] { new MySqlParameter("pCustomerId", MySqlDbType.Int32) { Value = id } };
        return WasUpdated(_dataService.ExecSP("DeleteCustomer", parameters));
    }

    private static IEnumerable<MySqlParameter> CreateParameters(Customer customer) => [
        new MySqlParameter("pFirstName", MySqlDbType.VarChar, 50) { Value = customer.FirstName },
        new MySqlParameter("pLastName", MySqlDbType.VarChar, 50) { Value = customer.LastName },
        new MySqlParameter("pSSN", MySqlDbType.VarChar, 50) { Value = customer.SocialSecurityNumber },
        new MySqlParameter("pEmail", MySqlDbType.VarChar, 254) { Value = customer.Email },
        new MySqlParameter("pCountryId", MySqlDbType.Int32) { Value = customer.CountryId },
        new MySqlParameter("pMobile", MySqlDbType.VarChar, 30) { Value = customer.MobilePhone },
        new MySqlParameter("pStateName", MySqlDbType.VarChar, 100) { Value = customer.StateName }
    ];

    private static bool WasUpdated(DataSet data) =>
        GetFirstTable(data) is { Rows.Count: > 0 } table &&
        table.Columns.Contains("affectedRows") && Convert.ToInt32(table.Rows[0]["affectedRows"]) > 0;

    private static DataTable? GetFirstTable(DataSet? data) =>
        data is { Tables.Count: > 0 } ? data.Tables[0] : null;

    private static Customer MapCustomer(DataRow row) => new()
    {
        IdCustomer = Convert.ToInt32(row["IdCustomer"]),
        FirstName = ReadString(row, "FirstName"),
        LastName = ReadString(row, "LastName"),
        SocialSecurityNumber = ReadString(row, "SSN"),
        Email = ReadString(row, "Email"),
        CountryName = ReadString(row, "Country"),
        CountryId = row.Table.Columns.Contains("CountryId") && row["CountryId"] != DBNull.Value
            ? Convert.ToInt32(row["CountryId"])
            : 0,
        MobilePhone = ReadString(row, "Mobile"),
        StateName = ReadString(row, "StateName")
    };

    private static string ReadString(DataRow row, string column) =>
        row.Table.Columns.Contains(column) && row[column] != DBNull.Value
            ? Convert.ToString(row[column]) ?? string.Empty
            : string.Empty;
}
