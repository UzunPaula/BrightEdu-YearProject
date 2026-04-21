// using BrightEdu.Application.DesignPatterns.Adapter.Steps;
// using BrightEdu.Application.DTOs;
// using BrightEdu.Domain.Entities;
//
// namespace BrightEdu.Infrastructure.DesignPatterns.Adapter.Steps;
//
// public class TemplateLessonAdapter : ITemplateLessonAdapter
// {
//     public LessonCreationModel Adapt(Lesson source)
//     {
//         // Creez modelul comun în care voi transforma lecția template.
//         var lessonCreationModel = new LessonCreationModel
//         {
//             Id = source.Id,
//             Title = source.Title,
//             Steps = new List<LessonCreationStepModel>()
//         };
//
//         // Parcurg toți pașii lecției template.
//         foreach (var step in source.Steps)
//         {
//             // Parcurg toți pașii lecției template.
//             var adaptedStep = new LessonCreationStepModel
//             {
//                 Id = step.Id,
//                 Type = step.Type,
//                 Order = step.Order
//             };
//
//             // Dacă step-ul este ContentStep, adaptez conținutul text.
//             if (step is ContentStep contentStep)
//             {
//                 adaptedStep.Content = contentStep.Content;
//             }
//             // Dacă step-ul este QuestionStep, adaptez datele întrebării.
//             else if (step is QuestionStep questionStep)
//             {
//                 adaptedStep.QuestionText = questionStep.QuestionText;
//                 adaptedStep.Options = questionStep.Options.ToList();
//                 adaptedStep.CorrectOptionIndex = questionStep.CorrectOptionIndex;
//             }
//             // Adaug step-ul adaptat în modelul final al lecției.
//             lessonCreationModel.Steps.Add(adaptedStep);
//         }
//         // Returnez lecția template în format comun.
//         return lessonCreationModel;
//     }
// }