using System.Collections.Generic;

namespace EDocSys.Application.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
                $"Permissions.{module}.Preview",
                $"Permissions.{module}.Print",
            };
        }

        public static class Dashboard
        {
            public const string View = "Permissions.Dashboard.View";
            public const string Create = "Permissions.Dashboard.Create";
            public const string Edit = "Permissions.Dashboard.Edit";
            public const string Delete = "Permissions.Dashboard.Delete";
            public const string Preview = "Permissions.Dashboard.Preview";
            public const string Print = "Permissions.Dashboard.Print";
        }

        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
            public const string Preview = "Permissions.Products.Preview";
            public const string Print = "Permissions.Products.Print";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Preview = "Permissions.Users.Preview";
            public const string Print = "Permissions.Users.Print";
        }

        public static class Brands
        {
            public const string View = "Permissions.Brands.View";
            public const string Create = "Permissions.Brands.Create";
            public const string Edit = "Permissions.Brands.Edit";
            public const string Delete = "Permissions.Brands.Delete";
            public const string Preview = "Permissions.Brands.Preview";
            public const string Print = "Permissions.Brands.Print";
        }

        public static class Departments
        {
            public const string View = "Permissions.Departments.View";
            public const string Create = "Permissions.Departments.Create";
            public const string Edit = "Permissions.Departments.Edit";
            public const string Delete = "Permissions.Departments.Delete";
            public const string Preview = "Permissions.Departments.Preview";
            public const string Print = "Permissions.Departments.Print";
        }

        public static class Companies
        {
            public const string View = "Permissions.Companies.View";
            public const string Create = "Permissions.Companies.Create";
            public const string Edit = "Permissions.Companies.Edit";
            public const string Delete = "Permissions.Companies.Delete";
            public const string Preview = "Permissions.Companies.Preview";
            public const string Print = "Permissions.Companies.Print";
        }

        public static class Procedures
        {
            public const string View = "Permissions.Procedures.View";
            public const string Create = "Permissions.Procedures.Create";
            public const string Edit = "Permissions.Procedures.Edit";
            public const string Delete = "Permissions.Procedures.Delete";
            public const string Preview = "Permissions.Procedures.Preview";
            public const string Print = "Permissions.Procedures.Print";
        }

        public static class SOPs
        {
            public const string View = "Permissions.SOPs.View";
            public const string Create = "Permissions.SOPs.Create";
            public const string Edit = "Permissions.SOPs.Edit";
            public const string Delete = "Permissions.SOPs.Delete";
            public const string Preview = "Permissions.SOPs.Preview";
            public const string Print = "Permissions.SOPs.Print";
        }

        public static class WIs
        {
            public const string View = "Permissions.WIs.View";
            public const string Create = "Permissions.WIs.Create";
            public const string Edit = "Permissions.WIs.Edit";
            public const string Delete = "Permissions.WIs.Delete";
            public const string Preview = "Permissions.WIs.Preview";
            public const string Print = "Permissions.WIs.Print";
        }
    }
}