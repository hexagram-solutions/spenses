using Microsoft.AspNetCore.Components.Forms;

namespace Spenses.App.Infrastructure;

public static class EditFormExtensions
{
    public static void AddValidationErrors(this EditForm form, Dictionary<string, string[]> errors)
    {
        var editContext = form.EditContext ??
            throw new InvalidOperationException($"No {nameof(EditContext)} is set on the current form.");

        var messageStore = new ValidationMessageStore(editContext);

        foreach (var error in errors)
            messageStore.Add(editContext.Field(error.Key), error.Value);

        editContext.NotifyValidationStateChanged();
    }
}
