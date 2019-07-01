using LandonApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Filters
{
    //Filter je funkcionalita ktora bezi pred/po tom, ako ASP.NET Core spracuje request
    public class JsonExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;

        public JsonExceptionFilter(IHostingEnvironment env)
        {
            _env = env;
        }

        //tento priklad filtra moze zabezpecit zobrazovanie JSON error hlasky ak je vysledkom volania API exception, bez filtra je vrateny HTML markup pre stranku s chybovou hlaskou
        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();

            if (_env.IsDevelopment()) //pre moznost urcenia ake udaje sa maju zalogovat je mozne overit prostredie na kt aktualne bezi app
            {
                error.Message = context.Exception.Message;
                error.Detail = context.Exception.StackTrace;
            }
            else
            {
                error.Message = "A server error occured.";
                error.Detail = context.Exception.Message;
            }            

            context.Result = new ObjectResult(error)
            {
                StatusCode = 500
            };
        }
    }
}
