using AibolitAPI.EmailManager.Templates;
using AibolitAPI.Interfaces;

namespace AibolitAPI.EmailManager;

public class EmailTemplateFactory : IEmailTemplateFactory
{
    public IEmailTemplate GetTemplate(string type)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentNullException(nameof(type), "Template type cannot be null or empty");

        return type.ToLower() switch
        {
            "confirmation" => new AppointmentConfirmationTemplate(),
            "reminder" => new AppointmentReminderTemplate(),
            "cancellation" => new AppointmentCancellationTemplate(),
            "recommendation" => new NewRecommendationTemplate(),
            "prescription" => new NewPrescriptionTemplate(),
            "registration" => new RegistrationSuccessTemplate(),
            "diseaseinfo" => new DiseaseSearchInfoTemplate(),
            "friendly" => new FriendlyReminderTemplate(),
            _ => throw new ArgumentException("Invalid email type.")
        };
    }
}