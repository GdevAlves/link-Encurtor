using System.Text.Json;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using URLapi.Application.AI.Tools;
using UrlShortener.Domain.IRepositories;

namespace URLapi.Application.Agents;

public class UrlAnalyticsAgent
{
    private readonly AIAgent _agent;
    private readonly UrlAnalyticsPlugin _plugin;

    public UrlAnalyticsAgent(IUrlAccessRepository urlAccessRepository, string apiKey)
    {
        _plugin = new UrlAnalyticsPlugin(urlAccessRepository);

        IChatClient client = new GenerativeAIChatClient(apiKey);

        _agent = new ChatClientAgent(client,
            """
            Você é um assistente de análise de URLs.
            Você tem acesso a dados de acessos de links encurtados.
            Analise os dados e forneça insights acionáveis.
            Sempre cite números específicos e períodos.
            """,
            tools:
            [
                AIFunctionFactory.Create(_plugin.GetTopUrls),
                AIFunctionFactory.Create(_plugin.GetAccessPattern)
                // AIFunctionFactory.Create(_plugin.GetGeographicDistribution),
                // AIFunctionFactory.Create(_plugin.GetTrendingUrls)
            ]
        );
    }

    public async Task<object> GetInsightsAsync(Guid userId, string question, JsonElement? currentState, CancellationToken cancellationToken)
    {
        // Passa o userId no prompt para o modelo ter contexto ao chamar tools.
        // (AIAgent/tools não recebem parâmetros externos automaticamente.)
        var prompt = $"UserId: {userId}\nPergunta: {question}";

        AgentSession session;
        if (currentState == null)
        {
            session = await _agent.GetNewSessionAsync(cancellationToken);
        }
        else
        {
            session = await _agent.DeserializeSessionAsync(currentState.Value, cancellationToken: cancellationToken);
        }

        var response = await _agent.RunAsync(prompt, session, cancellationToken: cancellationToken);
        
        return new 
        {
            Answer = response.ToString(),
            Session = session.Serialize()
        };
    }
}