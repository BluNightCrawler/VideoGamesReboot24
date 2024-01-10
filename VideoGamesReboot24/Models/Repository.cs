namespace VideoGamesReboot24.Models
{
    public class Repository
    {
        private static List<TestModel> responses = new List<TestModel>();
        public static IEnumerable<TestModel> Response => responses;

        public static void AddResponse(TestModel response)
        {
            responses.Add(response);
        }

    }
}
