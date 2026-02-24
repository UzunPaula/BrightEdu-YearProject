namespace BrightEdu.Application.DesignPatterns.FactoryMethod.Steps;

public interface ILessonStepCreatorResolver
{
    ILessonStepCreator Resolve(string stepType);
}