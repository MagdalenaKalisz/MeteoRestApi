﻿// <auto-generated />
using System;
using Meteo.Persistence.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Meteo.Persistence.PostgreSql.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250315122513_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Meteo.Persistence.PostgreSql.Dao.OutboxMessageDao", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("OccurredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("ProcessedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("OccurredAt")
                        .HasDatabaseName("IX_OutboxMessages_OccurredAt");

                    b.ToTable("OutboxMessages");
                });

            modelBuilder.Entity("Meteo.Persistence.PostgreSql.Dao.WeatherForecastDao", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("DefinitionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("WeatherForecasts");
                });

            modelBuilder.Entity("Meteo.Persistence.PostgreSql.Dao.WeatherForecastDefinitionDao", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("Latitude", "Longitude")
                        .HasDatabaseName("IX_WeatherForecastDefinitions_LatLong");

                    b.ToTable("WeatherForecastDefinitions");
                });

            modelBuilder.Entity("Meteo.Persistence.PostgreSql.Dao.WeatherForecastForDayDao", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("ForecastDateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Humidity")
                        .HasColumnType("double precision");

                    b.Property<double>("Temperature")
                        .HasColumnType("double precision");

                    b.Property<Guid>("WeatherForecastId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WeatherForecastId");

                    b.ToTable("WeatherForecastForDays");
                });

            modelBuilder.Entity("Meteo.Persistence.PostgreSql.Dao.WeatherForecastForDayDao", b =>
                {
                    b.HasOne("Meteo.Persistence.PostgreSql.Dao.WeatherForecastDao", "WeatherForecast")
                        .WithMany("Forecasts")
                        .HasForeignKey("WeatherForecastId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WeatherForecast");
                });

            modelBuilder.Entity("Meteo.Persistence.PostgreSql.Dao.WeatherForecastDao", b =>
                {
                    b.Navigation("Forecasts");
                });
#pragma warning restore 612, 618
        }
    }
}
