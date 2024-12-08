using AibolitAPI.EmailManager.Templates;
using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager;

public class EmailTemplateFactory : IEmailTemplateFactory
{
    private readonly string _basePath;

    public EmailTemplateFactory(string basePath)
    {
        _basePath = basePath;
    }

    public IEmailTemplate GetTemplate(string templateName)
    {
        var filePath = Path.Combine(_basePath, $"{templateName}.html");

        return templateName switch
        {
            "registration" => new RegistrationSuccessTemplate(filePath),
            "appointment_cancellation" => new AppointmentCancellationTemplate(filePath),
            "appointment_confirmation" => new AppointmentConfirmationTemplate(filePath),
            "appointment_reminder" => new AppointmentReminderTemplate(filePath),
            "disease_search_info" => new DiseaseSearchInfoTemplate(filePath),
            "friendly_reminder" => new FriendlyReminderTemplate(filePath),
            "new_prescription" => new NewPrescriptionTemplate(filePath),
            "new_recommendation" => new NewRecommendationTemplate(filePath),
            _ => throw new ArgumentException("Template not found.")
        };
    }
}