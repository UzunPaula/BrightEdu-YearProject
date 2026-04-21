// using BrightEdu.Application.DesignPatterns.Composite.Steps;
//
// namespace BrightEdu.Infrastructure.DesignPatterns.Composite.Steps;
//
// // Reprezintă un pas simplu din lecție.
// public class LessonStepComponent : ILessonComponent
// {
//     public string Title { get; }
//
//     public LessonStepComponent(string title)
//     {
//         Title = title;
//     }
//
//     public void Display(int level = 0)
//     {
//         // Afișează pasul simplu din lecție.
//         Console.WriteLine(new string(' ', level * 2) + $"Step: {Title}");
//     }
// }