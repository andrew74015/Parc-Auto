using Microsoft.EntityFrameworkCore.Query;

namespace Database.ScalarMappedFunctions
{
    // CHARINDEX (Transact-SQL)
    public static class CharIndex
    {
        public static int Udf(string expressionToFind, [NotParameterized] string expressionToSearch)
        {
            throw new NotSupportedException();
        }
    }
}
