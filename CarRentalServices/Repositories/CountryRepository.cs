using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System.Data;

namespace CarRentalServices.Repositories;

public sealed class CountryRepository : ICountryRepository
{
    private readonly IMySqlDataService _dataService;

    public CountryRepository(IMySqlDataService dataService) => _dataService = dataService;

    public IEnumerable<Country> GetCountries()
    {
        var result = _dataService.ExecSP("GetCountries");
        return GetFirstTable(result)?.AsEnumerable().Select(MapCountry).ToList() ?? [];
    }

    public Country? GetCountryById(int id)
    {
        var result = _dataService.ExecSP("GetCountryById", [
            new MySqlParameter("pCountryId", MySqlDbType.Int32) { Value = id }
        ]);
        var table = GetFirstTable(result);
        return table is { Rows.Count: > 0 } ? MapCountry(table.Rows[0]) : null;
    }

    public bool InsertCountry(Country country)
    {
        var result = _dataService.ExecSP("InsertCountry", CreateParameters(country));
        var table = GetFirstTable(result);
        if (table is not { Rows.Count: > 0 } || !table.Columns.Contains("newCountryId")) return false;
        country.CountryId = Convert.ToInt32(table.Rows[0]["newCountryId"]);
        return country.CountryId > 0;
    }

    public bool UpdateCountry(Country country)
    {
        var parameters = CreateParameters(country).ToList();
        parameters.Insert(0, new MySqlParameter("pCountryId", MySqlDbType.Int32) { Value = country.CountryId });
        return WasUpdated(_dataService.ExecSP("UpdateCountry", parameters));
    }

    private static IEnumerable<MySqlParameter> CreateParameters(Country country) => [
        new MySqlParameter("pIso2Code", MySqlDbType.VarChar, 2) { Value = country.Iso2Code },
        new MySqlParameter("pIso3Code", MySqlDbType.VarChar, 3) { Value = country.Iso3Code },
        new MySqlParameter("pCountryName", MySqlDbType.VarChar, 100) { Value = country.CountryName }
    ];

    private static bool WasUpdated(DataSet data) =>
        GetFirstTable(data) is { Rows.Count: > 0 } table &&
        table.Columns.Contains("affectedRows") && Convert.ToInt32(table.Rows[0]["affectedRows"]) > 0;

    private static DataTable? GetFirstTable(DataSet? data) =>
        data is { Tables.Count: > 0 } ? data.Tables[0] : null;

    private static Country MapCountry(DataRow row) => new()
    {
        CountryId = Convert.ToInt32(row["CountryId"]),
        Iso2Code = Convert.ToString(row["Iso2Code"]) ?? string.Empty,
        Iso3Code = Convert.ToString(row["Iso3Code"]) ?? string.Empty,
        CountryName = Convert.ToString(row["CountryName"]) ?? string.Empty
    };
}
