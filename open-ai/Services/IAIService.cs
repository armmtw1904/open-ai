using open_ai.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace open_ai.Services
{
    public interface IAIService
    {
        Task<ClassifyLanguagesResult> ClassifyLanguagesAsync(string message_input);
    }
}
