using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BibliUteco.Services
{
    public class ToastMessage
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "info"; // success | error | info
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class ToastService
    {
        public event Action<ToastMessage>? OnShow;
        public event Action<Guid>? OnHide;

        public void ShowToast(string message, string title = "", string type = "info")
        {
            var toast = new ToastMessage { Title = title, Message = message, Type = type };
            OnShow?.Invoke(toast);

            // Programar ocultado automático sin usar System.Timers (más seguro en Blazor)
            _ = AutoHideAsync(toast.Id);
        }

        private async Task AutoHideAsync(Guid id)
        {
            try
            {
                await Task.Delay(5000);
                OnHide?.Invoke(id);
            }
            catch
            {
                // Ignorar errores de cancelación/race conditions
            }
        }

        public void HideToast(Guid id) => OnHide?.Invoke(id);
    }
}