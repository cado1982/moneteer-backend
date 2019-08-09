using System;
using System.Collections.Generic;
using System.Text;

namespace Moneteer.Domain
{
    public static class PostgresErrorCodes
    {
        public static string UniqueViolation = "23505";
        public static string ForeignKeyConstraintViolation = "23503";
    }
}
