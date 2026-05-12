namespace BrightEdu.Application.DesignPatterns.AbstractFactory.Steps;

// Rezolvatorul selectează fabrica potrivită pentru tipul de lecție cerut.
public interface ILessonComponentFactoryResolver
{
    ILessonComponentFactory Resolve(string factoryType);
}
