using System;
using System.Collections.Generic;
using System.Text;

namespace BasePlus.Common
{
    public enum OperationType
    {
        Insert = 1,
        Select,
        Update,
        Delete,
        SelectWithParameter,
        SelectWithJoin,
        SelectWithJoinAndParameter
    }

    public enum OrmType
    {
        Ado = 1,
        Dapper,
        EFCore,
        Contrib,
        GIS
    }

    public enum DbName
    {
        All = 0,
        MsSql = 1,
        Postgre
    }

    public enum TransactionType
    {
        YES = 1,
        NO,
        UNKNOWN
    }

    public enum DatabaseType
    {
        Sql = 1,
        Postgre
    }
}
