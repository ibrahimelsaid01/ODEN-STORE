using System.Globalization;
using System.Threading.RateLimiting;

namespace StoreOde.Extensions;

public static class RateLimitingExtensions
{
    public const string IdentityAuthenticationPolicy =
        "IdentityAuthentication";

    public const string IdentityEmailPolicy =
        "IdentityEmail";

    public const string IdentityPasswordResetPolicy =
        "IdentityPasswordReset";

    public const string ContactSubmissionPolicy =
        "ContactSubmission";

    public const string ReviewSubmissionPolicy =
        "ReviewSubmission";

    public static IServiceCollection AddStoreOdeRateLimiting(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            options.OnRejected =
                async (context, cancellationToken) =>
                {
                    var response =
                        context.HttpContext.Response;

                    response.StatusCode =
                        StatusCodes.Status429TooManyRequests;

                    if (context.Lease.TryGetMetadata(
                            MetadataName.RetryAfter,
                            out var retryAfter))
                    {
                        var retryAfterSeconds =
                            Math.Max(
                                1,
                                (int)Math.Ceiling(
                                    retryAfter.TotalSeconds));

                        response.Headers.RetryAfter =
                            retryAfterSeconds.ToString(
                                CultureInfo.InvariantCulture);
                    }

                    await response.WriteAsync(
                        "Too many requests. Please try again later.",
                        cancellationToken);
                };

            options.AddPolicy<string>(
                IdentityAuthenticationPolicy,
                httpContext =>
                    CreatePostOnlyClientFixedWindowPartition(
                        httpContext,
                        permitLimit: 20,
                        window: TimeSpan.FromMinutes(1)));

            options.AddPolicy<string>(
                IdentityEmailPolicy,
                httpContext =>
                    CreatePostOnlyClientFixedWindowPartition(
                        httpContext,
                        permitLimit: 6,
                        window: TimeSpan.FromMinutes(10)));

            options.AddPolicy<string>(
                IdentityPasswordResetPolicy,
                httpContext =>
                    CreatePostOnlyClientFixedWindowPartition(
                        httpContext,
                        permitLimit: 10,
                        window: TimeSpan.FromMinutes(10)));

            options.AddPolicy<string>(
                ContactSubmissionPolicy,
                httpContext =>
                    CreatePostOnlyClientFixedWindowPartition(
                        httpContext,
                        permitLimit: 5,
                        window: TimeSpan.FromMinutes(10)));

            options.AddPolicy<string>(
                ReviewSubmissionPolicy,
                httpContext =>
                    CreatePostOnlyClientFixedWindowPartition(
                        httpContext,
                        permitLimit: 3,
                        window: TimeSpan.FromMinutes(10)));
        });

        return services;
    }

    private static RateLimitPartition<string>
        CreatePostOnlyClientFixedWindowPartition(
            HttpContext httpContext,
            int permitLimit,
            TimeSpan window)
    {
        if (!HttpMethods.IsPost(
                httpContext.Request.Method))
        {
            return RateLimitPartition.GetNoLimiter(
                "non-post-request");
        }

        return CreateClientFixedWindowPartition(
            httpContext,
            permitLimit,
            window);
    }

    private static RateLimitPartition<string>
        CreateClientFixedWindowPartition(
            HttpContext httpContext,
            int permitLimit,
            TimeSpan window)
    {
        var clientKey =
            httpContext.Connection
                .RemoteIpAddress?
                .ToString()
            ?? "unknown-client";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientKey,
            factory: _ =>
                new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = permitLimit,
                    Window = window,
                    QueueLimit = 0,
                    QueueProcessingOrder =
                        QueueProcessingOrder.OldestFirst
                });
    }
}