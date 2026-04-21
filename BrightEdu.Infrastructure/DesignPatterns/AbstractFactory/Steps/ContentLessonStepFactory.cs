// using BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;
// using BrightEdu.Application.DTOs;
// using BrightEdu.Application.Features.Lessons.Mapper;
// using BrightEdu.Application.Interfaces;
// using BrightEdu.Domain.Entities;
// using BrightEdu.Infrastructure.DesignPatterns.FactoryMethod.Steps;
//
// namespace BrightEdu.Infrastructure.DesignPatterns.AbstractFactory.Steps;
//
// // Fabrica concretă pentru step-uri de tip "content".
// // Rol: decide dacă poate procesa tipul și creează ContentStep + mapper-ul potrivit.
// public class ContentLessonStepFactory : ILessonStepAbstractFactory
// {
//     // Tipul pe care îl suportă această fabrică.
//     private const string Type = "content";
//
//     // Creatorul face instanțierea efectivă a obiectului (logica de creare).
//     // Mapper-ul transformă entity -> DTO pentru response.
//     private readonly ContentStepCreator _creator;
//     private readonly ContentStepDtoMapper _mapper;
//
//     // Injectez dependențele prin constructor.
//     // Așa pot testa ușor și pot schimba implementări fără să modific clasa.
//     public ContentLessonStepFactory(ContentStepCreator creator, ContentStepDtoMapper mapper)
//     {
//         _creator = creator;
//         _mapper = mapper;
//     }
//
//     // Verific dacă această fabrică poate gestiona stepType primit din request.
//     // Compar case-insensitive ca să nu depind de litere mari/mici.
//     public bool CanHandle(string stepType)
//         => string.Equals(stepType, Type, StringComparison.OrdinalIgnoreCase);
//
//     // Creez step-ul concret pe baza datelor din request DTO.
//     // Nu fac new aici, dar deleg crearea către ContentStepCreator.
//     public LessonStep CreateStep(CreateLessonStepRequestDto request)
//         => _creator.Create(request);
//
//     // Returnez mapper-ul corect pentru acest tip de step.
//     // Clientul folosește mapper-ul prin interfață, fără să știe clasa concretă.
//     public ILessonStepDtoMapper CreateMapper()
//         => _mapper;
// }
//
