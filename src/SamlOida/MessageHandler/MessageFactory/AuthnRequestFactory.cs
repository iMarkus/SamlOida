﻿using SamlOida.Model;
using System;
using System.Globalization;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SamlOida.MessageHandler.MessageFactory
{
    public class AuthnRequestFactory : ISamlMessageFactory<SamlAuthnRequestMessage>
    {
        public XmlDocument CreateMessage(SamlOptions options, SamlAuthnRequestMessage messageRequestMessage)
        {
            var doc = new XmlDocument();

            var authnRequestElement = doc.CreateElement(SamlAuthenticationDefaults.SamlProtocolNsPrefix, "AuthnRequest", SamlAuthenticationDefaults.SamlProtocolNamespace);

            //Set namespaces explicitly avoiding to include namespace declarations in child elements
            authnRequestElement.SetAttribute($"xmlns:{SamlAuthenticationDefaults.SamlAssertionNsPrefix}", SamlAuthenticationDefaults.SamlAssertionNamespace);
            authnRequestElement.SetAttribute($"xmlns:{SamlAuthenticationDefaults.SamlProtocolNsPrefix}", SamlAuthenticationDefaults.SamlProtocolNamespace);

            //TODO: Use something else than Guid.NewGuid ?
            authnRequestElement.SetAttribute("ID", $"Issuer_{Guid.NewGuid()}");
            authnRequestElement.SetAttribute("Version", "2.0");

            //Does the Standardportal differ from SAML-Standard?
            authnRequestElement.SetAttribute("IssueInstant", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));

            authnRequestElement.SetAttribute("Destination", messageRequestMessage.Destination);
            authnRequestElement.SetAttribute("AssertionConsumerServiceURL", messageRequestMessage.AssertionConsumerServiceUrl);

            var issuerElement = doc.CreateElement(SamlAuthenticationDefaults.SamlAssertionNsPrefix, "Issuer", SamlAuthenticationDefaults.SamlAssertionNamespace);
            issuerElement.InnerText = messageRequestMessage.Issuer;

            authnRequestElement.AppendChild(issuerElement);
            doc.AppendChild(authnRequestElement);

            //TODO: Extract this?
            if (options.SignRequest)
            {
                var signedXml = new SignedXml(doc)
                {
                    SigningKey = options.ServiceProviderCertificate.PrivateKey
                };
                var reference = new Reference("");
                var env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);

                signedXml.AddReference(reference);
                signedXml.ComputeSignature();
                var signature = signedXml.GetXml();

                doc.DocumentElement.AppendChild(doc.ImportNode(signature, true));
            }

            return doc;
        }
    }
}
