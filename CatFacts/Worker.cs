using System.Text.Json.Nodes;

namespace CatFacts
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient? _httpClient;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _httpClient = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _httpClient!.Dispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _httpClient!.GetAsync("https://meowfacts.herokuapp.com");
                if (response.IsSuccessStatusCode)
                {
                    var node = JsonNode.Parse(await response.Content.ReadAsStringAsync());
                    _logger.LogInformation(node!["data"]![0]!.ToString());
                }
                else
                {
                    _logger.LogInformation("Request failed. Status code {StatusCode}", response.StatusCode);
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}