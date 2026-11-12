namespace AiPmaPlatform.Application.Common.Constants
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string PMO = "PMO";
        public const string PM = "PM";
        public const string Developer = "Developer";
        public const string QA = "QA";
        public const string Finance = "Finance";
        public const string Viewer = "Viewer";

        public static readonly string[] All = { Admin, PMO, PM, Developer, QA, Finance, Viewer };
    }
}