using BasePlus.Common.API;
using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;

namespace BasePlus.BusinessContracts
{
    public interface IUserService
    {
        bool SetCurrentDb(string s);
        Task<RestResult<int>> TestAll(IEnumerable<BasePlus.Common.API.Object> users);
        bool TestEF(IEnumerable<BasePlus.Common.API.Object> users);
        Task<RestResult<IEnumerable<User>>> GetUsers();
        Stopwatch GetDapperWithContrib();
        Task<RestResult<IEnumerable<User>>> GetFromSqlWithDapper();
        Task<RestResult<IEnumerable<User>>> GetFromSqlWithAdo();
        Task<RestResult<IEnumerable<User>>> GetUsers2();
        Task<RestResult<IEnumerable<User>>> GetUsers3();
        Task<RestResult<IEnumerable<MockUserInfo>>> GetUsersWithInfo();
        Task<RestResult<IEnumerable<MockUserInfo>>> GetUsersWithInfoWithParam(string city, string jobTitle, decimal balanceMin, decimal balanceMax, bool active, string cardType, string gender, DateTime dateStart, DateTime dateEnd);
        Task<RestResult<object>> UpdateUsersByDate(DateTime dateStart, DateTime dateEnd, DateTime newDate);
        Task<RestResult<int>> InsertUsers(IEnumerable<BasePlus.Common.API.Object> users);
        Task<RestResult<int>> InsertUsersDapper(IEnumerable<BasePlus.Common.API.Object> users);
        Task<RestResult<int>> InsertUsersDapperToSql(IEnumerable<BasePlus.Common.API.Object> users);
        Task<RestResult<int>> InsertUsersDapperToSqlWithTransaction(IEnumerable<BasePlus.Common.API.Object> users);

        Task<RestResult<int>> InsertUsersToSql(IEnumerable<BasePlus.Common.API.Object> users);
        
        Task<RestResult<int>> InsertUsersToSqlWithTransaction(IEnumerable<BasePlus.Common.API.Object> users);       
        Task<RestResult<int>> DeleteUsers();
    }
}
