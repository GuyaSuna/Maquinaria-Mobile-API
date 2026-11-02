using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;

public class UploadthingService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _appId;

    public UploadthingService(
        HttpClient http,
        IConfiguration config)
    {
        _http = http;

        var token = config["UPLOADTHING_TOKEN"]!;

        var json = Encoding.UTF8.GetString(
            Convert.FromBase64String(token));

        var datos = JsonNode.Parse(json)!;

        _apiKey = datos["apiKey"]!.ToString();
        _appId = datos["appId"]!.ToString();
    }

    public async Task<(string Url, string Key)> UploadAsync(
        Stream archivo,
        string nombre,
        string tipo,
        long tamaño)
    {
        var datos = new
        {
            fileName = nombre,
            fileType = tipo,
            fileSize = tamaño,
            acl = "public-read"
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.uploadthing.com/v7/prepareUpload");

        request.Headers.Add("x-uploadthing-api-key", _apiKey);
        request.Content = JsonContent.Create(datos);

        using var respuesta = await _http.SendAsync(request);
        respuesta.EnsureSuccessStatusCode();

        var resultado = await respuesta.Content
            .ReadFromJsonAsync<JsonNode>();

        var urlSubida = resultado!["url"]!.ToString();

        using var formulario = new MultipartFormDataContent();
        using var contenido = new StreamContent(archivo);

        contenido.Headers.ContentType = new MediaTypeHeaderValue(tipo);
            
        formulario.Add(contenido, "file", nombre);

        using var subida = await _http.PutAsync(
            urlSubida, formulario);

        subida.EnsureSuccessStatusCode();

        var key = new Uri(urlSubida)
            .AbsolutePath.Trim('/').Split('/').Last();

        return ($"https://{_appId}.ufs.sh/f/{key}", key);
    }
}
