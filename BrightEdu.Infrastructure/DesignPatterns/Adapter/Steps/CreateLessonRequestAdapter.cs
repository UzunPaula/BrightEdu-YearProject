// using BrightEdu.Application.DesignPatterns.Adapter.Steps;
// using BrightEdu.Application.DTOs;
//
// namespace BrightEdu.Infrastructure.DesignPatterns.Adapter.Steps;
//
// public class CreateLessonRequestAdapter : ILessonCreationAdapter
// {
//     public LessonCreationModel Adapt(CreateLessonRequestDto source)
//     {
//         // Creez modelul comun în care voi transforma datele primite din request.
//         var lessonCreationModel = new LessonCreationModel
//         {
//             Id = source.Id,
//             Title = source.Title,
//             Steps = new List<LessonCreationStepModel>()
//         };
//
//         // Parcurg toți pașii primiți din request.
//         foreach (var step in source.Steps)
//         {
//             // Copiez datele de bază comune pentru orice tip de step.
//             var adaptedStep = new LessonCreationStepModel
//             {
//                 Id = step.Id,
//                 Type = step.Type,
//                 Order = step.Order
//             };
//
//             // Dacă step-ul este de tip content, adaptez doar conținutul text.
//             if (step.Type == "content")
//             {
//                 adaptedStep.Content = step.Content;
//             }
//             else if (step.Type == "question") // Dacă step-ul este de tip question, adaptez câmpurile specifice întrebării.
//             {
//                 adaptedStep.QuestionText = step.QuestionText;
//                 adaptedStep.Options = step.Options?.ToList();
//                 adaptedStep.CorrectOptionIndex = step.CorrectOptionIndex;
//             }
//             // Adaug step-ul deja adaptat în modelul final al lecției.
//             lessonCreationModel.Steps.Add(adaptedStep);
//         }
//         // Returnez lecția în format comun, ușor de folosit de aplicație.
//         return lessonCreationModel;
//     }
// }