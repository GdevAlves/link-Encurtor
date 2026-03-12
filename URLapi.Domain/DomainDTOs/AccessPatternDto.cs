namespace URLapi.Domain.DomainDTOs;

public class AccessPatternDto
{
    /// <summary>
    /// Distribuição de acessos por hora do dia (0-23).
    /// </summary>
    public Dictionary<int, int> HourlyDistribution { get; init; } = new();

    /// <summary>
    /// Tendência diária de acessos (por data UTC: yyyy-MM-dd).
    /// </summary>
    public Dictionary<DateTime, int> DailyTrend { get; init; } = new();

    /// <summary>
    /// Hora (0-23) com mais acessos no período.
    /// </summary>
    public int PeakHour { get; init; }

    /// <summary>
    /// Total de acessos no período.
    /// </summary>
    public int TotalAccesses { get; init; }
}