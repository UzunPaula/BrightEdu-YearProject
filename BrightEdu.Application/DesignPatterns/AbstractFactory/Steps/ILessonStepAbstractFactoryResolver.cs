namespace BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;

public interface ILessonStepAbstractFactoryResolver
{
    ILessonStepAbstractFactory Resolve(string stepType);
}