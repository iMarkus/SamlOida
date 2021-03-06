﻿using SamlOida.MessageHandler.Parser;
using SamlOida.Model;
using System.Xml;

namespace SamlOida.MessageHandler.MessageFactory
{
    public class AuthnRequestFactory : ISamlMessageFactory<SamlAuthnRequestMessage>
    {
        public XmlDocument CreateMessage(SamlOptions options, SamlAuthnRequestMessage authnRequestMessage)
        {
            var doc = new XmlDocument();

            var authnRequestElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "AuthnRequest", SamlAuthenticationDefaults.SamlProtocolNamespace);

            SamlXmlExtensions.PropagateStandardElements(authnRequestElement, authnRequestMessage);

            authnRequestElement.SetAttribute("AssertionConsumerServiceURL", authnRequestMessage.AssertionConsumerServiceUrl);

            doc.AppendChild(authnRequestElement);
            
            if (options.SignOutgoingMessages)
                SamlXmlExtensions.SignElement(authnRequestElement, options);

            return doc;
        }
    }
}
