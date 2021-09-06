﻿using canonapi.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

public static class QueryHelper
{
    public static string qryMatchedResultCount = "SELECT SUM(tot.DR0) AS Kaggle_DR0, SUM(tot.DR0) AS Sushrut_DR0, SUM(tot.DR1) AS Kaggle_DR1, SUM(tot.DR1) AS Sushrut_DR1, SUM(tot.DR2) AS Kaggle_DR2, SUM(tot.DR2) AS Sushrut_DR2, SUM(tot.DR3) AS Kaggle_DR3, SUM(tot.DR3) AS Sushrut_DR3, SUM(tot.DR4) AS Kaggle_DR4, SUM(tot.DR4) AS Sushrut_DR4 FROM"
    + " ("
    + "SELECT COUNT(i.drlevel_kaggle) AS DR0, 0 AS DR1, 0 AS DR2, 0 AS DR3, 0 AS DR4 FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE i.drlevel_kaggle = i.drlevel_sushrut AND(i.drlevel_sushrut = 0 OR iu.drlevel_byuser = 0)"
    + " UNION ALL "
    + "SELECT 0 AS DR0, COUNT(i.drlevel_kaggle) AS DR1, 0 AS DR2, 0 AS DR3, 0 AS DR4 FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE i.drlevel_kaggle = i.drlevel_sushrut AND(i.drlevel_sushrut = 1 OR iu.drlevel_byuser = 1)"
    + " UNION ALL "
    + "SELECT 0 AS DR0, 0 AS DR1, COUNT(i.drlevel_kaggle) AS DR2, 0 AS DR3, 0 AS DR4 FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE i.drlevel_kaggle = i.drlevel_sushrut AND(i.drlevel_sushrut = 2 OR iu.drlevel_byuser = 2)"
    + " UNION ALL "
    + "SELECT 0 AS DR0, 0 AS DR1, 0 AS DR2, COUNT(i.drlevel_kaggle) AS DR3, 0 AS DR4 FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE i.drlevel_kaggle = i.drlevel_sushrut AND(i.drlevel_sushrut = 3 OR iu.drlevel_byuser = 3)"
    + " UNION ALL "
    + "SELECT 0 AS DR0, 0 AS DR1, 0 AS DR2, 0 AS DR3, COUNT(i.drlevel_kaggle) AS DR4 FROM images i LEFT OUTER JOIN imagedrbyusers iu ON i.imagename = iu.imagename WHERE i.drlevel_kaggle = i.drlevel_sushrut AND(i.drlevel_sushrut = 4 OR iu.drlevel_byuser = 4)"
    + ") AS tot;";

    public static string qryUnmatchedResultCount = "SELECT SUM(countdata.Kaggle_DR0) AS Kaggle_DR0, SUM(countdata.Sushrut_DR0) AS Sushrut_DR0, SUM(countdata.Kaggle_DR1) AS Kaggle_DR1, SUM(countdata.Sushrut_DR1) AS Sushrut_DR1, SUM(countdata.Kaggle_DR2) AS Kaggle_DR2, SUM(countdata.Sushrut_DR2) AS Sushrut_DR2, SUM(countdata.Kaggle_DR3) AS Kaggle_DR3, SUM(countdata.Sushrut_DR3) AS Sushrut_DR3, SUM(countdata.Kaggle_DR4) AS Kaggle_DR4, SUM(countdata.Sushrut_DR4) AS Sushrut_DR4 FROM"
    + " ("
    + "SELECT SUM(DR0.Kaggle_DR0) AS Kaggle_DR0, SUM(DR0.Sushrut_DR0) AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM"
    + " ("
    + "SELECT COUNT(drlevel_kaggle) AS Kaggle_DR0, 0 AS Sushrut_DR0 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_kaggle = 0"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR0, COUNT(drlevel_sushrut) AS Sushrut_DR0 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_sushrut = 0"
    + ") AS DR0"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, SUM(DR1.Kaggle_DR1) AS Kaggle_DR1, SUM(DR1.Sushrut_DR1) AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM"
    + " ("
    + "SELECT COUNT(drlevel_kaggle) AS Kaggle_DR1, 0 AS Sushrut_DR1 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_kaggle = 1"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR1, COUNT(drlevel_sushrut) AS Sushrut_DR1 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_sushrut = 1"
    + ") AS DR1"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, SUM(DR2.Kaggle_DR2) AS Kaggle_DR2, SUM(DR2.Sushrut_DR2) AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM"
    + " ("
    + "SELECT COUNT(drlevel_kaggle) AS Kaggle_DR2, 0 AS Sushrut_DR2 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_kaggle = 2"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR2, COUNT(drlevel_sushrut) AS Sushrut_DR2 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_sushrut = 2"
    + ") AS DR2"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, SUM(DR3.Kaggle_DR3) AS Kaggle_DR3, SUM(DR3.Sushrut_DR3) AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM"
    + " ("
    + "SELECT COUNT(drlevel_kaggle) AS Kaggle_DR3, 0 AS Sushrut_DR3 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_kaggle = 3"

    + " UNION ALL "

    + " SELECT 0 AS Kaggle_DR3, COUNT(drlevel_sushrut) AS Sushrut_DR3 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_sushrut = 3"
    + ") AS DR3"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, SUM(DR4.Kaggle_DR4) AS Kaggle_DR4, SUM(DR4.Sushrut_DR4) AS Sushrut_DR4 FROM"
    + " ("
    + " SELECT COUNT(drlevel_kaggle) AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_kaggle = 4"

    + " UNION ALL "

    + "SELECT 0 AS Kaggle_DR4, COUNT(drlevel_sushrut) AS Sushrut_DR4 FROM images WHERE drlevel_kaggle <> drlevel_sushrut AND drlevel_sushrut = 4"
    + ") AS DR4"
    + ") AS countdata;";

    public static string qryAllResultCount = "SELECT SUM(tot.Kaggle_DR0) AS Kaggle_DR0, SUM(tot.Sushrut_DR0) AS Sushrut_DR0, SUM(tot.Kaggle_DR1) AS Kaggle_DR1, SUM(tot.Sushrut_DR1) AS Sushrut_DR1, SUM(tot.Kaggle_DR2) AS Kaggle_DR2, SUM(tot.Sushrut_DR2) AS Sushrut_DR2, SUM(tot.Kaggle_DR3) AS Kaggle_DR3, SUM(tot.Sushrut_DR3) AS Sushrut_DR3, SUM(tot.Kaggle_DR4) AS Kaggle_DR4, SUM(tot.Sushrut_DR4) AS Sushrut_DR4 FROM"
+ " ("
+ "SELECT COUNT(drlevel_kaggle) AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_kaggle = 0"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, COUNT(drlevel_sushrut) AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_sushrut = 0"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, COUNT(drlevel_kaggle) AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_kaggle = 1"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, COUNT(drlevel_sushrut) AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_sushrut = 1"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, COUNT(drlevel_kaggle) AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_kaggle = 2"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, COUNT(drlevel_sushrut) AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_sushrut = 2"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, COUNT(drlevel_kaggle) AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_kaggle = 3"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, COUNT(drlevel_sushrut) AS Sushrut_DR3, 0 AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_sushrut = 3"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, COUNT(drlevel_kaggle) AS Kaggle_DR4, 0 AS Sushrut_DR4 FROM images WHERE drlevel_kaggle = 4"
+ " UNION ALL "
+ "SELECT 0 AS Kaggle_DR0, 0 AS Sushrut_DR0, 0 AS Kaggle_DR1, 0 AS Sushrut_DR1, 0 AS Kaggle_DR2, 0 AS Sushrut_DR2, 0 AS Kaggle_DR3, 0 AS Sushrut_DR3, 0 AS Kaggle_DR4, COUNT(drlevel_sushrut) AS Sushrut_DR4 FROM images WHERE drlevel_sushrut = 4"
+ ") AS tot;";

    public static List<T> ExecuteQuery<T>(this ApplicationDbContext db, string query) where T : class, new()
    {
        using (var command = db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            db.Database.OpenConnection();

            using (var reader = command.ExecuteReader())
            {
                var lst = new List<T>();
                var lstColumns = new T().GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
                while (reader.Read())
                {
                    var newObject = new T();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        PropertyInfo prop = lstColumns.FirstOrDefault(a => a.Name.ToLower().Equals(name.ToLower()));
                        if (prop == null)
                        {
                            continue;
                        }
                        var val = reader.IsDBNull(i) ? null : reader[i];
                        prop.SetValue(newObject, val, null);
                    }
                    lst.Add(newObject);
                }

                return lst;
            }
        }
    }
}