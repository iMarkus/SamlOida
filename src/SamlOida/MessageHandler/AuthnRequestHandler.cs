﻿using SamlOida.Binding;
using SamlOida.MessageHandler.MessageFactory;
using SamlOida.Model;

namespace SamlOida.MessageHandler
{
    public class AuthnRequestHandler : OutgoingSamlMessageHandler<SamlAuthnRequestMessage>
    {
        //TODO: HttpRedirectBinding isn't always sufficient
        public AuthnRequestHandler(AuthnRequestFactory messageFactory, HttpRedirectBindingHandler binding) : base(messageFactory, binding)
        {
        }
    }
}