using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Api_Maquinaria.Service;

    public class MercadoPagoService
    {
        private readonly HttpClient _http;
        private readonly string _accessToken;

        public MercadoPagoService(HttpClient http,IConfiguration config)
        {
            _http = http;

            _accessToken = config["MercadoPago_AccessToken"] ?? throw new Exception(
                "No se configuró el token de Mercado Pago."
            );
        }

        public async Task<string> CrearPagoAsync( string equipmentId, string equipmentName)
        {
            var monto = "850.00";

            var orden = new
            {
                type = "online",
                processing_mode = "manual",
                total_amount = monto,
                external_reference = $"revision-{equipmentId}-{Guid.NewGuid():N}",
                items = new[]
                {
                new
                {
                    title = $"Revisión de {equipmentName}",
                    quantity = 1,
                    unit_price = monto
                }
            }
            };

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.mercadopago.com/v1/orders"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer", _accessToken
                );

            request.Headers.Add(
                "X-Idempotency-Key",
                Guid.NewGuid().ToString()
            );

            request.Content = JsonContent.Create(orden);

            using var response = await _http.SendAsync(request);

            response.EnsureSuccessStatusCode();

            using var json = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync()
            );

            var url = json.RootElement
                .GetProperty("checkout_url")
                .GetString();

            return url ?? throw new Exception(
                "Mercado Pago no devolvió la URL del checkout."
            );
        }
    }
