using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCollab.Infra
{
    public static class ErrorControllerAction
    {
        public const string BadRequest = "BadRequest";
        public const string Forbidden = "Forbidden";
        public const string InternalServerError = "InternalServerError";
        public const string MethodNotAllowed = "MethodNotAllowed";
        public const string NotFound = "NotFound";
        public const string Unauthorized = "Unauthorized";
    }

    public static class ErrorControllerRoute
    {
        public const string GetBadRequest = "Error" + "GetBadRequest";
        public const string GetForbidden = "Error" + "GetForbidden";
        public const string GetInternalServerError = "Error" + "GetInternalServerError";
        public const string GetMethodNotAllowed = "Error" + "GetMethodNotAllowed";
        public const string GetNotFound = "Error" + "GetNotFound";
        public const string GetUnauthorized = "Error" + "Unauthorized";
    }
}