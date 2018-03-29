﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SamlOida.MessageHandler;
using System;
using System.Text;
using System.Xml;

namespace SamlOida.Binding
{
    public class HttpPostBindingHandler :  ISamlBindingStrategy
    {
        private readonly IOptions<SamlOptions> _optionsMonitor;

        public HttpPostBindingHandler(IOptions<SamlOptions> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        }

        public void SendMessage(HttpContext context, XmlDocument message, string relayState = null)
        {
            var encodedMessage = EncodingHelper.EncodeMessage(message);

            var builder = new StringBuilder();
            //TODO: Extract IdP-SignonUrl!
            builder.Append($"<html><body><form action='{_optionsMonitor.Value.IdentityProviderSignOnUrl}'>");

            builder.Append($"<input type='hidden' name='{SamlAuthenticationDefaults.SamlRequestKey}' value='{encodedMessage}'/>");

            if (!string.IsNullOrEmpty(relayState))
                builder.Append($"<input type='hidden' name='{SamlAuthenticationDefaults.RelayStateKey}' value='{relayState}'/>");

            builder.Append("<input type='submit' value='Send'/>");
            builder.Append("</form></body></html>");

            context.Response.ContentType = "text/html";
            context.Response.WriteAsync(builder.ToString());
        }

        public ExtractionResult ExtractMessage(HttpContext context)
        {
            //TODO: Could also be SAMLRequest!
            var encodedMessage = context.Request.Form[SamlAuthenticationDefaults.SamlResponseKey];

            var binaryMessage = Convert.FromBase64String(encodedMessage);
            return new ExtractionResult
            {
                Message = binaryMessage.ToXmlDocument(),
                RelayState = context.Request.Form[SamlAuthenticationDefaults.RelayStateKey]
            };
        }
    }
}