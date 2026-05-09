using System.Diagnostics;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ServiFlow.Services
{
    public class WhatsAppService
    {
        private readonly string _sid;
        private readonly string _token;
        private readonly string _fromNumber;

        public WhatsAppService(IConfiguration config)
        {
            _sid = config["Twilio:AccountSid"] ?? "";
            _token = config["Twilio:AuthToken"] ?? "";
            _fromNumber = config["Twilio:FromNumber"] ?? "";
        }

        public void EnviarMensaje(string numero, string mensaje)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(numero))
                {
                    Debug.WriteLine("WhatsApp: número vacío.");
                    return;
                }

                var numeroDestino = FormatearNumeroWhatsApp(numero);

                Debug.WriteLine("====== INTENTO WHATSAPP ======");
                Debug.WriteLine("FROM: " + _fromNumber);
                Debug.WriteLine("TO: " + numeroDestino);
                Debug.WriteLine("MENSAJE: " + mensaje);

                TwilioClient.Init(_sid, _token);

                var message = MessageResource.Create(
                    from: new PhoneNumber(_fromNumber),
                    to: new PhoneNumber(numeroDestino),
                    body: mensaje
                );

                Debug.WriteLine("WhatsApp enviado. SID: " + message.Sid);
                Debug.WriteLine("Estado: " + message.Status);
            }
            catch (ApiException ex)
            {
                Debug.WriteLine("ERROR TWILIO API");
                Debug.WriteLine("Message: " + ex.Message);
                Debug.WriteLine("Code: " + ex.Code);
                Debug.WriteLine("Status: " + ex.Status);
                Debug.WriteLine("MoreInfo: " + ex.MoreInfo);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR GENERAL WHATSAPP");
                Debug.WriteLine(ex.ToString());
            }
        }

        private string FormatearNumeroWhatsApp(string numero)
        {
            numero = numero.Trim().Replace(" ", "").Replace("-", "");

            if (numero.StartsWith("whatsapp:"))
                return numero;

            if (!numero.StartsWith("+") && numero.StartsWith("3"))
                numero = "+57" + numero;

            return "whatsapp:" + numero;
        }
    }
}   