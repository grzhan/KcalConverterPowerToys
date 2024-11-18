using System;
using System.Collections.Generic;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.KcalConverter;

internal static class ErrorHandler
{
    internal static List<Result> OnError(string icon, string queryInput, string errorMessage, Exception exception = default)
    {
        string userMessage;

        if (errorMessage != default)
        {
            Log.Error($"Failed to calculate <{queryInput}>: {errorMessage}", typeof(KcalConverter.Main));
            userMessage = errorMessage;
        }
        else if (exception != default)
        {
            Log.Exception($"Exception when query for <{queryInput}>", exception, exception.GetType());
            userMessage = exception.Message;
        }
        else
        {
            throw new ArgumentException("The arguments error and exception have default values. One of them has to be filled with valid error data (error message/exception)!");
        }

        return new List<Result> { CreateErrorResult(userMessage, icon) };
    }
    
    private static Result CreateErrorResult(string errorMessage, string iconPath)
    {
        return new Result
        {
            Title = "Calculation failed",
            SubTitle = errorMessage,
            IcoPath = iconPath,
            Score = 300,
        };
    }
}