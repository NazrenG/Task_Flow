namespace Task_Flow.WebAPI.Services.Quizzes
{
    /// <summary>
    /// Quiz-də istifadəçiyə təklif olunan seçimlər.
    /// </summary>
    public static class QuizOptions
    {
        public static readonly IReadOnlyList<string> Occupations = new List<string>
        {
            "IT (Programming, Systems)",
            "Design (Graphic, UI/UX)",
            "Human Resources",
            "Software Programming",
            "Backend Developer",
            "Frontend Developer",
            "Other (please specify)"
        };

        // Hal-hazırda heç bir endpoint tərəfindən istifadə olunmur
        public static readonly IReadOnlyList<string> Professions = new List<string>
        {
            "Programming",
            "Marketing",
            "Accounting",
            "Education",
            "Other (please specify)"
        };
    }
}
