using System.Text;
using BrightEdu.Application.DesignPatterns.Composite.Steps;
using BrightEdu.Application.Interfaces;

namespace BrightEdu.Infrastructure.DesignPatterns.Composite.Steps;

public class BuildLessonCompositeService : IBuildLessonCompositeService
{
    public string BuildSampleLessonStructure()
    {
        var output = new StringBuilder();

        // Componenta principală, adică lecția întreagă.
        var rootSection = new LessonSectionComponent("Lecția: Fracții");

        // Creez prima secțiune a lecției și adaug pași simpli în ea.
        var introSection = new LessonSectionComponent("Secțiunea: Introducere");
        introSection.Add(new LessonStepComponent("ContentStep: Ce este o fracție"));
        introSection.Add(new LessonStepComponent("QuestionStep: Care este numărătorul?"));

        // Creez a doua secțiune a lecției și adaug alți pași simpli.
        var practiceSection = new LessonSectionComponent("Secțiunea: Exerciții");
        practiceSection.Add(new LessonStepComponent("QuestionStep: 1/2 este mai mare decât 1/4?"));
        practiceSection.Add(new LessonStepComponent("QuestionStep: Alege fracția corectă"));

        // Adaug secțiunile în lecția principală.
        rootSection.Add(introSection);
        rootSection.Add(practiceSection);

        // Parcurg recursiv toată structura și o scriu sub formă de text.
        WriteComponent(rootSection, output, 0);

        return output.ToString();
    }

    private void WriteComponent(ILessonComponent component, StringBuilder output, int level)
    {
        // Parcurg recursiv toată structura și o scriu sub formă de text.
        output.AppendLine(new string(' ', level * 2) + component.Title);

        // Dacă este secțiune, parcurg și afișez toate componentele din interior.
        if (component is LessonSectionComponent section)
        {
            foreach (var child in section.GetChildren())
            {
                WriteComponent(child, output, level + 1);
            }
        }
    }
}