using Microsoft.AspNetCore.OutputCaching;

namespace MedicalClinicAPI.Extensions;

public static class CacheServiceExtensions
{
    public static IServiceCollection AddCustomOutputCache(this IServiceCollection services)
    {
        // Output Cache Configuration
        services.AddOutputCache(options =>
        {
            // JWT base policy: Every user has a different cache 
            options.AddBasePolicy(builder => 
                builder.SetVaryByHeader("Authorization"));

            // Appointment policy: appointments_tag
            options.AddPolicy("AppointmentsPolicy", builder => 
                builder.Expire(TimeSpan.FromMinutes(10))
                       .Tag("appointments_tag"));

            // Patient policy: patients_tag
            options.AddPolicy("PatientsPolicy", builder => 
                builder.Expire(TimeSpan.FromMinutes(30)) 
                       .Tag("patients_tag"));

            // Doctor policy: doctors_tag
            options.AddPolicy("DoctorsPolicy", builder => 
                builder.Expire(TimeSpan.FromHours(1)) 
                       .Tag("doctors_tag"));

            // Doctor Schedule policy: schedules_tag
            options.AddPolicy("SchedulesPolicy", builder => 
                builder.Expire(TimeSpan.FromHours(1)) 
                       .Tag("schedules_tag"));

        });

        return services;
    }
}