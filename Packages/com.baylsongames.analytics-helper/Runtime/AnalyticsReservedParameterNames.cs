using System.Collections.Generic;

namespace BaylsonGames.Analytics
{
    public static class AnalyticsReservedParameterNames
    {
        private static readonly HashSet<string> Names = new HashSet<string>(
            new[]
            {
                "eventDate",
                "eventID",
                "eventLevel",
                "eventStoreInsertedTimestamp",
                "eventTimestamp",
                "eventUUID",
                "id",
                "mainEventID",
                "msSinceLastEvent",
                "parentEventID",
                "platform",
                "sessionID",
                "timestamp",
                "transactionVector",
                "userID",
                "SELECT",
                "FROM",
                "WHERE",
                "GROUP",
                "ORDER",
                "TABLE",
                "USER",
                "DATE",
                "TIME",
                "TIMESTAMP",
                "VALUE"
            },
            System.StringComparer.OrdinalIgnoreCase);

        public static bool IsReserved(string parameterName)
        {
            return !string.IsNullOrWhiteSpace(parameterName) && Names.Contains(parameterName);
        }
    }
}
